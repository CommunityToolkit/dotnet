// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for <see cref="IncrementalValueProvider{TValues}"/>.
/// </summary>
internal static class IncrementalValueProviderExtensions
{
    /// <summary>
    /// Groups items in a given <see cref="IncrementalValueProvider{TValue}"/> sequence by a specified key.
    /// </summary>
    /// <typeparam name="TLeft">The type of left items in each tuple.</typeparam>
    /// <typeparam name="TRight">The type of right items in each tuple.</typeparam>
    /// <param name="source">The input <see cref="IncrementalValuesProvider{TValues}"/> instance.</param>
    /// <param name="comparer">A <typeparamref name="TLeft"/> comparer.</param>
    /// <returns>An <see cref="IncrementalValuesProvider{TValues}"/> with the grouped results.</returns>
    public static IncrementalValuesProvider<(TLeft Left, ImmutableArray<TRight> Right)> GroupBy<TLeft, TRight>(
        this IncrementalValueProvider<ImmutableArray<(TLeft Left, TRight Right)>> source,
        IEqualityComparer<TLeft> comparer)
    {
        return source.SelectMany((item, _) =>
        {
            Dictionary<TLeft, ImmutableArray<TRight>.Builder> map = new(comparer);

            foreach ((TLeft hierarchy, TRight info) in item)
            {
                if (!map.TryGetValue(hierarchy, out ImmutableArray<TRight>.Builder builder))
                {
                    builder = ImmutableArray.CreateBuilder<TRight>();

                    map.Add(hierarchy, builder);
                }

                builder.Add(info);
            }

            ImmutableArray<(TLeft Hierarchy, ImmutableArray<TRight> Properties)>.Builder result =
                ImmutableArray.CreateBuilder<(TLeft, ImmutableArray<TRight>)>();

            foreach (KeyValuePair<TLeft, ImmutableArray<TRight>.Builder> entry in map)
            {
                result.Add((entry.Key, entry.Value.ToImmutable()));
            }

            return result;
        });
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file is ported and adapted from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for <see cref="IncrementalValuesProvider{TValues}"/>.
/// </summary>
internal static class IncrementalValuesProviderExtensions
{
    /// <summary>
    /// Groups items in a given <see cref="IncrementalValuesProvider{TValue}"/> sequence by a specified key.
    /// </summary>
    /// <typeparam name="TLeft">The type of left items in each tuple.</typeparam>
    /// <typeparam name="TRight">The type of right items in each tuple.</typeparam>
    /// <param name="source">The input <see cref="IncrementalValuesProvider{TValues}"/> instance.</param>
    /// <param name="comparer">A <typeparamref name="TLeft"/> comparer.</param>
    /// <returns>An <see cref="IncrementalValuesProvider{TValues}"/> with the grouped results.</returns>
    public static IncrementalValuesProvider<(TLeft Left, ImmutableArray<TRight> Right)> GroupBy<TLeft, TRight>(
        this IncrementalValuesProvider<(TLeft Left, TRight Right)> source,
        IEqualityComparer<TLeft> comparer)
    {
        return source.Collect().SelectMany((item, _) =>
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

    /// <summary>
    /// Creates a new <see cref="IncrementalValuesProvider{TValues}"/> instance with a given pair of comparers.
    /// </summary>
    /// <typeparam name="TLeft">The type of left items in each tuple.</typeparam>
    /// <typeparam name="TRight">The type of right items in each tuple.</typeparam>
    /// <param name="source">The input <see cref="IncrementalValuesProvider{TValues}"/> instance.</param>
    /// <param name="comparerLeft">An <see cref="IEqualityComparer{T}"/> instance for <typeparamref name="TLeft"/> items.</param>
    /// <param name="comparerRight">An <see cref="IEqualityComparer{T}"/> instance for <typeparamref name="TRight"/> items.</param>
    /// <returns>An <see cref="IncrementalValuesProvider{TValues}"/> with the specified comparers applied to each item.</returns>
    public static IncrementalValuesProvider<(TLeft Left, TRight Right)> WithComparers<TLeft, TRight>(
        this IncrementalValuesProvider<(TLeft Left, TRight Right)> source,
        IEqualityComparer<TLeft> comparerLeft,
        IEqualityComparer<TRight> comparerRight)
    {
        return source.WithComparer(new Comparer<TLeft, TRight>(comparerLeft, comparerRight));
    }

    /// <summary>
    /// Creates a new <see cref="IncrementalValuesProvider{TValues}"/> instance with a given pair of comparers.
    /// </summary>
    /// <typeparam name="T1">The type of first items in each tuple.</typeparam>
    /// <typeparam name="T2">The type of second items in each tuple.</typeparam>
    /// <typeparam name="T3">The type of third items in each tuple.</typeparam>
    /// <param name="source">The input <see cref="IncrementalValuesProvider{TValues}"/> instance.</param>
    /// <param name="comparer1">An <see cref="IEqualityComparer{T}"/> instance for <typeparamref name="T1"/> items.</param>
    /// <param name="comparer2">An <see cref="IEqualityComparer{T}"/> instance for <typeparamref name="T2"/> items.</param>
    /// <param name="comparer3">An <see cref="IEqualityComparer{T}"/> instance for <typeparamref name="T3"/> items.</param>
    /// <returns>An <see cref="IncrementalValuesProvider{TValues}"/> with the specified comparers applied to each item.</returns>
    public static IncrementalValuesProvider<(T1, T2, T3)> WithComparers<T1, T2, T3>(
        this IncrementalValuesProvider<(T1, T2, T3)> source,
        IEqualityComparer<T1> comparer1,
        IEqualityComparer<T2> comparer2,
        IEqualityComparer<T3> comparer3)
    {
        return source.WithComparer(new Comparer<T1, T2, T3>(comparer1, comparer2, comparer3));
    }

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for a value tuple.
    /// </summary>
    private sealed class Comparer<TLeft, TRight> : IEqualityComparer<(TLeft Left, TRight Right)>
    {
        /// <summary>
        /// The <typeparamref name="TLeft"/> comparer.
        /// </summary>
        private readonly IEqualityComparer<TLeft> comparerLeft;

        /// <summary>
        /// The <typeparamref name="TRight"/> comparer.
        /// </summary>
        private readonly IEqualityComparer<TRight> comparerRight;

        /// <summary>
        /// Creates a new <see cref="Comparer{TLeft, TRight}"/> instance with the specified parameters.
        /// </summary>
        /// <param name="comparerLeft">The <typeparamref name="TLeft"/> comparer.</param>
        /// <param name="comparerRight">The <typeparamref name="TRight"/> comparer.</param>
        public Comparer(IEqualityComparer<TLeft> comparerLeft, IEqualityComparer<TRight> comparerRight)
        {
            this.comparerLeft = comparerLeft;
            this.comparerRight = comparerRight;
        }

        /// <inheritdoc/>
        public bool Equals((TLeft Left, TRight Right) x, (TLeft Left, TRight Right) y)
        {
            return
                this.comparerLeft.Equals(x.Left, y.Left) &&
                this.comparerRight.Equals(x.Right, y.Right);
        }

        /// <inheritdoc/>
        public int GetHashCode((TLeft Left, TRight Right) obj)
        {
            return HashCode.Combine(
                this.comparerLeft.GetHashCode(obj.Left),
                this.comparerRight.GetHashCode(obj.Right));
        }
    }

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for a value tuple.
    /// </summary>
    private sealed class Comparer<T1, T2, T3> : IEqualityComparer<(T1, T2, T3)>
    {
        /// <summary>
        /// The <typeparamref name="T1"/> comparer.
        /// </summary>
        private readonly IEqualityComparer<T1> comparer1;

        /// <summary>
        /// The <typeparamref name="T2"/> comparer.
        /// </summary>
        private readonly IEqualityComparer<T2> comparer2;

        /// <summary>
        /// The <typeparamref name="T3"/> comparer.
        /// </summary>
        private readonly IEqualityComparer<T3> comparer3;

        /// <summary>
        /// Creates a new <see cref="Comparer{T1, T2, T3}"/> instance with the specified parameters.
        /// </summary>
        /// <param name="comparer1">The <typeparamref name="T1"/> comparer.</param>
        /// <param name="comparer2">The <typeparamref name="T2"/> comparer.</param>
        /// <param name="comparer3">The <typeparamref name="T3"/> comparer.</param>
        public Comparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2, IEqualityComparer<T3> comparer3)
        {
            this.comparer1 = comparer1;
            this.comparer2 = comparer2;
            this.comparer3 = comparer3;
        }

        /// <inheritdoc/>
        public bool Equals((T1, T2, T3) x, (T1, T2, T3) y)
        {
            return
                this.comparer1.Equals(x.Item1, y.Item1) &&
                this.comparer2.Equals(x.Item2, y.Item2) &&
                this.comparer3.Equals(x.Item3, y.Item3);
        }

        /// <inheritdoc/>
        public int GetHashCode((T1, T2, T3) obj)
        {
            return HashCode.Combine(
                this.comparer1.GetHashCode(obj.Item1),
                this.comparer2.GetHashCode(obj.Item2),
                this.comparer3.GetHashCode(obj.Item3));
        }
    }
}

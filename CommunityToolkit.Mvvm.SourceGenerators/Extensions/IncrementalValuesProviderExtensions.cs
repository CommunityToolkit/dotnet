// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file is ported and adapted from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for <see cref="IncrementalValuesProvider{TValues}"/>.
/// </summary>
internal static class IncrementalValuesProviderExtensions
{
    /// <summary>
    /// Creates a new <see cref="IncrementalValuesProvider{TValues}"/> instance with a gven pair of comparers.
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
    /// An <see cref="IEqualityComparer{T}"/> implementation for a value tuple.
    /// </summary>
    public sealed class Comparer<TLeft, TRight> : IEqualityComparer<(TLeft Left, TRight Right)>
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
}

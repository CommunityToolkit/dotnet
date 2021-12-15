// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file is ported from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for <see cref="IEqualityComparer{T}"/>.
/// </summary>
internal static class IEqualityComparerExtensions
{
    /// <summary>
    /// Gets an <see cref="IEqualityComparer{T}"/> for an <see cref="ImmutableArray{T}"/> sequence.
    /// </summary>
    /// <typeparam name="T">The type of items to compare.</typeparam>
    /// <param name="comparer">The compare for individual <typeparamref name="T"/> items.</param>
    public static IEqualityComparer<ImmutableArray<T>> ForImmutableArray<T>(this IEqualityComparer<T> comparer)
    {
        return new Comparer<T>(comparer);
    }

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for an <see cref="ImmutableArray{T}"/> value.
    /// </summary>
    public sealed class Comparer<T> : IEqualityComparer<ImmutableArray<T>>
    {
        /// <summary>
        /// The <typeparamref name="T"/> comparer.
        /// </summary>
        private readonly IEqualityComparer<T> comparer;

        /// <summary>
        /// Creates a new <see cref="Comparer{T}"/> instance with the specified parameters.
        /// </summary>
        /// <param name="comparer">The <typeparamref name="T"/> comparer.</param>
        public Comparer(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        /// <inheritdoc/>
        public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y)
        {
            return
                x == y ||
                x.SequenceEqual(y, this.comparer);
        }

        /// <inheritdoc/>
        public int GetHashCode(ImmutableArray<T> obj)
        {
            HashCode hashCode = default;

            hashCode.AddRange(obj, this.comparer);

            return hashCode.ToHashCode();
        }
    }
}

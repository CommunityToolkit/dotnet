// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <inheritdoc/>
partial record TypedConstantInfo
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="TypedConstantInfo"/>.
    /// </summary>
    public sealed class Comparer : IEqualityComparer<TypedConstantInfo>
    {
        /// <summary>
        /// The singleton <see cref="Comparer"/> instance.
        /// </summary>
        public static Comparer Default { get; } = new();

        /// <inheritdoc/>
        public bool Equals(TypedConstantInfo? x, TypedConstantInfo? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.IsEqualTo(y);
        }

        /// <inheritdoc/>
        public int GetHashCode(TypedConstantInfo obj)
        {
            HashCode hashCode = default;

            obj.AddToHashCode(ref hashCode);

            return hashCode.ToHashCode();
        }
    }
}

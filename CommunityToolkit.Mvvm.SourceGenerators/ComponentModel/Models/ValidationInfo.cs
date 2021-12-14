// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;

namespace CommunityToolkit.Mvvm.SourceGenerators.Input.Models;

/// <summary>
/// A model with gathered info on all validatable properties in a given type.
/// </summary>
/// <param name="FilenameHint">The filename hint for the current type.</param>
/// <param name="TypeName">The fully qualified type name of the target type.</param>
/// <param name="PropertyNames">The name of validatable properties.</param>
internal sealed record ValidationInfo(
    string FilenameHint,
    string TypeName,
    ImmutableArray<string> PropertyNames)
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="ValidationInfo"/>.
    /// </summary>
    public sealed class Comparer : IEqualityComparer<ValidationInfo>
    {
        /// <summary>
        /// The singleton <see cref="Comparer"/> instance.
        /// </summary>
        public static Comparer Default { get; } = new();

        /// <inheritdoc/>
        public bool Equals(ValidationInfo x, ValidationInfo y)
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

            return
                x.FilenameHint == y.FilenameHint &&
                x.TypeName == y.TypeName &&
                x.PropertyNames.SequenceEqual(y.PropertyNames);
        }

        /// <inheritdoc/>
        public int GetHashCode(ValidationInfo obj)
        {
            HashCode hashCode = default;

            hashCode.Add(obj.FilenameHint);
            hashCode.Add(obj.TypeName);
            hashCode.AddRange(obj.PropertyNames);

            return hashCode.ToHashCode();
        }
    }
}

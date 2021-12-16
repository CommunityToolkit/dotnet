// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <inheritdoc/>
partial record TypedConstantInfo
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="TypedConstantInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<TypedConstantInfo, Comparer>
    {
        /// <inheritdoc/>
        protected override void AddToHashCode(ref HashCode hashCode, TypedConstantInfo obj)
        {
            obj.AddToHashCode(ref hashCode);
        }

        /// <inheritdoc/>
        protected override bool AreEqual(TypedConstantInfo x, TypedConstantInfo y)
        {
            return x.IsEqualTo(y);
        }
    }
}

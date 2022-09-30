// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file is ported and adapted from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace CommunityToolkit.Mvvm.SourceGenerators.Models;

/// <summary>
/// A model representing a value and an associated set of diagnostic errors.
/// </summary>
/// <typeparam name="TValue">The type of the wrapped value.</typeparam>
/// <param name="Value">The wrapped value for the current result.</param>
/// <param name="Errors">The associated diagnostic errors, if any.</param>
internal sealed record Result<TValue>(TValue Value, ImmutableArray<DiagnosticInfo> Errors)
    where TValue : IEquatable<TValue>?
{
    /// <inheritdoc/>
    public bool Equals(Result<TValue>? obj) => Comparer.Default.Equals(this, obj);

    /// <inheritdoc/>
    public override int GetHashCode() => Comparer.Default.GetHashCode(this);

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="Result{TValue}"/>.
    /// </summary>
    private sealed class Comparer : Comparer<Result<TValue>, Comparer>
    {
        /// <inheritdoc/>
        protected override void AddToHashCode(ref HashCode hashCode, Result<TValue> obj)
        {
            hashCode.Add(obj.Value);
            hashCode.AddRange(obj.Errors);
        }

        /// <inheritdoc/>
        protected override bool AreEqual(Result<TValue> x, Result<TValue> y)
        {
            return
                EqualityComparer<TValue>.Default.Equals(x.Value, y.Value) &&
                x.Errors.SequenceEqual(y.Errors);
        }
    }
}

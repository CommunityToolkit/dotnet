// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace CommunityToolkit.Mvvm.SourceGenerators.Input.Models;

/// <summary>
/// A model with gathered info on all message types being handled by a recipient.
/// </summary>
/// <param name="FilenameHint">The filename hint for the current type.</param>
/// <param name="TypeName">The fully qualified type name of the target type.</param>
/// <param name="MessageTypes">The name of messages being received.</param>
internal sealed record RecipientInfo(
    string FilenameHint,
    string TypeName,
    ImmutableArray<string> MessageTypes)
{
    /// <summary>
    /// Gets an <see cref="IEqualityComparer{T}"/> implementation only matching by <see cref="FilenameHint"/>.
    /// </summary>
    public static IEqualityComparer<RecipientInfo> EqualityComparerByFilenameHint { get; } = new FilenameHintComparer();

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="RecipientInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<RecipientInfo, Comparer>
    {
        /// <inheritdoc/>
        protected override void AddToHashCode(ref HashCode hashCode, RecipientInfo obj)
        {
            hashCode.Add(obj.FilenameHint);
            hashCode.Add(obj.TypeName);
            hashCode.AddRange(obj.MessageTypes);
        }

        /// <inheritdoc/>
        protected override bool AreEqual(RecipientInfo x, RecipientInfo y)
        {
            return
                x.FilenameHint == y.FilenameHint &&
                x.TypeName == y.TypeName &&
                x.MessageTypes.SequenceEqual(y.MessageTypes);
        }
    }

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation only matching by <see cref="FilenameHint"/>.
    /// </summary>
    private sealed class FilenameHintComparer : IEqualityComparer<RecipientInfo>
    {
        /// <inheritdoc/>
        public bool Equals(RecipientInfo x, RecipientInfo y)
        {
            return x.FilenameHint == y.FilenameHint;
        }

        /// <inheritdoc/>
        public int GetHashCode(RecipientInfo obj)
        {
            return obj.FilenameHint.GetHashCode();
        }
    }
}

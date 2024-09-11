// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.HighPerformance.Streams;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// Helpers for working with the <see cref="ReadOnlySequence{T}"/> type.
/// </summary>
public static class ReadOnlySequenceExtensions
{
    /// <summary>
    /// Returns a <see cref="Stream"/> wrapping the contents of the given <see cref="Memory{T}"/> of <see cref="byte"/> instance.
    /// </summary>
    /// <param name="sequence">The input <see cref="ReadOnlySequence{T}"/> of <see cref="byte"/> instance.</param>
    /// <returns>A <see cref="Stream"/> wrapping the data within <paramref name="sequence"/>.</returns>
    /// <remarks>
    /// Since this method only receives a <see cref="ReadOnlySequence{T}"/> instance, which does not track
    /// the lifetime of its underlying buffer, it is responsibility of the caller to manage that.
    /// In particular, the caller must ensure that the target buffer is not disposed as long
    /// as the returned <see cref="Stream"/> is in use, to avoid unexpected issues.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream AsStream(this ReadOnlySequence<byte> sequence)
    {
        return ReadOnlySequenceStream.Create(sequence);
    }
}

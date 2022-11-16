// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using MemoryStream = CommunityToolkit.HighPerformance.Streams.MemoryStream;

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// Helpers for working with the <see cref="IMemoryOwner{T}"/> type.
/// </summary>
public static class IMemoryOwnerExtensions
{
    /// <summary>
    /// Returns a <see cref="Stream"/> wrapping the contents of the given <see cref="IMemoryOwner{T}"/> of <see cref="byte"/> instance.
    /// </summary>
    /// <param name="memoryOwner">The input <see cref="IMemoryOwner{T}"/> of <see cref="byte"/> instance.</param>
    /// <returns>A <see cref="Stream"/> wrapping the data within <paramref name="memoryOwner"/>.</returns>
    /// <remarks>
    /// The caller does not need to track the lifetime of the input <see cref="IMemoryOwner{T}"/> of <see cref="byte"/>
    /// instance, as the returned <see cref="Stream"/> will take care of disposing that buffer when it is closed.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="memoryOwner"/> has an invalid data store.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream AsStream(this IMemoryOwner<byte> memoryOwner)
    {
        return MemoryStream.Create(memoryOwner);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;

namespace CommunityToolkit.HighPerformance.Streams;

/// <summary>
/// An interface for types acting as sources for <see cref="Span{T}"/> instances.
/// </summary>
internal interface ISpanOwner
{
    /// <summary>
    /// Gets the length of the underlying memory area.
    /// </summary>
    int Length { get; }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> instance wrapping the underlying memory area.
    /// </summary>
    Span<byte> Span { get; }

    /// <summary>
    /// Gets a <see cref="Memory{T}"/> instance wrapping the underlying memory area.
    /// </summary>
    Memory<byte> Memory { get; }
}

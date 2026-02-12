// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace CommunityToolkit.HighPerformance.Memory.Views;

/// <summary>
/// A debug proxy used to display items in a 3D layout.
/// </summary>
/// <typeparam name="T">The type of items to display.</typeparam>
internal sealed class MemoryDebugView3D<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDebugView3D{T}"/> class with the specified parameters.
    /// </summary>
    /// <param name="memory">The input <see cref="Memory3D{T}"/> instance with the items to display.</param>
    public MemoryDebugView3D(Memory3D<T> memory)
    {
        Items = memory.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDebugView3D{T}"/> class with the specified parameters.
    /// </summary>
    /// <param name="memory">The input <see cref="ReadOnlyMemory3D{T}"/> instance with the items to display.</param>
    public MemoryDebugView3D(ReadOnlyMemory3D<T> memory)
    {
        Items = memory.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDebugView3D{T}"/> class with the specified parameters.
    /// </summary>
    /// <param name="span">The input <see cref="Span3D{T}"/> instance with the items to display.</param>
    public MemoryDebugView3D(Span3D<T> span)
    {
        Items = span.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDebugView3D{T}"/> class with the specified parameters.
    /// </summary>
    /// <param name="span">The input <see cref="ReadOnlySpan3D{T}"/> instance with the items to display.</param>
    public MemoryDebugView3D(ReadOnlySpan3D<T> span)
    {
        Items = span.ToArray();
    }

    /// <summary>
    /// Gets the items to display for the current instance
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public T[,,]? Items { get; }
}

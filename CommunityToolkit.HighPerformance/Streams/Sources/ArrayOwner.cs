// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Runtime.CompilerServices;
#if NETSTANDARD2_1_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace CommunityToolkit.HighPerformance.Streams;

/// <summary>
/// An <see cref="ISpanOwner"/> implementation wrapping an array.
/// </summary>
internal readonly struct ArrayOwner : ISpanOwner
{
    /// <summary>
    /// The wrapped <see cref="byte"/> array.
    /// </summary>
    private readonly byte[] array;

    /// <summary>
    /// The starting offset within <see cref="array"/>.
    /// </summary>
    private readonly int offset;

    /// <summary>
    /// The usable length within <see cref="array"/>.
    /// </summary>
    private readonly int length;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayOwner"/> struct.
    /// </summary>
    /// <param name="array">The wrapped <see cref="byte"/> array.</param>
    /// <param name="offset">The starting offset within <paramref name="array"/>.</param>
    /// <param name="length">The usable length within <paramref name="array"/>.</param>
    public ArrayOwner(byte[] array, int offset, int length)
    {
        this.array = array;
        this.offset = offset;
        this.length = length;
    }

    /// <summary>
    /// Gets an empty <see cref="ArrayOwner"/> instance.
    /// </summary>
    public static ArrayOwner Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Array.Empty<byte>(), 0, 0);
    }

    /// <inheritdoc/>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.length;
    }

    /// <inheritdoc/>
    public Span<byte> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
#if NETSTANDARD2_1_OR_GREATER
            ref byte r0 = ref this.array.DangerousGetReferenceAt(this.offset);

            return MemoryMarshal.CreateSpan(ref r0, this.length);
#else
            return this.array.AsSpan(this.offset, this.length);
#endif
        }
    }

    /// <inheritdoc/>
    public Memory<byte> Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.array.AsMemory(this.offset, this.length);
    }
}

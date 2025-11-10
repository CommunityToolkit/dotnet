// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Mvvm.Messaging.Internals;

/// <summary>
/// A simple buffer writer implementation using pooled arrays.
/// </summary>
/// <typeparam name="T">The type of items to store in the list.</typeparam>
/// <remarks>
/// This type is a <see langword="ref"/> <see langword="struct"/> to avoid the object allocation and to
/// enable the pattern-based <see cref="IDisposable"/> support. We aren't worried with consumers not
/// using this type correctly since it's private and only accessible within the parent type.
/// </remarks>
internal ref struct ArrayPoolBufferWriter<T>
{
    /// <summary>
    /// The default buffer size to use to expand empty arrays.
    /// </summary>
    private const int DefaultInitialBufferSize = 128;

    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    private T[] array;

    /// <summary>
    /// The span mapping to <see cref="array"/>.
    /// </summary>
    /// <remarks>All writes are done through this to avoid covariance checks.</remarks>
    private Span<T> span;

    /// <summary>
    /// The starting offset within <see cref="array"/>.
    /// </summary>
    private int index;

    /// <summary>
    /// Creates a new instance of the <see cref="ArrayPoolBufferWriter{T}"/> struct.
    /// </summary>
    /// <returns>A new <see cref="ArrayPoolBufferWriter{T}"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayPoolBufferWriter<T> Create()
    {
        ArrayPoolBufferWriter<T> instance;

        instance.span = instance.array = ArrayPool<T>.Shared.Rent(DefaultInitialBufferSize);
        instance.index = 0;

        return instance;
    }

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> with the current items.
    /// </summary>
    public readonly ReadOnlySpan<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.span.Slice(0, this.index);
    }

    /// <summary>
    /// Adds a new item to the current collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        Span<T> span = this.span;
        int index = this.index;

        if ((uint)index < (uint)span.Length)
        {
            span[index] = item;

            this.index = index + 1;
        }
        else
        {
            ResizeBufferAndAdd(item);
        }
    }

    /// <summary>
    /// Resets the underlying array and the stored items.
    /// </summary>
    public void Reset()
    {
        Array.Clear(this.array, 0, this.index);

        this.index = 0;
    }

    /// <summary>
    /// Resizes <see cref="array"/> when there is no space left for new items, then adds one
    /// </summary>
    /// <param name="item">The item to add.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ResizeBufferAndAdd(T item)
    {
        T[] rent = ArrayPool<T>.Shared.Rent(this.index << 2);

        Array.Copy(this.array, 0, rent, 0, this.index);
        Array.Clear(this.array, 0, this.index);

        ArrayPool<T>.Shared.Return(this.array);

        this.span = this.array = rent;

        this.span[this.index++] = item;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public readonly void Dispose()
    {
        Array.Clear(this.array, 0, this.index);

        ArrayPool<T>.Shared.Return(this.array);
    }
}

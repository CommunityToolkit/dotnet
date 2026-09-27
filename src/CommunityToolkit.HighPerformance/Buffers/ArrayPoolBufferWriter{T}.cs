// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers.Views;
#if NET6_0_OR_GREATER
using BitOperations = System.Numerics.BitOperations;
#else
using BitOperations = CommunityToolkit.HighPerformance.Helpers.Internals.BitOperations;
#endif

namespace CommunityToolkit.HighPerformance.Buffers;

/// <summary>
/// Represents a heap-based, array-backed output sink into which <typeparamref name="T"/> data can be written.
/// </summary>
/// <typeparam name="T">The type of items to write to the current instance.</typeparam>
/// <remarks>
/// This is a custom <see cref="IBufferWriter{T}"/> implementation that replicates the
/// functionality and API surface of the array-based buffer writer available in
/// .NET Standard 2.1, with the main difference being the fact that in this case
/// the arrays in use are rented from the shared <see cref="ArrayPool{T}"/> instance,
/// and that <see cref="ArrayPoolBufferWriter{T}"/> is also available on .NET Standard 2.0.
/// </remarks>
[DebuggerTypeProxy(typeof(MemoryDebugView<>))]
[DebuggerDisplay("{ToString(),raw}")]
public sealed class ArrayPoolBufferWriter<T> : IBuffer<T>, IMemoryOwner<T>
{
    /// <summary>
    /// The default buffer size to use to expand empty arrays.
    /// </summary>
    private const int DefaultInitialBufferSize = 256;

    /// <summary>
    /// The maximum size to target when growing buffers geometrically.
    /// </summary>
    private const int MaximumSegmentSize = 1024 * 1024;

    /// <summary>
    /// The <see cref="ArrayPool{T}"/> instance used to rent <see cref="array"/>.
    /// </summary>
    private readonly ArrayPool<T> pool;

    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    private T[]? array;

    /// <summary>
    /// The completed buffers that were previously used as active write targets.
    /// </summary>
    private List<BufferInfo>? buffers;

    /// <summary>
    /// The total number of written items in <see cref="buffers"/>.
    /// </summary>
    private int bufferedCount;

#pragma warning disable IDE0032 // Use field over auto-property (clearer and faster)
    /// <summary>
    /// The starting offset within <see cref="array"/>.
    /// </summary>
    private int index;
#pragma warning restore IDE0032

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayPoolBufferWriter{T}"/> class.
    /// </summary>
    public ArrayPoolBufferWriter()
        : this(ArrayPool<T>.Shared, DefaultInitialBufferSize)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayPoolBufferWriter{T}"/> class.
    /// </summary>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
    public ArrayPoolBufferWriter(ArrayPool<T> pool)
        : this(pool, DefaultInitialBufferSize)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayPoolBufferWriter{T}"/> class.
    /// </summary>
    /// <param name="initialCapacity">The minimum capacity with which to initialize the underlying buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="initialCapacity"/> is not valid.</exception>
    public ArrayPoolBufferWriter(int initialCapacity)
        : this(ArrayPool<T>.Shared, initialCapacity)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayPoolBufferWriter{T}"/> class.
    /// </summary>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
    /// <param name="initialCapacity">The minimum capacity with which to initialize the underlying buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="initialCapacity"/> is not valid.</exception>
    public ArrayPoolBufferWriter(ArrayPool<T> pool, int initialCapacity)
    {
        // Since we're using pooled arrays, we can rent the buffer with the
        // default size immediately, we don't need to use lazy initialization
        // to save unnecessary memory allocations in this case.
        // Additionally, we don't need to manually throw the exception if
        // the requested size is not valid, as that'll be thrown automatically
        // by the array pool in use when we try to rent an array with that size.
        this.pool = pool;
        this.array = pool.Rent(initialCapacity);
        this.index = 0;
    }

    /// <inheritdoc/>
    Memory<T> IMemoryOwner<T>.Memory
    {
        // This property is explicitly implemented so that it's hidden
        // under normal usage, as the name could be confusing when
        // displayed besides WrittenMemory and GetMemory().
        // The IMemoryOwner<T> interface is implemented primarily
        // so that the AsStream() extension can be used on this type,
        // allowing users to first create a ArrayPoolBufferWriter<byte>
        // instance to write data to, then get a stream through the
        // extension and let it take care of returning the underlying
        // buffer to the shared pool when it's no longer necessary.
        // Inlining is not needed here since this will always be a callvirt.
        get => MemoryMarshal.AsMemory(WrittenMemory);
    }

    /// <inheritdoc/>
    public ReadOnlyMemory<T> WrittenMemory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            EnsureSingleBuffer();

            return this.array.AsMemory(0, this.index);
        }
    }

    /// <inheritdoc/>
    public ReadOnlySpan<T> WrittenSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            EnsureSingleBuffer();

            return this.array.AsSpan(0, this.index);
        }
    }

    /// <inheritdoc/>
    public int WrittenCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.bufferedCount + this.index;
    }

    /// <inheritdoc/>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            T[]? array = this.array;

            if (array is null)
            {
                ThrowObjectDisposedException();
            }

            return this.bufferedCount + Math.Min(array!.Length, int.MaxValue - this.bufferedCount);
        }
    }

    /// <inheritdoc/>
    public int FreeCapacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            T[]? array = this.array;

            if (array is null)
            {
                ThrowObjectDisposedException();
            }

            return Math.Min(array!.Length - this.index, int.MaxValue - WrittenCount);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        T[]? array = this.array;

        if (array is null)
        {
            ThrowObjectDisposedException();
        }

        if (this.buffers is { Count: > 0 } buffers)
        {
            foreach (BufferInfo buffer in buffers)
            {
                buffer.Array.AsSpan(0, buffer.Length).Clear();
                this.pool.Return(buffer.Array);
            }

            buffers.Clear();
            this.bufferedCount = 0;
        }

        array.AsSpan(0, this.index).Clear();

        this.index = 0;
    }

    /// <inheritdoc/>
    public void Advance(int count)
    {
        T[]? array = this.array;

        if (array is null)
        {
            ThrowObjectDisposedException();
        }

        if (count < 0)
        {
            ThrowArgumentOutOfRangeExceptionForNegativeCount();
        }

        if (this.index > array!.Length - count)
        {
            ThrowArgumentExceptionForAdvancedTooFar();
        }

        if (count > int.MaxValue - WrittenCount)
        {
            ThrowArgumentExceptionForAdvancedTooFar();
        }

        this.index += count;
    }

    /// <inheritdoc/>
    public Memory<T> GetMemory(int sizeHint = 0)
    {
        CheckBufferAndEnsureCapacity(sizeHint);

        return this.array.AsMemory(this.index, FreeCapacity);
    }

    /// <inheritdoc/>
    public Span<T> GetSpan(int sizeHint = 0)
    {
        CheckBufferAndEnsureCapacity(sizeHint);

        return this.array.AsSpan(this.index, FreeCapacity);
    }

    /// <summary>
    /// Gets a <see cref="ReadOnlySequence{T}"/> over the written data.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySequence{T}"/> over the written data.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the current instance has been disposed.</exception>
    /// <remarks>
    /// The returned sequence is only valid as long as the current instance is not modified, cleared, consolidated or disposed.
    /// </remarks>
    public ReadOnlySequence<T> GetReadOnlySequence()
    {
        T[]? array = this.array;

        if (array is null)
        {
            ThrowObjectDisposedException();
        }

        if (this.bufferedCount + this.index == 0)
        {
            return ReadOnlySequence<T>.Empty;
        }

        if (this.buffers is not { Count: > 0 })
        {
            return new ReadOnlySequence<T>(array!, 0, this.index);
        }

        SequenceSegment? startSegment = null;
        SequenceSegment? endSegment = null;

        foreach (BufferInfo buffer in this.buffers)
        {
            SequenceSegment segment = new(buffer.Array.AsMemory(0, buffer.Length));

            if (startSegment is null)
            {
                startSegment = segment;
            }
            else
            {
                endSegment!.SetNext(segment);
            }

            endSegment = segment;
        }

        if (this.index > 0)
        {
            SequenceSegment segment = new(array!.AsMemory(0, this.index));

            endSegment!.SetNext(segment);
            endSegment = segment;
        }

        return new(startSegment!, 0, endSegment!, endSegment!.Memory.Length);
    }

    /// <summary>
    /// Gets an <see cref="ArraySegment{T}"/> instance wrapping the underlying <typeparamref name="T"/> array in use.
    /// </summary>
    /// <returns>An <see cref="ArraySegment{T}"/> instance wrapping the underlying <typeparamref name="T"/> array in use.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the buffer in use has already been disposed.</exception>
    /// <remarks>
    /// This method is meant to be used when working with APIs that only accept an array as input, and should be used with caution.
    /// In particular, the returned array is rented from an array pool, and it is responsibility of the caller to ensure that it's
    /// not used after the current <see cref="ArrayPoolBufferWriter{T}"/> instance is disposed. Doing so is considered undefined
    /// behavior, as the same array might be in use within another <see cref="ArrayPoolBufferWriter{T}"/> instance.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArraySegment<T> DangerousGetArray()
    {
        EnsureSingleBuffer();

        return new(this.array!, 0, this.index);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        T[]? array = this.array;

        if (array is null)
        {
            return;
        }

        this.array = null;

        if (this.buffers is { Count: > 0 } buffers)
        {
            foreach (BufferInfo buffer in buffers)
            {
                this.pool.Return(buffer.Array);
            }

            buffers.Clear();
        }

        this.bufferedCount = 0;
        this.index = 0;

        this.pool.Return(array);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        // See comments in MemoryOwner<T> about this
        if (typeof(T) == typeof(char) &&
            this.array is char[] chars)
        {
            if (this.bufferedCount == 0)
            {
                return new(chars, 0, this.index);
            }

#if NETSTANDARD2_0
            ArrayPool<char> pool = (ArrayPool<char>)(object)this.pool;
            char[] buffer = pool.Rent(WrittenCount);

            try
            {
                CopyWrittenCharsTo(buffer);

                return new(buffer, 0, WrittenCount);
            }
            finally
            {
                pool.Return(buffer);
            }
#else
            return string.Create(WrittenCount, this, static (destination, writer) => writer.CopyWrittenCharsTo(destination));
#endif
        }

        // Same representation used in Span<T>
        return $"CommunityToolkit.HighPerformance.Buffers.ArrayPoolBufferWriter<{typeof(T)}>[{WrittenCount}]";
    }

    /// <summary>
    /// Copies the written characters to a destination without modifying the writer.
    /// </summary>
    /// <param name="destination">The destination for the written characters.</param>
    private void CopyWrittenCharsTo(Span<char> destination)
    {
        foreach (BufferInfo buffer in this.buffers!)
        {
            ((char[])(object)buffer.Array).AsSpan(0, buffer.Length).CopyTo(destination);
            destination = destination.Slice(buffer.Length);
        }

        ((char[])(object)this.array!).AsSpan(0, this.index).CopyTo(destination);
    }

    /// <summary>
    /// Ensures that <see cref="array"/> has enough free space to contain a given number of new items.
    /// </summary>
    /// <param name="sizeHint">The minimum number of items to ensure space for in <see cref="array"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckBufferAndEnsureCapacity(int sizeHint)
    {
        T[]? array = this.array;

        if (array is null)
        {
            ThrowObjectDisposedException();
        }

        if (sizeHint < 0)
        {
            ThrowArgumentOutOfRangeExceptionForNegativeSizeHint();
        }

        if (sizeHint == 0)
        {
            sizeHint = 1;
        }

        // The public counts are ints, so never expose or retain more than int.MaxValue items.
        // Check before renting to preserve ownership and the current sequence on failure.
        if (sizeHint > int.MaxValue - WrittenCount)
        {
            ThrowArgumentOutOfRangeExceptionForExcessiveSizeHint();
        }

        if (sizeHint > array!.Length - this.index)
        {
            RentNextBuffer(sizeHint);
        }
    }

    /// <summary>
    /// Rents a new active buffer with enough space for the specified number of new items.
    /// </summary>
    /// <param name="sizeHint">The minimum number of items to ensure space for in <see cref="array"/>.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void RentNextBuffer(int sizeHint)
    {
        Debug.Assert(this.array is not null);

        T[] array = this.array!;

        // Rent before transferring ownership of the current buffer so that a failed rent
        // leaves the writer unchanged and still owning its current buffer.
        T[] nextArray = this.pool.Rent(GetMinimumBufferSize(array.Length, sizeHint));

        try
        {
            if (this.index > 0)
            {
                AddBuffer(array, this.index);
            }
            else
            {
                this.pool.Return(array);
            }
        }
        catch
        {
            // If transferring the current buffer fails, return the new one to avoid a leak.
            this.pool.Return(nextArray);

            throw;
        }

        this.array = nextArray;
        this.index = 0;
    }

    /// <summary>
    /// Adds a completed buffer to <see cref="buffers"/>.
    /// </summary>
    /// <param name="array">The buffer to add.</param>
    /// <param name="length">The number of written items in <paramref name="array"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddBuffer(T[] array, int length)
    {
        (this.buffers ??= new List<BufferInfo>()).Add(new BufferInfo(array, length));
        this.bufferedCount += length;
    }

    /// <summary>
    /// Gets the minimum size to use for a new active buffer.
    /// </summary>
    /// <param name="currentLength">The length of the current active buffer.</param>
    /// <param name="sizeHint">The minimum required number of writable items.</param>
    /// <returns>The minimum size to request to the pool.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetMinimumBufferSize(int currentLength, int sizeHint)
    {
        int nextSize = currentLength < MaximumSegmentSize
            ? Math.Min(currentLength * 2, MaximumSegmentSize)
            : currentLength;

        uint minimumSize = (uint)Math.Max(nextSize, sizeHint);

        // The ArrayPool<T> class has a maximum threshold of 1024 * 1024 for the maximum length of
        // pooled arrays, and once this is exceeded it will just allocate a new array every time
        // of exactly the requested size. In that case, we manually round up the requested size to
        // the nearest power of two, to avoid renting a new array for every consecutive write
        // once the active buffer exceeds that threshold.
        if (minimumSize > MaximumSegmentSize)
        {
            minimumSize = BitOperations.RoundUpToPowerOf2(minimumSize);
        }

        return (int)minimumSize;
    }

    /// <summary>
    /// Ensures all written data is represented by a single active buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureSingleBuffer()
    {
        T[]? array = this.array;

        if (array is null)
        {
            ThrowObjectDisposedException();
        }

        if (this.bufferedCount == 0)
        {
            return;
        }

        ConsolidateBuffers();
    }

    /// <summary>
    /// Consolidates all written data into a single active buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ConsolidateBuffers()
    {
        T[] currentBuffer = this.array!;
        int totalCount = this.bufferedCount + this.index;
        T[] targetBuffer = this.pool.Rent(totalCount);
        int offset = 0;

        foreach (BufferInfo buffer in this.buffers!)
        {
            buffer.Array.AsSpan(0, buffer.Length).CopyTo(targetBuffer.AsSpan(offset));
            offset += buffer.Length;
            this.pool.Return(buffer.Array);
        }

        currentBuffer.AsSpan(0, this.index).CopyTo(targetBuffer.AsSpan(offset));
        this.pool.Return(currentBuffer);

        this.buffers!.Clear();
        this.bufferedCount = 0;
        this.array = targetBuffer;
        this.index = totalCount;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> when the requested count is negative.
    /// </summary>
    private static void ThrowArgumentOutOfRangeExceptionForNegativeCount()
    {
        throw new ArgumentOutOfRangeException("count", "The count can't be a negative value.");
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> when the size hint is negative.
    /// </summary>
    private static void ThrowArgumentOutOfRangeExceptionForNegativeSizeHint()
    {
        throw new ArgumentOutOfRangeException("sizeHint", "The size hint can't be a negative value.");
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> when the requested size exceeds the supported length.
    /// </summary>
    private static void ThrowArgumentOutOfRangeExceptionForExcessiveSizeHint()
    {
        throw new ArgumentOutOfRangeException("sizeHint", "The buffer writer cannot contain more than int.MaxValue items.");
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> when the requested count is negative.
    /// </summary>
    private static void ThrowArgumentExceptionForAdvancedTooFar()
    {
        throw new ArgumentException("The buffer writer has advanced too far.");
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> when <see cref="array"/> is <see langword="null"/>.
    /// </summary>
    private static void ThrowObjectDisposedException()
    {
        throw new ObjectDisposedException("The current buffer has already been disposed.");
    }

    /// <summary>
    /// The metadata for each completed buffer.
    /// </summary>
    private readonly struct BufferInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferInfo"/> struct.
        /// </summary>
        /// <param name="array">The underlying array.</param>
        /// <param name="length">The number of written items.</param>
        public BufferInfo(T[] array, int length)
        {
            this.Array = array;
            this.Length = length;
        }

        /// <summary>
        /// Gets the underlying array.
        /// </summary>
        public T[] Array { get; }

        /// <summary>
        /// Gets the number of written items in <see cref="Array"/>.
        /// </summary>
        public int Length { get; }
    }

    /// <summary>
    /// A <see cref="ReadOnlySequenceSegment{T}"/> implementation for pooled buffers.
    /// </summary>
    private sealed class SequenceSegment : ReadOnlySequenceSegment<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceSegment"/> class.
        /// </summary>
        /// <param name="memory">The memory to wrap.</param>
        public SequenceSegment(ReadOnlyMemory<T> memory)
        {
            this.Memory = memory;
        }

        /// <summary>
        /// Sets the next segment and updates the running index.
        /// </summary>
        /// <param name="next">The next segment.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetNext(SequenceSegment next)
        {
            next.RunningIndex = this.RunningIndex + this.Memory.Length;
            this.Next = next;
        }
    }
}

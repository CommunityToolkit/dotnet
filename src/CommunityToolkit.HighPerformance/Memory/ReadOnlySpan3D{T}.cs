// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !NETSTANDARD2_1_OR_GREATER
using CommunityToolkit.HighPerformance.Helpers;
#endif
using CommunityToolkit.HighPerformance.Memory.Internals;
using CommunityToolkit.HighPerformance.Memory.Views;
#if !NETSTANDARD2_1_OR_GREATER
using RuntimeHelpers = CommunityToolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;
#endif

#pragma warning disable CS0809, CA1065

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// A readonly version of <see cref="Span3D{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items in the current <see cref="ReadOnlySpan3D{T}"/> instance.</typeparam>
[DebuggerTypeProxy(typeof(MemoryDebugView3D<>))]
[DebuggerDisplay("{ToString(),raw}")]
public readonly ref partial struct ReadOnlySpan3D<T>
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// The <typeparamref name="T"/> reference for the <see cref="ReadOnlySpan3D{T}"/> instance.
    /// </summary>
    private readonly ref readonly T reference;

    /// <summary>
    /// The depth of the specified 3D region.
    /// </summary>
    private readonly int depth;
#elif NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// The <see cref="ReadOnlySpan{T}"/> instance pointing to the first item in the target memory area.
    /// </summary>
    /// <remarks>
    /// The <see cref="ReadOnlySpan{T}.Length"/> field maps to the depth of the 3D region.
    /// This is done to save 4 bytes in the layout of the <see cref="ReadOnlySpan3D{T}"/> type.
    /// </remarks>
    private readonly ReadOnlySpan<T> span;
#else
    /// <summary>
    /// The target <see cref="object"/> instance, if present.
    /// </summary>
    private readonly object? instance;

    /// <summary>
    /// The initial byte offset within <see cref="instance"/>.
    /// </summary>
    private readonly nint offset;

    /// <summary>
    /// The depth of the specified 3D region.
    /// </summary>
    private readonly int depth;
#endif

    /// <summary>
    /// The height of the specified 3D region.
    /// </summary>
    private readonly int height;

    /// <summary>
    /// The width of the specified 3D region.
    /// </summary>
    private readonly int width;

    /// <summary>
    /// The slice stride of the specified 3D region.
    /// </summary>
    /// <remarks>
    /// This combines both the slice size (RowStride and height) and the slice pitch
    /// in a single value so that the indexing logic can be simplified
    /// (no need to recompute the sum every time) and be faster.
    /// </remarks>
    internal readonly int SliceStride;

    /// <summary>
    /// The row stride of the specified 3D region.
    /// </summary>
    /// <remarks>
    /// This combines both the width and row pitch in a single value so that the indexing
    /// logic can be simplified (no need to recompute the sum every time) and be faster.
    /// </remarks>
    internal readonly int RowStride;

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct with the specified parameters.
    /// </summary>
    /// <param name="value">The reference to the first <typeparamref name="T"/> item to map.</param>
    /// <param name="depth">The depth of the 3D memory area to map.</param>
    /// <param name="height">The height of the 3D memory area to map.</param>
    /// <param name="width">The width of the 3D memory area to map.</param>
    /// <param name="slicePitch">The slice pitch of the 3D memory area to map (the distance between each slice).</param>
    /// <param name="rowPitch">The row pitch of the 3D memory area to map (the distance between each row).</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan3D(in T value, int depth, int height, int width, int slicePitch, int rowPitch)
    {
#if NET8_0_OR_GREATER
        this.reference = ref value;
        this.depth = depth;
#else
        this.span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(value), depth);
#endif
        OverflowHelper.EnsureIsInNativeIntRange(depth, height, width, slicePitch, rowPitch);

        this.height = height;
        this.width = width;
        this.RowStride = width + rowPitch;
        this.SliceStride = (this.RowStride * height) + slicePitch;
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct with the specified parameters.
    /// </summary>
    /// <param name="pointer">The pointer to the first <typeparamref name="T"/> item to map.</param>
    /// <param name="depth">The depth of the 3D memory area to map.</param>
    /// <param name="height">The height of the 3D memory area to map.</param>
    /// <param name="width">The width of the 3D memory area to map.</param>
    /// <param name="slicePitch">The slice pitch of the 3D memory area to map (the distance between each slice).</param>
    /// <param name="rowPitch">The pitch of the 3D memory area to map (the distance between each row).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one of the parameters are negative.</exception>
    public unsafe ReadOnlySpan3D(void* pointer, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            ThrowHelper.ThrowArgumentExceptionForManagedType();
        }

        if (width < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
        }

        if (height < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
        }

        if (depth < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
        }

        if (rowPitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRowPitch();
        }

        if (slicePitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlicePitch();
        }

        OverflowHelper.EnsureIsInNativeIntRange(depth, height, width, slicePitch, rowPitch);

#if NET8_0_OR_GREATER
        this.reference = ref Unsafe.AsRef<T>(pointer);
        this.depth = depth;
#elif NETSTANDARD2_1_OR_GREATER
        this.span = new ReadOnlySpan<T>(pointer, depth);
#else
        this.instance = null;
        this.offset = (IntPtr)pointer;
        this.depth = depth;
#endif
        this.height = height;
        this.width = width;
        this.RowStride = width + rowPitch;
        this.SliceStride = (this.RowStride * height) + slicePitch;
    }

#if !NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct with the specified parameters.
    /// </summary>
    /// <param name="instance">The target <see cref="object"/> instance.</param>
    /// <param name="offset">The initial offset within the target instance.</param>
    /// <param name="depth">The depth of the 3D memory area to map.</param>
    /// <param name="height">The height of the 3D memory area to map.</param>
    /// <param name="width">The width of the 3D memory area to map.</param>
    /// <param name="slicePitch">The slice pitch of the 3D memory area to map.</param>
    /// <param name="rowPitch">The row pitch of the 3D memory area to map.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan3D(object? instance, IntPtr offset, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        this.instance = instance;
        this.offset = offset;
        this.depth = depth;
        this.height = height;
        this.width = width;
        this.RowStride = width + rowPitch;
        this.SliceStride = (this.RowStride * height) + slicePitch;
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct.
    /// </summary>
    /// <param name="array">The target array to wrap.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when either <paramref name="depth"/>, <paramref name="height"/>, or
    /// <paramref name="width"/> are invalid.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the requested volume is outside of bounds for <paramref name="array"/>.
    /// </exception>
    /// <remarks>The total volume must match the length of <paramref name="array"/>.</remarks>
    public ReadOnlySpan3D(T[] array, int depth, int height, int width)
        : this(array, offset: 0, depth, height, width, slicePitch: 0, rowPitch: 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct.
    /// </summary>
    /// <param name="array">The target array to wrap.</param>
    /// <param name="offset">The initial offset within <paramref name="array"/>.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <param name="slicePitch">The slice pitch in the resulting 3D area.</param>
    /// <param name="rowPitch">The row pitch in the resulting 3D area.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when one of the input parameters is out of range.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the requested volume is outside of bounds for <paramref name="array"/>.
    /// </exception>
    public ReadOnlySpan3D(T[] array, int offset, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        if ((uint)offset > (uint)array.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForOffset();
        }

        if (depth < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
        }

        if (height < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
        }

        if (width < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
        }

        if (slicePitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlicePitch();
        }

        if (rowPitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRowPitch();
        }

        int volume = OverflowHelper.ComputeInt32Volume(depth, height, width, slicePitch, rowPitch);
        int remaining = array.Length - offset;

        if (volume > remaining)
        {
            ThrowHelper.ThrowArgumentException();
        }

#if NET8_0_OR_GREATER
        this.reference = ref array.DangerousGetReferenceAt(offset);
        this.depth = depth;
#elif NETSTANDARD2_1_OR_GREATER
        this.span = MemoryMarshal.CreateReadOnlySpan(ref array.DangerousGetReferenceAt(offset), depth);
#else
        this.instance = array;
        this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(offset));
        this.depth = depth;
#endif
        this.height = height;
        this.width = width;
        this.RowStride = width + rowPitch;
        this.SliceStride = (this.RowStride * height) + slicePitch;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct wrapping a 3D array.
    /// </summary>
    /// <param name="array">The given 3D array to wrap.</param>
    public ReadOnlySpan3D(T[,,]? array)
    {
        if (array is null)
        {
            this = default;

            return;
        }

#if NET8_0_OR_GREATER
        this.reference = ref array.DangerousGetReference();
        this.depth = array.GetLength(0);
#elif NETSTANDARD2_1_OR_GREATER
        this.span = MemoryMarshal.CreateReadOnlySpan(ref array.DangerousGetReference(), array.GetLength(0));
#else
        this.instance = array;
        this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(0, 0, 0));
        this.depth = array.GetLength(0);
#endif
        this.height = array.GetLength(1);
        this.width = this.RowStride = array.GetLength(2);
        this.SliceStride = this.RowStride * this.height;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct wrapping a 3D array.
    /// </summary>
    /// <param name="array">The given 3D array to wrap.</param>
    /// <param name="slice">The target slice to map within <paramref name="array"/>.</param>
    /// <param name="row">The target row to map within <paramref name="array"/>.</param>
    /// <param name="column">The target column to map within <paramref name="array"/>.</param>
    /// <param name="depth">The depth to map within <paramref name="array"/>.</param>
    /// <param name="height">The height to map within <paramref name="array"/>.</param>
    /// <param name="width">The width to map within <paramref name="array"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when either <paramref name="slice"/>, <paramref name="row"/>, <paramref name="column"/>,
    /// <paramref name="height"/>, <paramref name="width"/>, or <paramref name="height"/>
    /// are negative or not within the bounds that are valid for <paramref name="array"/>.
    /// </exception>
    public ReadOnlySpan3D(T[,,]? array, int slice, int row, int column, int depth, int height, int width)
    {
        if (array is null)
        {
            if (slice != 0 || row != 0 || column != 0 || depth != 0|| height != 0 || width != 0)
            {
                ThrowHelper.ThrowArgumentException();
            }

            this = default;

            return;
        }

        int slices = array.GetLength(0);
        int rows = array.GetLength(1);
        int columns = array.GetLength(2);

        if ((uint)slice >= (uint)slices)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlice();
        }

        if ((uint)row >= (uint)rows)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
        }

        if ((uint)column >= (uint)columns)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForColumn();
        }

        if ((uint)depth > (uint)(slices - slice))
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
        }

        if ((uint)height > (uint)(rows - row))
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
        }

        if ((uint)width > (uint)(columns - column))
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
        }

#if NET8_0_OR_GREATER
        this.reference = ref array.DangerousGetReferenceAt(slice, row, column);
        this.depth = depth;
#elif NETSTANDARD2_1_OR_GREATER
        this.span = MemoryMarshal.CreateSpan(ref array.DangerousGetReferenceAt(slice, row, column), depth);
#else
        this.instance = array;
        this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(slice, row, column));
        this.depth = depth;
#endif
        this.height = height;
        this.width = width;
        this.RowStride = columns;
        this.SliceStride = this.RowStride * rows;
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct.
    /// </summary>
    /// <param name="span">The target <see cref="ReadOnlySpan{T}"/> to wrap.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when either <paramref name="depth"/>, <paramref name="height"/>, or <paramref name="width"/> are invalid.
    /// </exception>
    /// <remarks>The total volume must match the length of <paramref name="span"/>.</remarks>
    internal ReadOnlySpan3D(ReadOnlySpan<T> span, int depth, int height, int width)
        : this(span, offset: 0, depth, height, width, slicePitch: 0, rowPitch: 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct.
    /// </summary>
    /// <param name="span">The target <see cref="ReadOnlySpan{T}"/> to wrap.</param>
    /// <param name="offset">The initial offset within <paramref name="span"/>.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <param name="slicePitch">The slice pitch in the resulting 3D area.</param>
    /// <param name="rowPitch">The row pitch in the resulting 3D area.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when one of the input parameters is out of range.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the requested volume is outside of bounds for <paramref name="span"/>.
    /// </exception>
    internal ReadOnlySpan3D(ReadOnlySpan<T> span, int offset, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        if ((uint)offset > (uint)span.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForOffset();
        }

        if (depth < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
        }

        if (height < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
        }

        if (width < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
        }

        if (slicePitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlicePitch();
        }

        if (rowPitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRowPitch();
        }

        int volume = OverflowHelper.ComputeInt32Volume(depth, height, width, slicePitch, rowPitch);
        int remaining = span.Length - offset;

        if (volume > remaining)
        {
            ThrowHelper.ThrowArgumentException();
        }

#if NET8_0_OR_GREATER
        this.reference = ref span.DangerousGetReferenceAt(offset);
        this.depth = depth;
#else
        this.span = MemoryMarshal.CreateSpan(ref span.DangerousGetReferenceAt(offset), depth);
#endif
        this.height = height;
        this.width = width;
        this.RowStride = width + rowPitch;
        this.SliceStride = (this.RowStride * height) + slicePitch;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ReadOnlySpan3D{T}"/> struct with the specified parameters.
    /// </summary>
    /// <param name="value">The reference to the first <typeparamref name="T"/> item to map.</param>
    /// <param name="depth">The depth of the 3D memory area to map.</param>
    /// <param name="height">The height of the 3D memory area to map.</param>
    /// <param name="width">The width of the 3D memory area to map.</param>
    /// <param name="slicePitch">The slice pitch of the 3D memory area to map (the distance between each slice).</param>
    /// <param name="rowPitch">The row pitch of the 3D memory area to map (the distance between each row).</param>
    /// <returns>A <see cref="ReadOnlySpan3D{T}"/> instance with the specified parameters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one of the parameters are negative.</exception>
    public static ReadOnlySpan3D<T> DangerousCreate(in T value, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        if (width < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
        }

        if (height < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
        }

        if (depth < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
        }

        if (slicePitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlicePitch();
        }

        if (rowPitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRowPitch();
        }

        OverflowHelper.EnsureIsInNativeIntRange(depth, height, width, slicePitch, rowPitch);

        return new(in value, depth, height, width, slicePitch, rowPitch);
    }
#endif

    /// <summary>
    /// Gets an empty <see cref="ReadOnlySpan3D{T}"/> instance.
    /// </summary>
    public static ReadOnlySpan3D<T> Empty => default;

    /// <summary>
    /// Gets a value indicating whether the current <see cref="ReadOnlySpan3D{T}"/> instance is empty.
    /// </summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Depth == 0 || this.height == 0 || this.width == 0;
    }

    /// <summary>
    /// Gets the length of the current <see cref="ReadOnlySpan3D{T}"/> instance.
    /// </summary>
    public nint Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (nint)(uint)Depth * (nint)(uint)this.height * (nint)(uint)this.width;
    }

    /// <summary>
    /// Gets the depth of the underlying 3D memory area.
    /// </summary>
    public int Depth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
#if NET8_0_OR_GREATER
            return this.depth;
#elif NETSTANDARD2_1_OR_GREATER
            return this.span.Length;
#else
            return this.depth;
#endif
        }
    }

    /// <summary>
    /// Gets the height of the underlying 3D memory area.
    /// </summary>
    public int Height
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.height;
    }

    /// <summary>
    /// Gets the width of the underlying 3D memory area.
    /// </summary>
    public int Width
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.width;
    }

    /// <summary>
    /// Gets the element at the specified zero-based indices.
    /// </summary>
    /// <param name="slice">The target slice to get the element from.</param>
    /// <param name="row">The target row to get the element from.</param>
    /// <param name="column">The target column to get the element from.</param>
    /// <returns>A reference to the element at the specified indices.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when either <paramref name="slice"/>, <paramref name="row"/>, or <paramref name="column"/> are invalid.
    /// </exception>
    public ref readonly T this[int slice, int row, int column]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)slice >= (uint)Depth ||
                (uint)row >= (uint)this.height ||
                (uint)column >= (uint)this.width)
            {
                ThrowHelper.ThrowIndexOutOfRangeException();
            }

            return ref DangerousGetReferenceAt(slice, row, column);
        }
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Gets the element at the specified zero-based indices.
    /// </summary>
    /// <param name="slice">The target slice to get the element from.</param>
    /// <param name="row">The target row to get the element from.</param>
    /// <param name="column">The target column to get the element from.</param>
    /// <returns>A reference to the element at the specified indices.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when either <paramref name="slice"/>, <paramref name="row"/>, or <paramref name="column"/> are invalid.
    /// </exception>
    public ref readonly T this[Index slice, Index row, Index column]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this[slice.GetOffset(Depth), row.GetOffset(this.height), column.GetOffset(this.width)];
    }

    /// <summary>
    /// Slices the current instance with the specified parameters.
    /// </summary>
    /// <param name="slices">The target range of slices to select.</param>
    /// <param name="rows">The target range of rows to select.</param>
    /// <param name="columns">The target range of columns to select.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when either <paramref name="slices"/>, <paramref name="rows"/>, or <paramref name="columns"/> are invalid.
    /// </exception>
    /// <returns>A new <see cref="ReadOnlySpan3D{T}"/> instance representing a slice of the current one.</returns>
    public ReadOnlySpan3D<T> this[Range slices, Range rows, Range columns]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            (int slice, int depth) = slices.GetOffsetAndLength(Depth);
            (int row, int height) = rows.GetOffsetAndLength(this.height);
            (int column, int width) = columns.GetOffsetAndLength(this.width);

            return Slice(slice, row, column, depth, height, width);
        }
    }
#endif

    /// <summary>
    /// Copies the contents of this <see cref="ReadOnlySpan3D{T}"/> into a destination <see cref="Span{T}"/> instance.
    /// </summary>
    /// <param name="destination">The destination <see cref="Span{T}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination" /> is shorter than the source <see cref="ReadOnlySpan3D{T}"/> instance.
    /// </exception>
    public void CopyTo(Span<T> destination)
    {
        if (IsEmpty)
        {
            return;
        }

        if (TryGetSpan(out ReadOnlySpan<T> span))
        {
            span.CopyTo(destination);
        }
        else
        {
            if (Length > destination.Length)
            {
                ThrowHelper.ThrowArgumentExceptionForDestinationTooShort();
            }

            int depth = Depth;
            int offset = 0;

            if (this.SliceStride == this.height * this.RowStride)
            {
                // Copy one slice at a time
                for (int z = 0; z < depth; z++)
                {
                    GetSliceSpan(z).CopyTo(destination.Slice(offset));
                    offset += this.height * this.width;
                }
            }
            else
            {
                // Copy each row individually
#if NETSTANDARD2_1_OR_GREATER
                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < this.height; y++)
                    {
                        GetRowSpan(z, y).CopyTo(destination.Slice(offset));
                        offset += this.width;
                    }
                }
#else
                nint height = (nint)(uint)this.height;
                nint width = (nint)(uint)this.width;

                ref T destinationRef = ref MemoryMarshal.GetReference(destination);

                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        ref T sourceStart = ref DangerousGetReferenceAt(z, y, 0);
                        ref T sourceEnd = ref Unsafe.Add(ref sourceStart, width);

                        while (Unsafe.IsAddressLessThan(ref sourceStart, ref sourceEnd))
                        {
                            destinationRef = sourceStart;

                            sourceStart = ref Unsafe.Add(ref sourceStart, 1);
                            destinationRef = ref Unsafe.Add(ref destinationRef, 1);
                        }
                    }
                }
#endif
            }
        }
    }

    /// <summary>
    /// Copies the contents of this <see cref="ReadOnlySpan3D{T}"/> into a destination <see cref="Span3D{T}"/> instance.
    /// For this API to succeed, the target <see cref="Span3D{T}"/> has to have the same shape as the current one.
    /// </summary>
    /// <param name="destination">The destination <see cref="Span3D{T}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination" /> does not have the same shape as the source <see cref="ReadOnlySpan3D{T}"/> instance.
    /// </exception>
    public void CopyTo(Span3D<T> destination)
    {
        if (destination.Depth != Depth ||
            destination.Height != this.height ||
            destination.Width != this.width)
        {
            ThrowHelper.ThrowArgumentExceptionForDestinationWithNotSameShape();
        }

        if (IsEmpty)
        {
            return;
        }

        if (destination.TryGetSpan(out Span<T> span))
        {
            CopyTo(span);
        }
        else
        {
            int depth = Depth;

            if (this.SliceStride == this.height * this.RowStride &&
                destination.SliceStride == destination.Height * destination.RowStride)
            {
                for (int z = 0; z < depth; z++)
                {
                    GetSliceSpan(z).CopyTo(destination.GetSliceSpan(z));
                }
            }
            else
            {
                // Copy each row individually
#if NETSTANDARD2_1_OR_GREATER
                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < this.height; y++)
                    {
                        GetRowSpan(z, y).CopyTo(destination.GetRowSpan(z, y));
                    }
                }
#else
                nint height = (nint)(uint)this.height;
                nint width = (nint)(uint)this.width;

                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        ref T sourceStart = ref DangerousGetReferenceAt(z, y, 0);
                        ref T sourceEnd = ref Unsafe.Add(ref sourceStart, width);
                        ref T destinationRef = ref destination.DangerousGetReferenceAt(z, y, 0);

                        while (Unsafe.IsAddressLessThan(ref sourceStart, ref sourceEnd))
                        {
                            destinationRef = sourceStart;

                            sourceStart = ref Unsafe.Add(ref sourceStart, 1);
                            destinationRef = ref Unsafe.Add(ref destinationRef, 1);
                        }
                    }
                }
#endif
            }
        }
    }

    /// <summary>
    /// Attempts to copy the current <see cref="ReadOnlySpan3D{T}"/> instance to a destination <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The target <see cref="Span{T}"/> of the copy operation.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyTo(Span<T> destination)
    {
        if (destination.Length >= Length)
        {
            CopyTo(destination);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to copy the current <see cref="ReadOnlySpan3D{T}"/> instance to a destination <see cref="Span3D{T}"/>.
    /// </summary>
    /// <param name="destination">The target <see cref="Span3D{T}"/> of the copy operation.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyTo(Span3D<T> destination)
    {
        if (destination.Depth == Depth &&
            destination.Height == this.height &&
            destination.Width == this.width)
        {
            CopyTo(destination);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns a reference to the 0th element of the <see cref="ReadOnlySpan3D{T}"/> instance. If the current
    /// instance is empty, returns a <see langword="null"/> reference. It can be used for pinning
    /// and is required to support the use of span within a fixed statement.
    /// </summary>
    /// <returns>A reference to the 0th element, or a <see langword="null"/> reference.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public unsafe ref readonly T GetPinnableReference()
    {
        ref readonly T r0 = ref Unsafe.AsRef<T>(null);

        if (Length != 0)
        {
#if NET8_0_OR_GREATER
            r0 = ref this.reference;
#elif NETSTANDARD2_1_OR_GREATER
            r0 = ref MemoryMarshal.GetReference(this.span);
#else
            r0 = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(this.instance, this.offset);
#endif
        }

        return ref r0;
    }

    /// <summary>
    /// Returns a reference to the first element within the current instance, with no bounds check.
    /// </summary>
    /// <returns>A reference to the first element within the current instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T DangerousGetReference()
    {
#if NET8_0_OR_GREATER
        return ref Unsafe.AsRef(in this.reference);
#elif NETSTANDARD2_1_OR_GREATER
        return ref MemoryMarshal.GetReference(this.span);
#else
        return ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(this.instance, this.offset);
#endif
    }

    /// <summary>
    /// Returns a reference to a specified element within the current instance, with no bounds check.
    /// </summary>
    /// <param name="i">The target slice to get the element from.</param>
    /// <param name="j">The target row to get the element from.</param>
    /// <param name="k">The target column to get the element from.</param>
    /// <returns>A reference to the element at the specified indices.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T DangerousGetReferenceAt(int i, int j, int k)
    {
#if NET8_0_OR_GREATER
        ref T r0 = ref Unsafe.AsRef(in this.reference);
#elif NETSTANDARD2_1_OR_GREATER
        ref T r0 = ref MemoryMarshal.GetReference(this.span);
#else
        ref T r0 = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(this.instance, this.offset);
#endif
        nint index = ((nint)(uint)i * (nint)(uint)this.SliceStride) +
                     ((nint)(uint)j * (nint)(uint)this.RowStride) +
                     (nint)(uint)k;

        return ref Unsafe.Add(ref r0, index);
    }

    /// <summary>
    /// Slices the current instance with the specified parameters.
    /// </summary>
    /// <param name="slice">The target slice to map within the current instance.</param>
    /// <param name="row">The target row to map within the current instance.</param>
    /// <param name="column">The target column to map within the current instance.</param>
    /// <param name="depth">The depth to map within the current instance.</param>
    /// <param name="height">The height to map within the current instance.</param>
    /// <param name="width">The width to map within the current instance.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when either <paramref name="slice"/>, <paramref name="row"/>, or <paramref name="column"/>,
    /// <paramref name="depth"/>, <paramref name="height"/>, or <paramref name="width"/>
    /// are negative or not within the bounds that are valid for the current instance.
    /// </exception>
    /// <returns>A new <see cref="ReadOnlySpan3D{T}"/> instance representing a slice of the current one.</returns>
    /// <remarks>
    /// <para>
    /// Contrary to <see cref="Span{T}.Slice(int, int)"/>, this method will throw an <see cref="ArgumentOutOfRangeException"/>
    /// if attempting to perform a slice operation that would result in either axes being 0. That is, trying to call
    /// <see cref="Slice(int, int, int, int, int, int)"/> as e.g. <c>Slice(slice: 2, row: 1, column: 0, depth: 1, height: 0, width: 2)</c>
    /// on an instance that has 3 slices, 2 rows, and 1 column will throw, rather than returning a new <see cref="ReadOnlySpan3D{T}"/>
    /// instance with 1 slice, 0 rows and 2 columns. For contrast, trying to e.g. call <c>Slice(start: 1, length: 0)</c>
    /// on a <see cref="Span{T}"/> instance of length 1 would return a span of length 0, with the internal reference being
    /// set to right past the end of the memory.
    /// </para>
    /// <para>
    /// This is by design, and it is due to the internal memory layout that <see cref="ReadOnlySpan3D{T}"/> has. That is, in the case
    /// of <see cref="Span{T}"/>, the only edge case scenario would be to obtain a new span of size 0, referencing the very end
    /// of the backing object (e.g. an array or a <see cref="string"/> instance). In that case, the GC can correctly track things.
    /// With <see cref="ReadOnlySpan3D{T}"/>, on the other hand, it would be possible to slice an instance with a size of 0 in either axis,
    /// but with the computed starting reference pointing well past the end of the internal memory area. Such a behavior would not
    /// be valid if the reference was pointing to a managed object, and it would cause memory corruptions (i.e. "GC holes").
    /// </para>
    /// <para>
    /// If you specifically need to be able to obtain empty values from slicing past the valid range, consider performing the range
    /// validation yourself (i.e. through some helper method), and then only invoking <see cref="Slice(int, int, int, int, int, int)"/>
    /// once the parameters are in the accepted range. Otherwise, consider returning another return explicitly, such as
    /// <see cref="Empty"/>.
    /// </para>
    /// </remarks>
    public unsafe ReadOnlySpan3D<T> Slice(int slice, int row, int column, int depth, int height, int width)
    {
        if ((uint)slice >= Depth)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlice();
        }

        if ((uint)row >= this.height)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
        }

        if ((uint)column >= this.width)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForColumn();
        }

        if ((uint)depth > (Depth - slice) || depth == 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
        }

        if ((uint)height > (this.height - row) || height == 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
        }

        if ((uint)width > (this.width - column) || width == 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
        }

        nint shift = ((nint)(uint)slice * (nint)(uint)this.SliceStride) +
                     ((nint)(uint)row * (nint)(uint)this.RowStride) +
                     (nint)(uint)column;

        int rowPitch = this.RowStride - width;
        int slicePitch = this.SliceStride - (height * this.RowStride);

#if NET8_0_OR_GREATER
        ref T r0 = ref Unsafe.Add(ref Unsafe.AsRef(in this.reference), shift);

        return new(in r0, depth, height, width, slicePitch, rowPitch);
#elif NETSTANDARD2_1_OR_GREATER
        ref T r0 = ref this.span.DangerousGetReferenceAt(shift);

        return new(in r0, depth, height, width, slicePitch, rowPitch);
#else
        IntPtr offset = this.offset + (shift * (nint)(uint)sizeof(T));

        return new(this.instance, offset, depth, height, width, slicePitch, rowPitch);
#endif
    }

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan2D{T}"/> for a specified slice.
    /// </summary>
    /// <param name="slice">The index of the target slice to retrieve.</param>
    /// <exception cref="ArgumentOutOfRangeException">Throw when <paramref name="slice"/> is out of range.</exception>
    /// <returns>The resulting slice <see cref="ReadOnlySpan2D{T}"/>.</returns>
    public unsafe ReadOnlySpan2D<T> GetSliceSpan(int slice)
    {
        if ((uint)slice >= (uint)Depth)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlice();
        }

        int rowPitch = this.RowStride - this.width;

#if NETSTANDARD2_1_OR_GREATER
        ref T r0 = ref DangerousGetReferenceAt(slice, 0, 0);

        return new(in r0, this.height, this.width, rowPitch);
#else
        nint shift = (nint)(uint)slice * (nint)(uint)this.SliceStride;
        IntPtr offset = this.offset + (shift * (nint)(uint)sizeof(T));

        return new(this.instance, offset, this.height, this.width, rowPitch);
#endif
    }

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> for a specified row in a specific slice.
    /// </summary>
    /// <param name="slice">The index of the target slice to retrieve.</param>
    /// <param name="row">The index of the target row in <paramref name="slice"/> to retrieve.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Throw when either <paramref name="slice"/> or <paramref name="row"/> are out of range.
    /// </exception>
    /// <returns>The resulting row <see cref="ReadOnlySpan{T}"/>.</returns>
    public unsafe ReadOnlySpan<T> GetRowSpan(int slice, int row)
    {
        if ((uint)slice >= (uint)Depth)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlice();
        }

        if ((uint)row >= (uint)this.height)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
        }

#if NETSTANDARD2_1_OR_GREATER
        ref T r0 = ref DangerousGetReferenceAt(slice, row, 0);

        return MemoryMarshal.CreateReadOnlySpan(ref r0, this.width);
#else
        nint shift = ((nint)(uint)slice * (nint)(uint)this.SliceStride) +
                     ((nint)(uint)row * (nint)(uint)this.RowStride);

        if (this.instance is null)
        {
            return new ReadOnlySpan<T>((void*)(this.offset + (shift * (nint)(uint)sizeof(T))), this.width);
        }

        if (this.instance.GetType() == typeof(T[]))
        {
            T[] array = Unsafe.As<T[]>(this.instance)!;
            ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, this.offset + (shift * (nint)(uint)sizeof(T)));
            int index = array.AsSpan().IndexOf(ref r0);

            return array.AsSpan(index, this.width);
        }

        throw new NotSupportedException("GetRowSpan is not supported for this backing store on NETSTANDARD2.0.");
#endif
    }

    /// <summary>
    /// Tries to get a <see cref="ReadOnlySpan{T}"/> instance, if the underlying buffer is contiguous and small enough.
    /// </summary>
    /// <param name="span">The resulting <see cref="ReadOnlySpan{T}"/>, in case of success.</param>
    /// <returns>Whether or not <paramref name="span"/> was correctly assigned.</returns>
    public bool TryGetSpan(out ReadOnlySpan<T> span)
    {
        // We can only create a ReadOnlySpan<T> if the buffer is contiguous
        if (this.RowStride == this.width &&
            this.SliceStride == (this.width * this.height) &&
            Length <= int.MaxValue)
        {
#if NET8_0_OR_GREATER
            span = MemoryMarshal.CreateReadOnlySpan(in this.reference, (int)Length);

            return true;
#elif NETSTANDARD2_1_OR_GREATER
            span = MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(this.span), (int)Length);

            return true;
#else
            // An empty ReadOnlySpan3D<T> is still valid
            if (IsEmpty)
            {
                span = default;

                return true;
            }

            // Pinned ReadOnlySpan3D<T>
            if (this.instance is null)
            {
                unsafe
                {
                    span = new ReadOnlySpan<T>((void*)this.offset, (int)Length);
                }

                return true;
            }

            // Without ReadOnlySpan<T> runtime support, we can only get a ReadOnlySpan<T> from a T[] instance
            if (this.instance.GetType() == typeof(T[]))
            {
                T[] array = Unsafe.As<T[]>(this.instance)!;
                int index = array.AsSpan().IndexOf(ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, this.offset));

                span = array.AsSpan(index, (int)Length);

                return true;
            }
#endif
        }

        span = default;

        return false;
    }

    /// <summary>
    /// Copies the contents of the current <see cref="ReadOnlySpan3D{T}"/> instance into a new 3D array.
    /// </summary>
    /// <returns>A 3D array containing the data in the current <see cref="ReadOnlySpan3D{T}"/> instance.</returns>
    public T[,,] ToArray()
    {
        T[,,] array = new T[Depth, this.height, this.width];

#if NETSTANDARD2_1_OR_GREATER
        CopyTo(array.AsSpan());
#else
        // Skip the initialization if the array is empty
        if (Length > 0)
        {
            int depth = Depth;
            nint height = (nint)(uint)this.height;
            nint width = (nint)(uint)this.width;

            ref T destinationRef = ref array.DangerousGetReference();

            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    ref T sourceStart = ref DangerousGetReferenceAt(z, y, 0);
                    ref T sourceEnd = ref Unsafe.Add(ref sourceStart, width);

                    while (Unsafe.IsAddressLessThan(ref sourceStart, ref sourceEnd))
                    {
                        destinationRef = sourceStart;

                        sourceStart = ref Unsafe.Add(ref sourceStart, 1);
                        destinationRef = ref Unsafe.Add(ref destinationRef, 1);
                    }
                }
            }
        }
#endif

        return array;
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.Equals(object)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals() on Span will always throw an exception. Use == instead.")]
    public override bool Equals(object? obj)
    {
        throw new NotSupportedException("CommunityToolkit.HighPerformance.ReadOnlySpan3D<T>.Equals(object) is not supported.");
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.GetHashCode()"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on Span will always throw an exception.")]
    public override int GetHashCode()
    {
        throw new NotSupportedException("CommunityToolkit.HighPerformance.ReadOnlySpan3D<T>.GetHashCode() is not supported.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"CommunityToolkit.HighPerformance.ReadOnlySpan3D<{typeof(T)}>[{Depth}, {this.height}, {this.width}]";
    }

    /// <summary>
    /// Checks whether two <see cref="ReadOnlySpan3D{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="ReadOnlySpan3D{T}"/> instance to compare.</param>
    /// <param name="right">The second <see cref="ReadOnlySpan3D{T}"/> instance to compare.</param>
    /// <returns>Whether or not <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
    public static bool operator ==(ReadOnlySpan3D<T> left, ReadOnlySpan3D<T> right)
    {
        return
#if NET8_0_OR_GREATER
            Unsafe.AreSame(ref Unsafe.AsRef(in left.reference), ref Unsafe.AsRef(in right.reference)) &&
            left.depth == right.depth &&
#elif NETSTANDARD2_1_OR_GREATER
            left.span == right.span &&
#else
            ReferenceEquals(
                left.instance, right.instance) &&
                left.offset == right.offset &&
                left.depth == right.depth &&
#endif
            left.height == right.height &&
            left.width == right.width &&
            left.RowStride == right.RowStride &&
            left.SliceStride == right.SliceStride;
    }

    /// <summary>
    /// Checks whether two <see cref="ReadOnlySpan3D{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="ReadOnlySpan3D{T}"/> instance to compare.</param>
    /// <param name="right">The second <see cref="ReadOnlySpan3D{T}"/> instance to compare.</param>
    /// <returns>Whether or not <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
    public static bool operator !=(ReadOnlySpan3D<T> left, ReadOnlySpan3D<T> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Implicitly converts a given 3D array into a <see cref="ReadOnlySpan3D{T}"/> instance.
    /// </summary>
    /// <param name="array">The input 3D array to convert.</param>
    public static implicit operator ReadOnlySpan3D<T>(T[,,]? array) => new(array);

    /// <summary>
    /// Implicitly converts a given <see cref="Span3D{T}"/> into a <see cref="ReadOnlySpan3D{T}"/> instance.
    /// </summary>
    /// <param name="span">The input <see cref="Span3D{T}"/> to convert.</param>
    public static implicit operator ReadOnlySpan3D<T>(Span3D<T> span)
    {
        int rowPitch = span.RowStride - span.Width;
        int slicePitch = span.SliceStride - (rowPitch * span.Height);

#if NETSTANDARD2_1_OR_GREATER
        return new(in span.DangerousGetReference(), span.Depth, span.Height, span.Width, slicePitch, rowPitch);
#else
        return new(span.Instance!, span.Offset, span.Depth, span.Height, span.Width, slicePitch, rowPitch);
#endif
    }
}
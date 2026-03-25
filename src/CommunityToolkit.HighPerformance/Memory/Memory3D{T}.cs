// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if NETSTANDARD2_1_OR_GREATER
using CommunityToolkit.HighPerformance.Buffers.Internals;
#endif
using CommunityToolkit.HighPerformance.Helpers;
using CommunityToolkit.HighPerformance.Memory.Internals;
using CommunityToolkit.HighPerformance.Memory.Views;
using static CommunityToolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;

#pragma warning disable CA2231

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// <see cref="Memory3D{T}"/> represents a 3D region of arbitrary memory. It is to <see cref="Span3D{T}"/>
/// what <see cref="Memory{T}"/> is to <see cref="Span{T}"/>. For further details on how the internal layout
/// is structured, see the docs for <see cref="Span3D{T}"/>. The <see cref="Memory3D{T}"/> type can wrap arrays
/// of any rank, provided that a valid series of parameters for the target memory area(s) are specified.
/// </summary>
/// <typeparam name="T">The type of items in the current <see cref="Memory3D{T}"/> instance.</typeparam>
[DebuggerTypeProxy(typeof(MemoryDebugView3D<>))]
[DebuggerDisplay("{ToString(),raw}")]
public readonly struct Memory3D<T> : IEquatable<Memory3D<T>>
{
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

    /// <summary>
    /// The height of the specified 3D region.
    /// </summary>
    private readonly int height;

    /// <summary>
    /// The width of the specified 3D region.
    /// </summary>
    private readonly int width;

    /// <summary>
    /// The pitch of each row in the specified 3D region.
    /// </summary>
    private readonly int rowPitch;

    /// <summary>
    /// The pitch of each slice in the specified 3D region.
    /// </summary>
    private readonly int slicePitch;

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct.
    /// </summary>
    /// <param name="array">The target array to wrap.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <exception cref="ArrayTypeMismatchException">
    /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when either <paramref name="depth"/>, <paramref name="height"/> or <paramref name="width"/> are invalid.
    /// </exception>
    /// <remarks>The total volume must match the length of <paramref name="array"/>.</remarks>
    public Memory3D(T[] array, int depth, int height, int width)
        : this(array, 0, depth, height, width, slicePitch: 0, rowPitch: 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct.
    /// </summary>
    /// <param name="array">The target array to wrap.</param>
    /// <param name="offset">The initial offset within <paramref name="array"/>.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <param name="rowPitch">The pitch of each row in the resulting 3D area.</param>
    /// <param name="slicePitch">The pitch of each slice in the resulting 3D area.</param>
    /// <exception cref="ArrayTypeMismatchException">
    /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when one of the input parameters is out of range.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the requested volume is outside of bounds for <paramref name="array"/>.
    /// </exception>
    public Memory3D(T[] array, int offset, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        if (array.IsCovariant())
        {
            ThrowHelper.ThrowArrayTypeMismatchException();
        }

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

        if (rowPitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRowPitch();
        }

        if (slicePitch < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlicePitch();
        }

        int volume = OverflowHelper.ComputeInt32Volume(depth, height, width, slicePitch, rowPitch);
        int remaining = array.Length - offset;

        if (volume > remaining)
        {
            ThrowHelper.ThrowArgumentException();
        }

        this.instance = array;
        this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(offset));
        this.depth = depth;
        this.height = height;
        this.width = width;
        this.rowPitch = rowPitch;
        this.slicePitch = slicePitch;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct wrapping a 3D array.
    /// </summary>
    /// <param name="array">The given 3D array to wrap.</param>
    /// <exception cref="ArrayTypeMismatchException">
    /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
    /// </exception>
    public Memory3D(T[,,]? array)
    {
        if (array is null)
        {
            this = default;

            return;
        }

        if (array.IsCovariant())
        {
            ThrowHelper.ThrowArrayTypeMismatchException();
        }

        this.instance = array;
        this.offset = GetArray3DDataByteOffset<T>();
        this.depth = array.GetLength(0);
        this.height = array.GetLength(1);
        this.width = array.GetLength(2);
        this.rowPitch = 0;
        this.slicePitch = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct wrapping a 3D array.
    /// </summary>
    /// <param name="array">The given 3D array to wrap.</param>
    /// <param name="slice">The target slice to map within <paramref name="array"/>.</param>
    /// <param name="row">The target row to map within <paramref name="array"/>.</param>
    /// <param name="column">The target column to map within <paramref name="array"/>.</param>
    /// <param name="depth">The depth to map within <paramref name="array"/>.</param>
    /// <param name="height">The height to map within <paramref name="array"/>.</param>
    /// <param name="width">The width to map within <paramref name="array"/>.</param>
    /// <exception cref="ArrayTypeMismatchException">
    /// Thrown when <paramref name="array"/> doesn't match <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when either <paramref name="slice"/>, <paramref name="row"/>, or <paramref name="column"/>,
    /// <paramref name="depth"/>, <paramref name="height"/>, or <paramref name="width"/>
    /// are negative or not within the bounds that are valid for <paramref name="array"/>.
    /// </exception>
    public Memory3D(T[,,]? array, int slice, int row, int column, int depth, int height, int width)
    {
        if (array is null)
        {
            if (slice != 0 || row != 0 || column != 0 || depth != 0 || height != 0 || width != 0)
            {
                ThrowHelper.ThrowArgumentException();
            }

            this = default;

            return;
        }

        if (array.IsCovariant())
        {
            ThrowHelper.ThrowArrayTypeMismatchException();
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

        this.instance = array;
        this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(slice, row, column));
        this.depth = depth;
        this.height = height;
        this.width = width;
        this.slicePitch = slices - depth;
        this.rowPitch = columns - width;
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct.
    /// </summary>
    /// <param name="memoryManager">The target <see cref="MemoryManager{T}"/> to wrap.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when either <paramref name="depth"/>, <paramref name="height"/>, or <paramref name="width"/> are invalid.
    /// </exception>
    /// <remarks>The total volume must match the length of <paramref name="memoryManager"/>.</remarks>
    public Memory3D(MemoryManager<T> memoryManager, int depth, int height, int width)
        : this(memoryManager, offset: 0, depth, height, width, slicePitch: 0, rowPitch: 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct.
    /// </summary>
    /// <param name="memoryManager">The target <see cref="MemoryManager{T}"/> to wrap.</param>
    /// <param name="offset">The initial offset within <paramref name="memoryManager"/>.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <param name="rowPitch">The pitch of each row in the resulting 3D area.</param>
    /// <param name="slicePitch">The pitch of each slice in the resulting 3D area.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when one of the input parameters is out of range.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the requested volume is outside of bounds for <paramref name="memoryManager"/>.
    /// </exception>
    public unsafe Memory3D(MemoryManager<T> memoryManager, int offset, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        int length = memoryManager.GetSpan().Length;

        if ((uint)offset > (uint)length)
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
        int remaining = length - offset;

        if (volume > remaining)
        {
            ThrowHelper.ThrowArgumentException();
        }

        this.instance = memoryManager;
        this.offset = (nint)(uint)offset * (nint)(uint)sizeof(T);
        this.depth = depth;
        this.height = height;
        this.width = width;
        this.slicePitch = slicePitch;
        this.rowPitch = rowPitch;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct.
    /// </summary>
    /// <param name="memory">The target <see cref="Memory{T}"/> to wrap.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when either <paramref name="depth"/>, <paramref name="height"/>, or <paramref name="width"/> are invalid.
    /// </exception>
    /// <remarks>The total volume must match the length of <paramref name="memory"/>.</remarks>
    internal Memory3D(Memory<T> memory, int depth, int height, int width)
        : this(memory, offset: 0, depth, height, width, slicePitch: 0, rowPitch: 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct.
    /// </summary>
    /// <param name="memory">The target <see cref="Memory{T}"/> to wrap.</param>
    /// <param name="offset">The initial offset within <paramref name="memory"/>.</param>
    /// <param name="depth">The depth of the resulting 3D area.</param>
    /// <param name="height">The height of each slice in the resulting 3D area.</param>
    /// <param name="width">The width of each row in the resulting 3D area.</param>
    /// <param name="slicePitch">The pitch of each slice in the resulting 3D area.</param>
    /// <param name="rowPitch">The pitch of each row in the resulting 3D area.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when one of the input parameters is out of range.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the requested volume is outside of bounds for <paramref name="memory"/>.
    /// </exception>
    internal unsafe Memory3D(Memory<T> memory, int offset, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        if ((uint)offset > (uint)memory.Length)
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
        int remaining = memory.Length - offset;

        if (volume > remaining)
        {
            ThrowHelper.ThrowArgumentException();
        }

        // Check if the Memory<T> instance wraps a string. This is possible in case
        // consumers do an unsafe cast for the entire Memory<T> object, and while not
        // really safe it is still supported in CoreCLR too, so we're following suit here.
        if (typeof(T) == typeof(char) &&
            MemoryMarshal.TryGetString(Unsafe.As<Memory<T>, Memory<char>>(ref memory), out string? text, out int textStart, out _))
        {
            ref char r0 = ref text.DangerousGetReferenceAt(textStart + offset);

            this.instance = text;
            this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(text, ref r0);
        }
        else if (MemoryMarshal.TryGetArray(memory, out ArraySegment<T> segment))
        {
            // Check if the input Memory<T> instance wraps an array we can access.
            // This is fine, since Memory<T> on its own doesn't control the lifetime
            // of the underlying array anyway, and this Memory3D<T> type would do the same.
            // Using the array directly makes retrieving a Span3D<T> faster down the line,
            // as we no longer have to jump through the boxed Memory<T> first anymore.
            T[] array = segment.Array!;

            this.instance = array;
            this.offset = ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array.DangerousGetReferenceAt(segment.Offset + offset));
        }
        else if (MemoryMarshal.TryGetMemoryManager<T, MemoryManager<T>>(memory, out MemoryManager<T>? memoryManager, out int memoryManagerStart, out _))
        {
            this.instance = memoryManager;
            this.offset = (nint)(uint)(memoryManagerStart + offset) * (nint)(uint)sizeof(T);
        }
        else
        {
            ThrowHelper.ThrowArgumentExceptionForUnsupportedType();

            this.instance = null;
            this.offset = default;
        }

        this.depth = depth;
        this.height = height;
        this.width = width;
        this.slicePitch = slicePitch;
        this.rowPitch = rowPitch;
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory3D{T}"/> struct with the specified parameters.
    /// </summary>
    /// <param name="instance">The target <see cref="object"/> instance.</param>
    /// <param name="offset">The initial offset within <see cref="instance"/>.</param>
    /// <param name="depth">The depth of the 3D memory area to map.</param>
    /// <param name="height">The height of the 3D memory area to map.</param>
    /// <param name="width">The width of the 3D memory area to map.</param>
    /// <param name="slicePitch">The pitch of each slice in the 3D memory area to map.</param>
    /// <param name="rowPitch">The pitch of each row in the 3D memory area to map.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Memory3D(object instance, IntPtr offset, int depth, int height, int width, int slicePitch, int rowPitch)
    {
        this.instance = instance;
        this.offset = offset;
        this.depth = depth;
        this.height = height;
        this.width = width;
        this.slicePitch = slicePitch;
        this.rowPitch = rowPitch;
    }

    /// <summary>
    /// Creates a new <see cref="Memory3D{T}"/> instance from an arbitrary object.
    /// </summary>
    /// <param name="instance">The <see cref="object"/> instance holding the data to map.</param>
    /// <param name="value">The target reference to point to (it must be within <paramref name="instance"/>).</param>
    /// <param name="depth">The depth of the 3D memory area to map.</param>
    /// <param name="height">The height of the 3D memory area to map.</param>
    /// <param name="width">The width of the 3D memory area to map.</param>
    /// <param name="slicePitch">The pitch of each slice in the 3D memory area to map.</param>
    /// <param name="rowPitch">The pitch of each row in the 3D memory area to map.</param>
    /// <returns>A <see cref="Memory3D{T}"/> instance with the specified parameters.</returns>
    /// <remarks>The <paramref name="value"/> parameter is not validated, and it's responsibility of the caller to ensure it's valid.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when one of the input parameters is out of range.
    /// </exception>
    public static Memory3D<T> DangerousCreate(object instance, ref T value, int depth, int height, int width, int slicePitch, int rowPitch)
    {
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

        OverflowHelper.EnsureIsInNativeIntRange(depth, height, width, slicePitch, rowPitch);

        IntPtr offset = ObjectMarshal.DangerousGetObjectDataByteOffset(instance, ref value);

        return new(instance, offset, depth, height, width, slicePitch, rowPitch);
    }

    /// <summary>
    /// Gets an empty <see cref="Memory3D{T}"/> instance.
    /// </summary>
    public static Memory3D<T> Empty => default;

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Memory3D{T}"/> instance is empty.
    /// </summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.depth == 0 || this.height == 0 || this.width == 0;
    }

    /// <summary>
    /// Gets the length of the current <see cref="Memory3D{T}"/> instance.
    /// </summary>
    public nint Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (nint)(uint)this.depth * (nint)(uint)this.height * (nint)(uint)this.width;
    }

    /// <summary>
    /// Gets the depth of the underlying 3D memory area.
    /// </summary>
    public int Depth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.depth;
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
    /// Gets a <see cref="Span3D{T}"/> instance from the current memory.
    /// </summary>
    public Span3D<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (this.instance is not null)
            {
#if NETSTANDARD2_1_OR_GREATER
                if (this.instance is MemoryManager<T> memoryManager)
                {
                    ref T r0 = ref memoryManager.GetSpan().DangerousGetReference();
                    ref T r1 = ref Unsafe.AddByteOffset(ref r0, this.offset);

                    return new(ref r1, this.depth, this.height, this.width, this.slicePitch, this.rowPitch);
                }
                else
                {
                    ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(this.instance, this.offset);

                    return new(ref r0, this.depth, this.height, this.width, this.slicePitch, this.rowPitch);
                }
#else
                return new(this.instance, this.offset, this.depth, this.height, this.width, this.slicePitch, this.rowPitch);
#endif
            }

            return default;
        }
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Slices the current instance with the specified parameters.
    /// </summary>
    /// <param name="slices">The target range of slices to select.</param>
    /// <param name="rows">The target range of rows to select.</param>
    /// <param name="columns">The target range of columns to select.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when either <paramref name="slices"/>, <paramref name="rows"/>, or <paramref name="columns"/> are invalid.
    /// </exception>
    /// <returns>A new <see cref="Memory3D{T}"/> instance representing a slice of the current one.</returns>
    public Memory3D<T> this[Range slices, Range rows, Range columns]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            (int slice, int depth) = slices.GetOffsetAndLength(this.depth);
            (int row, int height) = rows.GetOffsetAndLength(this.height);
            (int column, int width) = columns.GetOffsetAndLength(this.width);

            return Slice(slice, row, column, depth, height, width);
        }
    }
#endif

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
    /// Thrown when either <paramref name="slice"/>, <paramref name="row"/>, <paramref name="column"/>,
    /// <paramref name="depth"/>, <paramref name="height"/>, or <paramref name="width"/>
    /// are negative or not within the bounds that are valid for the current instance.
    /// </exception>
    /// <returns>A new <see cref="Memory3D{T}"/> instance representing a slice of the current one.</returns>
    /// <remarks>See additional remarks in the <see cref="Span3D{T}.Slice(int, int, int, int, int, int)"/> docs.</remarks>
    public unsafe Memory3D<T> Slice(int slice, int row, int column, int depth, int height, int width)
    {
        if ((uint)slice >= this.depth)
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

        if ((uint)depth > (this.depth - slice))
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForDepth();
        }

        if ((uint)height > (this.height - row))
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForHeight();
        }

        if ((uint)width > (this.width - column))
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForWidth();
        }

        int shift = (slice * ((this.width + this.rowPitch) * this.height + this.slicePitch)) +
                    (row * (this.width + this.rowPitch)) +
                    column;

        int rowPitch = this.rowPitch + (this.width - width);
        int slicePitch = this.slicePitch + ((this.width + this.rowPitch) * (this.height - height));

        IntPtr offset = this.offset + (shift * sizeof(T));

        return new(this.instance!, offset, depth, height, width, slicePitch, rowPitch);
    }

    /// <summary>
    /// Copies the contents of this <see cref="Memory3D{T}"/> into a destination <see cref="Memory{T}"/> instance.
    /// </summary>
    /// <param name="destination">The destination <see cref="Memory{T}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination" /> is shorter than the source <see cref="Memory3D{T}"/> instance.
    /// </exception>
    public void CopyTo(Memory<T> destination) => Span.CopyTo(destination.Span);

    /// <summary>
    /// Attempts to copy the current <see cref="Memory3D{T}"/> instance to a destination <see cref="Memory{T}"/>.
    /// </summary>
    /// <param name="destination">The target <see cref="Memory{T}"/> of the copy operation.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyTo(Memory<T> destination) => Span.TryCopyTo(destination.Span);

    /// <summary>
    /// Copies the contents of this <see cref="Memory3D{T}"/> into a destination <see cref="Memory3D{T}"/> instance.
    /// For this API to succeed, the target <see cref="Memory3D{T}"/> has to have the same shape as the current one.
    /// </summary>
    /// <param name="destination">The destination <see cref="Memory3D{T}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination" /> is shorter than the source <see cref="Memory3D{T}"/> instance.
    /// </exception>
    public void CopyTo(Memory3D<T> destination) => Span.CopyTo(destination.Span);

    /// <summary>
    /// Attempts to copy the current <see cref="Memory3D{T}"/> instance to a destination <see cref="Memory3D{T}"/>.
    /// For this API to succeed, the target <see cref="Memory3D{T}"/> has to have the same shape as the current one.
    /// </summary>
    /// <param name="destination">The target <see cref="Memory3D{T}"/> of the copy operation.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyTo(Memory3D<T> destination) => Span.TryCopyTo(destination.Span);

    /// <summary>
    /// Creates a handle for the memory.
    /// The GC will not move the memory until the returned <see cref="MemoryHandle"/>
    /// is disposed, enabling taking and using the memory's address.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// An instance with non-primitive (non-blittable) members cannot be pinned.
    /// </exception>
    /// <returns>A <see cref="MemoryHandle"/> instance wrapping the pinned handle.</returns>
    public unsafe MemoryHandle Pin()
    {
        if (this.instance is not null)
        {
            if (this.instance is MemoryManager<T> memoryManager)
            {
                return memoryManager.Pin();
            }

            GCHandle handle = GCHandle.Alloc(this.instance, GCHandleType.Pinned);

            void* pointer = Unsafe.AsPointer(ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(this.instance, this.offset));

            return new(pointer, handle);
        }

        return default;
    }

    /// <summary>
    /// Tries to get a <see cref="Memory{T}"/> instance, if the underlying buffer is contiguous and small enough.
    /// </summary>
    /// <param name="memory">The resulting <see cref="Memory{T}"/>, in case of success.</param>
    /// <returns>Whether or not <paramref name="memory"/> was correctly assigned.</returns>
    public bool TryGetMemory(out Memory<T> memory)
    {
        if (this.slicePitch == 0 &&
            this.rowPitch == 0 &&
            Length <= int.MaxValue)
        {
            // Empty Memory3D<T> instance
            if (this.instance is null)
            {
                memory = default;
            }
            else if (typeof(T) == typeof(char) && this.instance.GetType() == typeof(string))
            {
                // Here we need to create a Memory<char> from the wrapped string, and to do so we need to do an inverse
                // lookup to find the initial index of the string with respect to the byte offset we're currently using,
                // which refers to the raw string object data. This can include variable padding or other additional
                // fields on different runtimes. The lookup operation is still O(1) and just computes the byte offset
                // difference between the start of the Span<char> (which directly wraps just the actual character data
                // within the string), and the input reference, which we can get from the byte offset in use. The result
                // is the character index which we can use to create the final Memory<char> instance.
                string text = Unsafe.As<string>(this.instance)!;
                int index = text.AsSpan().IndexOf(in ObjectMarshal.DangerousGetObjectDataReferenceAt<char>(text, this.offset));
                ReadOnlyMemory<char> temp = text.AsMemory(index, (int)Length);

                // The string type could still be present if a user ends up creating a
                // Memory3D<T> instance from a string using DangerousCreate. Similarly to
                // how CoreCLR handles the equivalent case in Memory<T>, here we just do
                // the necessary steps to still retrieve a Memory<T> instance correctly
                // wrapping the target string. In this case, it is up to the caller
                // to make sure not to ever actually write to the resulting Memory<T>.
                memory = MemoryMarshal.AsMemory<T>(Unsafe.As<ReadOnlyMemory<char>, Memory<T>>(ref temp));
            }
            else if (this.instance is MemoryManager<T> memoryManager)
            {
                // If the object is a MemoryManager<T>, just slice it as needed
                memory = memoryManager.Memory.Slice((int)(nint)this.offset, this.depth * this.height * this.width);
            }
            else if (this.instance.GetType() == typeof(T[]))
            {
                // If it's a T[] array, also handle the initial offset
                T[] array = Unsafe.As<T[]>(this.instance)!;
                int index = array.AsSpan().IndexOf(ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, this.offset));

                memory = array.AsMemory(index, this.depth * this.height * this.width);
            }
#if NETSTANDARD2_1_OR_GREATER
            else if (this.instance.GetType() == typeof(T[,]) ||
                     this.instance.GetType() == typeof(T[,,]))
            {
                // If the object is a 2D or 3D array, we can create a Memory<T> from the RawObjectMemoryManager<T> type.
                // We just need to use the precomputed offset pointing to the first item in the current instance,
                // and the current usable length. We don't need to retrieve the current index, as the manager just offsets.
                memory = new RawObjectMemoryManager<T>(this.instance, this.offset, this.depth * this.height * this.width).Memory;
            }
#endif
            else
            {
                // Reuse a single failure path to reduce
                // the number of returns in the method
                goto Failure;
            }

            return true;
        }

        Failure:

        memory = default;

        return false;
    }

    /// <summary>
    /// Copies the contents of the current <see cref="Memory3D{T}"/> instance into a new 3D array.
    /// </summary>
    /// <returns>A 3D array containing the data in the current <see cref="Memory3D{T}"/> instance.</returns>
    public T[,,] ToArray() => Span.ToArray();

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj)
    {
        if (obj is Memory3D<T> memory)
        {
            return Equals(memory);
        }

        if (obj is ReadOnlyMemory3D<T> readOnlyMemory)
        {
            return readOnlyMemory.Equals(this);
        }

        return false;
    }

    /// <inheritdoc/>
    public bool Equals(Memory3D<T> other)
    {
        return
            this.instance == other.instance &&
            this.offset == other.offset &&
            this.depth == other.depth &&
            this.height == other.height &&
            this.width == other.width &&
            this.slicePitch == other.slicePitch &&
            this.rowPitch == other.rowPitch;
    }

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
        if (this.instance is not null)
        {
            return HashCode.Combine(
                RuntimeHelpers.GetHashCode(this.instance),
                this.offset,
                this.depth,
                this.height,
                this.width,
                this.slicePitch,
                this.rowPitch);
        }

        return 0;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"CommunityToolkit.HighPerformance.Memory3D<{typeof(T)}>[{this.depth}, {this.height}, {this.width}]";
    }

    /// <summary>
    /// Defines an implicit conversion of an array to a <see cref="Memory3D{T}"/>
    /// </summary>
    public static implicit operator Memory3D<T>(T[,,]? array) => new(array);
}
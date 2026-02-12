// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET8_0_OR_GREATER
using System;
#endif
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Enumerables;
using CommunityToolkit.HighPerformance.Memory.Internals;
#if NETSTANDARD2_1_OR_GREATER && !NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#elif NETSTANDARD2_0
using RuntimeHelpers = CommunityToolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;
#endif

namespace CommunityToolkit.HighPerformance;

/// <inheritdoc/>
partial struct Span3D<T>
{
    /// <summary>
    /// Gets an enumerable that traverses items in a specified row of a specific slice.
    /// </summary>
    /// <param name="slice">The target slice to enumerate within the current <see cref="Span3D{T}"/> instance.</param>
    /// <param name="row">The target row to enumerate within the current <see cref="Span3D{T}"/> instance.</param>
    /// <returns>A <see cref="RefEnumerable{T}"/> with target items to enumerate.</returns>
    /// <remarks>The returned <see cref="RefEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RefEnumerable<T> GetRow(int slice, int row)
    {
        if ((uint)slice >= Depth)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlice();
        }

        if ((uint)row >= this.height)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
        }

        nint startIndex = (nint)(uint)this.SliceStride * (nint)(uint)slice +
                          (nint)(uint)this.RowStride * (nint)(uint)row;

        ref T r0 = ref DangerousGetReference();
        ref T r1 = ref Unsafe.Add(ref r0, startIndex);

#if NETSTANDARD2_1_OR_GREATER
        return new(ref r1, length: this.width, step: 1);
#else
        IntPtr offset = RuntimeHelpers.GetObjectDataOrReferenceByteOffset(this.Instance, ref r1);

        return new(this.Instance, offset, length: this.width, step: 1);
#endif
    }

    /// <summary>
    /// Gets an enumerable that traverses items in a specified column of a specific slice.
    /// </summary>
    /// <param name="slice">The target slice to enumerate within the current <see cref="Span3D{T}"/> instance.</param>
    /// <param name="column">The target column to enumerate within the current <see cref="Span3D{T}"/> instance.</param>
    /// <returns>A <see cref="RefEnumerable{T}"/> with target items to enumerate.</returns>
    /// <remarks>The returned <see cref="RefEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RefEnumerable<T> GetColumn(int slice,int column)
    {
        if ((uint)slice >= Depth)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForSlice();
        }

        if ((uint)column >= this.width)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForColumn();
        }

        nint startIndex = (nint)(uint)this.SliceStride * (nint)(uint)slice + column;

        ref T r0 = ref DangerousGetReference();
        ref T r1 = ref Unsafe.Add(ref r0, startIndex);

#if NETSTANDARD2_1_OR_GREATER
        return new(ref r1, length: this.height, step: this.RowStride);
#else
        IntPtr offset = RuntimeHelpers.GetObjectDataOrReferenceByteOffset(this.Instance, ref r1);

        return new(this.Instance, offset, length: this.height, step: this.RowStride);
#endif
    }

    /// <summary>
    /// Gets an enumerable that traverses items in a specified row along slices.
    /// </summary>
    /// <param name="row">The target row to enumerate within the current <see cref="Span3D{T}"/> instance.</param>
    /// <param name="column">The target column to enumerate within the current <see cref="Span3D{T}"/> instance.</param>
    /// <returns>A <see cref="RefEnumerable{T}"/> with target items to enumerate.</returns>
    /// <remarks>The returned <see cref="RefEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RefEnumerable<T> GetDepthColumn(int row, int column)
    {
        if ((uint)row >= this.height)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForRow();
        }

        if ((uint)column >= this.width)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForColumn();
        }

        nint startIndex = (nint)(uint)this.RowStride * (nint)(uint)row + column;

        ref T r0 = ref DangerousGetReference();
        ref T r1 = ref Unsafe.Add(ref r0, startIndex);

#if NETSTANDARD2_1_OR_GREATER
        return new(ref r1, length: Depth, step: this.SliceStride);
#else
        IntPtr offset = RuntimeHelpers.GetObjectDataOrReferenceByteOffset(this.Instance, ref r1);

        return new(this.Instance, offset, length: Depth, step: this.SliceStride);
#endif
    }

    /// <summary>
    /// Returns an enumerator for the current <see cref="Span3D{T}"/> instance.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to traverse the items in the current <see cref="Span3D{T}"/> instance
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Provides an enumerator for the elements of a <see cref="Span3D{T}"/> instance.
    /// </summary>
    public ref struct Enumerator
    {
#if NET8_0_OR_GREATER
        /// <summary>
        /// The <typeparamref name="T"/> reference for the <see cref="Span3D{T}"/> instance.
        /// </summary>
        private readonly ref T reference;

        /// <summary>
        /// The depth of the specified 3D region.
        /// </summary>
        private readonly int depth;
#elif NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// The <see cref="Span{T}"/> instance pointing to the first item in the target memory area.
        /// </summary>
        /// <remarks>Just like in <see cref="Span3D{T}"/>, the length is the depth of the 3D region.</remarks>
        private readonly Span<T> span;
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
        /// The row stride of the specified 3D region.
        /// </summary>
        private readonly int rowStride;

        /// <summary>
        /// The slice stride of the specified 3D region.
        /// </summary>
        private readonly int sliceStride;

        /// <summary>
        /// The current horizontal offset.
        /// </summary>
        private int x;

        /// <summary>
        /// The current vertical offset.
        /// </summary>
        private int y;

        /// <summary>
        /// The current slice offset.
        /// </summary>
        private int z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        /// </summary>
        /// <param name="span">The target <see cref="Span3D{T}"/> instance to enumerate.</param>
        internal Enumerator(Span3D<T> span)
        {
#if NET8_0_OR_GREATER
            this.reference = ref span.reference;
            this.depth = span.depth;
#elif NETSTANDARD2_1_OR_GREATER
            this.span = span.span;
#else
            this.instance = span.Instance;
            this.offset = span.Offset;
            this.depth = span.depth;
#endif
            this.height = span.height;
            this.width = span.width;
            this.rowStride = span.RowStride;
            this.sliceStride = span.SliceStride;
            this.x = -1;
            this.y = 0;
            this.z = 0;
        }

        /// <summary>
        /// Implements the duck-typed <see cref="System.Collections.IEnumerator.MoveNext"/> method.
        /// </summary>
        /// <returns><see langword="true"/> whether a new element is available, <see langword="false"/> otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int x = this.x + 1;

            // Horizontal move, within range
            if (x < this.width)
            {
                this.x = x;

                return true;
            }

            // We reached the end of a row and there is at least
            // another row available: wrap to a new line and continue.
            this.x = 0;
            int y = this.y + 1;

            if (y < this.height)
            {
                this.y = y;

                return true;
            }

            // We reached the end of a slice and there is at least
            // another slice available: wrap to the start of the next one and continue.
            this.y = 0;
            this.z++;

#if NET8_0_OR_GREATER
            return this.z < this.depth;
#elif NETSTANDARD2_1_OR_GREATER
            return this.z < this.span.Length;
#else
            return this.z < this.depth;
#endif
        }

        /// <summary>
        /// Gets the duck-typed <see cref="System.Collections.Generic.IEnumerator{T}.Current"/> property.
        /// </summary>
        public readonly ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if NET8_0_OR_GREATER
                ref T r0 = ref this.reference;
#elif NETSTANDARD2_1_OR_GREATER
                ref T r0 = ref MemoryMarshal.GetReference(this.span);
#else
                ref T r0 = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<T>(this.instance, this.offset);
#endif
                nint index = ((nint)(uint)this.z * (nint)(uint)this.sliceStride) +
                             ((nint)(uint)this.y * (nint)(uint)this.rowStride) +
                             (nint)(uint)this.x;

                return ref Unsafe.Add(ref r0, index);
            }
        }
    }
}
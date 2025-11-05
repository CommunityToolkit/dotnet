// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CommunityToolkit.HighPerformance.Enumerables;

/// <summary>
/// A <see langword="ref"/> <see langword="struct"/> that enumerates the items in a given <see cref="ReadOnlySpan{T}"/> instance.
/// </summary>
/// <typeparam name="T">The type of items to enumerate.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public ref struct ReadOnlySpanEnumerable<T>
{
    /// <summary>
    /// The source <see cref="ReadOnlySpan{T}"/> instance.
    /// </summary>
    private readonly ReadOnlySpan<T> span;

    /// <summary>
    /// The current index within <see cref="span"/>.
    /// </summary>
    private int index;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpanEnumerable{T}"/> struct.
    /// </summary>
    /// <param name="span">The source <see cref="ReadOnlySpan{T}"/> instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpanEnumerable(ReadOnlySpan<T> span)
    {
        this.span = span;
        this.index = -1;
    }

    /// <summary>
    /// Implements the duck-typed <see cref="IEnumerable{T}.GetEnumerator"/> method.
    /// </summary>
    /// <returns>An <see cref="ReadOnlySpanEnumerable{T}"/> instance targeting the current <see cref="ReadOnlySpan{T}"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpanEnumerable<T> GetEnumerator() => this;

    /// <summary>
    /// Implements the duck-typed <see cref="System.Collections.IEnumerator.MoveNext"/> method.
    /// </summary>
    /// <returns><see langword="true"/> whether a new element is available, <see langword="false"/> otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        return ++this.index < this.span.Length;
    }

    /// <summary>
    /// Gets the duck-typed <see cref="IEnumerator{T}.Current"/> property.
    /// </summary>
    public readonly Item Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
#if NETSTANDARD2_1_OR_GREATER
            ref T r0 = ref MemoryMarshal.GetReference(this.span);
            ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)this.index);

            // See comment in SpanEnumerable<T> about this
            return new(ref ri, this.index);
#else
            return new(this.span, this.index);
#endif
        }
    }

    /// <summary>
    /// An item from a source <see cref="Span{T}"/> instance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly ref struct Item
    {
#if NET8_0_OR_GREATER
        /// <summary>
        /// The <typeparamref name="T"/> reference for the <see cref="Item"/> instance.
        /// </summary>
        private readonly ref readonly T reference;

        /// <summary>
        /// The index of the current <see cref="Item"/> instance.
        /// </summary>
        private readonly int index;
#else
        /// <summary>
        /// The source <see cref="ReadOnlySpan{T}"/> instance.
        /// </summary>
        private readonly ReadOnlySpan<T> span;
#endif

#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> struct.
        /// </summary>
        /// <param name="value">A reference to the target value.</param>
        /// <param name="index">The index of the target value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Item(ref T value, int index)
        {
#if NET8_0_OR_GREATER
            this.reference = ref value;
            this.index = index;
#else
            this.span = MemoryMarshal.CreateReadOnlySpan(ref value, index);
#endif
        }
#else
        /// <summary>
        /// The current index within <see cref="span"/>.
        /// </summary>
        private readonly int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> struct.
        /// </summary>
        /// <param name="span">The source <see cref="ReadOnlySpan{T}"/> instance.</param>
        /// <param name="index">The current index within <paramref name="span"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Item(ReadOnlySpan<T> span, int index)
        {
            this.span = span;
            this.index = index;
        }
#endif

        /// <summary>
        /// Gets the reference to the current value.
        /// </summary>
        public ref readonly T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if NET8_0_OR_GREATER
                return ref this.reference;
#elif NETSTANDARD2_1_OR_GREATER
                return ref MemoryMarshal.GetReference(this.span);
#else
                ref T r0 = ref MemoryMarshal.GetReference(this.span);
                ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)this.index);

                return ref ri;
#endif
            }
        }

        /// <summary>
        /// Gets the current index.
        /// </summary>
        public int Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if NET8_0_OR_GREATER
                return this.index;
#elif NETSTANDARD2_1_OR_GREATER
                return this.span.Length;
#else
                return this.index;
#endif
            }
        }
    }
}

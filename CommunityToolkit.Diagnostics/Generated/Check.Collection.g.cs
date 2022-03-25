// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// =====================
// Auto generated file
// =====================

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Check
{

    /// <summary>
    /// Checks that the input <see cref="Span{T}"/> instance must be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="span"/> is empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(Span<T> span)
    {
        return span.Length == 0;
    }

    /// <summary>
    /// Checks that the input <see cref="Span{T}"/> instance must not be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="span"/> is not empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(Span<T> span)
    {
        return span.Length != 0;
    }

    /// <summary>
    /// Checks that the input <see cref="Span{T}"/> instance must have a size of a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(Span<T> span, int size)
    {
        return span.Length == size;
    }

    /// <summary>
    /// Checks that the input <see cref="Span{T}"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> not is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo<T>(Span<T> span, int size)
    {
        return span.Length != size;
    }

    /// <summary>
    /// Checks that the input <see cref="Span{T}"/> instance must have a size over a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is greater than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan<T>(Span<T> span, int size)
    {
        return span.Length > size;
    }

    /// <summary>
    /// Checks that the input <see cref="Span{T}"/> instance must have a size of at least or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is greater than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo<T>(Span<T> span, int size)
    {
        return span.Length >= size;
    }

    /// <summary>
    /// Checks that the input <see cref="Span{T}"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is less than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan<T>(Span<T> span, int size)
    {
        return span.Length < size;
    }

    /// <summary>
    /// Checks that the input <see cref="Span{T}"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is less than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(Span<T> span, int size)
    {
        return span.Length <= size;
    }

    /// <summary>
    /// Checks that the source <see cref="Span{T}"/> instance must have the same size of a destination <see cref="Span{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="Span{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="Span{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is the same length as <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(Span<T> source, Span<T> destination)
    {
        return source.Length == destination.Length;
    }

    /// <summary>
    /// Checks that the source <see cref="Span{T}"/> instance must have a size of less than or equal to that of a destination <see cref="Span{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="Span{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="Span{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="source"/> is less than or equal to the length of <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(Span<T> source, Span<T> destination)
    {
        return source.Length <= destination.Length;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see cref="Span{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="span"/>.</param>
    /// <param name="span">The input <see cref="Span{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is a valid index in <paramref name="span"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor<T>(int index, Span<T> span)
    {

        return (uint)index < (uint)span.Length;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see cref="Span{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="span"/>.</param>
    /// <param name="span">The input <see cref="Span{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is not a valid index in <paramref name="span"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor<T>(int index, Span<T> span)
    {
        return (uint)index >= (uint)span.Length;
    }


    /// <summary>
    /// Checks that the input <see cref="ReadOnlySpan{T}"/> instance must be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="span"/> is empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(ReadOnlySpan<T> span)
    {
        return span.Length == 0;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlySpan{T}"/> instance must not be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="span"/> is not empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(ReadOnlySpan<T> span)
    {
        return span.Length != 0;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlySpan{T}"/> instance must have a size of a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(ReadOnlySpan<T> span, int size)
    {
        return span.Length == size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlySpan{T}"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> not is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo<T>(ReadOnlySpan<T> span, int size)
    {
        return span.Length != size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlySpan{T}"/> instance must have a size over a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is greater than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan<T>(ReadOnlySpan<T> span, int size)
    {
        return span.Length > size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlySpan{T}"/> instance must have a size of at least or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is greater than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo<T>(ReadOnlySpan<T> span, int size)
    {
        return span.Length >= size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlySpan{T}"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is less than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan<T>(ReadOnlySpan<T> span, int size)
    {
        return span.Length < size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlySpan{T}"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="span"/> is less than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(ReadOnlySpan<T> span, int size)
    {
        return span.Length <= size;
    }

    /// <summary>
    /// Checks that the source <see cref="ReadOnlySpan{T}"/> instance must have the same size of a destination <see cref="ReadOnlySpan{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is the same length as <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(ReadOnlySpan<T> source, Span<T> destination)
    {
        return source.Length == destination.Length;
    }

    /// <summary>
    /// Checks that the source <see cref="ReadOnlySpan{T}"/> instance must have a size of less than or equal to that of a destination <see cref="ReadOnlySpan{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="ReadOnlySpan{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="source"/> is less than or equal to the length of <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(ReadOnlySpan<T> source, Span<T> destination)
    {
        return source.Length <= destination.Length;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see cref="ReadOnlySpan{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="span"/>.</param>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is a valid index in <paramref name="span"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor<T>(int index, ReadOnlySpan<T> span)
    {

        return (uint)index < (uint)span.Length;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see cref="ReadOnlySpan{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="span"/>.</param>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is not a valid index in <paramref name="span"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor<T>(int index, ReadOnlySpan<T> span)
    {
        return (uint)index >= (uint)span.Length;
    }


    /// <summary>
    /// Checks that the input <see cref="Memory{T}"/> instance must be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="memory"/> is empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(Memory<T> memory)
    {
        return memory.Length == 0;
    }

    /// <summary>
    /// Checks that the input <see cref="Memory{T}"/> instance must not be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="memory"/> is not empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(Memory<T> memory)
    {
        return memory.Length != 0;
    }

    /// <summary>
    /// Checks that the input <see cref="Memory{T}"/> instance must have a size of a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(Memory<T> memory, int size)
    {
        return memory.Length == size;
    }

    /// <summary>
    /// Checks that the input <see cref="Memory{T}"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> not is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo<T>(Memory<T> memory, int size)
    {
        return memory.Length != size;
    }

    /// <summary>
    /// Checks that the input <see cref="Memory{T}"/> instance must have a size over a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is greater than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan<T>(Memory<T> memory, int size)
    {
        return memory.Length > size;
    }

    /// <summary>
    /// Checks that the input <see cref="Memory{T}"/> instance must have a size of at least or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is greater than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo<T>(Memory<T> memory, int size)
    {
        return memory.Length >= size;
    }

    /// <summary>
    /// Checks that the input <see cref="Memory{T}"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is less than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan<T>(Memory<T> memory, int size)
    {
        return memory.Length < size;
    }

    /// <summary>
    /// Checks that the input <see cref="Memory{T}"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is less than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(Memory<T> memory, int size)
    {
        return memory.Length <= size;
    }

    /// <summary>
    /// Checks that the source <see cref="Memory{T}"/> instance must have the same size of a destination <see cref="Memory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is the same length as <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(Memory<T> source, Memory<T> destination)
    {
        return source.Length == destination.Length;
    }

    /// <summary>
    /// Checks that the source <see cref="Memory{T}"/> instance must have a size of less than or equal to that of a destination <see cref="Memory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="Memory{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="source"/> is less than or equal to the length of <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(Memory<T> source, Memory<T> destination)
    {
        return source.Length <= destination.Length;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see cref="Memory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="memory"/>.</param>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is a valid index in <paramref name="memory"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor<T>(int index, Memory<T> memory)
    {

        return (uint)index < (uint)memory.Length;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see cref="Memory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="Memory{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="memory"/>.</param>
    /// <param name="memory">The input <see cref="Memory{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is not a valid index in <paramref name="memory"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor<T>(int index, Memory<T> memory)
    {
        return (uint)index >= (uint)memory.Length;
    }


    /// <summary>
    /// Checks that the input <see cref="ReadOnlyMemory{T}"/> instance must be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="memory"/> is empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(ReadOnlyMemory<T> memory)
    {
        return memory.Length == 0;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlyMemory{T}"/> instance must not be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="memory"/> is not empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(ReadOnlyMemory<T> memory)
    {
        return memory.Length != 0;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlyMemory{T}"/> instance must have a size of a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(ReadOnlyMemory<T> memory, int size)
    {
        return memory.Length == size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlyMemory{T}"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> not is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo<T>(ReadOnlyMemory<T> memory, int size)
    {
        return memory.Length != size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlyMemory{T}"/> instance must have a size over a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is greater than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan<T>(ReadOnlyMemory<T> memory, int size)
    {
        return memory.Length > size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlyMemory{T}"/> instance must have a size of at least or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is greater than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo<T>(ReadOnlyMemory<T> memory, int size)
    {
        return memory.Length >= size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlyMemory{T}"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is less than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan<T>(ReadOnlyMemory<T> memory, int size)
    {
        return memory.Length < size;
    }

    /// <summary>
    /// Checks that the input <see cref="ReadOnlyMemory{T}"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="memory"/> is less than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(ReadOnlyMemory<T> memory, int size)
    {
        return memory.Length <= size;
    }

    /// <summary>
    /// Checks that the source <see cref="ReadOnlyMemory{T}"/> instance must have the same size of a destination <see cref="ReadOnlyMemory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is the same length as <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(ReadOnlyMemory<T> source, Memory<T> destination)
    {
        return source.Length == destination.Length;
    }

    /// <summary>
    /// Checks that the source <see cref="ReadOnlyMemory{T}"/> instance must have a size of less than or equal to that of a destination <see cref="ReadOnlyMemory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="ReadOnlyMemory{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="source"/> is less than or equal to the length of <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(ReadOnlyMemory<T> source, Memory<T> destination)
    {
        return source.Length <= destination.Length;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see cref="ReadOnlyMemory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="memory"/>.</param>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is a valid index in <paramref name="memory"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor<T>(int index, ReadOnlyMemory<T> memory)
    {

        return (uint)index < (uint)memory.Length;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see cref="ReadOnlyMemory{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ReadOnlyMemory{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="memory"/>.</param>
    /// <param name="memory">The input <see cref="ReadOnlyMemory{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is not a valid index in <paramref name="memory"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor<T>(int index, ReadOnlyMemory<T> memory)
    {
        return (uint)index >= (uint)memory.Length;
    }


    /// <summary>
    /// Checks that the input <see typeparamref="T"/> array instance must be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="array">The input <see typeparamref="T"/> array instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="array"/> is empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(T[] array)
    {
        return array.Length == 0;
    }

    /// <summary>
    /// Checks that the input <see typeparamref="T"/> array instance must not be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="array">The input <see typeparamref="T"/> array instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="array"/> is not empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(T[] array)
    {
        return array.Length != 0;
    }

    /// <summary>
    /// Checks that the input <see typeparamref="T"/> array instance must have a size of a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="array">The input <see typeparamref="T"/> array instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="array"/> is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(T[] array, int size)
    {
        return array.Length == size;
    }

    /// <summary>
    /// Checks that the input <see typeparamref="T"/> array instance must have a size not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="array">The input <see typeparamref="T"/> array instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="array"/> not is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo<T>(T[] array, int size)
    {
        return array.Length != size;
    }

    /// <summary>
    /// Checks that the input <see typeparamref="T"/> array instance must have a size over a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="array">The input <see typeparamref="T"/> array instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="array"/> is greater than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan<T>(T[] array, int size)
    {
        return array.Length > size;
    }

    /// <summary>
    /// Checks that the input <see typeparamref="T"/> array instance must have a size of at least or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="array">The input <see typeparamref="T"/> array instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="array"/> is greater than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo<T>(T[] array, int size)
    {
        return array.Length >= size;
    }

    /// <summary>
    /// Checks that the input <see typeparamref="T"/> array instance must have a size of less than a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="array">The input <see typeparamref="T"/> array instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="array"/> is less than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan<T>(T[] array, int size)
    {
        return array.Length < size;
    }

    /// <summary>
    /// Checks that the input <see typeparamref="T"/> array instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="array">The input <see typeparamref="T"/> array instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="array"/> is less than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(T[] array, int size)
    {
        return array.Length <= size;
    }

    /// <summary>
    /// Checks that the source <see typeparamref="T"/> array instance must have the same size of a destination <see typeparamref="T"/> array instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="source">The source <see typeparamref="T"/> array instance to check the size for.</param>
    /// <param name="destination">The destination <see typeparamref="T"/> array instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is the same length as <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(T[] source, T[] destination)
    {
        return source.Length == destination.Length;
    }

    /// <summary>
    /// Checks that the source <see typeparamref="T"/> array instance must have a size of less than or equal to that of a destination <see typeparamref="T"/> array instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="source">The source <see typeparamref="T"/> array instance to check the size for.</param>
    /// <param name="destination">The destination <see typeparamref="T"/> array instance to check the size for.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="source"/> is less than or equal to the length of <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(T[] source, T[] destination)
    {
        return source.Length <= destination.Length;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see typeparamref="T"/> array instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="array"/>.</param>
    /// <param name="array">The input <see typeparamref="T"/> array instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is a valid index in <paramref name="array"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor<T>(int index, T[] array)
    {

        return (uint)index < (uint)array.Length;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see typeparamref="T"/> array instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see typeparamref="T"/> array instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="array"/>.</param>
    /// <param name="array">The input <see typeparamref="T"/> array instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is not a valid index in <paramref name="array"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor<T>(int index, T[] array)
    {
        return (uint)index >= (uint)array.Length;
    }


    /// <summary>
    /// Checks that the input <see cref="List{T}"/> instance must be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="list">The input <see cref="List{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="list"/> is empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(List<T> list)
    {
        return list.Count == 0;
    }

    /// <summary>
    /// Checks that the input <see cref="List{T}"/> instance must not be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="list">The input <see cref="List{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="list"/> is not empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(List<T> list)
    {
        return list.Count != 0;
    }

    /// <summary>
    /// Checks that the input <see cref="List{T}"/> instance must have a size of a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="list">The input <see cref="List{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="list"/> is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(List<T> list, int size)
    {
        return list.Count == size;
    }

    /// <summary>
    /// Checks that the input <see cref="List{T}"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="list">The input <see cref="List{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="list"/> not is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo<T>(List<T> list, int size)
    {
        return list.Count != size;
    }

    /// <summary>
    /// Checks that the input <see cref="List{T}"/> instance must have a size over a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="list">The input <see cref="List{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="list"/> is greater than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan<T>(List<T> list, int size)
    {
        return list.Count > size;
    }

    /// <summary>
    /// Checks that the input <see cref="List{T}"/> instance must have a size of at least or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="list">The input <see cref="List{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="list"/> is greater than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo<T>(List<T> list, int size)
    {
        return list.Count >= size;
    }

    /// <summary>
    /// Checks that the input <see cref="List{T}"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="list">The input <see cref="List{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="list"/> is less than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan<T>(List<T> list, int size)
    {
        return list.Count < size;
    }

    /// <summary>
    /// Checks that the input <see cref="List{T}"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="list">The input <see cref="List{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="list"/> is less than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(List<T> list, int size)
    {
        return list.Count <= size;
    }

    /// <summary>
    /// Checks that the source <see cref="List{T}"/> instance must have the same size of a destination <see cref="List{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="List{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="List{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is the same length as <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(List<T> source, List<T> destination)
    {
        return source.Count == destination.Count;
    }

    /// <summary>
    /// Checks that the source <see cref="List{T}"/> instance must have a size of less than or equal to that of a destination <see cref="List{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="List{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="List{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="source"/> is less than or equal to the length of <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(List<T> source, List<T> destination)
    {
        return source.Count <= destination.Count;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see cref="List{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="list"/>.</param>
    /// <param name="list">The input <see cref="List{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is a valid index in <paramref name="list"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor<T>(int index, List<T> list)
    {

        return (uint)index < (uint)list.Count;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see cref="List{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="List{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="list"/>.</param>
    /// <param name="list">The input <see cref="List{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is not a valid index in <paramref name="list"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor<T>(int index, List<T> list)
    {
        return (uint)index >= (uint)list.Count;
    }


    /// <summary>
    /// Checks that the input <see cref="ICollection{T}"/> instance must be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="collection"/> is empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(ICollection<T> collection)
    {
        return collection.Count == 0;
    }

    /// <summary>
    /// Checks that the input <see cref="ICollection{T}"/> instance must not be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="collection"/> is not empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(ICollection<T> collection)
    {
        return collection.Count != 0;
    }

    /// <summary>
    /// Checks that the input <see cref="ICollection{T}"/> instance must have a size of a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(ICollection<T> collection, int size)
    {
        return collection.Count == size;
    }

    /// <summary>
    /// Checks that the input <see cref="ICollection{T}"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> not is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo<T>(ICollection<T> collection, int size)
    {
        return collection.Count != size;
    }

    /// <summary>
    /// Checks that the input <see cref="ICollection{T}"/> instance must have a size over a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is greater than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan<T>(ICollection<T> collection, int size)
    {
        return collection.Count > size;
    }

    /// <summary>
    /// Checks that the input <see cref="ICollection{T}"/> instance must have a size of at least or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is greater than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo<T>(ICollection<T> collection, int size)
    {
        return collection.Count >= size;
    }

    /// <summary>
    /// Checks that the input <see cref="ICollection{T}"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is less than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan<T>(ICollection<T> collection, int size)
    {
        return collection.Count < size;
    }

    /// <summary>
    /// Checks that the input <see cref="ICollection{T}"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is less than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(ICollection<T> collection, int size)
    {
        return collection.Count <= size;
    }

    /// <summary>
    /// Checks that the source <see cref="ICollection{T}"/> instance must have the same size of a destination <see cref="ICollection{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is the same length as <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(ICollection<T> source, ICollection<T> destination)
    {
        return source.Count == destination.Count;
    }

    /// <summary>
    /// Checks that the source <see cref="ICollection{T}"/> instance must have a size of less than or equal to that of a destination <see cref="ICollection{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="ICollection{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="source"/> is less than or equal to the length of <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(ICollection<T> source, ICollection<T> destination)
    {
        return source.Count <= destination.Count;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see cref="ICollection{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="collection"/>.</param>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is a valid index in <paramref name="collection"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor<T>(int index, ICollection<T> collection)
    {

        return (uint)index < (uint)collection.Count;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see cref="ICollection{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="ICollection{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="collection"/>.</param>
    /// <param name="collection">The input <see cref="ICollection{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is not a valid index in <paramref name="collection"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor<T>(int index, ICollection<T> collection)
    {
        return (uint)index >= (uint)collection.Count;
    }


    /// <summary>
    /// Checks that the input <see cref="IReadOnlyCollection{T}"/> instance must be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="collection"/> is empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>(IReadOnlyCollection<T> collection)
    {
        return collection.Count == 0;
    }

    /// <summary>
    /// Checks that the input <see cref="IReadOnlyCollection{T}"/> instance must not be empty.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="collection"/> is not empty, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(IReadOnlyCollection<T> collection)
    {
        return collection.Count != 0;
    }

    /// <summary>
    /// Checks that the input <see cref="IReadOnlyCollection{T}"/> instance must have a size of a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(IReadOnlyCollection<T> collection, int size)
    {
        return collection.Count == size;
    }

    /// <summary>
    /// Checks that the input <see cref="IReadOnlyCollection{T}"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> not is <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo<T>(IReadOnlyCollection<T> collection, int size)
    {
        return collection.Count != size;
    }

    /// <summary>
    /// Checks that the input <see cref="IReadOnlyCollection{T}"/> instance must have a size over a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is greater than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan<T>(IReadOnlyCollection<T> collection, int size)
    {
        return collection.Count > size;
    }

    /// <summary>
    /// Checks that the input <see cref="IReadOnlyCollection{T}"/> instance must have a size of at least or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is greater than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo<T>(IReadOnlyCollection<T> collection, int size)
    {
        return collection.Count >= size;
    }

    /// <summary>
    /// Checks that the input <see cref="IReadOnlyCollection{T}"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is less than <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan<T>(IReadOnlyCollection<T> collection, int size)
    {
        return collection.Count < size;
    }

    /// <summary>
    /// Checks that the input <see cref="IReadOnlyCollection{T}"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="collection"/> is less than or equal to <paramref name="size"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(IReadOnlyCollection<T> collection, int size)
    {
        return collection.Count <= size;
    }

    /// <summary>
    /// Checks that the source <see cref="IReadOnlyCollection{T}"/> instance must have the same size of a destination <see cref="IReadOnlyCollection{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is the same length as <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo<T>(IReadOnlyCollection<T> source, ICollection<T> destination)
    {
        return source.Count == destination.Count;
    }

    /// <summary>
    /// Checks that the source <see cref="IReadOnlyCollection{T}"/> instance must have a size of less than or equal to that of a destination <see cref="IReadOnlyCollection{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="source">The source <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="IReadOnlyCollection{T}"/> instance to check the size for.</param>
    /// <returns><see langword="true"/> if the length of <paramref name="source"/> is less than or equal to the length of <paramref name="destination"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo<T>(IReadOnlyCollection<T> source, ICollection<T> destination)
    {
        return source.Count <= destination.Count;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see cref="IReadOnlyCollection{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="collection"/>.</param>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is a valid index in <paramref name="collection"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor<T>(int index, IReadOnlyCollection<T> collection)
    {

        return (uint)index < (uint)collection.Count;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see cref="IReadOnlyCollection{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The item of items in the input <see cref="IReadOnlyCollection{T}"/> instance.</typeparam>
    /// <param name="index">The input index to be used to access <paramref name="collection"/>.</param>
    /// <param name="collection">The input <see cref="IReadOnlyCollection{T}"/> instance to use to validate <paramref name="index"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is not a valid index in <paramref name="collection"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor<T>(int index, IReadOnlyCollection<T> collection)
    {
        return (uint)index >= (uint)collection.Count;
    }

}

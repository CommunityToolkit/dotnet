// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#pragma warning disable CS8777

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Check
{
    /// <summary>
    /// Checks that the input <see cref="string"/> instance must be <see langword="null"/> or empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(string? text)
    {
        return string.IsNullOrEmpty(text);
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must not be <see langword="null"/> or empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrEmpty([NotNull] string? text)
    {
        return !string.IsNullOrEmpty(text);
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace(string? text)
    {
        return string.IsNullOrWhiteSpace(text);
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must not be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrWhiteSpace([NotNull] string? text)
    {
        return !string.IsNullOrWhiteSpace(text);
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must be empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(string text)
    {
        return text.Length == 0;
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must not be empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty(string text)
    {
        return text.Length != 0;
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must be whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(string text)
    {
        return string.IsNullOrWhiteSpace(text);
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must not be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotWhiteSpace(string text)
    {
        return !string.IsNullOrWhiteSpace(text);
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must have a size of a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo(string text, int size)
    {
        return text.Length == size;
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeNotEqualTo(string text, int size)
    {
        return text.Length != size;
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must have a size over a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThan(string text, int size)
    {
        return text.Length > size;
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must have a size of at least specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeGreaterThanOrEqualTo(string text, int size)
    {
        return text.Length >= size;
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThan(string text, int size)
    {
        return text.Length < size;
    }

    /// <summary>
    /// Checks that the input <see cref="string"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo(string text, int size)
    {
        return text.Length <= size;
    }

    /// <summary>
    /// Checks that the source <see cref="string"/> instance must have the same size of a destination <see cref="string"/> instance.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="string"/> instance to check the size for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeEqualTo(string text, string destination)
    {
        return text.Length == destination.Length;
    }

    /// <summary>
    /// Checks that the source <see cref="string"/> instance must have a size of less than or equal to that of a destination <see cref="string"/> instance.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="string"/> instance to check the size for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSizeLessThanOrEqualTo(string text, string destination)
    {
        return text.Length <= destination.Length;
    }

    /// <summary>
    /// Checks that the input index is valid for a given <see cref="string"/> instance.
    /// </summary>
    /// <param name="index">The input index to be used to access <paramref name="text"/>.</param>
    /// <param name="text">The input <see cref="string"/> instance to use to validate <paramref name="index"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeFor(int index, string text)
    {
        return (uint)index < (uint)text.Length;
    }

    /// <summary>
    /// Checks that the input index is not valid for a given <see cref="string"/> instance.
    /// </summary>
    /// <param name="index">The input index to be used to access <paramref name="text"/>.</param>
    /// <param name="text">The input <see cref="string"/> instance to use to validate <paramref name="index"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRangeFor(int index, string text)
    {
        return (uint)index >= (uint)text.Length;
    }
}

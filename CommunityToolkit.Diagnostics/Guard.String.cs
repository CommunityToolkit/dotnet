// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#pragma warning disable CS8777

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must be <see langword="null"/> or empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is neither <see langword="null"/> nor empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? IsNullOrEmpty(string? text, [CallerArgumentExpression("text")] string name = "")
    {
        if (!string.IsNullOrEmpty(text))
        {
            ThrowHelper.ThrowArgumentExceptionForIsNullOrEmpty(text, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must not be <see langword="null"/> or empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsNotNullOrEmpty([NotNull] string? text, [CallerArgumentExpression("text")] string name = "")
    {
        if (string.IsNullOrEmpty(text))
        {
            ThrowHelper.ThrowArgumentExceptionForIsNotNullOrEmpty(text, name);
        }

        return text!;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is neither <see langword="null"/> nor whitespace.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? IsNullOrWhiteSpace(string? text, [CallerArgumentExpression("text")] string name = "")
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            ThrowHelper.ThrowArgumentExceptionForIsNullOrWhiteSpace(text, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must not be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is whitespace.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsNotNullOrWhiteSpace([NotNull] string? text, [CallerArgumentExpression("text")] string name = "")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            ThrowHelper.ThrowArgumentExceptionForIsNotNullOrWhiteSpace(text, name);
        }

        return text!;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must be empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsEmpty(string text, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length != 0)
        {
            ThrowHelper.ThrowArgumentExceptionForIsEmpty(text, name);
        }

        return string.Empty;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must not be empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsNotEmpty(string text, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length == 0)
        {
            ThrowHelper.ThrowArgumentExceptionForIsNotEmpty(name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must be whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is neither <see langword="null"/> nor whitespace.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsWhiteSpace(string text, [CallerArgumentExpression("text")] string name = "")
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            ThrowHelper.ThrowArgumentExceptionForIsWhiteSpace(text, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must not be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is <see langword="null"/> or whitespace.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsNotWhiteSpace(string text, [CallerArgumentExpression("text")] string name = "")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            ThrowHelper.ThrowArgumentExceptionForIsNotWhiteSpace(text, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size of a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is != <paramref name="size"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeEqualTo(string text, int size, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length != size)
        {
            ThrowHelper.ThrowArgumentExceptionForHasSizeEqualTo(text, size, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is == <paramref name="size"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeNotEqualTo(string text, int size, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length == size)
        {
            ThrowHelper.ThrowArgumentExceptionForHasSizeNotEqualTo(text, size, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size over a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is &lt;= <paramref name="size"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeGreaterThan(string text, int size, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length <= size)
        {
            ThrowHelper.ThrowArgumentExceptionForHasSizeGreaterThan(text, size, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size of at least specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is &lt; <paramref name="size"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeGreaterThanOrEqualTo(string text, int size, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length < size)
        {
            ThrowHelper.ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo(text, size, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is >= <paramref name="size"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeLessThan(string text, int size, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length >= size)
        {
            ThrowHelper.ThrowArgumentExceptionForHasSizeLessThan(text, size, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is > <paramref name="size"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeLessThanOrEqualTo(string text, int size, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length > size)
        {
            ThrowHelper.ThrowArgumentExceptionForHasSizeLessThanOrEqualTo(text, size, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the source <see cref="string"/> instance must have the same size of a destination <see cref="string"/> instance.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="string"/> instance to check the size for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is != the one of <paramref name="destination"/>.</exception>
    /// <remarks>The <see cref="string"/> type is immutable, but the name of this API is kept for consistency with the other overloads.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeEqualTo(string text, string destination, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length != destination.Length)
        {
            ThrowHelper.ThrowArgumentExceptionForHasSizeEqualTo(text, destination, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the source <see cref="string"/> instance must have a size of less than or equal to that of a destination <see cref="string"/> instance.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="string"/> instance to check the size for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is > the one of <paramref name="destination"/>.</exception>
    /// <remarks>The <see cref="string"/> type is immutable, but the name of this API is kept for consistency with the other overloads.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeLessThanOrEqualTo(string text, string destination, [CallerArgumentExpression("text")] string name = "")
    {
        if (text.Length > destination.Length)
        {
            ThrowHelper.ThrowArgumentExceptionForHasSizeLessThanOrEqualTo(text, destination, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input index is valid for a given <see cref="string"/> instance.
    /// </summary>
    /// <param name="index">The input index to be used to access <paramref name="text"/>.</param>
    /// <param name="text">The input <see cref="string"/> instance to use to validate <paramref name="index"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is not valid to access <paramref name="text"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsInRangeFor(int index, string text, [CallerArgumentExpression("index")] string name = "")
    {
        if ((uint)index >= (uint)text.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRangeFor(index, text, name);
        }

        return text;
    }

    /// <summary>
    /// Asserts that the input index is not valid for a given <see cref="string"/> instance.
    /// </summary>
    /// <param name="index">The input index to be used to access <paramref name="text"/>.</param>
    /// <param name="text">The input <see cref="string"/> instance to use to validate <paramref name="index"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is valid to access <paramref name="text"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsNotInRangeFor(int index, string text, [CallerArgumentExpression("index")] string name = "")
    {
        if ((uint)index < (uint)text.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor(index, text, name);
        }

        return text;
    }
}
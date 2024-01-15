// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class FGuard
{
    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must be <see langword="null"/> or <see cref="string.Empty"/>.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> that is <see langword="null"/> or <see cref="string.Empty"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is neither <see langword="null"/> nor empty.</exception>
    /// <seealso cref="string.IsNullOrEmpty(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? IsNullOrEmpty(string? text, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.IsNullOrEmpty(text, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must not be <see langword="null"/> or empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> that is not <see langword="null"/> or <see cref="string.Empty"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is empty.</exception>
    /// <seealso cref="string.IsNullOrEmpty(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static string IsNotNullOrEmpty([NotNull] string? text, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.IsNotNullOrEmpty(text, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> that is <see langword="null"/> or whitespace.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is neither <see langword="null"/> nor whitespace.</exception>
    /// <seealso cref="string.IsNullOrWhiteSpace(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? IsNullOrWhiteSpace(string? text, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.IsNullOrWhiteSpace(text, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must not be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is <see langword="null"/>.</exception>
    /// <returns>The <paramref name="text"/> that is not <see langword="null"/> or whitespace.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is whitespace.</exception>
    /// <seealso cref="string.IsNullOrWhiteSpace(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsNotNullOrWhiteSpace([NotNull] string? text, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.IsNotNullOrWhiteSpace(text, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must be empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> that is <see cref="string.Empty"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is empty.</exception>
    /// <seealso cref="string.IsNullOrEmpty(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsEmpty(string text, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.IsEmpty(text, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must not be empty.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> that is not <see cref="string.Empty"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is empty.</exception>
    /// <seealso cref="string.IsNullOrEmpty(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsNotEmpty(string text, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.IsNotEmpty(text, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must be whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> that is whitespace.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is neither <see langword="null"/> nor whitespace.</exception>
    /// <seealso cref="string.IsNullOrWhiteSpace(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsWhiteSpace(string text, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.IsWhiteSpace(text, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must not be <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> that is not whitespace.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="text"/> is <see langword="null"/> or whitespace.</exception>
    /// <seealso cref="string.IsNullOrWhiteSpace(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string IsNotWhiteSpace(string text, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.IsNotWhiteSpace(text, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size of a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> whose length is == <paramref name="size"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is != <paramref name="size"/>.</exception>
    /// <seealso cref="string.Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeEqualTo(string text, int size, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.HasSizeEqualTo(text, size, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size not equal to a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> whose length is != <paramref name="size"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is == <paramref name="size"/>.</exception>
    /// <seealso cref="string.Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeNotEqualTo(string text, int size, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.HasSizeNotEqualTo(text, size, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size over a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> whose length is > <paramref name="size"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is &lt;= <paramref name="size"/>.</exception>
    /// <seealso cref="string.Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeGreaterThan(string text, int size, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.HasSizeGreaterThan(text, size, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size of at least specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> whose length is >= <paramref name="size"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is &lt; <paramref name="size"/>.</exception>
    /// <seealso cref="string.Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeGreaterThanOrEqualTo(string text, int size, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.HasSizeGreaterThanOrEqualTo(text, size, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size of less than a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> whose length is &lt; <paramref name="size"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is >= <paramref name="size"/>.</exception>
    /// <seealso cref="string.Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeLessThan(string text, int size, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.HasSizeLessThan(text, size, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input <see cref="string"/> instance must have a size of less than or equal to a specified value.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to check the size for.</param>
    /// <param name="size">The target size to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> whose length is &lt;= <paramref name="size"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is > <paramref name="size"/>.</exception>
    /// <seealso cref="string.Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeLessThanOrEqualTo(string text, int size, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.HasSizeLessThanOrEqualTo(text, size, name);
        return text;
    }

    /// <summary>
    /// Asserts that the source <see cref="string"/> instance must have the same size of a destination <see cref="string"/> instance.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="string"/> instance to check the size for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> whose length is == <paramref name="destination"/> length.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is != the one of <paramref name="destination"/>.</exception>
    /// <remarks>The <see cref="string"/> type is immutable, but the name of this API is kept for consistency with the other overloads.</remarks>
    /// <seealso cref="string.Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeEqualTo(string text, string destination, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.HasSizeEqualTo(text, destination, name);
        return text;
    }

    /// <summary>
    /// Asserts that the source <see cref="string"/> instance must have a size of less than or equal to that of a destination <see cref="string"/> instance.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> instance to check the size for.</param>
    /// <param name="destination">The destination <see cref="string"/> instance to check the size for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="text"/> whose length is &lt;= <paramref name="destination"/> length.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of <paramref name="text"/> is > the one of <paramref name="destination"/>.</exception>
    /// <remarks>The <see cref="string"/> type is immutable, but the name of this API is kept for consistency with the other overloads.</remarks>
    /// <seealso cref="string.Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string HasSizeLessThanOrEqualTo(string text, string destination, [CallerArgumentExpression(nameof(text))] string name = "")
    {
        Guard.HasSizeLessThanOrEqualTo(text, destination, name);
        return text;
    }

    /// <summary>
    /// Asserts that the input index is valid for a given <see cref="string"/> instance.
    /// </summary>
    /// <param name="index">The input index to be used to access <paramref name="text"/>.</param>
    /// <param name="text">The input <see cref="string"/> instance to use to validate <paramref name="index"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="index"/>, which is valid for this <paramref name="text"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is not valid to access <paramref name="text"/>.</exception>
    /// <seealso cref="string.this[int]"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IsInRangeFor(int index, string text, [CallerArgumentExpression(nameof(index))] string name = "")
    {
        Guard.IsInRangeFor(index, text, name);
        return index;
    }

    /// <summary>
    /// Asserts that the input index is not valid for a given <see cref="string"/> instance.
    /// </summary>
    /// <param name="index">The input index to be used to access <paramref name="text"/>.</param>
    /// <param name="text">The input <see cref="string"/> instance to use to validate <paramref name="index"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="index"/>, which is not valid for this <paramref name="text"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is valid to access <paramref name="text"/>.</exception>
    /// <seealso cref="string.this[int]"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IsNotInRangeFor(int index, string text, [CallerArgumentExpression(nameof(index))] string name = "")
    {
        Guard.IsNotInRangeFor(index, text, name);
        return index;
    }
}

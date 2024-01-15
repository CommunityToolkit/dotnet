// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class FGuard
{
    /// <summary>
    /// Asserts that the input <see cref="Stream"/> instance must support reading.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="stream"/>, which supports reading.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> doesn't support reading.</exception>
    /// <seealso cref="Stream.CanRead"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream CanRead(Stream stream, [CallerArgumentExpression(nameof(stream))] string name = "")
    {
        Guard.CanRead(stream, name);
        return stream;
    }

    /// <summary>
    /// Asserts that the input <see cref="Stream"/> instance must support writing.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="stream"/>, which supports writing.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> doesn't support writing.</exception>
    /// <seealso cref="Stream.CanWrite"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream CanWrite(Stream stream, [CallerArgumentExpression(nameof(stream))] string name = "")
    {
        Guard.CanWrite(stream, name);
        return stream;
    }

    /// <summary>
    /// Asserts that the input <see cref="Stream"/> instance must support seeking.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="stream"/>, which supports seeking.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> doesn't support seeking.</exception>
    /// <seealso cref="Stream.CanSeek"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream CanSeek(Stream stream, [CallerArgumentExpression(nameof(stream))] string name = "")
    {
        Guard.CanSeek(stream, name);
        return stream;
    }

    /// <summary>
    /// Asserts that the input <see cref="Stream"/> instance must be at the starting position.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="stream"/>, which is at the starting position.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> is not at the starting position.</exception>
    /// <seealso cref="Stream.Position"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream IsAtStartPosition(Stream stream, [CallerArgumentExpression(nameof(stream))] string name = "")
    {
        Guard.IsAtStartPosition(stream, name);
        return stream;
    }
}

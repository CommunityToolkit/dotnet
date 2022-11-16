// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <summary>
    /// Asserts that the input <see cref="Stream"/> instance must support reading.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> doesn't support reading.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CanRead(Stream stream, [CallerArgumentExpression(nameof(stream))] string name = "")
    {
        if (stream.CanRead)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForCanRead(stream, name);
    }

    /// <summary>
    /// Asserts that the input <see cref="Stream"/> instance must support writing.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> doesn't support writing.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CanWrite(Stream stream, [CallerArgumentExpression(nameof(stream))] string name = "")
    {
        if (stream.CanWrite)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForCanWrite(stream, name);
    }

    /// <summary>
    /// Asserts that the input <see cref="Stream"/> instance must support seeking.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> doesn't support seeking.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CanSeek(Stream stream, [CallerArgumentExpression(nameof(stream))] string name = "")
    {
        if (stream.CanSeek)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForCanSeek(stream, name);
    }

    /// <summary>
    /// Asserts that the input <see cref="Stream"/> instance must be at the starting position.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> is not at the starting position.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsAtStartPosition(Stream stream, [CallerArgumentExpression(nameof(stream))] string name = "")
    {
        if (stream.Position == 0)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsAtStartPosition(stream, name);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Check
{
    /// <summary>
    /// Checks that the input <see cref="Stream"/> instance must support reading.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanRead(Stream stream)
    {
        return stream.CanRead;
    }

    /// <summary>
    /// Checks that the input <see cref="Stream"/> instance must support writing.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanWrite(Stream stream)
    {
        return stream.CanWrite;
    }

    /// <summary>
    /// Checks that the input <see cref="Stream"/> instance must support seeking.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanSeek(Stream stream)
    {
        return stream.CanSeek;
    }

    /// <summary>
    /// Checks that the input <see cref="Stream"/> instance must be at the starting position.
    /// </summary>
    /// <param name="stream">The input <see cref="Stream"/> instance to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAtStartPosition(Stream stream)
    {
        return stream.Position == 0;
    }
}

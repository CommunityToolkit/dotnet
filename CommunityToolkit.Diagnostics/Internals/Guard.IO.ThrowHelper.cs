// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <inheritdoc/>
    partial class ThrowHelper
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="CanRead"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForCanRead(Stream stream, string name)
        {
            throw new ArgumentException($"Stream {AssertString(name)} ({stream.GetType().ToTypeString()}) doesn't support reading.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="CanWrite"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForCanWrite(Stream stream, string name)
        {
            throw new ArgumentException($"Stream {AssertString(name)} ({stream.GetType().ToTypeString()}) doesn't support writing.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="CanSeek"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForCanSeek(Stream stream, string name)
        {
            throw new ArgumentException($"Stream {AssertString(name)} ({stream.GetType().ToTypeString()}) doesn't support seeking.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsAtStartPosition"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsAtStartPosition(Stream stream, string name)
        {
            throw new ArgumentException($"Stream {AssertString(name)} ({stream.GetType().ToTypeString()}) must be at position {AssertString(0)}, was at {AssertString(stream.Position)}.", name);
        }
    }
}

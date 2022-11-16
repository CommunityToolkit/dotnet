// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <inheritdoc/>
    partial class ThrowHelper
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotEmpty{T}(Span{T},string)"/> fails.
        /// </summary>
        /// <typeparam name="T">The item of items in the input <see cref="Span{T}"/> instance.</typeparam>
        /// <remarks>This method is needed because <see cref="Span{T}"/> can't be used as a generic type parameter.</remarks>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotEmptyWithSpan<T>(string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(Span<T>).ToTypeString()}) must not be empty.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotEmpty{T}(ReadOnlySpan{T},string)"/> fails.
        /// </summary>
        /// <typeparam name="T">The item of items in the input <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
        /// <remarks>This method is needed because <see cref="ReadOnlySpan{T}"/> can't be used as a generic type parameter.</remarks>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotEmptyWithReadOnlySpan<T>(string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(ReadOnlySpan<T>).ToTypeString()}) must not be empty.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotEmpty{T}(T[],string)"/> (or an overload) fails.
        /// </summary>
        /// <typeparam name="T">The item of items in the input collection.</typeparam>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotEmpty<T>(string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(T).ToTypeString()}) must not be empty.", name);
        }
    }
}

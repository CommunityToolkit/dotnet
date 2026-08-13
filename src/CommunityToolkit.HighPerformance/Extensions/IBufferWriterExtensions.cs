// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.HighPerformance.Streams;

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// Helpers for working with the <see cref="IBufferWriter{T}"/> type.
/// </summary>
public static class IBufferWriterExtensions
{
    /// <summary>
    /// Returns a <see cref="Stream"/> that can be used to write to a target an <see cref="IBufferWriter{T}"/> of <see cref="byte"/> instance.
    /// </summary>
    /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance.</param>
    /// <returns>A <see cref="Stream"/> wrapping <paramref name="writer"/> and writing data to its underlying buffer.</returns>
    /// <remarks>The returned <see cref="Stream"/> can only be written to and does not support seeking.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream AsStream(this IBufferWriter<byte> writer)
    {
        if (writer.GetType() == typeof(ArrayPoolBufferWriter<byte>))
        {
            // If the input writer is of type ArrayPoolBufferWriter<byte>, we can use the type
            // specific buffer writer owner to let the JIT elide callvirts when accessing it.
            ArrayPoolBufferWriter<byte>? internalWriter = Unsafe.As<ArrayPoolBufferWriter<byte>>(writer)!;

            return new IBufferWriterStream<ArrayBufferWriterOwner>(new ArrayBufferWriterOwner(internalWriter));
        }

        return new IBufferWriterStream<IBufferWriterOwner>(new IBufferWriterOwner(writer));
    }

    /// <summary>
    /// Writes a value of a specified type into a target <see cref="IBufferWriter{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of value to write.</typeparam>
    /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance to write to.</param>
    /// <param name="value">The input value to write to <paramref name="writer"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="writer"/> reaches the end.</exception>
    /// <remarks>
    /// The <c>sizeHint</c> passed to <see cref="IBufferWriter{T}.GetSpan"/> is a hint, not a demand: a writer is
    /// free to return less. The write is therefore delegated to <see cref="BuffersExtensions"/>, which loops over
    /// whatever spans it is given, rather than requiring <paramref name="value"/> to fit in a single span.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Write<T>(this IBufferWriter<byte> writer, T value)
        where T : unmanaged
    {
        // the address of a parameter is stack-based, so this needs no pinning
        BuffersExtensions.Write(writer, new ReadOnlySpan<byte>(&value, sizeof(T)));
    }

    /// <summary>
    /// Writes a value of a specified type into a target <see cref="IBufferWriter{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of value to write.</typeparam>
    /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance to write to.</param>
    /// <param name="value">The input value to write to <paramref name="writer"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="writer"/> reaches the end.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this IBufferWriter<T> writer, T value)
    {
        Span<T> span = writer.GetSpan(1);

        if (span.Length < 1)
        {
            ThrowArgumentExceptionForEndOfBuffer();
        }

        MemoryMarshal.GetReference(span) = value;

        writer.Advance(1);
    }

    /// <summary>
    /// Writes a series of items of a specified type into a target <see cref="IBufferWriter{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of value to write.</typeparam>
    /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance to write to.</param>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> to write to <paramref name="writer"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="writer"/> reaches the end.</exception>
    /// <remarks>
    /// The <c>sizeHint</c> passed to <see cref="IBufferWriter{T}.GetSpan"/> is a hint, not a demand: a writer is
    /// free to return less. The write is therefore delegated to <see cref="BuffersExtensions"/>, which loops over
    /// whatever spans it is given, rather than requiring <paramref name="span"/> to fit in a single span.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this IBufferWriter<byte> writer, ReadOnlySpan<T> span)
        where T : unmanaged
    {
        BuffersExtensions.Write(writer, MemoryMarshal.AsBytes(span));
    }

#if !NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Writes a series of items of a specified type into a target <see cref="IBufferWriter{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of value to write.</typeparam>
    /// <param name="writer">The target <see cref="IBufferWriter{T}"/> instance to write to.</param>
    /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> to write to <paramref name="writer"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="writer"/> reaches the end.</exception>
    /// <remarks>
    /// This is not an extension method: <see cref="BuffersExtensions.Write"/> has the same signature and is always
    /// available (the System.Memory package is a dependency of this package on netstandard2.0), so an extension
    /// method here would only be an ambiguity. It remains for binary compatibility.
    /// </remarks>
    [Obsolete("Use System.Buffers.BuffersExtensions.Write instead; this overload exists only for binary compatibility.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(IBufferWriter<T> writer, ReadOnlySpan<T> span)
    {
        BuffersExtensions.Write(writer, span);
    }
#endif

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when trying to write too many bytes to the target writer.
    /// </summary>
    private static void ThrowArgumentExceptionForEndOfBuffer()
    {
        throw new ArgumentException("The current buffer writer can't contain the requested input data.");
    }
}

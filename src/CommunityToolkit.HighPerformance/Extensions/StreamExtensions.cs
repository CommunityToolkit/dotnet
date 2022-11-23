// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !NETSTANDARD2_1_OR_GREATER
using System.Buffers;
#endif
using System.Threading;
using System.Threading.Tasks;
#if NETSTANDARD2_1_OR_GREATER
using System.ComponentModel;
#endif

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// Helpers for working with the <see cref="Stream"/> type.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Asynchronously reads a sequence of bytes from a given <see cref="Stream"/> instance.
    /// </summary>
    /// <param name="stream">The source <see cref="Stream"/> to read data from.</param>
    /// <param name="buffer">The destination <see cref="Memory{T}"/> to write data to.</param>
    /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> for the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the operation being performed.</returns>
#if NETSTANDARD2_1_OR_GREATER
    [Obsolete("This API is only available for binary compatibility, but Stream.ReadAsync should be used instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static ValueTask<int> ReadAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER
        return stream.ReadAsync(buffer, cancellationToken);
#else
        if (cancellationToken.IsCancellationRequested)
        {
            return new(Task.FromCanceled<int>(cancellationToken));
        }

        // If the memory wraps an array, extract it and use it directly
        if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> segment))
        {
            return new(stream.ReadAsync(segment.Array!, segment.Offset, segment.Count, cancellationToken));
        }

        // Local function used as the fallback path. This happens when the input memory
        // doesn't wrap an array instance we can use. We use a local function as we need
        // the body to be asynchronous, in order to execute the finally block after the
        // write operation has been completed. By separating the logic, we can keep the
        // main method as a synchronous, value-task returning function. This fallback
        // path should hopefully be pretty rare, as memory instances are typically just
        // created around arrays, often being rented from a memory pool in particular.
        static async Task<int> ReadAsyncFallback(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
        {
            byte[] rent = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                int bytesRead = await stream.ReadAsync(rent, 0, buffer.Length, cancellationToken).ConfigureAwait(false);

                if (bytesRead > 0)
                {
                    rent.AsSpan(0, bytesRead).CopyTo(buffer.Span);
                }

                return bytesRead;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rent);
            }
        }

        return new(ReadAsyncFallback(stream, buffer, cancellationToken));
#endif
    }

    /// <summary>
    /// Asynchronously writes a sequence of bytes to a given <see cref="Stream"/> instance.
    /// </summary>
    /// <param name="stream">The destination <see cref="Stream"/> to write data to.</param>
    /// <param name="buffer">The source <see cref="ReadOnlyMemory{T}"/> to read data from.</param>
    /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> for the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the operation being performed.</returns>
#if NETSTANDARD2_1_OR_GREATER
    [Obsolete("This API is only available for binary compatibility, but Stream.WriteAsync should be used instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static ValueTask WriteAsync(this Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER
        return stream.WriteAsync(buffer, cancellationToken);
#else
        if (cancellationToken.IsCancellationRequested)
        {
            return new(Task.FromCanceled(cancellationToken));
        }

        if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> segment))
        {
            return new(stream.WriteAsync(segment.Array!, segment.Offset, segment.Count, cancellationToken));
        }

        // Local function, same idea as above
        static async Task WriteAsyncFallback(Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            byte[] rent = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                buffer.Span.CopyTo(rent);

                await stream.WriteAsync(rent, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rent);
            }
        }

        return new(WriteAsyncFallback(stream, buffer, cancellationToken));
#endif
    }

    /// <summary>
    /// Reads a sequence of bytes from a given <see cref="Stream"/> instance.
    /// </summary>
    /// <param name="stream">The source <see cref="Stream"/> to read data from.</param>
    /// <param name="buffer">The target <see cref="Span{T}"/> to write data to.</param>
    /// <returns>The number of bytes that have been read.</returns>
#if NETSTANDARD2_1_OR_GREATER
    [Obsolete("This API is only available for binary compatibility, but Stream.Read should be used instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int Read(this Stream stream, Span<byte> buffer)
    {
#if NETSTANDARD2_1_OR_GREATER
        return stream.Read(buffer);
#else
        byte[] rent = ArrayPool<byte>.Shared.Rent(buffer.Length);

        try
        {
            int bytesRead = stream.Read(rent, 0, buffer.Length);

            if (bytesRead > 0)
            {
                rent.AsSpan(0, bytesRead).CopyTo(buffer);
            }

            return bytesRead;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rent);
        }
#endif
    }

    /// <summary>
    /// Writes a sequence of bytes to a given <see cref="Stream"/> instance.
    /// </summary>
    /// <param name="stream">The destination <see cref="Stream"/> to write data to.</param>
    /// <param name="buffer">The source <see cref="Span{T}"/> to read data from.</param>
#if NETSTANDARD2_1_OR_GREATER
    [Obsolete("This API is only available for binary compatibility, but Stream.Read should be used instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
    {
#if NETSTANDARD2_1_OR_GREATER
        stream.Write(buffer);
#else
        byte[] rent = ArrayPool<byte>.Shared.Rent(buffer.Length);

        try
        {
            buffer.CopyTo(rent);

            stream.Write(rent, 0, buffer.Length);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rent);
        }
#endif
    }

    /// <summary>
    /// Reads a value of a specified type from a source <see cref="Stream"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="stream">The source <see cref="Stream"/> instance to read from.</param>
    /// <returns>The <typeparamref name="T"/> value read from <paramref name="stream"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="stream"/> reaches the end.</exception>
#if NETSTANDARD2_1_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static unsafe T Read<T>(this Stream stream)
        where T : unmanaged
    {
#if NETSTANDARD2_1_OR_GREATER
        T result = default;
        int length = sizeof(T);

        unsafe
        {
            if (stream.Read(new Span<byte>(&result, length)) != length)
            {
                ThrowInvalidOperationExceptionForEndOfStream();
            }
        }

        return result;
#else
        int length = sizeof(T);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

        try
        {
            if (stream.Read(buffer, 0, length) != length)
            {
                ThrowInvalidOperationExceptionForEndOfStream();
            }

            return Unsafe.ReadUnaligned<T>(ref buffer[0]);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
#endif
    }

    /// <summary>
    /// Writes a value of a specified type into a target <see cref="Stream"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of value to write.</typeparam>
    /// <param name="stream">The target <see cref="Stream"/> instance to write to.</param>
    /// <param name="value">The input value to write to <paramref name="stream"/>.</param>
#if NETSTANDARD2_1_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static unsafe void Write<T>(this Stream stream, in T value)
        where T : unmanaged
    {
#if NETSTANDARD2_1_OR_GREATER
        ref T r0 = ref Unsafe.AsRef(value);
        ref byte r1 = ref Unsafe.As<T, byte>(ref r0);
        int length = sizeof(T);

        ReadOnlySpan<byte> span = MemoryMarshal.CreateReadOnlySpan(ref r1, length);

        stream.Write(span);
#else
        int length = sizeof(T);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

        try
        {
            Unsafe.WriteUnaligned(ref buffer[0], value);

            stream.Write(buffer, 0, length);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
#endif
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> when <see cref="Read{T}"/> fails.
    /// </summary>
    private static void ThrowInvalidOperationExceptionForEndOfStream()
    {
        throw new InvalidOperationException("The stream didn't contain enough data to read the requested item.");
    }
}

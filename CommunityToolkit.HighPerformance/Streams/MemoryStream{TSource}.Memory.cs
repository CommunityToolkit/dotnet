// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NETSTANDARD2_1_OR_GREATER

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityToolkit.HighPerformance.Streams;

/// <inheritdoc/>
partial class MemoryStream<TSource>
{
    /// <inheritdoc/>
    public sealed override void CopyTo(Stream destination, int bufferSize)
    {
        MemoryStream.ValidateDisposed(this.disposed);

        Span<byte> source = this.source.Span.Slice(this.position);

        this.position += source.Length;

        destination.Write(source);
    }

    /// <inheritdoc/>
    public sealed override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return new(Task.FromCanceled<int>(cancellationToken));
        }

        try
        {
            int result = Read(buffer.Span);

            return new(result);
        }
        catch (OperationCanceledException e)
        {
            return new(Task.FromCanceled<int>(e.CancellationToken));
        }
        catch (Exception e)
        {
            return new(Task.FromException<int>(e));
        }
    }

    /// <inheritdoc/>
    public sealed override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return new(Task.FromCanceled(cancellationToken));
        }

        try
        {
            Write(buffer.Span);

            return default;
        }
        catch (OperationCanceledException e)
        {
            return new(Task.FromCanceled(e.CancellationToken));
        }
        catch (Exception e)
        {
            return new(Task.FromException(e));
        }
    }

    /// <inheritdoc/>
    public sealed override int Read(Span<byte> buffer)
    {
        MemoryStream.ValidateDisposed(this.disposed);

        int
            bytesAvailable = this.source.Length - this.position,
            bytesCopied = Math.Min(bytesAvailable, buffer.Length);

        Span<byte> source = this.source.Span.Slice(this.position, bytesCopied);

        source.CopyTo(buffer);

        this.position += bytesCopied;

        return bytesCopied;
    }

    /// <inheritdoc/>
    public sealed override void Write(ReadOnlySpan<byte> buffer)
    {
        MemoryStream.ValidateDisposed(this.disposed);
        MemoryStream.ValidateCanWrite(CanWrite);

        Span<byte> destination = this.source.Span.Slice(this.position);

        if (!buffer.TryCopyTo(destination))
        {
            MemoryStream.ThrowArgumentExceptionForEndOfStreamOnWrite();
        }

        this.position += buffer.Length;
    }
}

#endif
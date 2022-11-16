// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

#if NETSTANDARD2_1_OR_GREATER

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityToolkit.HighPerformance.Streams;

/// <inheritdoc/>
partial class IBufferWriterStream<TWriter>
{
    /// <inheritdoc/>
    public override void CopyTo(Stream destination, int bufferSize)
    {
        throw MemoryStream.GetNotSupportedException();
    }

    /// <inheritdoc/>
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        throw MemoryStream.GetNotSupportedException();
    }

    /// <inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
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
    public override int Read(Span<byte> buffer)
    {
        throw MemoryStream.GetNotSupportedException();
    }

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        MemoryStream.ValidateDisposed(this.disposed);

        Span<byte> destination = this.bufferWriter.GetSpan(buffer.Length);

        if (!buffer.TryCopyTo(destination))
        {
            MemoryStream.ThrowArgumentExceptionForEndOfStreamOnWrite();
        }

        this.bufferWriter.Advance(buffer.Length);
    }
}

#endif
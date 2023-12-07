// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityToolkit.HighPerformance.Streams;

/// <summary>
/// A <see cref="Stream"/> implementation wrapping a <see cref="ReadOnlySequence{T}"/> of <see cref="byte"/> instance.
/// </summary>
internal sealed partial class ReadOnlySequenceStream : Stream
{
    /// <summary>
    /// The <see cref="ReadOnlySequence{T}"/> instance currently in use.
    /// </summary>
    private readonly ReadOnlySequence<byte> source;

    /// <summary>
    /// The current position within <see cref="source"/>.
    /// </summary>
    private int position;

    /// <summary>
    /// Indicates whether or not the current instance has been disposed
    /// </summary>
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySequenceStream"/> class with the specified <see cref="ReadOnlySequence{T}"/> source.
    /// </summary>
    /// <param name="source">The <see cref="ReadOnlySequence{T}"/> source.</param>
    public ReadOnlySequenceStream(ReadOnlySequence<byte> source)
    {
        this.source = source;
    }

    /// <inheritdoc/>
    public sealed override bool CanRead
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !this.disposed;
    }

    /// <inheritdoc/>
    public sealed override bool CanSeek
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !this.disposed;
    }

    /// <inheritdoc/>
    public sealed override bool CanWrite
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => false;
    }

    /// <inheritdoc/>
    public sealed override long Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            MemoryStream.ValidateDisposed(this.disposed);

            return this.source.Length;
        }
    }

    /// <inheritdoc/>
    public sealed override long Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            MemoryStream.ValidateDisposed(this.disposed);

            return this.position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            MemoryStream.ValidateDisposed(this.disposed);
            MemoryStream.ValidatePosition(value, this.source.Length);

            this.position = unchecked((int)value);
        }
    }

    /// <summary>
    /// Creates a new <see cref="Stream"/> from the input <see cref="ReadOnlySequence{T}"/> of <see cref="byte"/> instance.
    /// </summary>
    /// <param name="sequence">The input <see cref="ReadOnlySequence{T}"/> instance.</param>
    /// <returns>A <see cref="Stream"/> wrapping the underlying data for <paramref name="sequence"/>.</returns>
    public static Stream Create(ReadOnlySequence<byte> sequence)
    {
        return new ReadOnlySequenceStream(sequence);
    }

    /// <inheritdoc/>
    public sealed override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        try
        {
            MemoryStream.ValidateDisposed(this.disposed);

            if (this.position >= this.source.Length)
            {
                return Task.CompletedTask;
            }

            if (this.source.IsSingleSegment)
            {
                ReadOnlyMemory<byte> buffer = this.source.First.Slice(this.position);

                this.position = (int)this.source.Length;

                return destination.WriteAsync(buffer, cancellationToken).AsTask();
            }

            async Task CoreCopyToAsync(Stream destination, CancellationToken cancellationToken)
            {
                ReadOnlySequence<byte> sequence = this.source.Slice(this.position);

                this.position = (int)this.source.Length;

                foreach (ReadOnlyMemory<byte> segment in sequence)
                {
                    await destination.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
                }
            }

            return CoreCopyToAsync(destination, cancellationToken);
        }
        catch (OperationCanceledException e)
        {
            return Task.FromCanceled(e.CancellationToken);
        }
        catch (Exception e)
        {
            return Task.FromException(e);
        }
    }

    /// <inheritdoc/>
    public sealed override void Flush()
    {
    }

    /// <inheritdoc/>
    public sealed override Task FlushAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public sealed override Task<int> ReadAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<int>(cancellationToken);
        }

        try
        {
            int result = Read(buffer, offset, count);

            return Task.FromResult(result);
        }
        catch (OperationCanceledException e)
        {
            return Task.FromCanceled<int>(e.CancellationToken);
        }
        catch (Exception e)
        {
            return Task.FromException<int>(e);
        }
    }

    public sealed override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        throw MemoryStream.GetNotSupportedException();
    }

    /// <inheritdoc/>
    public sealed override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        throw MemoryStream.GetNotSupportedException();
    }

    /// <inheritdoc/>
    public sealed override long Seek(long offset, SeekOrigin origin)
    {
        MemoryStream.ValidateDisposed(this.disposed);

        long index = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => this.position + offset,
            SeekOrigin.End => this.source.Length + offset,
            _ => MemoryStream.ThrowArgumentExceptionForSeekOrigin()
        };

        MemoryStream.ValidatePosition(index, this.source.Length);

        this.position = unchecked((int)index);

        return index;
    }

    /// <inheritdoc/>
    public sealed override void SetLength(long value)
    {
        throw MemoryStream.GetNotSupportedException();
    }

    /// <inheritdoc/>
    public sealed override int Read(byte[]? buffer, int offset, int count)
    {
        MemoryStream.ValidateDisposed(this.disposed);
        MemoryStream.ValidateBuffer(buffer, offset, count);

        if (this.position >= this.source.Length)
        {
            return 0;
        }

        ReadOnlySequence<byte> sequence = this.source.Slice(this.position);
        Span<byte> destination = buffer.AsSpan(offset, count);
        int bytesCopied = 0;

        foreach (ReadOnlyMemory<byte> segment in sequence)
        {
            int bytesToCopy = Math.Min(segment.Length, destination.Length);

            segment.Span.Slice(0, bytesToCopy).CopyTo(destination);

            destination = destination.Slice(bytesToCopy);
            bytesCopied += bytesToCopy;

            this.position += bytesToCopy;

            if (destination.Length == 0)
            {
                break;
            }
        }

        return bytesCopied;
    }

    /// <inheritdoc/>
    public sealed override int ReadByte()
    {
        MemoryStream.ValidateDisposed(this.disposed);

        if (this.position == this.source.Length)
        {
            return -1;
        }

        ReadOnlySequence<byte> sequence = this.source.Slice(this.position);

        this.position++;

        return sequence.First.Span[0];
    }

    /// <inheritdoc/>
    public sealed override void Write(byte[]? buffer, int offset, int count)
    {
        throw MemoryStream.GetNotSupportedException();
    }

    /// <inheritdoc/>
    public sealed override void WriteByte(byte value)
    {
        throw MemoryStream.GetNotSupportedException();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
    }
}

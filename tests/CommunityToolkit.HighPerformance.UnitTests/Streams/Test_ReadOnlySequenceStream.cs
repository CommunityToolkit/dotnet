// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Streams;

[TestClass]
public partial class Test_ReadOnlySequenceStream
{
    [TestMethod]
    public void Test_ReadOnlySequenceStream_Lifecycle()
    {
        ReadOnlySequence<byte> sequence = CreateReadOnlySequence(new byte[100]);

        Stream stream = sequence.AsStream();

        Assert.IsTrue(stream.CanRead);
        Assert.IsTrue(stream.CanSeek);
        Assert.IsFalse(stream.CanWrite);
        Assert.AreEqual(stream.Length, sequence.Length);
        Assert.AreEqual(stream.Position, 0);

        stream.Dispose();

        Assert.IsFalse(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsFalse(stream.CanWrite);

        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Length);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Position);
    }

    [TestMethod]
    public void Test_ReadOnlySequenceStream_Seek()
    {
        Stream stream = CreateReadOnlySequence(new byte[50], new byte[50]).AsStream();

        Assert.AreEqual(stream.Position, 0);

        stream.Position = 42;

        Assert.AreEqual(stream.Position, 42);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Position = -1);
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Position = 120);

        _ = stream.Seek(0, SeekOrigin.Begin);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Seek(-1, SeekOrigin.Begin));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Seek(120, SeekOrigin.Begin));

        Assert.AreEqual(stream.Position, 0);

        _ = stream.Seek(-1, SeekOrigin.End);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Seek(20, SeekOrigin.End));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Seek(-120, SeekOrigin.End));

        Assert.AreEqual(stream.Position, stream.Length - 1);

        _ = stream.Seek(42, SeekOrigin.Begin);
        _ = stream.Seek(20, SeekOrigin.Current);
        _ = stream.Seek(-30, SeekOrigin.Current);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Seek(-64, SeekOrigin.Current));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Seek(80, SeekOrigin.Current));

        Assert.AreEqual(stream.Position, 32);
    }

    [TestMethod]
    public void Test_ReadOnlySequenceStream_Read_Array()
    {
        Memory<byte> data = CreateRandomData(64);

        Stream stream = CreateReadOnlySequence(data).AsStream();

        stream.Position = 0;

        byte[] result = new byte[data.Length];

        int bytesRead = stream.Read(result, 0, result.Length);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result));

        stream.Dispose();

        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Read(result, 0, result.Length));
    }

    [TestMethod]
    public void Test_ReadOnlySequenceStream_NotFromStart_Read_Array()
    {
        const int offset = 8;

        Memory<byte> data = CreateRandomData(64);

        Stream stream = CreateReadOnlySequence(data).AsStream();

        stream.Position = offset;

        byte[] result = new byte[data.Length - offset];

        int bytesRead = stream.Read(result, 0, result.Length);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(stream.Position, result.Length + offset);
        Assert.IsTrue(data.Span.Slice(offset).SequenceEqual(result));

        stream.Dispose();

        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Read(result, 0, result.Length));
    }

    [TestMethod]
    public void Test_ReadOnlySequenceStream_ReadByte()
    {
        Memory<byte> data = new byte[] { 1, 128, 255, 32 };

        Stream stream = CreateReadOnlySequence(data.Slice(0,2), data.Slice(2, 2)).AsStream();

        Span<byte> result = stackalloc byte[4];

        foreach (ref byte value in result)
        {
            value = checked((byte)stream.ReadByte());
        }

        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result));

        int exitCode = stream.ReadByte();

        Assert.AreEqual(exitCode, -1);
    }

    [TestMethod]
    public void Test_ReadOnlySequenceStream_Read_Span()
    {
        Memory<byte> data = CreateRandomData(64);

        Stream stream = CreateReadOnlySequence(data).AsStream();

        stream.Position = 0;

        Span<byte> result = new byte[data.Length];

        int bytesRead = stream.Read(result);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result));
    }

    [TestMethod]
    public async Task Test_ReadOnlySequenceStream_ReadAsync_Memory()
    {
        Memory<byte> data = CreateRandomData(64);

        Stream stream = CreateReadOnlySequence(data).AsStream();

        Memory<byte> result = new byte[data.Length];

        int bytesRead = await stream.ReadAsync(result);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result.Span));
    }

    [TestMethod]
    public void Test_ReadOnlySequenceStream_SigleSegment_CopyTo()
    {
        Memory<byte> data = CreateRandomData(64);

        Stream source = CreateReadOnlySequence(data).AsStream();

        Stream destination = new byte[100].AsMemory().AsStream();

        source.CopyTo(destination);

        Assert.AreEqual(source.Position, destination.Position);

        destination.Position = 0;

        Memory<byte> result = new byte[data.Length];

        int bytesRead = destination.Read(result.Span);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(destination.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result.Span));
    }

    [TestMethod]
    public void Test_ReadOnlySequenceStream_CopyTo()
    {
        Memory<byte> data = CreateRandomData(64);

        Stream source = CreateReadOnlySequence(data.Slice(0, 32), data.Slice(32)).AsStream();

        Stream destination = new byte[100].AsMemory().AsStream();

        source.CopyTo(destination);

        Assert.AreEqual(source.Position, destination.Position);

        destination.Position = 0;

        Memory<byte> result = new byte[data.Length];

        int bytesRead = destination.Read(result.Span);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(destination.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result.Span));
    }

    [TestMethod]
    public async Task Test_ReadOnlySequenceStream_SigleSegment_CopyToAsync()
    {
        Memory<byte> data = CreateRandomData(64);

        Stream source = CreateReadOnlySequence(data).AsStream();

        Stream destination = new byte[100].AsMemory().AsStream();

        await source.CopyToAsync(destination);

        Assert.AreEqual(source.Position, destination.Position);

        destination.Position = 0;

        Memory<byte> result = new byte[data.Length];

        int bytesRead = await destination.ReadAsync(result);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(destination.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result.Span));
    }

    [TestMethod]
    public async Task Test_ReadOnlySequenceStream_SigleSegment_NotFromStart_CopyToAsync()
    {
        const int offset = 8;

        Memory<byte> data = CreateRandomData(64);

        Stream source = CreateReadOnlySequence(data).AsStream();

        source.Position = offset;

        Stream destination = new byte[100].AsMemory().AsStream();

        await source.CopyToAsync(destination);

        Assert.AreEqual(source.Position, destination.Position + offset);

        destination.Position = 0;

        Memory<byte> result = new byte[data.Length - offset];

        int bytesRead = await destination.ReadAsync(result);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(destination.Position, data.Length - offset);
        Assert.IsTrue(data.Span.Slice(offset).SequenceEqual(result.Span));
    }

    [TestMethod]
    public async Task Test_ReadOnlySequenceStream_MultipleSegments_CopyToAsync()
    {
        Memory<byte> data = CreateRandomData(64);

        Stream source = CreateReadOnlySequence(data.Slice(0, 16), data.Slice(16, 16), data.Slice(32, 16), data.Slice(48, 16)).AsStream();

        Stream destination = new byte[100].AsMemory().AsStream();

        await source.CopyToAsync(destination);

        Assert.AreEqual(source.Position, destination.Position);

        destination.Position = 0;

        Memory<byte> result = new byte[data.Length];

        int bytesRead = await destination.ReadAsync(result);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(destination.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result.Span));
    }

    [TestMethod]
    public async Task Test_ReadOnlySequenceStream_MultipleSegments_NotFromStart_CopyToAsync()
    {
        const int offset = 8;

        Memory<byte> data = CreateRandomData(64);

        Stream source = CreateReadOnlySequence(data.Slice(0, 16), data.Slice(16, 16), data.Slice(32, 16), data.Slice(48, 16)).AsStream();

        source.Position = offset;

        Stream destination = new byte[100].AsMemory().AsStream();

        await source.CopyToAsync(destination);

        Assert.AreEqual(source.Position, destination.Position + offset);

        destination.Position = 0;

        Memory<byte> result = new byte[data.Length - offset];

        int bytesRead = await destination.ReadAsync(result);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(destination.Position, data.Length - offset);
        Assert.IsTrue(data.Span.Slice(offset).SequenceEqual(result.Span));
    }

    /// <summary>
    /// Creates a random <see cref="byte"/> array filled with random data.
    /// </summary>
    /// <param name="count">The number of array items to create.</param>
    /// <returns>The returned random array.</returns>
    private static byte[] CreateRandomData(int count)
    {
        Random? random = new(DateTime.Now.Ticks.GetHashCode());

        byte[] data = new byte[count];

        foreach (ref byte n in MemoryMarshal.AsBytes(data.AsSpan()))
        {
            n = (byte)random.Next(0, byte.MaxValue);
        }

        return data;
    }

    /// <summary>
    /// Creates a <see cref="ReadOnlySequence{T}"/> value from the input segments.
    /// </summary>
    /// <param name="segments">The input <see cref="ReadOnlyMemory{T}"/> segments.</param>
    /// <returns>The resulting <see cref="ReadOnlySequence{T}"/> value.</returns>
    private static ReadOnlySequence<byte> CreateReadOnlySequence(params ReadOnlyMemory<byte>[] segments)
    {
        if (segments is not { Length: > 0 })
        {
            return ReadOnlySequence<byte>.Empty;
        }
        
        if (segments.Length == 1)
        {
            return new(segments[0]);
        }

        ReadOnlySequenceSegmentOfByte first = new(segments[0]);
        ReadOnlySequenceSegmentOfByte last = first;
        long length = first.Memory.Length;

        for (int i = 1; i < segments.Length; i++)
        {
            ReadOnlyMemory<byte> segment = segments[i];

            length += segment.Length;

            last = last.Append(segment);
        }

        return new(first, 0, last, (int)(length - last.RunningIndex));
    }

    /// <summary>
    /// A custom <see cref="ReadOnlySequenceSegment{T}"/> that supports appending new segments.
    /// </summary>
    private sealed class ReadOnlySequenceSegmentOfByte : ReadOnlySequenceSegment<byte>
    {
        public ReadOnlySequenceSegmentOfByte(ReadOnlyMemory<byte> memory)
        {
            Memory = memory;
        }

        public ReadOnlySequenceSegmentOfByte Append(ReadOnlyMemory<byte> memory)
        {
            ReadOnlySequenceSegmentOfByte nextSegment = new(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };

            Next = nextSegment;

            return nextSegment;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Streams;

[TestClass]
public class Test_IBufferWriterStream
{
    [TestMethod]
    public void Test_IBufferWriterStream_Lifecycle()
    {
        ArrayPoolBufferWriter<byte> writer = new();

        // Get a stream from a buffer writer and validate that it can only be written to.
        // This is to mirror the same functionality as the IBufferWriter<T> interface.
        Stream stream = ((IBufferWriter<byte>)writer).AsStream();

        Assert.IsFalse(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsTrue(stream.CanWrite);

        _ = Assert.ThrowsException<NotSupportedException>(() => stream.Length);
        _ = Assert.ThrowsException<NotSupportedException>(() => stream.Position);

        // Dispose the stream and check that no operation is now allowed
        stream.Dispose();

        Assert.IsFalse(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsFalse(stream.CanWrite);
        _ = Assert.ThrowsException<NotSupportedException>(() => stream.Length);
        _ = Assert.ThrowsException<NotSupportedException>(() => stream.Position);
    }

    [TestMethod]
    public void Test_IBufferWriterStream_Write_Array()
    {
        ArrayPoolBufferWriter<byte> writer = new();
        Stream stream = ((IBufferWriter<byte>)writer).AsStream();

        byte[] data = Test_MemoryStream.CreateRandomData(64);

        // Write random data to the stream wrapping the buffer writer, and validate
        // that the state of the writer is consistent, and the written content matches.
        stream.Write(data, 0, data.Length);

        Assert.AreEqual(writer.WrittenCount, data.Length);
        Assert.IsTrue(writer.WrittenSpan.SequenceEqual(data));

        // A few tests with invalid inputs (null buffers, invalid indices, etc.)
        _ = Assert.ThrowsException<ArgumentNullException>(() => stream.Write(null!, 0, 10));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Write(data, -1, 10));
        _ = Assert.ThrowsException<ArgumentException>(() => stream.Write(data, 200, 10));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Write(data, 0, -24));
        _ = Assert.ThrowsException<ArgumentException>(() => stream.Write(data, 0, 200));

        stream.Dispose();

        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Write(data, 0, data.Length));
    }

    [TestMethod]
    public async Task Test_IBufferWriterStream_WriteAsync_Array()
    {
        ArrayPoolBufferWriter<byte> writer = new();
        Stream stream = ((IBufferWriter<byte>)writer).AsStream();

        byte[] data = Test_MemoryStream.CreateRandomData(64);

        // Same test as above, but using an asynchronous write instead
        await stream.WriteAsync(data, 0, data.Length);

        Assert.AreEqual(writer.WrittenCount, data.Length);
        Assert.IsTrue(writer.WrittenSpan.SequenceEqual(data));

        _ = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stream.WriteAsync(null!, 0, 10));
        _ = await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => stream.WriteAsync(data, -1, 10));
        _ = await Assert.ThrowsExceptionAsync<ArgumentException>(() => stream.WriteAsync(data, 200, 10));
        _ = await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => stream.WriteAsync(data, 0, -24));
        _ = await Assert.ThrowsExceptionAsync<ArgumentException>(() => stream.WriteAsync(data, 0, 200));

        stream.Dispose();

        _ = await Assert.ThrowsExceptionAsync<ObjectDisposedException>(() => stream.WriteAsync(data, 0, data.Length));
    }

    [TestMethod]
    public void Test_IBufferWriterStream_WriteByte()
    {
        ArrayPoolBufferWriter<byte> writer = new();
        Stream stream = ((IBufferWriter<byte>)writer).AsStream();

        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 128, 255, 32 };

        foreach (HighPerformance.Enumerables.ReadOnlySpanEnumerable<byte>.Item item in data.Enumerate())
        {
            // Since we're enumerating, we can also double check the current written count
            // at each iteration, to ensure the writes are done correctly every time.
            Assert.AreEqual(writer.WrittenCount, item.Index);

            // Write a number of bytes one by one to test this API as well
            stream.WriteByte(item.Value);
        }

        // Validate the final written length and actual data
        Assert.AreEqual(writer.WrittenCount, data.Length);
        Assert.IsTrue(data.SequenceEqual(writer.WrittenSpan));

        _ = Assert.ThrowsException<NotSupportedException>(() => stream.ReadByte());
    }

    [TestMethod]
    public void Test_IBufferWriterStream_Write_Span()
    {
        ArrayPoolBufferWriter<byte> writer = new();
        Stream stream = ((IBufferWriter<byte>)writer).AsStream();

        Memory<byte> data = Test_MemoryStream.CreateRandomData(64);

        // This will use the extension when on .NET Standard 2.0, as the
        // Stream class doesn't have Spam<T> or Memory<T> public APIs there.
        stream.Write(data.Span);

        Assert.AreEqual(writer.WrittenCount, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(writer.WrittenSpan));
    }

    [TestMethod]
    public async Task Test_IBufferWriterStream_WriteAsync_Memory()
    {
        ArrayPoolBufferWriter<byte> writer = new();
        Stream stream = ((IBufferWriter<byte>)writer).AsStream();

        Memory<byte> data = Test_MemoryStream.CreateRandomData(64);

        // Same as the other asynchronous test above, but writing from a Memory<T>
        await stream.WriteAsync(data);

        Assert.AreEqual(writer.WrittenCount, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(writer.WrittenSpan));
    }
}

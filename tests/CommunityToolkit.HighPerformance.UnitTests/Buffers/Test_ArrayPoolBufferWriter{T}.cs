// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Buffers;

[TestClass]
public class Test_ArrayPoolBufferWriterOfT
{
    [TestMethod]
    [DataRow(0, 256)] // 256 is the default initial size for ArrayPoolBufferWriter<T>
    [DataRow(4, 256)]
    [DataRow(7, 256)]
    [DataRow(27, 256)]
    [DataRow(188, 256)]
    [DataRow(257, 512)]
    [DataRow(358, 512)]
    [DataRow(799, 1024)]
    [DataRow(1024, 1024)]
    [DataRow(1025, 2048)]
    [DataRow((1024 * 1024) - 1, 1024 * 1024)]
    [DataRow(1024 * 1024, 1024 * 1024)]
    [DataRow((1024 * 1024) + 1, 2 * 1024 * 1024)]
    [DataRow(2 * 1024 * 1024, 2 * 1024 * 1024)]
    [DataRow((2 * 1024 * 1024) + 1, 4 * 1024 * 1024)]
    [DataRow(3 * 1024 * 1024, 4 * 1024 * 1024)]
    public void Test_ArrayPoolBufferWriterOfT_BufferSize(int request, int expected)
    {
        using ArrayPoolBufferWriter<byte>? writer = new();

        // Request a Span<T> of a specified size and discard it. We're just invoking this
        // method to force the ArrayPoolBufferWriter<T> instance to internally resize the
        // buffer to ensure it can contain at least this number of items. After this, we
        // can use reflection to get the internal array and ensure the size equals the
        // expected one, which matches the "round up to power of 2" logic we need. This
        // is documented within the resize method in ArrayPoolBufferWriter<T>, and it's
        // done to prevent repeated allocations of arrays in some scenarios.
        _ = writer.GetSpan(request);

        FieldInfo? arrayFieldInfo = typeof(ArrayPoolBufferWriter<byte>).GetField("array", BindingFlags.Instance | BindingFlags.NonPublic);

        byte[] array = (byte[])arrayFieldInfo!.GetValue(writer)!;

        Assert.AreEqual(array.Length, expected);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_AllocateAndGetMemoryAndSpan()
    {
        ArrayPoolBufferWriter<byte>? writer = new();

        Assert.AreEqual(writer.Capacity, 256);
        Assert.AreEqual(writer.FreeCapacity, 256);
        Assert.AreEqual(writer.WrittenCount, 0);
        Assert.IsTrue(writer.WrittenMemory.IsEmpty);
        Assert.IsTrue(writer.WrittenSpan.IsEmpty);

        Span<byte> span = writer.GetSpan(43);

        Assert.IsTrue(span.Length >= 43);

        writer.Advance(43);

        Assert.AreEqual(writer.Capacity, 256);
        Assert.AreEqual(writer.FreeCapacity, 256 - 43);
        Assert.AreEqual(writer.WrittenCount, 43);
        Assert.AreEqual(writer.WrittenMemory.Length, 43);
        Assert.AreEqual(writer.WrittenSpan.Length, 43);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Advance(-1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.GetMemory(-1));
        _ = Assert.ThrowsException<ArgumentException>(() => writer.Advance(1024));

        writer.Dispose();

        _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.WrittenMemory);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.WrittenSpan.Length);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.Capacity);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.FreeCapacity);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.Clear());
        _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.Advance(1));
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_AllocateFromCustomPoolAndGetMemoryAndSpan()
    {
        TrackingArrayPool<byte>? pool = new();

        using (ArrayPoolBufferWriter<byte>? writer = new(pool))
        {
            Assert.AreEqual(pool.RentedArrays.Count, 1);

            Assert.AreEqual(writer.Capacity, 256);
            Assert.AreEqual(writer.FreeCapacity, 256);
            Assert.AreEqual(writer.WrittenCount, 0);
            Assert.IsTrue(writer.WrittenMemory.IsEmpty);
            Assert.IsTrue(writer.WrittenSpan.IsEmpty);

            Span<byte> span = writer.GetSpan(43);

            Assert.IsTrue(span.Length >= 43);

            writer.Advance(43);

            Assert.AreEqual(writer.Capacity, 256);
            Assert.AreEqual(writer.FreeCapacity, 256 - 43);
            Assert.AreEqual(writer.WrittenCount, 43);
            Assert.AreEqual(writer.WrittenMemory.Length, 43);
            Assert.AreEqual(writer.WrittenSpan.Length, 43);

            _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.Advance(-1));
            _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => writer.GetMemory(-1));
            _ = Assert.ThrowsException<ArgumentException>(() => writer.Advance(1024));

            writer.Dispose();

            _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.WrittenMemory);
            _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.WrittenSpan.Length);
            _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.Capacity);
            _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.FreeCapacity);
            _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.Clear());
            _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.Advance(1));
        }

        Assert.AreEqual(pool.RentedArrays.Count, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_ArrayPoolBufferWriterOfT_InvalidRequestedSize()
    {
        _ = new ArrayPoolBufferWriter<byte>(-1);

        Assert.Fail("You shouldn't be here");
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_Clear()
    {
        using ArrayPoolBufferWriter<byte>? writer = new();

        Span<byte> span = writer.GetSpan(4).Slice(0, 4);

        byte[] data = { 1, 2, 3, 4 };

        data.CopyTo(span);

        writer.Advance(4);

        Assert.AreEqual(writer.WrittenCount, 4);
        Assert.IsTrue(span.SequenceEqual(data));

        writer.Clear();

        Assert.AreEqual(writer.WrittenCount, 0);
        Assert.IsTrue(span.ToArray().All(b => b == 0));
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_MultipleDispose()
    {
        ArrayPoolBufferWriter<byte>? writer = new();

        writer.Dispose();
        writer.Dispose();
        writer.Dispose();
        writer.Dispose();
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_AsStream()
    {
        const int GuidSize = 16;

        ArrayPoolBufferWriter<byte>? writer = new();
        Guid guid = Guid.NewGuid();

        // Here we first get a stream with the extension targeting ArrayPoolBufferWriter<T>.
        // This will wrap it into a custom internal stream type and produce a write-only
        // stream that essentially mirrors the IBufferWriter<T> functionality as a stream.
        using (Stream writeStream = writer.AsStream())
        {
            writeStream.Write(guid);
        }

        Assert.AreEqual(writer.WrittenCount, GuidSize);

        // Here we get a readable stream instead, and read from it to ensure
        // the previous data was written correctly from the writeable stream.
        using (Stream stream = writer.WrittenMemory.AsStream())
        {
            Assert.AreEqual(stream.Length, GuidSize);

            byte[] result = new byte[GuidSize];

            _ = stream.Read(result, 0, result.Length);

            // Read the guid data and ensure it matches our initial guid
            Assert.IsTrue(new Guid(result).Equals(guid));
        }

        // Do a dummy write just to ensure the writer isn't disposed here.
        // This is because we got a stream from a memory, not a memory owner.
        writer.Write((byte)42);
        writer.Advance(1);

        writer.Dispose();

        // Now check that the writer is actually disposed instead
        _ = Assert.ThrowsException<ObjectDisposedException>(() => writer.Capacity);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_AllocateAndGetArray()
    {
        ArrayPoolBufferWriter<int>? bufferWriter = new();

        // Write some random data
        bufferWriter.Write(Enumerable.Range(0, 127).ToArray());

        // Get the array for the written segment
        ArraySegment<int> segment = bufferWriter.DangerousGetArray();

        Assert.IsNotNull(segment.Array);
        Assert.IsTrue(segment.Array.Length >= bufferWriter.WrittenSpan.Length);
        Assert.AreEqual(segment.Offset, 0);
        Assert.AreEqual(segment.Count, bufferWriter.WrittenSpan.Length);

        _ = MemoryMarshal.TryGetArray(bufferWriter.WrittenMemory, out ArraySegment<int> writtenSegment);

        // The array is the same one as the one from the written span
        Assert.AreSame(segment.Array, writtenSegment.Array);

        bufferWriter.Dispose();

        _ = Assert.ThrowsException<ObjectDisposedException>(() => bufferWriter.DangerousGetArray());
    }
}

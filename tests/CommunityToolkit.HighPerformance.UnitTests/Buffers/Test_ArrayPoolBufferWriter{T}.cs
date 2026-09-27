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

        Assert.HasCount(expected, array);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_AllocateAndGetMemoryAndSpan()
    {
        ArrayPoolBufferWriter<byte>? writer = new();

        Assert.AreEqual(256, writer.Capacity);
        Assert.AreEqual(256, writer.FreeCapacity);
        Assert.AreEqual(0, writer.WrittenCount);
        Assert.IsTrue(writer.WrittenMemory.IsEmpty);
        Assert.IsTrue(writer.WrittenSpan.IsEmpty);

        Span<byte> span = writer.GetSpan(43);

        Assert.IsGreaterThanOrEqualTo(43, span.Length);

        writer.Advance(43);

        Assert.AreEqual(256, writer.Capacity);
        Assert.AreEqual(256 - 43, writer.FreeCapacity);
        Assert.AreEqual(43, writer.WrittenCount);
        Assert.AreEqual(43, writer.WrittenMemory.Length);
        Assert.AreEqual(43, writer.WrittenSpan.Length);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.Advance(-1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.GetMemory(-1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => writer.Advance(1024));

        writer.Dispose();

        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WrittenMemory);
        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WrittenSpan.Length);
        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Capacity);
        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.FreeCapacity);
        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Clear());
        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Advance(1));
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_AllocateFromCustomPoolAndGetMemoryAndSpan()
    {
        TrackingArrayPool<byte>? pool = new();

        using (ArrayPoolBufferWriter<byte>? writer = new(pool))
        {
            Assert.HasCount(1, pool.RentedArrays);

            Assert.AreEqual(256, writer.Capacity);
            Assert.AreEqual(256, writer.FreeCapacity);
            Assert.AreEqual(0, writer.WrittenCount);
            Assert.IsTrue(writer.WrittenMemory.IsEmpty);
            Assert.IsTrue(writer.WrittenSpan.IsEmpty);

            Span<byte> span = writer.GetSpan(43);

            Assert.IsGreaterThanOrEqualTo(43, span.Length);

            writer.Advance(43);

            Assert.AreEqual(256, writer.Capacity);
            Assert.AreEqual(256 - 43, writer.FreeCapacity);
            Assert.AreEqual(43, writer.WrittenCount);
            Assert.AreEqual(43, writer.WrittenMemory.Length);
            Assert.AreEqual(43, writer.WrittenSpan.Length);

            _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.Advance(-1));
            _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.GetMemory(-1));
            _ = Assert.ThrowsExactly<ArgumentException>(() => writer.Advance(1024));

            writer.Dispose();

            _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WrittenMemory);
            _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WrittenSpan.Length);
            _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Capacity);
            _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.FreeCapacity);
            _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Clear());
            _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Advance(1));
        }

        Assert.IsEmpty(pool.RentedArrays);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_InvalidRequestedSize()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ArrayPoolBufferWriter<byte>(-1));
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_Clear()
    {
        using ArrayPoolBufferWriter<byte>? writer = new();

        Span<byte> span = writer.GetSpan(4).Slice(0, 4);

        byte[] data = { 1, 2, 3, 4 };

        data.CopyTo(span);

        writer.Advance(4);

        Assert.AreEqual(4, writer.WrittenCount);
        Assert.IsTrue(span.SequenceEqual(data));

        writer.Clear();

        Assert.AreEqual(0, writer.WrittenCount);
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

        Assert.AreEqual(GuidSize, writer.WrittenCount);

        // Here we get a readable stream instead, and read from it to ensure
        // the previous data was written correctly from the writeable stream.
        using (Stream stream = writer.WrittenMemory.AsStream())
        {
            Assert.AreEqual(GuidSize, stream.Length);

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
        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Capacity);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_GetReadOnlySequence_DoesNotConsolidate()
    {
        Random random = new(Guid.NewGuid().GetHashCode());
        int firstLength = random.Next(16, 33);
        int secondLength = random.Next(34, 65);

        byte[] first = GetRandomBytes(firstLength);
        byte[] second = GetRandomBytes(secondLength);

        TrackingArrayPool<byte> pool = new();

        using ArrayPoolBufferWriter<byte> writer = new(pool, firstLength);

        first.CopyTo(writer.GetSpan(firstLength));
        writer.Advance(firstLength);

        second.CopyTo(writer.GetSpan(secondLength));
        writer.Advance(secondLength);

        byte[] expected = first.Concat(second).ToArray();

        Assert.HasCount(2, pool.RentedArrays);

        ReadOnlySequence<byte> sequence = writer.GetReadOnlySequence();

        Assert.IsTrue(sequence.ToArray().SequenceEqual(expected));
        Assert.HasCount(2, pool.RentedArrays);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_CharToString_EmptyAndSingleBuffer()
    {
        TrackingArrayPool<char> pool = new();

        using (ArrayPoolBufferWriter<char> writer = new(pool))
        {
            char[] array = pool.RentedArrays.Single();
            int capacity = writer.Capacity;

            Assert.AreEqual(string.Empty, writer.ToString());
            Assert.HasCount(1, pool.RentedArrays);
            Assert.IsTrue(pool.RentedArrays.Contains(array));
            Assert.AreEqual(capacity, writer.Capacity);
            Assert.AreEqual(capacity, writer.FreeCapacity);
            Assert.AreEqual(0, writer.WrittenCount);

            string value = Guid.NewGuid().ToString("N");

            value.AsSpan().CopyTo(writer.GetSpan(value.Length));
            writer.Advance(value.Length);

            ReadOnlySequence<char> sequence = writer.GetReadOnlySequence();
            int freeCapacity = writer.FreeCapacity;

            Assert.AreEqual(value, writer.ToString());
            Assert.AreEqual(value, new string(sequence.ToArray()));
            Assert.AreEqual(value.Length, writer.WrittenCount);
            Assert.AreEqual(capacity, writer.Capacity);
            Assert.AreEqual(freeCapacity, writer.FreeCapacity);
            Assert.HasCount(1, pool.RentedArrays);
            Assert.IsTrue(pool.RentedArrays.Contains(array));
        }

        Assert.IsEmpty(pool.RentedArrays);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Test_ArrayPoolBufferWriterOfT_CharToString_PreservesSegments(bool emptyActiveBuffer)
    {
        TrackingArrayPool<char> pool = new();

        using (ArrayPoolBufferWriter<char> writer = new(pool, Guid.NewGuid().ToString("N").Length))
        {
            string first = Guid.NewGuid().ToString("N");
            string second = Guid.NewGuid().ToString("N");
            string third = Guid.NewGuid().ToString("N");
            string expected = first + second + third;

            foreach (string value in new[] { first, second })
            {
                value.AsSpan().CopyTo(writer.GetSpan(value.Length));
                writer.Advance(value.Length);
            }

            third.AsSpan().CopyTo(writer.GetSpan(writer.FreeCapacity + 1));
            writer.Advance(third.Length);

            if (emptyActiveBuffer)
            {
                _ = writer.GetMemory(writer.FreeCapacity + 1);
            }

            ReadOnlySequence<char> sequence = writer.GetReadOnlySequence();
            char[][] rentedArrays = pool.RentedArrays.ToArray();
            int writtenCount = writer.WrittenCount;
            int capacity = writer.Capacity;
            int freeCapacity = writer.FreeCapacity;

            Assert.IsGreaterThanOrEqualTo(3, rentedArrays.Length);

            Assert.AreEqual(expected, writer.ToString());
            Assert.AreEqual(expected, writer.ToString());
            Assert.AreEqual(expected, new string(sequence.ToArray()));
            Assert.AreEqual(writtenCount, writer.WrittenCount);
            Assert.AreEqual(capacity, writer.Capacity);
            Assert.AreEqual(freeCapacity, writer.FreeCapacity);
            Assert.HasCount(rentedArrays.Length, pool.RentedArrays);

            foreach (char[] array in rentedArrays)
            {
                Assert.IsTrue(pool.RentedArrays.Contains(array));
            }
        }

        Assert.IsEmpty(pool.RentedArrays);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_CharToString_Disposed()
    {
        ArrayPoolBufferWriter<char> writer = new();

        writer.Dispose();

        Assert.AreEqual($"CommunityToolkit.HighPerformance.Buffers.ArrayPoolBufferWriter<{typeof(char)}>[0]", writer.ToString());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Test_ArrayPoolBufferWriterOfT_ReadOnlyMembers_PreserveSegments(bool emptyActiveBuffer)
    {
        TrackingArrayPool<byte> pool = new();

        using (ArrayPoolBufferWriter<byte> writer = new(pool, Guid.NewGuid().ToByteArray().Length))
        {
            byte[] first = Guid.NewGuid().ToByteArray();
            byte[] second = Guid.NewGuid().ToByteArray();
            byte[] expected = first.Concat(second).ToArray();

            first.CopyTo(writer.GetSpan(first.Length));
            writer.Advance(first.Length);

            second.CopyTo(writer.GetSpan(first.Length + 1));
            writer.Advance(second.Length);

            if (emptyActiveBuffer)
            {
                _ = writer.GetMemory(writer.FreeCapacity + 1);
            }

            ReadOnlySequence<byte> sequence = writer.GetReadOnlySequence();
            byte[][] rentedArrays = pool.RentedArrays.ToArray();
            int writtenCount = writer.WrittenCount;
            int capacity = writer.Capacity;
            int freeCapacity = writer.FreeCapacity;
            string display = $"CommunityToolkit.HighPerformance.Buffers.ArrayPoolBufferWriter<{typeof(byte)}>[{writtenCount}]";

            Assert.IsGreaterThanOrEqualTo(2, rentedArrays.Length);

            Assert.AreEqual(display, writer.ToString());
            Assert.AreEqual(writtenCount, writer.WrittenCount);
            Assert.AreEqual(capacity, writer.Capacity);
            Assert.AreEqual(freeCapacity, writer.FreeCapacity);
            Assert.IsTrue(writer.GetReadOnlySequence().ToArray().SequenceEqual(expected));
            Assert.AreEqual(display, writer.ToString());
            Assert.IsTrue(sequence.ToArray().SequenceEqual(expected));
            Assert.AreEqual(writtenCount, writer.WrittenCount);
            Assert.AreEqual(capacity, writer.Capacity);
            Assert.AreEqual(freeCapacity, writer.FreeCapacity);
            Assert.HasCount(rentedArrays.Length, pool.RentedArrays);

            foreach (byte[] array in rentedArrays)
            {
                Assert.IsTrue(pool.RentedArrays.Contains(array));
            }
        }

        Assert.IsEmpty(pool.RentedArrays);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_WrittenMemory_ConsolidatesBuffers_AndDangerousGetArrayMatches()
    {
        Random random = new(Guid.NewGuid().GetHashCode());
        int firstLength = random.Next(16, 33);
        int secondLength = random.Next(34, 65);

        byte[] first = GetRandomBytes(firstLength);
        byte[] second = GetRandomBytes(secondLength);

        TrackingArrayPool<byte> pool = new();

        using ArrayPoolBufferWriter<byte> writer = new(pool, firstLength);

        first.CopyTo(writer.GetSpan(firstLength));
        writer.Advance(firstLength);

        second.CopyTo(writer.GetSpan(secondLength));
        writer.Advance(secondLength);

        byte[] expected = first.Concat(second).ToArray();

        Assert.HasCount(2, pool.RentedArrays);

        ReadOnlyMemory<byte> memory = writer.WrittenMemory;

        Assert.IsTrue(memory.Span.SequenceEqual(expected));
        Assert.HasCount(1, pool.RentedArrays);

        ArraySegment<byte> segment = writer.DangerousGetArray();

        Assert.HasCount(memory.Length, segment);
        Assert.IsTrue(memory.Span.SequenceEqual(segment.AsSpan()));

        _ = MemoryMarshal.TryGetArray(memory, out ArraySegment<byte> writtenSegment);

        Assert.AreSame(writtenSegment.Array, segment.Array);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_Clear_ReturnsRetainedBuffersOnly()
    {
        Random random = new(Guid.NewGuid().GetHashCode());
        int firstLength = random.Next(16, 33);
        int secondLength = random.Next(34, 65);

        TrackingArrayPool<byte> pool = new();

        using ArrayPoolBufferWriter<byte> writer = new(pool, firstLength);

        writer.GetSpan(firstLength).Slice(0, firstLength).Fill(1);
        writer.Advance(firstLength);

        writer.GetSpan(secondLength).Slice(0, secondLength).Fill(2);
        writer.Advance(secondLength);

        Assert.HasCount(2, pool.RentedArrays);

        writer.Clear();

        Assert.AreEqual(0, writer.WrittenCount);
        Assert.IsTrue(writer.WrittenMemory.IsEmpty);
        Assert.HasCount(1, pool.RentedArrays);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_Capacity_TracksOnlyWritableSpace()
    {
        TrackingArrayPool<byte> pool = new();

        using (ArrayPoolBufferWriter<byte> writer = new(pool, Guid.NewGuid().ToByteArray().Length))
        {
            Assert.AreEqual(writer.Capacity - writer.WrittenCount, writer.FreeCapacity);

            byte[] first = GetRandomBytes(new Random(Guid.NewGuid().GetHashCode()).Next(1, writer.Capacity));

            first.CopyTo(writer.GetSpan(first.Length));
            writer.Advance(first.Length);

            Assert.AreEqual(writer.Capacity - writer.WrittenCount, writer.FreeCapacity);

            byte[] second = GetRandomBytes(writer.FreeCapacity + 1);

            second.CopyTo(writer.GetSpan(second.Length));

            Assert.HasCount(2, pool.RentedArrays);
            Assert.AreEqual(first.Length, writer.WrittenCount);
            Assert.AreEqual(writer.Capacity - writer.WrittenCount, writer.FreeCapacity);

            writer.Advance(second.Length);

            byte[] expected = first.Concat(second).ToArray();

            Assert.IsTrue(writer.GetReadOnlySequence().ToArray().SequenceEqual(expected));
            Assert.AreEqual(writer.Capacity - writer.WrittenCount, writer.FreeCapacity);

            ArraySegment<byte> contiguous = writer.DangerousGetArray();

            Assert.IsTrue(contiguous.AsSpan().SequenceEqual(expected));
            Assert.HasCount(1, pool.RentedArrays);
            Assert.AreEqual(writer.Capacity - writer.WrittenCount, writer.FreeCapacity);

            writer.Clear();

            Assert.AreEqual(0, writer.WrittenCount);
            Assert.AreEqual(writer.Capacity, writer.FreeCapacity);
        }

        Assert.IsEmpty(pool.RentedArrays);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_SmallWrites_GrowSegmentsGeometrically()
    {
        TrackingArrayPool<byte> pool = new();

        using (ArrayPoolBufferWriter<byte> writer = new(pool))
        {
            byte[] chunk = GetRandomBytes(writer.Capacity);
            byte[] expected = new byte[1024 * 1024];

            for (int offset = 0; offset < expected.Length; offset += chunk.Length)
            {
                chunk.CopyTo(writer.GetSpan(chunk.Length));
                writer.Advance(chunk.Length);
                chunk.CopyTo(expected.AsSpan(offset));
            }

            Assert.AreEqual(expected.Length, writer.WrittenCount);
            Assert.AreEqual(writer.Capacity - writer.WrittenCount, writer.FreeCapacity);
            Assert.IsLessThanOrEqualTo(16, pool.RentedArrays.Count);
            Assert.IsTrue(pool.RentedArrays.Any(array => array.Length > chunk.Length));
            Assert.IsTrue(writer.GetReadOnlySequence().ToArray().SequenceEqual(expected));
        }

        Assert.IsEmpty(pool.RentedArrays);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Test_ArrayPoolBufferWriterOfT_FailedRent_PreservesCurrentBuffer(bool hasWrittenData)
    {
        TrackingArrayPool<byte> pool = new();
        byte[] data;

        using (ArrayPoolBufferWriter<byte> writer = new(pool))
        {
            byte[] originalArray = writer.DangerousGetArray().Array!;
            data = hasWrittenData ? GetRandomBytes(new Random(Guid.NewGuid().GetHashCode()).Next(1, writer.Capacity)) : Array.Empty<byte>();

            data.CopyTo(writer.GetSpan(data.Length));
            writer.Advance(data.Length);

            int originalCapacity = writer.Capacity;
            int sizeHint = writer.FreeCapacity + 1;

            pool.FailNextRent = true;

            _ = Assert.ThrowsExactly<InvalidOperationException>(() => writer.GetMemory(sizeHint));

            Assert.HasCount(1, pool.RentedArrays);
            Assert.IsTrue(pool.RentedArrays.Contains(originalArray));
            Assert.AreEqual(originalCapacity, writer.Capacity);
            Assert.AreEqual(data.Length, writer.WrittenCount);
            Assert.IsTrue(writer.WrittenSpan.SequenceEqual(data));

            _ = writer.GetMemory(sizeHint);

            Assert.IsTrue(writer.GetReadOnlySequence().ToArray().SequenceEqual(data));
        }

        Assert.IsEmpty(pool.RentedArrays);
    }

    [TestMethod]
    public void Test_ArrayPoolBufferWriterOfT_IntLengthBoundary_RejectsWritesWithoutChangingOwnership()
    {
        TrackingArrayPool<byte> pool = new();
        byte[] first = GetRandomBytes(new Random(Guid.NewGuid().GetHashCode()).Next(8, 33));
        byte[] last = GetRandomBytes(new Random(Guid.NewGuid().GetHashCode()).Next(1, 4));

        using (ArrayPoolBufferWriter<byte> writer = new(pool, first.Length))
        {
            first.CopyTo(writer.GetSpan(first.Length));
            writer.Advance(first.Length);
            _ = writer.GetMemory(writer.FreeCapacity + 1);

            FieldInfo bufferedCountField = typeof(ArrayPoolBufferWriter<byte>).GetField("bufferedCount", BindingFlags.Instance | BindingFlags.NonPublic)!;
            byte[][] rentedArrays = pool.RentedArrays.ToArray();
            int physicalFreeCapacity = writer.FreeCapacity;

            // Simulate the count of already completed segments without allocating gigabytes.
            // Restore the actual count before reading or disposing the writer.
            try
            {
                bufferedCountField.SetValue(writer, int.MaxValue - physicalFreeCapacity);

                // Without the length guard, this request would rent another array.
                _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.GetMemory(physicalFreeCapacity + 1));
                Assert.HasCount(rentedArrays.Length, pool.RentedArrays);

                bufferedCountField.SetValue(writer, int.MaxValue - last.Length);

                Assert.AreEqual(int.MaxValue - last.Length, writer.WrittenCount);
                Assert.AreEqual(last.Length, writer.FreeCapacity);
                Assert.AreEqual(int.MaxValue, writer.Capacity);
                Assert.AreEqual(last.Length, writer.GetMemory().Length);
                Assert.AreEqual(last.Length, writer.GetSpan().Length);

                _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.GetMemory(last.Length + 1));
                _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.GetSpan(last.Length + 1));
                Assert.HasCount(rentedArrays.Length, pool.RentedArrays);

                last.CopyTo(writer.GetSpan(last.Length));
                writer.Advance(last.Length);

                Assert.AreEqual(int.MaxValue, writer.WrittenCount);
                Assert.AreEqual(0, writer.FreeCapacity);
                Assert.AreEqual(int.MaxValue, writer.Capacity);
                _ = Assert.ThrowsExactly<ArgumentException>(() => writer.Advance(1));
                _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.GetMemory());
                _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.GetSpan());
                Assert.AreEqual(int.MaxValue, writer.WrittenCount);
                Assert.HasCount(rentedArrays.Length, pool.RentedArrays);

                foreach (byte[] array in rentedArrays)
                {
                    Assert.IsTrue(pool.RentedArrays.Contains(array));
                }
            }
            finally
            {
                bufferedCountField.SetValue(writer, first.Length);
            }

            CollectionAssert.AreEqual(first.Concat(last).ToArray(), writer.GetReadOnlySequence().ToArray());
        }

        Assert.IsEmpty(pool.RentedArrays);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Test_ArrayPoolBufferWriterOfT_DebugView_PreservesSequenceAndPool(bool emptyActiveBuffer)
    {
        TrackingArrayPool<byte> pool = new();
        byte[] first = GetRandomBytes(new Random(Guid.NewGuid().GetHashCode()).Next(8, 33));
        byte[] second = GetRandomBytes(new Random(Guid.NewGuid().GetHashCode()).Next(1, 17));
        byte[] expected = first.Concat(second).ToArray();

        using (ArrayPoolBufferWriter<byte> writer = new(pool, first.Length))
        {
            first.CopyTo(writer.GetSpan(first.Length));
            writer.Advance(first.Length);
            second.CopyTo(writer.GetSpan(writer.FreeCapacity + 1));
            writer.Advance(second.Length);

            if (emptyActiveBuffer)
            {
                _ = writer.GetMemory(writer.FreeCapacity + 1);
            }

            ReadOnlySequence<byte> sequence = writer.GetReadOnlySequence();
            byte[][] rentedArrays = pool.RentedArrays.ToArray();
            int capacity = writer.Capacity;
            int freeCapacity = writer.FreeCapacity;
            Type viewType = typeof(ArrayPoolBufferWriter<byte>).Assembly.GetType("CommunityToolkit.HighPerformance.Buffers.Views.MemoryDebugView`1")!.MakeGenericType(typeof(byte));
            object view = Activator.CreateInstance(viewType, writer)!;
            byte[] items = (byte[])viewType.GetProperty("Items")!.GetValue(view)!;

            CollectionAssert.AreEqual(expected, items);
            CollectionAssert.AreEqual(expected, sequence.ToArray());
            Assert.AreEqual(expected.Length, writer.WrittenCount);
            Assert.AreEqual(capacity, writer.Capacity);
            Assert.AreEqual(freeCapacity, writer.FreeCapacity);
            Assert.HasCount(rentedArrays.Length, pool.RentedArrays);

            foreach (byte[] array in rentedArrays)
            {
                Assert.IsTrue(pool.RentedArrays.Contains(array));
            }
        }

        Assert.IsEmpty(pool.RentedArrays);
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
        Assert.IsGreaterThanOrEqualTo(bufferWriter.WrittenSpan.Length, segment.Array.Length);
        Assert.AreEqual(0, segment.Offset);
        Assert.AreEqual(segment.Count, bufferWriter.WrittenSpan.Length);

        _ = MemoryMarshal.TryGetArray(bufferWriter.WrittenMemory, out ArraySegment<int> writtenSegment);

        // The array is the same one as the one from the written span
        Assert.AreSame(segment.Array, writtenSegment.Array);

        bufferWriter.Dispose();

        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => bufferWriter.DangerousGetArray());
    }

    private static byte[] GetRandomBytes(int length)
    {
        byte[] data = new byte[length];

        new Random(Guid.NewGuid().GetHashCode()).NextBytes(data);

        return data;
    }
}

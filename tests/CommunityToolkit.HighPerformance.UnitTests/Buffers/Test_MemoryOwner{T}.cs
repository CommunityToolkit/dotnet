// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Linq;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;

namespace CommunityToolkit.HighPerformance.UnitTests.Buffers;

[TestClass]
public class Test_MemoryOwnerOfT
{
    [TestMethod]
    public void Test_MemoryOwnerOfT_AllocateAndGetMemoryAndSpan()
    {
        using MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127);

        Assert.IsTrue(buffer.Length == 127);
        Assert.IsTrue(buffer.Memory.Length == 127);
        Assert.IsTrue(buffer.Span.Length == 127);

        buffer.Span.Fill(42);

        Assert.IsTrue(buffer.Memory.Span.ToArray().All(i => i == 42));
        Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
    }

    [TestMethod]
    public void Test_MemoryOwnerOfT_AllocateFromCustomPoolAndGetMemoryAndSpan()
    {
        TrackingArrayPool<int>? pool = new();

        using (MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127, pool))
        {
            Assert.AreEqual(pool.RentedArrays.Count, 1);

            Assert.IsTrue(buffer.Length == 127);
            Assert.IsTrue(buffer.Memory.Length == 127);
            Assert.IsTrue(buffer.Span.Length == 127);

            buffer.Span.Fill(42);

            Assert.IsTrue(buffer.Memory.Span.ToArray().All(i => i == 42));
            Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
        }

        Assert.AreEqual(pool.RentedArrays.Count, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_MemoryOwnerOfT_InvalidRequestedSize()
    {
        using MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(-1);

        Assert.Fail("You shouldn't be here");
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void Test_MemoryOwnerOfT_DisposedMemory()
    {
        MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127);

        buffer.Dispose();

        _ = buffer.Memory;
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void Test_MemoryOwnerOfT_DisposedSpan()
    {
        MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127);

        buffer.Dispose();

        _ = buffer.Span;
    }

    [TestMethod]
    public void Test_MemoryOwnerOfT_MultipleDispose()
    {
        MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127);

        buffer.Dispose();
        buffer.Dispose();
        buffer.Dispose();
        buffer.Dispose();

        // This test consists in just getting here without crashes.
        // We're validating that calling Dispose multiple times
        // by accident doesn't cause issues, and just does nothing.
    }

    [TestMethod]
    public void Test_MemoryOwnerOfT_PooledBuffersAndClear()
    {
        using (MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127))
        {
            buffer.Span.Fill(42);
        }

        using (MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127))
        {
            Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
        }

        using (MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127, AllocationMode.Clear))
        {
            Assert.IsTrue(buffer.Span.ToArray().All(i => i == 0));
        }
    }

    [TestMethod]
    public void Test_MemoryOwnerOfT_AllocateAndGetArray()
    {
        MemoryOwner<int>? buffer = MemoryOwner<int>.Allocate(127);

        // Here we allocate a MemoryOwner<T> instance with a requested size of 127, which means it
        // internally requests an array of size 127 from ArrayPool<T>.Shared. We then get the array
        // segment, so we need to verify that (since buffer is not disposed) the returned array is
        // not null, is of size >= the requested one (since ArrayPool<T> by definition returns an
        // array that is at least of the requested size), and that the offset and count properties
        // match our input values (same length, and offset at 0 since the buffer was not sliced).
        ArraySegment<int> segment = buffer.DangerousGetArray();

        Assert.IsNotNull(segment.Array);
        Assert.IsTrue(segment.Array.Length >= buffer.Length);
        Assert.AreEqual(segment.Offset, 0);
        Assert.AreEqual(segment.Count, buffer.Length);

        MemoryOwner<int>? second = buffer.Slice(10, 80);

        // The original buffer instance is disposed here, because calling Slice transfers
        // the ownership of the internal buffer to the new instance (this is documented in
        // XML docs for the MemoryOwner<T>.Slice method).
        _ = Assert.ThrowsException<ObjectDisposedException>(() => buffer.DangerousGetArray());

        segment = second.DangerousGetArray();

        // Same as before, but we now also verify the initial offset != 0, as we used Slice
        Assert.IsNotNull(segment.Array);
        Assert.IsTrue(segment.Array.Length >= second.Length);
        Assert.AreEqual(segment.Offset, 10);
        Assert.AreEqual(segment.Count, second.Length);

        second.Dispose();

        _ = Assert.ThrowsException<ObjectDisposedException>(() => second.DangerousGetArray());
    }
}

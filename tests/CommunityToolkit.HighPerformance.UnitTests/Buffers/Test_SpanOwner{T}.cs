// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Linq;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;

namespace CommunityToolkit.HighPerformance.UnitTests.Buffers;

[TestClass]
public class Test_SpanOwnerOfT
{
    [TestMethod]
    public void Test_SpanOwnerOfT_AllocateAndGetMemoryAndSpan()
    {
        using SpanOwner<int> buffer = SpanOwner<int>.Allocate(127);

        Assert.IsTrue(buffer.Length == 127);
        Assert.IsTrue(buffer.Span.Length == 127);

        buffer.Span.Fill(42);

        Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
    }

    [TestMethod]
    public void Test_SpanOwnerOfT_AllocateFromCustomPoolAndGetMemoryAndSpan()
    {
        TrackingArrayPool<int>? pool = new();

        using (SpanOwner<int> buffer = SpanOwner<int>.Allocate(127, pool))
        {
            Assert.AreEqual(pool.RentedArrays.Count, 1);

            Assert.IsTrue(buffer.Length == 127);
            Assert.IsTrue(buffer.Span.Length == 127);

            buffer.Span.Fill(42);

            Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
        }

        Assert.AreEqual(pool.RentedArrays.Count, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_SpanOwnerOfT_InvalidRequestedSize()
    {
        using SpanOwner<int> buffer = SpanOwner<int>.Allocate(-1);

        Assert.Fail("You shouldn't be here");
    }

    [TestMethod]
    public void Test_SpanOwnerOfT_PooledBuffersAndClear()
    {
        using (SpanOwner<int> buffer = SpanOwner<int>.Allocate(127))
        {
            buffer.Span.Fill(42);
        }

        using (SpanOwner<int> buffer = SpanOwner<int>.Allocate(127))
        {
            Assert.IsTrue(buffer.Span.ToArray().All(i => i == 42));
        }

        using (SpanOwner<int> buffer = SpanOwner<int>.Allocate(127, AllocationMode.Clear))
        {
            Assert.IsTrue(buffer.Span.ToArray().All(i => i == 0));
        }
    }

    [TestMethod]
    public void Test_SpanOwnerOfT_AllocateAndGetArray()
    {
        using SpanOwner<int> buffer = SpanOwner<int>.Allocate(127);

        ArraySegment<int> segment = buffer.DangerousGetArray();

        // See comments in the MemoryOwner<T> tests about this. The main difference
        // here is that we don't do the disposed checks, as SpanOwner<T> is optimized
        // with the assumption that usages after dispose are undefined behavior. This
        // is all documented in the XML docs for the SpanOwner<T> type.
        Assert.IsNotNull(segment.Array);
        Assert.IsTrue(segment.Array.Length >= buffer.Length);
        Assert.AreEqual(segment.Offset, 0);
        Assert.AreEqual(segment.Count, buffer.Length);
    }
}

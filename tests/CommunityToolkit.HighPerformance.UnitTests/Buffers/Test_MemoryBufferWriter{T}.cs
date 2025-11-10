// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Buffers;

[TestClass]
public class Test_MemoryBufferWriterOfT
{
    [TestMethod]
    public void Test_MemoryBufferWriterOfT_AllocateAndGetMemoryAndSpan()
    {
        Memory<byte> memory = new byte[256];

        MemoryBufferWriter<byte>? writer = new(memory);

        Assert.AreEqual(256, writer.Capacity);
        Assert.AreEqual(256, writer.FreeCapacity);
        Assert.AreEqual(0, writer.WrittenCount);
        Assert.IsTrue(writer.WrittenMemory.IsEmpty);
        Assert.IsTrue(writer.WrittenSpan.IsEmpty);

        Span<byte> span = writer.GetSpan(43);

        Assert.AreEqual(span.Length, memory.Length);

        writer.Advance(43);

        Assert.AreEqual(256, writer.Capacity);
        Assert.AreEqual(256 - 43, writer.FreeCapacity);
        Assert.AreEqual(43, writer.WrittenCount);
        Assert.AreEqual(43, writer.WrittenMemory.Length);
        Assert.AreEqual(43, writer.WrittenSpan.Length);

        Assert.AreEqual(memory.Length - 43, writer.GetSpan().Length);
        Assert.AreEqual(memory.Length - 43, writer.GetMemory().Length);
        Assert.AreEqual(memory.Length - 43, writer.GetSpan(22).Length);
        Assert.AreEqual(memory.Length - 43, writer.GetMemory(22).Length);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.Advance(-1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => writer.GetMemory(-1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => writer.GetSpan(1024));
        _ = Assert.ThrowsExactly<ArgumentException>(() => writer.GetMemory(1024));
        _ = Assert.ThrowsExactly<ArgumentException>(() => writer.Advance(1024));
    }

    [TestMethod]
    public void Test_MemoryBufferWriterOfT_Clear()
    {
        Memory<byte> memory = new byte[256];

        MemoryBufferWriter<byte>? writer = new(memory);

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
}

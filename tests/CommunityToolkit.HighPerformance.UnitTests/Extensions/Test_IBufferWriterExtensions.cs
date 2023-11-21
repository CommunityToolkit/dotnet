// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
#if NET6_0_OR_GREATER
using System.Buffers;
#endif
using System.IO;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

[TestClass]
public class Test_IBufferWriterExtensions
{
    [TestMethod]
    public unsafe void Test_IBufferWriterExtensions_WriteReadOverBytes()
    {
        ArrayPoolBufferWriter<byte> writer = new();

        byte b = 255;
        char c = '$';
        float f = 3.14f;
        double d = 6.28;
        Guid guid = Guid.NewGuid();

        writer.Write(b);
        writer.Write(c);
        writer.Write(f);
        writer.Write(d);
        writer.Write(guid);

        int count = sizeof(byte) + sizeof(char) + sizeof(float) + sizeof(double) + sizeof(Guid);

        Assert.AreEqual(count, writer.WrittenCount);

        using Stream reader = writer.WrittenMemory.AsStream();

        Assert.AreEqual(b, reader.Read<byte>());
        Assert.AreEqual(c, reader.Read<char>());
        Assert.AreEqual(f, reader.Read<float>());
        Assert.AreEqual(d, reader.Read<double>());
        Assert.AreEqual(guid, reader.Read<Guid>());
    }

    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteReadItem_Guid()
    {
        Test_IBufferWriterExtensions_WriteReadItem(Guid.NewGuid(), Guid.NewGuid());
    }

    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteReadItem_String()
    {
        Test_IBufferWriterExtensions_WriteReadItem("Hello", "World");
    }

    private static void Test_IBufferWriterExtensions_WriteReadItem<T>(T a, T b)
        where T : IEquatable<T>
    {
        ArrayPoolBufferWriter<T> writer = new();

        writer.Write(a);
        writer.Write(b);

        Assert.AreEqual(2, writer.WrittenCount);

        ReadOnlySpan<T> span = writer.WrittenSpan;

        Assert.AreEqual(a, span[0]);
        Assert.AreEqual(b, span[1]);
    }

    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteReadOverBytes_ReadOnlySpan()
    {
        int[] buffer = new int[128];

        Random? random = new(42);

        foreach (ref int n in buffer.AsSpan())
        {
            n = random.Next(int.MinValue, int.MaxValue);
        }

        ArrayPoolBufferWriter<byte> writer = new();

        writer.Write<int>(buffer);

        Assert.AreEqual(sizeof(int) * buffer.Length, writer.WrittenCount);

        ReadOnlySpan<byte> span = writer.WrittenSpan;

        Assert.IsTrue(span.SequenceEqual(buffer.AsSpan().AsBytes()));
    }

    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteReadOverItems_ReadOnlySpan()
    {
        int[] buffer = new int[128];

        Random? random = new(42);

        foreach (ref int n in buffer.AsSpan())
        {
            n = random.Next(int.MinValue, int.MaxValue);
        }

        ArrayPoolBufferWriter<int> writer = new();

        writer.Write(buffer.AsSpan());

        Assert.AreEqual(buffer.Length, writer.WrittenCount);

        ReadOnlySpan<int> span = writer.WrittenSpan;

        Assert.IsTrue(span.SequenceEqual(buffer.AsSpan()));
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/798
    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteExceedingFreeCapacity()
    {
        ArrayPoolBufferWriter<byte> writer = new();

        // Leave only one byte of free capacity
        int count = writer.Capacity - 1;
        
        for (int i = 0; i < count; i++)
        {
            writer.Write<byte>(0);
        }

        // Write 4 bytes
        writer.Write(1);
    }
}

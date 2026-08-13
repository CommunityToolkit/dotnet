// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

// this was previously #if NET6_0_OR_GREATER, to dodge the CS0121 ambiguity between
// BuffersExtensions.Write and IBufferWriterExtensions.Write on the netstandard2.0 build
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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

    // See https://github.com/CommunityToolkit/dotnet/issues/1208
    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteBytes_SegmentedWriter()
    {
        byte[] payload = CreateBytes(64);

        SegmentedWriter<byte> writer = new();

        writer.Write<byte>(payload);

        CollectionAssert.AreEqual(payload, writer.ToArray());

        // the payload is 8x the segment size; a single request could not have satisfied it
        Assert.IsGreaterThan(1, writer.Requests);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/1208
    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteBytes_SegmentedWriter_ExtensionSyntax()
    {
        byte[] payload = CreateBytes(64);

        SegmentedWriter<byte> writer = new();

        // this binds to IBufferWriterExtensions.Write<byte>(IBufferWriter<byte>, ReadOnlySpan<byte>), which
        // out-competes BuffersExtensions.Write<T>(IBufferWriter<T>, ReadOnlySpan<T>) on all TFMs; it must
        // therefore behave the same as the method it hides
        writer.Write(payload.AsSpan());

        CollectionAssert.AreEqual(payload, writer.ToArray());
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/1208
    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteItems_SegmentedWriter()
    {
        int[] payload = CreateInts(64);

        SegmentedWriter<int> writer = new();

        writer.Write(payload.AsSpan());

        CollectionAssert.AreEqual(payload, writer.ToArray());
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/1208
    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteBlittedItems_SegmentedWriter()
    {
        int[] payload = CreateInts(16);

        SegmentedWriter<byte> writer = new();

        writer.Write<int>(payload);

        byte[] written = writer.ToArray();

        Assert.HasCount(sizeof(int) * payload.Length, written);
        Assert.IsTrue(written.AsSpan().SequenceEqual(MemoryMarshal.AsBytes(payload.AsSpan())));
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/1208
    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteValue_StraddlingSegmentBoundary()
    {
        const long Value = 0x0102030405060708;

        SegmentedWriter<byte> writer = new();

        // five bytes of padding leaves three bytes in the current segment, so the value cannot fit
        writer.Write<byte>(new byte[5]);
        writer.Write(Value);

        byte[] written = writer.ToArray();

        Assert.HasCount(5 + sizeof(long), written);
        Assert.AreEqual(Value, MemoryMarshal.Read<long>(written.AsSpan(5)));
    }

#if NETFRAMEWORK
    // See https://github.com/CommunityToolkit/dotnet/issues/1208; the T-to-T overload is retained on
    // netstandard2.0 (which is what net472 resolves) for binary compatibility, but is no longer an
    // extension method
    [TestMethod]
    public void Test_IBufferWriterExtensions_WriteItems_ObsoleteCompatShim()
    {
        int[] payload = CreateInts(64);

        SegmentedWriter<int> writer = new();

#pragma warning disable CS0618 // obsolete
        IBufferWriterExtensions.Write<int>(writer, payload);
#pragma warning restore CS0618

        CollectionAssert.AreEqual(payload, writer.ToArray());
    }
#endif

    private static byte[] CreateBytes(int count)
    {
        byte[] payload = new byte[count];

        new Random(42).NextBytes(payload);

        return payload;
    }

    private static int[] CreateInts(int count)
    {
        int[] payload = new int[count];
        Random random = new(42);

        for (int i = 0; i < payload.Length; i++)
        {
            payload[i] = random.Next(int.MinValue, int.MaxValue);
        }

        return payload;
    }

    /// <summary>
    /// An <see cref="IBufferWriter{T}"/> that never hands out more than <see cref="SegmentSize"/> elements at a
    /// time, whatever <c>sizeHint</c> asks for; the hint is a hint, not a demand.
    /// </summary>
    private sealed class SegmentedWriter<T> : IBufferWriter<T>
    {
        private const int SegmentSize = 8;

        private readonly List<T[]> segments = new();
        private readonly List<int> counts = new();
        private int used;

        /// <summary>
        /// Gets the number of <see cref="GetSpan"/> calls received.
        /// </summary>
        public int Requests { get; private set; }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            Requests++;

            if (this.segments.Count == 0 || this.used == SegmentSize)
            {
                this.segments.Add(new T[SegmentSize]);
                this.counts.Add(0);
                this.used = 0;
            }

            return this.segments[this.segments.Count - 1].AsSpan(this.used);
        }

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            throw new NotSupportedException();
        }

        public void Advance(int count)
        {
            this.used += count;
            this.counts[this.counts.Count - 1] = this.used;
        }

        public T[] ToArray()
        {
            List<T> result = new();

            for (int i = 0; i < this.segments.Count; i++)
            {
                for (int j = 0; j < this.counts[i]; j++)
                {
                    result.Add(this.segments[i][j]);
                }
            }

            return result.ToArray();
        }
    }
}

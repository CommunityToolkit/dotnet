// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Streams;

[TestClass]
public partial class Test_MemoryStream
{
    [TestMethod]
    public void Test_MemoryStream_Lifecycle()
    {
        Memory<byte> memory = new byte[100];

        Stream stream = memory.AsStream();

        Assert.IsTrue(stream.CanRead);
        Assert.IsTrue(stream.CanSeek);
        Assert.IsTrue(stream.CanWrite);
        Assert.AreEqual(stream.Length, memory.Length);
        Assert.AreEqual(stream.Position, 0);

        stream.Dispose();

        Assert.IsFalse(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsFalse(stream.CanWrite);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Length);
        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Position);
    }

    [TestMethod]
    public void Test_MemoryStream_Seek()
    {
        Stream stream = new byte[100].AsMemory().AsStream();

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

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3536
    [TestMethod]
    public void Test_MemoryStream_WriteToEndAndRefreshPosition()
    {
        byte[] array = new byte[10];
        byte[] temp = new byte[1];
        ReadOnlyMemory<byte> memory = array;

        using Stream? stream = memory.AsStream();

        for (int i = 0; i < array.Length; i++)
        {
            int read = stream.Read(temp, 0, 1);

            Assert.AreEqual(read, 1);
            Assert.AreEqual(stream.Position, i + 1);
        }

        Assert.AreEqual(stream.Position, array.Length);

        // These should not throw, seeking to the end is valid
        stream.Position = stream.Position;
        Assert.AreEqual(stream.Position, array.Length);

        _ = stream.Seek(array.Length, SeekOrigin.Begin);
        Assert.AreEqual(stream.Position, array.Length);

        _ = stream.Seek(0, SeekOrigin.Current);
        Assert.AreEqual(stream.Position, array.Length);

        _ = stream.Seek(0, SeekOrigin.End);
        Assert.AreEqual(stream.Position, array.Length);
    }

    [TestMethod]
    public void Test_MemoryStream_ReadWrite_Array()
    {
        Stream stream = new byte[100].AsMemory().AsStream();

        byte[] data = CreateRandomData(64);

        stream.Write(data, 0, data.Length);

        Assert.AreEqual(stream.Position, data.Length);

        stream.Position = 0;

        byte[] result = new byte[data.Length];

        int bytesRead = stream.Read(result, 0, result.Length);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.AsSpan().SequenceEqual(result));

        _ = Assert.ThrowsException<ArgumentNullException>(() => stream.Write(null!, 0, 10));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Write(data, -1, 10));
        _ = Assert.ThrowsException<ArgumentException>(() => stream.Write(data, 200, 10));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Write(data, 0, -24));
        _ = Assert.ThrowsException<ArgumentException>(() => stream.Write(data, 0, 200));

        stream.Dispose();

        _ = Assert.ThrowsException<ObjectDisposedException>(() => stream.Write(data, 0, data.Length));
    }

    [TestMethod]
    public async Task Test_MemoryStream_ReadWriteAsync_Array()
    {
        Stream stream = new byte[100].AsMemory().AsStream();

        byte[] data = CreateRandomData(64);

        await stream.WriteAsync(data, 0, data.Length);

        Assert.AreEqual(stream.Position, data.Length);

        stream.Position = 0;

        byte[] result = new byte[data.Length];

        int bytesRead = await stream.ReadAsync(result, 0, result.Length);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.AsSpan().SequenceEqual(result));

        _ = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => stream.WriteAsync(null!, 0, 10));
        _ = await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => stream.WriteAsync(data, -1, 10));
        _ = await Assert.ThrowsExceptionAsync<ArgumentException>(() => stream.WriteAsync(data, 200, 10));
        _ = await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => stream.WriteAsync(data, 0, -24));
        _ = await Assert.ThrowsExceptionAsync<ArgumentException>(() => stream.WriteAsync(data, 0, 200));

        stream.Dispose();

        _ = await Assert.ThrowsExceptionAsync<ObjectDisposedException>(() => stream.WriteAsync(data, 0, data.Length));
    }

    [TestMethod]
    public void Test_MemoryStream_ReadWriteByte()
    {
        Stream stream = new byte[4].AsMemory().AsStream();

        ReadOnlySpan<byte> data = stackalloc byte[] { 1, 128, 255, 32 };

        foreach (HighPerformance.Enumerables.ReadOnlySpanEnumerable<byte>.Item item in data.Enumerate())
        {
            Assert.AreEqual(stream.Position, item.Index);

            stream.WriteByte(item.Value);
        }

        Assert.AreEqual(stream.Position, data.Length);

        stream.Position = 0;

        Span<byte> result = stackalloc byte[4];

        foreach (ref byte value in result)
        {
            value = checked((byte)stream.ReadByte());
        }

        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.SequenceEqual(result));

        _ = Assert.ThrowsException<ArgumentException>(() => stream.WriteByte(128));

        int exitCode = stream.ReadByte();

        Assert.AreEqual(exitCode, -1);
    }

    [TestMethod]
    public void Test_MemoryStream_ReadWrite_Span()
    {
        Stream stream = new byte[100].AsMemory().AsStream();

        Memory<byte> data = CreateRandomData(64);

        // This will use the extension when on .NET Standard 2.0, as the
        // Stream class doesn't have Spam<T> or Memory<T> public APIs there.
        stream.Write(data.Span);

        Assert.AreEqual(stream.Position, data.Length);

        stream.Position = 0;

        Span<byte> result = new byte[data.Length];

        int bytesRead = stream.Read(result);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result));
    }

    [TestMethod]
    public async Task Test_MemoryStream_ReadWriteAsync_Memory()
    {
        Stream stream = new byte[100].AsMemory().AsStream();

        Memory<byte> data = CreateRandomData(64);

        await stream.WriteAsync(data);

        Assert.AreEqual(stream.Position, data.Length);

        stream.Position = 0;

        Memory<byte> result = new byte[data.Length];

        int bytesRead = await stream.ReadAsync(result);

        Assert.AreEqual(bytesRead, result.Length);
        Assert.AreEqual(stream.Position, data.Length);
        Assert.IsTrue(data.Span.SequenceEqual(result.Span));
    }

    [TestMethod]
    public void Test_MemoryStream_CopyTo()
    {
        Stream source = new byte[100].AsMemory().AsStream();

        Memory<byte> data = CreateRandomData(64);

        source.Write(data.Span);

        Assert.AreEqual(source.Position, data.Length);

        source.Position = 0;

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
    public async Task Test_MemoryStream_CopyToAsync()
    {
        Stream source = new byte[100].AsMemory().AsStream();

        Memory<byte> data = CreateRandomData(64);

        await source.WriteAsync(data);

        Assert.AreEqual(source.Position, data.Length);

        source.Position = 0;

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

    /// <summary>
    /// Creates a random <see cref="byte"/> array filled with random data.
    /// </summary>
    /// <param name="count">The number of array items to create.</param>
    /// <returns>The returned random array.</returns>
    internal static byte[] CreateRandomData(int count)
    {
        Random? random = new(DateTime.Now.Ticks.GetHashCode());

        byte[] data = new byte[count];

        foreach (ref byte n in MemoryMarshal.AsBytes(data.AsSpan()))
        {
            n = (byte)random.Next(0, byte.MaxValue);
        }

        return data;
    }
}

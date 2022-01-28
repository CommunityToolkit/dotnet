// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.IO;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Streams;

[TestClass]
public class Test_IMemoryOwnerStream
{
    [TestMethod]
    public void Test_IMemoryOwnerStream_Lifecycle()
    {
        MemoryOwner<byte> buffer = MemoryOwner<byte>.Allocate(100);

        Stream stream = buffer.AsStream();

        Assert.IsTrue(stream.CanRead);
        Assert.IsTrue(stream.CanSeek);
        Assert.IsTrue(stream.CanWrite);
        Assert.AreEqual(stream.Length, buffer.Length);
        Assert.AreEqual(stream.Position, 0);

        stream.Dispose();

        _ = Assert.ThrowsException<ObjectDisposedException>(() => buffer.Memory);
    }
}

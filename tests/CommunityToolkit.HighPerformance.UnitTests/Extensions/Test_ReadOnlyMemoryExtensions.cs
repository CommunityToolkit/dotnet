// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

[TestClass]
public class Test_ReadOnlyMemoryExtensions
{
    [TestMethod]
    public void Test_ReadOnlyMemoryExtensions_EmptyMemoryStream()
    {
        ReadOnlyMemory<byte> memory = default;

        Stream stream = memory.AsStream();

        Assert.IsNotNull(stream);
        Assert.AreEqual(stream.Length, memory.Length);
        Assert.IsFalse(stream.CanWrite);
    }

    [TestMethod]
    public void Test_ReadOnlyMemoryExtensions_MemoryStream()
    {
        ReadOnlyMemory<byte> memory = new byte[1024];

        Stream stream = memory.AsStream();

        Assert.IsNotNull(stream);
        Assert.AreEqual(stream.Length, memory.Length);
        Assert.IsFalse(stream.CanWrite);
    }

#if NET8_0_OR_GREATER
    [TestMethod]
    public void Test_ReadOnlyMemoryExtensions_AsMemory2D_Empty()
    {
        ReadOnlyMemory2D<int> empty1 = ((ReadOnlyMemory<int>)Array.Empty<int>().AsMemory()).AsMemory2D(0, 0);

        Assert.IsTrue(empty1.IsEmpty);
        Assert.AreEqual(0, empty1.Length);
        Assert.AreEqual(0, empty1.Width);
        Assert.AreEqual(0, empty1.Height);

        ReadOnlyMemory2D<int> empty2 = ((ReadOnlyMemory<int>)Array.Empty<int>().AsMemory()).AsMemory2D(4, 0);

        Assert.IsTrue(empty2.IsEmpty);
        Assert.AreEqual(0, empty2.Length);
        Assert.AreEqual(0, empty2.Width);
        Assert.AreEqual(4, empty2.Height);

        ReadOnlyMemory2D<int> empty3 = ((ReadOnlyMemory<int>)Array.Empty<int>().AsMemory()).AsMemory2D(0, 7);

        Assert.IsTrue(empty3.IsEmpty);
        Assert.AreEqual(0, empty3.Length);
        Assert.AreEqual(7, empty3.Width);
        Assert.AreEqual(0, empty3.Height);
    }

    [TestMethod]
    public void Test_ReadOnlyMemoryExtensions_AsMemory3D_Empty()
    {
        ReadOnlyMemory3D<int> empty1 = ((ReadOnlyMemory<int>)Array.Empty<int>().AsMemory()).AsMemory3D(0, 0, 0);

        Assert.IsTrue(empty1.IsEmpty);
        Assert.AreEqual(0, empty1.Length);
        Assert.AreEqual(0, empty1.Width);
        Assert.AreEqual(0, empty1.Height);

        ReadOnlyMemory3D<int> empty2 = ((ReadOnlyMemory<int>)Array.Empty<int>().AsMemory()).AsMemory3D(4, 0, 0);

        Assert.IsTrue(empty2.IsEmpty);
        Assert.AreEqual(0, empty2.Length);
        Assert.AreEqual(4, empty2.Width);
        Assert.AreEqual(0, empty2.Height);
        Assert.AreEqual(0, empty2.Depth);

        ReadOnlyMemory3D<int> empty3 = ((ReadOnlyMemory<int>)Array.Empty<int>().AsMemory()).AsMemory3D(0, 7, 0);

        Assert.IsTrue(empty3.IsEmpty);
        Assert.AreEqual(0, empty3.Length);
        Assert.AreEqual(0, empty3.Width);
        Assert.AreEqual(7, empty3.Height);
        Assert.AreEqual(0, empty2.Depth);

        ReadOnlyMemory3D<int> empty4 = ((ReadOnlyMemory<int>)Array.Empty<int>().AsMemory()).AsMemory3D(0, 0, 3);

        Assert.IsTrue(empty4.IsEmpty);
        Assert.AreEqual(0, empty4.Length);
        Assert.AreEqual(3, empty4.Width);
        Assert.AreEqual(0, empty4.Height);
        Assert.AreEqual(0, empty2.Depth);
    }
#endif
}
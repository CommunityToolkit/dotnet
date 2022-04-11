// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Linq;
using CommunityToolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

[TestClass]
public class Test_ArrayPoolExtensions
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_ArrayPoolExtensions_Resize_InvalidSize()
    {
        int[]? array = null;

        ArrayPool<int>.Shared.Resize(ref array, -1);
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_Resize_NewArray()
    {
        int[]? array = null;

        ArrayPool<int>.Shared.Resize(ref array, 10);

        Assert.IsNotNull(array);
        Assert.IsTrue(array.Length >= 10);
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_Resize_SameSize()
    {
        int[] array = ArrayPool<int>.Shared.Rent(10);
        int[] backup = array;

        ArrayPool<int>.Shared.Resize(ref array, array.Length);

        Assert.AreSame(array, backup);
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_Resize_Expand()
    {
        int[] array = ArrayPool<int>.Shared.Rent(16);
        int[] backup = array;

        array.AsSpan().Fill(7);

        ArrayPool<int>.Shared.Resize(ref array, 32);

        Assert.AreNotSame(array, backup);
        Assert.IsTrue(array.Length >= 32);
        Assert.IsTrue(array.AsSpan(0, 16).ToArray().All(i => i == 7));
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_Resize_Shrink()
    {
        int[] array = ArrayPool<int>.Shared.Rent(32);
        int[] backup = array;

        array.AsSpan().Fill(7);

        ArrayPool<int>.Shared.Resize(ref array, 16);

        Assert.AreNotSame(array, backup);
        Assert.IsTrue(array.Length >= 16);
        Assert.IsTrue(array.AsSpan(0, 16).ToArray().All(i => i == 7));
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_Resize_Clear()
    {
        int[] array = ArrayPool<int>.Shared.Rent(16);
        int[] backup = array;

        array.AsSpan().Fill(7);

        ArrayPool<int>.Shared.Resize(ref array, 0, true);

        Assert.AreNotSame(array, backup);
        Assert.IsTrue(backup.AsSpan(0, 16).ToArray().All(i => i == 0));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_ArrayPoolExtensions_EnsureCapacity_InvalidCapacity()
    {
        int[]? array = null;

        ArrayPool<int>.Shared.EnsureCapacity(ref array, -1);
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_EnsureCapacity_IdenticalCapacity()
    {
        int[]? array = ArrayPool<int>.Shared.Rent(10);
        int[]? backup = array;

        ArrayPool<int>.Shared.EnsureCapacity(ref array, 10);
        Assert.AreSame(backup, array);
        Assert.IsTrue(array.Length >= 10);
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_EnsureCapacity_NewArray()
    {
        int[]? array = null;

        ArrayPool<int>.Shared.EnsureCapacity(ref array, 7);

        Assert.IsNotNull(array);
        Assert.IsTrue(array.Length >= 7);
        int[]? backup = array;

        ArrayPool<int>.Shared.EnsureCapacity(ref array, 64);

        Assert.AreNotSame(backup, array);
        Assert.IsTrue(array.Length >= 64);
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_EnsureCapacity_SufficientCapacity()
    {
        int[]? array = ArrayPool<int>.Shared.Rent(16);
        int[]? backup = array;

        ArrayPool<int>.Shared.EnsureCapacity(ref array, 8);
        Assert.AreSame(backup, array);

        ArrayPool<int>.Shared.EnsureCapacity(ref array, 16);
        Assert.AreSame(backup, array);

        ArrayPool<int>.Shared.EnsureCapacity(ref array, 0);
        Assert.AreSame(backup, array);
    }

    [TestMethod]
    public void Test_ArrayPoolExtensions_EnsureCapacity_ClearArray()
    {
        int[]? array = ArrayPool<int>.Shared.Rent(16);
        int[]? backup = array;

        array.AsSpan().Fill(7);
        Assert.IsTrue(backup.All(i => i == 7));

        ArrayPool<int>.Shared.EnsureCapacity(ref array, 256, true);

        Assert.AreNotSame(backup, array);
        Assert.IsTrue(backup.All(i => i == default));
        Assert.IsTrue(array.Length >= 256);
    }
}

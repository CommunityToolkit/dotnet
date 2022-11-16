// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Common.UnitTests.Extensions;

[TestClass]
public class Test_ArrayExtensions
{
    [TestMethod]
    public void Test_ArrayExtensions_Jagged_GetColumn()
    {
        int[][] array =
        {
            new int[] { 5, 2, 4 },
            new int[] { 6, 3 },
            new int[] { 7 }
        };

        int[]? col = array.GetColumn(1).ToArray();

        CollectionAssert.AreEquivalent(new int[] { 2, 3, 0 }, col);
    }

    [TestMethod]
    public void Test_ArrayExtensions_Jagged_GetColumn_Exception()
    {
        int[][] array =
        {
            new int[] { 5, 2, 4 },
            new int[] { 6, 3 },
            new int[] { 7 }
        };

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _ = array.GetColumn(-1).ToArray();
            });

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _ = array.GetColumn(3).ToArray();
            });
    }

    [TestMethod]
    public void Test_ArrayExtensions_Rectangular_ToString()
    {
        int[,] array =
        {
            { 5, 2,  4 },
            { 6, 3, -1 },
            { 7, 0,  9 }
        };

        string value = array.ToArrayString();

        Debug.WriteLine(value);

        Assert.AreEqual("[[5,\t2,\t4]," + Environment.NewLine + " [6,\t3,\t-1]," + Environment.NewLine + " [7,\t0,\t9]]", value);
    }

    [TestMethod]
    public void Test_ArrayExtensions_Jagged_ToString()
    {
        int[][] array =
        {
            new int[] { 5, 2 },
            new int[] { 6, 3, -1, 2 },
            new int[] { 7, 0,  9 }
        };

        string value = array.ToArrayString();

        Debug.WriteLine(value);

        Assert.AreEqual("[[5,\t2]," + Environment.NewLine + " [6,\t3,\t-1,\t2]," + Environment.NewLine + " [7,\t0,\t9]]", value);
    }
}

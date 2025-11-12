// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

/* ====================================================================
*                                 NOTE
* ====================================================================
* All the tests here mirror the ones for Memory2D<T>, as the two types
* are basically the same except for some small differences in return types
* or some checks being done upon construction. See comments in the test
* file for Memory2D<T> for more info on these tests. */
[TestClass]
public class Test_ReadOnlyMemory2DT
{
    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Empty()
    {
        ReadOnlyMemory2D<int> empty1 = default;

        Assert.IsTrue(empty1.IsEmpty);
        Assert.AreEqual(0, empty1.Length);
        Assert.AreEqual(0, empty1.Width);
        Assert.AreEqual(0, empty1.Height);

        ReadOnlyMemory2D<string> empty2 = ReadOnlyMemory2D<string>.Empty;

        Assert.IsTrue(empty2.IsEmpty);
        Assert.AreEqual(0, empty2.Length);
        Assert.AreEqual(0, empty2.Width);
        Assert.AreEqual(0, empty2.Height);

#if NET6_0_OR_GREATER
        MemoryManager<int> memoryManager = new UnmanagedSpanOwner<int>(1);
        ReadOnlyMemory2D<int> empty5 = new(memoryManager, 0, 0);

        Assert.IsTrue(empty5.IsEmpty);
        Assert.AreEqual(0, empty5.Length);
        Assert.AreEqual(0, empty5.Width);
        Assert.AreEqual(0, empty5.Height);

        ReadOnlyMemory2D<int> empty6 = new(memoryManager, 4, 0);

        Assert.IsTrue(empty6.IsEmpty);
        Assert.AreEqual(0, empty6.Length);
        Assert.AreEqual(0, empty6.Width);
        Assert.AreEqual(4, empty6.Height);

        ReadOnlyMemory2D<int> empty7 = new(memoryManager, 0, 7);

        Assert.IsTrue(empty7.IsEmpty);
        Assert.AreEqual(0, empty7.Length);
        Assert.AreEqual(7, empty7.Width);
        Assert.AreEqual(0, empty7.Height);
#endif
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Array1DConstructor()
    {
        int[] array =
        {
            1, 2, 3, 4, 5, 6
        };

        ReadOnlyMemory2D<int> memory2d = new(array, 1, 2, 2, 1);

        Assert.IsFalse(memory2d.IsEmpty);
        Assert.AreEqual(4, memory2d.Length);
        Assert.AreEqual(2, memory2d.Width);
        Assert.AreEqual(2, memory2d.Height);
        Assert.AreEqual(2, memory2d.Span[0, 0]);
        Assert.AreEqual(6, memory2d.Span[1, 1]);

        // Here we check to ensure a covariant array conversion is allowed for ReadOnlyMemory2D<T>
        _ = new ReadOnlyMemory2D<object>(new string[1], 1, 1);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, -99, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 0, -10, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 0, 1, 1, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 0, 1, -100, 1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlyMemory2D<int>(array, 0, 2, 4, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlyMemory2D<int>(array, 0, 3, 3, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlyMemory2D<int>(array, 1, 2, 3, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlyMemory2D<int>(array, 0, 10, 1, 120));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Array2DConstructor_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array);

        Assert.IsFalse(memory2d.IsEmpty);
        Assert.AreEqual(6, memory2d.Length);
        Assert.AreEqual(3, memory2d.Width);
        Assert.AreEqual(2, memory2d.Height);
        Assert.AreEqual(2, memory2d.Span[0, 1]);
        Assert.AreEqual(6, memory2d.Span[1, 2]);

        _ = new ReadOnlyMemory2D<object>(new string[1, 2]);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Array2DConstructor_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array, 0, 1, 2, 2);

        Assert.IsFalse(memory2d.IsEmpty);
        Assert.AreEqual(4, memory2d.Length);
        Assert.AreEqual(2, memory2d.Width);
        Assert.AreEqual(2, memory2d.Height);
        Assert.AreEqual(2, memory2d.Span[0, 0]);
        Assert.AreEqual(6, memory2d.Span[1, 1]);

        _ = new ReadOnlyMemory2D<object>(new string[1, 2]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<object>(new string[1, 2], 0, 0, 2, 2));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Array3DConstructor_1()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 10, 20, 30 },
                { 40, 50, 60 }
            }
        };

        ReadOnlyMemory2D<int> memory2d = new(array, 1);

        Assert.IsFalse(memory2d.IsEmpty);
        Assert.AreEqual(6, memory2d.Length);
        Assert.AreEqual(3, memory2d.Width);
        Assert.AreEqual(2, memory2d.Height);
        Assert.AreEqual(20, memory2d.Span[0, 1]);
        Assert.AreEqual(60, memory2d.Span[1, 2]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 20));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 2));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Array3DConstructor_2()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 10, 20, 30 },
                { 40, 50, 60 }
            }
        };

        ReadOnlyMemory2D<int> memory2d = new(array, 1, 0, 1, 2, 2);

        Assert.IsFalse(memory2d.IsEmpty);
        Assert.AreEqual(4, memory2d.Length);
        Assert.AreEqual(2, memory2d.Width);
        Assert.AreEqual(2, memory2d.Height);
        Assert.AreEqual(20, memory2d.Span[0, 0]);
        Assert.AreEqual(60, memory2d.Span[1, 1]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, -1, 1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 1, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 1, 1, -1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 1, 1, 1, -1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 1, 1, 1, 1, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 2, 0, 0, 2, 3));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 0, 0, 1, 2, 3));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 0, 0, 0, 2, 4));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array, 0, 0, 0, 3, 3));
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ReadOnlyMemory2DT_ReadOnlyMemoryConstructor()
    {
        ReadOnlyMemory<int> memory = new[]
        {
            1, 2, 3, 4, 5, 6
        };

        ReadOnlyMemory2D<int> memory2d = memory.AsMemory2D(1, 2, 2, 1);

        Assert.IsFalse(memory2d.IsEmpty);
        Assert.AreEqual(4, memory2d.Length);
        Assert.AreEqual(2, memory2d.Width);
        Assert.AreEqual(2, memory2d.Height);
        Assert.AreEqual(2, memory2d.Span[0, 0]);
        Assert.AreEqual(6, memory2d.Span[1, 1]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory2D(-99, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory2D(0, -10, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory2D(0, 1, 1, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory2D(0, 1, -100, 1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory2D(0, 2, 4, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory2D(0, 3, 3, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory2D(1, 2, 3, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory2D(0, 10, 1, 120));
    }
#endif

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Slice_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array);

        ReadOnlyMemory2D<int> slice1 = memory2d.Slice(1, 1, 1, 2);

        Assert.AreEqual(2, slice1.Length);
        Assert.AreEqual(1, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(5, slice1.Span[0, 0]);
        Assert.AreEqual(6, slice1.Span[0, 1]);

        ReadOnlyMemory2D<int> slice2 = memory2d.Slice(0, 1, 2, 2);

        Assert.AreEqual(4, slice2.Length);
        Assert.AreEqual(2, slice2.Height);
        Assert.AreEqual(2, slice2.Width);
        Assert.AreEqual(2, slice2.Span[0, 0]);
        Assert.AreEqual(5, slice2.Span[1, 0]);
        Assert.AreEqual(6, slice2.Span[1, 1]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(-1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(1, -1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(1, 1, 1, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(1, 1, -1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(10, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(1, 12, 1, 12));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(1, 1, 55, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(0, 0, 2, 4));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(0, 0, 3, 3));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(0, 1, 2, 3));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory2D<int>(array).Slice(1, 0, 2, 3));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Slice_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array);

        ReadOnlyMemory2D<int> slice1 = memory2d.Slice(0, 0, 2, 2);

        Assert.AreEqual(4, slice1.Length);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(1, slice1.Span[0, 0]);
        Assert.AreEqual(5, slice1.Span[1, 1]);

        ReadOnlyMemory2D<int> slice2 = slice1.Slice(1, 0, 1, 2);

        Assert.AreEqual(2, slice2.Length);
        Assert.AreEqual(1, slice2.Height);
        Assert.AreEqual(2, slice2.Width);
        Assert.AreEqual(4, slice2.Span[0, 0]);
        Assert.AreEqual(5, slice2.Span[0, 1]);

        ReadOnlyMemory2D<int> slice3 = slice2.Slice(0, 1, 1, 1);

        Assert.AreEqual(1, slice3.Length);
        Assert.AreEqual(1, slice3.Height);
        Assert.AreEqual(1, slice3.Width);
        Assert.AreEqual(5, slice3.Span[0, 0]);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_TryGetReadOnlyMemory_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array);

        bool success = memory2d.TryGetMemory(out ReadOnlyMemory<int> memory);

#if NETFRAMEWORK
        Assert.IsFalse(success);
        Assert.IsTrue(memory.IsEmpty);
#else
        Assert.IsTrue(success);
        Assert.HasCount(memory.Length, array);
        Assert.IsTrue(Unsafe.AreSame(ref array[0, 0], ref Unsafe.AsRef(in memory.Span[0])));
#endif
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_TryGetReadOnlyMemory_2()
    {
        int[] array = { 1, 2, 3, 4 };

        ReadOnlyMemory2D<int> memory2d = new(array, 2, 2);

        bool success = memory2d.TryGetMemory(out ReadOnlyMemory<int> memory);

        Assert.IsTrue(success);
        Assert.HasCount(memory.Length, array);
        Assert.AreEqual(3, memory.Span[2]);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ReadOnlyMemory2DT_TryGetReadOnlyMemory_3()
    {
        ReadOnlyMemory<int> data = new[] { 1, 2, 3, 4 };

        ReadOnlyMemory2D<int> memory2d = data.AsMemory2D(2, 2);

        bool success = memory2d.TryGetMemory(out ReadOnlyMemory<int> memory);

        Assert.IsTrue(success);
        Assert.AreEqual(memory.Length, data.Length);
        Assert.AreEqual(3, memory.Span[2]);
    }
#endif

    [TestMethod]
    public unsafe void Test_ReadOnlyMemory2DT_Pin_1()
    {
        int[] array = { 1, 2, 3, 4 };

        ReadOnlyMemory2D<int> memory2d = new(array, 2, 2);

        using System.Buffers.MemoryHandle pin = memory2d.Pin();

        Assert.AreEqual(1, ((int*)pin.Pointer)[0]);
        Assert.AreEqual(4, ((int*)pin.Pointer)[3]);
    }

    [TestMethod]
    public unsafe void Test_ReadOnlyMemory2DT_Pin_2()
    {
        int[] array = { 1, 2, 3, 4 };

        ReadOnlyMemory2D<int> memory2d = new(array, 2, 2);

        using System.Buffers.MemoryHandle pin = memory2d.Pin();

        Assert.AreEqual(1, ((int*)pin.Pointer)[0]);
        Assert.AreEqual(4, ((int*)pin.Pointer)[3]);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_ToArray_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array);

        int[,] copy = memory2d.ToArray();

        Assert.AreEqual(copy.GetLength(0), array.GetLength(0));
        Assert.AreEqual(copy.GetLength(1), array.GetLength(1));

        CollectionAssert.AreEqual(array, copy);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_ToArray_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array, 0, 0, 2, 2);

        int[,] copy = memory2d.ToArray();

        Assert.AreEqual(2, copy.GetLength(0));
        Assert.AreEqual(2, copy.GetLength(1));

        int[,] expected =
        {
            { 1, 2 },
            { 4, 5 }
        };

        CollectionAssert.AreEqual(expected, copy);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_Equals()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> readOnlyMemory2D = new(array);

        Assert.IsFalse(readOnlyMemory2D.Equals(null));
        Assert.IsFalse(readOnlyMemory2D.Equals(new ReadOnlyMemory2D<int>(array, 0, 1, 2, 2)));
        Assert.IsTrue(readOnlyMemory2D.Equals(new ReadOnlyMemory2D<int>(array)));
        Assert.IsTrue(readOnlyMemory2D.Equals(readOnlyMemory2D));

        Memory2D<int> memory2d = array;

        Assert.IsTrue(readOnlyMemory2D.Equals((object)memory2d));
        Assert.IsFalse(readOnlyMemory2D.Equals((object)memory2d.Slice(0, 1, 2, 2)));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_GetHashCode()
    {
        Assert.AreEqual(0, ReadOnlyMemory2D<int>.Empty.GetHashCode());

        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array);

        int a = memory2d.GetHashCode();
        int b = memory2d.GetHashCode();

        Assert.AreEqual(a, b);

        int c = new ReadOnlyMemory2D<int>(array, 0, 1, 2, 2).GetHashCode();

        Assert.AreNotEqual(a, c);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory2DT_ToString()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlyMemory2D<int> memory2d = new(array);

        string text = memory2d.ToString();

        const string expected = "CommunityToolkit.HighPerformance.ReadOnlyMemory2D<System.Int32>[2, 3]";

        Assert.AreEqual(expected, text);
    }

#if NET6_0_OR_GREATER
    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3536
    [TestMethod]
    [DataRow(720, 1280)]
    public void Test_ReadOnlyMemory2DT_CastAndSlice_WorksCorrectly(int height, int width)
    {
        ReadOnlyMemory2D<int> data =
            new byte[width * height * sizeof(int)]
            .AsMemory()
            .Cast<byte, int>()
            .AsMemory2D(height: height, width: width);

        ReadOnlyMemory2D<int> slice = data.Slice(
            row: height / 2,
            column: 0,
            height: height / 2,
            width: width);

        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[height / 2, 0]), ref Unsafe.AsRef(in slice.Span[0, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[height / 2, width - 1]), ref Unsafe.AsRef(in slice.Span[0, width - 1])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[height - 1, 0]), ref Unsafe.AsRef(in slice.Span[(height / 2) - 1, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[height - 1, width - 1]), ref Unsafe.AsRef(in slice.Span[(height / 2) - 1, width - 1])));
    }
#endif
}

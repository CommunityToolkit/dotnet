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
 * All the tests here mirror the ones for ReadOnlyMemory3D<T>, as the two types
 * are basically the same except for some small differences in return types
 * or some checks being done upon construction. See comments in the test
 * file for ReadOnlyMemory3D<T> for more info on these tests. */
[TestClass]
public class Test_ReadOnlyMemory3DT
{
    [TestMethod]
    public void Test_ReadOnlyMemory3DT_Empty()
    {
        ReadOnlyMemory3D<int> empty1 = default;

        Assert.IsTrue(empty1.IsEmpty);
        Assert.AreEqual(0, empty1.Length);
        Assert.AreEqual(0, empty1.Depth);
        Assert.AreEqual(0, empty1.Height);
        Assert.AreEqual(0, empty1.Width);

        ReadOnlyMemory3D<string> empty2 = ReadOnlyMemory3D<string>.Empty;

        Assert.IsTrue(empty2.IsEmpty);
        Assert.AreEqual(0, empty2.Length);
        Assert.AreEqual(0, empty2.Depth);
        Assert.AreEqual(0, empty2.Height);
        Assert.AreEqual(0, empty2.Width);

        ReadOnlyMemory3D<int> empty3 = new int[0, 2, 3];

        Assert.IsTrue(empty3.IsEmpty);
        Assert.AreEqual(0, empty3.Length);
        Assert.AreEqual(0, empty3.Depth);
        Assert.AreEqual(2, empty3.Height);
        Assert.AreEqual(3, empty3.Width);

        ReadOnlyMemory3D<int> empty4 = new int[2, 0, 3];

        Assert.IsTrue(empty4.IsEmpty);
        Assert.AreEqual(0, empty4.Length);
        Assert.AreEqual(2, empty4.Depth);
        Assert.AreEqual(0, empty4.Height);
        Assert.AreEqual(3, empty4.Width);

        ReadOnlyMemory3D<int> empty5 = new int[2, 3, 0];

        Assert.IsTrue(empty5.IsEmpty);
        Assert.AreEqual(0, empty5.Length);
        Assert.AreEqual(2, empty5.Depth);
        Assert.AreEqual(3, empty5.Height);
        Assert.AreEqual(0, empty5.Width);

#if NET6_0_OR_GREATER
        MemoryManager<int> memoryManager = new UnmanagedSpanOwner<int>(1);
        ReadOnlyMemory3D<int> empty6 = new(memoryManager, 0, 0, 0);

        Assert.IsTrue(empty6.IsEmpty);
        Assert.AreEqual(0, empty6.Length);
        Assert.AreEqual(0, empty6.Depth);
        Assert.AreEqual(0, empty6.Height);
        Assert.AreEqual(0, empty6.Width);

        ReadOnlyMemory3D<int> empty7 = new(memoryManager, 2, 0, 0);

        Assert.IsTrue(empty7.IsEmpty);
        Assert.AreEqual(0, empty7.Length);
        Assert.AreEqual(2, empty7.Depth);
        Assert.AreEqual(0, empty7.Height);
        Assert.AreEqual(0, empty7.Width);

        ReadOnlyMemory3D<int> empty8 = new(memoryManager, 0, 2, 0);

        Assert.IsTrue(empty8.IsEmpty);
        Assert.AreEqual(0, empty8.Length);
        Assert.AreEqual(0, empty8.Depth);
        Assert.AreEqual(2, empty8.Height);
        Assert.AreEqual(0, empty8.Width);

        ReadOnlyMemory3D<int> empty9 = new(memoryManager, 0, 0, 3);

        Assert.IsTrue(empty9.IsEmpty);
        Assert.AreEqual(0, empty9.Length);
        Assert.AreEqual(0, empty9.Depth);
        Assert.AreEqual(0, empty9.Height);
        Assert.AreEqual(3, empty9.Width);
#endif
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_Array1DConstructor()
    {
        int[] array =
        {
            1, 2, 3, 4, 5,
            6, 7, 8, 9, 10,
            11, 12, 13, 14, 15,
            16, 17, 18, 19, 20
        };

        ReadOnlyMemory3D<int> memory3d = new(array, 1, 2, 2, 2, 2, 1);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(8, memory3d.Length);
        Assert.AreEqual(2, memory3d.Depth);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(2, memory3d.Width);
        Assert.AreEqual(2, memory3d.Span[0, 0, 0]);
        Assert.AreEqual(14, memory3d.Span[1, 1, 1]);

        // Here we check to ensure a covariant array conversion is allowed for ReadOnlyMemory3D<T>
        _ = new ReadOnlyMemory3D<object>(new string[1], 1, 1, 1);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, -1, 1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, -1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, 1, -1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, 1, 1, -1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, 1, 1, 1, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, 1, 1, 1, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlyMemory3D<int>(array, 0, 2, 2, 2, 30, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlyMemory3D<int>(array, 0, 2, 2, 2, 0, 30));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_Array3DConstructor_1()
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

        ReadOnlyMemory3D<int> memory3d = new(array);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(12, memory3d.Length);
        Assert.AreEqual(2, memory3d.Depth);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(3, memory3d.Width);
        Assert.AreEqual(2, memory3d.Span[0, 0, 1]);
        Assert.AreEqual(60, memory3d.Span[1, 1, 2]);

        _ = new ReadOnlyMemory3D<object>(new string[1, 1, 2]);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_Array3DConstructor_2()
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

        ReadOnlyMemory3D<int> memory3d = new(array, 0, 0, 1, 2, 2, 2);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(8, memory3d.Length);
        Assert.AreEqual(2, memory3d.Depth);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(2, memory3d.Width);
        Assert.AreEqual(2, memory3d.Span[0, 0, 0]);
        Assert.AreEqual(60, memory3d.Span[1, 1, 1]);

        _ = new ReadOnlyMemory3D<object>(new string[1, 1, 2]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, -1, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, -1, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, 0, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, 0, 0, 3, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, 0, 0, 1, 3, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array, 0, 0, 0, 1, 1, 5));
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ReadOnlyMemory3DT_ReadOnlyMemoryConstructor()
    {
        ReadOnlyMemory<int> memory = new[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18
        };

        ReadOnlyMemory3D<int> memory3d = memory.AsMemory3D(1, 2, 2, 2, 1, 2);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(8, memory3d.Length);
        Assert.AreEqual(2, memory3d.Width);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(2, memory3d.Depth);

        Assert.AreEqual(2, memory3d.Span[0, 0, 0]);
        Assert.AreEqual(3, memory3d.Span[0, 0, 1]);
        Assert.AreEqual(6, memory3d.Span[0, 1, 0]);
        Assert.AreEqual(7, memory3d.Span[0, 1, 1]);
        Assert.AreEqual(11, memory3d.Span[1, 0, 0]);
        Assert.AreEqual(16, memory3d.Span[1, 1, 1]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory3D(-99, 1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory3D(0, -10, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory3D(0, 1, -10, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory3D(0, 1, 1, -100, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory3D(0, 1, 1, 1, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => memory.AsMemory3D(0, 1, 1, 1, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory3D(0, 2, 4, 4, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory3D(0, 3, 3, 3, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory3D(1, 2, 3, 3, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory3D(0, 2, 2, 2, 11, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => memory.AsMemory3D(0, 2, 2, 2, 0, 20));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_MemoryManagerConstructor()
    {
        MemoryManager<int> memoryManager = new UnmanagedSpanOwner<int>(8);

        ReadOnlyMemory3D<int> memory3d = new(memoryManager, 2, 2, 2);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(8, memory3d.Length);
        Assert.AreEqual(2, memory3d.Depth);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(2, memory3d.Width);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(memoryManager, -1, 2, 2, 2, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(memoryManager, 0, -2, 2, 2, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(memoryManager, 0, 2, -2, 2, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(memoryManager, 0, 2, 2, -2, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(memoryManager, 0, 2, 2, 2, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(memoryManager, 0, 2, 2, 2, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlyMemory3D<int>(memoryManager, 0, 2, 2, 2, 30, 0));
    }
#endif

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_Slice_1()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        ReadOnlyMemory3D<int> memory3d = new(array);

        ReadOnlyMemory3D<int> slice1 = memory3d.Slice(1, 0, 1, 1, 2, 2);

        Assert.AreEqual(4, slice1.Length);
        Assert.AreEqual(1, slice1.Depth);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(8, slice1.Span[0, 0, 0]);
        Assert.AreEqual(12, slice1.Span[0, 1, 1]);

        ReadOnlyMemory3D<int> slice2 = memory3d.Slice(0, 1, 0, 2, 1, 3);

        Assert.AreEqual(6, slice2.Length);
        Assert.AreEqual(2, slice2.Depth);
        Assert.AreEqual(1, slice2.Height);
        Assert.AreEqual(3, slice2.Width);
        Assert.AreEqual(4, slice2.Span[0, 0, 0]);
        Assert.AreEqual(10, slice2.Span[1, 0, 0]);
        Assert.AreEqual(12, slice2.Span[1, 0, 2]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(-1, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(0, -1, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(0, 0, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(10, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(0, 10, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(0, 0, 10, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(0, 0, 0, 3, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(0, 0, 0, 1, 3, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlyMemory3D<int>(array).Slice(0, 0, 0, 1, 1, 5));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_Slice_2()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        ReadOnlyMemory3D<int> memory3d = new(array);

        ReadOnlyMemory3D<int> slice1 = memory3d.Slice(0, 0, 0, 2, 2, 2);

        Assert.AreEqual(8, slice1.Length);
        Assert.AreEqual(2, slice1.Depth);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(1, slice1.Span[0, 0, 0]);
        Assert.AreEqual(11, slice1.Span[1, 1, 1]);

        ReadOnlyMemory3D<int> slice2 = slice1.Slice(1, 0, 1, 1, 2, 1);

        Assert.AreEqual(2, slice2.Length);
        Assert.AreEqual(1, slice2.Depth);
        Assert.AreEqual(2, slice2.Height);
        Assert.AreEqual(1, slice2.Width);
        Assert.AreEqual(8, slice2.Span[0, 0, 0]);
        Assert.AreEqual(11, slice2.Span[0, 1, 0]);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_TryGetReadOnlyMemory_1()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        ReadOnlyMemory3D<int> memory3d = new(array);

        bool success = memory3d.TryGetMemory(out ReadOnlyMemory<int> memory);

#if NETFRAMEWORK
        Assert.IsFalse(success);
        Assert.IsTrue(memory.IsEmpty);
#else
        Assert.IsTrue(success);
        Assert.HasCount(memory.Length, array);
        Assert.IsTrue(Unsafe.AreSame(ref array[0, 0, 0], ref Unsafe.AsRef(in memory.Span[0])));
#endif
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_TryGetReadOnlyMemory_2()
    {
        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8 };

        ReadOnlyMemory3D<int> memory3d = new(array, 2, 2, 2);

        bool success = memory3d.TryGetMemory(out ReadOnlyMemory<int> memory);

        Assert.IsTrue(success);
        Assert.HasCount(memory.Length, array);
        Assert.AreEqual(8, memory.Span[7]);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ReadOnlyMemory3DT_TryGetReadOnlyMemory_3()
    {
        Memory<int> data = new[] { 1, 2, 3, 4 };

        ReadOnlyMemory3D<int> memory3d = data.AsMemory3D(1, 2, 2);

        bool success = memory3d.TryGetMemory(out ReadOnlyMemory<int> memory);

        Assert.IsTrue(success);
        Assert.AreEqual(memory.Length, data.Length);
        Assert.AreEqual(3, memory.Span[2]);
    }
#endif

    [TestMethod]
    public unsafe void Test_ReadOnlyMemory3DT_Pin_1()
    {
        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8 };

        ReadOnlyMemory3D<int> memory3d = new(array, 2, 2, 2);

        using MemoryHandle pin = memory3d.Pin();

        Assert.AreEqual(1, ((int*)pin.Pointer)[0]);
        Assert.AreEqual(8, ((int*)pin.Pointer)[7]);
    }

    [TestMethod]
    public unsafe void Test_ReadOnlyMemory3DT_Pin_2()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        ReadOnlyMemory3D<int> memory3d = new(array, 1, 0, 0, 1, 2, 2);

        using MemoryHandle pin = memory3d.Pin();

        Assert.AreEqual(7, ((int*)pin.Pointer)[0]);
        Assert.AreEqual(10, ((int*)pin.Pointer)[3]);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_ToArray_1()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        ReadOnlyMemory3D<int> memory3d = new(array);

        int[,,] copy = memory3d.ToArray();

        Assert.AreEqual(copy.GetLength(0), array.GetLength(0));
        Assert.AreEqual(copy.GetLength(1), array.GetLength(1));
        Assert.AreEqual(copy.GetLength(2), array.GetLength(2));

        CollectionAssert.AreEqual(array, copy);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_ToArray_2()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        ReadOnlyMemory3D<int> memory3d = new(array, 0, 0, 0, 1, 2, 2);

        int[,,] copy = memory3d.ToArray();

        Assert.AreEqual(1, copy.GetLength(0));
        Assert.AreEqual(2, copy.GetLength(1));
        Assert.AreEqual(2, copy.GetLength(2));

        int[,,] expected =
        {
            {
                { 1, 2 },
                { 4, 5 }
            }
        };

        CollectionAssert.AreEqual(expected, copy);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_Equals()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        ReadOnlyMemory3D<int> readOnlyMemory3d = new(array);

        Assert.IsFalse(readOnlyMemory3d.Equals(null));
        Assert.IsFalse(readOnlyMemory3d.Equals(new ReadOnlyMemory3D<int>(array, 0, 0, 1, 2, 2, 2)));
        Assert.IsTrue(readOnlyMemory3d.Equals(new ReadOnlyMemory3D<int>(array)));
        Assert.IsTrue(readOnlyMemory3d.Equals(readOnlyMemory3d));

        Memory3D<int> memory3d = array;

        Assert.IsTrue(readOnlyMemory3d.Equals((object)memory3d));
        Assert.IsFalse(readOnlyMemory3d.Equals((object)memory3d.Slice(0, 0, 1, 2, 2, 2)));
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_GetHashCode()
    {
        Assert.AreEqual(0, ReadOnlyMemory3D<int>.Empty.GetHashCode());

        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            }
        };

        ReadOnlyMemory3D<int> memory3d = new(array);

        int a = memory3d.GetHashCode();
        int b = memory3d.GetHashCode();

        Assert.AreEqual(a, b);

        int c = new ReadOnlyMemory3D<int>(array, 0, 0, 0, 1, 2, 2).GetHashCode();

        Assert.AreNotEqual(a, c);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_ToString()
    {
        int[,,] array = new int[2, 2, 3];

        ReadOnlyMemory3D<int> memory3d = new(array);

        string text = memory3d.ToString();

        const string expected = "CommunityToolkit.HighPerformance.ReadOnlyMemory3D<System.Int32>[2, 2, 3]";

        Assert.AreEqual(expected, text);
    }

    [TestMethod]
    public void Test_ReadOnlyMemory3DT_ImplicitCast()
    {
        int[,,] array = new int[2, 2, 3];

        ReadOnlyMemory3D<int> memory3d_1 = array;
        ReadOnlyMemory3D<int> memory3d_2 = new(array);

        Assert.IsTrue(memory3d_1.Equals(memory3d_2));
    }

#if NET6_0_OR_GREATER
    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3536
    [TestMethod]
    [DataRow(64, 180, 320)]
    public void Test_ReadOnlyMemory3DT_CastAndSlice_WorksCorrectly(int depth, int height, int width)
    {
        ReadOnlyMemory3D<int> data =
            new byte[depth * height * width * sizeof(int)]
            .AsMemory()
            .Cast<byte, int>()
            .AsMemory3D(depth: depth, height: height, width: width);

        ReadOnlyMemory3D<int> slice = data.Slice(
            slice: depth / 2,
            row: height / 2,
            column: 0,
            depth: depth / 2,
            height: height / 2,
            width: width);

        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[depth / 2, height / 2, 0]), ref Unsafe.AsRef(in slice.Span[0, 0, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[depth / 2, height / 2, width - 1]), ref Unsafe.AsRef(in slice.Span[0, 0, width - 1])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[depth / 2, height - 1, 0]), ref Unsafe.AsRef(in slice.Span[0, (height / 2) - 1, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[depth / 2, height - 1, width - 1]), ref Unsafe.AsRef(in slice.Span[0, (height / 2) - 1, width - 1])));

        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[depth - 1, height / 2, 0]), ref Unsafe.AsRef(in slice.Span[(depth / 2) - 1, 0, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[depth - 1, height / 2, width - 1]), ref Unsafe.AsRef(in slice.Span[(depth / 2) - 1, 0, width - 1])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[depth - 1, height - 1, 0]), ref Unsafe.AsRef(in slice.Span[(depth / 2) - 1, (height / 2) - 1, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data.Span[depth - 1, height - 1, width - 1]), ref Unsafe.AsRef(in slice.Span[(depth / 2) - 1, (height / 2) - 1, width - 1])));
    }
#endif
}
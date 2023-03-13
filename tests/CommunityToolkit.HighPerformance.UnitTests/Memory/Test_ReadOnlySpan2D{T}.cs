// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

/* ====================================================================
*                                 NOTE
* ====================================================================
* All the tests here mirror the ones for ReadOnlySpan2D<T>. See comments
* in the test file for Span2D<T> for more info on these tests. */
[TestClass]
public class Test_ReadOnlySpan2DT
{
    [TestMethod]
    public void Test_ReadOnlySpan2DT_Empty()
    {
        ReadOnlySpan2D<int> empty1 = default;

        Assert.IsTrue(empty1.IsEmpty);
        Assert.AreEqual(empty1.Length, 0);
        Assert.AreEqual(empty1.Width, 0);
        Assert.AreEqual(empty1.Height, 0);

        ReadOnlySpan2D<string> empty2 = ReadOnlySpan2D<string>.Empty;

        Assert.IsTrue(empty2.IsEmpty);
        Assert.AreEqual(empty2.Length, 0);
        Assert.AreEqual(empty2.Width, 0);
        Assert.AreEqual(empty2.Height, 0);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_RefConstructor()
    {
        ReadOnlySpan<int> span = stackalloc[]
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        ReadOnlySpan2D<int> span2d = ReadOnlySpan2D<int>.DangerousCreate(span[0], 2, 3, 0);

        Assert.IsFalse(span2d.IsEmpty);
        Assert.AreEqual(span2d.Length, 6);
        Assert.AreEqual(span2d.Width, 3);
        Assert.AreEqual(span2d.Height, 2);
        Assert.AreEqual(span2d[0, 0], 1);
        Assert.AreEqual(span2d[1, 2], 6);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => ReadOnlySpan2D<int>.DangerousCreate(Unsafe.AsRef<int>(null), -1, 0, 0));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => ReadOnlySpan2D<int>.DangerousCreate(Unsafe.AsRef<int>(null), 1, -2, 0));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => ReadOnlySpan2D<int>.DangerousCreate(Unsafe.AsRef<int>(null), 1, 0, -5));
    }
#endif

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_PtrConstructor()
    {
        int* ptr = stackalloc[]
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        ReadOnlySpan2D<int> span2d = new(ptr, 2, 3, 0);

        Assert.IsFalse(span2d.IsEmpty);
        Assert.AreEqual(span2d.Length, 6);
        Assert.AreEqual(span2d.Width, 3);
        Assert.AreEqual(span2d.Height, 2);
        Assert.AreEqual(span2d[0, 0], 1);
        Assert.AreEqual(span2d[1, 2], 6);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>((void*)0, -1, 0, 0));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>((void*)0, 1, -2, 0));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>((void*)0, 1, 0, -5));
        _ = Assert.ThrowsException<ArgumentException>(() => new ReadOnlySpan2D<string>((void*)0, 2, 2, 0));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_Array1DConstructor()
    {
        int[] array =
        {
            1, 2, 3, 4, 5, 6
        };

        ReadOnlySpan2D<int> span2d = new(array, 1, 2, 2, 1);

        Assert.IsFalse(span2d.IsEmpty);
        Assert.AreEqual(span2d.Length, 4);
        Assert.AreEqual(span2d.Width, 2);
        Assert.AreEqual(span2d.Height, 2);
        Assert.AreEqual(span2d[0, 0], 2);
        Assert.AreEqual(span2d[1, 1], 6);

        // Same for ReadOnlyMemory2D<T>, we need to check that covariant array conversions are allowed
        _ = new ReadOnlySpan2D<object>(new string[1], 1, 1);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, -99, 1, 1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 0, -10, 1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 0, 1, 1, -1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 0, 1, -100, 1));
        _ = Assert.ThrowsException<ArgumentException>(() => new ReadOnlySpan2D<int>(array, 0, 10, 1, 120));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_Array2DConstructor_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        Assert.IsFalse(span2d.IsEmpty);
        Assert.AreEqual(span2d.Length, 6);
        Assert.AreEqual(span2d.Width, 3);
        Assert.AreEqual(span2d.Height, 2);
        Assert.AreEqual(span2d[0, 1], 2);
        Assert.AreEqual(span2d[1, 2], 6);

        _ = new ReadOnlySpan2D<object>(new string[1, 2]);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_Array2DConstructor_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array, 0, 1, 2, 2);

        Assert.IsFalse(span2d.IsEmpty);
        Assert.AreEqual(span2d.Length, 4);
        Assert.AreEqual(span2d.Width, 2);
        Assert.AreEqual(span2d.Height, 2);
        Assert.AreEqual(span2d[0, 0], 2);
        Assert.AreEqual(span2d[1, 1], 6);

        _ = new ReadOnlySpan2D<object>(new string[1, 2]);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<object>(new string[1, 2], 0, 0, 2, 2));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_Array3DConstructor_1()
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

        ReadOnlySpan2D<int> span2d = new(array, 1);

        Assert.IsFalse(span2d.IsEmpty);
        Assert.AreEqual(span2d.Length, 6);
        Assert.AreEqual(span2d.Width, 3);
        Assert.AreEqual(span2d.Height, 2);
        Assert.AreEqual(span2d[0, 0], 10);
        Assert.AreEqual(span2d[0, 1], 20);
        Assert.AreEqual(span2d[1, 2], 60);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, -1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 20));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_Array3DConstructor_2()
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

        ReadOnlySpan2D<int> span2d = new(array, 1, 0, 1, 2, 2);

        Assert.IsFalse(span2d.IsEmpty);
        Assert.AreEqual(span2d.Length, 4);
        Assert.AreEqual(span2d.Width, 2);
        Assert.AreEqual(span2d.Height, 2);
        Assert.AreEqual(span2d[0, 0], 20);
        Assert.AreEqual(span2d[0, 1], 30);
        Assert.AreEqual(span2d[1, 1], 60);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, -1, 1, 1, 1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 1, -1, 1, 1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 1, 1, -1, 1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 1, 1, 1, -1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 1, 1, 1, 1, -1));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_CopyTo_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        int[] target = new int[array.Length];

        span2d.CopyTo(target);

        CollectionAssert.AreEqual(array, target);

        _ = Assert.ThrowsException<ArgumentException>(() => new ReadOnlySpan2D<int>(array).CopyTo(Span<int>.Empty));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_CopyTo_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array, 0, 1, 2, 2);

        int[] target = new int[4];

        span2d.CopyTo(target);

        int[] expected = { 2, 3, 5, 6 };

        CollectionAssert.AreEqual(target, expected);

        _ = Assert.ThrowsException<ArgumentException>(() => new ReadOnlySpan2D<int>(array).CopyTo(Span<int>.Empty));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_CopyTo2D_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        int[,] target = new int[2, 3];

        span2d.CopyTo(target);

        CollectionAssert.AreEqual(array, target);

        _ = Assert.ThrowsException<ArgumentException>(() => new ReadOnlySpan2D<int>(array).CopyTo(Span2D<int>.Empty));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_CopyTo2D_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array, 0, 1, 2, 2);

        int[,] target = new int[2, 2];

        span2d.CopyTo(target);

        int[,] expected =
        {
            { 2, 3 },
            { 5, 6 }
        };

        CollectionAssert.AreEqual(target, expected);

        _ = Assert.ThrowsException<ArgumentException>(() => new ReadOnlySpan2D<int>(array).CopyTo(new Span2D<int>(target)));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_TryCopyTo()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        int[] target = new int[array.Length];

        Assert.IsTrue(span2d.TryCopyTo(target));
        Assert.IsFalse(span2d.TryCopyTo(Span<int>.Empty));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_TryCopyTo2D()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        int[,] target = new int[2, 3];

        Assert.IsTrue(span2d.TryCopyTo(target));
        Assert.IsFalse(span2d.TryCopyTo(Span2D<int>.Empty));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_GetPinnableReference()
    {
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef<int>(null),
            ref Unsafe.AsRef(in ReadOnlySpan2D<int>.Empty.GetPinnableReference())));

        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        ref int r0 = ref Unsafe.AsRef(in span2d.GetPinnableReference());

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref array[0, 0]));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_DangerousGetReference()
    {
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef<int>(null),
            ref ReadOnlySpan2D<int>.Empty.DangerousGetReference()));

        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        ref int r0 = ref span2d.DangerousGetReference();

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref array[0, 0]));
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_Index_Indexer_1()
    {
        int[,] array = new int[4, 4];

        ReadOnlySpan2D<int> span2d = new(array);

        ref int arrayRef = ref array[1, 3];
        ref readonly int span2dRef = ref span2d[1, ^1];

        Assert.IsTrue(Unsafe.AreSame(ref arrayRef, ref Unsafe.AsRef(in span2dRef)));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_Index_Indexer_2()
    {
        int[,] array = new int[4, 4];

        ReadOnlySpan2D<int> span2d = new(array);

        ref int arrayRef = ref array[2, 1];
        ref readonly int span2dRef = ref span2d[^2, ^3];

        Assert.IsTrue(Unsafe.AreSame(ref arrayRef, ref Unsafe.AsRef(in span2dRef)));
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public unsafe void Test_ReadOnlySpan2DT_Index_Indexer_Fail()
    {
        int[,] array = new int[4, 4];

        ReadOnlySpan2D<int> span2d = new(array);

        ref readonly int span2dRef = ref span2d[^6, 2];
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_Range_Indexer_1()
    {
        int[,] array = new int[4, 4];

        ReadOnlySpan2D<int> span2d = new(array);
        ReadOnlySpan2D<int> slice = span2d[1.., 1..];

        Assert.AreEqual(slice.Length, 9);
        Assert.IsTrue(Unsafe.AreSame(ref array[1, 1], ref Unsafe.AsRef(in slice[0, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref array[3, 3], ref Unsafe.AsRef(in slice[2, 2])));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_Range_Indexer_2()
    {
        int[,] array = new int[4, 4];

        ReadOnlySpan2D<int> span2d = new(array);
        ReadOnlySpan2D<int> slice = span2d[0..^2, 1..^1];

        Assert.AreEqual(slice.Length, 4);
        Assert.IsTrue(Unsafe.AreSame(ref array[0, 1], ref Unsafe.AsRef(in slice[0, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref array[1, 2], ref Unsafe.AsRef(in slice[1, 1])));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public unsafe void Test_ReadOnlySpan2DT_Range_Indexer_Fail()
    {
        int[,] array = new int[4, 4];

        ReadOnlySpan2D<int> span2d = new(array);
        _ = span2d[0..6, 2..^1];

        Assert.Fail();
    }
#endif

    [TestMethod]
    public void Test_ReadOnlySpan2DT_Slice_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        ReadOnlySpan2D<int> slice1 = span2d.Slice(1, 1, 1, 2);

        Assert.AreEqual(slice1.Length, 2);
        Assert.AreEqual(slice1.Height, 1);
        Assert.AreEqual(slice1.Width, 2);
        Assert.AreEqual(slice1[0, 0], 5);
        Assert.AreEqual(slice1[0, 1], 6);

        ReadOnlySpan2D<int> slice2 = span2d.Slice(0, 1, 2, 2);

        Assert.AreEqual(slice2.Length, 4);
        Assert.AreEqual(slice2.Height, 2);
        Assert.AreEqual(slice2.Width, 2);
        Assert.AreEqual(slice2[0, 0], 2);
        Assert.AreEqual(slice2[1, 0], 5);
        Assert.AreEqual(slice2[1, 1], 6);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).Slice(-1, 1, 1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).Slice(1, -1, 1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).Slice(1, 1, 1, -1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).Slice(1, 1, -1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).Slice(10, 1, 1, 1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).Slice(1, 12, 1, 12));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).Slice(1, 1, 55, 1));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_Slice_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        ReadOnlySpan2D<int> slice1 = span2d.Slice(0, 0, 2, 2);

        Assert.AreEqual(slice1.Length, 4);
        Assert.AreEqual(slice1.Height, 2);
        Assert.AreEqual(slice1.Width, 2);
        Assert.AreEqual(slice1[0, 0], 1);
        Assert.AreEqual(slice1[1, 1], 5);

        ReadOnlySpan2D<int> slice2 = slice1.Slice(1, 0, 1, 2);

        Assert.AreEqual(slice2.Length, 2);
        Assert.AreEqual(slice2.Height, 1);
        Assert.AreEqual(slice2.Width, 2);
        Assert.AreEqual(slice2[0, 0], 4);
        Assert.AreEqual(slice2[0, 1], 5);

        ReadOnlySpan2D<int> slice3 = slice2.Slice(0, 1, 1, 1);

        Assert.AreEqual(slice3.Length, 1);
        Assert.AreEqual(slice3.Height, 1);
        Assert.AreEqual(slice3.Width, 1);
        Assert.AreEqual(slice3[0, 0], 5);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ReadOnlySpan2DT_GetRowReadOnlySpan()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        ReadOnlySpan<int> span = span2d.GetRowSpan(1);

        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef(span[0]),
            ref array[1, 0]));
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef(span[2]),
            ref array[1, 2]));

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).GetRowSpan(-1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).GetRowSpan(5));
    }
#endif

    [TestMethod]
    public void Test_ReadOnlySpan2DT_TryGetSpan_From1DArray_1()
    {
        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        ReadOnlySpan2D<int> span2d = new(array, 3, 3);

        bool success = span2d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span2d.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[0], ref Unsafe.AsRef(in span[0])));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_TryGetSpan_From1DArray_2()
    {
        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        ReadOnlySpan2D<int> span2d = new ReadOnlySpan2D<int>(array, 3, 3).Slice(1, 0, 2, 3);

        bool success = span2d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span2d.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[3], ref Unsafe.AsRef(in span[0])));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_TryGetSpan_From1DArray_3()
    {
        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        ReadOnlySpan2D<int> span2d = new ReadOnlySpan2D<int>(array, 3, 3).Slice(0, 1, 3, 2);

        bool success = span2d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsFalse(success);
        Assert.AreEqual(span.Length, 0);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_TryGetReadOnlySpan_From2DArray_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        bool success = span2d.TryGetSpan(out ReadOnlySpan<int> span);

#if NETFRAMEWORK
        // Can't get a ReadOnlySpan<T> over a T[,] array on .NET Standard 2.0
        Assert.IsFalse(success);
        Assert.AreEqual(span.Length, 0);
#else
        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span2d.Length);
#endif
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_TryGetReadOnlySpan_From2DArray_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array, 0, 0, 2, 2);

        bool success = span2d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsFalse(success);
        Assert.IsTrue(span.IsEmpty);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_ToArray_1()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        int[,] copy = span2d.ToArray();

        Assert.AreEqual(copy.GetLength(0), array.GetLength(0));
        Assert.AreEqual(copy.GetLength(1), array.GetLength(1));

        CollectionAssert.AreEqual(array, copy);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_ToArray_2()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array, 0, 0, 2, 2);

        int[,] copy = span2d.ToArray();

        Assert.AreEqual(copy.GetLength(0), 2);
        Assert.AreEqual(copy.GetLength(1), 2);

        int[,] expected =
        {
            { 1, 2 },
            { 4, 5 }
        };

        CollectionAssert.AreEqual(expected, copy);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void Test_ReadOnlySpan2DT_Equals()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        _ = span2d.Equals(null);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void Test_ReadOnlySpan2DT_GetHashCode()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        _ = span2d.GetHashCode();
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_ToString()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d = new(array);

        string text = span2d.ToString();

        const string expected = "CommunityToolkit.HighPerformance.ReadOnlySpan2D<System.Int32>[2, 3]";

        Assert.AreEqual(text, expected);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_opEquals()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d_1 = new(array);
        ReadOnlySpan2D<int> span2d_2 = new(array);

        Assert.IsTrue(span2d_1 == span2d_2);
        Assert.IsFalse(span2d_1 == ReadOnlySpan2D<int>.Empty);
        Assert.IsTrue(ReadOnlySpan2D<int>.Empty == ReadOnlySpan2D<int>.Empty);

        ReadOnlySpan2D<int> span2d_3 = new(array, 0, 0, 2, 2);

        Assert.IsFalse(span2d_1 == span2d_3);
        Assert.IsFalse(span2d_3 == ReadOnlySpan2D<int>.Empty);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_ImplicitCast()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        ReadOnlySpan2D<int> span2d_1 = array;
        ReadOnlySpan2D<int> span2d_2 = new(array);

        Assert.IsTrue(span2d_1 == span2d_2);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_GetRow()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        int i = 0;
        foreach (ref readonly int value in new ReadOnlySpan2D<int>(array).GetRow(1))
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(value), ref array[1, i++]));
        }

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan2D<int>(array).GetRow(1);

        int[] expected = { 4, 5, 6 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        Assert.AreSame(Array.Empty<int>(), default(ReadOnlyRefEnumerable<int>).ToArray());

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).GetRow(-1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).GetRow(2));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).GetRow(1000));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_Pointer_GetRow()
    {
        int* array = stackalloc[]
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        int i = 0;
        foreach (ref readonly int value in new ReadOnlySpan2D<int>(array, 2, 3, 0).GetRow(1))
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(value), ref array[3 + i++]));
        }

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan2D<int>(array, 2, 3, 0).GetRow(1);

        int[] expected = { 4, 5, 6 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 2, 3, 0).GetRow(-1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 2, 3, 0).GetRow(2));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 2, 3, 0).GetRow(1000));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_GetColumn()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        int i = 0;
        foreach (ref readonly int value in new ReadOnlySpan2D<int>(array).GetColumn(1))
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(value), ref array[i++, 1]));
        }

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan2D<int>(array).GetColumn(2);

        int[] expected = { 3, 6 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).GetColumn(-1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).GetColumn(3));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array).GetColumn(1000));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_Pointer_GetColumn()
    {
        int* array = stackalloc[]
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        int i = 0;
        foreach (ref readonly int value in new ReadOnlySpan2D<int>(array, 2, 3, 0).GetColumn(1))
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(value), ref array[(i++ * 3) + 1]));
        }

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan2D<int>(array, 2, 3, 0).GetColumn(2);

        int[] expected = { 3, 6 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 2, 3, 0).GetColumn(-1));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 2, 3, 0).GetColumn(3));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ReadOnlySpan2D<int>(array, 2, 3, 0).GetColumn(1000));
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_GetEnumerator()
    {
        int[,] array =
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        int[] result = new int[4];
        int i = 0;

        foreach (int item in new ReadOnlySpan2D<int>(array, 0, 1, 2, 2))
        {
            result[i++] = item;
        }

        int[] expected = { 2, 3, 5, 6 };

        CollectionAssert.AreEqual(result, expected);
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan2DT_Pointer_GetEnumerator()
    {
        int* array = stackalloc[]
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        int[] result = new int[4];
        int i = 0;

        foreach (int item in new ReadOnlySpan2D<int>(array + 1, 2, 2, 1))
        {
            result[i++] = item;
        }

        int[] expected = { 2, 3, 5, 6 };

        CollectionAssert.AreEqual(result, expected);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_GetEnumerator_Empty()
    {
        ReadOnlySpan2D<int>.Enumerator enumerator = ReadOnlySpan2D<int>.Empty.GetEnumerator();

        Assert.IsFalse(enumerator.MoveNext());
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_ReadOnlyRefEnumerable_Misc()
    {
        int[,] array1 =
        {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 },
            { 13, 14, 15, 16 }
        };

        ReadOnlySpan2D<int> span1 = array1;

        int[,] array2 = new int[4, 4];

        // Copy to enumerable with source step == 1, destination step == 1
        span1.GetRow(0).CopyTo(array2.GetRow(0));

        // Copy enumerable with source step == 1, destination step != 1
        span1.GetRow(1).CopyTo(array2.GetColumn(1));

        // Copy enumerable with source step != 1, destination step == 1
        span1.GetColumn(2).CopyTo(array2.GetRow(2));

        // Copy enumerable with source step != 1, destination step != 1
        span1.GetColumn(3).CopyTo(array2.GetColumn(3));

        int[,] result =
        {
            { 1, 5, 3, 4 },
            { 0, 6, 0, 8 },
            { 3, 7, 11, 12 },
            { 0, 8, 0, 16 }
        };

        CollectionAssert.AreEqual(array2, result);

        // Test a valid and an invalid TryCopyTo call with the RefEnumerable<T> overload
        bool shouldBeTrue = span1.GetRow(0).TryCopyTo(array2.GetColumn(0));
        bool shouldBeFalse = span1.GetRow(0).TryCopyTo(default(RefEnumerable<int>));

        result = new[,]
        {
            { 1, 5, 3, 4 },
            { 2, 6, 0, 8 },
            { 3, 7, 11, 12 },
            { 4, 8, 0, 16 }
        };

        CollectionAssert.AreEqual(array2, result);

        Assert.IsTrue(shouldBeTrue);
        Assert.IsFalse(shouldBeFalse);
    }

    [TestMethod]
    public void Test_ReadOnlySpan2DT_ReadOnlyRefEnumerable_Cast()
    {
        int[,] array1 =
        {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 },
            { 13, 14, 15, 16 }
        };

        int[] result = { 5, 6, 7, 8 };

        // Cast a RefEnumerable<T> to a readonly one and verify the contents
        int[] row = ((ReadOnlyRefEnumerable<int>)array1.GetRow(1)).ToArray();

        CollectionAssert.AreEqual(result, row);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

/* ====================================================================
 *                                 NOTE
 * ====================================================================
 * All the tests here mirror the ones for Span3D<T>. See comments
 * in the test file for Span3D<T> for more info on these tests. */
[TestClass]
public class Test_ReadOnlySpan3DT
{
    [TestMethod]
    public void Test_ReadOnlySpan3DT_Empty()
    {
        ReadOnlySpan3D<int> empty1 = default;

        Assert.IsTrue(empty1.IsEmpty);
        Assert.AreEqual(0, empty1.Length);
        Assert.AreEqual(0, empty1.Depth);
        Assert.AreEqual(0, empty1.Height);
        Assert.AreEqual(0, empty1.Width);

        ReadOnlySpan3D<int> empty2 = ReadOnlySpan3D<int>.Empty;

        Assert.IsTrue(empty2.IsEmpty);
        Assert.AreEqual(0, empty2.Length);
        Assert.AreEqual(0, empty2.Depth);
        Assert.AreEqual(0, empty2.Height);
        Assert.AreEqual(0, empty2.Width);

        ReadOnlySpan3D<int> empty3 = new int[0, 2, 3];

        Assert.IsTrue(empty3.IsEmpty);
        Assert.AreEqual(0, empty3.Length);
        Assert.AreEqual(0, empty3.Depth);
        Assert.AreEqual(2, empty3.Height);
        Assert.AreEqual(3, empty3.Width);

        ReadOnlySpan3D<int> empty4 = new int[2, 0, 3];

        Assert.IsTrue(empty4.IsEmpty);
        Assert.AreEqual(0, empty4.Length);
        Assert.AreEqual(2, empty4.Depth);
        Assert.AreEqual(0, empty4.Height);
        Assert.AreEqual(3, empty4.Width);

        ReadOnlySpan3D<int> empty5 = new int[2, 3, 0];

        Assert.IsTrue(empty5.IsEmpty);
        Assert.AreEqual(0, empty5.Length);
        Assert.AreEqual(2, empty5.Depth);
        Assert.AreEqual(3, empty5.Height);
        Assert.AreEqual(0, empty5.Width);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_RefConstructor()
    {
        ReadOnlySpan<int> span = stackalloc[]
        {
            1, 2, 3, 4, 5, 6,
            7, 8, 9, 10, 11, 12
        };

        ReadOnlySpan3D<int> span3d = ReadOnlySpan3D<int>.DangerousCreate(span[0], 2, 2, 3, 0, 0);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(12, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(3, span3d.Width);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ReadOnlySpan3D<int>.DangerousCreate(Unsafe.AsRef<int>(null), -1, 0, 0, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ReadOnlySpan3D<int>.DangerousCreate(Unsafe.AsRef<int>(null), 1, -2, 0, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ReadOnlySpan3D<int>.DangerousCreate(Unsafe.AsRef<int>(null), 1, 0, -5, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ReadOnlySpan3D<int>.DangerousCreate(Unsafe.AsRef<int>(null), 1, 0, 0, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ReadOnlySpan3D<int>.DangerousCreate(Unsafe.AsRef<int>(null), 1, 0, 0, 0, -1));
    }
#endif

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_PtrConstructor()
    {
        int* ptr = stackalloc[]
        {
            1, 2, 3, 4, 5, 6,
            7, 8, 9, 10, 11, 12
        };

        ReadOnlySpan3D<int> span3d = new(ptr, 2, 2, 3, 0, 0);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(12, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(3, span3d.Width);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>((void*)0, -1, 0, 0, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>((void*)0, 1, -2, 0, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>((void*)0, 1, 0, -5, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>((void*)0, 1, 0, 0, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>((void*)0, 1, 0, 0, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlySpan3D<string>((void*)0, 1, 1, 1, 0, 0));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_Array1DConstructor()
    {
        int[] array = Enumerable.Range(1, 20).ToArray();

        ReadOnlySpan3D<int> span3d = new(array, 1, 2, 2, 2, 2, 1);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(8, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(2, span3d.Width);

        // Same for ReadOnlyMemory3D<T>, we need to check that covariant array conversions are allowed
        _ = new ReadOnlySpan3D<object>(new string[1], 1, 1, 1);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, -1, 1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, -1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, 1, -1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, 1, 1, -1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, 1, 1, 1, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, 1, 1, 1, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlySpan3D<int>(array, 0, 4, 4, 4, 0, 0));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_Array3DConstructor_1()
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

        ReadOnlySpan3D<int> span3d = new(array);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(12, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(3, span3d.Width);

        _ = new ReadOnlySpan2D<object>(new string[1, 2]);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_Array3DConstructor_2()
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

        ReadOnlySpan3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(8, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(2, span3d.Width);

        _ = new ReadOnlySpan3D<object>(new string[1, 2, 1]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, -1, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, -1, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, 0, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, 0, 0, 3, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, 0, 0, 1, 3, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 0, 0, 0, 1, 1, 5));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_CopyTo_Empty()
    {
        ReadOnlySpan3D<int> span3d = ReadOnlySpan3D<int>.Empty;

        int[] target = new int[0];

        // Copying an empty ReadOnlySpan3D<T> to an empty array is just a no-op
        span3d.CopyTo(target);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_CopyTo_1()
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

        ReadOnlySpan3D<int> span3d = new(array);

        int[] target = new int[array.Length];

        span3d.CopyTo(target);

        int[] expected = Enumerable.Range(1, 12).ToArray();

        CollectionAssert.AreEqual(expected, target);

        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlySpan3D<int>(array).CopyTo(Span<int>.Empty));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_CopyTo_2()
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

        ReadOnlySpan3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

        int[] target = new int[8];

        span3d.CopyTo(target);

        int[] expected = { 2, 3, 5, 6, 8, 9, 11, 12 };

        CollectionAssert.AreEqual(target, expected);

        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlySpan3D<int>(array).CopyTo(Span<int>.Empty));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlySpan3D<int>(array, 0, 0, 1, 2, 2, 2).CopyTo(Span<int>.Empty));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_CopyTo3D_1()
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

        ReadOnlySpan3D<int> span3d = new(array);

        int[,,] target = new int[2, 2, 3];

        span3d.CopyTo(target);

        CollectionAssert.AreEqual(array, target);

        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlySpan3D<int>(array).CopyTo(Span3D<int>.Empty));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_CopyTo3D_2()
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

        ReadOnlySpan3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

        int[,,] target = new int[2, 2, 2];

        span3d.CopyTo(target);

        int[,,] expected =
        {
            {
                { 2, 3 },
                { 5, 6 }
            },
            {
                { 8, 9 },
                { 11, 12 }
            }
        };

        CollectionAssert.AreEqual(target, expected);

        _ = Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlySpan3D<int>(array).CopyTo(new Span3D<int>(target)));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_TryCopyTo()
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

        ReadOnlySpan3D<int> span3d = new(array);

        int[] target = new int[array.Length];

        Assert.IsTrue(span3d.TryCopyTo(target));
        Assert.IsFalse(span3d.TryCopyTo(Span<int>.Empty));

        int[] expected = Enumerable.Range(1, 12).ToArray();

        CollectionAssert.AreEqual(target, expected);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_TryCopyTo3D()
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

        ReadOnlySpan3D<int> span3d = new(array);

        int[,,] target = new int[2, 2, 3];

        Assert.IsTrue(span3d.TryCopyTo(target));
        Assert.IsFalse(span3d.TryCopyTo(Span3D<int>.Empty));

        CollectionAssert.AreEqual(target, array);
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_GetPinnableReference()
    {
        // Here we test that a ref from an empty ReadOnlySpan3D<T> returns a null ref
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef<int>(null),
            ref Unsafe.AsRef(in ReadOnlySpan3D<int>.Empty.GetPinnableReference())));

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

        ReadOnlySpan3D<int> span3d = new(array);

        ref int r0 = ref Unsafe.AsRef(in span3d.GetPinnableReference());

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref array[0, 0, 0]));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_DangerousGetReference()
    {
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef<int>(null),
            ref ReadOnlySpan3D<int>.Empty.DangerousGetReference()));

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

        ReadOnlySpan3D<int> span3d = new(array);

        ref int r0 = ref span3d.DangerousGetReference();

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref array[0, 0, 0]));
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Index_Indexer_1()
    {
        int[,,] array = new int[4, 4, 4];

        ReadOnlySpan3D<int> span3d = new(array);

        ref int arrayRef = ref array[1, 2, 3];
        ref readonly int span3dRef = ref span3d[1, 2, ^1];

        Assert.IsTrue(Unsafe.AreSame(ref arrayRef, ref Unsafe.AsRef(in span3dRef)));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Index_Indexer_2()
    {
        int[,,] array = new int[4, 4, 4];

        ReadOnlySpan3D<int> span3d = new(array);

        ref int arrayRef = ref array[2, 1, 0];
        ref readonly int span3dRef = ref span3d[^2, ^3, ^4];

        Assert.IsTrue(Unsafe.AreSame(ref arrayRef, ref Unsafe.AsRef(in span3dRef)));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Index_Indexer_Fail()
    {
        int[,,] array = new int[4, 4, 4];

        _ = Assert.ThrowsExactly<IndexOutOfRangeException>(() =>
        {
            ReadOnlySpan3D<int> span3d = new(array);

            ref readonly int span3dRef = ref span3d[^6, 2, 1];
        });
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Range_Indexer_1()
    {
        int[,,] array = new int[4, 4, 4];

        ReadOnlySpan3D<int> span3d = new(array);
        ReadOnlySpan3D<int> slice = span3d[1.., 1.., 1..];

        Assert.AreEqual(27, slice.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[1, 1, 1], ref Unsafe.AsRef(in slice[0, 0, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref array[3, 3, 3], ref Unsafe.AsRef(in slice[2, 2, 2])));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Range_Indexer_2()
    {
        int[,,] array = new int[4, 4, 4];

        ReadOnlySpan3D<int> span3d = new(array);
        ReadOnlySpan3D<int> slice = span3d[0..^2, 1..^1, 0..^2];

        Assert.AreEqual(8, slice.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[0, 1, 0], ref Unsafe.AsRef(in slice[0, 0, 0])));
        Assert.IsTrue(Unsafe.AreSame(ref array[1, 2, 1], ref Unsafe.AsRef(in slice[1, 1, 1])));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Range_Indexer_Fail()
    {
        int[,,] array = new int[4, 4, 4];

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            ReadOnlySpan3D<int> span3d = new(array);

            _ = span3d[0..6, 2..^1, 0..2];
        });
    }
#endif

    [TestMethod]
    public void Test_ReadOnlySpan3DT_Slice_1()
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

        ReadOnlySpan3D<int> span3d = new(array);

        ReadOnlySpan3D<int> slice1 = span3d.Slice(1, 0, 1, 1, 2, 2);

        Assert.AreEqual(4, slice1.Length);
        Assert.AreEqual(1, slice1.Depth);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(8, slice1[0, 0, 0]);
        Assert.AreEqual(12, slice1[0, 1, 1]);

        ReadOnlySpan3D<int> slice2 = span3d.Slice(0, 1, 0, 2, 1, 3);

        Assert.AreEqual(6, slice2.Length);
        Assert.AreEqual(2, slice2.Depth);
        Assert.AreEqual(1, slice2.Height);
        Assert.AreEqual(3, slice2.Width);
        Assert.AreEqual(4, slice2[0, 0, 0]);
        Assert.AreEqual(10, slice2[1, 0, 0]);
        Assert.AreEqual(12, slice2[1, 0, 2]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(-1, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(0, -1, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(0, 0, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(0, 0, 0, 0, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(0, 0, 0, 1, 0, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(0, 0, 0, 1, 1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(10, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(0, 10, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).Slice(0, 0, 10, 1, 1, 1));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_Slice_2()
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

        ReadOnlySpan3D<int> span3d = new(array);

        ReadOnlySpan3D<int> slice1 = span3d.Slice(0, 0, 0, 2, 2, 2);

        Assert.AreEqual(8, slice1.Length);
        Assert.AreEqual(2, slice1.Depth);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(1, slice1[0, 0, 0]);
        Assert.AreEqual(11, slice1[1, 1, 1]);

        ReadOnlySpan3D<int> slice2 = slice1.Slice(1, 0, 1, 1, 2, 1);

        Assert.AreEqual(2, slice2.Length);
        Assert.AreEqual(1, slice2.Depth);
        Assert.AreEqual(2, slice2.Height);
        Assert.AreEqual(1, slice2.Width);
        Assert.AreEqual(8, slice2[0, 0, 0]);
        Assert.AreEqual(11, slice2[0, 1, 0]);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ReadOnlySpan3DT_GetRowSpan()
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

        ReadOnlySpan3D<int> span3d = new(array);

        ReadOnlySpan<int> span = span3d.GetRowSpan(1, 0);

        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef(in span[0]),
            ref array[1, 0, 0]));
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef(in span[2]),
            ref array[1, 0, 2]));

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetRowSpan(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetRowSpan(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetRowSpan(5, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetRowSpan(0, 5));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_GetSliceSpan()
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

        ReadOnlySpan3D<int> span3d = new(array);

        ReadOnlySpan2D<int> slice = span3d.GetSliceSpan(1);

        Assert.AreEqual(2, slice.Height);
        Assert.AreEqual(3, slice.Width);
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef(in slice[0, 0]),
            ref array[1, 0, 0]));
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef(in slice[1, 2]),
            ref array[1, 1, 2]));

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetSliceSpan(-1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetSliceSpan(5));
    }
#endif

    [TestMethod]
    public void Test_ReadOnlySpan3DT_TryGetSpan_From1DArray_1()
    {
        int[] array = Enumerable.Range(1, 24).ToArray();

        ReadOnlySpan3D<int> span3d = new(array, 2, 3, 4);

        bool success = span3d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span3d.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[0], ref Unsafe.AsRef(in span[0])));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_TryGetSpan_From1DArray_2()
    {
        int[] array = Enumerable.Range(1, 24).ToArray();

        ReadOnlySpan3D<int> span3d = new ReadOnlySpan3D<int>(array, 2, 3, 4).Slice(1, 0, 0, 1, 3, 4);

        bool success = span3d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span3d.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[12], ref Unsafe.AsRef(in span[0])));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_TryGetSpan_From1DArray_3()
    {
        int[] array = Enumerable.Range(1, 24).ToArray();

        ReadOnlySpan3D<int> span3d = new ReadOnlySpan3D<int>(array, 2, 3, 4).Slice(0, 1, 1, 2, 2, 2);

        bool success = span3d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsFalse(success);
        Assert.AreEqual(0, span.Length);
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3947
    [TestMethod]
    public void Test_ReadOnlySpan3DT_TryGetSpan_From1DArray_4()
    {
        int[] array = new int[128];
        ReadOnlySpan3D<int> span3d = new(array, 2, 4, 16);

        bool success = span3d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span3d.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[0], ref Unsafe.AsRef(in span[0])));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_TryGetSpan_From3DArray_1()
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

        ReadOnlySpan3D<int> span3d = new(array);

        bool success = span3d.TryGetSpan(out ReadOnlySpan<int> span);

#if NETFRAMEWORK
        // Can't get a ReadOnlySpan<T> over a T[,,] array on .NET Standard 2.0
        Assert.IsFalse(success);
        Assert.AreEqual(0, span.Length);
#else
        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span3d.Length);
#endif
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_TryGetSpan_From3DArray_2()
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

        ReadOnlySpan3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

        bool success = span3d.TryGetSpan(out ReadOnlySpan<int> span);

        Assert.IsFalse(success);
        Assert.IsTrue(span.IsEmpty);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_ToArray_1()
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

        ReadOnlySpan3D<int> span3d = new(array);

        int[,,] copy = span3d.ToArray();

        Assert.AreEqual(copy.GetLength(0), array.GetLength(0));
        Assert.AreEqual(copy.GetLength(1), array.GetLength(1));
        Assert.AreEqual(copy.GetLength(2), array.GetLength(2));

        CollectionAssert.AreEqual(array, copy);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_ToArray_2()
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

        ReadOnlySpan3D<int> span3d = new(array, 0, 0, 0, 1, 2, 2);

        int[,,] copy = span3d.ToArray();

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
    public void Test_ReadOnlySpan3DT_Equals()
    {
        int[,,] array = new int[1, 1, 1];

        _ = Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            ReadOnlySpan3D<int> span3d = new(array);

            _ = span3d.Equals(null);
        });
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_GetHashCode()
    {
        int[,,] array = new int[1, 1, 1];

        _ = Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            ReadOnlySpan3D<int> span3d = new(array);

            _ = span3d.GetHashCode();
        });
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_ToString()
    {
        int[,,] array = new int[2, 2, 3];

        ReadOnlySpan3D<int> span3d = new(array);

        string text = span3d.ToString();

        const string expected = "CommunityToolkit.HighPerformance.ReadOnlySpan3D<System.Int32>[2, 2, 3]";

        Assert.AreEqual(expected, text);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_opEquals()
    {
        int[,,] array = new int[2, 2, 3];

        ReadOnlySpan3D<int> span3d_1 = new(array);
        ReadOnlySpan3D<int> span3d_2 = new(array);

        Assert.IsTrue(span3d_1 == span3d_2);
        Assert.IsFalse(span3d_1 == ReadOnlySpan3D<int>.Empty);
        Assert.IsTrue(ReadOnlySpan3D<int>.Empty == ReadOnlySpan3D<int>.Empty);

        ReadOnlySpan3D<int> span3d_3 = new(array, 0, 0, 0, 1, 2, 3);

        Assert.IsFalse(span3d_1 == span3d_3);
        Assert.IsFalse(span3d_3 == ReadOnlySpan3D<int>.Empty);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_ImplicitCast()
    {
        int[,,] array = new int[2, 2, 3];

        ReadOnlySpan3D<int> span3d_1 = array;
        ReadOnlySpan3D<int> span3d_2 = new(array);

        Assert.IsTrue(span3d_1 == span3d_2);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_GetRow()
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

        int i = 0;
        foreach (ref readonly int value in new ReadOnlySpan3D<int>(array).GetRow(1, 0))
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in value), ref array[1, 0, i++]));
        }

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan3D<int>(array).GetRow(1, 0);

        int[] expected = { 7, 8, 9 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetRow(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetRow(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetRow(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetRow(0, 2));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Pointer_GetRow()
    {
        int* array = stackalloc[]
        {
            1, 2, 3,
            4, 5, 6,
            7, 8, 9,
            10, 11, 12
        };

        int i = 0;
        foreach (ref readonly int value in new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetRow(1, 1))
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in value), ref array[9 + i++]));
        }

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetRow(1, 1);

        int[] expected = { 10, 11, 12 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetRow(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetRow(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetRow(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetRow(0, 2));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_GetColumn()
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

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan3D<int>(array).GetColumn(0, 2);

        int[] expected = { 3, 6 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetColumn(0, 3));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Pointer_GetColumn()
    {
        int* array = stackalloc[]
        {
            1, 2, 3,
            4, 5, 6,
            7, 8, 9,
            10, 11, 12
        };

        int i = 0;
        foreach (ref readonly int value in new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(0, 1))
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in value), ref array[(i++ * 3) + 1]));
        }

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(1, 2);

        int[] expected = { 9, 12 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(0, 3));
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_GetDepthColumn()
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

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan3D<int>(array).GetDepthColumn(1, 1);

        int[] expected = { 5, 11 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetDepthColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetDepthColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetDepthColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array).GetDepthColumn(0, 3));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Pointer_GetDepthColumn()
    {
        int* array = stackalloc[]
        {
            1, 2, 3,
            4, 5, 6,
            7, 8, 9,
            10, 11, 12
        };

        int i = 0;
        foreach (ref readonly int value in new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetDepthColumn(1, 1))
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in value), ref array[(i++ * (2 * 3)) + (1 * 3) + 1]));
        }

        ReadOnlyRefEnumerable<int> enumerable = new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetDepthColumn(1, 2);

        int[] expected = { 6, 12 };

        CollectionAssert.AreEqual(expected, enumerable.ToArray());

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ReadOnlySpan3D<int>(array, 2, 2, 3, 0, 0).GetColumn(0, 3));
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_GetEnumerator()
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

        int[] result = new int[4];
        int i = 0;

        foreach (int item in new ReadOnlySpan3D<int>(array, 0, 1, 1, 2, 1, 2))
        {
            result[i++] = item;
        }

        int[] expected = { 5, 6, 11, 12 };

        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public unsafe void Test_ReadOnlySpan3DT_Pointer_GetEnumerator()
    {
        int* array = stackalloc[]
        {
            1, 2, 3,
            4, 5, 6,
            7, 8, 9,
            10, 11, 12
        };

        int[] result = new int[4];
        int i = 0;

        foreach (int item in new ReadOnlySpan3D<int>(array + 4, 2, 1, 2, 1, 3))
        {
            result[i++] = item;
        }

        int[] expected = { 5, 6, 11, 12 };

        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_GetEnumerator_Empty()
    {
        ReadOnlySpan3D<int>.Enumerator enumerator = ReadOnlySpan3D<int>.Empty.GetEnumerator();

        Assert.IsFalse(enumerator.MoveNext());
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_ReadOnlyRefEnumerable_Misc()
    {
        int[,,] array1 =
        {
            { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 16 } },
            { { 17, 18, 19, 20 }, { 21, 22, 23, 24 }, { 25, 26, 27, 28 }, { 29, 30, 31, 32 } }
        };

        ReadOnlySpan3D<int> span1 = array1;

        int[,,] array2 = new int[2, 4, 4];

        // Copy to enumerable with source step == 1, destination step == 1
        span1.GetRow(0, 0).CopyTo(array2.GetRow(0, 0));

        // Copy enumerable with source step == 1, destination step != 1
        span1.GetRow(0, 1).CopyTo(array2.GetColumn(0, 1));

        // Copy enumerable with source step != 1, destination step == 1
        span1.GetColumn(0, 2).CopyTo(array2.GetRow(0, 2));

        // Copy enumerable with source step != 1, destination step != 1
        span1.GetColumn(0, 3).CopyTo(array2.GetColumn(0, 3));

        // Copy from second slice to first slice in array2
        span1.GetRow(1, 0).CopyTo(array2.GetRow(1, 0));
        span1.GetColumn(1, 1).CopyTo(array2.GetColumn(1, 2));

        // Copy depth column at position (1, 0) to row at depth 0, row 1
        span1.GetDepthColumn(1, 0).CopyTo(array2.GetRow(0, 1));

        // Copy depth column at position (2, 1) to column at depth 1, column 1
        span1.GetDepthColumn(2, 1).CopyTo(array2.GetColumn(1, 1));

        int[,,] result =
        {
            { { 1, 5, 3, 4 }, { 5, 21, 0, 8 }, { 3, 7, 11, 12 }, { 0, 8, 0, 16 } },
            { { 17, 10, 18, 20 }, { 0, 26, 22, 0 }, { 0, 0, 26, 0 }, { 0, 0, 30, 0 } }
        };

        CollectionAssert.AreEqual(array2, result);

        // Test a valid and an invalid TryCopyTo call with the RefEnumerable<T> overload
        bool shouldBeTrue = span1.GetRow(0, 0).TryCopyTo(array2.GetColumn(0, 0));
        bool shouldBeFalse = span1.GetRow(0, 0).TryCopyTo(default(RefEnumerable<int>));

        result = new[,,]
        {
            { { 1, 5, 3, 4 }, { 2, 21, 0, 8 }, { 3, 7, 11, 12 }, { 4, 8, 0, 16 } },
            { { 17, 10, 18, 20 }, { 0, 26, 22, 0 }, { 0, 0, 26, 0 }, { 0, 0, 30, 0 } }
        };

        CollectionAssert.AreEqual(array2, result);

        Assert.IsTrue(shouldBeTrue);
        Assert.IsFalse(shouldBeFalse);
    }

    [TestMethod]
    public void Test_ReadOnlySpan3DT_ReadOnlyRefEnumerable_Cast()
    {
        int[,,] array1 =
        {
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 }
            },
            {
                { 17, 18, 19, 20 },
                { 21, 22, 23, 24 },
                { 25, 26, 27, 28 },
                { 29, 30, 31, 32 }
            }
        };

        int[] result = { 5, 6, 7, 8 };

        // Cast a RefEnumerable<T> to a readonly one and verify the contents
        int[] row = ((ReadOnlyRefEnumerable<int>)array1.GetRow(0, 1)).ToArray();

        CollectionAssert.AreEqual(result, row);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ReadOnlySpan3DT_FromMemoryManager_Indexing()
    {
        const int w = 10;
        const int h = 10;
        const int d = 5;
        const int l = w * h * d;

        byte[] b = new byte[l];
        short[] s = new short[l];

        for (int i = 0; i < l; ++i)
        {
            b[i] = (byte)(i % 256);
            s[i] = (short)i;
        }

        Memory3DTester<byte> byteTester = new(d, w, h, b);
        Span3D<byte> byteSpan3DFromArray = byteTester.GetMemory3DFromArray().Span;

        Assert.AreEqual(11, byteSpan3DFromArray[0, 0, 0]);

        Span3D<byte> byteSpan3DFromMemoryManager = byteTester.GetMemory3DFromMemoryManager().Span;

        Assert.AreEqual(11, byteSpan3DFromMemoryManager[0, 0, 0]);

        Memory3DTester<short> shortTester = new(d, w, h, s);
        Span3D<short> shortSpan3DFromArray = shortTester.GetMemory3DFromArray().Span;
        Span3D<short> shortSpan3DFromMemoryManager = shortTester.GetMemory3DFromMemoryManager().Span;

        Assert.AreEqual(11, shortSpan3DFromArray[0, 0, 0]);
        Assert.AreEqual(11, shortSpan3DFromMemoryManager[0, 0, 0]);
    }
#endif
}

#if NET6_0_OR_GREATER
public sealed class Memory3DTester<T> : MemoryManager<T>
    where T : unmanaged
{
    private readonly T[] data;

    public Memory3DTester(int d, int w, int h, T[] data)
    {
        if (d < 2 || w < 2 || h < 2)
        {
            throw new ArgumentException("The 'd', 'w', and 'h' arguments must be at least 2.");
        }

        this.data = data;

        Width = w;
        Height = h;
        Depth = d;
    }

    public int Width { get; }

    public int Height { get; }

    public int Depth { get; }

    public Memory3D<T> GetMemory3DFromMemoryManager()
    {
        return new(this, Width + 1, Depth - 1, Height - 1, Width - 1, 1, 1);
    }

    public Memory3D<T> GetMemory3DFromArray()
    {
        return new(this.data, Width + 1, Depth - 1, Height - 1, Width - 1, 1, 1);
    }

    /// <inheritdoc/>
    public override Span<T> GetSpan()
    {
        return new(this.data);
    }

    /// <inheritdoc/>
    public override MemoryHandle Pin(int elementIndex = 0)
    {
        return default;
    }

    /// <inheritdoc/>
    public override void Unpin()
    {
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
    }
}
#endif
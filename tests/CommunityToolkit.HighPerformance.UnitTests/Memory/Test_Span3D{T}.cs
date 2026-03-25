// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_Span3DT
{
    [TestMethod]
    public void Test_Span3DT_Empty()
    {
        // Like in the tests for Memory3D<T>, here we validate a number of empty spans
        Span3D<int> empty1 = default;

        Assert.IsTrue(empty1.IsEmpty);
        Assert.AreEqual(0, empty1.Length);
        Assert.AreEqual(0, empty1.Depth);
        Assert.AreEqual(0, empty1.Height);
        Assert.AreEqual(0, empty1.Width);

        Span3D<int> empty2 = Span3D<int>.Empty;

        Assert.IsTrue(empty2.IsEmpty);
        Assert.AreEqual(0, empty2.Length);
        Assert.AreEqual(0, empty2.Depth);
        Assert.AreEqual(0, empty2.Height);
        Assert.AreEqual(0, empty2.Width);

        Span3D<int> empty3 = new int[0, 2, 3];

        Assert.IsTrue(empty3.IsEmpty);
        Assert.AreEqual(0, empty3.Length);
        Assert.AreEqual(0, empty3.Depth);
        Assert.AreEqual(2, empty3.Height);
        Assert.AreEqual(3, empty3.Width);

        Span3D<int> empty4 = new int[2, 0, 3];

        Assert.IsTrue(empty4.IsEmpty);
        Assert.AreEqual(0, empty4.Length);
        Assert.AreEqual(2, empty4.Depth);
        Assert.AreEqual(0, empty4.Height);
        Assert.AreEqual(3, empty4.Width);

        Span3D<int> empty5 = new int[2, 3, 0];

        Assert.IsTrue(empty5.IsEmpty);
        Assert.AreEqual(0, empty5.Length);
        Assert.AreEqual(2, empty5.Depth);
        Assert.AreEqual(3, empty5.Height);
        Assert.AreEqual(0, empty5.Width);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public unsafe void Test_Span3DT_RefConstructor()
    {
        Span<int> span = stackalloc[]
        {
            1, 2, 3, 4, 5, 6,
            7, 8, 9, 10, 11, 12
        };

        // Test for a Span3D<T> instance created from a target reference. This is only supported
        // on runtimes with fast Span<T> support (as we need the API to power this with just a ref).
        Span3D<int> span3d = Span3D<int>.DangerousCreate(ref span[0], 2, 2, 3, 0, 0);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(12, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(3, span3d.Width);

        span3d[0, 0, 0] = 99;
        span3d[1, 1, 2] = 101;

        // Validate that those values were mapped to the right spot in the target span
        Assert.AreEqual(99, span[0]);
        Assert.AreEqual(101, span[11]);

        // A few cases with invalid indices
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Span3D<int>.DangerousCreate(ref Unsafe.AsRef<int>(null), -1, 0, 0, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Span3D<int>.DangerousCreate(ref Unsafe.AsRef<int>(null), 1, -2, 0, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Span3D<int>.DangerousCreate(ref Unsafe.AsRef<int>(null), 1, 0, -5, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Span3D<int>.DangerousCreate(ref Unsafe.AsRef<int>(null), 1, 0, 0, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Span3D<int>.DangerousCreate(ref Unsafe.AsRef<int>(null), 1, 0, 0, 0, -1));
    }
#endif

    [TestMethod]
    public unsafe void Test_Span3DT_PtrConstructor()
    {
        int* ptr = stackalloc[]
        {
            1, 2, 3, 4, 5, 6,
            7, 8, 9, 10, 11, 12
        };

        // Same as above, but creating a Span3D<T> from a raw pointer
        Span3D<int> span3d = new(ptr, 2, 2, 3, 0, 0);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(12, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(3, span3d.Width);

        span3d[0, 0, 0] = 99;
        span3d[1, 1, 2] = 101;

        Assert.AreEqual(99, ptr[0]);
        Assert.AreEqual(101, ptr[11]);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>((void*)0, -1, 0, 0, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>((void*)0, 1, -2, 0, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>((void*)0, 1, 0, -5, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>((void*)0, 1, 0, 0, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>((void*)0, 1, 0, 0, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new Span3D<string>((void*)0, 1, 1, 1, 0, 0));
    }

    [TestMethod]
    public void Test_Span3DT_Array1DConstructor()
    {
        int[] array = Enumerable.Range(1, 20).ToArray();

        // Same as above, but wrapping a 1D array with data in row-major order
        Span3D<int> span3d = new(array, 1, 2, 2, 2, 2, 1);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(8, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(2, span3d.Width);

        span3d[0, 0, 0] = 99;
        span3d[1, 1, 1] = 101;

        Assert.AreEqual(99, array[1]);
        Assert.AreEqual(101, array[13]);

        // The first check fails due to the array covariance test mentioned in the Memory3D<T> tests.
        // The others just validate a number of cases with invalid arguments (e.g. out of range).
        _ = Assert.ThrowsExactly<ArrayTypeMismatchException>(() => new Span3D<object>(new string[1], 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, -1, 1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, -1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, 1, -1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, 1, 1, -1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, 1, 1, 1, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, 1, 1, 1, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new Span3D<int>(array, 0, 4, 4, 4, 0, 0));
    }

    [TestMethod]
    public void Test_Span3DT_Array3DConstructor_1()
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

        // Same as above, but directly wrapping a 3D array
        Span3D<int> span3d = new(array);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(12, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(3, span3d.Width);

        span3d[0, 1, 2] = 99;
        span3d[1, 0, 1] = 101;

        Assert.AreEqual(99, array[0, 1, 2]);
        Assert.AreEqual(101, array[1, 0, 1]);

        _ = Assert.ThrowsExactly<ArrayTypeMismatchException>(() => new Span3D<object>(new string[1, 1, 1]));
    }

    [TestMethod]
    public void Test_Span3DT_Array3DConstructor_2()
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

        // Same as above, but with a custom slicing over the target 3D array
        Span3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

        Assert.IsFalse(span3d.IsEmpty);
        Assert.AreEqual(8, span3d.Length);
        Assert.AreEqual(2, span3d.Depth);
        Assert.AreEqual(2, span3d.Height);
        Assert.AreEqual(2, span3d.Width);

        span3d[1, 1, 1] = 101;

        Assert.AreEqual(101, array[1, 1, 2]);

        _ = Assert.ThrowsExactly<ArrayTypeMismatchException>(() => new Span3D<object>(new string[1, 1, 1], 0, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, -1, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, -1, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, 0, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, 0, 0, 3, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, 0, 0, 1, 3, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 0, 0, 0, 1, 1, 5));
    }

    [TestMethod]
    public void Test_Span3DT_FillAndClear_1()
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

        // Tests for the Fill and Clear APIs for Span3D<T>. These should fill
        // or clear the entire wrapped 3D array (just like e.g. Span<T>.Fill).
        Span3D<int> span3d = new(array);

        span3d.Fill(42);

        Assert.IsTrue(array.Cast<int>().All(n => n == 42));

        span3d.Clear();

        Assert.IsTrue(array.Cast<int>().All(n => n == 0));
    }

    [TestMethod]
    public void Test_Span3DT_Fill_Empty()
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

        // Same as above, but with an initial slicing as well to ensure
        // these method work correctly with different internal offsets
        Span3D<int> span3d = new(array, 0, 0, 0, 0, 0, 0);

        span3d.Fill(42);
        CollectionAssert.AreEqual(array, array);

        span3d.Clear();
        CollectionAssert.AreEqual(array, array);
    }

    [TestMethod]
    public void Test_Span3DT_FillAndClear_2()
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

        // Same as above, just with different slicing to a target smaller 3D area
        Span3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

        span3d.Fill(42);

        int[,,] filled =
        {
            {
                { 1, 42, 42 },
                { 4, 42, 42 }
            },
            {
                { 10, 42, 42 },
                { 40, 42, 42 }
            }
        };

        CollectionAssert.AreEqual(array, filled);

        span3d.Clear();

        int[,,] cleared =
        {
            {
                { 1, 0, 0 },
                { 4, 0, 0 }
            },
            {
                { 10, 0, 0 },
                { 40, 0, 0 }
            }
        };

        CollectionAssert.AreEqual(array, cleared);
    }

    [TestMethod]
    public void Test_Span3DT_CopyTo_Empty()
    {
        Span3D<int> span3d = Span3D<int>.Empty;

        int[] target = new int[0];

        // Copying an empty Span3D<T> to an empty array is just a no-op
        span3d.CopyTo(target);
    }

    [TestMethod]
    public void Test_Span3DT_CopyTo_1()
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

        Span3D<int> span3d = new(array);

        int[] target = new int[array.Length];

        // Here we copy a Span3D<T> to a target Span<T> mapping an array.
        // This is valid, and the data will just be copied in row-major order,
        // one slice at a time.
        span3d.CopyTo(target);

        int[] expected = Enumerable.Range(1, 12).ToArray();

        CollectionAssert.AreEqual(expected, target);

        // Exception due to the target span being too small for the source Span3D<T> instance
        _ = Assert.ThrowsExactly<ArgumentException>(() => new Span3D<int>(array).CopyTo(Span<int>.Empty));
    }

    [TestMethod]
    public void Test_Span3DT_CopyTo_2()
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

        // Same as above, but with different initial slicing
        Span3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

        int[] target = new int[8];

        span3d.CopyTo(target);

        int[] expected = { 2, 3, 5, 6, 8, 9, 11, 12 };

        CollectionAssert.AreEqual(target, expected);

        _ = Assert.ThrowsExactly<ArgumentException>(() => new Span3D<int>(array).CopyTo(Span<int>.Empty));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new Span3D<int>(array, 0, 0, 1, 2, 2, 2).CopyTo(Span<int>.Empty));
    }

    [TestMethod]
    public void Test_Span3DT_CopyTo3D_1()
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

        Span3D<int> span3d = new(array);

        int[,,] target = new int[2, 2, 3];

        // Same as above, but copying to a target Span3D<T> instead. Note
        // that this method uses the implicit T[,,] to Span3D<T> conversion.
        span3d.CopyTo(target);

        CollectionAssert.AreEqual(array, target);

        _ = Assert.ThrowsExactly<ArgumentException>(() => new Span3D<int>(array).CopyTo(Span3D<int>.Empty));
    }

    [TestMethod]
    public void Test_Span3DT_CopyTo3D_2()
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

        // Same as above, but with extra initial slicing
        Span3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

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

        _ = Assert.ThrowsExactly<ArgumentException>(() => new Span3D<int>(array).CopyTo(new Span3D<int>(target)));
    }

    [TestMethod]
    public void Test_Span3DT_TryCopyTo()
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

        Span3D<int> span3d = new(array);

        int[] target = new int[array.Length];

        // Here we test the safe TryCopyTo method, which will fail gracefully
        Assert.IsTrue(span3d.TryCopyTo(target));
        Assert.IsFalse(span3d.TryCopyTo(Span<int>.Empty));

        int[] expected = Enumerable.Range(1, 12).ToArray();

        CollectionAssert.AreEqual(target, expected);
    }

    [TestMethod]
    public void Test_Span3DT_TryCopyTo3D()
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

        // Same as above, but copying to a 3D array with the safe TryCopyTo method
        Span3D<int> span3d = new(array);

        int[,,] target = new int[2, 2, 3];

        Assert.IsTrue(span3d.TryCopyTo(target));
        Assert.IsFalse(span3d.TryCopyTo(Span3D<int>.Empty));

        CollectionAssert.AreEqual(target, array);
    }

    [TestMethod]
    public unsafe void Test_Span3DT_GetPinnableReference()
    {
        // Here we test that a ref from an empty Span3D<T> returns a null ref
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef<int>(null),
            ref Span3D<int>.Empty.GetPinnableReference()));

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

        Span3D<int> span3d = new(array);

        ref int r0 = ref span3d.GetPinnableReference();

        // Here we test that GetPinnableReference returns a ref to the first array element
        Assert.IsTrue(Unsafe.AreSame(ref r0, ref array[0, 0, 0]));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_DangerousGetReference()
    {
        // Same as above, but using DangerousGetReference instead (faster, no conditional check)
        Assert.IsTrue(Unsafe.AreSame(
            ref Unsafe.AsRef<int>(null),
            ref Span3D<int>.Empty.DangerousGetReference()));

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

        Span3D<int> span3d = new(array);

        ref int r0 = ref span3d.DangerousGetReference();

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref array[0, 0, 0]));
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public unsafe void Test_Span3DT_Index_Indexer_1()
    {
        int[,,] array = new int[4, 4, 4];

        Span3D<int> span3d = new(array);

        ref int arrayRef = ref array[1, 2, 3];
        ref int span3dRef = ref span3d[1, 2, ^1];

        Assert.IsTrue(Unsafe.AreSame(ref arrayRef, ref span3dRef));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Index_Indexer_2()
    {
        int[,,] array = new int[4, 4, 4];

        Span3D<int> span3d = new(array);

        ref int arrayRef = ref array[2, 1, 0];
        ref int span3dRef = ref span3d[^2, ^3, ^4];

        Assert.IsTrue(Unsafe.AreSame(ref arrayRef, ref span3dRef));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Index_Indexer_Fail()
    {
        int[,,] array = new int[4, 4, 4];

        _ = Assert.ThrowsExactly<IndexOutOfRangeException>(() =>
        {
            Span3D<int> span3d = new(array);

            ref int span3dRef = ref span3d[^6, 2, 1];
        });
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Range_Indexer_1()
    {
        int[,,] array = new int[4, 4, 4];

        Span3D<int> span3d = new(array);
        Span3D<int> slice = span3d[1.., 1.., 1..];

        Assert.AreEqual(27, slice.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[1, 1, 1], ref slice[0, 0, 0]));
        Assert.IsTrue(Unsafe.AreSame(ref array[3, 3, 3], ref slice[2, 2, 2]));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Range_Indexer_2()
    {
        int[,,] array = new int[4, 4, 4];

        Span3D<int> span3d = new(array);
        Span3D<int> slice = span3d[0..^2, 1..^1, 0..^2];

        Assert.AreEqual(8, slice.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[0, 1, 0], ref slice[0, 0, 0]));
        Assert.IsTrue(Unsafe.AreSame(ref array[1, 2, 1], ref slice[1, 1, 1]));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Range_Indexer_Fail()
    {
        int[,,] array = new int[4, 4, 4];

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            Span3D<int> span3d = new(array);

            _ = span3d[0..6, 2..^1, 0..2];
        });
    }
#endif

    [TestMethod]
    public void Test_Span3DT_Slice_1()
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

        // Here we have a number of tests that just take an initial 3D array, create a Span3D<T>,
        // perform a number of slicing operations and then validate the parameters for the resulting
        // instances, and that the indexer works correctly and maps to the right original elements.
        Span3D<int> span3d = new(array);

        Span3D<int> slice1 = span3d.Slice(1, 0, 1, 1, 2, 2);

        Assert.AreEqual(4, slice1.Length);
        Assert.AreEqual(1, slice1.Depth);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(8, slice1[0, 0, 0]);
        Assert.AreEqual(12, slice1[0, 1, 1]);

        Span3D<int> slice2 = span3d.Slice(0, 1, 0, 2, 1, 3);

        Assert.AreEqual(6, slice2.Length);
        Assert.AreEqual(2, slice2.Depth);
        Assert.AreEqual(1, slice2.Height);
        Assert.AreEqual(3, slice2.Width);
        Assert.AreEqual(4, slice2[0, 0, 0]);
        Assert.AreEqual(10, slice2[1, 0, 0]);
        Assert.AreEqual(12, slice2[1, 0, 2]);

        // Some checks for invalid arguments
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(-1, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(0, -1, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(0, 0, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(0, 0, 0, 0, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(0, 0, 0, 1, 0, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(0, 0, 0, 1, 1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(10, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(0, 10, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).Slice(0, 0, 10, 1, 1, 1));
    }

    [TestMethod]
    public void Test_Span3DT_Slice_2()
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

        Span3D<int> span3d = new(array);

        // Same as above, but with some different slicing
        Span3D<int> slice1 = span3d.Slice(0, 0, 0, 2, 2, 2);

        Assert.AreEqual(8, slice1.Length);
        Assert.AreEqual(2, slice1.Depth);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(1, slice1[0, 0, 0]);
        Assert.AreEqual(11, slice1[1, 1, 1]);

        Span3D<int> slice2 = slice1.Slice(1, 0, 1, 1, 2, 1);

        Assert.AreEqual(2, slice2.Length);
        Assert.AreEqual(1, slice2.Depth);
        Assert.AreEqual(2, slice2.Height);
        Assert.AreEqual(1, slice2.Width);
        Assert.AreEqual(8, slice2[0, 0, 0]);
        Assert.AreEqual(11, slice2[0, 1, 0]);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_Span3DT_GetRowSpan()
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

        Span3D<int> span3d = new(array);

        // Here we create a Span3D<T> from a 3D array and want to get a Span<T> from
        // a specific row. This is only supported on runtimes with fast Span<T> support
        // for the same reason mentioned in the Memory3D<T> tests (we need the Span<T>
        // constructor that only takes a target ref). Then we just get some references
        // to items in this span and compare them against references into the original
        // 3D array to ensure they match and point to the correct elements from there.
        Span<int> span = span3d.GetRowSpan(1, 0);

        Assert.IsTrue(Unsafe.AreSame(
            ref span[0],
            ref array[1, 0, 0]));
        Assert.IsTrue(Unsafe.AreSame(
            ref span[2],
            ref array[1, 0, 2]));

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetRowSpan(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetRowSpan(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetRowSpan(5, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetRowSpan(0, 5));
    }

    [TestMethod]
    public void Test_Span3DT_GetSliceSpan()
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

        Span3D<int> span3d = new(array);

        // Here we create a Span3D<T> from a 3D array and want to get a Span<T> from
        // a specific slice. This is only supported on runtimes with fast Span<T> support
        // for the same reason mentioned in the Memory3D<T> tests (we need the Span<T>
        // constructor that only takes a target ref). Then we just get some references
        // to items in this span and compare them against references into the original
        // 3D array to ensure they match and point to the correct elements from there.
        Span2D<int> slice = span3d.GetSliceSpan(1);

        Assert.AreEqual(2, slice.Height);
        Assert.AreEqual(3, slice.Width);
        Assert.IsTrue(Unsafe.AreSame(
            ref slice[0, 0],
            ref array[1, 0, 0]));
        Assert.IsTrue(Unsafe.AreSame(
            ref slice[1, 2],
            ref array[1, 1, 2]));

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetSliceSpan(-1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetSliceSpan(5));
    }
#endif

    [TestMethod]
    public void Test_Span3DT_TryGetSpan_From1DArray_1()
    {
        int[] array = Enumerable.Range(1, 24).ToArray();

        Span3D<int> span3d = new(array, 2, 3, 4);

        bool success = span3d.TryGetSpan(out Span<int> span);

        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span3d.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[0], ref span[0]));
    }

    [TestMethod]
    public void Test_Span3DT_TryGetSpan_From1DArray_2()
    {
        int[] array = Enumerable.Range(1, 24).ToArray();

        Span3D<int> span3d = new Span3D<int>(array, 2, 3, 4).Slice(1, 0, 0, 1, 3, 4);

        bool success = span3d.TryGetSpan(out Span<int> span);

        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span3d.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[12], ref span[0]));
    }

    [TestMethod]
    public void Test_Span3DT_TryGetSpan_From1DArray_3()
    {
        int[] array = Enumerable.Range(1, 24).ToArray();

        Span3D<int> span3d = new Span3D<int>(array, 2, 3, 4).Slice(0, 1, 1, 2, 2, 2);

        bool success = span3d.TryGetSpan(out Span<int> span);

        Assert.IsFalse(success);
        Assert.AreEqual(0, span.Length);
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3947
    [TestMethod]
    public void Test_Span3DT_TryGetSpan_From1DArray_4()
    {
        int[] array = new int[128];
        Span3D<int> span3d = new(array, 2, 4, 16);

        bool success = span3d.TryGetSpan(out Span<int> span);

        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span3d.Length);
        Assert.IsTrue(Unsafe.AreSame(ref array[0], ref span[0]));
    }

    [TestMethod]
    public void Test_Span3DT_TryGetSpan_From3DArray_1()
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

        Span3D<int> span3d = new(array);

        // This API tries to get a Span<T> for the entire contents of Span3D<T>.
        // This only works on runtimes if the underlying data is contiguous
        // and of a size that can fit into a single Span<T>. In this specific test,
        // this is not expected to work on .NET Standard 2.0 because it can't create a
        // Span<T> from a 3D array (reasons explained in the comments for the test above).
        bool success = span3d.TryGetSpan(out Span<int> span);

#if NETFRAMEWORK
        // Can't get a Span<T> over a T[,,] array on .NET Standard 2.0
        Assert.IsFalse(success);
        Assert.AreEqual(0, span.Length);
#else
        Assert.IsTrue(success);
        Assert.AreEqual(span.Length, span3d.Length);
#endif
    }

    [TestMethod]
    public void Test_Span3DT_TryGetSpan_From3DArray_2()
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

        // Same as above, but this will always fail because we're creating
        // a Span3D<T> wrapping non-contiguous data (the pitch is not 0).
        Span3D<int> span3d = new(array, 0, 0, 1, 2, 2, 2);

        bool success = span3d.TryGetSpan(out Span<int> span);

        Assert.IsFalse(success);
        Assert.IsTrue(span.IsEmpty);
    }

    [TestMethod]
    public void Test_Span3DT_ToArray_1()
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

        // Here we create a Span3D<T> and verify that ToArray() produces
        // a 3D array that is identical to the original one being wrapped.
        Span3D<int> span3d = new(array);

        int[,,] copy = span3d.ToArray();

        Assert.AreEqual(copy.GetLength(0), array.GetLength(0));
        Assert.AreEqual(copy.GetLength(1), array.GetLength(1));
        Assert.AreEqual(copy.GetLength(2), array.GetLength(2));

        CollectionAssert.AreEqual(array, copy);
    }

    [TestMethod]
    public void Test_Span3DT_ToArray_2()
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

        // Same as above, but with extra initial slicing
        Span3D<int> span3d = new(array, 0, 0, 0, 1, 2, 2);

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
    public void Test_Span3DT_Equals()
    {
        int[,,] array = new int[1, 1, 1];

        // Span3D<T>.Equals always throw (this mirrors the behavior of Span<T>.Equals)
        _ = Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            Span3D<int> span3d = new(array);

            _ = span3d.Equals(null);
        });
    }

    [TestMethod]
    public void Test_Span3DT_GetHashCode()
    {
        int[,,] array = new int[1, 1, 1];

        // Same as above, this always throws
        _ = Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            Span3D<int> span3d = new(array);

            _ = span3d.GetHashCode();
        });
    }

    [TestMethod]
    public void Test_Span3DT_ToString()
    {
        int[,,] array = new int[2, 2, 3];

        Span3D<int> span3d = new(array);

        // Verify that we get the nicely formatted string
        string text = span3d.ToString();

        const string expected = "CommunityToolkit.HighPerformance.Span3D<System.Int32>[2, 2, 3]";

        Assert.AreEqual(expected, text);
    }

    [TestMethod]
    public void Test_Span3DT_opEquals()
    {
        int[,,] array = new int[2, 2, 3];

        // Create two Span3D<T> instances wrapping the same array with the same
        // parameters, and verify that the equality operators work correctly.
        Span3D<int> span3d_1 = new(array);
        Span3D<int> span3d_2 = new(array);

        Assert.IsTrue(span3d_1 == span3d_2);
        Assert.IsFalse(span3d_1 == Span3D<int>.Empty);
        Assert.IsTrue(Span3D<int>.Empty == Span3D<int>.Empty);

        // Same as above, but verify that a sliced span is not reported as equal
        Span3D<int> span3d_3 = new(array, 0, 0, 0, 1, 2, 3);

        Assert.IsFalse(span3d_1 == span3d_3);
        Assert.IsFalse(span3d_3 == Span3D<int>.Empty);
    }

    [TestMethod]
    public void Test_Span3DT_ImplicitCast()
    {
        int[,,] array = new int[2, 2, 3];

        // Verify that an explicit constructor and the implicit conversion
        // operator generate an identical Span3D<T> instance from the array.
        Span3D<int> span3d_1 = array;
        Span3D<int> span3d_2 = new(array);

        Assert.IsTrue(span3d_1 == span3d_2);
    }

    [TestMethod]
    public void Test_Span3DT_GetRow()
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

        // Get a target row and verify the contents match with our data
        RefEnumerable<int> enumerable = new Span3D<int>(array).GetRow(1, 0);

        int[] expected = { 7, 8, 9 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetRow(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetRow(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetRow(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetRow(0, 2));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Pointer_GetRow()
    {
        int* array = stackalloc[]
        {
            1, 2, 3,
            4, 5, 6,
            7, 8, 9,
            10, 11, 12
        };

        // Same as above, but with a Span3D<T> wrapping a raw pointer
        RefEnumerable<int> enumerable = new Span3D<int>(array, 2, 2, 3, 0, 0).GetRow(1, 0);

        int[] expected = { 7, 8, 9 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetRow(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetRow(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetRow(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetRow(0, 2));
    }

    [TestMethod]
    public void Test_Span3DT_GetColumn()
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

        // Same as above, but getting a column instead
        RefEnumerable<int> enumerable = new Span3D<int>(array).GetColumn(0, 2);

        int[] expected = { 3, 6 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetColumn(0, 3));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Pointer_GetColumn()
    {
        int* array = stackalloc[]
        {
            1, 2, 3,
            4, 5, 6,
            7, 8, 9,
            10, 11, 12
        };

        // Same as above, but wrapping a raw pointer
        RefEnumerable<int> enumerable = new Span3D<int>(array, 2, 2, 3, 0, 0).GetColumn(0, 2);

        int[] expected = { 3, 6 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetColumn(0, 3));
    }

    [TestMethod]
    public void Test_Span3DT_GetDepthColumn()
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

        // Same as above, but getting a column along depth instead
        RefEnumerable<int> enumerable = new Span3D<int>(array).GetDepthColumn(1, 1);

        int[] expected = { 5, 11 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetDepthColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetDepthColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetDepthColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array).GetDepthColumn(0, 3));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Pointer_GetDepthColumn()
    {
        int* array = stackalloc[]
        {
            1, 2, 3,
            4, 5, 6,
            7, 8, 9,
            10, 11, 12
        };

        // Same as above, but wrapping a raw pointer
        RefEnumerable<int> enumerable = new Span3D<int>(array, 2, 2, 3, 0, 0).GetDepthColumn(0, 2);

        int[] expected = { 3, 9 };

        CollectionAssert.AreEqual(enumerable.ToArray(), expected);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetDepthColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetDepthColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetDepthColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Span3D<int>(array, 2, 2, 3, 0, 0).GetDepthColumn(0, 3));
    }

    [TestMethod]
    public unsafe void Test_Span3DT_GetEnumerator()
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

        int[] result = new int[8];
        int i = 0;

        // Here we want to test the Span3D<T> enumerator. We create a Span3D<T> instance over
        // a given section of the initial 3D array, then iterate over it and store the items
        // into a temporary array. We then just compare the contents to ensure they match.
        foreach (ref int item in new Span3D<int>(array, 0, 0, 1, 2, 2, 2))
        {
            int slice = i / 4;
            int row = (i % 4) / 2;
            int column = (i % 2) + 1;

            // Check the reference to ensure it points to the right original item
            Assert.IsTrue(Unsafe.AreSame(
                ref array[slice, row, column],
                ref item));

            // Also store the value to compare it later (redundant, but just in case)
            result[i++] = item;
        }

        int[] expected = { 2, 3, 5, 6, 8, 9, 11, 12 };

        CollectionAssert.AreEqual(result, expected);
    }

    [TestMethod]
    public unsafe void Test_Span3DT_Pointer_GetEnumerator()
    {
        int* array = stackalloc[]
        {
            1, 2, 3, 4, 5, 6,
            7, 8, 9, 10, 11, 12
        };

        int[] result = new int[8];
        int i = 0;

        // Same test as above, but wrapping a raw pointer
        foreach (ref int item in new Span3D<int>(array + 1, 2, 2, 2, 0, 1))
        {
            int slice = i / 4;
            int row = (i % 4) / 2;
            int column = (i % 2);

            int index = ((slice * 6) + (row * 3) + column) + 1;

            // Check the reference again
            Assert.IsTrue(Unsafe.AreSame(
                ref Unsafe.AsRef<int>(&array[index]),
                ref item));

            result[i++] = item;
        }

        int[] expected = { 2, 3, 5, 6, 8, 9, 11, 12 };

        CollectionAssert.AreEqual(result, expected);
    }

    [TestMethod]
    public void Test_Span3DT_GetEnumerator_Empty()
    {
        Span3D<int>.Enumerator enumerator = Span3D<int>.Empty.GetEnumerator();

        // Ensure that an enumerator from an empty Span3D<T> can't move next
        Assert.IsFalse(enumerator.MoveNext());
    }
}
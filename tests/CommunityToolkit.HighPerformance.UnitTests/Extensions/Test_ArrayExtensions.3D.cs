// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

public partial class Test_ArrayExtensions
{
    [TestMethod]
    public void Test_ArrayExtensions_3D_DangerousGetReference_Int()
    {
        int[,,] array = new int[10, 20, 12];

        // See comments in Test_ArrayExtensions.1D for how these tests work
        ref int r0 = ref array.DangerousGetReference();
        ref int r1 = ref array[0, 0, 0];

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_DangerousGetReference_String()
    {
        string[,,] array = new string[10, 20, 12];

        ref string r0 = ref array.DangerousGetReference();
        ref string r1 = ref array[0, 0, 0];

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_DangerousGetReferenceAt_Zero()
    {
        int[,,] array = new int[10, 20, 12];

        ref int r0 = ref array.DangerousGetReferenceAt(0, 0, 0);
        ref int r1 = ref array[0, 0, 0];

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_DangerousGetReferenceAt_Index()
    {
        int[,,] array = new int[10, 20, 12];

        ref int r0 = ref array.DangerousGetReferenceAt(5, 3, 4);
        ref int r1 = ref array[5, 3, 4];

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_AsSpan3DAndFillArrayMid()
    {
        bool[,,] test = new bool[3, 3, 3];

        // To fill an array we now go through the Span3D<T> type, which includes all
        // the necessary logic to perform the operation. In these tests we just create
        // one through the extension, slice it and then fill it. For instance in this
        // one, we're creating a Span3D<bool> from coordinates (1, 1, 1), with a depth of
        // 2, a height of 2, and a width of 2, and then filling it.
        // Then we just compare the results.
        test.AsSpan3D(1, 1, 1, 1, 2, 2).Fill(true);

        bool[,,]? expected = new[,,]
        {
            {
                { false, false, false },
                { false, false, false },
                { false, false, false },
            },
            {
                { false, false, false },
                { false,  true,  true },
                { false,  true,  true },
            },
            {
                { false, false, false },
                { false, false, false },
                { false, false, false },
            }
        };

        CollectionAssert.AreEqual(expected, test);
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_AsSpan3DAndFillArrayTwice()
    {
        bool[,,] test = new bool[3, 3, 3];

        test.AsSpan3D(0, 0, 0, 1, 1, 2).Fill(true);
        test.AsSpan3D(2, 1, 1, 1, 2, 2).Fill(true);

        bool[,,]? expected = new[,,]
        {
            {
                {  true,  true, false },
                { false, false, false },
                { false, false, false },
            },
            {
                { false, false, false },
                { false, false, false },
                { false, false, false },
            },
            {
                { false, false, false },
                { false,  true,  true },
                { false,  true,  true },
            }
        };

        CollectionAssert.AreEqual(expected, test);
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_AsSpan3DAndFillArrayBottomEdgeBoundary()
    {
        bool[,,] test = new bool[3, 4, 4];

        test.AsSpan3D(1, 2, 1, 2, 2, 3).Fill(true);

        bool[,,]? expected = new[,,]
        {
            {
                { false, false, false, false },
                { false, false, false, false },
                { false, false, false, false },
                { false, false, false, false },
            },
            {
                { false, false, false, false },
                { false, false, false, false },
                { false,  true,  true,  true },
                { false,  true,  true,  true },
            },
            {
                { false, false, false, false },
                { false, false, false, false },
                { false,  true,  true,  true },
                { false,  true,  true,  true },
            }
        };

        CollectionAssert.AreEqual(expected, test);
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_AsSpan3DAndFillArrayBottomRightCornerBoundary()
    {
        bool[,,] test = new bool[2, 2, 2];

        test.AsSpan3D(1, 1, 1, 1, 1, 1).Fill(true);

        bool[,,]? expected = new[,,]
        {
            {
                { false, false },
                { false, false },
            },
            {
                { false, false },
                { false,  true },
            }
        };

        CollectionAssert.AreEqual(expected, test);
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_GetRow_Rectangle()
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

        // Here we use the enumerator on the RefEnumerator<T> type to traverse items in a row
        // by reference. For each one, we check that the reference does in fact point to the
        // item we expect in the underlying array (in this case, items on row 1).
        int j = 0;
        foreach (ref int value in array.GetRow(1, 0))
        {
            Assert.IsTrue(Unsafe.AreSame(ref value, ref array[1, 0, j++]));
        }

        // Check that RefEnumerable<T>.ToArray() works correctly
        CollectionAssert.AreEqual(array.GetRow(1, 0).ToArray(), new[] { 7, 8, 9 });

        // Test an empty array
        Assert.AreSame(new int[1, 1, 0].GetRow(0, 0).ToArray(), Array.Empty<int>());

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetRow(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetRow(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetRow(20, 0));

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetRow(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetRow(0, 2));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetRow(0, 20));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_GetColumn_Rectangle()
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

        // Same as above, but this time we iterate a column instead (so non-contiguous items)
        int i = 0;
        foreach (ref int value in array.GetColumn(0, 1))
        {
            Assert.IsTrue(Unsafe.AreSame(ref value, ref array[0, i++, 1]));
        }

        CollectionAssert.AreEqual(array.GetColumn(0, 1).ToArray(), new[] { 2, 5 });

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetColumn(20, 0));

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetColumn(0, 3));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetColumn(0, 20));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_GetDepthColumn_Rectangle()
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

        // Same as above, but this time we iterate a depth column instead (so non-contiguous items)
        int i = 0;
        foreach (ref int value in array.GetDepthColumn(1, 2))
        {
            Assert.IsTrue(Unsafe.AreSame(ref value, ref array[i++, 1, 2]));
        }

        CollectionAssert.AreEqual(array.GetDepthColumn(1, 2).ToArray(), new[] { 6, 12 });

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetDepthColumn(-1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetDepthColumn(2, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetDepthColumn(20, 0));

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetDepthColumn(0, -1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetDepthColumn(0, 3));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetDepthColumn(0, 20));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_GetRow_Empty()
    {
        int[,,] array = new int[0, 0, 0];

        // Try to get a row from an empty array (the row index isn't in range)
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetRow(0, 0).ToArray());
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_GetColumn_Empty()
    {
        int[,,] array = new int[0, 0, 0];

        // Try to get a column from an empty array (the row index isn't in range)
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetColumn(0, 0).ToArray());
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_GetDepthColumn_Empty()
    {
        int[,,] array = new int[0, 0, 0];

        // Try to get a depth column from an empty array (the row index isn't in range)
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.GetDepthColumn(0, 0).ToArray());
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_GetRowOrColumn_Helpers()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            },
            {
                { 10, 11, 12 },
                { 13, 14, 15 },
                { 16, 17, 18 }
            },
            {
                { 19, 20, 21 },
                { 22, 23, 24 },
                { 25, 26, 27 }
            }
        };

        // Get a row and test the Clear method. Note that the Span3D<T> here is sliced
        // starting from the second column, so this method should clear the row from index 1.
        array.AsSpan3D(1, 1, 1, 2, 2, 2).GetRow(0, 0).Clear();

        int[,,] expected =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            },
            {
                { 10, 11, 12 },
                { 13, 0, 0 },
                { 16, 17, 18 }
            },
            {
                { 19, 20, 21 },
                { 22, 23, 24 },
                { 25, 26, 27 }
            }
        };

        CollectionAssert.AreEqual(array, expected);

        // Same as before, but this time we fill a column with a value
        array.GetColumn(2, 1).Fill(42);

        expected = new[,,]
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            },
            {
                { 10, 11, 12 },
                { 13, 0, 0 },
                { 16, 17, 18 }
            },
            {
                { 19, 42, 21 },
                { 22, 42, 24 },
                { 25, 42, 27 }
            }
        };

        CollectionAssert.AreEqual(array, expected);

        int[] copy = new int[3];

        // Get a row and copy items to a target span (in this case, wrapping an array)
        array.GetRow(0, 2).CopyTo(copy);

        int[] result = { 7, 8, 9 };

        CollectionAssert.AreEqual(copy, result);

        // Same as above, but copying from a depth column (so we test non-contiguous sequences too)
        array.GetDepthColumn(1, 2).CopyTo(copy);

        result = new[] { 6, 0, 24 };

        CollectionAssert.AreEqual(copy, result);

        // Some invalid attempts to copy to an empty span or sequence
        _ = Assert.ThrowsExactly<ArgumentException>(() => array.GetRow(0, 0).CopyTo(default(RefEnumerable<int>)));
        _ = Assert.ThrowsExactly<ArgumentException>(() => array.GetRow(0, 0).CopyTo(default(Span<int>)));

        _ = Assert.ThrowsExactly<ArgumentException>(() => array.GetColumn(0, 0).CopyTo(default(RefEnumerable<int>)));
        _ = Assert.ThrowsExactly<ArgumentException>(() => array.GetColumn(0, 0).CopyTo(default(Span<int>)));

        _ = Assert.ThrowsExactly<ArgumentException>(() => array.GetDepthColumn(0, 0).CopyTo(default(RefEnumerable<int>)));
        _ = Assert.ThrowsExactly<ArgumentException>(() => array.GetDepthColumn(0, 0).CopyTo(default(Span<int>)));

        // Same as CopyTo, but this will fail gracefully with an invalid target
        Assert.IsTrue(array.GetRow(0, 2).TryCopyTo(copy));
        Assert.IsFalse(array.GetRow(0, 0).TryCopyTo(default(Span<int>)));

        result = new[] { 7, 8, 9 };

        CollectionAssert.AreEqual(copy, result);

        // Also fill a row and then further down clear a column (trying out all possible combinations)
        array.GetRow(0, 1).Fill(99);

        expected = new[,,]
        {
            {
                { 1, 2, 3 },
                { 99, 99, 99 },
                { 7, 8, 9 }
            },
            {
                { 10, 11, 12 },
                { 13, 0, 0 },
                { 16, 17, 18 }
            },
            {
                { 19, 42, 21 },
                { 22, 42, 24 },
                { 25, 42, 27 }
            }
        };

        CollectionAssert.AreEqual(array, expected);

        array.GetDepthColumn(2, 0).Clear();

        expected = new[,,]
        {
            {
                { 1, 2, 3 },
                { 99, 99, 99 },
                { 0, 8, 9 }
            },
            {
                { 10, 11, 12 },
                { 13, 0, 0 },
                { 0, 17, 18 }
            },
            {
                { 19, 42, 21 },
                { 22, 42, 24 },
                { 0, 42, 27 }
            }
        };

        CollectionAssert.AreEqual(array, expected);
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_ReadOnlyGetRowOrColumn_Helpers()
    {
        int[,,] array =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            },
            {
                { 10, 11, 12 },
                { 13, 14, 15 },
                { 16, 17, 18 }
            },
            {
                { 19, 20, 21 },
                { 22, 23, 24 },
                { 25, 26, 27 }
            }
        };

        // This test pretty much does the same things as the method above, but this time
        // using a source ReadOnlySpan3D<T>, so that the sequence type being tested is
        // ReadOnlyRefEnumerable<T> instead (which shares most features but is separate).
        ReadOnlySpan3D<int> span3D = array;

        int[] copy = new int[3];

        span3D.GetRow(1, 2).CopyTo(copy);

        int[] result = { 16, 17, 18 };

        CollectionAssert.AreEqual(copy, result);

        span3D.GetColumn(0, 1).CopyTo(copy);

        result = new[] { 2, 5, 8 };

        CollectionAssert.AreEqual(copy, result);

        span3D.GetDepthColumn(0, 2).CopyTo(copy);

        result = new[] { 3, 12, 21 };

        CollectionAssert.AreEqual(copy, result);

        _ = Assert.ThrowsExactly<ArgumentException>(() => ((ReadOnlySpan3D<int>)array).GetRow(0, 0).CopyTo(default(RefEnumerable<int>)));
        _ = Assert.ThrowsExactly<ArgumentException>(() => ((ReadOnlySpan3D<int>)array).GetRow(0, 0).CopyTo(default(Span<int>)));

        _ = Assert.ThrowsExactly<ArgumentException>(() => ((ReadOnlySpan3D<int>)array).GetColumn(0, 0).CopyTo(default(RefEnumerable<int>)));
        _ = Assert.ThrowsExactly<ArgumentException>(() => ((ReadOnlySpan3D<int>)array).GetColumn(0, 0).CopyTo(default(Span<int>)));

        _ = Assert.ThrowsExactly<ArgumentException>(() => ((ReadOnlySpan3D<int>)array).GetDepthColumn(0, 0).CopyTo(default(RefEnumerable<int>)));
        _ = Assert.ThrowsExactly<ArgumentException>(() => ((ReadOnlySpan3D<int>)array).GetDepthColumn(0, 0).CopyTo(default(Span<int>)));

        Assert.IsTrue(span3D.GetRow(2, 1).TryCopyTo(copy));
        Assert.IsFalse(span3D.GetRow(2, 1).TryCopyTo(default(Span<int>)));

        result = new[] { 22, 23, 24 };

        CollectionAssert.AreEqual(copy, result);
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_RefEnumerable_Misc()
    {
        int[,,] array1 =
        {
            {
                { 1, 2 },
                { 3, 4 }
            },
            {
                { 5, 6 },
                { 7, 8 }
            }
        };

        int[,,] array2 = new int[2, 2, 2];

        // Copy to enumerable with source step == 1, destination step == 1
        array1.GetRow(0, 0).CopyTo(array2.GetRow(0, 0));

        // Copy enumerable with source step == 1, destination step != 1
        array1.GetRow(0, 1).CopyTo(array2.GetColumn(0, 1));

        // Copy enumerable with source step != 1, destination step == 1
        array1.GetColumn(1, 0).CopyTo(array2.GetRow(1, 1));

        // Copy enumerable with source step != 1, destination step != 1
        array1.GetDepthColumn(0, 1).CopyTo(array2.GetDepthColumn(1, 0));

        int[,,] result =
        {
            {
                { 1, 3 },
                { 2, 4 }
            },
            {
                { 0, 0 },
                { 6, 7 }
            }
        };

        CollectionAssert.AreEqual(array2, result);

        // Test a valid and an invalid TryCopyTo call with the RefEnumerable<T> overload
        bool shouldBeTrue = array1.GetRow(0, 0).TryCopyTo(array2.GetColumn(1, 1));
        bool shouldBeFalse = array1.GetRow(0, 0).TryCopyTo(default(RefEnumerable<int>));

        result = new[,,]
        {
            {
                { 1, 3 },
                { 2, 4 }
            },
            {
                { 0, 1 },
                { 6, 2 }
            }
        };

        CollectionAssert.AreEqual(array2, result);

        Assert.IsTrue(shouldBeTrue);
        Assert.IsFalse(shouldBeFalse);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ArrayExtensions_3D_AsSpan_Empty()
    {
        int[,,] array = new int[0, 0, 0];

        Span<int> span = array.AsSpan();

        // Check that the empty array was loaded properly
        Assert.HasCount(span.Length, array);
        Assert.IsTrue(span.IsEmpty);
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_AsSpan_Populated()
    {
        int[,,] array =
        {
            {
                { 1, 2 },
                { 3, 4 }
            },
            {
                { 5, 6 },
                { 7, 8 }
            }
        };

        Span<int> span = array.AsSpan();

        // Test the total length of the span
        Assert.HasCount(span.Length, array);

        ref int r0 = ref array[0, 0, 0];
        ref int r1 = ref span[0];

        // Similarly to the top methods, here we compare a given reference to
        // ensure they point to the right element back in the original array.
        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }
#endif
}
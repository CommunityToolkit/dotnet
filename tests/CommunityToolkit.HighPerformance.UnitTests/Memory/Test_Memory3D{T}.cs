// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_Memory3DT
{
    [TestMethod]
    public void Test_Memory3DT_Empty()
    {
        // Create a few empty Memory3D<T> instances in different ways and
        // check to ensure the right parameters were used to initialize them.
        Memory3D<int> empty1 = default;

        Assert.IsTrue(empty1.IsEmpty);
        Assert.AreEqual(0, empty1.Length);
        Assert.AreEqual(0, empty1.Depth);
        Assert.AreEqual(0, empty1.Height);
        Assert.AreEqual(0, empty1.Width);

        Memory3D<string> empty2 = Memory3D<string>.Empty;

        Assert.IsTrue(empty2.IsEmpty);
        Assert.AreEqual(0, empty2.Length);
        Assert.AreEqual(0, empty2.Depth);
        Assert.AreEqual(0, empty2.Height);
        Assert.AreEqual(0, empty2.Width);

        Memory3D<int> empty3 = new int[0, 2, 3];

        Assert.IsTrue(empty3.IsEmpty);
        Assert.AreEqual(0, empty3.Length);
        Assert.AreEqual(0, empty3.Depth);
        Assert.AreEqual(2, empty3.Height);
        Assert.AreEqual(3, empty3.Width);

        Memory3D<int> empty4 = new int[2, 0, 3];

        Assert.IsTrue(empty4.IsEmpty);
        Assert.AreEqual(0, empty4.Length);
        Assert.AreEqual(2, empty4.Depth);
        Assert.AreEqual(0, empty4.Height);
        Assert.AreEqual(3, empty4.Width);

        Memory3D<int> empty5 = new int[2, 3, 0];

        Assert.IsTrue(empty5.IsEmpty);
        Assert.AreEqual(0, empty5.Length);
        Assert.AreEqual(2, empty5.Depth);
        Assert.AreEqual(3, empty5.Height);
        Assert.AreEqual(0, empty5.Width);

#if NET6_0_OR_GREATER
        MemoryManager<int> memoryManager = new UnmanagedSpanOwner<int>(1);
        Memory3D<int> empty6 = new(memoryManager, 0, 0, 0);

        Assert.IsTrue(empty6.IsEmpty);
        Assert.AreEqual(0, empty6.Length);
        Assert.AreEqual(0, empty6.Depth);
        Assert.AreEqual(0, empty6.Height);
        Assert.AreEqual(0, empty6.Width);

        Memory3D<int> empty7 = new(memoryManager, 2, 0, 0);

        Assert.IsTrue(empty7.IsEmpty);
        Assert.AreEqual(0, empty7.Length);
        Assert.AreEqual(2, empty7.Depth);
        Assert.AreEqual(0, empty7.Height);
        Assert.AreEqual(0, empty7.Width);

        Memory3D<int> empty8 = new(memoryManager, 0, 2, 0);

        Assert.IsTrue(empty8.IsEmpty);
        Assert.AreEqual(0, empty8.Length);
        Assert.AreEqual(0, empty8.Depth);
        Assert.AreEqual(2, empty8.Height);
        Assert.AreEqual(0, empty8.Width);

        Memory3D<int> empty9 = new(memoryManager, 0, 0, 3);

        Assert.IsTrue(empty9.IsEmpty);
        Assert.AreEqual(0, empty9.Length);
        Assert.AreEqual(0, empty9.Depth);
        Assert.AreEqual(0, empty9.Height);
        Assert.AreEqual(3, empty9.Width);
#endif
    }

    [TestMethod]
    public void Test_Memory3DT_Array1DConstructor()
    {
        int[] array =
        {
            1, 2, 3, 4, 5,
            6, 7, 8, 9, 10,
            11, 12, 13, 14, 15,
            16, 17, 18, 19, 20
        };

        // Create a memory over a 1D array with 23 data in row-major order. This tests
        // the T[] array constructor for Memory3D<T> with custom sizes and pitches.
        Memory3D<int> memory3d = new(array, 1, 2, 2, 2, 2, 1);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(8, memory3d.Length);
        Assert.AreEqual(2, memory3d.Depth);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(2, memory3d.Width);
        Assert.AreEqual(2, memory3d.Span[0, 0, 0]);
        Assert.AreEqual(14, memory3d.Span[1, 1, 1]);

        // Also ensure the right exceptions are thrown with invalid parameters, such as
        // negative indices, indices out of range, values that are too big, etc.
        _ = Assert.ThrowsExactly<ArrayTypeMismatchException>(() => new Memory3D<object>(new string[1], 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, -1, 1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, -1, 1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, 1, -1, 1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, 1, 1, -1, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, 1, 1, 1, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, 1, 1, 1, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new Memory3D<int>(array, 0, 2, 2, 2, 30, 0));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new Memory3D<int>(array, 0, 2, 2, 2, 0, 30));
    }

    [TestMethod]
    public void Test_Memory3DT_Array3DConstructor_1()
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

        // Test the constructor taking a T[,,] array that is mapped directly (no slicing)
        Memory3D<int> memory3d = new(array);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(12, memory3d.Length);
        Assert.AreEqual(2, memory3d.Depth);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(3, memory3d.Width);
        Assert.AreEqual(2, memory3d.Span[0, 0, 1]);
        Assert.AreEqual(60, memory3d.Span[1, 1, 2]);

        // Here we test the check for covariance: we can't create a Memory3D<T> from a U[,,] array
        // where U is assignable to T (as in, U : T). This would cause a type safety violation on write.
        _ = Assert.ThrowsExactly<ArrayTypeMismatchException>(() => new Memory3D<object>(new string[1, 1, 1]));
    }

    [TestMethod]
    public void Test_Memory3DT_Array3DConstructor_2()
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

        // Same as above, but this time we also slice the memory to test the other constructor
        Memory3D<int> memory3d = new(array, 0, 0, 1, 2, 2, 2);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(8, memory3d.Length);
        Assert.AreEqual(2, memory3d.Depth);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(2, memory3d.Width);
        Assert.AreEqual(2, memory3d.Span[0, 0, 0]);
        Assert.AreEqual(60, memory3d.Span[1, 1, 1]);

        _ = Assert.ThrowsExactly<ArrayTypeMismatchException>(() => new Memory3D<object>(new string[1, 1, 1], 0, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, -1, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, -1, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, 0, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, 0, 0, 3, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, 0, 0, 1, 3, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array, 0, 0, 0, 1, 1, 5));
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_Memory3DT_MemoryConstructor()
    {
        Memory<int> memory = new[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18
        };

        // We also test the constructor that takes an input Memory<T> instance.
        // This is only available on runtimes with fast Span<T> support, as otherwise
        // the implementation would be too complex and slow to work in this case.
        // Conceptually, this works the same as when wrapping a 1D array with row-major items.
        Memory3D<int> memory3d = memory.AsMemory3D(1, 2, 2, 2, 1, 2);

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
    public void Test_Memory3DT_MemoryManagerConstructor()
    {
        MemoryManager<int> memoryManager = new UnmanagedSpanOwner<int>(8);

        Memory3D<int> memory3d = new(memoryManager, 2, 2, 2);

        Assert.IsFalse(memory3d.IsEmpty);
        Assert.AreEqual(8, memory3d.Length);
        Assert.AreEqual(2, memory3d.Depth);
        Assert.AreEqual(2, memory3d.Height);
        Assert.AreEqual(2, memory3d.Width);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(memoryManager, -1, 2, 2, 2, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(memoryManager, 0, -2, 2, 2, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(memoryManager, 0, 2, -2, 2, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(memoryManager, 0, 2, 2, -2, 0, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(memoryManager, 0, 2, 2, 2, -1, 0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(memoryManager, 0, 2, 2, 2, 0, -1));
        _ = Assert.ThrowsExactly<ArgumentException>(() => new Memory3D<int>(memoryManager, 0, 2, 2, 2, 30, 0));
    }
#endif

    [TestMethod]
    public void Test_Memory3DT_Slice_1()
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

        Memory3D<int> memory3d = new(array);

        // Test a slice from a Memory3D<T> with valid parameters
        Memory3D<int> slice1 = memory3d.Slice(1, 0, 1, 1, 2, 2);

        Assert.AreEqual(4, slice1.Length);
        Assert.AreEqual(1, slice1.Depth);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(8, slice1.Span[0, 0, 0]);
        Assert.AreEqual(12, slice1.Span[0, 1, 1]);

        // Same above, but we test slicing a pre-sliced instance as well. This
        // is done to verify that the internal offsets are properly tracked
        // across multiple slicing operations, instead of just in the first.
        Memory3D<int> slice2 = memory3d.Slice(0, 1, 0, 2, 1, 3);

        Assert.AreEqual(6, slice2.Length);
        Assert.AreEqual(2, slice2.Depth);
        Assert.AreEqual(1, slice2.Height);
        Assert.AreEqual(3, slice2.Width);
        Assert.AreEqual(4, slice2.Span[0, 0, 0]);
        Assert.AreEqual(10, slice2.Span[1, 0, 0]);
        Assert.AreEqual(12, slice2.Span[1, 0, 2]);

        // A few invalid slicing operations, with out of range parameters
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(-1, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(0, -1, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(0, 0, -1, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(10, 0, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(0, 10, 0, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(0, 0, 10, 1, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(0, 0, 0, 3, 1, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(0, 0, 0, 1, 3, 1));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Memory3D<int>(array).Slice(0, 0, 0, 1, 1, 5));
    }

    [TestMethod]
    public void Test_Memory3DT_Slice_2()
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

        Memory3D<int> memory3d = new(array);

        // Mostly the same test as above, just with different parameters
        Memory3D<int> slice1 = memory3d.Slice(0, 0, 0, 2, 2, 2);

        Assert.AreEqual(8, slice1.Length);
        Assert.AreEqual(2, slice1.Depth);
        Assert.AreEqual(2, slice1.Height);
        Assert.AreEqual(2, slice1.Width);
        Assert.AreEqual(1, slice1.Span[0, 0, 0]);
        Assert.AreEqual(11, slice1.Span[1, 1, 1]);

        Memory3D<int> slice2 = slice1.Slice(1, 0, 1, 1, 2, 1);

        Assert.AreEqual(2, slice2.Length);
        Assert.AreEqual(1, slice2.Depth);
        Assert.AreEqual(2, slice2.Height);
        Assert.AreEqual(1, slice2.Width);
        Assert.AreEqual(8, slice2.Span[0, 0, 0]);
        Assert.AreEqual(11, slice2.Span[0, 1, 0]);
    }

    [TestMethod]
    public void Test_Memory3DT_TryGetMemory_1()
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

        Memory3D<int> memory3d = new(array);

        // Here we test that we can get a Memory<T> from a 3D one when the underlying
        // data is contiguous. Note that in this case this can only work on runtimes
        // with fast Span<T> support, because otherwise it's not possible to get a
        // Memory<T> (or a Span<T> too, for that matter) from a 3D array.
        bool success = memory3d.TryGetMemory(out Memory<int> memory);

#if NETFRAMEWORK
        Assert.IsFalse(success);
        Assert.IsTrue(memory.IsEmpty);
#else
        Assert.IsTrue(success);
        Assert.HasCount(memory.Length, array);
        Assert.IsTrue(Unsafe.AreSame(ref array[0, 0, 0], ref memory.Span[0]));
#endif
    }

    [TestMethod]
    public void Test_Memory3DT_TryGetMemory_2()
    {
        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8 };

        Memory3D<int> memory3d = new(array, 2, 2, 2);

        // Same test as above, but this will always succeed on all runtimes,
        // as creating a Memory<T> from a 1D array is always supported.
        bool success = memory3d.TryGetMemory(out Memory<int> memory);

        Assert.IsTrue(success);
        Assert.HasCount(memory.Length, array);
        Assert.AreEqual(8, memory.Span[7]);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_Memory3DT_TryGetMemory_3()
    {
        Memory<int> data = new[] { 1, 2, 3, 4 };

        Memory3D<int> memory3d = data.AsMemory3D(1, 2, 2);

        // Same as above, just with the extra Memory<T> indirection. Same as above,
        // this test is only supported on runtimes with fast Span<T> support.
        // On others, we just don't expose the Memory<T>.AsMemory3D extension.
        bool success = memory3d.TryGetMemory(out Memory<int> memory);

        Assert.IsTrue(success);
        Assert.AreEqual(memory.Length, data.Length);
        Assert.AreEqual(3, memory.Span[2]);
    }
#endif

    [TestMethod]
    public unsafe void Test_Memory3DT_Pin_1()
    {
        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8 };

        // We create a Memory3D<T> from an array and verify that pinning this
        // instance correctly returns a pointer to the right array element.
        Memory3D<int> memory3d = new(array, 2, 2, 2);

        using MemoryHandle pin = memory3d.Pin();

        Assert.AreEqual(1, ((int*)pin.Pointer)[0]);
        Assert.AreEqual(8, ((int*)pin.Pointer)[7]);
    }

    [TestMethod]
    public unsafe void Test_Memory3DT_Pin_2()
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

        // Same as above, but we test with a sliced Memory3D<T> instance
        Memory3D<int> memory3d = new(array, 1, 0, 0, 1, 2, 2);

        using MemoryHandle pin = memory3d.Pin();

        Assert.AreEqual(7, ((int*)pin.Pointer)[0]);
        Assert.AreEqual(10, ((int*)pin.Pointer)[3]);
    }

    [TestMethod]
    public void Test_Memory3DT_ToArray_1()
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

        // Here we create a Memory3D<T> instance from a 3D array and then verify that
        // calling ToArray() creates an array that matches the contents of the first.
        Memory3D<int> memory3d = new(array);

        int[,,] copy = memory3d.ToArray();

        Assert.AreEqual(copy.GetLength(0), array.GetLength(0));
        Assert.AreEqual(copy.GetLength(1), array.GetLength(1));
        Assert.AreEqual(copy.GetLength(2), array.GetLength(2));

        CollectionAssert.AreEqual(array, copy);
    }

    [TestMethod]
    public void Test_Memory3DT_ToArray_2()
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

        // Same as above, but with a sliced Memory3D<T> instance
        Memory3D<int> memory3d = new(array, 0, 0, 0, 1, 2, 2);

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
    public void Test_Memory3DT_Equals()
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

        // Here we want to verify that the Memory3D<T>.Equals method works correctly. This is true
        // when the wrapped instance is the same, and the various internal offsets and sizes match.
        Memory3D<int> memory3d = new(array);

        Assert.IsFalse(memory3d.Equals(null));
        Assert.IsFalse(memory3d.Equals(new Memory3D<int>(array, 0, 0, 1, 2, 2, 2)));
        Assert.IsTrue(memory3d.Equals(new Memory3D<int>(array)));
        Assert.IsTrue(memory3d.Equals(memory3d));

        // This should work also when casting to a ReadOnlyMemory3D<T> instance
        ReadOnlyMemory3D<int> readOnlyMemory3d = memory3d;

        Assert.IsTrue(memory3d.Equals(readOnlyMemory3d));
        Assert.IsFalse(memory3d.Equals(readOnlyMemory3d.Slice(0, 0, 1, 2, 2, 2)));
    }

    [TestMethod]
    public void Test_Memory3DT_GetHashCode()
    {
        // An empty Memory3D<T> has just 0 as the hashcode
        Assert.AreEqual(0, Memory3D<int>.Empty.GetHashCode());

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

        Memory3D<int> memory3d = new(array);

        // Ensure that the GetHashCode method is repeatable
        int a = memory3d.GetHashCode();
        int b = memory3d.GetHashCode();

        Assert.AreEqual(a, b);

        // The hashcode shouldn't match when the size is different
        int c = new Memory3D<int>(array, 0, 0, 0, 1, 2, 2).GetHashCode();

        Assert.AreNotEqual(a, c);
    }

    [TestMethod]
    public void Test_Memory3DT_ToString()
    {
        int[,,] array = new int[2, 2, 3];

        Memory3D<int> memory3d = new(array);

        // Here we just want to verify that the type is nicely printed as expected, along with the size
        string text = memory3d.ToString();

        const string expected = "CommunityToolkit.HighPerformance.Memory3D<System.Int32>[2, 2, 3]";

        Assert.AreEqual(expected, text);
    }

    [TestMethod]
    public void Test_Memory3DT_ImplicitCast()
    {
        int[,,] array = new int[2, 2, 3];

        Memory3D<int> memory3d_1 = array;
        Memory3D<int> memory3d_2 = new(array);

        Assert.IsTrue(memory3d_1.Equals(memory3d_2));
    }

#if NET6_0_OR_GREATER
    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3536
    [TestMethod]
    [DataRow(64, 180, 320)]  // depth, height, width
    public void Test_Memory3DT_CastAndSlice_WorksCorrectly(int depth, int height, int width)
    {
        Memory3D<int> data =
            new byte[depth * height * width * sizeof(int)]
            .AsMemory()
            .Cast<byte, int>()
            .AsMemory3D(depth: depth, height: height, width: width);

        Memory3D<int> slice = data.Slice(
            slice: depth / 2,
            row: height / 2,
            column: 0,
            depth: depth / 2,
            height: height / 2,
            width: width);

        Assert.IsTrue(Unsafe.AreSame(ref data.Span[depth / 2, height / 2, 0], ref slice.Span[0, 0, 0]));
        Assert.IsTrue(Unsafe.AreSame(ref data.Span[depth / 2, height / 2, width - 1], ref slice.Span[0, 0, width - 1]));
        Assert.IsTrue(Unsafe.AreSame(ref data.Span[depth / 2, height - 1, 0], ref slice.Span[0, (height / 2) - 1, 0]));
        Assert.IsTrue(Unsafe.AreSame(ref data.Span[depth / 2, height - 1, width - 1], ref slice.Span[0, (height / 2) - 1, width - 1]));

        Assert.IsTrue(Unsafe.AreSame(ref data.Span[depth - 1, height / 2, 0], ref slice.Span[(depth / 2) - 1, 0, 0]));
        Assert.IsTrue(Unsafe.AreSame(ref data.Span[depth - 1, height / 2, width - 1], ref slice.Span[(depth / 2) - 1, 0, width - 1]));
        Assert.IsTrue(Unsafe.AreSame(ref data.Span[depth - 1, height - 1, 0], ref slice.Span[(depth / 2) - 1, (height / 2) - 1, 0]));
        Assert.IsTrue(Unsafe.AreSame(ref data.Span[depth - 1, height - 1, width - 1], ref slice.Span[(depth / 2) - 1, (height / 2) - 1, width - 1]));
    }
#endif
}
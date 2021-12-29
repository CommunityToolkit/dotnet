// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;

namespace CommunityToolkit.HighPerformance.UnitTests.Helpers;

[TestClass]
public class Test_HashCodeOfT
{
    /// <summary>
    /// Gets the list of counts to test the extension for
    /// </summary>
    private static ReadOnlySpan<int> TestCounts => new[] { 0, 1, 7, 128, 255, 256, short.MaxValue, short.MaxValue + 1, 123_938, 1_678_922, 71_890_819 };

    [TestMethod]
    public void Test_HashCodeOfT_VectorSupportedTypes_TestRepeatCount8()
    {
        TestForType<byte>();
        TestForType<sbyte>();
        TestForType<bool>();
    }

    [TestMethod]
    public void Test_HashCodeOfT_VectorSupportedTypes_TestRepeatCount16()
    {
        TestForType<ushort>();
        TestForType<short>();
    }

    [TestMethod]
    public void Test_HashCodeOfT_VectorSupportedTypes_TestRepeatCount32()
    {
        TestForType<uint>();
        TestForType<int>();
        TestForType<float>();
    }

    [TestMethod]
    public void Test_HashCodeOfT_VectorSupportedTypes_TestRepeatCount64()
    {
        TestForType<ulong>();
        TestForType<long>();
        TestForType<double>();
    }

    [TestMethod]
    public void Test_HashCodeOfT_VectorUnsupportedTypes_TestRepeat()
    {
        TestForType<char>();
    }

#if NETCOREAPP3_1_OR_GREATER
    [TestMethod]
    public void Test_HashCodeOfT_ManagedType_TestRepeat()
    {
        ReadOnlySpan<int> localTestCounts = TestCounts.Slice(0, 8);

        // Only rent a single array of the maximum necessary size, to save space
        string[] data = new string[localTestCounts[localTestCounts.Length - 1]];

        Random? random = new();
        foreach (ref string text in data.AsSpan())
        {
            text = random.NextDouble().ToString("E");
        }

        foreach (int count in localTestCounts)
        {
            Span<string> iterationData = data.AsSpan().Slice(0, count);

            int hash1 = HashCode<string>.Combine(iterationData);
            int hash2 = HashCode<string>.Combine(iterationData);

            Assert.AreEqual(hash1, hash2, $"Failed {typeof(string)} test with count {count}: got {hash1} and then {hash2}");
        }
    }
#endif

    /// <summary>
    /// Performs a test for a specified type.
    /// </summary>
    /// <typeparam name="T">The type to test.</typeparam>
    private static void TestForType<T>()
        where T : unmanaged, IEquatable<T>
    {
        foreach (int count in TestCounts)
        {
            using UnmanagedSpanOwner<T> data = CreateRandomData<T>(count);

            int hash1 = HashCode<T>.Combine(data.GetSpan());
            int hash2 = HashCode<T>.Combine(data.GetSpan());

            Assert.AreEqual(hash1, hash2, $"Failed {typeof(T)} test with count {count}: got {hash1} and then {hash2}");
        }
    }

    /// <summary>
    /// Creates a random <typeparamref name="T"/> array filled with random data.
    /// </summary>
    /// <typeparam name="T">The type of items to put in the array.</typeparam>
    /// <param name="count">The number of array items to create.</param>
    /// <returns>An array of random <typeparamref name="T"/> elements.</returns>
    private static UnmanagedSpanOwner<T> CreateRandomData<T>(int count)
        where T : unmanaged
    {
        Random? random = new(count);

        UnmanagedSpanOwner<T> data = new(count);

        foreach (ref byte n in MemoryMarshal.AsBytes(data.GetSpan()))
        {
            n = (byte)random.Next(0, byte.MaxValue);
        }

        return data;
    }
}

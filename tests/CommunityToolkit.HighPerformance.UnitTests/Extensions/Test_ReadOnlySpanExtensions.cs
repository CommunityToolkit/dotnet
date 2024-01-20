// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

[TestClass]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601", Justification = "Partial test class")]
public partial class Test_ReadOnlySpanExtensions
{
    [TestMethod]
    public void Test_ReadOnlySpanExtensions_DangerousGetReference()
    {
        using UnmanagedSpanOwner<int>? owner = CreateRandomData<int>(12, default);

        ReadOnlySpan<int> data = owner.GetSpan();

        ref int r0 = ref data.DangerousGetReference();
        ref int r1 = ref Unsafe.AsRef(in data[0]);

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_DangerousGetReferenceAt_Zero()
    {
        using UnmanagedSpanOwner<int>? owner = CreateRandomData<int>(12, default);

        ReadOnlySpan<int> data = owner.GetSpan();

        ref int r0 = ref data.DangerousGetReference();
        ref int r1 = ref data.DangerousGetReferenceAt(0);

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_DangerousGetReferenceAt_Index()
    {
        using UnmanagedSpanOwner<int>? owner = CreateRandomData<int>(12, default);

        ReadOnlySpan<int> data = owner.GetSpan();

        ref int r0 = ref data.DangerousGetReferenceAt(5);
        ref int r1 = ref Unsafe.AsRef(in data[5]);

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(4)]
    [DataRow(22)]
    [DataRow(43)]
    [DataRow(44)]
    [DataRow(45)]
    [DataRow(46)]
    [DataRow(100)]
    [DataRow(int.MaxValue)]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    public void Test_ReadOnlySpanExtensions_DangerousGetLookupReferenceAt(int i)
    {
        ReadOnlySpan<byte> table = new byte[]
        {
            0xFF, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 1, 1, 0, 1, 1, 1, 1,
            0, 1, 0, 1, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 1, 0, 1
        };

        ref byte ri = ref Unsafe.AsRef(in table.DangerousGetLookupReferenceAt(i));

        bool isInRange = (uint)i < (uint)table.Length;

        if (isInRange)
        {
            Assert.IsTrue(Unsafe.AreSame(ref ri, ref Unsafe.AsRef(in table[i])));
        }
        else
        {
            Assert.IsTrue(Unsafe.AreSame(ref ri, ref MemoryMarshal.GetReference(table)));
        }
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_IndexOf_Empty()
    {
        static void Test<T>()
        {
            T? a = default;

            int indexOfA = default(ReadOnlySpan<T?>).IndexOf(in a);

            Assert.AreEqual(-1, indexOfA);

            ReadOnlySpan<T?> data = new T?[] { default };

            int indexOfData = data.Slice(1).IndexOf(in data[0]);

            Assert.AreEqual(-1, indexOfData);
        }

        Test<byte>();
        Test<int>();
        Test<Guid>();
        Test<string>();
        Test<object>();
        Test<IEnumerable<int>>();
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_IndexOf_NotEmpty()
    {
        static void Test<T>()
        {
            ReadOnlySpan<T?> data = new T?[] { default, default, default, default };

            for (int i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(i, data.IndexOf(in data[i]));
            }
        }

        Test<byte>();
        Test<int>();
        Test<Guid>();
        Test<string>();
        Test<object>();
        Test<IEnumerable<int>>();
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_IndexOf_NotEmpty_OutOfRange()
    {
        static void Test<T>()
        {
            // Before start
            ReadOnlySpan<T?> data = new T?[] { default, default, default, default };

            int index = data.Slice(1).IndexOf(in data[0]);

            Assert.AreEqual(-1, index);

            // After end
            data = new T?[] { default, default, default, default };

            index = data.Slice(0, 2).IndexOf(in data[2]);

            Assert.AreEqual(-1, index);

            // Local variable
            T?[]? dummy = new T?[] { default };

             data = new T?[] { default, default, default, default };

            index = data.IndexOf(in dummy[0]);

            Assert.AreEqual(-1, index);
        }

        Test<byte>();
        Test<int>();
        Test<Guid>();
        Test<string>();
        Test<object>();
        Test<IEnumerable<int>>();
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_Enumerate()
    {
        ReadOnlySpan<int> data = new[] { 1, 2, 3, 4, 5, 6, 7 };

        int i = 0;

        foreach (HighPerformance.Enumerables.ReadOnlySpanEnumerable<int>.Item item in data.Enumerate())
        {
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in data[i]), ref Unsafe.AsRef(in item.Value)));
            Assert.AreEqual(i, item.Index);

            i++;
        }
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_Enumerate_Empty()
    {
        ReadOnlySpan<int> data = Array.Empty<int>();

        foreach (HighPerformance.Enumerables.ReadOnlySpanEnumerable<int>.Item _ in data.Enumerate())
        {
            Assert.Fail("Empty source sequence");
        }
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_Tokenize_Empty()
    {
        string text = string.Empty;

        List<string>? result = new();

        foreach (ReadOnlySpan<char> token in text.AsSpan().Tokenize(','))
        {
            result.Add(new string(token.ToArray()));
        }

        string[]? tokens = text.Split(',');

        CollectionAssert.AreEqual(result, tokens);
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_Tokenize_Comma()
    {
        string text = "name,surname,city,year,profession,age";

        List<string>? result = new();

        foreach (ReadOnlySpan<char> token in text.AsSpan().Tokenize(','))
        {
            result.Add(new string(token.ToArray()));
        }

        string[]? tokens = text.Split(',');

        CollectionAssert.AreEqual(result, tokens);
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_Tokenize_CommaWithMissingValues()
    {
        string text = ",name,,city,,,profession,,age,,";

        List<string>? result = new();

        foreach (ReadOnlySpan<char> token in text.AsSpan().Tokenize(','))
        {
            result.Add(new string(token.ToArray()));
        }

        string[]? tokens = text.Split(',');

        CollectionAssert.AreEqual(result, tokens);
    }

    [TestMethod]
    public void Test_ReadOnlySpanExtensions_CopyTo_RefEnumerable()
    {
        int[,] array = new int[4, 5];

        ReadOnlySpan<int> values1 = new[] { 10, 20, 30, 40, 50 };
        ReadOnlySpan<int> values2 = new[] { 11, 22, 33, 44, 55 };

        // Copy a span to a target row and column with valid lengths
        values1.CopyTo(array.GetRow(0));
        values2.Slice(0, 4).CopyTo(array.GetColumn(1));

        int[,] result =
        {
            { 10, 11, 30, 40, 50 },
            { 0, 22, 0, 0, 0 },
            { 0, 33, 0, 0, 0 },
            { 0, 44, 0, 0, 0 }
        };

        CollectionAssert.AreEqual(array, result);

        // Try to copy to a valid row and an invalid column (too short for the source span)
        bool shouldBeTrue = values1.TryCopyTo(array.GetRow(2));
        bool shouldBeFalse = values2.TryCopyTo(array.GetColumn(3));

        Assert.IsTrue(shouldBeTrue);
        Assert.IsFalse(shouldBeFalse);

        result = new[,]
        {
            { 10, 11, 30, 40, 50 },
            { 0, 22, 0, 0, 0 },
            { 10, 20, 30, 40, 50 },
            { 0, 44, 0, 0, 0 }
        };

        CollectionAssert.AreEqual(array, result);
    }
}

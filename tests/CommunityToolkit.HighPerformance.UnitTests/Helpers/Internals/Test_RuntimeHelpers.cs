// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET6_0_OR_GREATER

using System;
using CommunityToolkit.HighPerformance.Helpers.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS0649

namespace CommunityToolkit.HighPerformance.UnitTests.Helpers.Internals;

[TestClass]
public class Test_RuntimeHelpers
{
    [TestMethod]
    public void Test_RuntimeHelpers_ManagedTypes()
    {
        Assert.IsTrue(RuntimeHelpers.IsReferenceOrContainsReferences<object>());
        Assert.IsTrue(RuntimeHelpers.IsReferenceOrContainsReferences<string>());
        Assert.IsTrue(RuntimeHelpers.IsReferenceOrContainsReferences<int[]>());
        Assert.IsTrue(RuntimeHelpers.IsReferenceOrContainsReferences<Memory<int>>());
        Assert.IsTrue(RuntimeHelpers.IsReferenceOrContainsReferences<ManagedStruct1>());
        Assert.IsTrue(RuntimeHelpers.IsReferenceOrContainsReferences<ManagedStruct2>());
        Assert.IsTrue(RuntimeHelpers.IsReferenceOrContainsReferences<ManagedStruct3>());
    }

    [TestMethod]
    public void Test_RuntimeHelpers_UnmanagedTypes()
    {
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<byte>());
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<int>());
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<IntPtr>());
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<Guid>());
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<byte>());
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<UnmanagedStruct1>());
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<UnmanagedStruct2>());
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<GenericStruct<UnmanagedStruct1>>());
        Assert.IsFalse(RuntimeHelpers.IsReferenceOrContainsReferences<GenericStruct<UnmanagedStruct2>>());
    }

    private unsafe struct ManagedStruct1
    {
        public int A;
        public Memory<int> B;
    }

    private struct ManagedStruct2
    {
        public int A;
        public ManagedStruct1 B;
    }

    private struct ManagedStruct3
    {
        public GenericStruct<Memory<int>> A;
    }

    private unsafe struct UnmanagedStruct1
    {
        public int A;
        public double B;
        public Guid C;
        public IntPtr D;
        public int* E;
        public void** F;
        public GenericStruct<int> G;
        public GenericStruct<Guid> H;
    }

    private struct UnmanagedStruct2
    {
        public int A;
        public UnmanagedStruct1 B;
        public UnmanagedStruct1? C;
        public Guid? D;
    }

    private struct GenericStruct<T>
        where T : struct
    {
        public T A;
        public T? B;
    }
}

#endif

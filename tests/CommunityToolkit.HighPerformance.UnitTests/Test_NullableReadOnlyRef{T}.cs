// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET7_0_OR_GREATER

using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_NullableReadOnlyRefOfT
{
    [TestMethod]
    public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_Ok()
    {
        int value = 1;
        NullableReadOnlyRef<int> reference = new(value);

        Assert.IsTrue(reference.HasValue);
        Assert.IsTrue(Unsafe.AreSame(ref value, ref Unsafe.AsRef(in reference.Value)));
    }

    [TestMethod]
    public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_Null()
    {
        Assert.IsFalse(default(NullableReadOnlyRef<int>).HasValue);
        Assert.IsFalse(NullableReadOnlyRef<int>.Null.HasValue);

        Assert.IsFalse(default(NullableReadOnlyRef<string>).HasValue);
        Assert.IsFalse(NullableReadOnlyRef<string>.Null.HasValue);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_Null_Exception()
    {
        NullableReadOnlyRef<int> reference = default;

        _ = reference.Value;
    }

    [TestMethod]
    public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ImplicitRefCast()
    {
        int value = 42;
        Ref<int> reference = new(ref value);
        NullableReadOnlyRef<int> nullableRef = reference;

        Assert.IsTrue(nullableRef.HasValue);
        Assert.IsTrue(Unsafe.AreSame(ref reference.Value, ref Unsafe.AsRef(in nullableRef.Value)));
    }

    [TestMethod]
    public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ImplicitReadOnlyRefCast()
    {
        int value = 42;
        ReadOnlyRef<int> reference = new(value);
        NullableReadOnlyRef<int> nullableRef = reference;

        Assert.IsTrue(nullableRef.HasValue);
        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(in reference.Value), ref Unsafe.AsRef(in nullableRef.Value)));
    }

    [TestMethod]
    public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ImplicitNullableRefCast()
    {
        int value = 42;
        NullableRef<int> reference = new(ref value);
        NullableReadOnlyRef<int> nullableRef = reference;

        Assert.IsTrue(nullableRef.HasValue);
        Assert.IsTrue(Unsafe.AreSame(ref reference.Value, ref Unsafe.AsRef(in nullableRef.Value)));
    }

    [TestMethod]
    public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ExplicitCastOfT()
    {
        int value = 42;
        NullableRef<int> reference = new(ref value);

        Assert.AreEqual(value, (int)reference);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ExplicitCastOfT_Exception()
    {
        NullableRef<int> invalid = default;

        _ = (int)invalid;
    }
}

#endif
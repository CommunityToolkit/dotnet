// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET7_0_OR_GREATER

using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_NullableRefOfT
{
    [TestMethod]
    public void Test_NullableRefOfT_CreateNullableRefOfT_Ok()
    {
        int value = 1;
        NullableRef<int> reference = new(ref value);

        Assert.IsTrue(reference.HasValue);
        Assert.IsTrue(Unsafe.AreSame(ref value, ref reference.Value));

        reference.Value++;

        Assert.AreEqual(value, 2);
    }

    [TestMethod]
    public void Test_NullableRefOfT_CreateNullableRefOfT_Null()
    {
        Assert.IsFalse(default(NullableRef<int>).HasValue);
        Assert.IsFalse(NullableRef<int>.Null.HasValue);

        Assert.IsFalse(default(NullableRef<string>).HasValue);
        Assert.IsFalse(NullableRef<string>.Null.HasValue);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_NullableRefOfT_CreateNullableRefOfT_Null_Exception()
    {
        NullableRef<int> reference = default;

        _ = reference.Value;
    }

    [TestMethod]
    public void Test_NullableRefOfT_CreateNullableRefOfT_ImplicitRefCast()
    {
        int value = 42;
        Ref<int> reference = new(ref value);
        NullableRef<int> nullableRef = reference;

        Assert.IsTrue(nullableRef.HasValue);
        Assert.IsTrue(Unsafe.AreSame(ref reference.Value, ref nullableRef.Value));
    }

    [TestMethod]
    public void Test_NullableRefOfT_CreateNullableRefOfT_ExplicitCastOfT()
    {
        int value = 42;
        NullableRef<int> reference = new(ref value);

        Assert.AreEqual(value, (int)reference);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_NullableRefOfT_CreateNullableRefOfT_ExplicitCastOfT_Exception()
    {
        NullableRef<int> invalid = default;

        _ = (int)invalid;
    }
}

#endif

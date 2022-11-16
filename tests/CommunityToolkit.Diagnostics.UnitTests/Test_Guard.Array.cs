// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Diagnostics.UnitTests;

public partial class Test_Guard
{
    [TestMethod]
    public void Test_Guard_IsEmpty_ArrayOk()
    {
        Guard.IsEmpty(new int[0], nameof(Test_Guard_IsEmpty_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsEmpty_ArrayFail()
    {
        Guard.IsEmpty(new int[1], nameof(Test_Guard_IsEmpty_ArrayFail));
    }

    [TestMethod]
    public void Test_Guard_IsNotEmpty_ArrayOk()
    {
        Guard.IsNotEmpty(new int[1], nameof(Test_Guard_IsNotEmpty_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsNotEmpty_ArrayFail()
    {
        Guard.IsNotEmpty(new int[0], nameof(Test_Guard_IsNotEmpty_ArrayFail));
    }

    [TestMethod]
    public void Test_Guard_HasSizeEqualTo_ArrayOk()
    {
        Guard.HasSizeEqualTo(new int[4], 4, nameof(Test_Guard_HasSizeEqualTo_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeEqualTo_ArrayFail()
    {
        Guard.HasSizeEqualTo(new int[3], 4, nameof(Test_Guard_HasSizeEqualTo_ArrayOk));
    }

    [TestMethod]
    public void Test_Guard_HasSizeNotEqualTo_ArrayOk()
    {
        Guard.HasSizeNotEqualTo(new int[3], 4, nameof(Test_Guard_HasSizeNotEqualTo_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeNotEqualTo_ArrayFail()
    {
        Guard.HasSizeNotEqualTo(new int[4], 4, nameof(Test_Guard_HasSizeNotEqualTo_ArrayFail));
    }

    [TestMethod]
    public void Test_Guard_HasSizeGreaterThan_ArrayOk()
    {
        Guard.HasSizeGreaterThan(new int[5], 2, nameof(Test_Guard_HasSizeGreaterThan_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeGreaterThan_ArrayEqualFail()
    {
        Guard.HasSizeGreaterThan(new int[4], 4, nameof(Test_Guard_HasSizeGreaterThan_ArrayEqualFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeGreaterThan_ArraySmallerFail()
    {
        Guard.HasSizeGreaterThan(new int[1], 4, nameof(Test_Guard_HasSizeGreaterThan_ArraySmallerFail));
    }

    [TestMethod]
    public void Test_Guard_HasSizeGreaterThanOrEqualTo_ArrayOk()
    {
        Guard.HasSizeGreaterThanOrEqualTo(new int[5], 2, nameof(Test_Guard_HasSizeGreaterThanOrEqualTo_ArrayOk));
        Guard.HasSizeGreaterThanOrEqualTo(new int[2], 2, nameof(Test_Guard_HasSizeGreaterThanOrEqualTo_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeGreaterThanOrEqualTo_ArrayFail()
    {
        Guard.HasSizeGreaterThan(new int[1], 4, nameof(Test_Guard_HasSizeGreaterThanOrEqualTo_ArrayFail));
    }

    [TestMethod]
    public void Test_Guard_HasSizeLessThan_ArrayOk()
    {
        Guard.HasSizeLessThan(new int[1], 5, nameof(Test_Guard_HasSizeLessThan_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeLessThan_ArrayEqualFail()
    {
        Guard.HasSizeLessThan(new int[4], 4, nameof(Test_Guard_HasSizeLessThan_ArrayEqualFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeLessThan_ArrayGreaterFail()
    {
        Guard.HasSizeLessThan(new int[6], 4, nameof(Test_Guard_HasSizeLessThan_ArrayGreaterFail));
    }

    [TestMethod]
    public void Test_Guard_HasSizeLessThanOrEqualTo_ArrayOk()
    {
        Guard.HasSizeLessThanOrEqualTo(new int[1], 5, nameof(Test_Guard_HasSizeLessThanOrEqualTo_ArrayOk));
        Guard.HasSizeLessThanOrEqualTo(new int[5], 5, nameof(Test_Guard_HasSizeLessThanOrEqualTo_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeLessThanOrEqualTo_ArrayFail()
    {
        Guard.HasSizeLessThanOrEqualTo(new int[8], 4, nameof(Test_Guard_HasSizeLessThanOrEqualTo_ArrayFail));
    }

    [TestMethod]
    public void Test_Guard_HasSizeEqualToArray_ArrayOk()
    {
        Guard.HasSizeEqualTo(new int[1], new int[1], nameof(Test_Guard_HasSizeEqualToArray_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeEqualToArray_ArrayFail()
    {
        Guard.HasSizeEqualTo(new int[8], new int[2], nameof(Test_Guard_HasSizeEqualToArray_ArrayFail));
    }

    [TestMethod]
    public void Test_Guard_HasSizeLessThanOrEqualToArray_ArrayOk()
    {
        Guard.HasSizeLessThanOrEqualTo(new int[2], new int[5], nameof(Test_Guard_HasSizeLessThanOrEqualToArray_ArrayOk));
        Guard.HasSizeLessThanOrEqualTo(new int[4], new int[4], nameof(Test_Guard_HasSizeLessThanOrEqualToArray_ArrayOk));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_HasSizeLessThanOrEqualToArray_ArrayFail()
    {
        Guard.HasSizeLessThanOrEqualTo(new int[8], new int[2], nameof(Test_Guard_HasSizeLessThanOrEqualToArray_ArrayFail));
    }
}

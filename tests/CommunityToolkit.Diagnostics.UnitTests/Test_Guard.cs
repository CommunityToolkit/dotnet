// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Diagnostics.UnitTests;

[TestClass]
public partial class Test_Guard
{
    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNull()
    {
        static void TestIsNull<T>(T? obj)
        {
            Guard.IsNull(obj, $"Guard<{typeof(T)}>.IsNull({obj?.ToString() ?? "null"})");
            Assert.IsNull(FGuard.IsNull(obj, $"FGuard<{typeof(T)}>.IsNull({obj?.ToString() ?? "null"})"));
        }

        TestIsNull<object>(null);
        TestIsNull<object?>(null);
        TestIsNull<int?>(null);
        TestIsNull<string>(null);
        TestIsNull<string?>(null);

        static void TestIsNull_Fail<T>(T? obj)
        {
            _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNull(obj, $"Guard<{typeof(T)}>.IsNull({obj?.ToString() ?? "null"}) Fail"));
            _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNull(obj, $"FGuard<{typeof(T)}>.IsNull({obj?.ToString() ?? "null"}) Fail"));
        }

        TestIsNull_Fail(new object());
        TestIsNull_Fail(7);
        TestIsNull_Fail<int?>(7);
        TestIsNull_Fail("Hello");
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNotNull()
    {
        static void TestIsNotNull<T>(T? obj)
        {
            Guard.IsNotNull(obj, $"Guard<{typeof(T)}>.IsNotNull({obj?.ToString() ?? "null"})");
            if (obj is ValueType)
            {
                Assert.AreEqual(obj, FGuard.IsNotNull(obj, $"FGuard<{typeof(T)}>.IsNotNull({obj?.ToString() ?? "null"})"));
            }
            else
            {
                Assert.AreSame(obj, FGuard.IsNotNull(obj, $"FGuard<{typeof(T)}>.IsNotNull({obj?.ToString() ?? "null"})"));
            }
        }

        TestIsNotNull(new object());
        TestIsNotNull(7);
        TestIsNotNull<int?>(7);
        TestIsNotNull("Hello");

        static void TestIsNotNull_Fail<T>(T? obj)
        {
            _ = Assert.ThrowsException<ArgumentNullException>(() => Guard.IsNotNull(obj, $"Guard<{typeof(T)}>.IsNotNull({obj?.ToString() ?? "null"}) Fail"));
            _ = Assert.ThrowsException<ArgumentNullException>(() => FGuard.IsNotNull(obj, $"FGuard<{typeof(T)}>.IsNotNull({obj?.ToString() ?? "null"}) Fail"));
        }

        TestIsNotNull_Fail<object>(null);
        TestIsNotNull_Fail<object?>(null);
        TestIsNotNull_Fail<int?>(null);
        TestIsNotNull_Fail<string>(null);
        TestIsNotNull_Fail<string?>(null);
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsOfType()
    {
        Guard.IsOfType<string>("Hello", "Guard.IsOfType<string>");
        Assert.AreSame("Hello", FGuard.IsOfType<string>("Hello", "FGuard.IsOfType<string>"));
        Guard.IsOfType<int>(7, "Guard.IsOfType<int>");
        Assert.AreEqual(7, FGuard.IsOfType<int>(7, "FGuard.IsOfType<int>"));
        //Assert.AreEqual(7, FGuard.IsBoxed<int>(7, "FGuard.IsBoxed<int>"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsOfType<string>(7, "Guard.IsOfType<string> Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsOfType<string>(7, "FGuard.IsOfType<string> Fail"));
        //_ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsBoxed<long>(7, "FGuard.IsBoxed<long> Fail"));

        Guard.IsOfType("Hello", typeof(string), "Guard.IsOfType(typeof(string))");
        Assert.AreSame("Hello", FGuard.IsOfType("Hello", typeof(string), "FGuard.IsOfType(typeof(string))"));
        Guard.IsOfType(7, typeof(int), "Guard.IsOfType(typeof(int))");
        Assert.AreEqual(7, FGuard.IsOfType(7, typeof(int), "FGuard.IsOfType(typeof(int))"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsOfType(7, typeof(string), "Guard.IsOfType(typeof(string)) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsOfType(7, typeof(string), "FGuard.IsOfType(typeof(string)) Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsAssignableToType()
    {
        Guard.IsAssignableToType<string>("Hello", "Guard.IsAssignableToType<string>");
        Assert.AreSame("Hello", FGuard.IsAssignableToType<string>("Hello", "FGuard.IsAssignableToType<string>"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsAssignableToType<string>(7, "Guard.IsAssignableToType<string> Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsAssignableToType<string>(7, "FGuard.IsAssignableToType<string> Fail"));

        Guard.IsAssignableToType("Hello", typeof(string), "Guard.IsAssignableToType(typeof(string))");
        Assert.AreSame("Hello", FGuard.IsAssignableToType("Hello", typeof(string), "FGuard.IsAssignableToType(typeof(string))"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsAssignableToType(7, typeof(string), "Guard.IsAssignableToType(typeof(string)) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsAssignableToType(7, typeof(string), "FGuard.IsAssignableToType(typeof(string)) Fail"));
    }

    [TestMethod]
    public void Test_Guard_IsNotAssignableToType()
    {
        Guard.IsNotAssignableToType<int>("Hello", "Guard.IsNotAssignableToType<int>");
        Assert.AreSame("Hello", FGuard.IsNotAssignableToType<int>("Hello", "FGuard.IsNotAssignableToType<int>"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNotAssignableToType<string>("Hello", "Guard.IsNotAssignableToType<string> Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNotAssignableToType<string>("Hello", "FGuard.IsNotAssignableToType<string> Fail"));

        Guard.IsNotAssignableToType("Hello", typeof(int), "Guard.IsNotAssignableToType(typeof(int))");
        Assert.AreSame("Hello", FGuard.IsNotAssignableToType("Hello", typeof(int), "FGuard.IsNotAssignableToType(typeof(int))"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNotAssignableToType<string>("Hello", "Guard.IsNotAssignableToType<string> Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNotAssignableToType<string>("Hello", "FGuard.IsNotAssignableToType<string> Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNullOrEmpty()
    {
        Guard.IsNullOrEmpty(null, "Guard.IsNullOrEmpty(null)");
        Assert.IsNull(FGuard.IsNullOrEmpty(null, "FGuard.IsNullOrEmpty(null)"));
        Guard.IsNullOrEmpty(string.Empty, "Guard.IsNullOrEmpty(empty)");
        Assert.AreSame(string.Empty, FGuard.IsNullOrEmpty(string.Empty, "FGuard.IsNullOrEmpty(empty)"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNullOrEmpty("Hello", "Guard.IsNullOrEmpty Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNullOrEmpty("Hello", "FGuard.IsNullOrEmpty Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNotNullOrEmpty_Ok()
    {
        Guard.IsNotNullOrEmpty("Hello", "Guard.IsNotNullOrEmpty");
        Assert.AreSame("Hello", FGuard.IsNotNullOrEmpty("Hello", "FGuard.IsNotNullOrEmpty"));

        _ = Assert.ThrowsException<ArgumentNullException>(() => Guard.IsNotNullOrEmpty(null, "Guard.IsNotNullOrEmpty(null) Fail"));
        _ = Assert.ThrowsException<ArgumentNullException>(() => FGuard.IsNotNullOrEmpty(null, "FGuard.IsNotNullOrEmpty(null) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNotNullOrEmpty(string.Empty, "Guard.IsNotNullOrEmpty(empty) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNotNullOrEmpty(string.Empty, "FGuard.IsNotNullOrEmpty(empty) Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNullOrWhiteSpace_Ok()
    {
        Guard.IsNullOrWhiteSpace(null, "Guard.IsNullOrWhiteSpace(null)");
        Assert.IsNull(FGuard.IsNullOrWhiteSpace(null, "FGuard.IsNullOrWhiteSpace(null)"));
        Guard.IsNullOrWhiteSpace(" \t ", "Guard.IsNullOrWhiteSpace(WhiteSpace)");
        Assert.AreSame(" \t ", FGuard.IsNullOrWhiteSpace(" \t ", "FGuard.IsNullOrWhiteSpace(WhiteSpace)"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNullOrWhiteSpace("Hello", "Guard.IsNullOrWhiteSpace Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNullOrWhiteSpace("Hello", "FGuard.IsNullOrWhiteSpace Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNotNullOrWhiteSpace()
    {
        Guard.IsNotNullOrWhiteSpace("Hello", "Guard.IsNotNullOrWhiteSpace");
        Assert.AreSame("Hello", FGuard.IsNotNullOrWhiteSpace("Hello", "FGuard.IsNotNullOrWhiteSpace"));

        _ = Assert.ThrowsException<ArgumentNullException>(() => Guard.IsNotNullOrWhiteSpace(null, "Guard.IsNotNullOrWhiteSpace(null) Fail"));
        _ = Assert.ThrowsException<ArgumentNullException>(() => FGuard.IsNotNullOrWhiteSpace(null, "FGuard.IsNotNullOrWhiteSpace(null) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNotNullOrWhiteSpace(" \t ", "Guard.IsNotNullOrWhiteSpace(WhiteSpace) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNotNullOrWhiteSpace(" \t ", "FGuard.IsNotNullOrWhiteSpace(WhiteSpace) Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsEmpty()
    {
        Guard.IsEmpty(string.Empty, "Guard.IsEmpty");
        Assert.AreSame(string.Empty, FGuard.IsEmpty(string.Empty, "FGuard.IsEmpty"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsEmpty("Hello", "Guard.IsEmpty Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsEmpty("Hello", "FGuard.IsEmpty Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNotEmpty()
    {
        Guard.IsNotEmpty("Hello", "Guard.IsNotEmpty");
        Assert.AreSame("Hello", FGuard.IsNotEmpty("Hello", "FGuard.IsNotEmpty"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNotEmpty(string.Empty, "Guard.IsNotEmpty Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNotEmpty(string.Empty, "FGuard.IsNotEmpty Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsWhiteSpace()
    {
        Guard.IsWhiteSpace(" \t ", "Guard.IsWhiteSpace");
        Assert.AreSame(" \t ", FGuard.IsWhiteSpace(" \t ", "FGuard.IsWhiteSpace"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsWhiteSpace("Hello", "Guard.IsWhiteSpace Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsWhiteSpace("Hello", "FGuard.IsWhiteSpace Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNotWhiteSpace()
    {
        Guard.IsNotWhiteSpace("Hello", "Guard.IsNotWhiteSpace");
        Assert.AreSame("Hello", FGuard.IsNotWhiteSpace("Hello", "FGuard.IsNotWhiteSpace"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.IsNotWhiteSpace(" \t ", "Guard.IsNotWhiteSpace Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.IsNotWhiteSpace(" \t ", "FGuard.IsNotWhiteSpace Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_HasSizeEqualTo()
    {
        Guard.HasSizeEqualTo("Hello", 5, "Guard.HasSizeEqualTo(int)");
        Assert.AreSame("Hello", FGuard.HasSizeEqualTo("Hello", 5, "FGuard.HasSizeEqualTo(int)"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeEqualTo("Hello", 4, "Guard.HasSizeEqualTo(4) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeEqualTo("Hello", 4, "FGuard.HasSizeEqualTo(4) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeEqualTo("Hello", 6, "Guard.HasSizeEqualTo(6) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeEqualTo("Hello", 6, "FGuard.HasSizeEqualTo(6) Fail"));

        Guard.HasSizeEqualTo("Hello", " Hi! ", "Guard.HasSizeEqualTo(string)");
        Assert.AreSame("Hello", FGuard.HasSizeEqualTo("Hello", " Hi! ", "FGuard.HasSizeEqualTo(string)"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeEqualTo("Hello", " Hi ", "Guard.HasSizeEqualTo( Hi ) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeEqualTo("Hello", " Hi ", "FGuard.HasSizeEqualTo( Hi ) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeEqualTo("Hello", "Hello!", "Guard.HasSizeEqualTo(Hello!) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeEqualTo("Hello", "Hello!", "FGuard.HasSizeEqualTo(Hello!) Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_HasSizeNotEqualTo()
    {
        Guard.HasSizeNotEqualTo("Hello", 4, "Guard.HasSizeNotEqualTo(4)");
        Assert.AreSame("Hello", FGuard.HasSizeNotEqualTo("Hello", 4, "FGuard.HasSizeNotEqualTo(4)"));
        Guard.HasSizeNotEqualTo("Hello", 6, "Guard.HasSizeNotEqualTo(6)");
        Assert.AreSame("Hello", FGuard.HasSizeNotEqualTo("Hello", 6, "FGuard.HasSizeNotEqualTo(6)"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeNotEqualTo("Hello", 5, "Guard.HasSizeNotEqualTo Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeNotEqualTo("Hello", 5, "FGuard.HasSizeNotEqualTo Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_HasSizeGreaterThan()
    {
        Guard.HasSizeGreaterThan("Hello", int.MinValue, "Guard.HasSizeGreaterThan(MinValue)");
        Assert.AreSame("Hello", FGuard.HasSizeGreaterThan("Hello", int.MinValue, "FGuard.HasSizeGreaterThan(MinValue)"));
        Guard.HasSizeGreaterThan("Hello", -1, "Guard.HasSizeGreaterThan(-1)");
        Assert.AreSame("Hello", FGuard.HasSizeGreaterThan("Hello", -1, "FGuard.HasSizeGreaterThan(-1)"));
        Guard.HasSizeGreaterThan("Hello", 3, "Guard.HasSizeGreaterThan(3)");
        Assert.AreSame("Hello", FGuard.HasSizeGreaterThan("Hello", 3, "FGuard.HasSizeGreaterThan(3)"));
        Guard.HasSizeGreaterThan("Hello", 4, "Guard.HasSizeGreaterThan(4)");
        Assert.AreSame("Hello", FGuard.HasSizeGreaterThan("Hello", 4, "FGuard.HasSizeGreaterThan(4)"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeGreaterThan("Hello", 5, "Guard.HasSizeGreaterThan(5) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeGreaterThan("Hello", 5, "FGuard.HasSizeGreaterThan(5) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeGreaterThan("Hello", 6, "Guard.HasSizeGreaterThan(6) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeGreaterThan("Hello", 6, "FGuard.HasSizeGreaterThan(6) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeGreaterThan("Hello", int.MaxValue, "Guard.HasSizeGreaterThan(MaxValue) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeGreaterThan("Hello", int.MaxValue, "FGuard.HasSizeGreaterThan(MaxValue) Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_HasSizeGreaterThanOrEqualTo()
    {
        Guard.HasSizeGreaterThanOrEqualTo("Hello", int.MinValue, "Guard.HasSizeGreaterThanOrEqualTo");
        Assert.AreSame("Hello", FGuard.HasSizeGreaterThanOrEqualTo("Hello", int.MinValue, "FGuard.HasSizeGreaterThanOrEqualTo"));
        Guard.HasSizeGreaterThanOrEqualTo("Hello", -1, "Guard.HasSizeGreaterThanOrEqualTo");
        Assert.AreSame("Hello", FGuard.HasSizeGreaterThanOrEqualTo("Hello", -1, "FGuard.HasSizeGreaterThanOrEqualTo"));
        Guard.HasSizeGreaterThanOrEqualTo("Hello", 4, "Guard.HasSizeGreaterThanOrEqualTo");
        Assert.AreSame("Hello", FGuard.HasSizeGreaterThanOrEqualTo("Hello", 4, "FGuard.HasSizeGreaterThanOrEqualTo"));
        Guard.HasSizeGreaterThanOrEqualTo("Hello", 5, "Guard.HasSizeGreaterThanOrEqualTo");
        Assert.AreSame("Hello", FGuard.HasSizeGreaterThanOrEqualTo("Hello", 5, "FGuard.HasSizeGreaterThanOrEqualTo"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeGreaterThanOrEqualTo("Hello", 6, "Guard.HasSizeGreaterThanOrEqualTo Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeGreaterThanOrEqualTo("Hello", 6, "FGuard.HasSizeGreaterThanOrEqualTo Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeGreaterThanOrEqualTo("Hello", 7, "Guard.HasSizeGreaterThanOrEqualTo Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeGreaterThanOrEqualTo("Hello", 7, "FGuard.HasSizeGreaterThanOrEqualTo Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeGreaterThanOrEqualTo("Hello", int.MaxValue, "Guard.HasSizeGreaterThanOrEqualTo Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeGreaterThanOrEqualTo("Hello", int.MaxValue, "FGuard.HasSizeGreaterThanOrEqualTo Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_HasSizeLessThan()
    {
        Guard.HasSizeLessThan("Hello", 6, "Guard.HasSizeLessThan");
        Assert.AreSame("Hello", FGuard.HasSizeLessThan("Hello", 6, "FGuard.HasSizeLessThan"));
        Guard.HasSizeLessThan("Hello", 7, "Guard.HasSizeLessThan");
        Assert.AreSame("Hello", FGuard.HasSizeLessThan("Hello", 7, "FGuard.HasSizeLessThan"));
        Guard.HasSizeLessThan("Hello", int.MaxValue, "Guard.HasSizeLessThan");
        Assert.AreSame("Hello", FGuard.HasSizeLessThan("Hello", int.MaxValue, "FGuard.HasSizeLessThan"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThan("Hello", 5, "Guard.HasSizeLessThan Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThan("Hello", 5, "FGuard.HasSizeLessThan Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThan("Hello", 4, "Guard.HasSizeLessThan Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThan("Hello", 4, "FGuard.HasSizeLessThan Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThan("Hello", -1, "Guard.HasSizeLessThan Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThan("Hello", -1, "FGuard.HasSizeLessThan Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThan("Hello", int.MinValue, "Guard.HasSizeLessThan Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThan("Hello", int.MinValue, "FGuard.HasSizeLessThan Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_HasSizeLessThanOrEqualTo()
    {
        Guard.HasSizeLessThanOrEqualTo("Hello", 5, "Guard.HasSizeLessThanOrEqualTo(int)");
        Assert.AreSame("Hello", FGuard.HasSizeLessThanOrEqualTo("Hello", 5, "FGuard.HasSizeLessThanOrEqualTo(int)"));
        Guard.HasSizeLessThanOrEqualTo("Hello", 6, "Guard.HasSizeLessThanOrEqualTo(int)");
        Assert.AreSame("Hello", FGuard.HasSizeLessThanOrEqualTo("Hello", 6, "FGuard.HasSizeLessThanOrEqualTo(int)"));
        Guard.HasSizeLessThanOrEqualTo("Hello", int.MaxValue, "Guard.HasSizeLessThanOrEqualTo(int)");
        Assert.AreSame("Hello", FGuard.HasSizeLessThanOrEqualTo("Hello", int.MaxValue, "FGuard.HasSizeLessThanOrEqualTo(int)"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThanOrEqualTo("Hello", 4, "Guard.HasSizeLessThanOrEqualTo(int) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThanOrEqualTo("Hello", 4, "FGuard.HasSizeLessThanOrEqualTo(int) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThanOrEqualTo("Hello", 3, "Guard.HasSizeLessThanOrEqualTo(int) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThanOrEqualTo("Hello", 3, "FGuard.HasSizeLessThanOrEqualTo(int) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThanOrEqualTo("Hello", -1, "Guard.HasSizeLessThanOrEqualTo(int) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThanOrEqualTo("Hello", -1, "FGuard.HasSizeLessThanOrEqualTo(int) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThanOrEqualTo("Hello", int.MinValue, "Guard.HasSizeLessThanOrEqualTo(int) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThanOrEqualTo("Hello", int.MinValue, "FGuard.HasSizeLessThanOrEqualTo(int) Fail"));

        Guard.HasSizeLessThanOrEqualTo("Hello", " Hi! ", "Guard.HasSizeLessThanOrEqualTo(string)");
        Assert.AreSame("Hello", FGuard.HasSizeLessThanOrEqualTo("Hello", " Hi! ", "FGuard.HasSizeLessThanOrEqualTo(string)"));
        Guard.HasSizeLessThanOrEqualTo("Hello", "Hello!", "Guard.HasSizeLessThanOrEqualTo(string)");
        Assert.AreSame("Hello", FGuard.HasSizeLessThanOrEqualTo("Hello", "Hello!", "FGuard.HasSizeLessThanOrEqualTo(string)"));

        _ = Assert.ThrowsException<ArgumentException>(() => Guard.HasSizeLessThanOrEqualTo("Hello", " Hi ", "Guard.HasSizeLessThanOrEqualTo(string) Fail"));
        _ = Assert.ThrowsException<ArgumentException>(() => FGuard.HasSizeLessThanOrEqualTo("Hello", " Hi ", "FGuard.HasSizeLessThanOrEqualTo(string) Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsInRangeFor()
    {
        Guard.IsInRangeFor(0, "Hello", "Guard.IsInRangeFor(0)");
        Assert.AreEqual(0, FGuard.IsInRangeFor(0, "Hello", "FGuard.IsInRangeFor(0)"));
        Guard.IsInRangeFor(4, "Hello", "Guard.IsInRangeFor(4)");
        Assert.AreEqual(4, FGuard.IsInRangeFor(4, "Hello", "FGuard.IsInRangeFor(4)"));

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Guard.IsInRangeFor(int.MinValue, "Hello", "Guard.IsInRangeFor(MinValue) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => FGuard.IsInRangeFor(int.MinValue, "Hello", "FGuard.IsInRangeFor(MinValue) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Guard.IsInRangeFor(-1, "Hello", "Guard.IsInRangeFor(-1) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => FGuard.IsInRangeFor(-1, "Hello", "FGuard.IsInRangeFor(-1) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Guard.IsInRangeFor(5, "Hello", "Guard.IsInRangeFor(5) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => FGuard.IsInRangeFor(5, "Hello", "FGuard.IsInRangeFor(5) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Guard.IsInRangeFor(int.MaxValue, "Hello", "Guard.IsInRangeFor(MaxValue) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => FGuard.IsInRangeFor(int.MaxValue, "Hello", "FGuard.IsInRangeFor(MaxValue) Fail"));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_IsNotInRangeFor()
    {
        Guard.IsNotInRangeFor(int.MinValue, "Hello", "Guard.IsNotInRangeFor(MinValue)");
        Assert.AreEqual(int.MinValue, FGuard.IsNotInRangeFor(int.MinValue, "Hello", "FGuard.IsNotInRangeFor(MinValue)"));
        Guard.IsNotInRangeFor(-1, "Hello", "Guard.IsNotInRangeFor(-1)");
        Assert.AreEqual(-1, FGuard.IsNotInRangeFor(-1, "Hello", "FGuard.IsNotInRangeFor(-1)"));
        Guard.IsNotInRangeFor(5, "Hello", "Guard.IsNotInRangeFor(5)");
        Assert.AreEqual(5, FGuard.IsNotInRangeFor(5, "Hello", "FGuard.IsNotInRangeFor(5)"));
        Guard.IsNotInRangeFor(int.MaxValue, "Hello", "Guard.IsNotInRangeFor(MaxValue)");
        Assert.AreEqual(int.MaxValue, FGuard.IsNotInRangeFor(int.MaxValue, "Hello", "FGuard.IsNotInRangeFor(MaxValue)"));

        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Guard.IsNotInRangeFor(0, "Hello", "Guard.IsNotInRangeFor(0) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Assert.AreEqual(0, FGuard.IsNotInRangeFor(0, "Hello", "FGuard.IsNotInRangeFor(0) Fail")));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Guard.IsNotInRangeFor(4, "Hello", "Guard.IsNotInRangeFor(4) Fail"));
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Assert.AreEqual(4, FGuard.IsNotInRangeFor(4, "Hello", "FGuard.IsNotInRangeFor(4) Fail")));
    }

    [TestMethod]
    public void Test_Guard_IsEqualTo_Ok()
    {
        Guard.IsEqualTo("Hello", "Hello", nameof(Test_Guard_IsEqualTo_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsEqualTo_Fail()
    {
        Guard.IsEqualTo("Hello", "World", nameof(Test_Guard_IsEqualTo_Fail));
    }

    [TestMethod]
    public void Test_Guard_IsNotEqualTo_Ok()
    {
        Guard.IsNotEqualTo("Hello", "World", nameof(Test_Guard_IsNotEqualTo_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsNotEqualTo_Fail()
    {
        Guard.IsNotEqualTo("Hello", "Hello", nameof(Test_Guard_IsNotEqualTo_Fail));
    }

    [TestMethod]
    public void Test_Guard_IsBitwiseEqualTo_Ok()
    {
        Guard.IsBitwiseEqualTo(byte.MaxValue, byte.MaxValue, nameof(Test_Guard_IsBitwiseEqualTo_Ok));
        Guard.IsBitwiseEqualTo((float)Math.PI, (float)Math.PI, nameof(Test_Guard_IsBitwiseEqualTo_Ok));
        Guard.IsBitwiseEqualTo(double.Epsilon, double.Epsilon, nameof(Test_Guard_IsBitwiseEqualTo_Ok));

        Guid guid = Guid.NewGuid();
        Guard.IsBitwiseEqualTo(guid, guid, nameof(Test_Guard_IsBitwiseEqualTo_Ok));

        // tests the >16 byte case where the loop is called
        BiggerThanLimit biggerThanLimit = new(0, 3, ulong.MaxValue, ulong.MinValue);
        Guard.IsBitwiseEqualTo(biggerThanLimit, biggerThanLimit, nameof(Test_Guard_IsBitwiseEqualTo_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsBitwiseEqualTo_Size8Fail()
    {
        Guard.IsBitwiseEqualTo(double.PositiveInfinity, double.Epsilon, nameof(Test_Guard_IsBitwiseEqualTo_Size8Fail));
        Guard.IsBitwiseEqualTo(DateTime.Now, DateTime.Today, nameof(Test_Guard_IsBitwiseEqualTo_Size8Fail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsBitwiseEqualTo_Size16Fail()
    {
        Guard.IsBitwiseEqualTo(decimal.MaxValue, decimal.MinusOne, nameof(Test_Guard_IsBitwiseEqualTo_Size16Fail));
        Guard.IsBitwiseEqualTo(Guid.NewGuid(), Guid.NewGuid(), nameof(Test_Guard_IsBitwiseEqualTo_Size16Fail));
    }

    // a >16 byte struct for testing IsBitwiseEqual's pathway for >16 byte types
    private struct BiggerThanLimit
    {
        public BiggerThanLimit(ulong a, ulong b, ulong c, ulong d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public ulong A;

        public ulong B;

        public ulong C;

        public ulong D;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsBitwiseEqualTo_SequenceEqualFail()
    {
        // tests the >16 byte case where the loop is called
        BiggerThanLimit biggerThanLimit0 = new(0, 3, ulong.MaxValue, ulong.MinValue);
        BiggerThanLimit biggerThanLimit1 = new(long.MaxValue + 1UL, 99, ulong.MaxValue ^ 0xF7UL, ulong.MinValue ^ 5555UL);

        Guard.IsBitwiseEqualTo(biggerThanLimit0, biggerThanLimit1, nameof(Test_Guard_IsBitwiseEqualTo_SequenceEqualFail));
    }

    [TestMethod]
    public void Test_Guard_IsReferenceEqualTo_Ok()
    {
        object? obj = new();

        Guard.IsReferenceEqualTo(obj, obj, nameof(Test_Guard_IsReferenceEqualTo_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsReferenceEqualTo_Fail()
    {
        Guard.IsReferenceEqualTo(new object(), new object(), nameof(Test_Guard_IsReferenceEqualTo_Fail));
    }

    [TestMethod]
    public void Test_Guard_IsReferenceNotEqualTo_Ok()
    {
        Guard.IsReferenceNotEqualTo(new object(), new object(), nameof(Test_Guard_IsReferenceEqualTo_Fail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsReferenceNotEqualTo_Fail()
    {
        object? obj = new();

        Guard.IsReferenceNotEqualTo(obj, obj, nameof(Test_Guard_IsReferenceEqualTo_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsTrue_Ok()
    {
        Guard.IsTrue(true, nameof(Test_Guard_IsTrue_Ok));
        Guard.IsTrue(true, nameof(Test_Guard_IsTrue_Ok), "Hello world");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsTrue_Fail()
    {
        Guard.IsTrue(false, nameof(Test_Guard_IsTrue_Fail));
    }

    [TestMethod]
    public void Test_Guard_IsTrue_Fail_WithMessage()
    {
        try
        {
            Guard.IsTrue(false, nameof(Test_Guard_IsTrue_Fail_WithMessage), "Hello world");
        }
        catch (ArgumentException e)
        {
            Assert.IsTrue(e.Message.Contains("\"Hello world\""));

            return;
        }

        // Compiler detects this is unreachable from attribute,
        // but we leave the assertion to double check that's valid
        Assert.Fail();
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_Guard_IsTrue_WithHandler_Ok()
    {
        Guard.IsTrue(true, nameof(Test_Guard_IsTrue_Ok), $"This is an interpolated message: {DateTime.Now.Year}, {"hello".AsSpan()}");
    }

    [TestMethod]
    public void Test_Guard_IsTrue_WithHandler_Fail()
    {
        try
        {
            Guard.IsTrue(false, nameof(Test_Guard_IsTrue_WithHandler_Fail), $"This is an interpolated message: {DateTime.Now.Year}, {"hello".AsSpan()}");
        }
        catch (ArgumentException e)
        {
            Assert.IsTrue(e.Message.Contains($"This is an interpolated message: {DateTime.Now.Year}, {"hello".AsSpan()}"));

            return;
        }

        // Compiler detects this is unreachable from attribute,
        // but we leave the assertion to double check that's valid
        Assert.Fail();
    }
#endif

    [TestMethod]
    public void Test_Guard_IsFalse_Ok()
    {
        Guard.IsFalse(false, nameof(Test_Guard_IsFalse_Ok));
        Guard.IsFalse(false, nameof(Test_Guard_IsFalse_Ok), "Hello world");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_Guard_IsFalse_Fail()
    {
        Guard.IsFalse(true, nameof(Test_Guard_IsFalse_Fail));
    }

    [TestMethod]
    public void Test_Guard_IsFalse_Fail_WithMessage()
    {
        try
        {
            Guard.IsFalse(true, nameof(Test_Guard_IsFalse_Fail_WithMessage), "Hello world");
        }
        catch (ArgumentException e)
        {
            Assert.IsTrue(e.Message.Contains("\"Hello world\""));

            return;
        }

        Assert.Fail();
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_Guard_IsFalse_WithHandler_Ok()
    {
        Guard.IsFalse(false, nameof(Test_Guard_IsFalse_WithHandler_Ok), $"This is an interpolated message: {DateTime.Now.Year}, {"hello".AsSpan()}");
    }

    [TestMethod]
    public void Test_Guard_IsFalse_WithHandler_Fail()
    {
        try
        {
            Guard.IsFalse(true, nameof(Test_Guard_IsFalse_WithHandler_Fail), $"This is an interpolated message: {DateTime.Now.Year}, {"hello".AsSpan()}");
        }
        catch (ArgumentException e)
        {
            Assert.IsTrue(e.Message.Contains($"This is an interpolated message: {DateTime.Now.Year}, {"hello".AsSpan()}"));

            return;
        }

        // Compiler detects this is unreachable from attribute,
        // but we leave the assertion to double check that's valid
        Assert.Fail();
    }
#endif

    [TestMethod]
    public void Test_Guard_IsLessThan_Ok()
    {
        Guard.IsLessThan(1, 2, nameof(Test_Guard_IsLessThan_Ok));
        Guard.IsLessThan(1.2f, 3.14f, nameof(Test_Guard_IsLessThan_Ok));
        Guard.IsLessThan(DateTime.Now, DateTime.MaxValue, nameof(Test_Guard_IsLessThan_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsLessThan_EqualsFalse()
    {
        Guard.IsLessThan(1, 1, nameof(Test_Guard_IsLessThan_EqualsFalse));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsLessThan_GreaterFalse()
    {
        Guard.IsLessThan(2, 1, nameof(Test_Guard_IsLessThan_GreaterFalse));
    }

    [TestMethod]
    public void Test_Guard_IsLessThanOrEqualTo_Ok()
    {
        Guard.IsLessThanOrEqualTo(1, 2, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
        Guard.IsLessThanOrEqualTo(1, 1, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
        Guard.IsLessThanOrEqualTo(0.1f, (float)Math.PI, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
        Guard.IsLessThanOrEqualTo((float)Math.PI, (float)Math.PI, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
        Guard.IsLessThanOrEqualTo(DateTime.Today, DateTime.MaxValue, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
        Guard.IsLessThanOrEqualTo(DateTime.MaxValue, DateTime.MaxValue, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsLessThanOrEqualTo_False()
    {
        Guard.IsLessThanOrEqualTo(2, 1, nameof(Test_Guard_IsLessThanOrEqualTo_False));
    }

    [TestMethod]
    public void Test_Guard_IsGreaterThan_Ok()
    {
        Guard.IsGreaterThan(2, 1, nameof(Test_Guard_IsGreaterThan_Ok));
        Guard.IsGreaterThan(3.14f, 2.1f, nameof(Test_Guard_IsGreaterThan_Ok));
        Guard.IsGreaterThan(DateTime.MaxValue, DateTime.Today, nameof(Test_Guard_IsGreaterThan_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsGreaterThan_EqualsFalse()
    {
        Guard.IsGreaterThan(1, 1, nameof(Test_Guard_IsGreaterThan_EqualsFalse));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsGreaterThan_LowerFalse()
    {
        Guard.IsGreaterThan(1, 2, nameof(Test_Guard_IsGreaterThan_LowerFalse));
    }

    [TestMethod]
    public void Test_Guard_IsGreaterThanOrEqualTo_Ok()
    {
        Guard.IsGreaterThanOrEqualTo(2, 1, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
        Guard.IsGreaterThanOrEqualTo(1, 1, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
        Guard.IsGreaterThanOrEqualTo((float)Math.PI, 1, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
        Guard.IsGreaterThanOrEqualTo((float)Math.PI, (float)Math.PI, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
        Guard.IsGreaterThanOrEqualTo(DateTime.MaxValue, DateTime.Today, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
        Guard.IsGreaterThanOrEqualTo(DateTime.MaxValue, DateTime.MaxValue, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsGreaterThanOrEqualTo_False()
    {
        Guard.IsGreaterThanOrEqualTo(1, 2, nameof(Test_Guard_IsGreaterThanOrEqualTo_False));
    }

    [TestMethod]
    public void Test_Guard_IsInRange_Ok()
    {
        Guard.IsInRange(1, 0, 4, nameof(Test_Guard_IsInRange_Ok));
        Guard.IsInRange(0, 0, 2, nameof(Test_Guard_IsInRange_Ok));
        Guard.IsInRange(3.14f, 0, 10, nameof(Test_Guard_IsInRange_Ok));
        Guard.IsInRange(1, 0, 3.14f, nameof(Test_Guard_IsInRange_Ok));
        Guard.IsInRange(1, -50, 2, nameof(Test_Guard_IsInRange_Ok));
        Guard.IsInRange(-44, -44, 0, nameof(Test_Guard_IsInRange_Ok));
        Guard.IsInRange(3.14f, -float.Epsilon, 22, nameof(Test_Guard_IsInRange_Ok));
        Guard.IsInRange(1, int.MinValue, int.MaxValue, nameof(Test_Guard_IsInRange_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsInRange_LowerFail()
    {
        Guard.IsInRange(-3, 0, 4, nameof(Test_Guard_IsInRange_LowerFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsInRange_EqualFail()
    {
        Guard.IsInRange(0, 4, 4, nameof(Test_Guard_IsInRange_EqualFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsInRange_HigherFail()
    {
        Guard.IsInRange(0, 20, 4, nameof(Test_Guard_IsInRange_HigherFail));
    }

    [TestMethod]
    public void Test_Guard_IsNotInRange_Ok()
    {
        Guard.IsNotInRange(0, 4, 10, nameof(Test_Guard_IsNotInRange_Ok));
        Guard.IsNotInRange(-4, 0, 2, nameof(Test_Guard_IsNotInRange_Ok));
        Guard.IsNotInRange(12f, 0, 10, nameof(Test_Guard_IsNotInRange_Ok));
        Guard.IsNotInRange(-1, 0, 3.14f, nameof(Test_Guard_IsNotInRange_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsNotInRange_LowerEqualFail()
    {
        Guard.IsNotInRange(0, 0, 4, nameof(Test_Guard_IsNotInRange_LowerEqualFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsNotInRange_InnerFail()
    {
        Guard.IsNotInRange(2, 0, 4, nameof(Test_Guard_IsNotInRange_InnerFail));
    }

    [TestMethod]
    public void Test_Guard_IsInRangeFor_Ok()
    {
        Span<int> span = stackalloc int[10];

        Guard.IsInRangeFor(0, span, nameof(Test_Guard_IsInRangeFor_Ok));
        Guard.IsInRangeFor(4, span, nameof(Test_Guard_IsInRangeFor_Ok));
        Guard.IsInRangeFor(9, span, nameof(Test_Guard_IsInRangeFor_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsInRangeFor_LowerFail()
    {
        Span<int> span = stackalloc int[10];

        Guard.IsInRangeFor(-2, span, nameof(Test_Guard_IsInRangeFor_LowerFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsInRangeFor_EqualFail()
    {
        Span<int> span = stackalloc int[10];

        Guard.IsInRangeFor(10, span, nameof(Test_Guard_IsInRangeFor_EqualFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsInRangeFor_HigherFail()
    {
        Span<int> span = stackalloc int[10];

        Guard.IsInRangeFor(99, span, nameof(Test_Guard_IsInRangeFor_HigherFail));
    }

    [TestMethod]
    public void Test_Guard_IsNotInRangeFor_Ok()
    {
        Span<int> span = stackalloc int[10];

        Guard.IsNotInRangeFor(-2, span, nameof(Test_Guard_IsNotInRangeFor_Ok));
        Guard.IsNotInRangeFor(10, span, nameof(Test_Guard_IsNotInRangeFor_Ok));
        Guard.IsNotInRangeFor(2222, span, nameof(Test_Guard_IsNotInRangeFor_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsNotInRangeFor_LowerFail()
    {
        Span<int> span = stackalloc int[10];

        Guard.IsNotInRangeFor(0, span, nameof(Test_Guard_IsNotInRangeFor_LowerFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsNotInRangeFor_MiddleFail()
    {
        Span<int> span = stackalloc int[10];

        Guard.IsNotInRangeFor(6, span, nameof(Test_Guard_IsNotInRangeFor_MiddleFail));
    }

    [TestMethod]
    public void Test_Guard_IsBetween_Ok()
    {
        Guard.IsBetween(1, 0, 4, nameof(Test_Guard_IsBetween_Ok));
        Guard.IsBetween(3.14f, 0, 10, nameof(Test_Guard_IsBetween_Ok));
        Guard.IsBetween(1, 0, 3.14, nameof(Test_Guard_IsBetween_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsBetween_LowerFail()
    {
        Guard.IsBetween(-1, 0, 4, nameof(Test_Guard_IsBetween_LowerFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsBetween_EqualFail()
    {
        Guard.IsBetween(0, 0, 4, nameof(Test_Guard_IsBetween_EqualFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsBetween_HigherFail()
    {
        Guard.IsBetween(6, 0, 4, nameof(Test_Guard_IsBetween_HigherFail));
    }

    [TestMethod]
    public void Test_Guard_IsNotBetween_Ok()
    {
        Guard.IsNotBetween(0, 0, 4, nameof(Test_Guard_IsNotBetween_Ok));
        Guard.IsNotBetween(10, 0, 10, nameof(Test_Guard_IsNotBetween_Ok));
        Guard.IsNotBetween(-5, 0, 3.14, nameof(Test_Guard_IsNotBetween_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsNotBetween_Fail()
    {
        Guard.IsNotBetween(1, 0, 4, nameof(Test_Guard_IsNotBetween_Fail));
    }

    [TestMethod]
    public void Test_Guard_IsBetweenOrEqualTo_Ok()
    {
        Guard.IsBetweenOrEqualTo(1, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
        Guard.IsBetweenOrEqualTo(10, 0, 10, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
        Guard.IsBetweenOrEqualTo(1, 0, 3.14, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsBetweenOrEqualTo_LowerFail()
    {
        Guard.IsBetweenOrEqualTo(-1, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_LowerFail));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsBetweenOrEqualTo_HigherFail()
    {
        Guard.IsBetweenOrEqualTo(6, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_HigherFail));
    }

    [TestMethod]
    public void Test_Guard_IsNotBetweenOrEqualTo_Ok()
    {
        Guard.IsNotBetweenOrEqualTo(6, 0, 4, nameof(Test_Guard_IsNotBetweenOrEqualTo_Ok));
        Guard.IsNotBetweenOrEqualTo(-10, 0, 10, nameof(Test_Guard_IsNotBetweenOrEqualTo_Ok));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Guard_IsNotBetweenOrEqualTo_Fail()
    {
        Guard.IsNotBetweenOrEqualTo(3, 0, 4, nameof(Test_Guard_IsNotBetweenOrEqualTo_Fail));
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_CallerArgumentExpression_1()
    {
        string? thisStringShouldNotBeNull = null;

        try
        {
            Guard.IsNotNull(thisStringShouldNotBeNull);
        }
        catch (ArgumentNullException e)
        {
            Assert.AreEqual(e.ParamName, nameof(thisStringShouldNotBeNull));

            return;
        }

        Assert.Fail();
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_CallerArgumentExpression_3()
    {
        int[] thisArrayShouldNotBeShorterThan10 = Array.Empty<int>();

        try
        {
            Guard.HasSizeGreaterThanOrEqualTo(thisArrayShouldNotBeShorterThan10, 10);
        }
        catch (ArgumentException e)
        {
            Assert.AreEqual(e.ParamName, nameof(thisArrayShouldNotBeShorterThan10));

            return;
        }

        Assert.Fail();
    }
}

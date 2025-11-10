// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Diagnostics.UnitTests;

[TestClass]
public partial class Test_Guard
{
    [TestMethod]
    public void Test_Guard_IsNull_Ok()
    {
        Guard.IsNull<object>(null, nameof(Test_Guard_IsNull_Ok));
        Guard.IsNull<int>(null, nameof(Test_Guard_IsNull_Ok));

        static void Test<T>(T? obj)
        {
            Guard.IsNull(obj, nameof(Test_Guard_IsNull_Ok));
        }

        Test<string>(null);
        Test<int?>(null);
    }

    [TestMethod]
    public void Test_Guard_IsNull_ClassFail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsNull(new object(), nameof(Test_Guard_IsNull_ClassFail)));
    }

    [TestMethod]
    public void Test_Guard_IsNull_StructFail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsNull(7, nameof(Test_Guard_IsNull_StructFail)));
    }

    [TestMethod]
    public void Test_Guard_IsNull_GenericClassFail()
    {
        static void Test<T>(T? obj)
        {
            Guard.IsNull(obj, nameof(Test_Guard_IsNull_GenericClassFail));
        }

        _ = Assert.ThrowsExactly<ArgumentException>(() => Test("Hi!"));
    }

    [TestMethod]
    public void Test_Guard_IsNull_GenericStructFail()
    {
        static void Test<T>(T? obj)
        {
            Guard.IsNull(obj, nameof(Test_Guard_IsNull_GenericStructFail));
        }

        _ = Assert.ThrowsExactly<ArgumentException>(() => Test(42));
    }

    [TestMethod]
    public void Test_Guard_IsNotNull_Ok()
    {
        Guard.IsNotNull(new object(), nameof(Test_Guard_IsNotNull_Ok));
        Guard.IsNotNull(7, nameof(Test_Guard_IsNotNull_Ok));

        static void Test<T>(T? obj)
        {
            Guard.IsNotNull(obj, nameof(Test_Guard_IsNotNull_Ok));
        }

        Test("Hi!");
        Test(42);
        Test<int?>(42);
    }

    [TestMethod]
    public void Test_Guard_IsNotNull_ClassFail()
    {
        _ = Assert.ThrowsExactly<ArgumentNullException>(() => Guard.IsNotNull<object>(null, nameof(Test_Guard_IsNotNull_ClassFail)));
    }

    [TestMethod]
    public void Test_Guard_IsNotNull_StructFail()
    {
        _ = Assert.ThrowsExactly<ArgumentNullException>(() => Guard.IsNotNull<int>(null, nameof(Test_Guard_IsNotNull_StructFail)));
    }

    [TestMethod]
    public void Test_Guard_IsNotNull_GenericClassFail()
    {
        static void Test<T>(T? obj)
        {
            Guard.IsNotNull(obj, nameof(Test_Guard_IsNotNull_GenericClassFail));
        }

        _ = Assert.ThrowsExactly<ArgumentNullException>(() => Test<string>(null));
    }

    [TestMethod]
    public void Test_Guard_IsNotNull_GenericStructFail()
    {
        static void Test<T>(T? obj)
        {
            Guard.IsNotNull(obj, nameof(Test_Guard_IsNotNull_GenericClassFail));
        }

        _ = Assert.ThrowsExactly<ArgumentNullException>(() => Test<int?>(null));
    }

    [TestMethod]
    public void Test_Guard_IsOfT_Ok()
    {
        Guard.IsOfType<string>("Hello", nameof(Test_Guard_IsOfT_Ok));
        Guard.IsOfType<int>(7, nameof(Test_Guard_IsOfT_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsOfT_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsOfType<string>(7, nameof(Test_Guard_IsOfT_Fail)));
    }

    [TestMethod]
    public void Test_Guard_IsOfType_Ok()
    {
        Guard.IsOfType("Hello", typeof(string), nameof(Test_Guard_IsOfType_Ok));
        Guard.IsOfType(7, typeof(int), nameof(Test_Guard_IsOfType_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsOfType_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsOfType(7, typeof(string), nameof(Test_Guard_IsOfType_Fail)));
    }

    [TestMethod]
    public void Test_Guard_IsAssignableToT_Ok()
    {
        Guard.IsAssignableToType<string>("Hello", nameof(Test_Guard_IsAssignableToT_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsAssignableToT_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsAssignableToType<string>(7, nameof(Test_Guard_IsAssignableToT_Fail)));
    }

    [TestMethod]
    public void Test_Guard_IsAssignableToType_Ok()
    {
        Guard.IsAssignableToType("Hello", typeof(string), nameof(Test_Guard_IsAssignableToType_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsAssignableToType_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsAssignableToType(7, typeof(string), nameof(Test_Guard_IsAssignableToType_Fail)));
    }

    [TestMethod]
    public void Test_Guard_IsNullOrEmpty_Ok()
    {
        Guard.IsNullOrEmpty(null, nameof(Test_Guard_IsNullOrEmpty_Ok));
        Guard.IsNullOrEmpty(string.Empty, nameof(Test_Guard_IsNullOrEmpty_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsNullOrEmpty_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsNullOrEmpty("Hello", nameof(Test_Guard_IsNullOrEmpty_Fail)));
    }

    [TestMethod]
    public void Test_Guard_IsNotNullOrEmpty_Ok()
    {
        Guard.IsNotNullOrEmpty("Hello", nameof(Test_Guard_IsNotNullOrEmpty_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsNotNullOrEmpty_Null()
    {
        _ = Assert.ThrowsExactly<ArgumentNullException>(() => Guard.IsNotNullOrEmpty(null, nameof(Test_Guard_IsNotNullOrEmpty_Null)));
    }

    [TestMethod]
    public void Test_Guard_IsNotNullOrEmpty_Empty()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsNotNullOrEmpty(string.Empty, nameof(Test_Guard_IsNotNullOrEmpty_Empty)));
    }

    [TestMethod]
    public void Test_Guard_IsNotNullOrWhiteSpace_Ok()
    {
        Guard.IsNotNullOrWhiteSpace("Hello", nameof(Test_Guard_IsNotNullOrWhiteSpace_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsNotNullOrWhiteSpace_Null()
    {
        _ = Assert.ThrowsExactly<ArgumentNullException>(() => Guard.IsNotNullOrWhiteSpace(null, nameof(Test_Guard_IsNotNullOrWhiteSpace_Null)));
    }

    [TestMethod]
    public void Test_Guard_IsNotNullOrWhiteSpace_Empty()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsNotNullOrWhiteSpace("  ", nameof(Test_Guard_IsNotNullOrWhiteSpace_Empty)));
    }

    [TestMethod]
    public void Test_Guard_IsEqualTo_Ok()
    {
        Guard.IsEqualTo("Hello", "Hello", nameof(Test_Guard_IsEqualTo_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsEqualTo_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsEqualTo("Hello", "World", nameof(Test_Guard_IsEqualTo_Fail)));
    }

    [TestMethod]
    public void Test_Guard_IsNotEqualTo_Ok()
    {
        Guard.IsNotEqualTo("Hello", "World", nameof(Test_Guard_IsNotEqualTo_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsNotEqualTo_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsNotEqualTo("Hello", "Hello", nameof(Test_Guard_IsNotEqualTo_Fail)));
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
    public void Test_Guard_IsBitwiseEqualTo_Size8Fail_Double()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsBitwiseEqualTo(double.PositiveInfinity, double.Epsilon, nameof(Test_Guard_IsBitwiseEqualTo_Size8Fail_Double)));
    }

    [TestMethod]
    public void Test_Guard_IsBitwiseEqualTo_Size8Fail_DateTime()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsBitwiseEqualTo(DateTime.Now, DateTime.Today, nameof(Test_Guard_IsBitwiseEqualTo_Size8Fail_DateTime)));
    }

    [TestMethod]
    public void Test_Guard_IsBitwiseEqualTo_Size16Fail_Decimal()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsBitwiseEqualTo(decimal.MaxValue, decimal.MinusOne, nameof(Test_Guard_IsBitwiseEqualTo_Size16Fail_Decimal)));
    }

    [TestMethod]
    public void Test_Guard_IsBitwiseEqualTo_Size16Fail_Guid()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsBitwiseEqualTo(Guid.NewGuid(), Guid.NewGuid(), nameof(Test_Guard_IsBitwiseEqualTo_Size16Fail_Guid)));
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
    public void Test_Guard_IsBitwiseEqualTo_SequenceEqualFail()
    {
        // tests the >16 byte case where the loop is called
        BiggerThanLimit biggerThanLimit0 = new(0, 3, ulong.MaxValue, ulong.MinValue);
        BiggerThanLimit biggerThanLimit1 = new(long.MaxValue + 1UL, 99, ulong.MaxValue ^ 0xF7UL, ulong.MinValue ^ 5555UL);

        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsBitwiseEqualTo(biggerThanLimit0, biggerThanLimit1, nameof(Test_Guard_IsBitwiseEqualTo_SequenceEqualFail)));
    }

    [TestMethod]
    public void Test_Guard_IsReferenceEqualTo_Ok()
    {
        object? obj = new();

        Guard.IsReferenceEqualTo(obj, obj, nameof(Test_Guard_IsReferenceEqualTo_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsReferenceEqualTo_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsReferenceEqualTo(new object(), new object(), nameof(Test_Guard_IsReferenceEqualTo_Fail)));
    }

    [TestMethod]
    public void Test_Guard_IsReferenceNotEqualTo_Ok()
    {
        Guard.IsReferenceNotEqualTo(new object(), new object(), nameof(Test_Guard_IsReferenceEqualTo_Fail));
    }

    [TestMethod]
    public void Test_Guard_IsReferenceNotEqualTo_Fail()
    {
        object? obj = new();

        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsReferenceNotEqualTo(obj, obj, nameof(Test_Guard_IsReferenceEqualTo_Ok)));
    }

    [TestMethod]
    public void Test_Guard_IsTrue_Ok()
    {
        Guard.IsTrue(true, nameof(Test_Guard_IsTrue_Ok));
        Guard.IsTrue(true, nameof(Test_Guard_IsTrue_Ok), "Hello world");
    }

    [TestMethod]
    public void Test_Guard_IsTrue_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsTrue(false, nameof(Test_Guard_IsTrue_Fail)));
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
            Assert.Contains("\"Hello world\"", e.Message);

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
            Assert.Contains($"This is an interpolated message: {DateTime.Now.Year}, {"hello".AsSpan()}", e.Message);

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
    public void Test_Guard_IsFalse_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsFalse(true, nameof(Test_Guard_IsFalse_Fail)));
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
            Assert.Contains("\"Hello world\"", e.Message);

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
            Assert.Contains($"This is an interpolated message: {DateTime.Now.Year}, {"hello".AsSpan()}", e.Message);

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
    public void Test_Guard_IsLessThan_EqualsFalse()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsLessThan(1, 1, nameof(Test_Guard_IsLessThan_EqualsFalse)));
    }

    [TestMethod]
    public void Test_Guard_IsLessThan_GreaterFalse()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsLessThan(2, 1, nameof(Test_Guard_IsLessThan_GreaterFalse)));
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
    public void Test_Guard_IsLessThanOrEqualTo_False()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsLessThanOrEqualTo(2, 1, nameof(Test_Guard_IsLessThanOrEqualTo_False)));
    }

    [TestMethod]
    public void Test_Guard_IsGreaterThan_Ok()
    {
        Guard.IsGreaterThan(2, 1, nameof(Test_Guard_IsGreaterThan_Ok));
        Guard.IsGreaterThan(3.14f, 2.1f, nameof(Test_Guard_IsGreaterThan_Ok));
        Guard.IsGreaterThan(DateTime.MaxValue, DateTime.Today, nameof(Test_Guard_IsGreaterThan_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsGreaterThan_EqualsFalse()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsGreaterThan(1, 1, nameof(Test_Guard_IsGreaterThan_EqualsFalse)));
    }

    [TestMethod]
    public void Test_Guard_IsGreaterThan_LowerFalse()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsGreaterThan(1, 2, nameof(Test_Guard_IsGreaterThan_LowerFalse)));
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
    public void Test_Guard_IsGreaterThanOrEqualTo_False()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsGreaterThanOrEqualTo(1, 2, nameof(Test_Guard_IsGreaterThanOrEqualTo_False)));
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
    public void Test_Guard_IsInRange_LowerFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsInRange(-3, 0, 4, nameof(Test_Guard_IsInRange_LowerFail)));
    }

    [TestMethod]
    public void Test_Guard_IsInRange_EqualFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsInRange(0, 4, 4, nameof(Test_Guard_IsInRange_EqualFail)));
    }

    [TestMethod]
    public void Test_Guard_IsInRange_HigherFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsInRange(0, 20, 4, nameof(Test_Guard_IsInRange_HigherFail)));
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
    public void Test_Guard_IsNotInRange_LowerEqualFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsNotInRange(0, 0, 4, nameof(Test_Guard_IsNotInRange_LowerEqualFail)));
    }

    [TestMethod]
    public void Test_Guard_IsNotInRange_InnerFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsNotInRange(2, 0, 4, nameof(Test_Guard_IsNotInRange_InnerFail)));
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
    public void Test_Guard_IsInRangeFor_LowerFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            Span<int> span = stackalloc int[10];

            Guard.IsInRangeFor(-2, span, nameof(Test_Guard_IsInRangeFor_LowerFail));
        });
    }

    [TestMethod]
    public void Test_Guard_IsInRangeFor_EqualFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            Span<int> span = stackalloc int[10];

            Guard.IsInRangeFor(10, span, nameof(Test_Guard_IsInRangeFor_EqualFail));
        });
    }

    [TestMethod]
    public void Test_Guard_IsInRangeFor_HigherFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            Span<int> span = stackalloc int[10];

            Guard.IsInRangeFor(99, span, nameof(Test_Guard_IsInRangeFor_HigherFail));
        });
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
    public void Test_Guard_IsNotInRangeFor_LowerFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            Span<int> span = stackalloc int[10];

            Guard.IsNotInRangeFor(0, span, nameof(Test_Guard_IsNotInRangeFor_LowerFail));
        });
    }

    [TestMethod]
    public void Test_Guard_IsNotInRangeFor_MiddleFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            Span<int> span = stackalloc int[10];

            Guard.IsNotInRangeFor(6, span, nameof(Test_Guard_IsNotInRangeFor_MiddleFail));
        });
    }

    [TestMethod]
    public void Test_Guard_IsBetween_Ok()
    {
        Guard.IsBetween(1, 0, 4, nameof(Test_Guard_IsBetween_Ok));
        Guard.IsBetween(3.14f, 0, 10, nameof(Test_Guard_IsBetween_Ok));
        Guard.IsBetween(1, 0, 3.14, nameof(Test_Guard_IsBetween_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsBetween_LowerFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsBetween(-1, 0, 4, nameof(Test_Guard_IsBetween_LowerFail)));
    }

    [TestMethod]
    public void Test_Guard_IsBetween_EqualFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsBetween(0, 0, 4, nameof(Test_Guard_IsBetween_EqualFail)));
    }

    [TestMethod]
    public void Test_Guard_IsBetween_HigherFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsBetween(6, 0, 4, nameof(Test_Guard_IsBetween_HigherFail)));
    }

    [TestMethod]
    public void Test_Guard_IsNotBetween_Ok()
    {
        Guard.IsNotBetween(0, 0, 4, nameof(Test_Guard_IsNotBetween_Ok));
        Guard.IsNotBetween(10, 0, 10, nameof(Test_Guard_IsNotBetween_Ok));
        Guard.IsNotBetween(-5, 0, 3.14, nameof(Test_Guard_IsNotBetween_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsNotBetween_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsNotBetween(1, 0, 4, nameof(Test_Guard_IsNotBetween_Fail)));
    }

    [TestMethod]
    public void Test_Guard_IsBetweenOrEqualTo_Ok()
    {
        Guard.IsBetweenOrEqualTo(1, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
        Guard.IsBetweenOrEqualTo(10, 0, 10, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
        Guard.IsBetweenOrEqualTo(1, 0, 3.14, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsBetweenOrEqualTo_LowerFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsBetweenOrEqualTo(-1, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_LowerFail)));
    }

    [TestMethod]
    public void Test_Guard_IsBetweenOrEqualTo_HigherFail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsBetweenOrEqualTo(6, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_HigherFail)));
    }

    [TestMethod]
    public void Test_Guard_IsNotBetweenOrEqualTo_Ok()
    {
        Guard.IsNotBetweenOrEqualTo(6, 0, 4, nameof(Test_Guard_IsNotBetweenOrEqualTo_Ok));
        Guard.IsNotBetweenOrEqualTo(-10, 0, 10, nameof(Test_Guard_IsNotBetweenOrEqualTo_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsNotBetweenOrEqualTo_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Guard.IsNotBetweenOrEqualTo(3, 0, 4, nameof(Test_Guard_IsNotBetweenOrEqualTo_Fail)));
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
            Assert.AreEqual(nameof(thisStringShouldNotBeNull), e.ParamName);

            return;
        }

        Assert.Fail();
    }

    [TestCategory("Guard")]
    [TestMethod]
    public void Test_Guard_CallerArgumentExpression_2()
    {
        int thisIndexIsOutOfRange = 42;

        try
        {
            Guard.IsInRangeFor(thisIndexIsOutOfRange, "Hello world");
        }
        catch (ArgumentOutOfRangeException e)
        {
            Assert.AreEqual(nameof(thisIndexIsOutOfRange), e.ParamName);

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
            Assert.AreEqual(nameof(thisArrayShouldNotBeShorterThan10), e.ParamName);

            return;
        }

        Assert.Fail();
    }
}

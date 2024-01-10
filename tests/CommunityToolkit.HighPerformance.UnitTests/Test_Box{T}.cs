// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_BoxOfT
{
    [TestMethod]
    public void Test_BoxOfT_PrimitiveTypes()
    {
        Test(true, false);
        Test<byte>(27, 254);
        Test('a', '$');
        Test(4221124, 1241241);
        Test(3.14f, 2342.222f);
        Test(8394324ul, 1343431241ul);
        Test(184013.234324, 14124.23423);
    }

    [TestMethod]
    public void Test_BoxOfT_OtherTypes()
    {
        Test(DateTime.Now, DateTime.FromBinary(278091429014));
        Test(Guid.NewGuid(), Guid.NewGuid());
    }

    internal struct TestStruct : IEquatable<TestStruct>
    {
        public int Number;
        public char Character;
        public string Text;

        /// <inheritdoc/>
        public readonly bool Equals(TestStruct other)
        {
            return
                this.Number == other.Number &&
                this.Character == other.Character &&
                this.Text == other.Text;
        }
    }

    [TestMethod]
    public void Test_BoxOfT_CustomStruct()
    {
        TestStruct a = new() { Number = 42, Character = 'a', Text = "Hello" };
        TestStruct b = new() { Number = 38293, Character = 'z', Text = "World" };

        Test(a, b);
    }

    [TestMethod]
    public void Test_BoxOfT_Invalid()
    {
        long lValue = 0x0123_4567_89AB_CDEF;
        int iValue = Unsafe.As<long, int>(ref lValue);
        object lObj = lValue;

        bool success = Box<int>.TryGetFrom(lValue, out Box<int>? iBox);
        Assert.IsFalse(success);
        Assert.IsNull(iBox);

        _ = Assert.ThrowsException<InvalidCastException>(() => Box<int>.GetFrom(lObj));

        iBox = Box<int>.DangerousGetFrom(lObj);
        object iBoxObj = iBox;
        Assert.IsNotNull(iBox);
        Assert.IsNotNull(iBoxObj);
        Assert.AreEqual(iValue, iBox.GetReference());
        Assert.AreEqual(lValue.ToString(), iBox.ToString());
        Assert.AreEqual(lValue.ToString(), iBoxObj.ToString());
        Assert.AreEqual(lValue.GetHashCode(), iBox.GetHashCode());
        Assert.AreEqual(lValue.GetHashCode(), iBoxObj.GetHashCode());
        Assert.AreEqual(iValue, (int)iBox);
        Assert.AreEqual(lValue, (long)iBoxObj);
        _ = Assert.ThrowsException<InvalidCastException>(() => (int)iBoxObj);
        Assert.IsTrue(iBox.Equals(lValue));
        Assert.IsTrue(iValue.Equals(iBox));
        Assert.IsTrue(lValue.Equals((object)iBox));
        Assert.IsTrue(iBoxObj.Equals(lValue));
        Assert.IsTrue(lValue.Equals(iBoxObj));
    }

    /// <summary>
    /// Tests the <see cref="Box{T}"/> type for a given pair of values.
    /// </summary>
    /// <typeparam name="T">The type to test.</typeparam>
    /// <param name="value">The initial <typeparamref name="T"/> value.</param>
    /// <param name="test">The new <typeparamref name="T"/> value to assign and test.</param>
    private static void Test<T>(T value, T test)
        where T : struct, IEquatable<T>
    {
        Box<T>? box = value;
        object? boxObj = box;
        Assert.IsNotNull(box);
        Assert.AreEqual(value, box.GetReference());
        Assert.AreEqual(value.ToString(), box.ToString());
        Assert.AreEqual(value.ToString(), boxObj.ToString());
        Assert.AreEqual(value.GetHashCode(), box.GetHashCode());
        Assert.AreEqual(value.GetHashCode(), boxObj.GetHashCode());
        Assert.AreEqual(value, (T)box);
        Assert.AreEqual(value, (T)boxObj);
        Assert.IsTrue(box.Equals(value));
        Assert.IsTrue(value.Equals(box));
        Assert.IsTrue(boxObj.Equals(value));
        Assert.IsTrue(value.Equals(boxObj));

        object obj = value;
        bool success = Box<T>.TryGetFrom(obj, out box);
        boxObj = box;
        Assert.IsTrue(success);
        Assert.AreSame(obj, box);
        Assert.AreEqual(value, box!.GetReference());
        Assert.AreEqual(value.ToString(), box!.ToString());
        Assert.AreEqual(value.ToString(), boxObj!.ToString());
        Assert.AreEqual(value.GetHashCode(), box.GetHashCode());
        Assert.AreEqual(value.GetHashCode(), boxObj.GetHashCode());
        Assert.AreEqual(value, (T)box);
        Assert.AreEqual(value, (T)boxObj);
        Assert.IsTrue(box.Equals(value));
        Assert.IsTrue(value.Equals(box));
        Assert.IsTrue(boxObj.Equals(value));
        Assert.IsTrue(value.Equals(boxObj));

        box = Box<T>.GetFrom(obj);
        boxObj = box;
        Assert.AreSame(obj, box);
        Assert.AreEqual(value, box.GetReference());
        Assert.AreEqual(value.ToString(), box.ToString());
        Assert.AreEqual(value.ToString(), boxObj.ToString());
        Assert.AreEqual(value.GetHashCode(), box.GetHashCode());
        Assert.AreEqual(value.GetHashCode(), boxObj.GetHashCode());
        Assert.AreEqual(value, (T)box);
        Assert.AreEqual(value, (T)boxObj);
        Assert.IsTrue(box.Equals(value));
        Assert.IsTrue(value.Equals(box));
        Assert.IsTrue(boxObj.Equals(value));
        Assert.IsTrue(value.Equals(boxObj));

        box = Box<T>.DangerousGetFrom(obj);
        boxObj = box;
        Assert.AreSame(obj, box);
        Assert.AreEqual(value, box.GetReference());
        Assert.AreEqual(value.ToString(), box.ToString());
        Assert.AreEqual(value.ToString(), boxObj.ToString());
        Assert.AreEqual(value.GetHashCode(), box.GetHashCode());
        Assert.AreEqual(value.GetHashCode(), boxObj.GetHashCode());
        Assert.AreEqual(value, (T)box);
        Assert.AreEqual(value, (T)boxObj);
        Assert.IsTrue(box.Equals(value));
        Assert.IsTrue(value.Equals(box));
        Assert.IsTrue(boxObj.Equals(value));
        Assert.IsTrue(value.Equals(boxObj));

        box.GetReference() = test;
        Assert.AreEqual(test, box.GetReference());
        Assert.AreEqual(test.ToString(), box.ToString());
        Assert.AreEqual(test.ToString(), boxObj.ToString());
        Assert.AreEqual(test.GetHashCode(), box.GetHashCode());
        Assert.AreEqual(test.GetHashCode(), boxObj.GetHashCode());
        Assert.AreEqual(test, (T)box);
        Assert.AreEqual(test, (T)boxObj);
        Assert.IsTrue(box.Equals(test));
        Assert.IsTrue(test.Equals(box));
        Assert.IsTrue(boxObj.Equals(test));
        Assert.IsTrue(test.Equals(boxObj));

        // Testing that unboxing uses a fast process without unnecessary type-checking
        _ = (ValueTuple)Unsafe.As<Box<T>, Box<ValueTuple>>(ref box);
        _ = Unsafe.As<Box<T>, Box<ValueTuple>>(ref box).GetReference();
    }
}

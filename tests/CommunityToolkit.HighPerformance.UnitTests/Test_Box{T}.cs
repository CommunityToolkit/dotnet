// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
        public bool Equals(TestStruct other)
        {
            return
                this.Number == other.Number &&
                this.Character == other.Character &&
                this.Text == other.Text;
        }
    }

    [TestMethod]
    public void TestBoxOfT_CustomStruct()
    {
        TestStruct a = new() { Number = 42, Character = 'a', Text = "Hello" };
        TestStruct b = new() { Number = 38293, Character = 'z', Text = "World" };

        Test(a, b);
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

        Assert.AreEqual(value, box.GetReference());
        Assert.AreEqual(value.ToString(), box.ToString());
        Assert.AreEqual(value.GetHashCode(), box.GetHashCode());

        object obj = value;

        bool success = Box<T>.TryGetFrom(obj, out box);

        Assert.IsTrue(success);
        Assert.IsTrue(ReferenceEquals(obj, box));
        Assert.IsNotNull(box);
        Assert.AreEqual(value, box.GetReference());
        Assert.AreEqual(value.ToString(), box.ToString());
        Assert.AreEqual(value.GetHashCode(), box.GetHashCode());

        box = Box<T>.DangerousGetFrom(obj);

        Assert.IsTrue(ReferenceEquals(obj, box));
        Assert.AreEqual(value, box.GetReference());
        Assert.AreEqual(value.ToString(), box.ToString());
        Assert.AreEqual(value.GetHashCode(), box.GetHashCode());

        box.GetReference() = test;

        Assert.AreEqual(test, box.GetReference());
        Assert.AreEqual(test.ToString(), box.ToString());
        Assert.AreEqual(test.GetHashCode(), box.GetHashCode());
        Assert.AreEqual(obj, test);
    }
}

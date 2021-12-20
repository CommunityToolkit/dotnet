// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET6_0_OR_GREATER

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

[TestClass]
public class Test_NullableExtensions
{
    [TestMethod]
    public void Test_NullableExtensions_DangerousGetValueOrDefaultReference()
    {
        static void Test<T>(T before, T after)
            where T : struct
        {
            T? nullable = before;
            ref T reference = ref nullable.DangerousGetValueOrDefaultReference();

            Assert.AreEqual(nullable!.Value, before);

            reference = after;

            Assert.AreEqual(nullable.Value, after);
        }

        Test(0, 42);
        Test(1.3f, 3.14f);
        Test(0.555, 8.49);
        Test(Vector4.Zero, new Vector4(1, 5.55f, 2, 3.14f));
        Test(Matrix4x4.Identity, Matrix4x4.CreateOrthographic(35, 88.34f, 9.99f, 24.6f));
        Test(new[] { 1, 2 }.AsMemory(), new[] { 3, 4 }.AsMemory());
        Test(new[] { "a", "b" }.AsMemory(), new[] { "c", "d" }.AsMemory());
    }

    [TestMethod]
    public void Test_NullableExtensions_DangerousGetValueOrNullReference_HasValue()
    {
        static void Test<T>(T before, T after)
            where T : struct
        {
            T? nullable = before;
            ref T reference = ref nullable.DangerousGetValueOrNullReference();

            Assert.IsFalse(Unsafe.IsNullRef(ref reference));
            Assert.AreEqual(nullable!.Value, before);

            reference = after;

            Assert.AreEqual(nullable.Value, after);
        }

        Test(0, 42);
        Test(1.3f, 3.14f);
        Test(0.555, 8.49);
        Test(Vector4.Zero, new Vector4(1, 5.55f, 2, 3.14f));
        Test(Matrix4x4.Identity, Matrix4x4.CreateOrthographic(35, 88.34f, 9.99f, 24.6f));
        Test(new[] { 1, 2 }.AsMemory(), new[] { 3, 4 }.AsMemory());
        Test(new[] { "a", "b" }.AsMemory(), new[] { "c", "d" }.AsMemory());
    }

    [TestMethod]
    public void Test_NullableExtensions_DangerousGetValueOrNullReference_IsNull()
    {
        static void Test<T>()
            where T : struct
        {
            T? nullable = null;
            ref T reference = ref nullable.DangerousGetValueOrNullReference();

            Assert.IsTrue(Unsafe.IsNullRef(ref reference));
        }

        Test<int>();
        Test<float>();
        Test<double>();
        Test<Vector4>();
        Test<Matrix4x4>();
        Test<Memory<int>>();
        Test<Memory<string>>();
    }
}

#endif
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_RefOfT
{
    [TestMethod]
#if NETFRAMEWORK
    public void Test_RefOfT_CreateRefOfT()
    {
        FieldOwner model = new() { Value = 1 };
        Ref<int> reference = new(model, ref model.Value);

        Assert.IsTrue(Unsafe.AreSame(ref model.Value, ref reference.Value));

        reference.Value++;

        Assert.AreEqual(model.Value, 2);
    }

    /// <summary>
    /// A dummy model that owns an <see cref="int"/> field.
    /// </summary>
    private sealed class FieldOwner
    {
        public int Value;
    }
#else
    public void Test_RefOfT_CreateRefOfT()
    {
        int value = 1;
        Ref<int> reference = new(ref value);

        Assert.IsTrue(Unsafe.AreSame(ref value, ref reference.Value));

        reference.Value++;

        Assert.AreEqual(value, 2);
    }
#endif
}

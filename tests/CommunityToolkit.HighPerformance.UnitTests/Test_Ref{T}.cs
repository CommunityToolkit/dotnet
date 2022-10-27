// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET7_0_OR_GREATER

using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_RefOfT
{
    [TestMethod]
    public void Test_RefOfT_CreateRefOfT()
    {
        int value = 1;
        Ref<int> reference = new(ref value);

        Assert.IsTrue(Unsafe.AreSame(ref value, ref reference.Value));

        reference.Value++;

        Assert.AreEqual(value, 2);
    }
}

#endif
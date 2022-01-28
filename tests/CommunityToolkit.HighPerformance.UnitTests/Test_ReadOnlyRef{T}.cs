// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

#if NET7_0_OR_GREATER

using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_ReadOnlyRefOfT
{
    [TestMethod]
    public void Test_RefOfT_CreateRefOfT()
    {
        int value = 1;
        ReadOnlyRef<int> reference = new(value);

        Assert.IsTrue(Unsafe.AreSame(ref value, ref Unsafe.AsRef(reference.Value)));
    }
}

#endif
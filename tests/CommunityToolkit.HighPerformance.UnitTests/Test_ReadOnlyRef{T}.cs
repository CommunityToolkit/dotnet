// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests;

[TestClass]
public class Test_ReadOnlyRefOfT
{
    [TestMethod]
#if NETFRAMEWORK
    public void Test_RefOfT_CreateRefOfT()
    {
        ReadOnlyFieldOwner model = new();
        ReadOnlyRef<int> reference = new(model, model.Value);

        Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(model.Value), ref Unsafe.AsRef(reference.Value)));
    }

    /// <summary>
    /// A dummy model that owns an <see cref="int"/> field.
    /// </summary>
    private sealed class ReadOnlyFieldOwner
    {
        public readonly int Value = 1;
    }
#else
    public void Test_RefOfT_CreateRefOfT()
    {
        int value = 1;
        ReadOnlyRef<int> reference = new(value);

        Assert.IsTrue(Unsafe.AreSame(ref value, ref Unsafe.AsRef(reference.Value)));
    }
#endif
}

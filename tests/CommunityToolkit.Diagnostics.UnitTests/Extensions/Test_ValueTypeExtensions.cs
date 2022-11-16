// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Diagnostics.UnitTests.Extensions;

[TestClass]
public class Test_ValueTypeExtensions
{
    [TestMethod]
    public void Test_ValueTypeExtensions_ToHexString()
    {
        Assert.AreEqual(((byte)0).ToHexString(), "0x00");
        Assert.AreEqual(((byte)127).ToHexString(), "0x7F");
        Assert.AreEqual(((byte)255).ToHexString(), "0xFF");
        Assert.AreEqual(((ushort)6458).ToHexString(), "0x193A");
        Assert.AreEqual(6458.ToHexString(), "0x0000193A");
        Assert.AreEqual((-1).ToHexString(), "0xFFFFFFFF");
        Assert.AreEqual(true.ToHexString(), "0x01");
    }
}

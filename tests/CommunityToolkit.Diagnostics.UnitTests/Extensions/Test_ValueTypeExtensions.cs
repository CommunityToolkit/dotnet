// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Diagnostics.UnitTests.Extensions;

[TestClass]
public class Test_ValueTypeExtensions
{
    [TestMethod]
    public void Test_ValueTypeExtensions_ToHexString()
    {
        Assert.AreEqual("0x00", ((byte)0).ToHexString());
        Assert.AreEqual("0x7F", ((byte)127).ToHexString());
        Assert.AreEqual("0xFF", ((byte)255).ToHexString());
        Assert.AreEqual("0x193A", ((ushort)6458).ToHexString());
        Assert.AreEqual("0x0000193A", 6458.ToHexString());
        Assert.AreEqual("0xFFFFFFFF", (-1).ToHexString());
        Assert.AreEqual("0x01", true.ToHexString());
    }
}

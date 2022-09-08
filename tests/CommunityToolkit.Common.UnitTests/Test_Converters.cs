// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Common.UnitTests;

[TestClass]
public class Test_Converters
{
    [TestMethod]
    [DataRow(1024L - 1, "1023 bytes")]
    [DataRow(1024L, "1.0 KB")]
    [DataRow(1024L * 1024, "1.0 MB")]
    [DataRow(1024L * 1024 * 1024, "1.0 GB")]
    [DataRow(1024L * 1024 * 1024 * 1024, "1.0 TB")]
    [DataRow(1024L * 1024 * 1024 * 1024 * 1024, "1.0 PB")]
    [DataRow(1024L * 1024 * 1024 * 1024 * 1024 * 1024, "1.0 EB")]
    public void Test_ToFileSizeString(long size, string expected)
    {
        Assert.AreEqual(expected, Converters.ToFileSizeString(size));
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CommunityToolkit.Diagnostics.UnitTests;

public partial class Test_Guard
{
    [TestMethod]
    public void Test_EnumCache_Initialization_Ok()
    {
        TestEnum[] values = [.. EnumCache<TestEnum>.Values];
        TestEnum[] comparisonValues = [
            TestEnum.Value0,
            TestEnum.Value1,
            TestEnum.Value2,
            TestEnum.Value3,
            TestEnum.Value4,
        ];

        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(values[i], comparisonValues[i]);
        }
    }

    [TestMethod]
    public void Test_Guard_IsDefined_Ok()
    {
        Guard.IsDefined(TestEnum.Value0, nameof(Test_Guard_IsDefined_Ok));
        Guard.IsDefined(TestEnum.Value1, nameof(Test_Guard_IsDefined_Ok));
        Guard.IsDefined(TestEnum.Value2, nameof(Test_Guard_IsDefined_Ok));
        Guard.IsDefined(TestEnum.Value3, nameof(Test_Guard_IsDefined_Ok));
        Guard.IsDefined(TestEnum.Value4, nameof(Test_Guard_IsDefined_Ok));
    }

    [TestMethod]
    public void Test_Guard_IsDefined_Fail()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => Guard.IsDefined((TestEnum)10, nameof(Test_Guard_IsDefined_Fail)));
    }
}

file enum TestEnum
{
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
}

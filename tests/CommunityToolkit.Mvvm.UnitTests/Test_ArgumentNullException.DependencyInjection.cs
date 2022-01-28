// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

public partial class Test_ArgumentNullException
{
    [TestMethod]
    public void Test_ArgumentNullException_Ioc()
    {
        Ioc ioc = new();

        Assert(() => ioc.GetService(serviceType: null!), "serviceType");
        Assert(() => ioc.ConfigureServices(serviceProvider: null!), "serviceProvider");
    }
}

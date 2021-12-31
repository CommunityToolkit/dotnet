// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.KeepSourceGeneratorAttributes.UnitTests;

[TestClass]
public class Test_SourceGeneratorAttributes
{
    [TestMethod]
    public void Test_AttributeMetadata()
    {
#if !MVVMTOOLKIT_KEEP_SOURCE_GENERATOR_ATTRIBUTES
        Assert.Fail();
#endif
        Assert.AreEqual(3, typeof(TestModel).GetCustomAttributes().Count(a => a.GetType().FullName!.Contains("CommunityToolkit.Mvvm")));
        Assert.AreEqual(3, typeof(TestModel).GetField(nameof(TestModel.name))!.GetCustomAttributes().Count(a => a.GetType().FullName!.Contains("CommunityToolkit.Mvvm")));
        Assert.AreEqual(1, typeof(TestModel).GetMethod(nameof(TestModel.Test))!.GetCustomAttributes().Count(a => a.GetType().FullName!.Contains("CommunityToolkit.Mvvm")));
    }

    [INotifyPropertyChanged]
    [ObservableObject]
    [ObservableRecipient]
    public class TestModel
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor("")]
        [AlsoNotifyCanExecuteFor("")]
        public string? name;

        [ICommand]
        public void Test()
        {
        }
    }
}

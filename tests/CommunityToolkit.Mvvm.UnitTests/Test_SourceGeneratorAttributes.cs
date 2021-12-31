// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_SourceGeneratorAttributes
{
    [TestMethod]
    public void Test_AttributeMetadata()
    {
#if MVVMTOOLKIT_KEEP_SOURCE_GENERATOR_ATTRIBUTES
        Assert.Fail();
#endif
        Assert.IsFalse(typeof(INotifyPropertyChangedModel).GetCustomAttributes().Any(a => a.GetType().FullName!.Contains("CommunityToolkit.Mvvm")));
        Assert.IsFalse(typeof(TestModel).GetCustomAttributes().Any(a => a.GetType().FullName!.Contains("CommunityToolkit.Mvvm")));
        Assert.IsFalse(typeof(TestModel).GetField(nameof(TestModel.name))!.GetCustomAttributes().Any(a => a.GetType().FullName!.Contains("CommunityToolkit.Mvvm")));
        Assert.IsFalse(typeof(TestModel).GetMethod(nameof(TestModel.Test))!.GetCustomAttributes().Any(a => a.GetType().FullName!.Contains("CommunityToolkit.Mvvm")));
        Assert.IsFalse(typeof(ObservableRecipientModel).GetCustomAttributes().Any(a => a.GetType().FullName!.Contains("CommunityToolkit.Mvvm")));
    }

    [INotifyPropertyChanged]
    public partial class INotifyPropertyChangedModel
    {
    }

    [ObservableObject]
    public partial class TestModel
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(Name))]
        [AlsoNotifyCanExecuteFor(nameof(TestCommand))]
        public string? name;

        [ICommand]
        public void Test()
        {
        }
    }

    [ObservableRecipient]
    public partial class ObservableRecipientModel : ObservableObject
    {
    }
}

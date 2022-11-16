// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

/// <summary>
/// This class contains general unit tests for source generators, without a specific dependency on one.
/// For instance, this can be used for tests that validate common generation helpers used by all generators.
/// </summary>
[TestClass]
public partial class Test_SourceGenerators
{
    [TestMethod]
    public void Test_SourceGenerators_NestedTypesThatAreNotJustClasses()
    {
        // This test just needs to compile, mostly
        NestedStructType.NestedInterfaceType.NestedRecord.MyViewModel model = new();

        Assert.IsNull(model.Name);
        Assert.IsTrue(model.TestCommand is IRelayCommand);
    }

    public partial struct NestedStructType
    {
        public partial interface NestedInterfaceType
        {
            public partial record NestedRecord
            {
                [ObservableRecipient]
                public partial class MyViewModel : ObservableValidator
                {
                    [ObservableProperty]
                    [Required]
                    private string? name;

                    [RelayCommand]
                    private void Test()
                    {
                    }
                }
            }
        }
    }

    [TestMethod]
    public void Test_SourceGenerators_NestedTypesThatAreNotJustClassesAndWithGenerics()
    {
        // This test just needs to compile, mostly
        NestedStructTypeWithGenerics<int, float>.NestedInterfaceType<string>.NestedRecord<string>.MyViewModel model = new();

        Assert.IsNull(model.Name);
        Assert.IsTrue(model.TestCommand is IRelayCommand);
    }

    public partial struct NestedStructTypeWithGenerics<T1, T2>
        where T2 : struct
    {
        public partial interface NestedInterfaceType<TFoo>
        {
            public partial record NestedRecord<TBar>
            {
                [INotifyPropertyChanged]
                public partial class MyViewModel
                {
                    [ObservableProperty]
                    private string? name;

                    [RelayCommand]
                    private void Test()
                    {
                    }
                }
            }
        }
    }
}

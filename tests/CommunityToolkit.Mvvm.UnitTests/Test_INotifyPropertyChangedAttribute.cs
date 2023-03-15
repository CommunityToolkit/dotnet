// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable MVVMTK0032

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_INotifyPropertyChangedAttribute
{
    [TestMethod]
    public void Test_INotifyPropertyChanged_Events()
    {
        SampleModel? model = new();

        (PropertyChangedEventArgs, int) changed = default;

        model.PropertyChanged += (s, e) =>
        {
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changed = (e, model.Data);
        };

        model.Data = 42;

        Assert.AreEqual(nameof(SampleModel.Data), changed.Item1?.PropertyName);
        Assert.AreEqual(42, changed.Item2);
    }

    [INotifyPropertyChanged]
    public partial class SampleModel
    {
        private int data;

        public int Data
        {
            get => this.data;
            set => SetProperty(ref this.data, value);
        }
    }

    [TestMethod]
    public void Test_INotifyPropertyChanged_WithoutHelpers()
    {
        Assert.IsTrue(typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(SampleModelWithoutHelpers)));
        Assert.IsFalse(typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(SampleModelWithoutHelpers)));

        // This just needs to check that it compiles
        _ = nameof(SampleModelWithoutHelpers.PropertyChanged);

        MethodInfo[]? methods = typeof(SampleModelWithoutHelpers).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        Assert.AreEqual(2, methods.Length);
        Assert.AreEqual("OnPropertyChanged", methods[0].Name);
        Assert.AreEqual("OnPropertyChanged", methods[1].Name);

        System.Type[]? types = typeof(SampleModelWithoutHelpers).GetNestedTypes(BindingFlags.NonPublic);

        Assert.AreEqual(0, types.Length);
    }

    [INotifyPropertyChanged(IncludeAdditionalHelperMethods = false)]
    public partial class SampleModelWithoutHelpers
    {
    }

    [TestMethod]
    public void Test_INotifyPropertyChanged_WithGeneratedProperties()
    {
        Assert.IsTrue(typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(SampleModelWithINPCAndObservableProperties)));
        Assert.IsFalse(typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(SampleModelWithINPCAndObservableProperties)));

        SampleModelWithINPCAndObservableProperties model = new();
        List<PropertyChangedEventArgs> eventArgs = new();

        model.PropertyChanged += (s, e) => eventArgs.Add(e);

        model.X = 42;
        model.Y = 66;

        Assert.AreEqual(2, eventArgs.Count);
        Assert.AreEqual(nameof(SampleModelWithINPCAndObservableProperties.X), eventArgs[0].PropertyName);
        Assert.AreEqual(nameof(SampleModelWithINPCAndObservableProperties.Y), eventArgs[1].PropertyName);
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4167
    [INotifyPropertyChanged]
    public partial class SampleModelWithINPCAndObservableProperties
    {
        [ObservableProperty]
        private int x;

        [ObservableProperty]
        private int y;
    }

    [TestMethod]
    public void Test_INotifyPropertyChanged_WithGeneratedProperties_ExternalNetStandard20Assembly()
    {
        Assert.IsTrue(typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(ExternalAssembly.SampleModelWithINPCAndObservableProperties)));
        Assert.IsFalse(typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(ExternalAssembly.SampleModelWithINPCAndObservableProperties)));

        ExternalAssembly.SampleModelWithINPCAndObservableProperties model = new();
        List<PropertyChangedEventArgs> eventArgs = new();

        model.PropertyChanged += (s, e) => eventArgs.Add(e);

        model.X = 42;
        model.Y = 66;

        Assert.AreEqual(2, eventArgs.Count);
        Assert.AreEqual(nameof(ExternalAssembly.SampleModelWithINPCAndObservableProperties.X), eventArgs[0].PropertyName);
        Assert.AreEqual(nameof(ExternalAssembly.SampleModelWithINPCAndObservableProperties.Y), eventArgs[1].PropertyName);
    }
}

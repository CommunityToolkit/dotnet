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

        Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel.Data));
        Assert.AreEqual(changed.Item2, 42);
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

        Assert.AreEqual(methods.Length, 2);
        Assert.AreEqual(methods[0].Name, "OnPropertyChanged");
        Assert.AreEqual(methods[1].Name, "OnPropertyChanged");

        System.Type[]? types = typeof(SampleModelWithoutHelpers).GetNestedTypes(BindingFlags.NonPublic);

        Assert.AreEqual(types.Length, 0);
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

        Assert.AreEqual(eventArgs.Count, 2);
        Assert.AreEqual(eventArgs[0].PropertyName, nameof(SampleModelWithINPCAndObservableProperties.X));
        Assert.AreEqual(eventArgs[1].PropertyName, nameof(SampleModelWithINPCAndObservableProperties.Y));
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

#if ROSLYN_4_12_0_OR_GREATER
    [TestMethod]
    public void Test_INotifyPropertyChanged_WithGeneratedPartialProperties()
    {
        Assert.IsTrue(typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(SampleModelWithINPCAndObservablePartialProperties)));
        Assert.IsFalse(typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(SampleModelWithINPCAndObservablePartialProperties)));

        SampleModelWithINPCAndObservablePartialProperties model = new();
        List<PropertyChangedEventArgs> eventArgs = new();

        model.PropertyChanged += (s, e) => eventArgs.Add(e);

        model.X = 42;
        model.Y = 66;

        Assert.AreEqual(eventArgs.Count, 2);
        Assert.AreEqual(eventArgs[0].PropertyName, nameof(SampleModelWithINPCAndObservablePartialProperties.X));
        Assert.AreEqual(eventArgs[1].PropertyName, nameof(SampleModelWithINPCAndObservablePartialProperties.Y));
    }

    [INotifyPropertyChanged]
    public partial class SampleModelWithINPCAndObservablePartialProperties
    {
        [ObservableProperty]
        public partial int X { get; set; }

        [ObservableProperty]
        public partial int Y { get; set; }
    }
#endif

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

        Assert.AreEqual(eventArgs.Count, 2);
        Assert.AreEqual(eventArgs[0].PropertyName, nameof(ExternalAssembly.SampleModelWithINPCAndObservableProperties.X));
        Assert.AreEqual(eventArgs[1].PropertyName, nameof(ExternalAssembly.SampleModelWithINPCAndObservableProperties.Y));
    }
}

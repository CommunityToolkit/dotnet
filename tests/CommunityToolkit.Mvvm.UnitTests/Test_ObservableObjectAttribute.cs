// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_ObservableObjectAttribute
{
    [TestMethod]
    public void Test_ObservableObjectAttribute_Events()
    {
        Assert.IsTrue(typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(SampleModel)));
        Assert.IsTrue(typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(SampleModel)));

        SampleModel? model = new();

        (PropertyChangingEventArgs, int) changing = default;
        (PropertyChangedEventArgs, int) changed = default;

        model.PropertyChanging += (s, e) =>
        {
            Assert.IsNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changing = (e, model.Data);
        };

        model.PropertyChanged += (s, e) =>
        {
            Assert.IsNotNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changed = (e, model.Data);
        };

        model.Data = 42;

        Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModel.Data));
        Assert.AreEqual(changing.Item2, 0);
        Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel.Data));
        Assert.AreEqual(changed.Item2, 42);
    }

    [TestMethod]
    public void Test_ObservableObjectAttribute_OnSealedClass_Events()
    {
        Assert.IsTrue(typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(SampleModelSealed)));
        Assert.IsTrue(typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(SampleModelSealed)));

        SampleModelSealed? model = new();

        (PropertyChangingEventArgs, int) changing = default;
        (PropertyChangedEventArgs, int) changed = default;

        model.PropertyChanging += (s, e) =>
        {
            Assert.IsNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changing = (e, model.Data);
        };

        model.PropertyChanged += (s, e) =>
        {
            Assert.IsNotNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changed = (e, model.Data);
        };

        model.Data = 42;

        Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModelSealed.Data));
        Assert.AreEqual(changing.Item2, 0);
        Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModelSealed.Data));
        Assert.AreEqual(changed.Item2, 42);
    }

    [ObservableObject]
    public partial class SampleModel
    {
        private int data;

        public int Data
        {
            get => this.data;
            set => SetProperty(ref this.data, value);
        }
    }

    [ObservableObject]
    public sealed partial class SampleModelSealed
    {
        private int data;

        public int Data
        {
            get => this.data;
            set => SetProperty(ref this.data, value);
        }
    }
}

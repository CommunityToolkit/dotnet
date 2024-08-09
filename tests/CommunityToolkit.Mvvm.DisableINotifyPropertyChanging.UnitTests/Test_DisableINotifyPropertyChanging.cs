// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.DisableINotifyPropertyChanging.UnitTests;

[TestClass]
public class Test_DisableINotifyPropertyChanging
{
    static Test_DisableINotifyPropertyChanging()
    {
        AppContext.SetSwitch("MVVMTOOLKIT_ENABLE_INOTIFYPROPERTYCHANGING_SUPPORT", false);
    }

    [TestMethod]
    public void Test_ObservableObject_Events()
    {
        SampleModel<int>? model = new();

        (PropertyChangedEventArgs, int) changed = default;

        model.PropertyChanging += (s, e) => Assert.Fail();

        model.PropertyChanged += (s, e) =>
        {
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changed = (e, model.Data);
        };

        model.Data = 42;

        Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel<int>.Data));
        Assert.AreEqual(changed.Item2, 42);
    }

    public class SampleModel<T> : ObservableObject
    {
        private T? data;

        public T? Data
        {
            get => this.data;
            set => SetProperty(ref this.data, value);
        }

        protected override void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            Assert.Fail();
        }
    }
}

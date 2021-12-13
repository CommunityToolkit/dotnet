// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_ArgumentNullException
{
    [TestMethod]
    public void Test_ObservableObject()
    {
        TestObservableObject model = new();

        Assert(() => model.NullPropertyChanged(), "e");
        Assert(() => model.NullPropertyChanging(), "e");

        Assert(() => model.SetProperty(comparer: null!), "comparer");
        Assert(() => model.SetProperty(callback: (Action<string>)null!), "callback");
        Assert(() => model.SetProperty(comparer: null!, s => { }), "comparer");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, callback: null!), "callback");
        Assert(() => model.SetProperty(model: null!, (object m, string v) => { }), "model");
        Assert(() => model.SetProperty(new object(), callback: null!), "callback");
        Assert(() => model.SetProperty(comparer: null!, new object(), (object m, string v) => { }), "comparer");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, model: null!, (object m, string v) => { }), "model");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, new object(), callback: null!), "callback");
        Assert(() => model.SetProperty(callback: (Action<Task?>)null!), "callback");
        Assert(() => model.SetProperty(callback: (Action<Task<string>?>)null!), "callback");
    }

    [TestMethod]
    public void Test_ObservableRecipient()
    {
        Assert(() => new TestObservableRecipient(null!), "messenger");

        TestObservableRecipient model = new(new WeakReferenceMessenger());

        Assert(() => model.SetProperty(comparer: null!), "comparer");
        Assert(() => model.SetProperty(callback: null!), "callback");
        Assert(() => model.SetProperty(comparer: null!, s => { }), "comparer");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, callback: null!), "callback");
        Assert(() => model.SetProperty(model: null!, (object m, string v) => { }), "model");
        Assert(() => model.SetProperty(new object(), callback: null!), "callback");
        Assert(() => model.SetProperty(comparer: null!, new object(), (object m, string v) => { }), "comparer");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, model: null!, (object m, string v) => { }), "model");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, new object(), callback: null!), "callback");
    }

    [TestMethod]
    public void Test_ObservableValidator()
    {
        Assert(() => new TestObservableValidator(null!), "validationContext");

        TestObservableValidator model = new();

        Assert(() => model.SetProperty(propertyName: null!), "propertyName");
        Assert(() => model.SetProperty(comparer: null!, ""), "comparer");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, propertyName: null!), "propertyName");
        Assert(() => model.SetProperty(callback: null!, ""), "callback");
        Assert(() => model.SetProperty(s => { }, propertyName: null!), "propertyName");
        Assert(() => model.SetProperty(comparer: null!, s => { }, ""), "comparer");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, callback: null!, ""), "callback");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, s => { }, propertyName: null!), "propertyName");
        Assert(() => model.SetProperty(model: null!, (m, v) => { }, ""), "model");
        Assert(() => model.SetProperty(new object(), callback: null!, ""), "callback");
        Assert(() => model.SetProperty(new object(), (m, v) => { }, propertyName: null!), "propertyName");
        Assert(() => model.SetProperty(comparer: null!, new object(), (m, v) => { }, ""), "comparer");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, model: null!, (m, v) => { }, ""), "model");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, new object(), callback: null!, ""), "callback");
        Assert(() => model.SetProperty(EqualityComparer<string>.Default, new object(), (m, v) => { }, propertyName: null!), "propertyName");

        Assert(() => model.TrySetProperty(propertyName: null!), "propertyName");
        Assert(() => model.TrySetProperty(comparer: null!, ""), "comparer");
        Assert(() => model.TrySetProperty(EqualityComparer<string>.Default, propertyName: null!), "propertyName");
        Assert(() => model.TrySetProperty(callback: null!, ""), "callback");
        Assert(() => model.TrySetProperty(s => { }, propertyName: null!), "propertyName");
        Assert(() => model.TrySetProperty(comparer: null!, s => { }, ""), "comparer");
        Assert(() => model.TrySetProperty(EqualityComparer<string>.Default, callback: null!, ""), "callback");
        Assert(() => model.TrySetProperty(EqualityComparer<string>.Default, s => { }, propertyName: null!), "propertyName");
        Assert(() => model.TrySetProperty(model: null!, (m, v) => { }, ""), "model");
        Assert(() => model.TrySetProperty(new object(), callback: null!, ""), "callback");
        Assert(() => model.TrySetProperty(new object(), (m, v) => { }, propertyName: null!), "propertyName");
        Assert(() => model.TrySetProperty(comparer: null!, new object(), (m, v) => { }, ""), "comparer");
        Assert(() => model.TrySetProperty(EqualityComparer<string>.Default, model: null!, (m, v) => { }, ""), "model");
        Assert(() => model.TrySetProperty(EqualityComparer<string>.Default, new object(), callback: null!, ""), "callback");
        Assert(() => model.TrySetProperty(EqualityComparer<string>.Default, new object(), (m, v) => { }, propertyName: null!), "propertyName");

        Assert(() => model.ValidateProperty(null!), "propertyName");
    }

    private class TestObservableObject : ObservableObject
    {
        public void NullPropertyChanged()
        {
            OnPropertyChanged((PropertyChangedEventArgs)null!);
        }

        public void NullPropertyChanging()
        {
            OnPropertyChanging((PropertyChangingEventArgs)null!);
        }

        public void SetProperty(IEqualityComparer<string> comparer)
        {
            string dummy = "";

            _ = SetProperty(ref dummy, dummy, comparer);
        }

        public void SetProperty(Action<string> callback)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, callback);
        }

        public void SetProperty(IEqualityComparer<string> comparer, Action<string> callback)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, comparer, callback);
        }

        public void SetProperty(object model, Action<object, string> callback)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, model, callback);
        }

        public void SetProperty(IEqualityComparer<string> comparer, object model, Action<object, string> callback)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, comparer, model, callback);
        }

        public void SetProperty(Action<Task?> callback)
        {
            TaskNotifier? dummy = null;

            _ = SetPropertyAndNotifyOnCompletion(ref dummy, null, callback);
        }

        public void SetProperty(Action<Task<string>?> callback)
        {
            TaskNotifier<string>? dummy = null;

            _ = SetPropertyAndNotifyOnCompletion(ref dummy, null, callback);
        }
    }

    private class TestObservableRecipient : ObservableRecipient
    {
        public TestObservableRecipient(IMessenger messenger)
            : base(messenger)
        {
        }

        public void SetProperty(IEqualityComparer<string> comparer)
        {
            string dummy = "";

            _ = SetProperty(ref dummy, dummy, comparer, true);
        }

        public void SetProperty(Action<string> callback)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, callback, true);
        }

        public void SetProperty(IEqualityComparer<string> comparer, Action<string> callback)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, comparer, callback, true);
        }

        public void SetProperty(object model, Action<object, string> callback)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, model, callback, true);
        }

        public void SetProperty(IEqualityComparer<string> comparer, object model, Action<object, string> callback)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, comparer, model, callback, true);
        }
    }

    internal class TestObservableValidator : ObservableValidator
    {
        public TestObservableValidator()
        {
        }

        public TestObservableValidator(ValidationContext validationContext)
            : base(validationContext)
        {
        }

        public void SetProperty(string propertyName)
        {
            string dummy = "";

            _ = SetProperty(ref dummy, dummy, true, propertyName);
        }

        public void SetProperty(IEqualityComparer<string> comparer, string propertyName)
        {
            string dummy = "";

            _ = SetProperty(ref dummy, dummy, comparer, true, propertyName);
        }

        public void SetProperty(Action<string> callback, string propertyName)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, callback, true, propertyName);
        }

        public void SetProperty(IEqualityComparer<string> comparer, Action<string> callback, string propertyName)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, comparer, callback, true, propertyName);
        }

        public void SetProperty(object model, Action<object, string> callback, string propertyName)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, model, callback, true, propertyName);
        }

        public void SetProperty(IEqualityComparer<string> comparer, object model, Action<object, string> callback, string propertyName)
        {
            string dummy = "";

            _ = SetProperty(dummy, dummy, comparer, model, callback, true, propertyName);
        }

        public void TrySetProperty(string propertyName)
        {
            string dummy = "";

            _ = TrySetProperty(ref dummy, dummy, out _, propertyName);
        }

        public void TrySetProperty(IEqualityComparer<string> comparer, string propertyName)
        {
            string dummy = "";

            _ = TrySetProperty(ref dummy, dummy, comparer, out _, propertyName);
        }

        public void TrySetProperty(Action<string> callback, string propertyName)
        {
            string dummy = "";

            _ = TrySetProperty(dummy, dummy, callback, out _, propertyName);
        }

        public void TrySetProperty(IEqualityComparer<string> comparer, Action<string> callback, string propertyName)
        {
            string dummy = "";

            _ = TrySetProperty(dummy, dummy, comparer, callback, out _, propertyName);
        }

        public void TrySetProperty(object model, Action<object, string> callback, string propertyName)
        {
            string dummy = "";

            _ = TrySetProperty(dummy, dummy, model, callback, out _, propertyName);
        }

        public void TrySetProperty(IEqualityComparer<string> comparer, object model, Action<object, string> callback, string propertyName)
        {
            string dummy = "";

            _ = TrySetProperty(dummy, dummy, comparer, model, callback, out _, propertyName);
        }

        public void ValidateProperty(string propertyName)
        {
            base.ValidateProperty("", propertyName);
        }
    }

    /// <summary>
    /// Asserts that a given <see cref="Action"/> thrown an <see cref="System.ArgumentNullException"/> for the specified parameter.
    /// </summary>
    /// <param name="action">The input <see cref="Action"/> to invoke.</param>
    /// <param name="parameterName">The parameter name to check.</param>
    private static void Assert(Action action, string parameterName)
    {
        System.ArgumentNullException exception = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsException<System.ArgumentNullException>(action);

        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(exception.ParamName, parameterName);
    }
}

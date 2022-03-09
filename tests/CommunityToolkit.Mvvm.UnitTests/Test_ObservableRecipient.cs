// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
#if !NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
#endif
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS0618

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public class Test_ObservableRecipient
{
    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_ObservableRecipient_Activation(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        SomeRecipient<int>? viewmodel = new(messenger);

        Assert.IsFalse(viewmodel.IsActivatedCheck);

        viewmodel.IsActive = true;

        Assert.IsTrue(viewmodel.IsActivatedCheck);
        Assert.IsTrue(viewmodel.CurrentMessenger.IsRegistered<SampleMessage>(viewmodel));

        viewmodel.IsActive = false;

        Assert.IsFalse(viewmodel.IsActivatedCheck);
        Assert.IsFalse(viewmodel.CurrentMessenger.IsRegistered<SampleMessage>(viewmodel));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_ObservableRecipient_IsSame(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        SomeRecipient<int>? viewmodel = new(messenger);

        Assert.AreSame(viewmodel.CurrentMessenger, messenger);
    }

    [TestMethod]
    public void Test_ObservableRecipient_Default()
    {
        SomeRecipient<int>? viewmodel = new();

        Assert.AreSame(viewmodel.CurrentMessenger, WeakReferenceMessenger.Default);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_ObservableRecipient_Injection(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        SomeRecipient<int>? viewmodel = new(messenger);

        Assert.AreSame(viewmodel.CurrentMessenger, messenger);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_ObservableRecipient_Broadcast(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        SomeRecipient<int>? viewmodel = new(messenger);

        PropertyChangedMessage<int>? message = null;

        messenger.Register<PropertyChangedMessage<int>>(messenger, (r, m) => message = m);

        viewmodel.Data = 42;

        Assert.IsNotNull(message);
        Assert.AreSame(message.Sender, viewmodel);
        Assert.AreEqual(message.OldValue, 0);
        Assert.AreEqual(message.NewValue, 42);
        Assert.AreEqual(message.PropertyName, nameof(SomeRecipient<int>.Data));
    }

    [TestMethod]
    public void Test_IRecipient_VerifyTrimmingAnnotation()
    {
#if NET6_0_OR_GREATER
        System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute? attribute =
            typeof(Messaging.__Internals.__IMessengerExtensions)
            .GetCustomAttribute<System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute>();

        Assert.IsNotNull(attribute);
        Assert.AreEqual(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, attribute.MemberTypes);
#else
        IEnumerable<Attribute> attributes = typeof(Messaging.__Internals.__IMessengerExtensions).GetCustomAttributes();

        Assert.IsFalse(attributes.Any(static a => a.GetType().Name is "DynamicallyAccessedMembersAttribute"));
#endif
    }

    public class SomeRecipient<T> : ObservableRecipient
    {
        public SomeRecipient()
        {
        }

        public SomeRecipient(IMessenger messenger)
            : base(messenger)
        {
        }

        public IMessenger CurrentMessenger => Messenger;

        private T? data;

        public T? Data
        {
            get => data;
            set => SetProperty(ref data, value, true);
        }

        public bool IsActivatedCheck { get; private set; }

        protected override void OnActivated()
        {
            IsActivatedCheck = true;

            Messenger.Register<SampleMessage>(this, (r, m) => { });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            IsActivatedCheck = false;
        }
    }

    public class SampleMessage
    {
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

partial class Test_Messenger
{
    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_CreateObservable(Type type)
    {
        IMessenger messenger = (IMessenger)Activator.CreateInstance(type)!;

        IObservable<MessageA> observable = messenger.CreateObservable<MessageA>();

        Assert.IsNotNull(observable);

        List<MessageA> messages = new();

        IDisposable disposable = observable.Subscribe(messages.Add);

        MessageA message1 = new();
        MessageA message2 = new();

        _ = messenger.Send(message1);
        _ = messenger.Send(message2);

        // The expected messages have been observed
        CollectionAssert.AreEqual(messages, new[] { message1, message2 });

        disposable.Dispose();

        _ = messenger.Send<MessageA>();

        // No messages are sent after unsubscribing the observable
        CollectionAssert.AreEqual(messages, new[] { message1, message2 });
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_CreateObservable_WithToken(Type type)
    {
        IMessenger messenger = (IMessenger)Activator.CreateInstance(type)!;

        IObservable<MessageA> observable = messenger.CreateObservable<MessageA, int>(42);

        Assert.IsNotNull(observable);

        List<MessageA> messages = new();

        IDisposable disposable = observable.Subscribe(messages.Add);

        MessageA message1 = new();
        MessageA message2 = new();

        _ = messenger.Send(message1, 42);
        _ = messenger.Send(message2, 42);

        _ = messenger.Send(new MessageA(), 1);
        _ = messenger.Send(new MessageA(), 999);

        // The expected messages have been observed (only for matching tokens)
        CollectionAssert.AreEqual(messages, new[] { message1, message2 });

        disposable.Dispose();

        _ = messenger.Send(new MessageA(), 42);
        _ = messenger.Send(new MessageA(), 1);

        // No messages are sent after unsubscribing the observable (regardless of token)
        CollectionAssert.AreEqual(messages, new[] { message1, message2 });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Messenger_CreateObservable_NullMessenger()
    {
        _ = IMessengerExtensions.CreateObservable<MessageA>(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Messenger_CreateObservable_WithToken_NullMessenger()
    {
        _ = IMessengerExtensions.CreateObservable<MessageA, string>(null!, "Hello");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Messenger_CreateObservable_WithToken_NullToken()
    {
        _ = IMessengerExtensions.CreateObservable<MessageA, string>(new WeakReferenceMessenger(), null!);
    }
}

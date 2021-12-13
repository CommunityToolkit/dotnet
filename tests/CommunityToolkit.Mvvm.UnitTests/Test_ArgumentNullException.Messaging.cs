// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

public partial class Test_ArgumentNullException
{
    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger(Type type)
    {
        IMessenger messenger = (IMessenger)Activator.CreateInstance(type)!;

        Assert(() => messenger.IsRegistered<DummyMessage, int>(recipient: null!, 42), "recipient");
        Assert(() => messenger.IsRegistered<DummyMessage, string>(new object(), null!), "token");

        Assert(() => messenger.Register<object, DummyMessage, string>(recipient: null!, "", (r, m) => { }), "recipient");
        Assert(() => messenger.Register<object, DummyMessage, string>(new object(), null!, (r, m) => { }), "token");
        Assert(() => messenger.Register<object, DummyMessage, string>(new object(), "", handler: null!), "handler");

        Assert(() => messenger.UnregisterAll(recipient: null!), "recipient");

        Assert(() => messenger.UnregisterAll(recipient: null!, ""), "recipient");
        Assert(() => messenger.UnregisterAll<string>(new object(), token: null!), "token");

        Assert(() => messenger.Unregister<DummyMessage, string>(recipient: null!, ""), "recipient");
        Assert(() => messenger.Unregister<DummyMessage, string>(new object(), token: null!), "token");

        Assert(() => messenger.Send<DummyMessage, string>(message: null!, ""), "message");
        Assert(() => messenger.Send<DummyMessage, string>(new DummyMessage(), token: null!), "token");
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_MessengerExtensions(Type type)
    {
        IMessenger messenger = (IMessenger)Activator.CreateInstance(type)!;

        Assert(() => ((IMessenger)null!).IsRegistered<DummyMessage>(new object()), "messenger");
        Assert(() => messenger.IsRegistered<DummyMessage>(recipient: null!), "recipient");

        Assert(() => ((IMessenger)null!).RegisterAll(new object()), "messenger");
        Assert(() => messenger.RegisterAll(recipient: null!), "recipient");

        Assert(() => ((IMessenger)null!).RegisterAll(new object(), ""), "messenger");
        Assert(() => messenger.RegisterAll(recipient: null!, ""), "recipient");
        Assert(() => messenger.RegisterAll<string>(new object(), token: null!), "token");

        Assert(() => ((IMessenger)null!).Register(new Recipient()), "messenger");
        Assert(() => messenger.Register<DummyMessage>(recipient: null!), "recipient");

        Assert(() => ((IMessenger)null!).Register(new Recipient(), ""), "messenger");
        Assert(() => messenger.Register<DummyMessage, string>(recipient: null!, ""), "recipient");
        Assert(() => messenger.Register<DummyMessage, string>(new Recipient(), token: null!), "token");

        Assert(() => ((IMessenger)null!).Register<DummyMessage>(new object(), (r, m) => { }), "messenger");
        Assert(() => messenger.Register<DummyMessage>(recipient: null!, (r, m) => { }), "recipient");
        Assert(() => messenger.Register<DummyMessage>(new object(), handler: null!), "handler");

        Assert(() => ((IMessenger)null!).Register<Recipient, DummyMessage>(new Recipient(), (r, m) => { }), "messenger");
        Assert(() => messenger.Register<Recipient, DummyMessage>(recipient: null!, (r, m) => { }), "recipient");
        Assert(() => messenger.Register<Recipient, DummyMessage>(new Recipient(), handler: null!), "handler");

        Assert(() => ((IMessenger)null!).Register<DummyMessage, string>(new Recipient(), "", (r, m) => { }), "messenger");
        Assert(() => messenger.Register<DummyMessage, string>(recipient: null!, "", (r, m) => { }), "recipient");
        Assert(() => messenger.Register<DummyMessage, string>(new Recipient(), token: null!, handler: null!), "token");
        Assert(() => messenger.Register<DummyMessage, string>(new Recipient(), "", handler: null!), "handler");

        Assert(() => ((IMessenger)null!).Unregister<DummyMessage>(new Recipient()), "messenger");
        Assert(() => messenger.Unregister<DummyMessage>(recipient: null!), "recipient");

        Assert(() => ((IMessenger)null!).Send<DummyMessage>(), "messenger");

        Assert(() => ((IMessenger)null!).Send(new DummyMessage()), "messenger");
        Assert(() => messenger.Send<DummyMessage>(message: null!), "message");

        Assert(() => ((IMessenger)null!).Send<DummyMessage, string>(""), "messenger");
        Assert(() => messenger.Send<DummyMessage, string>(token: null!), "token");
    }

    [TestMethod]
    public void Test_AsyncCollectionRequestMessage()
    {
        AsyncCollectionRequestMessage<int> message = new();

        Assert(() => message.Reply(response: (Task<int>)null!), "response");
        Assert(() => message.Reply(response: (Func<CancellationToken, Task<int>>)null!), "response");
    }

    [TestMethod]
    public void Test_AsyncRequestMessage()
    {
        AsyncRequestMessage<int> message = new();

        Assert(() => message.Reply(response: null!), "response");
    }

    [TestMethod]
    public void Test_PropertyChangedMessage()
    {
        Assert(() => new PropertyChangedMessage<int>(sender: null!, "", 0, 1), "sender");
    }

    [TestMethod]
    public void Test_RequestMessage()
    {
        Assert(() => _ = (int)(RequestMessage<int>)null!, "message");
    }

    internal class DummyMessage
    {
    }

    internal class Recipient : IRecipient<DummyMessage>
    {
        public void Receive(DummyMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

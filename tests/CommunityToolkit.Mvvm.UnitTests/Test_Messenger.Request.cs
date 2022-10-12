// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

partial class Test_Messenger
{
    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_RequestMessage_Ok(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();
        object? test = null;

        void Receive(object recipient, NumberRequestMessage m)
        {
            test = recipient;

            Assert.IsFalse(m.HasReceivedResponse);

            m.Reply(42);

            Assert.IsTrue(m.HasReceivedResponse);
        }

        messenger.Register<NumberRequestMessage>(recipient, Receive);

        int result = messenger.Send<NumberRequestMessage>();

        Assert.AreSame(test, recipient);
        Assert.AreEqual(result, 42);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_Messenger_RequestMessage_Fail_NoReply(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        NumberRequestMessage message = messenger.Send<NumberRequestMessage>();

        Assert.IsFalse(message.HasReceivedResponse);

        _ = message.Response;
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_Messenger_RequestMessage_Fail_MultipleReplies(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        static void Receive(object recipient, NumberRequestMessage m)
        {
            m.Reply(42);
            m.Reply(42);
        }

        messenger.Register<NumberRequestMessage>(recipient, Receive);

        int result = messenger.Send<NumberRequestMessage>();

        GC.KeepAlive(recipient);
    }

    public class NumberRequestMessage : RequestMessage<int>
    {
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public async Task Test_Messenger_AsyncRequestMessage_Ok_Sync(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        static void Receive(object recipient, AsyncNumberRequestMessage m)
        {
            Assert.IsFalse(m.HasReceivedResponse);

            m.Reply(42);

            Assert.IsTrue(m.HasReceivedResponse);
        }

        messenger.Register<AsyncNumberRequestMessage>(recipient, Receive);

        int result = await messenger.Send<AsyncNumberRequestMessage>();

        Assert.AreEqual(result, 42);

        GC.KeepAlive(recipient);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public async Task Test_Messenger_AsyncRequestMessage_Ok_Async(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        async Task<int> GetNumberAsync()
        {
            await Task.Delay(100);

            return 42;
        }

        void Receive(object recipient, AsyncNumberRequestMessage m)
        {
            Assert.IsFalse(m.HasReceivedResponse);

            m.Reply(GetNumberAsync());

            Assert.IsTrue(m.HasReceivedResponse);
        }

        messenger.Register<AsyncNumberRequestMessage>(recipient, Receive);

        int result = await messenger.Send<AsyncNumberRequestMessage>();

        Assert.AreEqual(result, 42);

        GC.KeepAlive(recipient);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task Test_Messenger_AsyncRequestMessage_Fail_NoReply(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        AsyncNumberRequestMessage message = messenger.Send<AsyncNumberRequestMessage>();

        Assert.IsFalse(message.HasReceivedResponse);

        _ = await message.Response;
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task Test_Messenger_AsyncRequestMessage_Fail_MultipleReplies(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        static void Receive(object recipient, AsyncNumberRequestMessage m)
        {
            m.Reply(42);
            m.Reply(42);
        }

        messenger.Register<AsyncNumberRequestMessage>(recipient, Receive);

        int result = await messenger.Send<AsyncNumberRequestMessage>();

        GC.KeepAlive(recipient);
    }

    public class AsyncNumberRequestMessage : AsyncRequestMessage<int>
    {
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_CollectionRequestMessage_Ok_NoReplies(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        static void Receive(object recipient, NumbersCollectionRequestMessage m)
        {
        }

        messenger.Register<NumbersCollectionRequestMessage>(recipient, Receive);

        IReadOnlyCollection<int>? results = messenger.Send<NumbersCollectionRequestMessage>().Responses;

        Assert.AreEqual(results.Count, 0);

        GC.KeepAlive(recipient);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_CollectionRequestMessage_Ok_MultipleReplies(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object recipient1 = new();
        object recipient2 = new();
        object recipient3 = new();
        object? r1 = null;
        object? r2 = null;
        object? r3 = null;

        void Receive1(object recipient, NumbersCollectionRequestMessage m)
        {
            r1 = recipient;
            m.Reply(1);
        }

        void Receive2(object recipient, NumbersCollectionRequestMessage m)
        {
            r2 = recipient;
            m.Reply(2);
        }

        void Receive3(object recipient, NumbersCollectionRequestMessage m)
        {
            r3 = recipient;
            m.Reply(3);
        }

        messenger.Register<NumbersCollectionRequestMessage>(recipient1, Receive1);
        messenger.Register<NumbersCollectionRequestMessage>(recipient2, Receive2);
        messenger.Register<NumbersCollectionRequestMessage>(recipient3, Receive3);

        List<int> responses = new();

        foreach (int response in messenger.Send<NumbersCollectionRequestMessage>())
        {
            responses.Add(response);
        }

        Assert.AreSame(r1, recipient1);
        Assert.AreSame(r2, recipient2);
        Assert.AreSame(r3, recipient3);

        CollectionAssert.AreEquivalent(responses, new[] { 1, 2, 3 });
    }

    public class NumbersCollectionRequestMessage : CollectionRequestMessage<int>
    {
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public async Task Test_Messenger_AsyncCollectionRequestMessage_Ok_NoReplies(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        static void Receive(object recipient, AsyncNumbersCollectionRequestMessage m)
        {
        }

        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient, Receive);

        IReadOnlyCollection<int>? results = await messenger.Send<AsyncNumbersCollectionRequestMessage>().GetResponsesAsync();

        Assert.AreEqual(results.Count, 0);

        GC.KeepAlive(recipient);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public async Task Test_Messenger_AsyncCollectionRequestMessage_Ok_MultipleReplies(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object recipient1 = new();
        object recipient2 = new();
        object recipient3 = new();
        object recipient4 = new();

        async Task<int> GetNumberAsync()
        {
            await Task.Delay(100);

            return 3;
        }

        void Receive1(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(1);
        void Receive2(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(Task.FromResult(2));
        void Receive3(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(GetNumberAsync());
        void Receive4(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(_ => GetNumberAsync());

        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient1, Receive1);
        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient2, Receive2);
        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient3, Receive3);
        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient4, Receive4);

        IReadOnlyCollection<int>? responses = await messenger.Send<AsyncNumbersCollectionRequestMessage>().GetResponsesAsync();

        CollectionAssert.AreEquivalent(responses.ToArray(), new[] { 1, 2, 3, 3 });

        GC.KeepAlive(recipient1);
        GC.KeepAlive(recipient2);
        GC.KeepAlive(recipient3);
        GC.KeepAlive(recipient4);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public async Task Test_Messenger_AsyncCollectionRequestMessage_Ok_MultipleReplies_Enumerate(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object recipient1 = new();
        object recipient2 = new();
        object recipient3 = new();
        object recipient4 = new();

        async Task<int> GetNumberAsync()
        {
            await Task.Delay(100);

            return 3;
        }

        void Receive1(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(1);
        void Receive2(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(Task.FromResult(2));
        void Receive3(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(GetNumberAsync());
        void Receive4(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(_ => GetNumberAsync());

        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient1, Receive1);
        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient2, Receive2);
        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient3, Receive3);
        messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient4, Receive4);

        List<int> responses = new();

        await foreach (int response in messenger.Send<AsyncNumbersCollectionRequestMessage>())
        {
            responses.Add(response);
        }

        CollectionAssert.AreEquivalent(responses, new[] { 1, 2, 3, 3 });

        GC.KeepAlive(recipient1);
        GC.KeepAlive(recipient2);
        GC.KeepAlive(recipient3);
        GC.KeepAlive(recipient4);
    }

    public class AsyncNumbersCollectionRequestMessage : AsyncCollectionRequestMessage<int>
    {
    }
}

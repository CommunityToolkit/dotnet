// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_Messenger
{
    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_UnregisterRecipientWithMessageType(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.Unregister<MessageA>(recipient);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_UnregisterRecipientWithMessageTypeAndToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.Unregister<MessageA, string>(recipient, nameof(MessageA));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_UnregisterRecipientWithToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.UnregisterAll(recipient, nameof(MessageA));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_UnregisterRecipientWithRecipient(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.UnregisterAll(recipient);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_RegisterAndUnregisterRecipientWithMessageType(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.Register<MessageA>(recipient, (r, m) => { });

        Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));

        messenger.Unregister<MessageA>(recipient);

        Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_RegisterAndUnregisterRecipientWithMessageTypeAndToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });

        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));

        messenger.Unregister<MessageA, string>(recipient, nameof(MessageA));

        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_RegisterAndUnregisterRecipientWithMessageTypeAndMultipleTokens(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "b"));

        messenger.Register<MessageA, int>(recipient, 1, (r, m) => { });

        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "b"));

        messenger.Register<MessageA, int>(recipient, 2, (r, m) => { });

        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "b"));

        messenger.Register<MessageA, string>(recipient, "a", (r, m) => { });

        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "b"));

        messenger.Register<MessageA, string>(recipient, "b", (r, m) => { });

        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, "b"));

        messenger.Unregister<MessageA, int>(recipient, 1);

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, "b"));

        messenger.Unregister<MessageA, int>(recipient, 2);

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, "b"));

        messenger.Unregister<MessageA, string>(recipient, "a");

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, "b"));

        messenger.Unregister<MessageA, string>(recipient, "b");

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 1));
        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipient, 2));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "a"));
        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, "b"));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_RegisterAndUnregisterRecipientWithToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });

        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));

        messenger.UnregisterAll(recipient, nameof(MessageA));

        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_RegisterAndUnregisterRecipientWithRecipient(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });

        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));

        messenger.UnregisterAll(recipient);

        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_IsRegistered_Register_Send_UnregisterOfTMessage_WithNoToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object a = new();

        Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

        object? recipient = null;
        string? result = null;

        messenger.Register<MessageA>(a, (r, m) =>
        {
            recipient = r;
            result = m.Text;
        });

        Assert.IsTrue(messenger.IsRegistered<MessageA>(a));

        _ = messenger.Send(new MessageA { Text = nameof(MessageA) });

        Assert.AreSame(recipient, a);
        Assert.AreEqual(result, nameof(MessageA));

        messenger.Unregister<MessageA>(a);

        Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

        recipient = null;
        result = null;

        _ = messenger.Send(new MessageA { Text = nameof(MessageA) });

        Assert.IsNull(recipient);
        Assert.IsNull(result);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_IsRegistered_Register_Send_UnregisterRecipient_WithNoToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object a = new();

        Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

        string? result = null;
        messenger.Register<MessageA>(a, (r, m) => result = m.Text);

        Assert.IsTrue(messenger.IsRegistered<MessageA>(a));

        _ = messenger.Send(new MessageA { Text = nameof(MessageA) });

        Assert.AreEqual(result, nameof(MessageA));

        messenger.UnregisterAll(a);

        Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

        result = null;
        _ = messenger.Send(new MessageA { Text = nameof(MessageA) });

        Assert.IsNull(result);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_IsRegistered_Register_Send_UnregisterOfTMessage_WithToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object a = new();

        Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

        string? result = null;
        messenger.Register<MessageA, string>(a, nameof(MessageA), (r, m) => result = m.Text);

        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(a, nameof(MessageA)));

        _ = messenger.Send(new MessageA { Text = nameof(MessageA) }, nameof(MessageA));

        Assert.AreEqual(result, nameof(MessageA));

        messenger.Unregister<MessageA, string>(a, nameof(MessageA));

        Assert.IsFalse(messenger.IsRegistered<MessageA, string>(a, nameof(MessageA)));

        result = null;
        _ = messenger.Send(new MessageA { Text = nameof(MessageA) }, nameof(MessageA));

        Assert.IsNull(result);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_DuplicateRegistrationWithMessageType(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.Register<MessageA>(recipient, (r, m) => { });

        _ = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            messenger.Register<MessageA>(recipient, (r, m) => { });
        });
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_DuplicateRegistrationWithMessageTypeAndToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        object? recipient = new();

        messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });

        _ = Assert.ThrowsException<InvalidOperationException>(() =>
        {
            messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });
        });
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_IRecipient_NoMessages(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        RecipientWithNoMessages? recipient = new();

        messenger.RegisterAll(recipient);

        // We just need to verify we got here with no errors, this
        // recipient has no declared handlers so there's nothing to do
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_IRecipient_SomeMessages_NoToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        RecipientWithSomeMessages? recipient = new();

        messenger.RegisterAll(recipient);

        Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
        Assert.IsTrue(messenger.IsRegistered<MessageB>(recipient));

        Assert.AreEqual(recipient.As, 0);
        Assert.AreEqual(recipient.Bs, 0);

        _ = messenger.Send<MessageA>();

        Assert.AreEqual(recipient.As, 1);
        Assert.AreEqual(recipient.Bs, 0);

        _ = messenger.Send<MessageB>();

        Assert.AreEqual(recipient.As, 1);
        Assert.AreEqual(recipient.Bs, 1);

        messenger.UnregisterAll(recipient);

        Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
        Assert.IsFalse(messenger.IsRegistered<MessageB>(recipient));
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/198
    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_IRecipient_SomeMessages_WithGenericTypeParameters(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        GenericRecipient<string> recipient = new();

        messenger.RegisterAll(recipient);

        Assert.IsTrue(messenger.IsRegistered<GenericRecipient<string>.Message>(recipient));
        Assert.IsTrue(messenger.IsRegistered<ValueChangedMessage<string>>(recipient));
        Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));

        Assert.IsNull(recipient.ReceivedMessage1);
        Assert.IsNull(recipient.ReceivedMessage2);
        Assert.IsNull(recipient.ReceivedMessage3);

        GenericRecipient<string>.Message message1 = messenger.Send(new GenericRecipient<string>.Message(new List<string> { "Hello world!" }));
        ValueChangedMessage<string> message2 = messenger.Send(new ValueChangedMessage<string>("Hello"));
        MessageA message3 = messenger.Send<MessageA>();

        Assert.AreSame(recipient.ReceivedMessage1, message1);
        Assert.AreSame(recipient.ReceivedMessage2, message2);
        Assert.AreSame(recipient.ReceivedMessage3, message3);

        messenger.UnregisterAll(recipient);

        Assert.IsFalse(messenger.IsRegistered<GenericRecipient<string>.Message>(recipient));
        Assert.IsFalse(messenger.IsRegistered<ValueChangedMessage<string>>(recipient));
        Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_IRecipient_SomeMessages_WithToken(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        RecipientWithSomeMessages? recipient = new();
        string? token = nameof(Test_Messenger_IRecipient_SomeMessages_WithToken);

        messenger.RegisterAll(recipient, token);

        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, token));
        Assert.IsTrue(messenger.IsRegistered<MessageB, string>(recipient, token));

        Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
        Assert.IsFalse(messenger.IsRegistered<MessageB>(recipient));

        Assert.AreEqual(recipient.As, 0);
        Assert.AreEqual(recipient.Bs, 0);

        _ = messenger.Send<MessageB, string>(token);

        Assert.AreEqual(recipient.As, 0);
        Assert.AreEqual(recipient.Bs, 1);

        _ = messenger.Send<MessageA, string>(token);

        Assert.AreEqual(recipient.As, 1);
        Assert.AreEqual(recipient.Bs, 1);

        messenger.UnregisterAll(recipient, token);

        Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
        Assert.IsFalse(messenger.IsRegistered<MessageB>(recipient));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_RegisterWithTypeParameter(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        RecipientWithNoMessages? recipient = new() { Number = 42 };

        int number = 0;

        messenger.Register<RecipientWithNoMessages, MessageA>(recipient, (r, m) => number = r.Number);

        _ = messenger.Send<MessageA>();

        Assert.AreEqual(number, 42);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger), false)]
    [DataRow(typeof(WeakReferenceMessenger), true)]
    public void Test_Messenger_Collect_Test(Type type, bool isWeak)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        WeakReference weakRecipient;

        void Test()
        {
            RecipientWithNoMessages? recipient = new() { Number = 42 };
            weakRecipient = new WeakReference(recipient);

            messenger.Register<MessageA>(recipient, (r, m) => { });

            Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
            Assert.IsTrue(weakRecipient.IsAlive);

            GC.KeepAlive(recipient);
        }

        Test();

        GC.Collect();

        Assert.AreEqual(!isWeak, weakRecipient.IsAlive);

        GC.KeepAlive(messenger);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_Reset(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        RecipientWithSomeMessages? recipient = new();

        messenger.RegisterAll(recipient);

        Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
        Assert.IsTrue(messenger.IsRegistered<MessageB>(recipient));

        messenger.Reset();

        Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
        Assert.IsFalse(messenger.IsRegistered<MessageB>(recipient));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_Default(Type type)
    {
        PropertyInfo defaultInfo = type.GetProperty("Default")!;

        object? default1 = defaultInfo!.GetValue(null);
        object? default2 = defaultInfo!.GetValue(null);

        Assert.IsNotNull(default1);
        Assert.IsNotNull(default2);
        Assert.AreSame(default1, default2);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_Cleanup(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;
        RecipientWithSomeMessages? recipient = new();

        messenger.Register<MessageA>(recipient);

        Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));

        void Test()
        {
            RecipientWithSomeMessages? recipient2 = new();

            messenger.Register<MessageB>(recipient2);

            Assert.IsTrue(messenger.IsRegistered<MessageB>(recipient2));

            GC.KeepAlive(recipient2);
        }

        Test();

        GC.Collect();

        // Here we just check that calling Cleanup doesn't alter the state
        // of the messenger. This method shouldn't really do anything visible
        // to consumers, it's just a way for messengers to compact their data.
        messenger.Cleanup();

        Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_ManyRecipients(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        void Test()
        {
            RecipientWithSomeMessages?[] recipients = Enumerable.Range(0, 512).Select(_ => new RecipientWithSomeMessages()).ToArray();

            foreach (RecipientWithSomeMessages? recipient in recipients)
            {
                messenger.RegisterAll(recipient!);
            }

            foreach (RecipientWithSomeMessages? recipient in recipients)
            {
                Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient!));
                Assert.IsTrue(messenger.IsRegistered<MessageB>(recipient!));
            }

            _ = messenger.Send<MessageA>();
            _ = messenger.Send<MessageB>();
            _ = messenger.Send<MessageB>();

            foreach (RecipientWithSomeMessages? recipient in recipients)
            {
                Assert.AreEqual(recipient!.As, 1);
                Assert.AreEqual(recipient!.Bs, 2);
            }

            foreach (ref RecipientWithSomeMessages? recipient in recipients.AsSpan())
            {
                recipient = null;
            }
        }

        Test();

        GC.Collect();

        // Just invoke a final cleanup to improve coverage, this is unrelated to this test in particular
        messenger.Cleanup();
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4081
    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_RegisterMultiple_UnregisterSingle(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        object? recipient1 = new();
        object? recipient2 = new();

        int handler1CalledCount = 0;
        int handler2CalledCount = 0;

        messenger.Register<object, MessageA>(recipient1, (r, m) => { handler1CalledCount++; });
        messenger.Register<object, MessageA>(recipient2, (r, m) => { handler2CalledCount++; });

        messenger.UnregisterAll(recipient2);

        _ = messenger.Send(new MessageA());

        Assert.AreEqual(1, handler1CalledCount);
        Assert.AreEqual(0, handler2CalledCount);
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_ConcurrentOperations_DefaultChannel_SeparateRecipients(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        RecipientWithConcurrency[] recipients = Enumerable.Range(0, 1024).Select(static _ => new RecipientWithConcurrency()).ToArray();
        DateTime end = DateTime.Now.AddSeconds(5);

        _ = Parallel.For(0, recipients.Length, i =>
        {
            RecipientWithConcurrency r = recipients[i];
            Random random = new(i);

            while (DateTime.Now < end)
            {
                Assert.IsFalse(messenger.IsRegistered<MessageA>(r));
                Assert.IsFalse(messenger.IsRegistered<MessageB>(r));

                // Here we randomize the way messages are subscribed. This ensures that we're testing both the normal
                // registration with a delegate, as well as the fast path within WeakReferenceMessenger that is enabled
                // when IRecipient<TMessage> is used. Since broadcasts are done in parallel here, it will happen that
                // some messages are broadcast when multiple recipients are active through both types of registration.
                // This test verifies that the messengers work fine when this is the case (and not just when only one
                // of the two method is used, as mixing them up as necessary is a perfectly supported scenario as well).
                switch (random.Next(0, 3))
                {
                    case 0:
                        messenger.Register<RecipientWithConcurrency, MessageA>(r, static (r, m) => r.Receive(m));
                        messenger.Register<RecipientWithConcurrency, MessageB>(r, static (r, m) => r.Receive(m));
                        break;
                    case 1:
                        messenger.Register<MessageA>(r);
                        messenger.Register<MessageB>(r);
                        break;
                    default:
                        messenger.RegisterAll(r);
                        break;
                }

                Assert.IsTrue(messenger.IsRegistered<MessageA>(r));
                Assert.IsTrue(messenger.IsRegistered<MessageB>(r));

                int a = r.As;
                int b = r.Bs;

                _ = messenger.Send<MessageA>();

                // We can't just check that the count has been increased by 1, as there may be other concurrent broadcasts
                Assert.IsTrue(r.As > a);

                _ = messenger.Send<MessageB>();

                Assert.IsTrue(r.Bs > b);

                switch (random.Next(0, 2))
                {
                    case 0:
                        messenger.Unregister<MessageA>(r);
                        messenger.Unregister<MessageB>(r);
                        break;
                    default:
                        messenger.UnregisterAll(r);
                        break;
                }
            }
        });
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_ConcurrentOperations_DefaultChannel_SharedRecipients(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        RecipientWithConcurrency[] recipients = Enumerable.Range(0, 512).Select(static _ => new RecipientWithConcurrency()).ToArray();
        DateTime end = DateTime.Now.AddSeconds(5);

        _ = Parallel.For(0, recipients.Length * 2, i =>
        {
            RecipientWithConcurrency r = recipients[i / 2];
            Random random = new(i);

            while (DateTime.Now < end)
            {
                if (i % 2 == 0)
                {
                    Assert.IsFalse(messenger.IsRegistered<MessageA>(r));

                    switch (random.Next(0, 2))
                    {
                        case 0:
                            messenger.Register<RecipientWithConcurrency, MessageA>(r, static (r, m) => r.Receive(m));
                            break;
                        default:
                            messenger.Register<MessageA>(r);
                            break;
                    }

                    Assert.IsTrue(messenger.IsRegistered<MessageA>(r));

                    int a = r.As;

                    _ = messenger.Send<MessageA>();

                    Assert.IsTrue(r.As > a);

                    messenger.Unregister<MessageA>(r);
                }
                else
                {
                    Assert.IsFalse(messenger.IsRegistered<MessageB>(r));

                    switch (random.Next(0, 2))
                    {
                        case 0:
                            messenger.Register<RecipientWithConcurrency, MessageB>(r, static (r, m) => r.Receive(m));
                            break;
                        default:
                            messenger.Register<MessageB>(r);
                            break;
                    }

                    Assert.IsTrue(messenger.IsRegistered<MessageB>(r));

                    int b = r.Bs;

                    _ = messenger.Send<MessageB>();

                    Assert.IsTrue(r.Bs > b);

                    messenger.Unregister<MessageB>(r);
                }
            }
        });
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_ConcurrentOperations_WithToken_SeparateRecipients(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        RecipientWithConcurrency[] recipients = Enumerable.Range(0, 1024).Select(static _ => new RecipientWithConcurrency()).ToArray();
        string[] tokens = Enumerable.Range(0, 16).Select(static i => i.ToString()).ToArray();
        DateTime end = DateTime.Now.AddSeconds(5);

        _ = Parallel.For(0, recipients.Length, i =>
        {
            RecipientWithConcurrency r = recipients[i];
            string token = tokens[i % tokens.Length];
            Random random = new(i);

            while (DateTime.Now < end)
            {
                Assert.IsFalse(messenger.IsRegistered<MessageA, string>(r, token));
                Assert.IsFalse(messenger.IsRegistered<MessageB, string>(r, token));

                switch (random.Next(0, 3))
                {
                    case 0:
                        messenger.Register<RecipientWithConcurrency, MessageA, string>(r, token, static (r, m) => r.Receive(m));
                        messenger.Register<RecipientWithConcurrency, MessageB, string>(r, token, static (r, m) => r.Receive(m));
                        break;
                    case 1:
                        messenger.Register<MessageA, string>(r, token);
                        messenger.Register<MessageB, string>(r, token);
                        break;
                    default:
                        messenger.RegisterAll(r, token);
                        break;
                }

                Assert.IsTrue(messenger.IsRegistered<MessageA, string>(r, token));
                Assert.IsTrue(messenger.IsRegistered<MessageB, string>(r, token));

                int a = r.As;
                int b = r.Bs;

                _ = messenger.Send<MessageA, string>(token);

                Assert.IsTrue(r.As > a);

                _ = messenger.Send<MessageB, string>(token);

                Assert.IsTrue(r.Bs > b);

                switch (random.Next(0, 3))
                {
                    case 0:
                        messenger.Unregister<MessageA, string>(r, token);
                        messenger.Unregister<MessageB, string>(r, token);
                        break;
                    case 1:
                        messenger.UnregisterAll(r, token);
                        break;
                    default:
                        messenger.UnregisterAll(r);
                        break;
                }
            }
        });
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_ConcurrentOperations_WithToken_SharedRecipients(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        RecipientWithConcurrency[] recipients = Enumerable.Range(0, 512).Select(static _ => new RecipientWithConcurrency()).ToArray();
        string[] tokens = Enumerable.Range(0, 16).Select(static i => i.ToString()).ToArray();
        DateTime end = DateTime.Now.AddSeconds(5);

        _ = Parallel.For(0, recipients.Length * 2, i =>
        {
            RecipientWithConcurrency r = recipients[i / 2];
            string token = tokens[i % tokens.Length];
            Random random = new(i);

            while (DateTime.Now < end)
            {
                if (i % 2 == 0)
                {
                    Assert.IsFalse(messenger.IsRegistered<MessageA, string>(r, token));

                    switch (random.Next(0, 2))
                    {
                        case 0:
                            messenger.Register<RecipientWithConcurrency, MessageA, string>(r, token, static (r, m) => r.Receive(m));
                            break;
                        default:
                            messenger.Register<MessageA, string>(r, token);
                            break;
                    }

                    Assert.IsTrue(messenger.IsRegistered<MessageA, string>(r, token));

                    int a = r.As;

                    _ = messenger.Send<MessageA, string>(token);

                    Assert.IsTrue(r.As > a);

                    messenger.Unregister<MessageA, string>(r, token);
                }
                else
                {
                    Assert.IsFalse(messenger.IsRegistered<MessageB, string>(r, token));

                    switch (random.Next(0, 2))
                    {
                        case 0:
                            messenger.Register<RecipientWithConcurrency, MessageB, string>(r, token, static (r, m) => r.Receive(m));
                            break;
                        default:
                            messenger.Register<MessageB, string>(r, token);
                            break;
                    }

                    Assert.IsTrue(messenger.IsRegistered<MessageB, string>(r, token));

                    int b = r.Bs;

                    _ = messenger.Send<MessageB, string>(token);

                    Assert.IsTrue(r.Bs > b);

                    messenger.Unregister<MessageB, string>(r, token);
                }
            }
        });
    }

    [TestMethod]
    [DataRow(typeof(StrongReferenceMessenger))]
    [DataRow(typeof(WeakReferenceMessenger))]
    public void Test_Messenger_ConcurrentOperations_Combined_SeparateRecipients(Type type)
    {
        IMessenger? messenger = (IMessenger)Activator.CreateInstance(type)!;

        RecipientWithConcurrency[] recipients = Enumerable.Range(0, 1024).Select(static _ => new RecipientWithConcurrency()).ToArray();
        string[] wordTokens = Enumerable.Range(0, 16).Select(static i => i.ToString()).ToArray();
        int[] numberTokens = Enumerable.Range(0, 16).ToArray();
        DateTime end = DateTime.Now.AddSeconds(5);

        _ = Parallel.For(0, recipients.Length, i =>
        {
            RecipientWithConcurrency r = recipients[i];
            string wordToken = wordTokens[i % wordTokens.Length];
            int numberToken = numberTokens[i % numberTokens.Length];
            Random random = new(i);

            while (DateTime.Now < end)
            {
                Assert.IsFalse(messenger.IsRegistered<MessageA, string>(r, wordToken));
                Assert.IsFalse(messenger.IsRegistered<MessageB, string>(r, wordToken));
                Assert.IsFalse(messenger.IsRegistered<MessageA, int>(r, numberToken));
                Assert.IsFalse(messenger.IsRegistered<MessageB, int>(r, numberToken));
                Assert.IsFalse(messenger.IsRegistered<MessageA>(r));
                Assert.IsFalse(messenger.IsRegistered<MessageB>(r));

                int choice = random.Next(0, 9);

                switch (choice)
                {
                    case 0:
                        messenger.Register<RecipientWithConcurrency, MessageA, string>(r, wordToken, static (r, m) => r.Receive(m));
                        messenger.Register<RecipientWithConcurrency, MessageB, string>(r, wordToken, static (r, m) => r.Receive(m));

                        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(r, wordToken));
                        Assert.IsTrue(messenger.IsRegistered<MessageB, string>(r, wordToken));
                        break;
                    case 1:
                        messenger.Register<RecipientWithConcurrency, MessageA, int>(r, numberToken, static (r, m) => r.Receive(m));
                        messenger.Register<RecipientWithConcurrency, MessageB, int>(r, numberToken, static (r, m) => r.Receive(m));

                        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(r, numberToken));
                        Assert.IsTrue(messenger.IsRegistered<MessageB, int>(r, numberToken));
                        break;
                    case 2:
                        messenger.Register<RecipientWithConcurrency, MessageA>(r, static (r, m) => r.Receive(m));
                        messenger.Register<RecipientWithConcurrency, MessageB>(r, static (r, m) => r.Receive(m));

                        Assert.IsTrue(messenger.IsRegistered<MessageA>(r));
                        Assert.IsTrue(messenger.IsRegistered<MessageB>(r));
                        break;
                    case 3:
                        messenger.Register<MessageA, string>(r, wordToken);
                        messenger.Register<MessageB, string>(r, wordToken);

                        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(r, wordToken));
                        Assert.IsTrue(messenger.IsRegistered<MessageB, string>(r, wordToken));
                        break;
                    case 4:
                        messenger.Register<MessageA, int>(r, numberToken);
                        messenger.Register<MessageB, int>(r, numberToken);

                        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(r, numberToken));
                        Assert.IsTrue(messenger.IsRegistered<MessageB, int>(r, numberToken));
                        break;
                    case 5:
                        messenger.Register<MessageA>(r);
                        messenger.Register<MessageB>(r);

                        Assert.IsTrue(messenger.IsRegistered<MessageA>(r));
                        Assert.IsTrue(messenger.IsRegistered<MessageB>(r));
                        break;
                    case 6:
                        messenger.RegisterAll(r, wordToken);

                        Assert.IsTrue(messenger.IsRegistered<MessageA, string>(r, wordToken));
                        Assert.IsTrue(messenger.IsRegistered<MessageB, string>(r, wordToken));
                        break;
                    case 7:
                        messenger.RegisterAll(r, numberToken);

                        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(r, numberToken));
                        Assert.IsTrue(messenger.IsRegistered<MessageB, int>(r, numberToken));
                        break;
                    default:
                        messenger.RegisterAll(r);

                        Assert.IsTrue(messenger.IsRegistered<MessageA>(r));
                        Assert.IsTrue(messenger.IsRegistered<MessageB>(r));
                        break;
                }

                int a = r.As;
                int b = r.Bs;

                if (choice is 0 or 3 or 6)
                {
                    _ = messenger.Send<MessageA, string>(wordToken);
                    _ = messenger.Send<MessageB, string>(wordToken);

                    messenger.UnregisterAll(r, wordToken);
                }
                else if (choice is 1 or 4 or 7)
                {
                    _ = messenger.Send<MessageA, int>(numberToken);
                    _ = messenger.Send<MessageB, int>(numberToken);

                    messenger.UnregisterAll(r, numberToken);
                }
                else
                {
                    _ = messenger.Send<MessageA>();
                    _ = messenger.Send<MessageB>();

                    messenger.UnregisterAll(r);
                }

                Assert.IsTrue(r.As > a);
                Assert.IsTrue(r.Bs > b);
            }
        });
    }

    public sealed class RecipientWithNoMessages
    {
        public int Number { get; set; }
    }

    public sealed class RecipientWithSomeMessages :
        IRecipient<MessageA>,
        IRecipient<MessageB>,
        ICloneable
    {
        public int As { get; private set; }

        public int Bs { get; private set; }

        public void Receive(MessageA message)
        {
            As++;
        }

        public void Receive(MessageB message)
        {
            Bs++;
        }

        // We also add the ICloneable interface to test that the message
        // interfaces are all handled correctly even when interleaved
        // by other unrelated interfaces in the type declaration.
        public object Clone() => throw new NotImplementedException();
    }

    public sealed class RecipientWithConcurrency :
        IRecipient<MessageA>,
        IRecipient<MessageB>
    {
        private volatile int a;
        private volatile int b;

        public int As => this.a;

        public int Bs => this.b;

        public void Receive(MessageA message)
        {
            _ = Interlocked.Increment(ref this.a);
        }

        public void Receive(MessageB message)
        {
            _ = Interlocked.Increment(ref this.b);
        }
    }

    public sealed class MessageA
    {
        public string? Text { get; set; }
    }

    public sealed class MessageB
    {
    }

    public partial class GenericRecipient<T> : IRecipient<GenericRecipient<T>.Message>, IRecipient<ValueChangedMessage<T>>, IRecipient<MessageA>
    {
        public Message? ReceivedMessage1 { get; private set; }

        public ValueChangedMessage<T>? ReceivedMessage2 { get; private set; }

        public MessageA? ReceivedMessage3 { get; private set; }

        public void Receive(Message message)
        {
            ReceivedMessage1 = message;
        }

        public void Receive(ValueChangedMessage<T> message)
        {
            ReceivedMessage2 = message;
        }

        public void Receive(MessageA message)
        {
            ReceivedMessage3 = message;
        }

        public sealed class Message : ValueChangedMessage<List<T>>
        {
            public Message(List<T> list)
                : base(list)
            {
            }
        }
    }
}

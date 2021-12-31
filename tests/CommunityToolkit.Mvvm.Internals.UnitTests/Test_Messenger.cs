// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.Internals.UnitTests;

[TestClass]
public partial class Test_Messenger
{
#if NETCOREAPP // Auto-trimming is disabled on .NET Framework
    [TestMethod]
    public void Test_WeakReferenceMessenger_AutoCleanup()
    {
        IMessenger messenger = new WeakReferenceMessenger();

        static int GetRecipientsMapCount(IMessenger messenger)
        {
            object recipientsMap =
                typeof(WeakReferenceMessenger)
                .GetField("recipientsMap", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(messenger)!;

            return (int)recipientsMap.GetType().GetProperty("Count")!.GetValue(recipientsMap)!;
        }

        WeakReference weakRecipient;

        void Test()
        {
            RecipientWithSomeMessages? recipient = new();
            weakRecipient = new WeakReference(recipient);

            messenger.Register<MessageA>(recipient);

            Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));

            Assert.AreEqual(GetRecipientsMapCount(messenger), 1);

            GC.KeepAlive(recipient);
        }

        Test();

        GC.Collect();

        Assert.IsFalse(weakRecipient.IsAlive);

        // Now that the recipient is collected, trigger another full GC collection
        // to let the automatic cleanup callback run and trim the messenger data
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.AreEqual(GetRecipientsMapCount(messenger), 0);

        GC.KeepAlive(messenger);
    }
#endif

    [TestMethod]
    public void Test_StrongReferenceMessenger_AutoTrimming_UnregisterAll()
    {
        StrongReferenceMessenger messenger = new();
        IDictionary2 recipientsMap = (IDictionary2)typeof(StrongReferenceMessenger).GetField("recipientsMap", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(messenger)!;
        IDictionary2<Type2, object> typesMap = (IDictionary2<Type2, object>)typeof(StrongReferenceMessenger).GetField("typesMap", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(messenger)!;

        RecipientWithSomeMessages recipientA = new();
        RecipientWithSomeMessages recipientB = new();

        messenger.Register<MessageA>(recipientA);

        Assert.IsTrue(messenger.IsRegistered<MessageA>(recipientA));

        // There's one registered handler
        Assert.AreEqual(1, recipientsMap.Count);
        Assert.AreEqual(1, typesMap.Count);

        messenger.Register<MessageB>(recipientA);

        Assert.IsTrue(messenger.IsRegistered<MessageB>(recipientA));

        // There's two message types, for the same recipient
        Assert.AreEqual(1, recipientsMap.Count);
        Assert.AreEqual(2, typesMap.Count);

        messenger.Register<MessageA>(recipientB);

        Assert.IsTrue(messenger.IsRegistered<MessageA>(recipientB));

        // Now there's two recipients too
        Assert.AreEqual(2, recipientsMap.Count);
        Assert.AreEqual(2, typesMap.Count);

        messenger.UnregisterAll(recipientA);

        Assert.IsFalse(messenger.IsRegistered<MessageA>(recipientA));
        Assert.IsFalse(messenger.IsRegistered<MessageB>(recipientA));

        // Only the second recipient is left, which has a single message type
        Assert.AreEqual(1, recipientsMap.Count);
        Assert.AreEqual(1, typesMap.Count);

        messenger.UnregisterAll(recipientB);

        Assert.IsFalse(messenger.IsRegistered<MessageB>(recipientA));

        // Now both lists should be empty
        Assert.AreEqual(0, recipientsMap.Count);
        Assert.AreEqual(0, typesMap.Count);
    }

    [TestMethod]
    public void Test_StrongReferenceMessenger_AutoTrimming_UnregisterAllWithToken()
    {
        StrongReferenceMessenger messenger = new();
        IDictionary2 recipientsMap = (IDictionary2)typeof(StrongReferenceMessenger).GetField("recipientsMap", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(messenger)!;
        IDictionary2<Type2, object> typesMap = (IDictionary2<Type2, object>)typeof(StrongReferenceMessenger).GetField("typesMap", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(messenger)!;

        RecipientWithSomeMessages recipientA = new();
        RecipientWithSomeMessages recipientB = new();

        messenger.Register<MessageA, int>(recipientA, 42);
        messenger.Register<MessageB, int>(recipientA, 42);
        messenger.Register<MessageA, int>(recipientB, 42);

        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipientA, 42));
        Assert.IsTrue(messenger.IsRegistered<MessageB, int>(recipientA, 42));
        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipientB, 42));

        // There are two recipients, for two message types
        Assert.AreEqual(2, recipientsMap.Count);
        Assert.AreEqual(2, typesMap.Count);

        messenger.UnregisterAll(recipientA, 42);

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipientA, 42));
        Assert.IsFalse(messenger.IsRegistered<MessageB, int>(recipientA, 42));

        // Only the second recipient is left again
        Assert.AreEqual(1, recipientsMap.Count);
        Assert.AreEqual(1, typesMap.Count);

        messenger.UnregisterAll(recipientB, 42);

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipientB, 42));

        // Now both lists should be empty
        Assert.AreEqual(0, recipientsMap.Count);
        Assert.AreEqual(0, typesMap.Count);
    }

    [TestMethod]
    public void Test_StrongReferenceMessenger_AutoTrimming_UnregisterAllWithMessageTypeAndToken()
    {
        StrongReferenceMessenger messenger = new();
        IDictionary2 recipientsMap = (IDictionary2)typeof(StrongReferenceMessenger).GetField("recipientsMap", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(messenger)!;
        IDictionary2<Type2, object> typesMap = (IDictionary2<Type2, object>)typeof(StrongReferenceMessenger).GetField("typesMap", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(messenger)!;

        RecipientWithSomeMessages recipientA = new();
        RecipientWithSomeMessages recipientB = new();

        messenger.Register<MessageA, int>(recipientA, 42);
        messenger.Register<MessageB, int>(recipientA, 42);
        messenger.Register<MessageA, int>(recipientB, 42);

        messenger.Unregister<MessageA, int>(recipientA, 42);

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipientA, 42));
        Assert.IsTrue(messenger.IsRegistered<MessageB, int>(recipientA, 42));
        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipientB, 42));

        // First recipient is still subscribed to MessageB.
        // The second one has a single subscription to MessageA.
        Assert.AreEqual(2, recipientsMap.Count);
        Assert.AreEqual(2, typesMap.Count);

        messenger.Unregister<MessageB, int>(recipientA, 42);

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipientA, 42));
        Assert.IsFalse(messenger.IsRegistered<MessageB, int>(recipientA, 42));
        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipientB, 42));

        // Only the second recipient is left
        Assert.AreEqual(1, recipientsMap.Count);
        Assert.AreEqual(1, typesMap.Count);

        messenger.Unregister<MessageA, int>(recipientB, 42);

        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipientA, 42));
        Assert.IsFalse(messenger.IsRegistered<MessageB, int>(recipientA, 42));
        Assert.IsFalse(messenger.IsRegistered<MessageA, int>(recipientB, 42));

        // Now both lists should be empty
        Assert.AreEqual(0, recipientsMap.Count);
        Assert.AreEqual(0, typesMap.Count);
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
        // interfaces are all handled correctly even when inteleaved
        // by other unrelated interfaces in the type declaration.
        public object Clone() => throw new NotImplementedException();
    }

    public sealed class MessageA
    {
        public string? Text { get; set; }
    }

    public sealed class MessageB
    {
    }
}

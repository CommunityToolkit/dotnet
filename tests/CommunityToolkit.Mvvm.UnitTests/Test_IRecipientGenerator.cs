// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS0618

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_IRecipientGenerator
{
    [TestMethod]
    public void Test_IRecipientGenerator_GeneratedRegistration()
    {
        StrongReferenceMessenger messenger = new();
        RecipientWithSomeMessages recipient = new();

        MessageA messageA = new();
        MessageB messageB = new();

        Action<IMessenger, object, int> registrator = Messaging.__Internals.__IMessengerExtensions.CreateAllMessagesRegistratorWithToken<int>(recipient);

        registrator(messenger, recipient, 42);

        Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 42));
        Assert.IsTrue(messenger.IsRegistered<MessageB, int>(recipient, 42));

        Assert.IsNull(recipient.A);
        Assert.IsNull(recipient.B);

        _ = messenger.Send(messageA, 42);

        Assert.AreSame(recipient.A, messageA);
        Assert.IsNull(recipient.B);

        _ = messenger.Send(messageB, 42);

        Assert.AreSame(recipient.A, messageA);
        Assert.AreSame(recipient.B, messageB);
    }

    [TestMethod]
    public void Test_IRecipientGenerator_TypeWithMultipleClassDeclarations()
    {
        RecipientWithMultipleClassDeclarations recipient = new();

        // This test really just needs to verify this compiles and executes normally
        _ = Messaging.__Internals.__IMessengerExtensions.CreateAllMessagesRegistratorWithToken<int>(recipient);
    }

    public sealed class RecipientWithSomeMessages :
        IRecipient<MessageA>,
        IRecipient<MessageB>
    {
        public MessageA? A { get; private set; }

        public MessageB? B { get; private set; }

        public void Receive(MessageA message)
        {
            A = message;
        }

        public void Receive(MessageB message)
        {
            B = message;
        }
    }

    public sealed class MessageA
    {
    }

    public sealed class MessageB
    {
    }

    public sealed partial class RecipientWithMultipleClassDeclarations : IRecipient<MessageA>
    {
        public void Receive(MessageA message)
        {
        }
    }

    // This empty partial type declarations needs to be present to ensure the generator
    // correctly handles cases where the source type has multiple class declarations.
    partial class RecipientWithMultipleClassDeclarations
    {
    }
}

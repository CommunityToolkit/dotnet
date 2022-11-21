// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Mvvm.Messaging;

/// <summary>
/// A <see langword="delegate"/> used to represent actions to invoke when a message is received.
/// The recipient is given as an input argument to allow message registrations to avoid creating
/// closures: if an instance method on a recipient needs to be invoked it is possible to just
/// cast the recipient to the right type and then access the local method from that instance.
/// </summary>
/// <typeparam name="TRecipient">The type of recipient for the message.</typeparam>
/// <typeparam name="TMessage">The type of message to receive.</typeparam>
/// <param name="recipient">The recipient that is receiving the message.</param>
/// <param name="message">The message being received.</param>
public delegate void MessageHandler<in TRecipient, in TMessage>(TRecipient recipient, TMessage message)
    where TRecipient : class
    where TMessage : class;

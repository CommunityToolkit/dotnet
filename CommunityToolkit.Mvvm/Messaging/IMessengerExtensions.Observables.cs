// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Messaging.Internals;

namespace CommunityToolkit.Mvvm.Messaging;

/// <inheritdoc/>
partial class IMessengerExtensions
{
    /// <summary>
    /// Creates an <see cref="IObservable{T}"/> instance that can be used to be notified whenever a message of a given type is broadcast by a messenger.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to use to receive notification for through the resulting <see cref="IObservable{T}"/> instance.</typeparam>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
    /// <returns>An <see cref="IObservable{T}"/> instance to receive notifications for <typeparamref name="TMessage"/> messages being broadcast.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> is <see langword="null"/>.</exception>
    public static IObservable<TMessage> CreateObservable<TMessage>(this IMessenger messenger)
        where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(messenger);

        return new Observable<TMessage>(messenger);
    }

    /// <summary>
    /// Creates an <see cref="IObservable{T}"/> instance that can be used to be notified whenever a message of a given type is broadcast by a messenger.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to use to receive notification for through the resulting <see cref="IObservable{T}"/> instance.</typeparam>
    /// <typeparam name="TToken">The type of token to identify what channel to use to receive messages.</typeparam>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
    /// <param name="token">A token used to determine the receiving channel to use.</param>
    /// <returns>An <see cref="IObservable{T}"/> instance to receive notifications for <typeparamref name="TMessage"/> messages being broadcast.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="messenger"/> or <paramref name="token"/> are <see langword="null"/>.</exception>
    public static IObservable<TMessage> CreateObservable<TMessage, TToken>(this IMessenger messenger, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        ArgumentNullException.ThrowIfNull(messenger);
        ArgumentNullException.For<TToken>.ThrowIfNull(token);

        return new Observable<TMessage, TToken>(messenger, token);
    }

    /// <summary>
    /// An <see cref="IObservable{T}"/> implementations for a given message type.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages to listen to.</typeparam>
    private sealed class Observable<TMessage> : IObservable<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// The <see cref="IMessenger"/> instance to use to register the recipient.
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// Creates a new <see cref="Observable{TMessage}"/> instance with the given parameters.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        public Observable(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TMessage> observer)
        {
            return new Recipient(this.messenger, observer);
        }

        /// <summary>
        /// An <see cref="IRecipient{TMessage}"/> implementation for <see cref="Observable{TMessage}"/>.
        /// </summary>
        private sealed class Recipient : IRecipient<TMessage>, IDisposable
        {
            /// <summary>
            /// The <see cref="IMessenger"/> instance to use to register the recipient.
            /// </summary>
            private readonly IMessenger messenger;

            /// <summary>
            /// The target <see cref="IObserver{T}"/> instance currently in use.
            /// </summary>
            private readonly IObserver<TMessage> observer;

            /// <summary>
            /// Creates a new <see cref="Recipient"/> instance with the specified parameters.
            /// </summary>
            /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
            /// <param name="observer">The <see cref="IObserver{T}"/> instance to use to create the recipient for.</param>
            public Recipient(IMessenger messenger, IObserver<TMessage> observer)
            {
                this.messenger = messenger;
                this.observer = observer;

                messenger.Register(this);
            }

            /// <inheritdoc/>
            public void Receive(TMessage message)
            {
                this.observer.OnNext(message);
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                this.messenger.Unregister<TMessage>(this);
            }
        }
    }

    /// <summary>
    /// An <see cref="IObservable{T}"/> implementations for a given pair of message and token types.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages to listen to.</typeparam>
    /// <typeparam name="TToken">The type of token to identify what channel to use to receive messages.</typeparam>
    private sealed class Observable<TMessage, TToken> : IObservable<TMessage>
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        /// <summary>
        /// The <see cref="IMessenger"/> instance to use to register the recipient.
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// The token used to determine the receiving channel to use.
        /// </summary>
        private readonly TToken token;

        /// <summary>
        /// Creates a new <see cref="Observable{TMessage, TToken}"/> instance with the given parameters.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
        /// <param name="token">A token used to determine the receiving channel to use.</param>
        public Observable(IMessenger messenger, TToken token)
        {
            this.messenger = messenger;
            this.token = token;
        }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TMessage> observer)
        {
            return new Recipient(this.messenger, observer, this.token);
        }

        /// <summary>
        /// An <see cref="IRecipient{TMessage}"/> implementation for <see cref="Observable{TMessage, TToken}"/>.
        /// </summary>
        private sealed class Recipient : IRecipient<TMessage>, IDisposable
        {
            /// <summary>
            /// The <see cref="IMessenger"/> instance to use to register the recipient.
            /// </summary>
            private readonly IMessenger messenger;

            /// <summary>
            /// The target <see cref="IObserver{T}"/> instance currently in use.
            /// </summary>
            private readonly IObserver<TMessage> observer;

            /// <summary>
            /// The token used to determine the receiving channel to use.
            /// </summary>
            private readonly TToken token;

            /// <summary>
            /// Creates a new <see cref="Recipient"/> instance with the specified parameters.
            /// </summary>
            /// <param name="messenger">The <see cref="IMessenger"/> instance to use to register the recipient.</param>
            /// <param name="observer">The <see cref="IObserver{T}"/> instance to use to create the recipient for.</param>
            /// <param name="token">A token used to determine the receiving channel to use.</param>
            public Recipient(IMessenger messenger, IObserver<TMessage> observer, TToken token)
            {
                this.messenger = messenger;
                this.observer = observer;
                this.token = token;

                messenger.Register(this, token);
            }

            /// <inheritdoc/>
            public void Receive(TMessage message)
            {
                this.observer.OnNext(message);
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                this.messenger.Unregister<TMessage, TToken>(this, this.token);
            }
        }
    }
}

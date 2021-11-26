// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using CommunityToolkit.Mvvm.Messaging.Internals;
#if NETSTANDARD2_0 || NET6_0_OR_GREATER
using RecipientsTable = System.Runtime.CompilerServices.ConditionalWeakTable2<object, object>;
#else
using RecipientsTable = System.Runtime.CompilerServices.ConditionalWeakTable<object, object>;
#endif

namespace CommunityToolkit.Mvvm.Messaging;

/// <summary>
/// A class providing a reference implementation for the <see cref="IMessenger"/> interface.
/// </summary>
/// <remarks>
/// <para>
/// This <see cref="IMessenger"/> implementation uses weak references to track the registered
/// recipients, so it is not necessary to manually unregister them when they're no longer needed.
/// </para>
/// <para>
/// The <see cref="WeakReferenceMessenger"/> type will automatically perform internal trimming when
/// full GC collections are invoked, so calling <see cref="Cleanup"/> manually is not necessary to
/// ensure that on average the internal data structures are as trimmed and compact as possible.
/// </para>
/// </remarks>
public sealed class WeakReferenceMessenger : IMessenger
{
    // This messenger uses the following logic to link stored instances together:
    // --------------------------------------------------------------------------------------------------------
    //                          Dictionary2<TToken, MessageHandlerDispatcher> mapping
    //                                        /                   /             /
    //                   ___(Type2.TToken)___/                   /             /         ___(if Type2.TToken is Unit)
    //                  /_________(Type2.TMessage)______________/             /         /
    //                 /                                    _________________/___MessageHandler<TRecipient, TMessage>
    //                /                                    /
    // Dictionary2<Type2, ConditionalWeakTable<object, object>> recipientsMap;
    // --------------------------------------------------------------------------------------------------------
    // Just like in the strong reference variant, each pair of message and token types is used as a key in the
    // recipients map. In this case, the values in the dictionary are ConditionalWeakTable<,> instances, that
    // link each registered recipient to a map of currently registered handlers, through a weak reference.
    // The value in each conditional table can either be Dictionary<TToken, MessageHandler<TRecipient, TMessage>>
    // or object. The first case is used when any token type other than the default Unit type is used, as in this
    // case there could be multiple handlers for each recipient that need to be tracked separately. This is done to
    // use the same unsafe cast as before to allow the generic handler delegates to be invoked without knowing
    // what type each recipient was registered with, and without the need to use reflection. If the token type is
    // the default one instead, the handler is directly stored in the table next to each recipient, with no dictionary.

    /// <summary>
    /// The map of currently registered recipients for all message types.
    /// </summary>
    private readonly Dictionary2<Type2, RecipientsTable> recipientsMap = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakReferenceMessenger"/> class.
    /// </summary>
    public WeakReferenceMessenger()
    {
        // Proxy function for the GC callback. This needs to be static and to take the target instance as
        // an input parameter in order to avoid rooting it from the Gen2GcCallback object invoking it.
        static void Gen2GcCallbackProxy(object target)
        {
            ((WeakReferenceMessenger)target).CleanupWithNonBlockingLock();
        }

        // Register an automatic GC callback to trigger a non-blocking cleanup. This will ensure that the
        // current messenger instance is trimmed and without leftover recipient maps that are no longer used.
        // This is necessary (as in, some form of cleanup, either explicit or automatic like in this case)
        // because the ConditionalWeakTable<TKey, TValue> instances will just remove key-value pairs on their
        // own as soon as a key (ie. a recipient) is collected, causing their own keys (ie. the Type2 instances
        // mapping to each conditional table for a pair of message and token types) to potentially remain in the
        // root mapping structure but without any remaining recipients actually registered there, which just
        // adds unnecessary overhead when trying to enumerate recipients during broadcasting operations later on.
        Gen2GcCallback.Register(Gen2GcCallbackProxy, this);
    }

    /// <summary>
    /// Gets the default <see cref="WeakReferenceMessenger"/> instance.
    /// </summary>
    public static WeakReferenceMessenger Default { get; } = new();

    /// <inheritdoc/>
    public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        lock (this.recipientsMap)
        {
            Type2 type2 = new(typeof(TMessage), typeof(TToken));

            // Get the conditional table associated with the target recipient, for the current pair
            // of token and message types. If it exists, check if there is a matching token.
            if (!this.recipientsMap.TryGetValue(type2, out RecipientsTable? table))
            {
                return false;
            }

            // Special case for unit tokens
            if (typeof(TToken) == typeof(Unit))
            {
                return table.TryGetValue(recipient, out _);
            }

            // Custom token type, so each recipient has an associated map
            return
                table.TryGetValue(recipient, out object? mapping) &&
                Unsafe.As<Dictionary2<TToken, object>>(mapping).ContainsKey(token);
        }
    }

    /// <inheritdoc/>
    public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
        where TRecipient : class
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        Register<TMessage, TToken>(recipient, token, new MessageHandlerDispatcher.For<TRecipient, TMessage>(handler));
    }

    /// <summary>
    /// Registers a recipient for a given type of message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to receive.</typeparam>
    /// <typeparam name="TToken">The type of token to use to pick the messages to receive.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token used to determine the receiving channel to use.</param>
    /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
    /// <remarks>
    /// This method is a variation of <see cref="Register{TRecipient, TMessage, TToken}(TRecipient, TToken, MessageHandler{TRecipient, TMessage})"/>
    /// that is specialized for recipients implementing <see cref="IRecipient{TMessage}"/>. See more comments at the top of this type, as well as
    /// within <see cref="Send{TMessage, TToken}(TMessage, TToken)"/> and in the <see cref="MessageHandlerDispatcher"/> types.
    /// </remarks>
    internal void Register<TMessage, TToken>(IRecipient<TMessage> recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        Register<TMessage, TToken>(recipient, token, MessageHandlerDispatcher.IRecipient.Instance);
    }

    /// <summary>
    /// Registers a recipient for a given type of message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to receive.</typeparam>
    /// <typeparam name="TToken">The type of token to use to pick the messages to receive.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token used to determine the receiving channel to use.</param>
    /// <param name="dispatcher">The input <see cref="MessageHandlerDispatcher"/> instance to register.</param>
    /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
    private void Register<TMessage, TToken>(object recipient, TToken token, MessageHandlerDispatcher dispatcher)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        lock (this.recipientsMap)
        {
            Type2 type2 = new(typeof(TMessage), typeof(TToken));

            // Get the conditional table for the pair of type arguments, or create it if it doesn't exist
            ref RecipientsTable? mapping = ref this.recipientsMap.GetOrAddValueRef(type2);

            mapping ??= new RecipientsTable();

            // Fast path for unit tokens
            if (typeof(TToken) == typeof(Unit))
            {
                if (!mapping.TryAdd(recipient, dispatcher))
                {
                    ThrowInvalidOperationExceptionForDuplicateRegistration();
                }
            }
            else
            {
                // Get or create the handlers dictionary for the target recipient
                Dictionary2<TToken, object>? map = Unsafe.As<Dictionary2<TToken, object>>(mapping.GetValue(recipient, static _ => new Dictionary2<TToken, object>()));

                // Add the new registration entry
                ref object? registeredHandler = ref map.GetOrAddValueRef(token);

                if (registeredHandler is not null)
                {
                    ThrowInvalidOperationExceptionForDuplicateRegistration();
                }

                // Store the input handler
                registeredHandler = dispatcher;
            }
        }
    }

    /// <inheritdoc/>
    public void UnregisterAll(object recipient)
    {
        lock (this.recipientsMap)
        {
            Dictionary2<Type2, RecipientsTable>.Enumerator enumerator = this.recipientsMap.GetEnumerator();

            // Traverse all the existing conditional tables and remove all the ones
            // with the target recipient as key. We don't perform a cleanup here,
            // as that is responsibility of a separate method defined below.
            while (enumerator.MoveNext())
            {
                _ = enumerator.Value.Remove(recipient);
            }
        }
    }

    /// <inheritdoc/>
    public void UnregisterAll<TToken>(object recipient, TToken token)
        where TToken : IEquatable<TToken>
    {
        lock (this.recipientsMap)
        {
            Dictionary2<Type2, RecipientsTable>.Enumerator enumerator = this.recipientsMap.GetEnumerator();

            // Same as above, with the difference being that this time we only go through
            // the conditional tables having a matching token type as key, and that we
            // only try to remove handlers with a matching token, if any.
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.TToken == typeof(TToken))
                {
                    if (typeof(TToken) == typeof(Unit))
                    {
                        _ = enumerator.Value.Remove(recipient);
                    }
                    else if (enumerator.Value.TryGetValue(recipient, out object? mapping))
                    {
                        _ = Unsafe.As<Dictionary2<TToken, object>>(mapping).TryRemove(token);
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public void Unregister<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        lock (this.recipientsMap)
        {
            Type2 type2 = new(typeof(TMessage), typeof(TToken));

            // Get the target mapping table for the combination of message and token types,
            // and remove the handler with a matching token (the entire map), if present.
            if (this.recipientsMap.TryGetValue(type2, out RecipientsTable? value))
            {
                if (typeof(TToken) == typeof(Unit))
                {
                    _ = value.Remove(recipient);
                }
                else if (value.TryGetValue(recipient, out object? mapping))
                {
                    _ = Unsafe.As<Dictionary2<TToken, object>>(mapping).TryRemove(token);
                }
            }
        }
    }

    /// <inheritdoc/>
    public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        ArrayPoolBufferWriter<object> bufferWriter;
        int i = 0;

        lock (this.recipientsMap)
        {
            Type2 type2 = new(typeof(TMessage), typeof(TToken));

            // Try to get the target table
            if (!this.recipientsMap.TryGetValue(type2, out RecipientsTable? table))
            {
                return message;
            }

            bufferWriter = ArrayPoolBufferWriter<object>.Create();

            // We need a local, temporary copy of all the pending recipients and handlers to
            // invoke, to avoid issues with handlers unregistering from messages while we're
            // holding the lock. To do this, we can just traverse the conditional table in use
            // to enumerate all the existing recipients for the token and message types pair
            // corresponding to the generic arguments for this invocation, and then track the
            // handlers with a matching token, and their corresponding recipients.
            foreach (KeyValuePair<object, object> pair in table)
            {
                if (typeof(TToken) == typeof(Unit))
                {
                    bufferWriter.Add(pair.Value);
                    bufferWriter.Add(pair.Key);
                    i++;
                }
                else
                {
                    Dictionary2<TToken, object>? map = Unsafe.As<Dictionary2<TToken, object>>(pair.Value);

                    if (map.TryGetValue(token, out object? handler))
                    {
                        bufferWriter.Add(handler);
                        bufferWriter.Add(pair.Key);
                        i++;
                    }
                }
            }
        }

        try
        {
            ReadOnlySpan<object> pairs = bufferWriter.Span;

            for (int j = 0; j < i; j++)
            {
                object recipient = pairs[(2 * j) + 1];
                object handler = pairs[2 * j];

                // This doesn't use reflection: a GetType() call being immediately compared to
                // a specific type just results in a direct comparison of the method table pointer
                // with a constant address corresponding to the method table address for that type.
                // That is, for instance on x64 and assuming handler is in rcx, this will produce:
                // =============================
                // L0000: mov rax, 0x7ffcbc87cc98
                // L000a: cmp [rcx], rax
                // =============================
                // Which is extremely fast. The reason for this conditional check in the first place
                // is that we're doing manual guarded devirtualization: if the handler is the marker
                // type and not an actual handler then we know that the recipient implements
                // IRecipient<TMessage>, so we can just cast to it and invoke it directly. This avoids
                // having to store the proxy callback when registering, and also skips an indirection
                // (invoking the delegate that then invokes the actual method). Additional note: this
                // pattern ensures that both casts below do not actually alias incompatible reference
                // types (as in, they would both succeed if they were safe casts), which lets the code
                // not rely on undefined behavior to run correctly (ie. we're not aliasing delegates).
                if (handler.GetType() == typeof(MessageHandlerDispatcher.IRecipient))
                {
                    Unsafe.As<IRecipient<TMessage>>(recipient).Receive(message);
                }
                else
                {
                    Unsafe.As<MessageHandlerDispatcher>(handler).Invoke(recipient, message);
                }
            }
        }
        finally
        {
            bufferWriter.Dispose();
        }

        return message;
    }

    /// <inheritdoc/>
    public void Cleanup()
    {
        lock (this.recipientsMap)
        {
            CleanupWithoutLock();
        }
    }

    /// <inheritdoc/>
    public void Reset()
    {
        lock (this.recipientsMap)
        {
            this.recipientsMap.Clear();
        }
    }

    /// <summary>
    /// Executes a cleanup without locking the current instance. This method has to be
    /// invoked when a lock on <see cref="recipientsMap"/> has already been acquired.
    /// </summary>
    private void CleanupWithNonBlockingLock()
    {
        object lockObject = this.recipientsMap;
        bool lockTaken = false;

        try
        {
            Monitor.TryEnter(lockObject, ref lockTaken);

            if (lockTaken)
            {
                CleanupWithoutLock();
            }
        }
        finally
        {
            if (lockTaken)
            {
                Monitor.Exit(lockObject);
            }
        }
    }

    /// <summary>
    /// Executes a cleanup without locking the current instance. This method has to be
    /// invoked when a lock on <see cref="recipientsMap"/> has already been acquired.
    /// </summary>
    private void CleanupWithoutLock()
    {
        using ArrayPoolBufferWriter<Type2> type2s = ArrayPoolBufferWriter<Type2>.Create();
        using ArrayPoolBufferWriter<object> emptyRecipients = ArrayPoolBufferWriter<object>.Create();

        Dictionary2<Type2, RecipientsTable>.Enumerator enumerator = this.recipientsMap.GetEnumerator();

        // First, we go through all the currently registered pairs of token and message types.
        // These represents all the combinations of generic arguments with at least one registered
        // handler, with the exception of those with recipients that have already been collected.
        while (enumerator.MoveNext())
        {
            emptyRecipients.Reset();

            bool hasAtLeastOneHandler = false;

            if (enumerator.Key.TToken == typeof(Unit))
            {
                // When the token type is unit, there can be no registered recipients with no handlers,
                // as when the single handler is unsubscribed the recipient is also removed immediately.
                // Therefore, we need to check that there exists at least one recipient for the message.
                foreach (KeyValuePair<object, object> pair in enumerator.Value)
                {
                    hasAtLeastOneHandler = true;

                    break;
                }
            }
            else
            {
                // Go through the currently alive recipients to look for those with no handlers left. We track
                // the ones we find to remove them outside of the loop (can't modify during enumeration).
                foreach (KeyValuePair<object, object> pair in enumerator.Value)
                {
                    if (Unsafe.As<IDictionary2>(pair.Value).Count == 0)
                    {
                        emptyRecipients.Add(pair.Key);
                    }
                    else
                    {
                        hasAtLeastOneHandler = true;
                    }
                }

                // Remove the handler maps for recipients that are still alive but with no handlers
                foreach (object recipient in emptyRecipients.Span)
                {
                    _ = enumerator.Value.Remove(recipient);
                }
            }

            // Track the type combinations with no recipients or handlers left
            if (!hasAtLeastOneHandler)
            {
                type2s.Add(enumerator.Key);
            }
        }

        // Remove all the mappings with no handlers left
        foreach (Type2 key in type2s.Span)
        {
            _ = this.recipientsMap.TryRemove(key);
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> when trying to add a duplicate handler.
    /// </summary>
    private static void ThrowInvalidOperationExceptionForDuplicateRegistration()
    {
        throw new InvalidOperationException("The target recipient has already subscribed to the target message");
    }
}

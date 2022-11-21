// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using CommunityToolkit.Mvvm.Messaging.Internals;

namespace CommunityToolkit.Mvvm.Messaging;

/// <summary>
/// A class providing a reference implementation for the <see cref="IMessenger"/> interface.
/// </summary>
/// <remarks>
/// This <see cref="IMessenger"/> implementation uses strong references to track the registered
/// recipients, so it is necessary to manually unregister them when they're no longer needed.
/// </remarks>
public sealed class StrongReferenceMessenger : IMessenger
{
    // This messenger uses the following logic to link stored instances together:
    // --------------------------------------------------------------------------------------------------------
    //   Dictionary2<Recipient, HashSet<IMapping>> recipientsMap;
    //                   |                 \________________[*]IDictionary2<Recipient, IDictionary2<TToken>>
    //                   |                  \_______________[*]IDictionary2<Recipient, object?>       /
    //                   |                                           \_________/_________/___        / 
    //                   |\                       _(recipients registrations)_/         /    \      /
    //                   | \__________________   /    _____(channel registrations)_____/______\____/
    //                   |                    \ /    /      __________________________/        \
    //                   |                     /    /      /                                    \
    //                   |      Dictionary2<Recipient, object?> mapping = Mapping________________\
    //                   | __________________/    /                |         /                    \
    //                   |/                      /                 |        /                      \
    //    Dictionary2<Recipient, Dictionary2<TToken, object?>> mapping = Mapping<TToken>____________\
    //                                         /                  /       /  /
    //                   ___(Type2.TToken)____/                  /       /  /
    //                  /________________(Type2.TMessage)_______/_______/__/
    //                 /       ________________________________/
    //                /       /
    // Dictionary2<Type2, IMapping> typesMap;
    // --------------------------------------------------------------------------------------------------------
    // Each combination of <TMessage, TToken> results in a concrete Mapping type (if TToken is Unit) or Mapping<Token> type,
    // which holds the references from registered recipients to handlers. Mapping is used when the default channel is being
    // requested, as in that case there will only ever be up to a handler per recipient, per message type. In that case,
    // each recipient will only track the message dispatcher (stored as an object?, see notes below), instead of a dictionary
    // mapping each TToken value to the corresponding dispatcher for that recipient. When a custom channel is used, the
    // dispatchers are stored in a <TToken, object?> dictionary, so that each recipient can have up to one registered handler
    // for a given token, for each message type. Note that the registered dispatchers are only stored as object references, as
    // they can either be null or a MessageHandlerDispatcher.For<TRecipient, TMessage> instance.
    //
    // The first case happens if the handler was registered through an IRecipient<TMessage> instance, while the second one is
    // used to wrap input MessageHandler<TRecipient, TMessage> instances. The MessageHandlerDispatcher.For<TRecipient, TMessage>
    // instances will just be cast to MessageHandlerDispatcher when invoking it. This allows users to retain type information on
    // each registered recipient, instead of having to manually cast each recipient to the right type within the handler
    // (additionally, using double dispatch here avoids the need to alias delegate types). The type conversion is guaranteed to be
    // respected due to how the messenger type itself works - as registered handlers are always invoked on their respective recipients.
    //
    // Each mapping is stored in the types map, which associates each pair of concrete types to its mapping instance. Mapping instances
    // are exposed as IMapping items, as each will be a closed type over a different combination of TMessage and TToken generic type
    // parameters (or just of TMessage, for the default channel). Each existing recipient is also stored in the main recipients map,
    // along with a set of all the existing (dictionaries of) handlers for that recipient (for all message types and token types, if any).
    //
    // A recipient is stored in the main map as long as it has at least one registered handler in any of the existing mappings for every
    // message/token type combination. The shared map is used to access the set of all registered handlers for a given recipient, without
    // having to know in advance the type of message or token being used for the registration, and without having to use reflection. This
    // is the same approach used in the types map, as we expose saved items as IMapping values too.
    //
    // Note that each mapping stored in the associated set for each recipient also indirectly implements either IDictionary2<Recipient, Token>
    // or IDictionary2<Recipient>, with any token type currently in use by that recipient (or none, if using the default channel). This allows
    // to retrieve the type-closed mappings of registered handlers with a given token type, for any message type, for every receiver, again
    // without having to use reflection. This shared map is used to unregister messages from a given recipients either unconditionally, by
    // message type, by token, or for a specific pair of message type and token value.

    /// <summary>
    /// The collection of currently registered recipients, with a link to their linked message receivers.
    /// </summary>
    /// <remarks>
    /// This collection is used to allow reflection-free access to all the existing
    /// registered recipients from <see cref="UnregisterAll"/> and other methods in this type,
    /// so that all the existing handlers can be removed without having to dynamically create
    /// the generic types for the containers of the various dictionaries mapping the handlers.
    /// </remarks>
    private readonly Dictionary2<Recipient, HashSet<IMapping>> recipientsMap = new();

    /// <summary>
    /// The <see cref="Mapping"/> and <see cref="Mapping{TToken}"/> instance for types combination.
    /// </summary>
    /// <remarks>
    /// The values are just of type <see cref="IDictionary2{T}"/> as we don't know the type parameters in advance.
    /// Each method relies on <see cref="GetOrAddMapping{TMessage,TToken}"/> to get the type-safe instance of the
    /// <see cref="Mapping"/> or <see cref="Mapping{TToken}"/> class for each pair of generic arguments in use.
    /// </remarks>
    private readonly Dictionary2<Type2, IMapping> typesMap = new();

    /// <summary>
    /// Gets the default <see cref="StrongReferenceMessenger"/> instance.
    /// </summary>
    public static StrongReferenceMessenger Default { get; } = new();

    /// <inheritdoc/>
    public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.For<TToken>.ThrowIfNull(token);

        lock (this.recipientsMap)
        {
            if (typeof(TToken) == typeof(Unit))
            {
                if (!TryGetMapping<TMessage>(out Mapping? mapping))
                {
                    return false;
                }

                Recipient key = new(recipient);

                return mapping.ContainsKey(key);
            }
            else
            {
                if (!TryGetMapping<TMessage, TToken>(out Mapping<TToken>? mapping))
                {
                    return false;
                }

                Recipient key = new(recipient);

                return
                    mapping.TryGetValue(key, out Dictionary2<TToken, object?>? handlers) &&
                    handlers.ContainsKey(token);
            }
        }
    }

    /// <inheritdoc/>
    public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
        where TRecipient : class
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.For<TToken>.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(handler);

        Register<TMessage, TToken>(recipient, token, new MessageHandlerDispatcher.For<TRecipient, TMessage>(handler));
    }

    /// <inheritdoc cref="WeakReferenceMessenger.Register{TMessage, TToken}(IRecipient{TMessage}, TToken)"/>
    internal void Register<TMessage, TToken>(IRecipient<TMessage> recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        Register<TMessage, TToken>(recipient, token, null);
    }

    /// <summary>
    /// Registers a recipient for a given type of message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to receive.</typeparam>
    /// <typeparam name="TToken">The type of token to use to pick the messages to receive.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token used to determine the receiving channel to use.</param>
    /// <param name="dispatcher">The input <see cref="MessageHandlerDispatcher"/> instance to register, or null.</param>
    /// <exception cref="InvalidOperationException">Thrown when trying to register the same message twice.</exception>
    private void Register<TMessage, TToken>(object recipient, TToken token, MessageHandlerDispatcher? dispatcher)
       where TMessage : class
       where TToken : IEquatable<TToken>
    {
        lock (this.recipientsMap)
        {
            Recipient key = new(recipient);
            IMapping mapping;

            // Fast path for unit tokens
            if (typeof(TToken) == typeof(Unit))
            {
                // Get the <TMessage> registration list for this recipient
                Mapping underlyingMapping = GetOrAddMapping<TMessage>();
                ref object? registeredHandler = ref underlyingMapping.GetOrAddValueRef(key);

                if (registeredHandler is not null)
                {
                    ThrowInvalidOperationExceptionForDuplicateRegistration();
                }

                // Store the input handler
                registeredHandler = dispatcher;

                mapping = underlyingMapping;
            }
            else
            {
                // Get the <TMessage, TToken> registration list for this recipient
                Mapping<TToken> underlyingMapping = GetOrAddMapping<TMessage, TToken>();
                ref Dictionary2<TToken, object?>? map = ref underlyingMapping.GetOrAddValueRef(key);

                map ??= new Dictionary2<TToken, object?>();

                // Add the new registration entry
                ref object? registeredHandler = ref map.GetOrAddValueRef(token);

                if (registeredHandler is not null)
                {
                    ThrowInvalidOperationExceptionForDuplicateRegistration();
                }

                registeredHandler = dispatcher;
                mapping = underlyingMapping;
            }

            // Make sure this registration map is tracked for the current recipient
            ref HashSet<IMapping>? set = ref this.recipientsMap.GetOrAddValueRef(key);

            set ??= new HashSet<IMapping>();

            _ = set.Add(mapping);
        }
    }

    /// <inheritdoc/>
    public void UnregisterAll(object recipient)
    {
        ArgumentNullException.ThrowIfNull(recipient);

        lock (this.recipientsMap)
        {
            // If the recipient has no registered messages at all, ignore
            Recipient key = new(recipient);

            if (!this.recipientsMap.TryGetValue(key, out HashSet<IMapping>? set))
            {
                return;
            }

            // Removes all the lists of registered handlers for the recipient
            foreach (IMapping mapping in set)
            {
                if (mapping.TryRemove(key) &&
                    mapping.Count == 0)
                {
                    // Maps here are really of type Mapping<,> and with unknown type arguments.
                    // If after removing the current recipient a given map becomes empty, it means
                    // that there are no registered recipients at all for a given pair of message
                    // and token types. In that case, we also remove the map from the types map.
                    // The reason for keeping a key in each mapping is that removing items from a
                    // dictionary (a hashed collection) only costs O(1) in the best case, while
                    // if we had tried to iterate the whole dictionary every time we would have
                    // paid an O(n) minimum cost for each single remove operation.
                    _ = this.typesMap.TryRemove(mapping.TypeArguments);
                }
            }

            // Remove the associated set in the recipients map
            _ = this.recipientsMap.TryRemove(key);
        }
    }

    /// <inheritdoc/>
    public void UnregisterAll<TToken>(object recipient, TToken token)
        where TToken : IEquatable<TToken>
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.For<TToken>.ThrowIfNull(token);

        // This method is never called with the unit type, so this path is not implemented. This
        // exception should not ever be thrown, it's here just to double check for regressions in
        // case a bug was introduced that caused this path to somehow be invoked with the Unit type.
        // This type is internal, so consumers of the library would never be able to pass it here,
        // and there are (and shouldn't be) any APIs publicly exposed from the library that would
        // cause this path to be taken either. When using the default channel, only UnregisterAll(object)
        // is supported, which would just unregister all recipients regardless of the selected channel.
        if (typeof(TToken) == typeof(Unit))
        {
            throw new NotImplementedException();
        }

        bool lockTaken = false;
        object[]? maps = null;
        int i = 0;

        // We use an explicit try/finally block here instead of the lock syntax so that we can use a single
        // one both to release the lock and to clear the rented buffer and return it to the pool. The reason
        // why we're declaring the buffer here and clearing and returning it in this outer finally block is
        // that doing so doesn't require the lock to be kept, and releasing it before performing this last
        // step reduces the total time spent while the lock is acquired, which in turn reduces the lock
        // contention in multi-threaded scenarios where this method is invoked concurrently.
        try
        {
            Monitor.Enter(this.recipientsMap, ref lockTaken);

            // Get the shared set of mappings for the recipient, if present
            Recipient key = new(recipient);

            if (!this.recipientsMap.TryGetValue(key, out HashSet<IMapping>? set))
            {
                return;
            }

            // Copy the candidate mappings for the target recipient to a local array, as we can't modify the
            // contents of the set while iterating it. The rented buffer is oversized and will also include
            // mappings for handlers of messages that are registered through a different token. Note that
            // we're using just an object array to minimize the number of total rented buffers, that would
            // just remain in the shared pool unused, other than when they are rented here. Instead, we're
            // using a type that would possibly also be used by the users of the library, which increases
            // the opportunities to reuse existing buffers for both. When we need to reference an item
            // stored in the buffer with the type we know it will have, we use Unsafe.As<T> to avoid the
            // expensive type check in the cast, since we already know the assignment will be valid.
            maps = ArrayPool<object>.Shared.Rent(set.Count);

            foreach (IMapping item in set)
            {
                // Select all mappings using the same token type
                if (item is IDictionary2<Recipient, IDictionary2<TToken>> mapping)
                {
                    maps[i++] = mapping;
                }
            }

            // Iterate through all the local maps. These are all the currently
            // existing maps of handlers for messages of any given type, with a token
            // of the current type, for the target recipient. We heavily rely on
            // interfaces here to be able to iterate through all the available mappings
            // without having to know the concrete type in advance, and without having
            // to deal with reflection: we can just check if the type of the closed interface
            // matches with the token type currently in use, and operate on those instances.
            foreach (object obj in maps.AsSpan(0, i))
            {
                IDictionary2<Recipient, IDictionary2<TToken>>? handlersMap = Unsafe.As<IDictionary2<Recipient, IDictionary2<TToken>>>(obj);

                // We don't need whether or not the map contains the recipient, as the
                // sequence of maps has already been copied from the set containing all
                // the mappings for the target recipients: it is guaranteed to be here.
                IDictionary2<TToken> holder = handlersMap[key];

                // Try to remove the registered handler for the input token,
                // for the current message type (unknown from here).
                if (holder.TryRemove(token) &&
                    holder.Count == 0)
                {
                    // If the map is empty, remove the recipient entirely from its container
                    _ = handlersMap.TryRemove(key);

                    IMapping mapping = Unsafe.As<IMapping>(handlersMap);

                    // This recipient has no registrations left for this combination of token
                    // and message type, so this mapping can be removed from its associated set.
                    _ = set.Remove(mapping);

                    // If the resulting set is empty, then this means that there are no more handlers
                    // left for this recipient for any message or token type, so the recipient can also
                    // be removed from the map of all existing recipients with at least one handler.
                    if (set.Count == 0)
                    {
                        _ = this.recipientsMap.TryRemove(key);
                    }

                    // If no handlers are left at all for any recipient, across all message types and token
                    // types, remove the set of mappings entirely for the current recipient, and remove the
                    // strong reference to it as well. This is the same situation that would've been achieved
                    // by just calling UnregisterAll(recipient).
                    if (handlersMap.Count == 0)
                    {
                        _ = this.typesMap.TryRemove(mapping.TypeArguments);
                    }
                }
            }
        }
        finally
        {
            // Release the lock, if we did acquire it
            if (lockTaken)
            {
                Monitor.Exit(this.recipientsMap);
            }

            // If we got to renting the array of maps, return it to the shared pool.
            // Remove references to avoid leaks coming from the shared memory pool.
            // We manually create a span and clear it as a small optimization, as
            // arrays rented from the pool can be larger than the requested size.
            if (maps is not null)
            {
                maps.AsSpan(0, i).Clear();

                ArrayPool<object>.Shared.Return(maps);
            }
        }
    }

    /// <inheritdoc/>
    public void Unregister<TMessage, TToken>(object recipient, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.For<TToken>.ThrowIfNull(token);

        lock (this.recipientsMap)
        {
            if (typeof(TToken) == typeof(Unit))
            {
                // Get the registration list, if available
                if (!TryGetMapping<TMessage>(out Mapping? mapping))
                {
                    return;
                }

                Recipient key = new(recipient);

                // Remove the handler (there can only be one for the unit type)
                if (!mapping.TryRemove(key))
                {
                    return;
                }

                // Remove the map entirely from this container, and remove the link to the map itself to
                // the current mapping between existing registered recipients (or entire recipients too).
                // This is the same as below, except for the unit type there can only be one handler, so
                // removing it already implies the target recipient has no remaining handlers left.
                _ = mapping.TryRemove(key);

                // If there are no handlers left at all for this type combination, drop it
                if (mapping.Count == 0)
                {
                    _ = this.typesMap.TryRemove(mapping.TypeArguments);
                }

                HashSet<IMapping> set = this.recipientsMap[key];

                // The current mapping no longer has any handlers left for this recipient.
                // Remove it and then also remove the recipient if this was the last handler.
                // Again, this is the same as below, except with the assumption of the unit type.
                _ = set.Remove(mapping);

                if (set.Count == 0)
                {
                    _ = this.recipientsMap.TryRemove(key);
                }
            }
            else
            {
                // Get the registration list, if available
                if (!TryGetMapping<TMessage, TToken>(out Mapping<TToken>? mapping))
                {
                    return;
                }

                Recipient key = new(recipient);

                if (!mapping.TryGetValue(key, out Dictionary2<TToken, object?>? dictionary))
                {
                    return;
                }

                // Remove the target handler
                if (dictionary.TryRemove(token) &&
                    dictionary.Count == 0)
                {
                    // If the map is empty, it means that the current recipient has no remaining
                    // registered handlers for the current <TMessage, TToken> combination, regardless,
                    // of the specific token value (ie. the channel used to receive messages of that type).
                    // We can remove the map entirely from this container, and remove the link to the map itself
                    // to the current mapping between existing registered recipients (or entire recipients too).
                    _ = mapping.TryRemove(key);

                    // If there are no handlers left at all for this type combination, drop it
                    if (mapping.Count == 0)
                    {
                        _ = this.typesMap.TryRemove(mapping.TypeArguments);
                    }

                    HashSet<IMapping> set = this.recipientsMap[key];

                    // The current mapping no longer has any handlers left for this recipient
                    _ = set.Remove(mapping);

                    // If the current recipients has no handlers left at all, remove it
                    if (set.Count == 0)
                    {
                        _ = this.recipientsMap.TryRemove(key);
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.For<TToken>.ThrowIfNull(token);

        object?[] rentedArray;
        Span<object?> pairs;
        int i = 0;

        lock (this.recipientsMap)
        {
            if (typeof(TToken) == typeof(Unit))
            {
                // Check whether there are any registered recipients
                if (!TryGetMapping<TMessage>(out Mapping? mapping))
                {
                    goto End;
                }

                // Check the number of remaining handlers, see below
                int totalHandlersCount = mapping.Count;

                if (totalHandlersCount == 0)
                {
                    goto End;
                }

                pairs = rentedArray = ArrayPool<object?>.Shared.Rent(2 * totalHandlersCount);

                // Same logic as below, except here we're only traversing one handler per recipient
                Dictionary2<Recipient, object?>.Enumerator mappingEnumerator = mapping.GetEnumerator();

                while (mappingEnumerator.MoveNext())
                {
                    pairs[2 * i] = mappingEnumerator.GetValue();
                    pairs[(2 * i) + 1] = mappingEnumerator.GetKey().Target;
                    i++;
                }
            }
            else
            {
                // Check whether there are any registered recipients
                if (!TryGetMapping<TMessage, TToken>(out Mapping<TToken>? mapping))
                {
                    goto End;
                }

                // We need to make a local copy of the currently registered handlers, since users might
                // try to unregister (or register) new handlers from inside one of the currently existing
                // handlers. We can use memory pooling to reuse arrays, to minimize the average memory
                // usage. In practice, we usually just need to pay the small overhead of copying the items.
                // The current mapping contains all the currently registered recipients and handlers for
                // the <TMessage, TToken> combination in use. In the worst case scenario, all recipients
                // will have a registered handler with a token matching the input one, meaning that we could
                // have at worst a number of pending handlers to invoke equal to the total number of recipient
                // in the mapping. This relies on the fact that tokens are unique, and that there is only
                // one handler associated with a given token. We can use this upper bound as the requested
                // size for each array rented from the pool, which guarantees that we'll have enough space.
                int totalHandlersCount = mapping.Count;

                if (totalHandlersCount == 0)
                {
                    goto End;
                }

                // Rent the array and also assign it to a span, which will be used to access values.
                // We're doing this to avoid the array covariance checks slowdown in the loops below.
                pairs = rentedArray = ArrayPool<object?>.Shared.Rent(2 * totalHandlersCount);

                // Copy the handlers to the local collection.
                // The array is oversized at this point, since it also includes
                // handlers for different tokens. We can reuse the same variable
                // to count the number of matching handlers to invoke later on.
                // This will be the array slice with valid handler in the rented buffer.
                Dictionary2<Recipient, Dictionary2<TToken, object?>>.Enumerator mappingEnumerator = mapping.GetEnumerator();

                // Explicit enumerator usage here as we're using a custom one
                // that doesn't expose the single standard Current property.
                while (mappingEnumerator.MoveNext())
                {
                    // Pick the target handler, if the token is a match for the recipient
                    if (mappingEnumerator.GetValue().TryGetValue(token, out object? handler))
                    {
                        // This span access should always guaranteed to be valid due to the size of the
                        // array being set according to the current total number of registered handlers,
                        // which will always be greater or equal than the ones matching the previous test.
                        // We're still using a checked span accesses here though to make sure an out of
                        // bounds write can never happen even if an error was present in the logic above.
                        pairs[2 * i] = handler;
                        pairs[(2 * i) + 1] = mappingEnumerator.GetKey().Target;
                        i++;
                    }
                }
            }
        }

        try
        {
            // The core broadcasting logic is the same as the weak reference messenger one
            WeakReferenceMessenger.SendAll(pairs, i, message);
        }
        finally
        {
            // As before, we also need to clear it first to avoid having potentially long
            // lasting memory leaks due to leftover references being stored in the pool.
            Array.Clear(rentedArray, 0, 2 * i);

            ArrayPool<object?>.Shared.Return(rentedArray);
        }

        End:
        return message;
    }

    /// <inheritdoc/>
    void IMessenger.Cleanup()
    {
        // The current implementation doesn't require any kind of cleanup operation, as
        // all the internal data structures are already kept in sync whenever a recipient
        // is added or removed. This method is implemented through an explicit interface
        // implementation so that developers using this type directly will not see it in
        // the API surface (as it wouldn't be useful anyway, since it's a no-op here).
    }

    /// <inheritdoc/>
    public void Reset()
    {
        lock (this.recipientsMap)
        {
            this.recipientsMap.Clear();
            this.typesMap.Clear();
        }
    }

    /// <summary>
    /// Tries to get the <see cref="Mapping"/> instance of currently
    /// registered recipients for the input <typeparamref name="TMessage"/> type.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to send.</typeparam>
    /// <param name="mapping">The resulting <see cref="Mapping"/> instance, if found.</param>
    /// <returns>Whether or not the required <see cref="Mapping"/> instance was found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetMapping<TMessage>([NotNullWhen(true)] out Mapping? mapping)
        where TMessage : class
    {
        Type2 key = new(typeof(TMessage), typeof(Unit));

        if (this.typesMap.TryGetValue(key, out IMapping? target))
        {
            // This method and the ones below are the only ones handling values in the types map,
            // and here we are sure that the object reference we have points to an instance of the
            // right type. Using an unsafe cast skips two conditional branches and is faster.
            mapping = Unsafe.As<Mapping>(target);

            return true;
        }

        mapping = null;

        return false;
    }

    /// <summary>
    /// Tries to get the <see cref="Mapping{TToken}"/> instance of currently registered recipients
    /// for the combination of types <typeparamref name="TMessage"/> and <typeparamref name="TToken"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to send.</typeparam>
    /// <typeparam name="TToken">The type of token to identify what channel to use to send the message.</typeparam>
    /// <param name="mapping">The resulting <see cref="Mapping{TToken}"/> instance, if found.</param>
    /// <returns>Whether or not the required <see cref="Mapping{TToken}"/> instance was found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetMapping<TMessage, TToken>([NotNullWhen(true)] out Mapping<TToken>? mapping)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        Type2 key = new(typeof(TMessage), typeof(TToken));

        if (this.typesMap.TryGetValue(key, out IMapping? target))
        {
            mapping = Unsafe.As<Mapping<TToken>>(target);

            return true;
        }

        mapping = null;

        return false;
    }

    /// <summary>
    /// Gets the <see cref="Mapping"/> instance of currently
    /// registered recipients for the input <typeparamref name="TMessage"/> type.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to send.</typeparam>
    /// <returns>A <see cref="Mapping"/> instance with the requested type arguments.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Mapping GetOrAddMapping<TMessage>()
        where TMessage : class
    {
        Type2 key = new(typeof(TMessage), typeof(Unit));
        ref IMapping? target = ref this.typesMap.GetOrAddValueRef(key);

        target ??= Mapping.Create<TMessage>();

        return Unsafe.As<Mapping>(target);
    }

    /// <summary>
    /// Gets the <see cref="Mapping{TToken}"/> instance of currently registered recipients
    /// for the combination of types <typeparamref name="TMessage"/> and <typeparamref name="TToken"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to send.</typeparam>
    /// <typeparam name="TToken">The type of token to identify what channel to use to send the message.</typeparam>
    /// <returns>A <see cref="Mapping{TToken}"/> instance with the requested type arguments.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Mapping<TToken> GetOrAddMapping<TMessage, TToken>()
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        Type2 key = new(typeof(TMessage), typeof(TToken));
        ref IMapping? target = ref this.typesMap.GetOrAddValueRef(key);

        target ??= Mapping<TToken>.Create<TMessage>();

        return Unsafe.As<Mapping<TToken>>(target);
    }

    /// <summary>
    /// A mapping type representing a link to recipients and their view of handlers per communication channel.
    /// </summary>
    /// <remarks>
    /// This type is a specialization of <see cref="Mapping{TToken}"/> for <see cref="Unit"/> tokens.
    /// </remarks>
    private sealed class Mapping : Dictionary2<Recipient, object?>, IMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <param name="messageType">The message type being used.</param>
        private Mapping(Type messageType)
        {
            TypeArguments = new Type2(messageType, typeof(Unit));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Mapping"/> class.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to receive.</typeparam>
        /// <returns>A new <see cref="Mapping"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mapping Create<TMessage>()
            where TMessage : class
        {
            return new(typeof(TMessage));
        }

        /// <inheritdoc/>
        public Type2 TypeArguments { get; }
    }

    /// <summary>
    /// A mapping type representing a link to recipients and their view of handlers per communication channel.
    /// </summary>
    /// <typeparam name="TToken">The type of token to use to pick the messages to receive.</typeparam>
    /// <remarks>
    /// This type is defined for simplicity and as a workaround for the lack of support for using type aliases
    /// over open generic types in C# (using type aliases can only be used for concrete, closed types).
    /// </remarks>
    private sealed class Mapping<TToken> : Dictionary2<Recipient, Dictionary2<TToken, object?>>, IMapping
        where TToken : IEquatable<TToken>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mapping{TToken}"/> class.
        /// </summary>
        /// <param name="messageType">The message type being used.</param>
        private Mapping(Type messageType)
        {
            TypeArguments = new Type2(messageType, typeof(TToken));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Mapping{TToken}"/> class.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to receive.</typeparam>
        /// <returns>A new <see cref="Mapping{TToken}"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mapping<TToken> Create<TMessage>()
            where TMessage : class
        {
            return new(typeof(TMessage));
        }

        /// <inheritdoc/>
        public Type2 TypeArguments { get; }
    }

    /// <summary>
    /// An interface for the <see cref="Mapping"/> and <see cref="Mapping{TToken}"/> types which allows to retrieve
    /// the type arguments from a given generic instance without having any prior knowledge about those arguments.
    /// </summary>
    private interface IMapping : IDictionary2<Recipient>
    {
        /// <summary>
        /// Gets the <see cref="Type2"/> instance representing the current type arguments.
        /// </summary>
        Type2 TypeArguments { get; }
    }

    /// <summary>
    /// A simple type representing a recipient.
    /// </summary>
    /// <remarks>
    /// This type is used to enable fast indexing in each mapping dictionary,
    /// since it acts as an external override for the <see cref="GetHashCode"/> and
    /// <see cref="Equals(object?)"/> methods for arbitrary objects, removing both
    /// the virtual call and preventing instances overriding those methods in this context.
    /// Using this type guarantees that all the equality operations are always only done
    /// based on reference equality for each registered recipient, regardless of its type.
    /// </remarks>
    private readonly struct Recipient : IEquatable<Recipient>
    {
        /// <summary>
        /// The registered recipient.
        /// </summary>
        public readonly object Target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Recipient"/> struct.
        /// </summary>
        /// <param name="target">The target recipient instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Recipient(object target)
        {
            this.Target = target;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Recipient other)
        {
            return ReferenceEquals(this.Target, other.Target);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Recipient other && Equals(other);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this.Target);
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> when trying to add a duplicate handler.
    /// </summary>
    private static void ThrowInvalidOperationExceptionForDuplicateRegistration()
    {
        throw new InvalidOperationException("The target recipient has already subscribed to the target message.");
    }
}

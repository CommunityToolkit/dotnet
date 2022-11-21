// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NETSTANDARD2_0

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices;

/// <summary>
/// A wrapper for <see cref="ConditionalWeakTable{TKey,TValue}"/>
/// that backports the enumerable support to .NET Standard 2.0 through an auxiliary list.
/// </summary>
/// <typeparam name="TKey">Tke key of items to store in the table.</typeparam>
/// <typeparam name="TValue">The values to store in the table.</typeparam>
internal sealed class ConditionalWeakTable2<TKey, TValue>
    where TKey : class
    where TValue : class?
{
    /// <summary>
    /// The underlying <see cref="ConditionalWeakTable{TKey,TValue}"/> instance.
    /// </summary>
    private readonly ConditionalWeakTable<TKey, TValue> table = new();

    /// <summary>
    /// A supporting linked list to store keys in <see cref="table"/>. This is needed to expose
    /// the ability to enumerate existing keys when there is no support for that in the BCL.
    /// </summary>
    private readonly LinkedList<WeakReference<TKey>> keys = new();

    /// <inheritdoc cref="ConditionalWeakTable{TKey,TValue}.TryGetValue"/>
    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
    {
        return this.table.TryGetValue(key, out value);
    }

    /// <inheritdoc cref="ConditionalWeakTableExtensions.TryAdd{TKey, TValue}(ConditionalWeakTable{TKey, TValue}, TKey, TValue)"/>
    public bool TryAdd(TKey key, TValue value)
    {
        if (!this.table.TryAdd(key, value))
        {
            return false;
        }

        // Check if the list of keys contains the given key.
        // If it does, we can just stop here and return the result.
        foreach (WeakReference<TKey> node in this.keys)
        {
            if (node.TryGetTarget(out TKey? target) &&
                ReferenceEquals(target, key))
            {
                return true;
            }
        }

        // Add the key to the list of weak references to track it
        _ = this.keys.AddFirst(new WeakReference<TKey>(key));

        return true;
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey,TValue}.GetValue"/>
    public TValue GetValue(TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback)
    {
        // Get or create the value. When this method returns, the key will be present in the table
        TValue value = this.table.GetValue(key, createValueCallback);

        // Check if the list of keys contains the given key.
        // If it does, we can just stop here and return the result.
        foreach (WeakReference<TKey> node in this.keys)
        {
            if (node.TryGetTarget(out TKey? target) &&
                ReferenceEquals(target, key))
            {
                return value;
            }
        }

        // Add the key to the list of weak references to track it
        _ = this.keys.AddFirst(new WeakReference<TKey>(key));

        return value;
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey,TValue}.Remove"/>
    public bool Remove(TKey key)
    {
        return this.table.Remove(key);
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// A custom enumerator that traverses items in a <see cref="ConditionalWeakTable{TKey, TValue}"/> instance.
    /// </summary>
    public ref struct Enumerator
    {
        /// <summary>
        /// The owner <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance for the enumerator.
        /// </summary>
        private readonly ConditionalWeakTable2<TKey, TValue> owner;

        /// <summary>
        /// The current <see cref="LinkedListNode{T}"/>, if any.
        /// </summary>
        private LinkedListNode<WeakReference<TKey>>? node;

        /// <summary>
        /// The current key, if available.
        /// </summary>
        private TKey? key;

        /// <summary>
        /// The current value, if available.
        /// </summary>
        private TValue? value;

        /// <summary>
        /// Indicates whether or not <see cref="MoveNext"/> has been called at least once.
        /// </summary>
        private bool isFirstMoveNextPending;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        /// </summary>
        /// <param name="owner">The owner <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance for the enumerator.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(ConditionalWeakTable2<TKey, TValue> owner)
        {
            this.owner = owner;
            this.node = null;
            this.key = null;
            this.value = null;
            this.isFirstMoveNextPending = true;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
        }

        /// <inheritdoc cref="Collections.IEnumerator.MoveNext"/>
        public bool MoveNext()
        {
            LinkedListNode<WeakReference<TKey>>? node;

            if (!this.isFirstMoveNextPending)
            {
                node = this.node!.Next;
            }
            else
            {
                node = this.owner.keys.First;

                this.isFirstMoveNextPending = false;
            }

            while (node is not null)
            {
                LinkedListNode<WeakReference<TKey>>? nextNode = node.Next;

                // Get the key and value for the current node
                if (node.Value.TryGetTarget(out TKey? target) &&
                    this.owner.table.TryGetValue(target!, out TValue? value))
                {
                    this.node = node;
                    this.key = target;
                    this.value = value;

                    return true;
                }
                else
                {
                    // If the current key has been collected, trim the list
                    this.owner.keys.Remove(node);
                }

                node = nextNode;
            }

            return false;
        }

        /// <summary>
        /// Gets the current key.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TKey GetKey()
        {
            return this.key!;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetValue()
        {
            return this.value!;
        }
    }
}

#endif
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NETSTANDARD2_1

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices;

/// <summary>
/// A wrapper for <see cref="ConditionalWeakTable{TKey,TValue}"/> with a custom enumerator.
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

    /// <inheritdoc cref="ConditionalWeakTable{TKey,TValue}.TryGetValue"/>
    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
    {
        return this.table.TryGetValue(key, out value);
    }

    /// <inheritdoc cref="ConditionalWeakTableExtensions.TryAdd{TKey, TValue}(ConditionalWeakTable{TKey, TValue}, TKey, TValue)"/>
    public bool TryAdd(TKey key, TValue value)
    {
        return this.table.TryAdd(key, value);
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey,TValue}.GetValue"/>
    public TValue GetValue(TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback)
    {
        return this.table.GetValue(key, createValueCallback);
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
    /// A custom enumerator that traverses items in a <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance.
    /// </summary>
    public ref struct Enumerator
    {
        /// <summary>
        /// The wrapped <see cref="IEnumerator{T}"/> instance for the enumerator.
        /// </summary>
        private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        /// </summary>
        /// <param name="owner">The owner <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance for the enumerator.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(ConditionalWeakTable2<TKey, TValue> owner)
        {
            this.enumerator = ((IEnumerable<KeyValuePair<TKey, TValue>>)owner.table).GetEnumerator();
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            this.enumerator.Dispose();
        }

        /// <inheritdoc cref="Collections.IEnumerator.MoveNext"/>
        public bool MoveNext()
        {
            return this.enumerator.MoveNext();
        }

        /// <summary>
        /// Gets the current key.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TKey GetKey()
        {
            return this.enumerator.Current.Key;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetValue()
        {
            return this.enumerator.Current.Value;
        }
    }
}

#endif
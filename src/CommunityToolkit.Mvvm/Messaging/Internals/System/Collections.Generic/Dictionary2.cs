// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

/// <summary>
/// A specialized <see cref="Dictionary{TKey, TValue}"/> implementation to be used with messenger types.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
[DebuggerDisplay("Count = {Count}")]
internal class Dictionary2<TKey, TValue> : IDictionary2<TKey, TValue>
    where TKey : IEquatable<TKey>
    where TValue : class?
{
    /// <summary>
    /// The index indicating the start of a free linked list.
    /// </summary>
    private const int StartOfFreeList = -3;

    /// <summary>
    /// The array of 1-based indices for the <see cref="Entry"/> items stored in <see cref="entries"/>.
    /// </summary>
    private int[] buckets;

    /// <summary>
    /// The array of currently stored key-value pairs (ie. the lists for each hash group).
    /// </summary>
    private Entry[] entries;

    /// <summary>
    /// A coefficient used to speed up retrieving the target bucket when doing lookups.
    /// </summary>
    private ulong fastModMultiplier;

    /// <summary>
    /// The current number of items stored in the map.
    /// </summary>
    private int count;

    /// <summary>
    /// The 1-based index for the start of the free list within <see cref="entries"/>.
    /// </summary>
    private int freeList;

    /// <summary>
    /// The total number of empty items.
    /// </summary>
    private int freeCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dictionary2{TKey, TValue}"/> class.
    /// </summary>
    public Dictionary2()
    {
        Initialize(0);
    }

    /// <inheritdoc/>
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.count - this.freeCount;
    }

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get
        {
            ref TValue value = ref FindValue(key);

            if (!Unsafe.IsNullRef(ref value))
            {
                return value;
            }

            ThrowArgumentExceptionForKeyNotFound(key);

            return default!;
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        int count = this.count;

        if (count > 0)
        {
#if NETSTANDARD2_0_OR_GREATER
            Array.Clear(this.buckets!, 0, this.buckets!.Length);
#else
            Array.Clear(this.buckets!);
#endif

            this.count = 0;
            this.freeList = -1;
            this.freeCount = 0;

            Array.Clear(this.entries!, 0, count);
        }
    }

    /// <summary>
    /// Checks whether or not the dictionary contains a pair with a specified key.
    /// </summary>
    /// <param name="key">The key to look for.</param>
    /// <returns>Whether or not the key was present in the dictionary.</returns>
    public bool ContainsKey(TKey key)
    {
        return !Unsafe.IsNullRef(ref FindValue(key));
    }

    /// <summary>
    /// Gets the value if present for the specified key.
    /// </summary>
    /// <param name="key">The key to look for.</param>
    /// <param name="value">The value found, otherwise <see langword="default"/>.</param>
    /// <returns>Whether or not the key was present.</returns>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ref TValue valRef = ref FindValue(key);

        if (!Unsafe.IsNullRef(ref valRef))
        {
            value = valRef;
            return true;
        }

        value = default;

        return false;
    }

    /// <inheritdoc/>
    public bool TryRemove(TKey key)
    {
        uint hashCode = (uint)key.GetHashCode();
        ref int bucket = ref GetBucket(hashCode);
        Entry[]? entries = this.entries;
        int last = -1;
        int i = bucket - 1;

        while (i >= 0)
        {
            ref Entry entry = ref entries[i];

            if (entry.HashCode == hashCode && entry.Key.Equals(key))
            {
                if (last < 0)
                {
                    bucket = entry.Next + 1;
                }
                else
                {
                    entries[last].Next = entry.Next;
                }

                entry.Next = StartOfFreeList - this.freeList;

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
#endif
                {
                    entry.Key = default!;
                }

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
#endif
                {
                    entry.Value = default!;
                }

                this.freeList = i;
                this.freeCount++;

                return true;
            }

            last = i;
            i = entry.Next;
        }

        return false;
    }

    /// <summary>
    /// Gets the value for the specified key, or, if the key is not present,
    /// adds an entry and returns the value by ref. This makes it possible to
    /// add or update a value in a single look up operation.
    /// </summary>
    /// <param name="key">Key to look for.</param>
    /// <returns>Reference to the new or existing value.</returns>
    public ref TValue? GetOrAddValueRef(TKey key)
    {
        Entry[] entries = this.entries;
        uint hashCode = (uint)key.GetHashCode();
        ref int bucket = ref GetBucket(hashCode);
        int i = bucket - 1;

        while (true)
        {
            if ((uint)i >= (uint)entries.Length)
            {
                break;
            }

            if (entries[i].HashCode == hashCode && entries[i].Key.Equals(key))
            {
                return ref entries[i].Value!;
            }

            i = entries[i].Next;
        }

        int index;

        if (this.freeCount > 0)
        {
            index = this.freeList;

            this.freeList = StartOfFreeList - entries[this.freeList].Next;
            this.freeCount--;
        }
        else
        {
            int count = this.count;

            if (count == entries.Length)
            {
                Resize();
                bucket = ref GetBucket(hashCode);
            }

            index = count;

            this.count = count + 1;

            entries = this.entries;
        }

        ref Entry entry = ref entries![index];

        entry.HashCode = hashCode;
        entry.Next = bucket - 1;
        entry.Key = key;
        entry.Value = default!;
        bucket = index + 1;

        return ref entry.Value!;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Enumerator for <see cref="Dictionary2{TKey,TValue}"/>.
    /// </summary>
    public ref struct Enumerator
    {
        /// <summary>
        /// The entries being enumerated.
        /// </summary>
        private readonly Entry[] entries;

        /// <summary>
        /// The current enumeration index.
        /// </summary>
        private int index;

        /// <summary>
        /// The current dictionary count.
        /// </summary>
        private readonly int count;

        /// <summary>
        /// Creates a new <see cref="Enumerator"/> instance.
        /// </summary>
        /// <param name="dictionary">The input dictionary to enumerate.</param>
        internal Enumerator(Dictionary2<TKey, TValue> dictionary)
        {
            this.entries = dictionary.entries;
            this.index = 0;
            this.count = dictionary.count;
        }

        /// <inheritdoc cref="IEnumerator.MoveNext"/>
        public bool MoveNext()
        {
            while ((uint)this.index < (uint)this.count)
            {
                // We need to preemptively increment the current index so that we still correctly keep track
                // of the current position in the dictionary even if the users don't access any of the
                // available properties in the enumerator. As this is a possibility, we can't rely on one of
                // them to increment the index before MoveNext is invoked again. We ditch the standard enumerator
                // API surface here to expose the Key/Value properties directly and minimize the memory copies.
                // For the same reason, we also removed the KeyValuePair<TKey, TValue> field here, and instead
                // rely on the properties lazily accessing the target instances directly from the current entry
                // pointed at by the index property (adjusted backwards to account for the increment here).
                if (this.entries![this.index++].Next >= -1)
                {
                    return true;
                }
            }

            this.index = this.count + 1;

            return false;
        }

        /// <summary>
        /// Gets the current key.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TKey GetKey()
        {
            return this.entries[this.index - 1].Key;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TValue GetValue()
        {
            return this.entries[this.index - 1].Value!;
        }
    }

    /// <summary>
    /// Gets the value for the specified key, or.
    /// </summary>
    /// <param name="key">Key to look for.</param>
    /// <returns>Reference to the existing value.</returns>
    private unsafe ref TValue FindValue(TKey key)
    {
        ref Entry entry = ref *(Entry*)null;
        uint hashCode = (uint)key.GetHashCode();
        int i = GetBucket(hashCode);
        Entry[] entries = this.entries;

        i--;
        do
        {
            if ((uint)i >= (uint)entries.Length)
            {
                goto ReturnNotFound;
            }

            entry = ref entries[i];

            if (entry.HashCode == hashCode && entry.Key.Equals(key))
            {
                goto ReturnFound;
            }

            i = entry.Next;
        }
        while (true);

        ReturnFound:
        ref TValue value = ref entry.Value!;

        Return:
        return ref value;

        ReturnNotFound:
        value = ref *(TValue*)null;

        goto Return;
    }

    /// <summary>
    /// Initializes the current instance.
    /// </summary>
    /// <param name="capacity">The target capacity.</param>
    /// <returns></returns>
    [MemberNotNull(nameof(buckets), nameof(entries))]
    private void Initialize(int capacity)
    {
        int size = HashHelpers.GetPrime(capacity);
        int[] buckets = new int[size];
        Entry[] entries = new Entry[size];

        this.freeList = -1;
        this.fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size);
        this.buckets = buckets;
        this.entries = entries;
    }

    /// <summary>
    /// Resizes the current dictionary to reduce the number of collisions
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Resize()
    {
        int newSize = HashHelpers.ExpandPrime(this.count);
        Entry[] entries = new Entry[newSize];
        int count = this.count;

        Array.Copy(this.entries, entries, count);

        this.buckets = new int[newSize];
        this.fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);

        for (int i = 0; i < count; i++)
        {
            if (entries[i].Next >= -1)
            {
                ref int bucket = ref GetBucket(entries[i].HashCode);

                entries[i].Next = bucket - 1;
                bucket = i + 1;
            }
        }

        this.entries = entries;
    }

    /// <summary>
    /// Gets a reference to a target bucket from an input hashcode.
    /// </summary>
    /// <param name="hashCode">The input hashcode.</param>
    /// <returns>A reference to the target bucket.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref int GetBucket(uint hashCode)
    {
        int[] buckets = this.buckets!;

        return ref buckets[HashHelpers.FastMod(hashCode, (uint)buckets.Length, this.fastModMultiplier)];
    }

    /// <summary>
    /// A type representing a map entry, ie. a node in a given list.
    /// </summary>
    private struct Entry
    {
        /// <summary>
        /// The cached hashcode for <see cref="Key"/>;
        /// </summary>
        public uint HashCode;

        /// <summary>
        /// 0-based index of next entry in chain: -1 means end of chain
        /// also encodes whether this entry this.itself_ is part of the free list by changing sign and subtracting 3,
        /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
        /// </summary>
        public int Next;

        /// <summary>
        /// The key for the value in the current node.
        /// </summary>
        public TKey Key;

        /// <summary>
        /// The value in the current node, if present.
        /// </summary>
        public TValue? Value;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when trying to load an element with a missing key.
    /// </summary>
    private static void ThrowArgumentExceptionForKeyNotFound(TKey key)
    {
        throw new ArgumentException($"The target key {key} was not present in the dictionary");
    }
}
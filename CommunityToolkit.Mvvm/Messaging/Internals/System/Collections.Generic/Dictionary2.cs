// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.Collections.Extensions;

namespace System.Collections.Generic;

/// <summary>
/// A specialized <see cref="Dictionary{TKey, TValue}"/> implementation to be used with messenger types.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
[DebuggerDisplay("Count = {Count}")]
internal class Dictionary2<TKey, TValue> : IDictionarySlim<TKey, TValue>
    where TKey : IEquatable<TKey>
    where TValue : class
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

            if (entry.hashCode == hashCode && entry.key.Equals(key))
            {
                if (last < 0)
                {
                    bucket = entry.next + 1;
                }
                else
                {
                    entries[last].next = entry.next;
                }

                entry.next = StartOfFreeList - this.freeList;

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
#endif
                {
                    entry.key = default!;
                }

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
#endif
                {
                    entry.value = default!;
                }

                this.freeList = i;
                this.freeCount++;

                return true;
            }

            last = i;
            i = entry.next;
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

            if (entries[i].hashCode == hashCode && entries[i].key.Equals(key))
            {
                return ref entries[i].value!;
            }

            i = entries[i].next;
        }

        int index;

        if (this.freeCount > 0)
        {
            index = this.freeList;

            this.freeList = StartOfFreeList - entries[this.freeList].next;
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

        entry.hashCode = hashCode;
        entry.next = bucket - 1;
        entry.key = key;
        entry.value = default!;
        bucket = index + 1;

        return ref entry.value!;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Enumerator for <see cref="DictionarySlim{TKey,TValue}"/>.
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
        private int count;

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
            if (this.count == 0)
            {
                return false;
            }

            this.count--;

            Entry[] entries = this.entries;

            // Here we traverse from the current offset until we find the next valid item.
            // We need to preemptively increment the current index so that we still correctly keep track
            // of the current position in the dictionary even if the users doesn't access any of the
            // available properties in the enumerator. As this is a possibility, we can't rely on one of
            // them to increment the index before MoveNext is invoked again. We ditch the standard enumerator
            // API surface here to expose the Key/Value properties directly and minimize the memory copies.
            // For the same reason, we also removed the KeyValuePair<TKey, TValue> field here, and instead
            // rely on the properties lazily accessing the target instances directly from the current entry
            // pointed at by the index property (adjusted backwards to account for the increment here).
            while (entries[this.index++].next < -1)
            {
            }

            return true;
        }

        /// <summary>
        /// Gets the current key.
        /// </summary>
        public TKey Key
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.entries[this.index - 1].key;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.entries[this.index - 1].value!;
        }
    }

    /// <summary>
    /// Gets the value for the specified key, or.
    /// </summary>
    /// <param name="key">Key to look for.</param>
    /// <returns>Reference to the existing value.</returns>
    private ref TValue FindValue(TKey key)
    {
        ref Entry entry = ref Unsafe.NullRef<Entry>();
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
            if (entry.hashCode == hashCode && entry.key.Equals(key))
            {
                goto ReturnFound;
            }

            i = entry.next;
        } while (true);

        ReturnFound:
        ref TValue value = ref entry.value!;

        Return:
        return ref value;

        ReturnNotFound:
        value = ref Unsafe.NullRef<TValue>();

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
            if (entries[i].next >= -1)
            {
                ref int bucket = ref GetBucket(entries[i].hashCode);

                entries[i].next = bucket - 1;
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
        /// The cached hashcode for <see cref="key"/>;
        /// </summary>
        public uint hashCode;

        /// <summary>
        /// 0-based index of next entry in chain: -1 means end of chain
        /// also encodes whether this entry this.itself_ is part of the free list by changing sign and subtracting 3,
        /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
        /// </summary>
        public int next;

        /// <summary>
        /// The key for the value in the current node.
        /// </summary>
        public TKey key;

        /// <summary>
        /// The value in the current node, if present.
        /// </summary>
        public TValue? value;
    }

    /// <summary>
    /// A helper class for <see cref="Dictionary2{TKey,TValue}"/>.
    /// </summary>
    internal static partial class HashHelpers
    {
        /// <summary>
        /// Maximum prime smaller than the maximum array length.
        /// </summary>
        private const int MaxPrimeArrayLength = 0x7FFFFFC3;

        private const int HashPrime = 101;

        /// <summary>
        /// Table of prime numbers to use as hash table sizes.
        /// </summary>
        private static readonly int[] primes =
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
        };

        /// <summary>
        /// Checks whether a value is a prime.
        /// </summary>
        /// <param name="candidate">The value to check.</param>
        /// <returns>Whether or not <paramref name="candidate"/> is a prime.</returns>
        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int limit = (int)Math.Sqrt(candidate);

                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            return candidate == 2;
        }

        /// <summary>
        /// Gets the smallest prime bigger than a specified value.
        /// </summary>
        /// <param name="min">The target minimum value.</param>
        /// <returns>The new prime that was found.</returns>
        public static int GetPrime(int min)
        {
            foreach (int prime in primes)
            {
                if (prime >= min)
                {
                    return prime;
                }
            }

            for (int i = min | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i) && ((i - 1) % HashPrime != 0))
                {
                    return i;
                }
            }

            return min;
        }

        /// <summary>
        /// Returns size of hashtable to grow to.
        /// </summary>
        /// <param name="oldSize">The previous table size.</param>
        /// <returns>The expanded table size.</returns>
        public static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;

            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
            {
                return MaxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }

        /// <summary>
        /// Returns approximate reciprocal of the divisor: ceil(2**64 / divisor).
        /// </summary>
        /// <remarks>This should only be used on 64-bit.</remarks>
        public static ulong GetFastModMultiplier(uint divisor)
        {
            return ulong.MaxValue / divisor + 1;
        }

        /// <summary>
        /// Performs a mod operation using the multiplier pre-computed with <see cref="GetFastModMultiplier"/>.
        /// </summary>
        /// <remarks>This should only be used on 64-bit.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FastMod(uint value, uint divisor, ulong multiplier)
        {
            return (uint)(((((multiplier * value) >> 32) + 1) * divisor) >> 32);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when trying to load an element with a missing key.
    /// </summary>
    private static void ThrowArgumentExceptionForKeyNotFound(TKey key)
    {
        throw new ArgumentException($"The target key {key} was not present in the dictionary");
    }
}
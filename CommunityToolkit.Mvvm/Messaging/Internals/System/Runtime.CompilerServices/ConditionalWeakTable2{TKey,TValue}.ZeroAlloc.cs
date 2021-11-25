// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET6_0_OR_GREATER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable CA1816

namespace CommunityToolkit.Mvvm.Messaging.Internals;

/// <summary>
/// A custom <see cref="ConditionalWeakTable{TKey, TValue}"/> instance that is specifically optimized to be used
/// by <see cref="WeakReferenceMessenger"/>. In particular, it offers zero-allocation enumeration of stored items.
/// </summary>
/// <typeparam name="TKey">Tke key of items to store in the table.</typeparam>
/// <typeparam name="TValue">The values to store in the table.</typeparam>
internal sealed class ConditionalWeakTable2<TKey, TValue>
    where TKey : class
    where TValue : class?
{
    /// <summary>
    /// Initial length of the table. Must be a power of two.
    /// </summary>
    private const int InitialCapacity = 8;

    /// <summary>
    /// This lock protects all mutation of data in the table. Readers do not take this lock.
    /// </summary>
    private readonly object lockObject;

    /// <summary>
    /// The actual storage for the table; swapped out as the table grows.
    /// </summary>
    private volatile Container container;

    /// <summary>
    /// The number of outstanding enumerators on the table
    /// </summary>
    private int activeEnumeratorRefCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalWeakTable2{TKey, TValue}"/> class.
    /// </summary>
    public ConditionalWeakTable2()
    {
        this.lockObject = new object();
        this.container = new Container(this);
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return this.container.TryGetValueWorker(key, out value);
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey, TValue}.Remove(TKey)"/>
    public bool Remove(TKey key)
    {
        lock (this.lockObject)
        {
            return this.container.Remove(key);
        }
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey, TValue}.GetValue(TKey, ConditionalWeakTable{TKey, TValue}.CreateValueCallback)"/>
    public TValue GetValue(TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback)
    {
        return TryGetValue(key, out TValue? existingValue) ?
            existingValue :
            GetValueLocked(key, createValueCallback);
    }

    /// <summary>
    /// Implements the functionality for <see cref="GetValue(TKey, ConditionalWeakTable{TKey, TValue}.CreateValueCallback)"/> under a lock.
    /// </summary>
    /// <param name="key">The input key.</param>
    /// <param name="createValueCallback">The callback to use to create a new item.</param>
    /// <returns>The new <typeparamref name="TValue"/> item to store.</returns>
    private TValue GetValueLocked(TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback)
    {
        // If we got here, the key was not in the table. Invoke the callback
        // (outside the lock) to generate the new value for the key.
        TValue newValue = createValueCallback(key);

        lock (this.lockObject)
        {
            // Now that we've taken the lock, must recheck in case we lost a race to add the key
            if (this.container.TryGetValueWorker(key, out TValue? existingValue))
            {
                return existingValue;
            }
            else
            {
                // Verified in-lock that we won the race to add the key. Add it now.
                CreateEntry(key, newValue);

                return newValue;
            }
        }
    }

    /// <inheritdoc/>
    public Enumerator GetEnumerator()
    {
        lock (this.lockObject)
        {
            Container c = this.container;

            return c is null || c.FirstFreeEntry == 0 ? default : new Enumerator(this);
        }
    }

    /// <summary>
    /// Provides an enumerator for the current <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance.
    /// </summary>
    public ref struct Enumerator
    {
        /// <summary>
        /// Parent table, set to null when disposed.
        /// </summary>
        private ConditionalWeakTable2<TKey, TValue>? table;

        /// <summary>
        /// Last index in the container that should be enumerated.
        /// </summary>
        private readonly int maxIndexInclusive;

        /// <summary>
        /// The current index into the container.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The current entry set by MoveNext and returned from <see cref="Current"/>.
        /// </summary>
        private KeyValuePair<TKey, TValue> current;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> class.
        /// </summary>
        /// <param name="table">The input <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance being enumerated.</param>
        public Enumerator(ConditionalWeakTable2<TKey, TValue> table)
        {
            // Store a reference to the parent table and increase its active enumerator count
            this.table = table;

            table.activeEnumeratorRefCount++;

            // Store the max index to be enumerated
            this.maxIndexInclusive = table.container.FirstFreeEntry - 1;
            this.currentIndex = -1;
            this.current = default;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            // Use an interlocked operation to ensure that only one thread can get access
            // to the _table for disposal and thus only decrement the ref count once
            ConditionalWeakTable2<TKey, TValue>? table = Interlocked.Exchange(ref this.table, null);

            if (table != null)
            {
                // Ensure we don't keep the last current alive unnecessarily
                this.current = default;

                // Decrement the ref count that was incremented when constructed
                lock (table.lockObject)
                {
                    table.activeEnumeratorRefCount--;
                }
            }
        }

        /// <inheritdoc cref="IEnumerator.MoveNext"/>
        public bool MoveNext()
        {
            // Start by getting the current table. If it's already been disposed, it will be null
            ConditionalWeakTable2<TKey, TValue>? table = this.table;

            if (table != null)
            {
                // Once have the table, we need to lock to synchronize with other operations on the table, like adding
                lock (table.lockObject)
                {
                    // From the table, we have to get the current container. This could have changed
                    // since we grabbed the enumerator, but the index-to-pair mapping should not have
                    // due to there being at least one active enumerator. If the table (or rather its
                    // container at the time) has already been finalized, this will be null.
                    Container c = table.container;

                    if (c != null)
                    {
                        // We have the container. Find the next entry to return, if there is one. We need to loop as we
                        // may try to get an entry that's already been removed or collected, in which case we try again.
                        while (this.currentIndex < this.maxIndexInclusive)
                        {
                            this.currentIndex++;

                            if (c.TryGetEntry(this.currentIndex, out TKey? key, out TValue? value))
                            {
                                this.current = new KeyValuePair<TKey, TValue>(key, value);

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public KeyValuePair<TKey, TValue> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.current;
        }
    }

    /// <summary>
    /// Worker for adding a new key/value pair. Will resize the container if it is full.
    /// </summary>
    /// <param name="key">The key for the new entry.</param>
    /// <param name="value">The value for the new entry.</param>
    private void CreateEntry(TKey key, TValue value)
    {
        Container c = this.container;

        if (!c.HasCapacity)
        {
            container = c = c.Resize();
        }

        c.CreateEntryNoResize(key, value);
    }

    /// <summary>
    /// A single entry within a <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance.
    /// </summary>
    private struct Entry
    {
        /// <summary>
        /// Holds key and value using a weak reference for the key and a strong reference for the
        /// value that is traversed only if the key is reachable without going through the value.
        /// </summary>
        public DependentHandle depHnd;

        /// <summary>
        /// Cached copy of key's hashcode.
        /// </summary>
        public int HashCode;

        /// <summary>
        /// Index of next entry, -1 if last.
        /// </summary>
        public int Next;
    }

    /// <summary>
    /// Container holds the actual data for the table. A given instance of Container always has the same capacity. When we need
    /// more capacity, we create a new Container, copy the old one into the new one, and discard the old one. This helps enable
    /// lock-free reads from the table, as readers never need to deal with motion of entries due to rehashing.
    /// </summary>
    private sealed class Container
    {
        /// <summary>
        /// The <see cref="ConditionalWeakTable2{TKey, TValue}"/> with which this container is associated.
        /// </summary>
        private readonly ConditionalWeakTable2<TKey, TValue> parent;

        /// <summary>
        /// <c>_buckets[hashcode &amp; (_buckets.Length - 1)]</c> contains index of the first entry in bucket (-1 if empty).
        /// </summary>
        private int[] buckets;

        /// <summary>
        /// The table entries containing the stored dependency handles
        /// </summary>
        private Entry[] entries;

        /// <summary>
        /// <c>_firstFreeEntry &lt; _entries.Length => table</c> has capacity, entries grow from the bottom of the table.
        /// </summary>
        private int firstFreeEntry;

        /// <summary>
        /// Flag detects if OOM or other background exception threw us out of the lock.
        /// </summary>
        private bool invalid;

        /// <summary>
        /// Set to true when initially finalized
        /// </summary>
        private bool finalized;

        /// <summary>
        /// Used to ensure the next allocated container isn't finalized until this one is GC'd.
        /// </summary>
        private volatile object? oldKeepAlive;

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="parent">The input <see cref="ConditionalWeakTable2{TKey, TValue}"/> object associated with the current instance.</param>
        internal Container(ConditionalWeakTable2<TKey, TValue> parent)
        {
            this.buckets = new int[InitialCapacity];

            for (int i = 0; i < this.buckets.Length; i++)
            {
                this.buckets[i] = -1;
            }

            this.entries = new Entry[InitialCapacity];

            // Only store the parent after all of the allocations have happened successfully.
            // Otherwise, as part of growing or clearing the container, we could end up allocating
            // a new Container that fails (OOMs) part way through construction but that gets finalized
            // and ends up clearing out some other container present in the associated CWT.
            this.parent = parent;
        }

        private Container(ConditionalWeakTable2<TKey, TValue> parent, int[] buckets, Entry[] entries, int firstFreeEntry)
        {
            this.parent = parent;
            this.buckets = buckets;
            this.entries = entries;
            this.firstFreeEntry = firstFreeEntry;
        }

        internal bool HasCapacity => this.firstFreeEntry < this.entries.Length;

        internal int FirstFreeEntry => this.firstFreeEntry;

        /// <summary>
        /// Worker for adding a new key/value pair. Container must NOT be full.
        /// </summary>
        internal void CreateEntryNoResize(TKey key, TValue value)
        {
            VerifyIntegrity();

            this.invalid = true;

            int hashCode = RuntimeHelpers.GetHashCode(key) & int.MaxValue;
            int newEntry = this.firstFreeEntry++;

            this.entries[newEntry].HashCode = hashCode;
            this.entries[newEntry].depHnd = new DependentHandle(key, value);

            int bucket = hashCode & (this.buckets.Length - 1);

            this.entries[newEntry].Next = this.buckets[bucket];

            // This write must be volatile, as we may be racing with concurrent readers. If they
            // see the new entry, they must also see all of the writes earlier in this method.
            Volatile.Write(ref this.buckets[bucket], newEntry);

            this.invalid = false;
        }

        /// <summary>
        /// Worker for finding a key/value pair. Must hold _lock.
        /// </summary>
        internal bool TryGetValueWorker(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            int entryIndex = FindEntry(key, out object? secondary);

            value = Unsafe.As<TValue>(secondary);

            return entryIndex != -1;
        }

        /// <summary>
        /// Returns -1 if not found (if key expires during FindEntry, this can be treated as "not found.").
        /// Must hold _lock, or be prepared to retry the search while holding _lock.
        /// </summary>
        /// <remarks>This method requires <paramref name="value"/> to be on the stack to be properly tracked.</remarks>
        internal int FindEntry(TKey key, out object? value)
        {
            int hashCode = RuntimeHelpers.GetHashCode(key) & int.MaxValue;
            int bucket = hashCode & (this.buckets.Length - 1);

            for (int entriesIndex = Volatile.Read(ref this.buckets[bucket]); entriesIndex != -1; entriesIndex = this.entries[entriesIndex].Next)
            {
                if (this.entries[entriesIndex].HashCode == hashCode)
                {
                    // if (_entries[entriesIndex].depHnd.UnsafeGetTargetAndDependent(out value) == key)
                    (object? oKey, value) = this.entries[entriesIndex].depHnd.TargetAndDependent;

                    if (oKey == key)
                    {
                        // Ensure we don't get finalized while accessing DependentHandle
                        GC.KeepAlive(this);

                        return entriesIndex;
                    }
                }
            }

            // Ensure we don't get finalized while accessing DependentHandle
            GC.KeepAlive(this);

            value = null;

            return -1;
        }

        /// <summary>
        /// Gets the entry at the specified entry index.
        /// </summary>
        internal bool TryGetEntry(int index, [NotNullWhen(true)] out TKey? key, [MaybeNullWhen(false)] out TValue value)
        {
            if (index < this.entries.Length)
            {
                // object? oKey = _entries[index].depHnd.UnsafeGetTargetAndDependent(out object? oValue);
                (object? oKey, object? oValue) = this.entries[index].depHnd.TargetAndDependent;

                // Ensure we don't get finalized while accessing DependentHandle
                GC.KeepAlive(this);

                if (oKey != null)
                {
                    key = Unsafe.As<TKey>(oKey);
                    value = Unsafe.As<TValue>(oValue)!;

                    return true;
                }
            }

            key = default;
            value = default;

            return false;
        }

        /// <summary>
        /// Removes the specified key from the table, if it exists.
        /// </summary>
        internal bool Remove(TKey key)
        {
            VerifyIntegrity();

            int entryIndex = FindEntry(key, out _);

            if (entryIndex != -1)
            {
                RemoveIndex(entryIndex);
                return true;
            }

            return false;
        }

        private void RemoveIndex(int entryIndex)
        {
            ref Entry entry = ref this.entries[entryIndex];

            // We do not free the handle here, as we may be racing with readers who already saw the hash code.
            // Instead, we simply overwrite the entry's hash code, so subsequent reads will ignore it.
            // The handle will be free'd in Container's finalizer, after the table is resized or discarded.
            Volatile.Write(ref entry.HashCode, -1);

            // Also, clear the key to allow GC to collect objects pointed to by the entry
            // entry.depHnd.UnsafeSetTargetToNull();
            entry.depHnd.Target = null;
        }

        /// <summary>
        /// Resize, and scrub expired keys off bucket lists. Must hold _lock.
        /// </summary>
        /// <remarks>
        /// _firstEntry is less than _entries.Length on exit, that is, the table has at least one free entry.
        /// </remarks>
        internal Container Resize()
        {
            bool hasExpiredEntries = false;
            int newSize = this.buckets.Length;

            if (this.parent is null || this.parent.activeEnumeratorRefCount == 0)
            {
                // If any expired or removed keys exist, we won't resize. If there any active
                // enumerators, though, we don't want to compact and thus have no expired entries.
                for (int entriesIndex = 0; entriesIndex < this.entries.Length; entriesIndex++)
                {
                    ref Entry entry = ref this.entries[entriesIndex];

                    if (entry.HashCode == -1)
                    {
                        // the entry was removed
                        hasExpiredEntries = true;
                        break;
                    }

                    if (entry.depHnd.IsAllocated &&
                        // entry.depHnd.UnsafeGetTarget() is null)
                        entry.depHnd.Target is null)
                    {
                        // the entry has expired
                        hasExpiredEntries = true;

                        break;
                    }
                }
            }

            if (!hasExpiredEntries)
            {
                // Not necessary to check for overflow here, the attempt to allocate new arrays will throw
                newSize = this.buckets.Length * 2;
            }

            return Resize(newSize);
        }

        internal Container Resize(int newSize)
        {
            // Reallocate both buckets and entries and rebuild the bucket and entries from scratch.
            // This serves both to scrub entries with expired keys and to put the new entries in the proper bucket.
            int[] newBuckets = new int[newSize];

            for (int bucketIndex = 0; bucketIndex < newBuckets.Length; bucketIndex++)
            {
                newBuckets[bucketIndex] = -1;
            }

            Entry[] newEntries = new Entry[newSize];
            int newEntriesIndex = 0;
            bool activeEnumerators = this.parent != null && this.parent.activeEnumeratorRefCount > 0;

            // Migrate existing entries to the new table.
            if (activeEnumerators)
            {
                // There's at least one active enumerator, which means we don't want to
                // remove any expired/removed entries, in order to not affect existing
                // entries indices.  Copy over the entries while rebuilding the buckets list,
                // as the buckets are dependent on the buckets list length, which is changing.
                for (; newEntriesIndex < this.entries.Length; newEntriesIndex++)
                {
                    ref Entry oldEntry = ref this.entries[newEntriesIndex];
                    ref Entry newEntry = ref newEntries[newEntriesIndex];
                    int hashCode = oldEntry.HashCode;

                    newEntry.HashCode = hashCode;
                    newEntry.depHnd = oldEntry.depHnd;

                    int bucket = hashCode & (newBuckets.Length - 1);

                    newEntry.Next = newBuckets[bucket];
                    newBuckets[bucket] = newEntriesIndex;
                }
            }
            else
            {
                // There are no active enumerators, which means we want to compact by removing expired/removed entries.
                for (int entriesIndex = 0; entriesIndex < this.entries.Length; entriesIndex++)
                {
                    ref Entry oldEntry = ref this.entries[entriesIndex];
                    int hashCode = oldEntry.HashCode;
                    DependentHandle depHnd = oldEntry.depHnd;

                    if (hashCode != -1 && depHnd.IsAllocated)
                    {
                        // if (depHnd.UnsafeGetTarget() is not null)
                        if (depHnd.Target is not null)
                        {
                            ref Entry newEntry = ref newEntries[newEntriesIndex];

                            // Entry is used and has not expired. Link it into the appropriate bucket list.
                            newEntry.HashCode = hashCode;
                            newEntry.depHnd = depHnd;

                            int bucket = hashCode & (newBuckets.Length - 1);

                            newEntry.Next = newBuckets[bucket];
                            newBuckets[bucket] = newEntriesIndex;
                            newEntriesIndex++;
                        }
                        else
                        {
                            // Pretend the item was removed, so that this container's finalizer will clean up this dependent handle.
                            Volatile.Write(ref oldEntry.HashCode, -1);
                        }
                    }
                }
            }

            // Create the new container. We want to transfer the responsibility of freeing the handles from
            // the old container to the new container, and also ensure that the new container isn't finalized
            // while the old container may still be in use. As such, we store a reference from the old container
            // to the new one, which will keep the new container alive as long as the old one is.
            Container newContainer = new(this.parent!, newBuckets, newEntries, newEntriesIndex);

            if (activeEnumerators)
            {
                // If there are active enumerators, both the old container and the new container may be storing
                // the same entries with -1 hash codes, which the finalizer will clean up even if the container
                // is not the active container for the table. To prevent that, we want to stop the old container
                // from being finalized, as it no longer has any responsibility for any cleanup.
                GC.SuppressFinalize(this);
            }

            // Once this is set, the old container's finalizer will not free transferred dependent handles
            this.oldKeepAlive = newContainer;

            // Ensure we don't get finalized while accessing DependentHandles.
            GC.KeepAlive(this);

            return newContainer;
        }

        private void VerifyIntegrity()
        {
            if (this.invalid)
            {
                static void Throw() => throw new InvalidOperationException("The current collection is in a corrupted state");

                Throw();
            }
        }

        ~Container()
        {
            // Skip doing anything if the container is invalid, including if somehow
            // the container object was allocated but its associated table never set.
            if (this.invalid || this.parent is null)
            {
                return;
            }

            // It's possible that the ConditionalWeakTable2 could have been resurrected, in which case code could
            // be accessing this Container as it's being finalized.  We don't support usage after finalization,
            // but we also don't want to potentially corrupt state by allowing dependency handles to be used as
            // or after they've been freed.  To avoid that, if it's at all possible that another thread has a
            // reference to this container via the CWT, we remove such a reference and then re-register for
            // finalization: the next time around, we can be sure that no references remain to this and we can
            // clean up the dependency handles without fear of corruption.
            if (!this.finalized)
            {
                this.finalized = true;

                lock (this.parent.lockObject)
                {
                    if (this.parent.container == this)
                    {
                        this.parent.container = null!;
                    }
                }

                // Next time it's finalized, we'll be sure there are no remaining refs
                GC.ReRegisterForFinalize(this);

                return;
            }

            Entry[] entries = this.entries;

            this.invalid = true;
            this.entries = null!;
            this.buckets = null!;

            if (entries != null)
            {
                for (int entriesIndex = 0; entriesIndex < entries.Length; entriesIndex++)
                {
                    // We need to free handles in two cases:
                    // - If this container still owns the dependency handle (meaning ownership hasn't been transferred
                    //   to another container that replaced this one), then it should be freed.
                    // - If this container had the entry removed, then even if in general ownership was transferred to
                    //   another container, removed entries are not, therefore this container must free them.
                    if (this.oldKeepAlive is null || entries[entriesIndex].HashCode == -1)
                    {
                        entries[entriesIndex].depHnd.Dispose();
                    }
                }
            }
        }
    }
}

#endif
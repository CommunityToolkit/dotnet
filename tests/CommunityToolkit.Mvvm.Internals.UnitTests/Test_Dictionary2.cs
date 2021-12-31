// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.Internals.UnitTests;

[TestClass]
public class Test_Dictionary2
{
    private static void AddItems<T>(T[] keys)
        where T : IEquatable<T>
    {
        object[] values = Enumerable.Range(0, keys.Length).Select(static x => new object()).ToArray();

        Dictionary2<T, object> dictionary = new();

        // Go through all keys once
        for (int i = 0; i < keys.Length; i++)
        {
            T key = keys[i];

            // Verify the key doesn't exist yet
            Assert.AreEqual(i, dictionary.Count);
            Assert.IsFalse(dictionary.ContainsKey(key));
            Assert.IsFalse(dictionary.TryGetValue(key, out object? obj));
            Assert.IsNull(obj);
            Assert.IsFalse(dictionary.TryRemove(key));

            _ = Assert.ThrowsException<ArgumentException>(() => _ = dictionary[key]);

            // This should create a new entry
            ref object? value = ref dictionary.GetOrAddValueRef(key);

            Assert.IsNull(value);

            value = values[i];

            // Verify the key now exists
            Assert.IsTrue(dictionary.ContainsKey(key));
            Assert.IsTrue(dictionary.TryGetValue(key, out obj));
            Assert.AreSame(obj, value);
            Assert.AreSame(obj, values[i]);

            // Get the key again, should point to the same location
            ref object? value2 = ref dictionary.GetOrAddValueRef(key);

            Assert.IsTrue(Unsafe.AreSame(ref value, ref value2!));
        }

        Assert.AreEqual(keys.Length, dictionary.Count);

        HashSet<T> keysSet = new();
        HashSet<object> valuesSet = new();

        Dictionary2<T, object>.Enumerator enumerator = dictionary.GetEnumerator();

        // Gather all key/value pairs through the enumerator
        while (enumerator.MoveNext())
        {
            Assert.IsTrue(keysSet.Add(enumerator.GetKey()));
            Assert.IsTrue(valuesSet.Add(enumerator.GetValue()));
        }

        // Verify we have all values we expect
        CollectionAssert.AreEquivalent(keys, keysSet.ToArray());
        CollectionAssert.AreEquivalent(values, valuesSet.ToArray());

        dictionary.Clear();

        // Verify the dictionary was cleared properly
        Assert.AreEqual(0, dictionary.Count);

        enumerator = dictionary.GetEnumerator();

        while (enumerator.MoveNext())
        {
            // Since the dictionary is now empty, we should never get here
            Assert.Fail();
        }
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(101)]
    [DataRow(2467)]
    public void AddItems_Int(int count)
    {
        AddItems(Enumerable.Range(0, count).ToArray());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(101)]
    [DataRow(2467)]
    public void AddItems_String(int count)
    {
        AddItems(Enumerable.Range(0, count).Select(static x => x.ToString()).ToArray());
    }

    private static void TryRemoveItems<T>(T[] keys)
        where T : IEquatable<T>
    {
        object[] values = Enumerable.Range(0, keys.Length).Select(static x => new object()).ToArray();

        Dictionary2<T, object> dictionary = new();

        // Populate the dictionary
        for (int i = 0; i < keys.Length; i++)
        {
            T key = keys[i];

            dictionary.GetOrAddValueRef(key) = values[i];
        }

        Random random = new(42);

        keys = keys
            .Select(x => (Key: x, Index: random.Next()))
            .OrderBy(static x => x.Index)
            .Select(static x => x.Key)
            .ToArray();

        values = keys.Select(k => dictionary[k]).ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            T key = keys[i];

            // Verify the state is consistent across removals
            Assert.AreEqual(keys.Length - i, dictionary.Count);
            Assert.IsTrue(dictionary.ContainsKey(key));
            Assert.IsTrue(dictionary.TryGetValue(key, out object? obj));
            Assert.AreSame(values[i], obj);

            // Remove the item
            Assert.IsTrue(dictionary.TryRemove(key));

            // Verify the state is consistent after the removal
            Assert.IsFalse(dictionary.ContainsKey(key));
            Assert.IsFalse(dictionary.TryGetValue(key, out obj));
            Assert.IsNull(obj);
        }

        Assert.AreEqual(0, dictionary.Count);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(101)]
    [DataRow(2467)]
    public void TryRemoveItems_Int(int count)
    {
        TryRemoveItems(Enumerable.Range(0, count).ToArray());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(101)]
    [DataRow(2467)]
    public void TryRemoveItems_String(int count)
    {
        TryRemoveItems(Enumerable.Range(0, count).Select(static x => x.ToString()).ToArray());
    }
}

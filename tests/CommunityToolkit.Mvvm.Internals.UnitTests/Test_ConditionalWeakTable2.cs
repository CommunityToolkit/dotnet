// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.Internals.UnitTests;

[TestClass]
public class Test_ConditionalWeakTable2
{
    [TestMethod]
    [DataRow(1)]
    [DataRow(100)]
    public void Add(int numObjects)
    {
        static Tuple<ConditionalWeakTable2<object, object>, WeakReference[], WeakReference[]> body(int count)
        {
            object[] keys = Enumerable.Range(0, count).Select(_ => new object()).ToArray();
            object[] values = Enumerable.Range(0, count).Select(_ => new object()).ToArray();
            ConditionalWeakTable2<object, object> cwt = new();

            for (int i = 0; i < count; i++)
            {
                _ = cwt.GetValue(keys[i], _ => values[i]);
            }

            for (int i = 0; i < count; i++)
            {
                object? value;

                Assert.IsTrue(cwt.TryGetValue(keys[i], out value));
                Assert.AreSame(values[i], value);
                Assert.AreSame(value, cwt.GetValue(keys[i], _ => new object()));
            }

            return Tuple.Create(cwt, keys.Select(k => new WeakReference(k)).ToArray(), values.Select(v => new WeakReference(v)).ToArray());
        }

        Tuple<ConditionalWeakTable2<object, object>, WeakReference[], WeakReference[]> result = body(numObjects);

        GC.Collect();

        Assert.IsNotNull(result.Item1);

        for (int i = 0; i < numObjects; i++)
        {
            Assert.IsFalse(result.Item2[i].IsAlive, $"Expected not to find key #{i}");
            Assert.IsFalse(result.Item3[i].IsAlive, $"Expected not to find value #{i}");
        }
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(100)]
    public void AddMany_ThenRemoveAll(int numObjects)
    {
        object[] keys = Enumerable.Range(0, numObjects).Select(_ => new object()).ToArray();
        object[] values = Enumerable.Range(0, numObjects).Select(_ => new object()).ToArray();
        ConditionalWeakTable2<object, object> cwt = new();

        for (int i = 0; i < numObjects; i++)
        {
            _ = cwt.GetValue(keys[i], _ => values[i]);
        }

        for (int i = 0; i < numObjects; i++)
        {
            Assert.AreSame(values[i], cwt.GetValue(keys[i], _ => new object()));
        }

        for (int i = 0; i < numObjects; i++)
        {
            Assert.IsTrue(cwt.Remove(keys[i]));
            Assert.IsFalse(cwt.Remove(keys[i]));
        }

        for (int i = 0; i < numObjects; i++)
        {
            Assert.IsFalse(cwt.TryGetValue(keys[i], out _));
        }
    }

    [TestMethod]
    [DataRow(100)]
    public void AddRemoveIteratively(int numObjects)
    {
        object[] keys = Enumerable.Range(0, numObjects).Select(_ => new object()).ToArray();
        object[] values = Enumerable.Range(0, numObjects).Select(_ => new object()).ToArray();
        ConditionalWeakTable2<object, object> cwt = new();

        for (int i = 0; i < numObjects; i++)
        {
            _ = cwt.GetValue(keys[i], _ => values[i]);

            Assert.AreSame(values[i], cwt.GetValue(keys[i], _ => new object()));
            Assert.IsTrue(cwt.Remove(keys[i]));
            Assert.IsFalse(cwt.Remove(keys[i]));
        }
    }

    [TestMethod]
    public void Concurrent_AddMany_DropReferences()
    {
        ConditionalWeakTable2<object, object> cwt = new();

        for (int i = 0; i < 10000; i++)
        {
            _ = cwt.GetValue(i.ToString(), _ => i.ToString());

            if (i % 1000 == 0)
            {
                GC.Collect();
            }
        }
    }

    [TestMethod]
    public void Concurrent_Add_Read_Remove_DifferentObjects()
    {
        ConditionalWeakTable2<object, object> cwt = new();
        DateTime end = DateTime.UtcNow + TimeSpan.FromSeconds(0.25);

        _ = Parallel.For(0, Environment.ProcessorCount, i =>
        {
            while (DateTime.UtcNow < end)
            {
                object key = new();
                object value = new();

                _ = cwt.GetValue(key, _ => value);

                Assert.AreSame(value, cwt.GetValue(key, _ => new object()));
                Assert.IsTrue(cwt.Remove(key));
                Assert.IsFalse(cwt.Remove(key));
            }
        });
    }

    [TestMethod]
    public void Concurrent_GetValue_Read_Remove_DifferentObjects()
    {
        ConditionalWeakTable2<object, object> cwt = new();
        DateTime end = DateTime.UtcNow + TimeSpan.FromSeconds(0.25);

        _ = Parallel.For(0, Environment.ProcessorCount, i =>
        {
            while (DateTime.UtcNow < end)
            {
                object key = new();
                object value = new();

                Assert.AreSame(value, cwt.GetValue(key, _ => value));
                Assert.IsTrue(cwt.Remove(key));
                Assert.IsFalse(cwt.Remove(key));
            }
        });
    }

    [TestMethod]
    public void Concurrent_GetValue_Read_Remove_SameObject()
    {
        object key = new();
        object value = new();
        ConditionalWeakTable2<object, object> cwt = new();
        DateTime end = DateTime.UtcNow + TimeSpan.FromSeconds(0.25);

        _ = Parallel.For(0, Environment.ProcessorCount, i =>
        {
            while (DateTime.UtcNow < end)
            {
                Assert.AreSame(value, cwt.GetValue(key, _ => value));

                _ = cwt.Remove(key);
            }
        });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference GetWeakCondTabRef(out ConditionalWeakTable2<object, object> cwt_out, out object key_out)
    {
        object key = new();
        object value = new();
        ConditionalWeakTable2<object, object> cwt = new();

        _ = cwt.GetValue(key, _ => value);
        _ = cwt.Remove(key);

        // Return 3 values to the caller, drop everything else on the floor.
        cwt_out = cwt;
        key_out = key;

        return new(value);
    }

    [TestMethod]
    public void AddRemove_DropValue()
    {
        // Verify that the removed entry is not keeping the value alive
        ConditionalWeakTable2<object, object> cwt;
        object key;

        WeakReference wrValue = GetWeakCondTabRef(out cwt, out key);

        GC.Collect();

        Assert.IsFalse(wrValue.IsAlive);

        GC.KeepAlive(cwt);
        GC.KeepAlive(key);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void GetWeakRefPair(out WeakReference<object> key_out, out WeakReference<object> val_out)
    {
        ConditionalWeakTable2<object, object> cwt = new();
        object key = new();
        object? value = cwt.GetValue(key, _ => new object());

        Assert.IsTrue(cwt.TryGetValue(key, out value));
        Assert.AreSame(value, cwt.GetValue(key, k => new object()));

        val_out = new WeakReference<object>(value!, false);
        key_out = new WeakReference<object>(key, false);
    }

    [TestMethod]
    public void GetOrCreateValue()
    {
        WeakReference<object> wrValue;
        WeakReference<object> wrKey;

        GetWeakRefPair(out wrKey, out wrValue);

        GC.Collect();

        Assert.IsFalse(wrValue.TryGetTarget(out _));
        Assert.IsFalse(wrKey.TryGetTarget(out _));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void GetWeakRefValPair(out WeakReference<object> key_out, out WeakReference<object> val_out)
    {
        ConditionalWeakTable2<object, object> cwt = new();
        object key = new();
        object? value = cwt.GetValue(key, k => new object());

        Assert.IsTrue(cwt.TryGetValue(key, out value));
        Assert.AreSame(value, cwt.GetValue(key, k => new object()));

        val_out = new WeakReference<object>(value!, false);
        key_out = new WeakReference<object>(key, false);
    }

    [TestMethod]
    public void GetValue()
    {
        WeakReference<object> wrValue;
        WeakReference<object> wrKey;

        GetWeakRefValPair(out wrKey, out wrValue);

        GC.Collect();

        Assert.IsFalse(wrValue.TryGetTarget(out _));
        Assert.IsFalse(wrKey.TryGetTarget(out _));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(100)]
    public void RemoveAllItems_AllValuesRemoved(int numObjects)
    {
        ConditionalWeakTable2<object, object> cwt = new();

        object[] keys = Enumerable.Range(0, numObjects).Select(_ => new object()).ToArray();
        object[] values = Enumerable.Range(0, numObjects).Select(_ => new object()).ToArray();

        for (int iter = 0; iter < 2; iter++)
        {
            for (int i = 0; i < numObjects; i++)
            {
                _ = cwt.GetValue(keys[i], _ => values[i]);

                Assert.AreSame(values[i], cwt.GetValue(keys[i], _ => new object()));
            }

            for (int i = 0; i < numObjects; i++)
            {
                Assert.IsTrue(cwt.Remove(keys[i]));
            }

            for (int i = 0; i < numObjects; i++)
            {
                Assert.IsFalse(cwt.TryGetValue(keys[i], out _));
            }
        }
    }

    [TestMethod]
    public void AddOrUpdateDataTest()
    {
        ConditionalWeakTable2<string, string> cwt = new();
        string key = "key1";

        _ = cwt.GetValue(key, _ => "value1");

        Assert.IsTrue(cwt.TryGetValue(key, out string? value));
        Assert.AreEqual("value1", value);
        Assert.AreEqual(value, cwt.GetValue(key, _ => ""));
        Assert.AreEqual(value, cwt.GetValue(key, k => "value1"));
    }

    // This test is skipped as the custom table doesn't have a Clear() method
    // public void Clear_EmptyTable()

    [TestMethod]
    public void RemoveAll_AddThenEmptyRepeatedly_ItemsRemoved()
    {
        ConditionalWeakTable2<object, object> cwt = new();
        object key = new();
        object value = new();
        object? result;

        for (int i = 0; i < 3; i++)
        {
            _ = cwt.GetValue(key, _ => value);

            Assert.IsTrue(cwt.TryGetValue(key, out result));
            Assert.AreSame(value, result);

            Assert.IsTrue(cwt.Remove(key));

            Assert.IsFalse(cwt.TryGetValue(key, out result));
            Assert.IsNull(result);
        }
    }

    [TestMethod]
    public void RemoveAll_AddMany_RemoveAll_AllItemsRemoved()
    {
        ConditionalWeakTable2<object, object> cwt = new();

        object[] keys = Enumerable.Range(0, 33).Select(_ => new object()).ToArray();
        object[] values = Enumerable.Range(0, keys.Length).Select(_ => new object()).ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            object current = cwt.GetValue(keys[i], _ => values[i]);

            Assert.AreSame(current, values[i]);
        }

        int count = 0;

        using (ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Assert.AreSame(enumerator.GetKey(), keys[count]);
                Assert.AreSame(enumerator.GetValue(), values[count]);

                count++;
            }
        }

        Assert.AreEqual(keys.Length, count);

        foreach (object key in keys)
        {
            Assert.IsTrue(cwt.Remove(key));
        }

        using (ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Assert.Fail();
            }
        }

        GC.KeepAlive(keys);
        GC.KeepAlive(values);
    }

    [TestMethod]
    public void GetEnumerator_Empty_ReturnsEmptyEnumerator()
    {
        ConditionalWeakTable2<object, object> cwt = new();

        using ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator();

        while (enumerator.MoveNext())
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void GetEnumerator_AddedAndRemovedItems_AppropriatelyShowUpInEnumeration()
    {
        ConditionalWeakTable2<object, object> cwt = new();

        object key1 = new();
        object value1 = new();

        for (int i = 0; i < 20; i++)
        {
            _ = cwt.GetValue(key1, _ => value1);

            int count = 0;

            using (ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }

            Assert.AreEqual(1, count);

            count = 0;

            KeyValuePair<object, object>? first = null;

            using (ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (first is not null)
                    {
                        Assert.Fail();
                    }

                    first = new KeyValuePair<object, object>(enumerator.GetKey(), enumerator.GetValue());

                    if (count > 0)
                    {
                        Assert.Fail();
                    }

                    count++;
                }
            }

            Assert.AreEqual(new KeyValuePair<object, object>(key1, value1), first);

            Assert.IsTrue(cwt.Remove(key1));

            count = 0;

            using (ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }

            Assert.AreEqual(0, count);
        }

        GC.KeepAlive(key1);
        GC.KeepAlive(value1);
    }

    [TestMethod]
    public void GetEnumerator_CollectedItemsNotEnumerated()
    {
        ConditionalWeakTable2<object, object> cwt = new();

        using ConditionalWeakTable2<object, object>.Enumerator enumerator1 = cwt.GetEnumerator();

        static void addItem(ConditionalWeakTable2<object, object> t) => t.GetValue(new object(), _ => new object());

        for (int i = 0; i < 10; i++)
        {
            addItem(cwt);
        }

        GC.Collect();

        int count = 0;

        using (ConditionalWeakTable2<object, object>.Enumerator enumerator2 = cwt.GetEnumerator())
        {
            while (enumerator2.MoveNext())
            {
                count++;
            }
        }

        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void GetEnumerator_MultipleEnumeratorsReturnSameResults()
    {
        ConditionalWeakTable2<object, object> cwt = new();

        object[] keys = Enumerable.Range(0, 33).Select(_ => new object()).ToArray();
        object[] values = Enumerable.Range(0, keys.Length).Select(_ => new object()).ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            _ = cwt.GetValue(keys[i], _ => values[i]);
        }

        using (ConditionalWeakTable2<object, object>.Enumerator enumerator1 = cwt.GetEnumerator())
        using (ConditionalWeakTable2<object, object>.Enumerator enumerator2 = cwt.GetEnumerator())
        {
            while (enumerator1.MoveNext())
            {
                Assert.IsTrue(enumerator2.MoveNext());
                Assert.AreEqual(enumerator1.GetKey(), enumerator2.GetKey());
                Assert.AreEqual(enumerator1.GetValue(), enumerator2.GetValue());
            }

            Assert.IsFalse(enumerator2.MoveNext());
        }

        GC.KeepAlive(keys);
        GC.KeepAlive(values);
    }

    [TestMethod]
    public void GetEnumerator_RemovedItems_RemovedFromResults()
    {
        ConditionalWeakTable2<object, object> cwt = new();

        object[] keys = Enumerable.Range(0, 33).Select(_ => new object()).ToArray();
        object[] values = Enumerable.Range(0, keys.Length).Select(_ => new object()).ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            _ = cwt.GetValue(keys[i], _ => values[i]);
        }

        int count;

        for (int i = 0; i < keys.Length; i++)
        {
            count = 0;

            using (ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }

            Assert.AreEqual(keys.Length - i, count);

            List<KeyValuePair<object, object>> pairs = new();

            using (ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    pairs.Add(new KeyValuePair<object, object>(enumerator.GetKey(), enumerator.GetValue()));
                }
            }

            CollectionAssert.AreEqual(
                Enumerable.Range(i, keys.Length - i).Select(j => new KeyValuePair<object, object>(keys[j], values[j])).ToArray(),
                pairs);

            Assert.IsTrue(cwt.Remove(keys[i]));
        }

        count = 0;

        using (ConditionalWeakTable2<object, object>.Enumerator enumerator = cwt.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                count++;
            }
        }

        Assert.AreEqual(0, count);

        GC.KeepAlive(keys);
        GC.KeepAlive(values);
    }

    // These tests are skipped as enumeration is only ever done under a lock, so
    // there is no need to test for additions/removals while an enumerator is alive.
    //
    // public static void GetEnumerator_ItemsAddedAfterGetEnumeratorNotIncluded();
    // public void GetEnumerator_ItemsRemovedAfterGetEnumeratorNotIncluded();
    // public void GetEnumerator_ItemsClearedAfterGetEnumeratorNotIncluded();
    // public void GetEnumerator_Current_ThrowsOnInvalidUse();
}

#endif

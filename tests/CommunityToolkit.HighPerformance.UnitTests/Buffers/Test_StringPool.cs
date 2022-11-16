// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Buffers;

[TestClass]
public class Test_StringPool
{
    [TestMethod]
    [DataRow(44, 4, 16, 64)]
    [DataRow(76, 8, 16, 128)]
    [DataRow(128, 8, 16, 128)]
    [DataRow(179, 8, 32, 256)]
    [DataRow(366, 16, 32, 512)]
    [DataRow(512, 16, 32, 512)]
    [DataRow(890, 16, 64, 1024)]
    [DataRow(1280, 32, 64, 2048)]
    [DataRow(2445, 32, 128, 4096)]
    [DataRow(5000, 64, 128, 8192)]
    [DataRow(8000, 64, 128, 8192)]
    [DataRow(12442, 64, 256, 16384)]
    [DataRow(234000, 256, 1024, 262144)]
    public void Test_StringPool_Cctor_Ok(int minimumSize, int x, int y, int size)
    {
        StringPool? pool = new(minimumSize);

        Assert.AreEqual(size, pool.Size);

        Array maps = (Array)typeof(StringPool).GetField("maps", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(pool)!;

        Assert.AreEqual(x, maps.Length);

        Type bucketType = Type.GetType("CommunityToolkit.HighPerformance.Buffers.StringPool+FixedSizePriorityMap, CommunityToolkit.HighPerformance")!;

        int[] buckets = (int[])bucketType.GetField("buckets", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(maps.GetValue(0))!;

        Assert.AreEqual(y, buckets.Length);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-3248234)]
    [DataRow(int.MinValue)]
    public void Test_StringPool_Cctor_Fail(int size)
    {
        try
        {
            StringPool? pool = new(size);

            Assert.Fail();
        }
        catch (ArgumentOutOfRangeException e)
        {
            ConstructorInfo? cctor = typeof(StringPool).GetConstructor(new[] { typeof(int) })!;

            Assert.AreEqual(cctor.GetParameters()[0].Name, e.ParamName);
        }
    }

    [TestMethod]
    public void Test_StringPool_Add_Empty()
    {
        StringPool.Shared.Add(string.Empty);

        bool found = StringPool.Shared.TryGet(ReadOnlySpan<char>.Empty, out string? text);

        Assert.IsTrue(found);
        Assert.AreSame(string.Empty, text);
    }

    [TestMethod]
    public void Test_StringPool_Add_Single()
    {
        StringPool? pool = new();

        string hello = nameof(hello);

        Assert.IsFalse(pool.TryGet(hello.AsSpan(), out _));

        pool.Add(hello);

        Assert.IsTrue(pool.TryGet(hello.AsSpan(), out string? hello2));

        Assert.AreSame(hello, hello2);
    }

    [TestMethod]
    public void Test_StringPool_Add_Misc()
    {
        StringPool? pool = new();

        string hello = nameof(hello);
        string helloworld = nameof(helloworld);
        string dotnetCommunityToolkit = nameof(dotnetCommunityToolkit);

        Assert.IsFalse(pool.TryGet(hello.AsSpan(), out _));
        Assert.IsFalse(pool.TryGet(helloworld.AsSpan(), out _));
        Assert.IsFalse(pool.TryGet(dotnetCommunityToolkit.AsSpan(), out _));

        pool.Add(hello);
        pool.Add(helloworld);
        pool.Add(dotnetCommunityToolkit);

        Assert.IsTrue(pool.TryGet(hello.AsSpan(), out string? hello2));
        Assert.IsTrue(pool.TryGet(helloworld.AsSpan(), out string? world2));
        Assert.IsTrue(pool.TryGet(dotnetCommunityToolkit.AsSpan(), out string? windowsCommunityToolkit2));

        Assert.AreSame(hello, hello2);
        Assert.AreSame(helloworld, world2);
        Assert.AreSame(dotnetCommunityToolkit, windowsCommunityToolkit2);
    }

    [TestMethod]
    public void Test_StringPool_Add_Overwrite()
    {
        StringPool? pool = new();

        DateTime today = DateTime.Today;

        string? text1 = ToStringNoInlining(today);

        pool.Add(text1);

        Assert.IsTrue(pool.TryGet(text1.AsSpan(), out string? result));

        Assert.AreSame(text1, result);

        string? text2 = ToStringNoInlining(today);

        pool.Add(text2);

        Assert.IsTrue(pool.TryGet(text2.AsSpan(), out result));

        Assert.AreNotSame(text1, result);
        Assert.AreSame(text2, result);
    }

    // Separate method just to ensure the JIT can't optimize things away
    // and make the test fail because different string instances are interned
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string ToStringNoInlining(object obj)
    {
        return obj.ToString()!;
    }

    [TestMethod]
    public void Test_StringPool_GetOrAdd_String_Empty()
    {
        string empty = StringPool.Shared.GetOrAdd(string.Empty);

        Assert.AreSame(string.Empty, empty);
    }

    [TestMethod]
    public void Test_StringPool_GetOrAdd_String_Misc()
    {
        StringPool? pool = new();

        string helloworld = nameof(helloworld);

        string cached = pool.GetOrAdd(helloworld);

        Assert.AreSame(helloworld, cached);

        Span<char> span = stackalloc char[helloworld.Length];

        helloworld.AsSpan().CopyTo(span);

        string helloworld2 = span.ToString();

        cached = pool.GetOrAdd(helloworld2);

        Assert.AreSame(helloworld, cached);

        cached = pool.GetOrAdd(new string(helloworld.ToCharArray()));

        Assert.AreSame(helloworld, cached);
    }

    [TestMethod]
    public void Test_StringPool_GetOrAdd_ReadOnlySpan_Empty()
    {
        string empty = StringPool.Shared.GetOrAdd(ReadOnlySpan<char>.Empty);

        Assert.AreSame(string.Empty, empty);
    }

    [TestMethod]
    public void Test_StringPool_GetOrAdd_ReadOnlySpan_Misc()
    {
        StringPool? pool = new();

        string hello = pool.GetOrAdd(nameof(hello).AsSpan());
        string helloworld = pool.GetOrAdd(nameof(helloworld).AsSpan());
        string dotnetCommunityToolkit = pool.GetOrAdd(nameof(dotnetCommunityToolkit).AsSpan());

        Assert.AreEqual(nameof(hello), hello);
        Assert.AreEqual(nameof(helloworld), helloworld);
        Assert.AreEqual(nameof(dotnetCommunityToolkit), dotnetCommunityToolkit);

        Assert.AreSame(hello, pool.GetOrAdd(hello.AsSpan()));
        Assert.AreSame(helloworld, pool.GetOrAdd(helloworld.AsSpan()));
        Assert.AreSame(dotnetCommunityToolkit, pool.GetOrAdd(dotnetCommunityToolkit.AsSpan()));

        pool.Reset();

        Assert.AreEqual(nameof(hello), hello);
        Assert.AreEqual(nameof(helloworld), helloworld);
        Assert.AreEqual(nameof(dotnetCommunityToolkit), dotnetCommunityToolkit);

#if NET6_0_OR_GREATER

        // .NET Framework reuses strings in a way that makes these tests fail.
        // The actual underlying APIs are still working as expected though.
        Assert.AreNotSame(hello, pool.GetOrAdd(hello.AsSpan()));
        Assert.AreNotSame(helloworld, pool.GetOrAdd(helloworld.AsSpan()));
        Assert.AreNotSame(dotnetCommunityToolkit, pool.GetOrAdd(dotnetCommunityToolkit.AsSpan()));
#endif
    }

    [TestMethod]
    public void Test_StringPool_GetOrAdd_Encoding_Empty()
    {
        string empty = StringPool.Shared.GetOrAdd(ReadOnlySpan<byte>.Empty, Encoding.ASCII);

        Assert.AreSame(string.Empty, empty);
    }

    [TestMethod]
    public void Test_StringPool_GetOrAdd_Encoding_Misc()
    {
        StringPool? pool = new();

        string helloworld = nameof(helloworld);

        pool.Add(helloworld);

        Span<byte> span = Encoding.UTF8.GetBytes(nameof(helloworld));

        string helloworld2 = pool.GetOrAdd(span, Encoding.UTF8);

        Assert.AreSame(helloworld, helloworld2);

        string windowsCommunityToolkit = nameof(windowsCommunityToolkit);

        Span<byte> span2 = Encoding.ASCII.GetBytes(windowsCommunityToolkit);

        string dotnetCommunityToolkit2 = pool.GetOrAdd(span2, Encoding.ASCII);
        string dotnetCommunityToolkit3 = pool.GetOrAdd(windowsCommunityToolkit);

        Assert.AreSame(dotnetCommunityToolkit2, dotnetCommunityToolkit3);
    }

    [TestMethod]
    public void Test_StringPool_GetOrAdd_Overflow()
    {
        StringPool? pool = new(32);

        // Fill the pool
        for (int i = 0; i < 4096; i++)
        {
            _ = pool.GetOrAdd(i.ToString());
        }

        // Get the buckets
        Array maps = (Array)typeof(StringPool).GetField("maps", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(pool)!;

        Type bucketType = Type.GetType("CommunityToolkit.HighPerformance.Buffers.StringPool+FixedSizePriorityMap, CommunityToolkit.HighPerformance")!;
        FieldInfo timestampInfo = bucketType.GetField("timestamp", BindingFlags.Instance | BindingFlags.NonPublic)!;

        // Force the timestamp to be the maximum value, or the test would take too long
        for (int i = 0; i < maps.LongLength; i++)
        {
            object map = maps.GetValue(i)!;

            timestampInfo.SetValue(map, uint.MaxValue);

            maps.SetValue(map, i);
        }

        // Force an overflow
        string text = "Hello world";

        _ = pool.GetOrAdd(text);

        Type heapEntryType = Type.GetType("CommunityToolkit.HighPerformance.Buffers.StringPool+FixedSizePriorityMap+HeapEntry, CommunityToolkit.HighPerformance")!;

        foreach (object? map in maps)
        {
            // Get the heap for each bucket
            Array heapEntries = (Array)bucketType.GetField("heapEntries", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(map)!;
            FieldInfo fieldInfo = heapEntryType.GetField("Timestamp")!;

            // Extract the array with the timestamps in the heap nodes
            uint[] array = heapEntries.Cast<object>().Select(entry => (uint)fieldInfo.GetValue(entry)!).ToArray();

            static bool IsMinHeap(uint[] array)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    int left = (i * 2) + 1;
                    int right = (i * 2) + 2;

                    if ((left < array.Length &&
                         array[left] <= array[i]) ||
                        (right < array.Length &&
                         array[right] <= array[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            // Verify that the current heap is indeed valid after the overflow
            Assert.IsTrue(IsMinHeap(array));
        }
    }
}

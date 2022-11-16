// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;

namespace CommunityToolkit.HighPerformance.UnitTests.Helpers;

[TestClass]
public partial class Test_ParallelHelper
{
    /// <summary>
    /// Gets the list of counts to test the For (1D) extensions for
    /// </summary>
    private static ReadOnlySpan<int> TestForCounts => new[] { 0, 1, 7, 128, 255, 256, short.MaxValue, short.MaxValue + 1, 123_938, 1_678_922, 71_890_819 };

    [TestMethod]
    public unsafe void Test_ParallelHelper_ForWithIndices()
    {
        foreach (int count in TestForCounts)
        {
            using UnmanagedSpanOwner<int> data = new(count);

            data.GetSpan().Clear();

            ParallelHelper.For(0, data.Length, new Assigner(data.Length, data.Ptr));

            foreach (HighPerformance.Enumerables.SpanEnumerable<int>.Item item in data.GetSpan().Enumerate())
            {
                if (item.Index != item.Value)
                {
                    Assert.Fail($"Invalid item at position {item.Index}, value was {item.Value}");
                }
            }
        }
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_ParallelHelper_ForInvalidRange_FromEnd()
    {
        ParallelHelper.For<Assigner>(..^1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test_ParallelHelper_ForInvalidRange_RangeAll()
    {
        ParallelHelper.For<Assigner>(..);
    }

    [TestMethod]
    public unsafe void Test_ParallelHelper_ForWithRanges()
    {
        foreach (int count in TestForCounts)
        {
            using UnmanagedSpanOwner<int> data = new(count);

            data.GetSpan().Clear();

            ParallelHelper.For(..data.Length, new Assigner(data.Length, data.Ptr));

            foreach (HighPerformance.Enumerables.SpanEnumerable<int>.Item item in data.GetSpan().Enumerate())
            {
                if (item.Index != item.Value)
                {
                    Assert.Fail($"Invalid item at position {item.Index}, value was {item.Value}");
                }
            }
        }
    }
#endif

    /// <summary>
    /// A type implementing <see cref="IAction"/> to initialize an array
    /// </summary>
    private readonly unsafe struct Assigner : IAction
    {
        private readonly int length;
        private readonly int* ptr;

        public Assigner(int length, int* ptr)
        {
            this.length = length;
            this.ptr = ptr;
        }

        /// <inheritdoc/>
        public void Invoke(int i)
        {
            if ((uint)i >= (uint)this.length)
            {
                throw new IndexOutOfRangeException($"The target position was out of range, was {i} and should've been in [0, {this.length})");
            }

            if (this.ptr[i] != 0)
            {
                throw new InvalidOperationException($"Invalid target position {i}, was {this.ptr[i]} instead of 0");
            }

            this.ptr[i] = i;
        }
    }
}

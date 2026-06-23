// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public void Test_ParallelHelper_ForInvalidRange_FromEnd()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => ParallelHelper.For<Assigner>(..^1));
    }

    [TestMethod]
    public void Test_ParallelHelper_ForInvalidRange_RangeAll()
    {
        _ = Assert.ThrowsExactly<ArgumentException>(() => ParallelHelper.For<Assigner>(..));
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

    [TestMethod]
    public void Test_ParallelHelper_ForLargeNegativeRange_DoesNotOverflow()
    {
        // Regression test for https://github.com/CommunityToolkit/dotnet/issues/1188.
        // [int.MinValue, 0) spans 2^31 elements. The range size used to be computed as
        // 'Math.Abs(start - end)', and 'Math.Abs(int.MinValue)' throws OverflowException
        // before any work runs. The action throws on its first invocation so the loop
        // exits immediately instead of iterating two billion times.
        Exception? caught = null;

        try
        {
            ParallelHelper.For(int.MinValue, 0, default(ThrowingAction), 1);
        }
        catch (Exception e)
        {
            caught = e;
        }

        Assert.IsNotNull(caught, "Expected the action to run and throw.");

        IEnumerable<Exception> leaves = caught is AggregateException aggregate
            ? aggregate.Flatten().InnerExceptions
            : new[] { caught };

        Assert.IsFalse(leaves.Any(static e => e is OverflowException), "The range size overflowed.");
        Assert.IsTrue(leaves.Any(static e => e is ThrowingActionException), "The action did not run.");
    }

    [TestMethod]
    public void Test_ParallelHelper_ForFullRange_IsParallelized()
    {
        // Regression test for https://github.com/CommunityToolkit/dotnet/issues/1188.
        // For [int.MinValue, int.MaxValue) the range size used to overflow to 1 ('start - end'
        // wraps around), so the helper silently ran the whole loop on the calling thread
        // instead of parallelizing it. When the work is parallelized, Parallel.For surfaces a
        // failing action as an AggregateException; the buggy single-threaded path would throw
        // the action's exception directly. The action throws on its first invocation so each
        // batch exits immediately instead of iterating billions of times.
        if (Environment.ProcessorCount < 2)
        {
            Assert.Inconclusive("Parallel dispatch requires more than one processor.");
        }

        Exception? caught = null;

        try
        {
            ParallelHelper.For(int.MinValue, int.MaxValue, default(ThrowingAction), 1);
        }
        catch (Exception e)
        {
            caught = e;
        }

        Assert.IsInstanceOfType(caught, typeof(AggregateException), "The work was not parallelized.");
        Assert.IsTrue(
            ((AggregateException)caught!).Flatten().InnerExceptions.All(static e => e is ThrowingActionException),
            "Unexpected exception type.");
    }

    /// <summary>
    /// An exception type thrown by <see cref="ThrowingAction"/>.
    /// </summary>
    private sealed class ThrowingActionException : Exception
    {
    }

    /// <summary>
    /// A type implementing <see cref="IAction"/> that throws on its first invocation.
    /// </summary>
    private readonly struct ThrowingAction : IAction
    {
        /// <inheritdoc/>
        public void Invoke(int i)
        {
            throw new ThrowingActionException();
        }
    }

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

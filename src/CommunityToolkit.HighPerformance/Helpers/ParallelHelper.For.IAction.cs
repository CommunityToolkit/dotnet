// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommunityToolkit.HighPerformance.Helpers;

/// <summary>
/// Helpers to work with parallel code in a highly optimized manner.
/// </summary>
public static partial class ParallelHelper
{
#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Executes a specified action in an optimized parallel loop.
    /// </summary>
    /// <typeparam name="TAction">The type of action (implementing <see cref="IAction"/>) to invoke for each iteration index.</typeparam>
    /// <param name="range">The iteration range.</param>
    /// <remarks>None of the bounds of <paramref name="range"/> can start from an end.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For<TAction>(Range range)
        where TAction : struct, IAction
    {
        For(range, default(TAction), 1);
    }

    /// <summary>
    /// Executes a specified action in an optimized parallel loop.
    /// </summary>
    /// <typeparam name="TAction">The type of action (implementing <see cref="IAction"/>) to invoke for each iteration index.</typeparam>
    /// <param name="range">The iteration range.</param>
    /// <param name="minimumActionsPerThread">
    /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
    /// should be parallelized, or to a greater number if each individual invocation is fast
    /// enough that it is more efficient to set a lower bound per each running thread.
    /// </param>
    /// <remarks>None of the bounds of <paramref name="range"/> can start from an end.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For<TAction>(Range range, int minimumActionsPerThread)
        where TAction : struct, IAction
    {
        For(range, default(TAction), minimumActionsPerThread);
    }

    /// <summary>
    /// Executes a specified action in an optimized parallel loop.
    /// </summary>
    /// <typeparam name="TAction">The type of action (implementing <see cref="IAction"/>) to invoke for each iteration index.</typeparam>
    /// <param name="range">The iteration range.</param>
    /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
    /// <remarks>None of the bounds of <paramref name="range"/> can start from an end.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For<TAction>(Range range, in TAction action)
        where TAction : struct, IAction
    {
        For(range, action, 1);
    }

    /// <summary>
    /// Executes a specified action in an optimized parallel loop.
    /// </summary>
    /// <typeparam name="TAction">The type of action (implementing <see cref="IAction"/>) to invoke for each iteration index.</typeparam>
    /// <param name="range">The iteration range.</param>
    /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
    /// <param name="minimumActionsPerThread">
    /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
    /// should be parallelized, or to a greater number if each individual invocation is fast
    /// enough that it is more efficient to set a lower bound per each running thread.
    /// </param>
    /// <remarks>None of the bounds of <paramref name="range"/> can start from an end.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For<TAction>(Range range, in TAction action, int minimumActionsPerThread)
        where TAction : struct, IAction
    {
        if (range.Start.IsFromEnd || range.End.IsFromEnd)
        {
            ThrowArgumentExceptionForRangeIndexFromEnd(nameof(range));
        }

        int start = range.Start.Value;
        int end = range.End.Value;

        For(start, end, action, minimumActionsPerThread);
    }
#endif

    /// <summary>
    /// Executes a specified action in an optimized parallel loop.
    /// </summary>
    /// <typeparam name="TAction">The type of action (implementing <see cref="IAction"/>) to invoke for each iteration index.</typeparam>
    /// <param name="start">The starting iteration index.</param>
    /// <param name="end">The final iteration index (exclusive).</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For<TAction>(int start, int end)
        where TAction : struct, IAction
    {
        For(start, end, default(TAction), 1);
    }

    /// <summary>
    /// Executes a specified action in an optimized parallel loop.
    /// </summary>
    /// <typeparam name="TAction">The type of action (implementing <see cref="IAction"/>) to invoke for each iteration index.</typeparam>
    /// <param name="start">The starting iteration index.</param>
    /// <param name="end">The final iteration index (exclusive).</param>
    /// <param name="minimumActionsPerThread">
    /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
    /// should be parallelized, or to a greater number if each individual invocation is fast
    /// enough that it is more efficient to set a lower bound per each running thread.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For<TAction>(int start, int end, int minimumActionsPerThread)
        where TAction : struct, IAction
    {
        For(start, end, default(TAction), minimumActionsPerThread);
    }

    /// <summary>
    /// Executes a specified action in an optimized parallel loop.
    /// </summary>
    /// <typeparam name="TAction">The type of action (implementing <see cref="IAction"/>) to invoke for each iteration index.</typeparam>
    /// <param name="start">The starting iteration index.</param>
    /// <param name="end">The final iteration index (exclusive).</param>
    /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For<TAction>(int start, int end, in TAction action)
        where TAction : struct, IAction
    {
        For(start, end, action, 1);
    }

    /// <summary>
    /// Executes a specified action in an optimized parallel loop.
    /// </summary>
    /// <typeparam name="TAction">The type of action (implementing <see cref="IAction"/>) to invoke for each iteration index.</typeparam>
    /// <param name="start">The starting iteration index.</param>
    /// <param name="end">The final iteration index (exclusive).</param>
    /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
    /// <param name="minimumActionsPerThread">
    /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
    /// should be parallelized, or to a greater number if each individual invocation is fast
    /// enough that it is more efficient to set a lower bound per each running thread.
    /// </param>
    public static void For<TAction>(int start, int end, in TAction action, int minimumActionsPerThread)
        where TAction : struct, IAction
    {
        if (minimumActionsPerThread <= 0)
        {
            ThrowArgumentOutOfRangeExceptionForInvalidMinimumActionsPerThread();
        }

        if (start > end)
        {
            ThrowArgumentOutOfRangeExceptionForStartGreaterThanEnd();
        }

        if (start == end)
        {
            return;
        }

        // The number of elements in [start, end) can exceed int.MaxValue (for example the
        // full [int.MinValue, int.MaxValue) range), so the range size is computed using 64-bit
        // arithmetic. Using int here would overflow: 'start - end' could wrap around to a small
        // value (silently degrading to a single-threaded run), and 'Math.Abs(int.MinValue)'
        // throws OverflowException. The earlier 'start > end' check guarantees count >= 1.
        long count = (long)end - start;
        long maxBatches = 1 + ((count - 1) / minimumActionsPerThread);
        int cores = Environment.ProcessorCount;
        int numBatches = (int)Math.Min(maxBatches, (long)cores);

        // Skip the parallel invocation when a single batch is needed
        if (numBatches == 1)
        {
            for (int i = start; i < end; i++)
            {
                Unsafe.AsRef(in action).Invoke(i);
            }

            return;
        }

        long batchSize = 1 + ((count - 1) / numBatches);

        ActionInvoker<TAction> actionInvoker = new(start, end, batchSize, action);

        // Run the batched operations in parallel
        _ = Parallel.For(
            0,
            numBatches,
            new ParallelOptions { MaxDegreeOfParallelism = numBatches },
            actionInvoker.Invoke);
    }

    // Wrapping struct acting as explicit closure to execute the processing batches
    private readonly struct ActionInvoker<TAction>
        where TAction : struct, IAction
    {
        private readonly int start;
        private readonly int end;
        private readonly long batchSize;
        private readonly TAction action;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ActionInvoker(
            int start,
            int end,
            long batchSize,
            in TAction action)
        {
            this.start = start;
            this.end = end;
            this.batchSize = batchSize;
            this.action = action;
        }

        /// <summary>
        /// Processes the batch of actions at a specified index
        /// </summary>
        /// <param name="i">The index of the batch to process</param>
        public void Invoke(int i)
        {
            // The batch offset is computed with 64-bit arithmetic so it doesn't overflow for
            // large ranges. 'low' and 'stop' are always within [start, end], which fits in an
            // int, so the narrowing casts below are safe.
            long offset = (long)i * this.batchSize;
            long low = this.start + offset;
            long high = low + this.batchSize;
            int stop = (int)Math.Min(high, (long)this.end);

            for (int j = (int)low; j < stop; j++)
            {
                Unsafe.AsRef(in this.action).Invoke(j);
            }
        }
    }
}

/// <summary>
/// A contract for actions being executed with an input index.
/// </summary>
/// <remarks>If the <see cref="Invoke"/> method is small enough, it is highly recommended to mark it with <see cref="MethodImplOptions.AggressiveInlining"/>.</remarks>
public interface IAction
{
    /// <summary>
    /// Executes the action associated with a specific index.
    /// </summary>
    /// <param name="i">The current index for the action to execute.</param>
    void Invoke(int i);
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace CommunityToolkit.Mvvm.UnitTests.Helpers;

/// <summary>
/// A helper class to validate scenarios related to <see cref="TaskScheduler"/>.
/// </summary>
internal static class TaskSchedulerTestHelper
{
    /// <summary>
    /// A custom <see cref="Delegate"/> for callbacks to <see cref="IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback)"/>.
    /// </summary>
    /// <param name="throwAction">An <see cref="Action"/> instance that throws a test exception to track.</param>
    /// <param name="completeAction">An <see cref="Action"/> that signals whenever the test has completed.</param>
    public delegate void TestCallback(Action throwAction, Action completeAction);

    /// <summary>
    /// Checks whether a given test exception is correctly bubbled up to <see cref="TaskScheduler.UnobservedTaskException"/>.
    /// </summary>
    /// <param name="callback">The <see cref="TestCallback"/> instance to use to run the test.</param>
    /// <returns>Whether or not the test exception was correctly bubbled up to <see cref="TaskScheduler.UnobservedTaskException"/>.</returns>
    public static async Task<bool> IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback callback)
    {
        TaskCompletionSource<object?> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        string guid = Guid.NewGuid().ToString();
        bool exceptionFound = false;

        void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();

            foreach (Exception exception in e.Exception!.InnerExceptions)
            {
                if (exception is TestException testException &&
                    testException.Message == guid)
                {
                    exceptionFound = true;

                    return;
                }
            }
        }

        EventHandler<UnobservedTaskExceptionEventArgs> handler = TaskScheduler_UnobservedTaskException;

        TaskScheduler.UnobservedTaskException += handler;

        try
        {
            // Enqueue a continuation that will throw and ignore the returned task. This has
            // to be a separate method to ensure the returned task isn't kept alive for longer.
            callback(
                () => throw new TestException(guid),
                () => tcs.SetResult(null));

            // Await for the continuation to actually run
            _ = await tcs.Task;

            // Wait for some additional time to ensure the exception is propagated. This is a bit counterintuitive, but the delay is
            // not actually for the event to be raised, but to ensure the task that is throwing has had time to be scheduled and fail.
            // The event is raised only when the exception wrapper inside that task is collected and its finalizer has run (that's where
            // the logic to raise the event is executed), which is why we're then calling GC.Collect() and GC.WaitForPendingFinalizers().
            // That is, we can't use a task completion source from that event, because that event is only guaranteed to actually be raised
            // when the finalizer for that task run, which is why we're calling the GC after the delay here.
            await Task.Delay(200);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        finally
        {
            TaskScheduler.UnobservedTaskException -= handler;
        }

        return exceptionFound;
    }

    /// <summary>
    /// A custom exception to support <see cref="IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback)"/>.
    /// </summary>
    private sealed class TestException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="TestException"/> instance with the specified parameters.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public TestException(string message)
            : base(message)
        {
        }
    }
}

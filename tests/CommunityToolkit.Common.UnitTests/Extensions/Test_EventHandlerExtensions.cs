// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using CommunityToolkit.Common.Deferred;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Common.UnitTests.Extensions;

[TestClass]
public class Test_EventHandlerExtensions
{
    [TestMethod]
    public void Test_EventHandlerExtensions_GettingDeferralCausesAwait()
    {
        TaskCompletionSource<bool>? tsc = new();

        TestClass? testClass = new();

        testClass.TestEvent += async (s, e) =>
        {
            EventDeferral? deferral = e.GetDeferral();

            _ = await tsc.Task;

            deferral.Complete();
        };

        Task? handlersTask = testClass.RaiseTestEvent();

        Assert.IsFalse(handlersTask.IsCompleted);

        tsc.SetResult(true);

        Assert.IsTrue(handlersTask.IsCompleted);
    }

    [TestMethod]
    public void Test_EventHandlerExtensions_NotGettingDeferralCausesNoAwait()
    {
        TaskCompletionSource<bool>? tsc = new();

        TestClass? testClass = new();

        testClass.TestEvent += async (s, e) =>
        {
            _ = await tsc.Task;
        };

        Task? handlersTask = testClass.RaiseTestEvent();

        Assert.IsTrue(handlersTask.IsCompleted);

        tsc.SetResult(true);
    }

    [TestMethod]
    public void Test_EventHandlerExtensions_UsingDeferralCausesAwait()
    {
        TaskCompletionSource<bool>? tsc = new();

        TestClass? testClass = new();

        testClass.TestEvent += async (s, e) =>
        {
            using (e.GetDeferral())
            {
                _ = await tsc.Task;
            }
        };

        Task? handlersTask = testClass.RaiseTestEvent();

        Assert.IsFalse(handlersTask.IsCompleted);

        tsc.SetResult(true);

        Assert.IsTrue(handlersTask.IsCompleted);
    }

    [TestMethod]
    [DataRow(0, 1)]
    [DataRow(1, 0)]
    public void Test_EventHandlerExtensions_MultipleHandlersCauseAwait(int firstToReleaseDeferral, int lastToReleaseDeferral)
    {
        TaskCompletionSource<bool>[]? tsc = new[]
        {
            new TaskCompletionSource<bool>(),
            new TaskCompletionSource<bool>()
        };

        TestClass? testClass = new();

        testClass.TestEvent += async (s, e) =>
        {
            EventDeferral? deferral = e.GetDeferral();

            _ = await tsc[0].Task;

            deferral.Complete();
        };

        testClass.TestEvent += async (s, e) =>
        {
            EventDeferral? deferral = e.GetDeferral();

            _ = await tsc[1].Task;

            deferral.Complete();
        };

        Task? handlersTask = testClass.RaiseTestEvent();

        Assert.IsFalse(handlersTask.IsCompleted);

        tsc[firstToReleaseDeferral].SetResult(true);

        Assert.IsFalse(handlersTask.IsCompleted);

        tsc[lastToReleaseDeferral].SetResult(true);

        Assert.IsTrue(handlersTask.IsCompleted);
    }

    private class TestClass
    {
        public event EventHandler<DeferredEventArgs>? TestEvent;

        public Task RaiseTestEvent() => TestEvent.InvokeAsync(this, new DeferredEventArgs());
    }
}

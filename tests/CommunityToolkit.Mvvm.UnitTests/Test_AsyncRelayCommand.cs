// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public class Test_AsyncRelayCommand
{
    [TestMethod]
    public async Task Test_AsyncRelayCommand_AlwaysEnabled()
    {
        int ticks = 0;

        AsyncRelayCommand? command = new(async () =>
        {
            await Task.Delay(1000);
            ticks++;
            await Task.Delay(1000);
        });

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        (object?, EventArgs?) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        command.NotifyCanExecuteChanged();

        Assert.AreSame(args.Item1, command);
        Assert.AreSame(args.Item2, EventArgs.Empty);

        Assert.IsNull(command.ExecutionTask);
        Assert.IsFalse(command.IsRunning);

        Task task = command.ExecuteAsync(null);

        Assert.IsNotNull(command.ExecutionTask);
        Assert.AreSame(command.ExecutionTask, task);
        Assert.IsTrue(command.IsRunning);

        await task;

        Assert.IsFalse(command.IsRunning);

        Assert.AreEqual(ticks, 1);

        command.Execute(new object());

        await command.ExecutionTask!;

        Assert.AreEqual(ticks, 2);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_WithCanExecuteFunctionTrue()
    {
        int ticks = 0;

        AsyncRelayCommand? command = new(
            () =>
            {
                ticks++;
                return Task.CompletedTask;
            }, () => true);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        command.Execute(null);

        Assert.AreEqual(ticks, 1);

        command.Execute(new object());

        Assert.AreEqual(ticks, 2);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_WithCanExecuteFunctionFalse()
    {
        int ticks = 0;

        AsyncRelayCommand? command = new(
            () =>
            {
                ticks++;
                return Task.CompletedTask;
            }, () => false);

        Assert.IsFalse(command.CanExecute(null));
        Assert.IsFalse(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        command.Execute(null);

        // It is the caller's responsibility to ensure that CanExecute is true
        // before calling Execute. This check verifies the logic is still called.
        Assert.AreEqual(ticks, 1);

        command.Execute(new object());

        Assert.AreEqual(ticks, 2);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommand_WithCancellation()
    {
        TaskCompletionSource<object?> tcs = new();

        // We need to test the cancellation support here, so we use the overload with an input
        // parameter, which is a cancellation token. The token is the one that is internally managed
        // by the AsyncRelayCommand instance, and canceled when using IAsyncRelayCommand.Cancel().
        AsyncRelayCommand? command = new(token => tcs.Task);

        List<PropertyChangedEventArgs> args = new();

        command.PropertyChanged += (s, e) => args.Add(e);

        // We have no canExecute parameter, so the command can always be invoked
        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        // The command isn't running, so it can't be canceled yet
        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        // Start the command, which will return the token from our task completion source.
        // We can use that to easily keep the command running while we do our tests, and then
        // stop the processing by completing the source when we need (see below).
        command.Execute(null);

        // The command is running, so it can be canceled, as we used the token overload
        Assert.IsTrue(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        // Validate the various event args for all the properties that were updated when executing the command
        Assert.AreEqual(args.Count, 4);
        Assert.AreEqual(args[0].PropertyName, nameof(IAsyncRelayCommand.ExecutionTask));
        Assert.AreEqual(args[1].PropertyName, nameof(IAsyncRelayCommand.IsRunning));
        Assert.AreEqual(args[2].PropertyName, nameof(IAsyncRelayCommand.CanBeCanceled));
        Assert.AreEqual(args[3].PropertyName, nameof(IAsyncRelayCommand.IsCancellationRequested));

        command.Cancel();

        // Verify that these two properties raised notifications correctly when canceling the command too.
        // We need to ensure all command properties support notifications so that users can bind to them.
        Assert.AreEqual(args.Count, 6);
        Assert.AreEqual(args[4].PropertyName, nameof(IAsyncRelayCommand.CanBeCanceled));
        Assert.AreEqual(args[5].PropertyName, nameof(IAsyncRelayCommand.IsCancellationRequested));

        Assert.IsTrue(command.IsCancellationRequested);

        // Complete the source, which will mark the command as completed too (as it returned the same task)
        tcs.SetResult(null);

        await command.ExecutionTask!;

        // Verify that the command can no longer be canceled, and that the cancellation is
        // instead still true, as that's reset when executing a command and not on completion.
        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsTrue(command.IsCancellationRequested);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommand_AllowConcurrentExecutions_Enable()
    {
        int index = 0;
        TaskCompletionSource<object?>[] cancellationTokenSources = new TaskCompletionSource<object?>[]
        {
            new TaskCompletionSource<object?>(),
            new TaskCompletionSource<object?>()
        };

        AsyncRelayCommand? command = new(() => cancellationTokenSources[index++].Task, allowConcurrentExecutions: true);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        (object?, EventArgs?) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        command.NotifyCanExecuteChanged();

        Assert.AreSame(args.Item1, command);
        Assert.AreSame(args.Item2, EventArgs.Empty);

        Assert.IsNull(command.ExecutionTask);
        Assert.IsFalse(command.IsRunning);

        Task task = command.ExecuteAsync(null);

        Assert.IsNotNull(command.ExecutionTask);
        Assert.AreSame(command.ExecutionTask, task);
        Assert.AreSame(command.ExecutionTask, cancellationTokenSources[0].Task);
        Assert.IsTrue(command.IsRunning);

        // The command can still be executed now
        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        Task newTask = command.ExecuteAsync(null);

        // A new task was returned
        Assert.AreSame(command.ExecutionTask, newTask);
        Assert.AreSame(command.ExecutionTask, cancellationTokenSources[1].Task);

        cancellationTokenSources[0].SetResult(null);
        cancellationTokenSources[1].SetResult(null);

        _ = await Task.WhenAll(cancellationTokenSources[0].Task, cancellationTokenSources[1].Task);

        Assert.IsFalse(command.IsRunning);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommand_AllowConcurrentExecutions_Disabled()
    {
        await Test_AsyncRelayCommand_AllowConcurrentExecutions_TestLogic(static task => new(async () => await task, allowConcurrentExecutions: false));
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommand_AllowConcurrentExecutions_Default()
    {
        await Test_AsyncRelayCommand_AllowConcurrentExecutions_TestLogic(static task => new(async () => await task));
    }

    /// <summary>
    /// Shared logic for <see cref="Test_AsyncRelayCommand_AllowConcurrentExecutions_Disabled"/> and <see cref="Test_AsyncRelayCommand_AllowConcurrentExecutions_Default"/>.
    /// </summary>
    /// <param name="factory">A factory to create the <see cref="AsyncRelayCommand"/> instance to test.</param>
    private static async Task Test_AsyncRelayCommand_AllowConcurrentExecutions_TestLogic(Func<Task, AsyncRelayCommand> factory)
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand? command = factory(tcs.Task);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        (object?, EventArgs?) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        command.NotifyCanExecuteChanged();

        Assert.AreSame(args.Item1, command);
        Assert.AreSame(args.Item2, EventArgs.Empty);

        Assert.IsNull(command.ExecutionTask);
        Assert.IsFalse(command.IsRunning);

        Task task = command.ExecuteAsync(null);

        Assert.IsNotNull(command.ExecutionTask);
        Assert.AreSame(command.ExecutionTask, task);
        Assert.IsTrue(command.IsRunning);

        // The command can't be executed now, as there's a pending operation
        Assert.IsFalse(command.CanExecute(null));
        Assert.IsFalse(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        tcs.SetResult(null);

        await task;

        Assert.IsFalse(command.IsRunning);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommand_ThrowingTaskBubblesToUnobservedTaskException()
    {
        static async Task TestMethodAsync(Action action)
        {
            await Task.Delay(100);

            action();
        }

        async void TestCallback(Action throwAction, Action completeAction)
        {
            AsyncRelayCommand command = new(() => TestMethodAsync(throwAction));

            command.Execute(null);

            await Task.Delay(200);

            completeAction();
        }

        bool success = await TaskSchedulerTestHelper.IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback);

        Assert.IsTrue(success);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/108
    [TestMethod]
    public void Test_AsyncRelayCommand_ExecuteDoesNotRaiseCanExecuteChanged()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand command = new(() => tcs.Task, allowConcurrentExecutions: true);

        (object? Sender, EventArgs? Args) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        Assert.IsTrue(command.CanExecute(null));

        command.Execute(null);

        Assert.IsNull(args.Sender);
        Assert.IsNull(args.Args);

        Assert.IsTrue(command.CanExecute(null));

        tcs.SetResult(null);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_ExecuteWithoutConcurrencyRaisesCanExecuteChanged()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand command = new(() => tcs.Task, allowConcurrentExecutions: false);

        (object? Sender, EventArgs? Args) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        Assert.IsTrue(command.CanExecute(null));

        command.Execute(null);

        Assert.AreSame(command, args.Sender);
        Assert.AreSame(EventArgs.Empty, args.Args);

        Assert.IsFalse(command.CanExecute(null));

        tcs.SetResult(null);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_ExecuteDoesNotRaiseCanExecuteChanged_WithCancellation()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand command = new(token => tcs.Task, allowConcurrentExecutions: true);

        (object? Sender, EventArgs? Args) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        Assert.IsTrue(command.CanExecute(null));

        command.Execute(null);

        Assert.IsNull(args.Sender);
        Assert.IsNull(args.Args);

        Assert.IsTrue(command.CanExecute(null));

        tcs.SetResult(null);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_ExecuteWithoutConcurrencyRaisesCanExecuteChanged_WithToken()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand command = new(token => tcs.Task, allowConcurrentExecutions: false);

        (object? Sender, EventArgs? Args) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        Assert.IsTrue(command.CanExecute(null));

        command.Execute(null);

        Assert.AreSame(command, args.Sender);
        Assert.AreSame(EventArgs.Empty, args.Args);

        Assert.IsFalse(command.CanExecute(null));

        tcs.SetResult(null);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_GetCancelCommand_DisabledCommand()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand command = new(() => tcs.Task);

        ICommand cancelCommand = command.CreateCancelCommand();

        Assert.IsNotNull(cancelCommand);
        Assert.IsFalse(cancelCommand.CanExecute(null));

        // No-op
        cancelCommand.Execute(null);

        Assert.AreEqual("CommunityToolkit.Mvvm.Input.Internals.DisabledCommand", cancelCommand.GetType().ToString());

        ICommand cancelCommand2 = command.CreateCancelCommand();

        Assert.IsNotNull(cancelCommand2);
        Assert.IsFalse(cancelCommand2.CanExecute(null));

        Assert.AreSame(cancelCommand, cancelCommand2);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_GetCancelCommand_WithToken()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand command = new(token => tcs.Task);

        ICommand cancelCommand = command.CreateCancelCommand();

        Assert.IsNotNull(cancelCommand);
        Assert.IsFalse(cancelCommand.CanExecute(null));

        // No-op
        cancelCommand.Execute(null);

        Assert.AreEqual("CommunityToolkit.Mvvm.Input.Internals.CancelCommand", cancelCommand.GetType().ToString());

        List<(object? Sender, EventArgs Args)> cancelCommandCanExecuteChangedArgs = new();

        cancelCommand.CanExecuteChanged += (s, e) => cancelCommandCanExecuteChangedArgs.Add((s, e));

        command.Execute(null);

        Assert.AreEqual(1, cancelCommandCanExecuteChangedArgs.Count);
        Assert.AreSame(cancelCommand, cancelCommandCanExecuteChangedArgs[0].Sender);
        Assert.AreSame(EventArgs.Empty, cancelCommandCanExecuteChangedArgs[0].Args);

        Assert.IsTrue(cancelCommand.CanExecute(null));

        cancelCommand.Execute(null);

        Assert.IsFalse(cancelCommand.CanExecute(null));
        Assert.AreEqual(2, cancelCommandCanExecuteChangedArgs.Count);
        Assert.AreSame(cancelCommand, cancelCommandCanExecuteChangedArgs[1].Sender);
        Assert.AreSame(EventArgs.Empty, cancelCommandCanExecuteChangedArgs[1].Args);
        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsTrue(command.IsCancellationRequested);
    }
}

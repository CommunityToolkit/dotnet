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
using Nito.AsyncEx;

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

        Assert.AreSame(command, args.Item1);
        Assert.AreSame(EventArgs.Empty, args.Item2);

        Assert.IsNull(command.ExecutionTask);
        Assert.IsFalse(command.IsRunning);

        Task task = command.ExecuteAsync(null);

        Assert.IsNotNull(command.ExecutionTask);
        Assert.AreSame(task, command.ExecutionTask);
        Assert.IsTrue(command.IsRunning);

        await task;

        Assert.IsFalse(command.IsRunning);

        Assert.AreEqual(1, ticks);

        command.Execute(new object());

        await command.ExecutionTask!;

        Assert.AreEqual(2, ticks);
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

        Assert.AreEqual(1, ticks);

        command.Execute(new object());

        Assert.AreEqual(2, ticks);
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
        Assert.AreEqual(1, ticks);

        command.Execute(new object());

        Assert.AreEqual(2, ticks);
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
        Assert.AreEqual(4, args.Count);
        Assert.AreEqual(nameof(IAsyncRelayCommand.ExecutionTask), args[0].PropertyName);
        Assert.AreEqual(nameof(IAsyncRelayCommand.IsRunning), args[1].PropertyName);
        Assert.AreEqual(nameof(IAsyncRelayCommand.CanBeCanceled), args[2].PropertyName);
        Assert.AreEqual(nameof(IAsyncRelayCommand.IsCancellationRequested), args[3].PropertyName);

        command.Cancel();

        // Verify that these two properties raised notifications correctly when canceling the command too.
        // We need to ensure all command properties support notifications so that users can bind to them.
        Assert.AreEqual(6, args.Count);
        Assert.AreEqual(nameof(IAsyncRelayCommand.CanBeCanceled), args[4].PropertyName);
        Assert.AreEqual(nameof(IAsyncRelayCommand.IsCancellationRequested), args[5].PropertyName);

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

        AsyncRelayCommand? command = new(() => cancellationTokenSources[index++].Task, AsyncRelayCommandOptions.AllowConcurrentExecutions);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        (object?, EventArgs?) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        command.NotifyCanExecuteChanged();

        Assert.AreSame(command, args.Item1);
        Assert.AreSame(EventArgs.Empty, args.Item2);

        args = default;

        Assert.IsNull(command.ExecutionTask);
        Assert.IsFalse(command.IsRunning);

        Task task = command.ExecuteAsync(null);

        Assert.IsNotNull(command.ExecutionTask);
        Assert.AreSame(task, command.ExecutionTask);
        Assert.AreSame(cancellationTokenSources[0].Task, command.ExecutionTask);
        Assert.IsTrue(command.IsRunning);

        // The command can still be executed now
        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        Task newTask = command.ExecuteAsync(null);

        // A new task was returned
        Assert.AreSame(newTask, command.ExecutionTask);
        Assert.AreSame(cancellationTokenSources[1].Task, command.ExecutionTask);

        cancellationTokenSources[0].SetResult(null);
        cancellationTokenSources[1].SetResult(null);

        _ = await Task.WhenAll(cancellationTokenSources[0].Task, cancellationTokenSources[1].Task);

        Assert.IsFalse(command.IsRunning);

        // CanExecute isn't raised again when the command completes, if concurrent executions are allowed
        Assert.IsNull(args.Item1);
        Assert.IsNull(args.Item2);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommand_AllowConcurrentExecutions_Disabled()
    {
        await Test_AsyncRelayCommand_AllowConcurrentExecutions_TestLogic(static task => new(async () => await task, AsyncRelayCommandOptions.None));
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

        Assert.AreSame(command, args.Item1);
        Assert.AreSame(EventArgs.Empty, args.Item2);

        args = default;

        Assert.IsNull(command.ExecutionTask);
        Assert.IsFalse(command.IsRunning);

        Task task = command.ExecuteAsync(null);

        // CanExecute is raised upon execution
        Assert.AreSame(command, args.Item1);
        Assert.AreSame(EventArgs.Empty, args.Item2);

        args = default;

        Assert.IsNotNull(command.ExecutionTask);
        Assert.AreSame(task, command.ExecutionTask);
        Assert.IsTrue(command.IsRunning);

        // The command can't be executed now, as there's a pending operation
        Assert.IsFalse(command.CanExecute(null));
        Assert.IsFalse(command.CanExecute(new object()));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        // CanExecute hasn't been raised again
        Assert.IsNull(args.Item1);
        Assert.IsNull(args.Item2);

        tcs.SetResult(null);

        await task;

        Assert.IsFalse(command.IsRunning);

        // CanExecute is raised automatically when command execution completes, if concurrent executions are disabled
        Assert.AreSame(command, args.Item1);
        Assert.AreSame(EventArgs.Empty, args.Item2);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_EnsureExceptionThrown_Synchronously()
    {
        Exception? executeException = null;

        AsyncRelayCommand command = new(async () =>
        {
            await Task.CompletedTask;

            throw new Exception(nameof(Test_AsyncRelayCommand_EnsureExceptionThrown_Synchronously));
        });

        try
        {
            AsyncContext.Run(async () =>
            {
                command.Execute(null);

                await Task.Delay(500);
            });
        }
        catch (Exception e)
        {
            executeException = e;
        }

        Assert.AreEqual(nameof(Test_AsyncRelayCommand_EnsureExceptionThrown_Synchronously), executeException?.Message);
    }

    // See https://github.com/CommunityToolkit/dotnet/pull/251
    [TestMethod]
    public async Task Test_AsyncRelayCommand_EnsureExceptionThrown()
    {
        const int delay = 500;

        Exception? executeException = null;
        Exception? executeAsyncException = null;

        AsyncRelayCommand command = new(async () =>
        {
            await Task.Delay(delay);

            throw new Exception(nameof(Test_AsyncRelayCommand_EnsureExceptionThrown));
        });

        try
        {
            // Use AsyncContext to test async void methods https://stackoverflow.com/a/14207615/5953643
            AsyncContext.Run(async () =>
            {
                command.Execute(null);

                await Task.Delay(delay * 2); // Ensure we don't escape `AsyncContext` before command throws Exception
            });
        }
        catch (Exception e)
        {
            executeException = e;
        }

        executeAsyncException = await Assert.ThrowsExceptionAsync<Exception>(() => command.ExecuteAsync(null));

        Assert.AreEqual(nameof(Test_AsyncRelayCommand_EnsureExceptionThrown), executeException?.Message);
        Assert.AreEqual(nameof(Test_AsyncRelayCommand_EnsureExceptionThrown), executeAsyncException?.Message);
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
            AsyncRelayCommand command = new(() => TestMethodAsync(throwAction), AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

            command.Execute(null);

            await Task.Delay(200);

            completeAction();
        }

        bool success = await TaskSchedulerTestHelper.IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback);

        Assert.IsTrue(success);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommand_ThrowingTaskBubblesToUnobservedTaskException_Synchronously()
    {
        static async Task TestMethodAsync(Action action)
        {
            await Task.CompletedTask;

            action();
        }

        async void TestCallback(Action throwAction, Action completeAction)
        {
            AsyncRelayCommand command = new(() => TestMethodAsync(throwAction), AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

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

        AsyncRelayCommand command = new(() => tcs.Task, AsyncRelayCommandOptions.AllowConcurrentExecutions);

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

        AsyncRelayCommand command = new(() => tcs.Task, AsyncRelayCommandOptions.None);

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

        AsyncRelayCommand command = new(token => tcs.Task, AsyncRelayCommandOptions.AllowConcurrentExecutions);

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

        AsyncRelayCommand command = new(token => tcs.Task, AsyncRelayCommandOptions.None);

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

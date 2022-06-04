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
public class Test_AsyncRelayCommandOfT
{
    [TestMethod]
    public async Task Test_AsyncRelayCommandOfT_AlwaysEnabled()
    {
        int ticks = 0;

        AsyncRelayCommand<string>? command = new(async s =>
        {
            await Task.Delay(1000);
            ticks = int.Parse(s!);
            await Task.Delay(1000);
        });

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute("1"));

        (object?, EventArgs?) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        command.NotifyCanExecuteChanged();

        Assert.AreSame(args.Item1, command);
        Assert.AreSame(args.Item2, EventArgs.Empty);

        Assert.IsNull(command.ExecutionTask);
        Assert.IsFalse(command.IsRunning);

        Task task = command.ExecuteAsync((object)"42");

        Assert.IsNotNull(command.ExecutionTask);
        Assert.AreSame(command.ExecutionTask, task);
        Assert.IsTrue(command.IsRunning);

        await task;

        Assert.IsFalse(command.IsRunning);

        Assert.AreEqual(ticks, 42);

        command.Execute("2");

        await command.ExecutionTask!;

        Assert.AreEqual(ticks, 2);

        _ = Assert.ThrowsException<InvalidCastException>(() => command.Execute(new object()));
    }

    [TestMethod]
    public void Test_AsyncRelayCommandOfT_WithCanExecuteFunctionTrue()
    {
        int ticks = 0;

        AsyncRelayCommand<string>? command = new(
            s =>
            {
                ticks = int.Parse(s!);
                return Task.CompletedTask;
            }, s => true);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute("1"));

        command.Execute("42");

        Assert.AreEqual(ticks, 42);

        command.Execute("2");

        Assert.AreEqual(ticks, 2);
    }

    [TestMethod]
    public void Test_AsyncRelayCommandOfT_WithCanExecuteFunctionFalse()
    {
        int ticks = 0;

        AsyncRelayCommand<string>? command = new(
            s =>
            {
                ticks = int.Parse(s!);
                return Task.CompletedTask;
            }, s => false);

        Assert.IsFalse(command.CanExecute(null));
        Assert.IsFalse(command.CanExecute("1"));

        command.Execute("2");

        // Like in the RelayCommand test, ensure Execute is unconditionally invoked
        Assert.AreEqual(ticks, 2);

        command.Execute("42");

        Assert.AreEqual(ticks, 42);
    }

    [TestMethod]
    public void Test_AsyncRelayCommandOfT_NullWithValueType()
    {
        int n = 0;

        AsyncRelayCommand<int>? command = new(i =>
        {
            n = i;
            return Task.CompletedTask;
        });

        Assert.IsFalse(command.CanExecute(null));
        _ = Assert.ThrowsException<NullReferenceException>(() => command.Execute(null));

        command = new AsyncRelayCommand<int>(
            i =>
            {
                n = i;
                return Task.CompletedTask;
            }, i => i > 0);

        Assert.IsFalse(command.CanExecute(null));
        _ = Assert.ThrowsException<NullReferenceException>(() => command.Execute(null));
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommandOfT_WithCancellation()
    {
        // See comments in Test_AsyncRelayCommand_WithCancellation for the logic below
        TaskCompletionSource<object?> tcs = new();
        AsyncRelayCommand<string> command = new((s, token) => tcs.Task);

        List<PropertyChangedEventArgs> args = new();

        command.PropertyChanged += (s, e) => args.Add(e);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute("Hello"));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        command.Execute(null);

        Assert.IsTrue(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        Assert.AreEqual(args.Count, 4);
        Assert.AreEqual(args[0].PropertyName, nameof(IAsyncRelayCommand.ExecutionTask));
        Assert.AreEqual(args[1].PropertyName, nameof(IAsyncRelayCommand.IsRunning));
        Assert.AreEqual(args[2].PropertyName, nameof(IAsyncRelayCommand.CanBeCanceled));
        Assert.AreEqual(args[3].PropertyName, nameof(IAsyncRelayCommand.IsCancellationRequested));

        command.Cancel();

        Assert.AreEqual(args.Count, 6);
        Assert.AreEqual(args[4].PropertyName, nameof(IAsyncRelayCommand.CanBeCanceled));
        Assert.AreEqual(args[5].PropertyName, nameof(IAsyncRelayCommand.IsCancellationRequested));

        Assert.IsTrue(command.IsCancellationRequested);

        tcs.SetResult(null);

        await command.ExecutionTask!;

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsTrue(command.IsCancellationRequested);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommandOfT_AllowConcurrentExecutions_Disabled()
    {
        await Test_AsyncRelayCommandOfT_AllowConcurrentExecutions_TestLogic(static task => new(async _ => await task, AsyncRelayCommandOptions.None));
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommandOfT_AllowConcurrentExecutions_Default()
    {
        await Test_AsyncRelayCommandOfT_AllowConcurrentExecutions_TestLogic(static task => new(async _ => await task));
    }

    /// <summary>
    /// Shared logic for <see cref="Test_AsyncRelayCommandOfT_AllowConcurrentExecutions_Disabled"/> and <see cref="Test_AsyncRelayCommandOfT_AllowConcurrentExecutions_Default"/>.
    /// </summary>
    /// <param name="factory">A factory to create the <see cref="AsyncRelayCommand{T}"/> instance to test.</param>
    private static async Task Test_AsyncRelayCommandOfT_AllowConcurrentExecutions_TestLogic(Func<Task, AsyncRelayCommand<string>> factory)
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand<string> command = factory(tcs.Task);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute("1"));

        (object?, EventArgs?) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        command.NotifyCanExecuteChanged();

        Assert.AreSame(args.Item1, command);
        Assert.AreSame(args.Item2, EventArgs.Empty);

        Assert.IsNull(command.ExecutionTask);
        Assert.IsFalse(command.IsRunning);

        Task task = command.ExecuteAsync((object)"42");

        Assert.IsNotNull(command.ExecutionTask);
        Assert.AreSame(command.ExecutionTask, task);
        Assert.IsTrue(command.IsRunning);

        // The command can't be executed now, as there's a pending operation
        Assert.IsFalse(command.CanExecute(null));
        Assert.IsFalse(command.CanExecute("2"));

        Assert.IsFalse(command.CanBeCanceled);
        Assert.IsFalse(command.IsCancellationRequested);

        tcs.SetResult(null);

        await task;

        Assert.IsFalse(command.IsRunning);

        _ = Assert.ThrowsException<InvalidCastException>(() => command.Execute(new object()));
    }

    [TestMethod]
    public void Test_AsyncRelayCommandOfT_EnsureExceptionThrown_Synchronously()
    {
        Exception? executeException = null;

        AsyncRelayCommand<int> command = new(async delay =>
        {
            await Task.CompletedTask;

            throw new Exception(nameof(Test_AsyncRelayCommandOfT_EnsureExceptionThrown_Synchronously));
        });

        try
        {
            AsyncContext.Run(async () =>
            {
                command.Execute((object)42);

                await Task.Delay(500);
            });
        }
        catch (Exception e)
        {
            executeException = e;
        }

        Assert.AreEqual(nameof(Test_AsyncRelayCommandOfT_EnsureExceptionThrown_Synchronously), executeException?.Message);
    }

    // See https://github.com/CommunityToolkit/dotnet/pull/251
    [TestMethod]
    public async Task Test_AsyncRelayCommandOfT_EnsureExceptionThrown()
    {
        const int delay = 500;

        Exception? executeException = null;
        Exception? executeAsyncException = null;

        AsyncRelayCommand<int> command = new(async delay =>
        {
            await Task.Delay(delay);

            throw new Exception(nameof(Test_AsyncRelayCommandOfT_EnsureExceptionThrown));
        });

        try
        {
            AsyncContext.Run(async () =>
            {
                command.Execute((object)delay);

                await Task.Delay(delay * 2);
            });
        }
        catch (Exception e)
        {
            executeException = e;
        }

        executeAsyncException = await Assert.ThrowsExceptionAsync<Exception>(() => command.ExecuteAsync((object)delay));

        Assert.AreEqual(nameof(Test_AsyncRelayCommandOfT_EnsureExceptionThrown), executeException?.Message);
        Assert.AreEqual(nameof(Test_AsyncRelayCommandOfT_EnsureExceptionThrown), executeAsyncException?.Message);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommandOfT_EnsureExceptionThrown_GenericExecute()
    {
        const int delay = 500;

        Exception? executeTException = null;
        Exception? executeAsyncException = null;

        AsyncRelayCommand<int> command = new(async delay =>
        {
            await Task.Delay(delay);

            throw new Exception(nameof(Test_AsyncRelayCommandOfT_EnsureExceptionThrown_GenericExecute));
        });

        try
        {
            AsyncContext.Run(async () =>
            {
                command.Execute(delay);

                await Task.Delay(delay * 2);
            });
        }
        catch (Exception e)
        {
            executeTException = e;
        }

        executeAsyncException = await Assert.ThrowsExceptionAsync<Exception>(() => command.ExecuteAsync(delay));

        Assert.AreEqual(nameof(Test_AsyncRelayCommandOfT_EnsureExceptionThrown_GenericExecute), executeTException?.Message);
        Assert.AreEqual(nameof(Test_AsyncRelayCommandOfT_EnsureExceptionThrown_GenericExecute), executeAsyncException?.Message);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommandOfT_ThrowingTaskBubblesToUnobservedTaskException()
    {
        static async Task TestMethodAsync(Action action)
        {
            await Task.Delay(100);

            action();
        }

        async void TestCallback(Action throwAction, Action completeAction)
        {
            AsyncRelayCommand<string> command = new(s => TestMethodAsync(throwAction), AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

            command.Execute(null);

            await Task.Delay(200);

            completeAction();
        }

        bool success = await TaskSchedulerTestHelper.IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback);

        Assert.IsTrue(success);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommandOfT_ThrowingTaskBubblesToUnobservedTaskException_Synchronously()
    {
        static async Task TestMethodAsync(Action action)
        {
            await Task.CompletedTask;

            action();
        }

        async void TestCallback(Action throwAction, Action completeAction)
        {
            AsyncRelayCommand<string> command = new(s => TestMethodAsync(throwAction), AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

            command.Execute(null);

            await Task.Delay(200);

            completeAction();
        }

        bool success = await TaskSchedulerTestHelper.IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback);

        Assert.IsTrue(success);
    }

    public async Task Test_AsyncRelayCommand_ExecuteDoesNotRaiseCanExecuteChanged()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand<string> command = new(s => tcs.Task, AsyncRelayCommandOptions.AllowConcurrentExecutions);

        (object? Sender, EventArgs? Args) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        Assert.IsTrue(command.CanExecute(""));

        command.Execute("");

        Assert.IsNull(args.Sender);
        Assert.IsNull(args.Args);

        args = default;

        Assert.IsTrue(command.CanExecute(""));

        tcs.SetResult(null);

        _ = await tcs.Task;

        // CanExecute isn't raised when the command completes
        Assert.IsNull(args.Sender);
        Assert.IsNull(args.Args);
    }

    [TestMethod]
    public async Task Test_AsyncRelayCommand_ExecuteWithoutConcurrencyRaisesCanExecuteChanged()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand<string> command = new(s => tcs.Task, AsyncRelayCommandOptions.None);

        (object? Sender, EventArgs? Args) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        Assert.IsTrue(command.CanExecute(""));

        command.Execute("");

        Assert.AreSame(command, args.Sender);
        Assert.AreSame(EventArgs.Empty, args.Args);

        Assert.IsFalse(command.CanExecute(""));

        args = default;

        tcs.SetResult(null);

        _ = await tcs.Task;

        // CanExecute is raised again when the command completes
        Assert.AreSame(command, args.Sender);
        Assert.AreSame(EventArgs.Empty, args.Args);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_ExecuteDoesNotRaiseCanExecuteChanged_WithCancellation()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand<string> command = new((s, token) => tcs.Task, AsyncRelayCommandOptions.AllowConcurrentExecutions);

        (object? Sender, EventArgs? Args) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        Assert.IsTrue(command.CanExecute(""));

        command.Execute("");

        Assert.IsNull(args.Sender);
        Assert.IsNull(args.Args);

        Assert.IsTrue(command.CanExecute(""));

        tcs.SetResult(null);
    }

    [TestMethod]
    public void Test_AsyncRelayCommand_ExecuteWithoutConcurrencyRaisesCanExecuteChanged_WithToken()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand<string> command = new((s, token) => tcs.Task, AsyncRelayCommandOptions.None);

        (object? Sender, EventArgs? Args) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        Assert.IsTrue(command.CanExecute(""));

        command.Execute("");

        Assert.AreSame(command, args.Sender);
        Assert.AreSame(EventArgs.Empty, args.Args);

        Assert.IsFalse(command.CanExecute(""));

        tcs.SetResult(null);
    }

    [TestMethod]
    public void Test_AsyncRelayCommandOfT_GetCancelCommand_DisabledCommand()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand<string> command = new(s => tcs.Task);

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
    public void Test_AsyncRelayCommandOfT_GetCancelCommand_WithToken()
    {
        TaskCompletionSource<object?> tcs = new();

        AsyncRelayCommand<string> command = new((s, token) => tcs.Task);

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

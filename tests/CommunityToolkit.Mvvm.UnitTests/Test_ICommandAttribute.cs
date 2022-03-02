// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_ICommandAttribute
{
    [TestMethod]
    public async Task Test_ICommandAttribute_RelayCommand()
    {
        MyViewModel model = new();

        model.IncrementCounterCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.IncrementCounterWithValueCommand.Execute(5);

        Assert.AreEqual(model.Counter, 6);

        await model.DelayAndIncrementCounterCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 7);

        await model.DelayAndIncrementCounterWithTokenCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 8);

        await model.DelayAndIncrementCounterWithValueCommand.ExecuteAsync(5);

        Assert.AreEqual(model.Counter, 13);

        await model.DelayAndIncrementCounterWithValueAndTokenCommand.ExecuteAsync(5);

        Assert.AreEqual(model.Counter, 18);

        List<Task> tasks = new();

        for (int i = 0; i < 10; i++)
        {
            if (model.AddValueToListAndDelayCommand.CanExecute(i))
            {
                tasks.Add(model.AddValueToListAndDelayCommand.ExecuteAsync(i));
            }
        }

        // All values should already be in the list, as commands are executed
        // concurrently. Each invocation should still be pending here, but the
        // values are added to the list before starting the delay.
        CollectionAssert.AreEqual(model.Values, Enumerable.Range(0, 10).ToArray());

        await Task.WhenAll(tasks);

        tasks.Clear();

        for (int i = 10; i < 20; i++)
        {
            if (model.AddValueToListAndDelayWithDefaultConcurrencyCommand.CanExecute(i))
            {
                tasks.Add(model.AddValueToListAndDelayWithDefaultConcurrencyCommand.ExecuteAsync(i));
            }
        }

        model.Tcs.SetResult(null);

        await Task.WhenAll(tasks);

        Assert.AreEqual(1, tasks.Count);

        // Only the first item should have been added
        CollectionAssert.AreEqual(model.Values, Enumerable.Range(0, 11).ToArray());

        for (int i = 1; i < tasks.Count; i++)
        {
            Assert.AreSame(Task.CompletedTask, tasks[i]);
        }

        tasks.Clear();

        for (int i = 11; i < 21; i++)
        {
            tasks.Add(model.AddValueToListAndDelayWithDefaultConcurrencyAsync_WithCancelCommandCommand.ExecuteAsync(i));
        }

        Assert.AreEqual(10, tasks.Count);

        // Only the first item should have been added, like the previous case
        CollectionAssert.AreEqual(model.Values, Enumerable.Range(0, 12).ToArray());
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_NoParameters_Property()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        model.IncrementCounter_NoParameters_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        Assert.IsFalse(model.IncrementCounter_NoParameters_PropertyCommand.CanExecute(null));

        // This and all test above also verify the logic is unconditionally invoked if CanExecute is ignored
        model.IncrementCounter_NoParameters_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_NoParameters_GeneratedProperty()
    {
        CanExecuteViewModel model = new();

        model.SetGeneratedFlag(true);

        model.IncrementCounter_NoParameters_GeneratedPropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.SetGeneratedFlag(false);

        Assert.IsFalse(model.IncrementCounter_NoParameters_GeneratedPropertyCommand.CanExecute(null));

        model.IncrementCounter_NoParameters_GeneratedPropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_WithParameter_Property()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        model.IncrementCounter_WithParameter_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        Assert.IsFalse(model.IncrementCounter_WithParameter_PropertyCommand.CanExecute(null));

        model.IncrementCounter_WithParameter_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_WithParameter_GeneratedProperty()
    {
        CanExecuteViewModel model = new();

        model.SetGeneratedFlag(true);

        model.IncrementCounter_WithParameter_GeneratedPropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.SetGeneratedFlag(false);

        Assert.IsFalse(model.IncrementCounter_WithParameter_GeneratedPropertyCommand.CanExecute(null));

        model.IncrementCounter_WithParameter_GeneratedPropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_NoParameters_MethodWithNoParameters()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        model.IncrementCounter_NoParameters_MethodWithNoParametersCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        Assert.IsFalse(model.IncrementCounter_WithParameter_PropertyCommand.CanExecute(null));

        model.IncrementCounter_WithParameter_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_WithParameters_MethodWithNoParameters()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        model.IncrementCounter_WithParameters_MethodWithNoParametersCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        Assert.IsFalse(model.IncrementCounter_WithParameters_MethodWithNoParametersCommand.CanExecute(null));

        model.IncrementCounter_WithParameters_MethodWithNoParametersCommand.Execute(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_WithParameters_MethodWithMatchingParameter()
    {
        CanExecuteViewModel model = new();

        model.IncrementCounter_WithParameters_MethodWithMatchingParameterCommand.Execute(new User { Name = nameof(CanExecuteViewModel) });

        Assert.AreEqual(model.Counter, 1);

        Assert.IsFalse(model.IncrementCounter_WithParameters_MethodWithMatchingParameterCommand.CanExecute(new User()));

        model.IncrementCounter_WithParameters_MethodWithMatchingParameterCommand.Execute(new User());

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public async Task Test_ICommandAttribute_CanExecute_Async_NoParameters_Property()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        await model.IncrementCounter_Async_NoParameters_PropertyCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        Assert.IsFalse(model.IncrementCounter_Async_NoParameters_PropertyCommand.CanExecute(null));

        await model.IncrementCounter_Async_NoParameters_PropertyCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public async Task Test_ICommandAttribute_CanExecute_Async_WithParameter_Property()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        await model.IncrementCounter_Async_WithParameter_PropertyCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        Assert.IsFalse(model.IncrementCounter_Async_WithParameter_PropertyCommand.CanExecute(null));

        await model.IncrementCounter_Async_WithParameter_PropertyCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public async Task Test_ICommandAttribute_CanExecute_Async_NoParameters_MethodWithNoParameters()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        await model.IncrementCounter_Async_NoParameters_MethodWithNoParametersCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        Assert.IsFalse(model.IncrementCounter_Async_WithParameter_PropertyCommand.CanExecute(null));

        await model.IncrementCounter_Async_WithParameter_PropertyCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public async Task Test_ICommandAttribute_CanExecute_Async_WithParameters_MethodWithNoParameters()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        await model.IncrementCounter_Async_WithParameters_MethodWithNoParametersCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        Assert.IsFalse(model.IncrementCounter_Async_WithParameters_MethodWithNoParametersCommand.CanExecute(null));

        await model.IncrementCounter_Async_WithParameters_MethodWithNoParametersCommand.ExecuteAsync(null);

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public async Task Test_ICommandAttribute_CanExecute_Async_WithParameters_MethodWithMatchingParameter()
    {
        CanExecuteViewModel model = new();

        await model.IncrementCounter_Async_WithParameters_MethodWithMatchingParameterCommand.ExecuteAsync(new User { Name = nameof(CanExecuteViewModel) });

        Assert.AreEqual(model.Counter, 1);

        Assert.IsFalse(model.IncrementCounter_Async_WithParameters_MethodWithMatchingParameterCommand.CanExecute(new User()));

        await model.IncrementCounter_Async_WithParameters_MethodWithMatchingParameterCommand.ExecuteAsync(new User());

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public async Task Test_ICommandAttribute_ConcurrencyControl_AsyncRelayCommand()
    {
        TaskCompletionSource<object?> tcs = new();

        MyViewModel model = new() { ExternalTask = tcs.Task };

        Task task = model.AwaitForExternalTaskCommand.ExecuteAsync(null);

        Assert.IsTrue(model.AwaitForExternalTaskCommand.IsRunning);
        Assert.IsFalse(model.AwaitForExternalTaskCommand.CanExecute(null));

        tcs.SetResult(null);

        await task;

        Assert.IsFalse(model.AwaitForExternalTaskCommand.IsRunning);
        Assert.IsTrue(model.AwaitForExternalTaskCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task Test_ICommandAttribute_ConcurrencyControl_AsyncRelayCommandOfT()
    {
        MyViewModel model = new();

        TaskCompletionSource<object?> tcs = new();

        Task task = model.AwaitForInputTaskCommand.ExecuteAsync(tcs.Task);

        Assert.IsTrue(model.AwaitForInputTaskCommand.IsRunning);
        Assert.IsFalse(model.AwaitForInputTaskCommand.CanExecute(null));

        tcs.SetResult(null);

        await task;

        Assert.IsFalse(model.AwaitForInputTaskCommand.IsRunning);
        Assert.IsTrue(model.AwaitForInputTaskCommand.CanExecute(null));
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/13
    [TestMethod]
    public void Test_ICommandAttribute_ViewModelRightAfterRegion()
    {
        ViewModelForIssue13 model = new();

        Assert.IsNotNull(model.GreetCommand);
        Assert.IsInstanceOfType(model.GreetCommand, typeof(RelayCommand));
    }

    [TestMethod]
    public async Task Test_ICommandAttribute_CancelCommands()
    {
        CancelCommandViewModel model = new();

        model.DoWorkCommand.Execute(null);

        Assert.IsTrue(model.DoWorkCancelCommand.CanExecute(null));

        model.DoWorkCancelCommand.Execute(null);

        await Task.Yield();

        Assert.IsTrue(model.Tcs1.Task.IsCanceled);
        Assert.IsTrue(model.Result1 is OperationCanceledException);

        model.DoWorkWithParameterCommand.Execute(42);

        Assert.IsTrue(model.DoWorkWithParameterCancelCommand.CanExecute(null));

        model.DoWorkWithParameterCancelCommand.Execute(null);

        await Task.Yield();

        Assert.IsTrue(model.Tcs2.Task.IsCanceled);
        Assert.IsTrue(model.Result2 is OperationCanceledException);
    }

    #region Region
    public class Region
    {
    }
    #endregion

    public partial class ViewModelForIssue13
    {
        [ICommand]
        private void Greet()
        {
        }
    }

    public sealed partial class MyViewModel
    {
        public Task? ExternalTask { get; set; }

        public int Counter { get; private set; }

        public List<int> Values { get; } = new();

        public TaskCompletionSource<object?> Tcs { get; } = new();

        /// <summary>This is a single line summary.</summary>
        [ICommand]
        private void IncrementCounter()
        {
            Counter++;
        }

        /// <summary>
        /// This is a multiline summary
        /// </summary>
        [ICommand]
        private void IncrementCounterWithValue(int count)
        {
            Counter += count;
        }

        /// <summary>This is single line with also other stuff below</summary>
        /// <returns>Foo bar baz</returns>
        /// <returns>A task</returns>
        [ICommand]
        private async Task DelayAndIncrementCounterAsync()
        {
            await Task.Delay(50);

            Counter += 1;
        }

        [ICommand(AllowConcurrentExecutions = true)]
        private async Task AddValueToListAndDelayAsync(int value)
        {
            Values.Add(value);

            await Task.Delay(100);
        }

        [ICommand]
        private async Task AddValueToListAndDelayWithDefaultConcurrencyAsync(int value)
        {
            Values.Add(value);

            _ = await Tcs.Task;
        }

        [ICommand(IncludeCancelCommand = true)]
        private async Task AddValueToListAndDelayWithDefaultConcurrencyAsync_WithCancelCommand(int value, CancellationToken token)
        {
            Values.Add(value);

            await Task.Delay(1000);
        }

        #region Test region

        /// <summary>
        /// This is multi line with also other stuff below
        /// </summary>
        /// <returns>Foo bar baz</returns>
        /// <returns>A task</returns>
        [ICommand]
        private async Task DelayAndIncrementCounterWithTokenAsync(CancellationToken token)
        {
            await Task.Delay(50);

            Counter += 1;
        }

        // This should not be ported over
        [ICommand]
        private async Task DelayAndIncrementCounterWithValueAsync(int count)
        {
            await Task.Delay(50);

            Counter += count;
        }

        #endregion

        [ICommand]
        private async Task DelayAndIncrementCounterWithValueAndTokenAsync(int count, CancellationToken token)
        {
            await Task.Delay(50);

            Counter += count;
        }

        [ICommand(AllowConcurrentExecutions = false)]
        private async Task AwaitForExternalTaskAsync()
        {
            await ExternalTask!;
        }

        [ICommand(AllowConcurrentExecutions = false)]
        private async Task AwaitForInputTaskAsync(Task task)
        {
            await task;
        }
    }
    
    public sealed partial class CanExecuteViewModel : ObservableObject
    {
        public int Counter { get; private set; }

        public bool Flag { get; set; }

        public void SetGeneratedFlag(bool flag)
        {
            GeneratedFlag = flag;
        }

        [ObservableProperty]
        private bool generatedFlag;

        private bool GetFlag1() => Flag;

        private bool GetFlag2(User user) => user.Name == nameof(CanExecuteViewModel);

        [ICommand(CanExecute = nameof(Flag))]
        private void IncrementCounter_NoParameters_Property()
        {
            Counter++;
        }

        [ICommand(CanExecute = nameof(Flag))]
        private void IncrementCounter_WithParameter_Property(User user)
        {
            Counter++;
        }

        [ICommand(CanExecute = nameof(GeneratedFlag))]
        private void IncrementCounter_NoParameters_GeneratedProperty()
        {
            Counter++;
        }

        [ICommand(CanExecute = nameof(GeneratedFlag))]
        private void IncrementCounter_WithParameter_GeneratedProperty(User user)
        {
            Counter++;
        }

        [ICommand(CanExecute = nameof(GetFlag1))]
        private void IncrementCounter_NoParameters_MethodWithNoParameters()
        {
            Counter++;
        }

        [ICommand(CanExecute = nameof(GetFlag1))]
        private void IncrementCounter_WithParameters_MethodWithNoParameters(User user)
        {
            Counter++;
        }

        [ICommand(CanExecute = nameof(GetFlag2))]
        private void IncrementCounter_WithParameters_MethodWithMatchingParameter(User user)
        {
            Counter++;
        }

        [ICommand(CanExecute = nameof(Flag))]
        private async Task IncrementCounter_Async_NoParameters_Property()
        {
            Counter++;

            await Task.Delay(100);
        }

        [ICommand(CanExecute = nameof(Flag))]
        private async Task IncrementCounter_Async_WithParameter_Property(User user)
        {
            Counter++;

            await Task.Delay(100);
        }

        [ICommand(CanExecute = nameof(GeneratedFlag))]
        private async Task IncrementCounter_Async_NoParameters_GeneratedProperty()
        {
            Counter++;

            await Task.Delay(100);
        }

        [ICommand(CanExecute = nameof(GeneratedFlag))]
        private async Task IncrementCounter_Async_WithParameter_GeneratedProperty(User user)
        {
            Counter++;

            await Task.Delay(100);
        }

        [ICommand(CanExecute = nameof(GetFlag1))]
        private async Task IncrementCounter_Async_NoParameters_MethodWithNoParameters()
        {
            Counter++;

            await Task.Delay(100);
        }

        [ICommand(CanExecute = nameof(GetFlag1))]
        private async Task IncrementCounter_Async_WithParameters_MethodWithNoParameters(User user)
        {
            Counter++;

            await Task.Delay(100);
        }

        [ICommand(CanExecute = nameof(GetFlag2))]
        private async Task IncrementCounter_Async_WithParameters_MethodWithMatchingParameter(User user)
        {
            Counter++;

            await Task.Delay(100);
        }
    }

    public sealed class User
    {
        public string? Name { get; set; }
    }

    public partial class CancelCommandViewModel
    {
        public TaskCompletionSource<object?> Tcs1 { get; } = new();

        public object? Result1 { get; private set; }

        public TaskCompletionSource<object?> Tcs2 { get; } = new();

        public object? Result2 { get; private set; }

        [ICommand(IncludeCancelCommand = true)]
        private async Task DoWorkAsync(CancellationToken token)
        {
            using CancellationTokenRegistration registration = token.Register(static state => ((TaskCompletionSource<object?>)state!).TrySetCanceled(), Tcs1);

            try
            {
                _ = await Tcs1.Task;

                Result1 = 42;
            }
            catch (OperationCanceledException e)
            {
                Result1 = e;
            }
        }

        [ICommand(IncludeCancelCommand = true)]
        private async Task DoWorkWithParameterAsync(int number, CancellationToken token)
        {
            using CancellationTokenRegistration registration = token.Register(static state => ((TaskCompletionSource<object?>)state!).TrySetCanceled(), Tcs2);

            try
            {
                _ = await Tcs2.Task;

                Result2 = 42;
            }
            catch (OperationCanceledException e)
            {
                Result2 = e;
            }
        }
    }
}

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
public partial class Test_RelayCommandAttribute
{
    [TestMethod]
    public async Task Test_RelayCommandAttribute_RelayCommand()
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

        model.Values.Clear();
        tasks.Clear();

        for (int i = 0; i < 10; i++)
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
        CollectionAssert.AreEqual(model.Values, new[] { 0 });

        model.ResetTcs();
        model.Values.Clear();
        tasks.Clear();

        for (int i = 0; i < 10; i++)
        {
            // Ignore the checks
            tasks.Add(model.AddValueToListAndDelayWithDefaultConcurrencyCommand.ExecuteAsync(i));
        }

        model.Tcs.SetResult(null);

        await Task.WhenAll(tasks);

        Assert.AreEqual(10, tasks.Count);

        CollectionAssert.AreEqual(model.Values, Enumerable.Range(0, 10).ToArray());

        model.Values.Clear();
        tasks.Clear();

        for (int i = 0; i < 10; i++)
        {
            if (model.AddValueToListAndDelayWithDefaultConcurrencyAsync_WithCancelCommandCommand.CanExecute(i))
            {
                tasks.Add(model.AddValueToListAndDelayWithDefaultConcurrencyAsync_WithCancelCommandCommand.ExecuteAsync(i));
            }
        }

        Assert.AreEqual(1, tasks.Count);

        // Same as above, only the first one is added
        CollectionAssert.AreEqual(model.Values, new[] { 0 });

        model.Values.Clear();
        tasks.Clear();

        for (int i = 0; i < 10; i++)
        {
            // Ignore the checks
            tasks.Add(model.AddValueToListAndDelayWithDefaultConcurrencyAsync_WithCancelCommandCommand.ExecuteAsync(i));
        }

        Assert.AreEqual(10, tasks.Count);

        CollectionAssert.AreEqual(model.Values, Enumerable.Range(0, 10).ToArray());
    }

    [TestMethod]
    public void Test_RelayCommandAttribute_CanExecute_NoParameters_Property()
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
    public void Test_RelayCommandAttribute_CanExecute_NoParameters_GeneratedProperty()
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
    public void Test_RelayCommandAttribute_CanExecute_WithParameter_Property()
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
    public void Test_RelayCommandAttribute_CanExecute_WithParameter_GeneratedProperty()
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
    public void Test_RelayCommandAttribute_CanExecute_NoParameters_MethodWithNoParameters()
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
    public void Test_RelayCommandAttribute_CanExecute_WithParameters_MethodWithNoParameters()
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
    public void Test_RelayCommandAttribute_CanExecute_WithParameters_MethodWithMatchingParameter()
    {
        CanExecuteViewModel model = new();

        model.IncrementCounter_WithParameters_MethodWithMatchingParameterCommand.Execute(new User { Name = nameof(CanExecuteViewModel) });

        Assert.AreEqual(model.Counter, 1);

        Assert.IsFalse(model.IncrementCounter_WithParameters_MethodWithMatchingParameterCommand.CanExecute(new User()));

        model.IncrementCounter_WithParameters_MethodWithMatchingParameterCommand.Execute(new User());

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public async Task Test_RelayCommandAttribute_CanExecute_Async_NoParameters_Property()
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
    public async Task Test_RelayCommandAttribute_CanExecute_Async_WithParameter_Property()
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
    public async Task Test_RelayCommandAttribute_CanExecute_Async_NoParameters_MethodWithNoParameters()
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
    public async Task Test_RelayCommandAttribute_CanExecute_Async_WithParameters_MethodWithNoParameters()
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
    public async Task Test_RelayCommandAttribute_CanExecute_Async_WithParameters_MethodWithMatchingParameter()
    {
        CanExecuteViewModel model = new();

        await model.IncrementCounter_Async_WithParameters_MethodWithMatchingParameterCommand.ExecuteAsync(new User { Name = nameof(CanExecuteViewModel) });

        Assert.AreEqual(model.Counter, 1);

        Assert.IsFalse(model.IncrementCounter_Async_WithParameters_MethodWithMatchingParameterCommand.CanExecute(new User()));

        await model.IncrementCounter_Async_WithParameters_MethodWithMatchingParameterCommand.ExecuteAsync(new User());

        Assert.AreEqual(model.Counter, 2);
    }

    [TestMethod]
    public async Task Test_RelayCommandAttribute_ConcurrencyControl_AsyncRelayCommand()
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
    public async Task Test_RelayCommandAttribute_ConcurrencyControl_AsyncRelayCommandOfT()
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
    public void Test_RelayCommandAttribute_ViewModelRightAfterRegion()
    {
        ViewModelForIssue13 model = new();

        Assert.IsNotNull(model.GreetCommand);
        Assert.IsInstanceOfType((object)model.GreetCommand, typeof(RelayCommand));
    }

    [TestMethod]
    public async Task Test_RelayCommandAttribute_CancelCommands()
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

    [TestMethod]
    public async Task Test_RelayCommandAttribute_TaskOfTReturns()
    {
        GenericTaskCommands model = new();

        Task greetCommandTask = model.GreetCommand.ExecuteAsync(null);
        Task greetWithTokenTask = model.GreetWithTokenCommand.ExecuteAsync(null);
        Task greetWithParamTask = model.GreetWithParamCommand.ExecuteAsync(null);
        Task greetWithParamAndCommandTask = model.GreetWithParamAndTokenCommand.ExecuteAsync(null);

        Assert.IsInstanceOfType(greetCommandTask, typeof(Task<string>));
        Assert.IsInstanceOfType(greetWithTokenTask, typeof(Task<string>));
        Assert.IsInstanceOfType(greetWithParamTask, typeof(Task<string>));
        Assert.IsInstanceOfType(greetWithParamAndCommandTask, typeof(Task<string>));

        Assert.AreEqual("Hello world", await (Task<string>)greetCommandTask);
        Assert.AreEqual("Hello world", await (Task<string>)greetWithTokenTask);
        Assert.AreEqual("Hello world", await (Task<string>)greetWithParamTask);
        Assert.AreEqual("Hello world", await (Task<string>)greetWithParamAndCommandTask);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/230
    [TestMethod]
    public void Test_RelayCommandAttribute_CultureAwareCommandName()
    {
        ModelWithCultureAwareCommandName model = new();

        // This just needs to ensure it compiles, really
        model.InitializeCommand.Execute(null);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/162
    [TestMethod]
    public async Task Test_RelayCommandAttribute_WithOnPrefixes()
    {
        ModelWithCommandMethodsWithOnPrefixes model = new();

        Assert.IsFalse(model.HasOnCommandRun);
        Assert.IsFalse(model.HasOnboardCommandRun);
        Assert.IsFalse(model.HasSubmitCommandRun);
        Assert.IsFalse(model.HasDownloadCommandRun);

        model.OnCommand.Execute(null);

        Assert.IsTrue(model.HasOnCommandRun);

        model.OnboardCommand.Execute(null);

        Assert.IsTrue(model.HasOnboardCommandRun);

        model.SubmitCommand.Execute(null);

        Assert.IsTrue(model.HasSubmitCommandRun);

        await model.DownloadCommand.ExecuteAsync(null);

        Assert.IsTrue(model.HasDownloadCommandRun);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/283
    [TestMethod]
    public void Test_RelayCommandAttribute_VerifyNoWarningsForNullableValues()
    {
        ModelWithCommandsWithNullabilityAnnotations model = new();

        // Here we just need to verify we don't get any warnings.
        // That is, that means the generated code has the right nullability annotations.
        model.NullableObjectCommand.Execute(null);
        model.ListWithNullableTypeCommand.Execute(null);
        model.ListWithNullableTypeCommand.Execute(new List<object?>());
        model.TupleWithNullableElementsCommand.Execute((DateTime.Now, null, null, null));
        model.TupleWithNullableElementsCommand.Execute((DateTime.Now, null, true, new List<string>()));
        model.TupleWithNullableElementsCommand.Execute((null, "Hello", null, null));
        model.TupleWithNullableElementsCommand.Execute((null, null, null, null));
    }

    #region Region
    public class Region
    {
    }
    #endregion

    public partial class ViewModelForIssue13
    {
        [RelayCommand]
        private void Greet()
        {
        }
    }

    public sealed partial class MyViewModel
    {
        public Task? ExternalTask { get; set; }

        public int Counter { get; private set; }

        public List<int> Values { get; } = new();

        public TaskCompletionSource<object?> Tcs { get; private set; } = new();

        public void ResetTcs() => Tcs = new TaskCompletionSource<object?>();

        /// <summary>This is a single line summary.</summary>
        [RelayCommand]
        private void IncrementCounter()
        {
            Counter++;
        }

        /// <summary>
        /// This is a multiline summary
        /// </summary>
        [RelayCommand]
        private void IncrementCounterWithValue(int count)
        {
            Counter += count;
        }

        /// <summary>This is single line with also other stuff below</summary>
        /// <returns>Foo bar baz</returns>
        /// <returns>A task</returns>
        [RelayCommand]
        private async Task DelayAndIncrementCounterAsync()
        {
            await Task.Delay(50);

            Counter += 1;
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task AddValueToListAndDelayAsync(int value)
        {
            Values.Add(value);

            await Task.Delay(100);
        }

        [RelayCommand]
        private async Task AddValueToListAndDelayWithDefaultConcurrencyAsync(int value)
        {
            Values.Add(value);

            _ = await Tcs.Task;
        }

        [RelayCommand(IncludeCancelCommand = true)]
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
        [RelayCommand]
        private async Task DelayAndIncrementCounterWithTokenAsync(CancellationToken token)
        {
            await Task.Delay(50);

            Counter += 1;
        }

        // This should not be ported over
        [RelayCommand]
        private async Task DelayAndIncrementCounterWithValueAsync(int count)
        {
            await Task.Delay(50);

            Counter += count;
        }

        #endregion

        [RelayCommand]
        private async Task DelayAndIncrementCounterWithValueAndTokenAsync(int count, CancellationToken token)
        {
            await Task.Delay(50);

            Counter += count;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AwaitForExternalTaskAsync()
        {
            await ExternalTask!;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
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

        [RelayCommand(CanExecute = nameof(Flag))]
        private void IncrementCounter_NoParameters_Property()
        {
            Counter++;
        }

        [RelayCommand(CanExecute = nameof(Flag))]
        private void IncrementCounter_WithParameter_Property(User user)
        {
            Counter++;
        }

        [RelayCommand(CanExecute = nameof(GeneratedFlag))]
        private void IncrementCounter_NoParameters_GeneratedProperty()
        {
            Counter++;
        }

        [RelayCommand(CanExecute = nameof(GeneratedFlag))]
        private void IncrementCounter_WithParameter_GeneratedProperty(User user)
        {
            Counter++;
        }

        [RelayCommand(CanExecute = nameof(GetFlag1))]
        private void IncrementCounter_NoParameters_MethodWithNoParameters()
        {
            Counter++;
        }

        [RelayCommand(CanExecute = nameof(GetFlag1))]
        private void IncrementCounter_WithParameters_MethodWithNoParameters(User user)
        {
            Counter++;
        }

        [RelayCommand(CanExecute = nameof(GetFlag2))]
        private void IncrementCounter_WithParameters_MethodWithMatchingParameter(User user)
        {
            Counter++;
        }

        [RelayCommand(CanExecute = nameof(Flag))]
        private async Task IncrementCounter_Async_NoParameters_Property()
        {
            Counter++;

            await Task.Delay(100);
        }

        [RelayCommand(CanExecute = nameof(Flag))]
        private async Task IncrementCounter_Async_WithParameter_Property(User user)
        {
            Counter++;

            await Task.Delay(100);
        }

        [RelayCommand(CanExecute = nameof(GeneratedFlag))]
        private async Task IncrementCounter_Async_NoParameters_GeneratedProperty()
        {
            Counter++;

            await Task.Delay(100);
        }

        [RelayCommand(CanExecute = nameof(GeneratedFlag))]
        private async Task IncrementCounter_Async_WithParameter_GeneratedProperty(User user)
        {
            Counter++;

            await Task.Delay(100);
        }

        [RelayCommand(CanExecute = nameof(GetFlag1))]
        private async Task IncrementCounter_Async_NoParameters_MethodWithNoParameters()
        {
            Counter++;

            await Task.Delay(100);
        }

        [RelayCommand(CanExecute = nameof(GetFlag1))]
        private async Task IncrementCounter_Async_WithParameters_MethodWithNoParameters(User user)
        {
            Counter++;

            await Task.Delay(100);
        }

        [RelayCommand(CanExecute = nameof(GetFlag2))]
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

        [RelayCommand(IncludeCancelCommand = true)]
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

        [RelayCommand(IncludeCancelCommand = true)]
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

    public partial class GenericTaskCommands
    {
        [RelayCommand]
        private async Task<string> GreetAsync()
        {
            await Task.Yield();

            return "Hello world";
        }

        [RelayCommand]
        private async Task<string> GreetWithTokenAsync(CancellationToken token)
        {
            await Task.Yield();

            return "Hello world";
        }

        [RelayCommand]
        private async Task<string> GreetWithParamAsync(object _)
        {
            await Task.Yield();

            return "Hello world";
        }

        [RelayCommand]
        private async Task<string> GreetWithParamAndTokenAsync(object _, CancellationToken token)
        {
            await Task.Yield();

            return "Hello world";
        }
    }

    partial class ModelWithCultureAwareCommandName
    {
        // This starts with "I" to ensure it's converted to lowercase using invariant culture
        [RelayCommand]
        private void Initialize()
        {
        }
    }

    partial class ModelWithCommandMethodsWithOnPrefixes
    {
        public bool HasOnCommandRun { get; private set; }

        public bool HasOnboardCommandRun { get; private set; }

        public bool HasSubmitCommandRun { get; private set; }

        public bool HasDownloadCommandRun { get; private set; }

        [RelayCommand]
        private void On()
        {
            HasOnCommandRun = true;
        }

        [RelayCommand]
        private void Onboard()
        {
            HasOnboardCommandRun = true;
        }

        [RelayCommand]
        private void OnSubmit()
        {
            HasSubmitCommandRun = true;
        }

        [RelayCommand]
        private async Task OnDownloadAsync()
        {
            await Task.Delay(100);

            HasDownloadCommandRun = true;
        }
    }

    partial class ModelWithCommandsWithNullabilityAnnotations
    {
        [RelayCommand]
        private void NullableObject(object? parameter)
        {
        }

        [RelayCommand]
        private void ListWithNullableType(List<object?> parameter)
        {
        }

        [RelayCommand]
        private void TupleWithNullableElements((DateTime? date, string? message, bool? shouldPrint, List<string>? stringList) parameter)
        {
        }
    }
}

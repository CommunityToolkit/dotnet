// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

    [TestMethod]
    public void Test_RelayCommandAttribute_VerifyOptions()
    {
        ModelWithCommandsWithCustomOptions model = new();

        static void AssertOptions(IAsyncRelayCommand command, AsyncRelayCommandOptions options)
        {
            AsyncRelayCommandOptions commandOptions =
                (AsyncRelayCommandOptions)typeof(AsyncRelayCommand)
                .GetField("options", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(command)!;

            Assert.AreEqual(commandOptions, options);
        }

        static void AssertOptionsOfT<T>(IAsyncRelayCommand<T> command, AsyncRelayCommandOptions options)
        {
            AsyncRelayCommandOptions commandOptions =
                (AsyncRelayCommandOptions)typeof(AsyncRelayCommand<T>)
                .GetField("options", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(command)!;

            Assert.AreEqual(commandOptions, options);
        }

        AssertOptions(model.DefaultCommand, AsyncRelayCommandOptions.None);
        AssertOptions(model.AllowConcurrentExecutionsCommand, AsyncRelayCommandOptions.AllowConcurrentExecutions);
        AssertOptions(model.FlowExceptionsToTaskSchedulerCommand, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        AssertOptions(model.AllowConcurrentExecutionsAndFlowExceptionsToTaskSchedulerCommand, AsyncRelayCommandOptions.AllowConcurrentExecutions | AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

        AssertOptionsOfT(model.OfTDefaultCommand, AsyncRelayCommandOptions.None);
        AssertOptionsOfT(model.OfTAndAllowConcurrentExecutionsCommand, AsyncRelayCommandOptions.AllowConcurrentExecutions);
        AssertOptionsOfT(model.OfTAndFlowExceptionsToTaskSchedulerCommand, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        AssertOptionsOfT(model.OfTAndAllowConcurrentExecutionsAndFlowExceptionsToTaskSchedulerCommand, AsyncRelayCommandOptions.AllowConcurrentExecutions | AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/294
    [TestMethod]
    public void Test_RelayCommandAttribute_CanExecuteWithNullabilityAnnotations()
    {
        ModelWithCommandWithNullableCanExecute model = new();

        Assert.IsTrue(model.DoSomething1Command.CanExecute("Hello"));
        Assert.IsTrue(model.DoSomething2Command.CanExecute("Hello"));
        Assert.IsTrue(model.DoSomething3Command.CanExecute((0, "Hello")));
    }

    [TestMethod]
    public void Test_RelayCommandAttribute_WithExplicitAttributesForFieldAndProperty()
    {
        FieldInfo fooField = typeof(MyViewModelWithExplicitFieldAndPropertyAttributes).GetField("fooCommand", BindingFlags.Instance | BindingFlags.NonPublic)!;

        Assert.IsNotNull(fooField.GetCustomAttribute<RequiredAttribute>());
        Assert.IsNotNull(fooField.GetCustomAttribute<MinLengthAttribute>());
        Assert.AreEqual(fooField.GetCustomAttribute<MinLengthAttribute>()!.Length, 1);
        Assert.IsNotNull(fooField.GetCustomAttribute<MaxLengthAttribute>());
        Assert.AreEqual(fooField.GetCustomAttribute<MaxLengthAttribute>()!.Length, 100);

        PropertyInfo fooProperty = typeof(MyViewModelWithExplicitFieldAndPropertyAttributes).GetProperty("FooCommand")!;

        Assert.IsNotNull(fooProperty.GetCustomAttribute<RequiredAttribute>());
        Assert.IsNotNull(fooProperty.GetCustomAttribute<MinLengthAttribute>());
        Assert.AreEqual(fooProperty.GetCustomAttribute<MinLengthAttribute>()!.Length, 1);
        Assert.IsNotNull(fooProperty.GetCustomAttribute<MaxLengthAttribute>());
        Assert.AreEqual(fooProperty.GetCustomAttribute<MaxLengthAttribute>()!.Length, 100);

        PropertyInfo barProperty = typeof(MyViewModelWithExplicitFieldAndPropertyAttributes).GetProperty("BarCommand")!;

        Assert.IsNotNull(barProperty.GetCustomAttribute<JsonPropertyNameAttribute>());
        Assert.AreEqual(barProperty.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name, "bar");
        Assert.IsNotNull(barProperty.GetCustomAttribute<XmlIgnoreAttribute>());

        PropertyInfo bazProperty = typeof(MyViewModelWithExplicitFieldAndPropertyAttributes).GetProperty("BazCommand")!;

        Assert.IsNotNull(bazProperty.GetCustomAttribute<Test_ObservablePropertyAttribute.TestAttribute>());

        static void ValidateTestAttribute(TestValidationAttribute testAttribute)
        {
            Assert.IsNotNull(testAttribute);
            Assert.IsNull(testAttribute.O);
            Assert.AreEqual(testAttribute.T, typeof(MyViewModelWithExplicitFieldAndPropertyAttributes));
            Assert.AreEqual(testAttribute.Flag, true);
            Assert.AreEqual(testAttribute.D, 6.28);
            CollectionAssert.AreEqual(testAttribute.Names, new[] { "Bob", "Ross" });

            object[]? nestedArray = (object[]?)testAttribute.NestedArray;

            Assert.IsNotNull(nestedArray);
            Assert.AreEqual(nestedArray!.Length, 3);
            Assert.AreEqual(nestedArray[0], 1);
            Assert.AreEqual(nestedArray[1], "Hello");
            Assert.IsTrue(nestedArray[2] is int[]);
            CollectionAssert.AreEqual((int[])nestedArray[2], new[] { 2, 3, 4 });

            Assert.AreEqual(testAttribute.Animal, Test_ObservablePropertyAttribute.Animal.Llama);
        }

        FieldInfo fooBarField = typeof(MyViewModelWithExplicitFieldAndPropertyAttributes).GetField("fooBarCommand", BindingFlags.Instance | BindingFlags.NonPublic)!;

        ValidateTestAttribute(fooBarField.GetCustomAttribute<TestValidationAttribute>()!);

        PropertyInfo fooBarProperty = typeof(MyViewModelWithExplicitFieldAndPropertyAttributes).GetProperty("FooBarCommand")!;

        ValidateTestAttribute(fooBarProperty.GetCustomAttribute<TestValidationAttribute>()!);

        FieldInfo barBazField = typeof(MyViewModelWithExplicitFieldAndPropertyAttributes).GetField("barBazCommand", BindingFlags.Instance | BindingFlags.NonPublic)!;

        Assert.IsNotNull(barBazField.GetCustomAttribute<Test_ObservablePropertyAttribute.TestAttribute>());

        PropertyInfo barBazCommand = typeof(MyViewModelWithExplicitFieldAndPropertyAttributes).GetProperty("BarBazCommand")!;

        Assert.IsNotNull(barBazCommand.GetCustomAttribute<Test_ObservablePropertyAttribute.TestAttribute>());

        Test_ObservablePropertyAttribute.PropertyInfoAttribute testAttribute2 = barBazCommand.GetCustomAttribute<Test_ObservablePropertyAttribute.PropertyInfoAttribute>()!;

        Assert.IsNotNull(testAttribute2);
        Assert.IsNull(testAttribute2.O);
        Assert.AreEqual(testAttribute2.T, typeof(MyViewModelWithExplicitFieldAndPropertyAttributes));
        Assert.AreEqual(testAttribute2.Flag, true);
        Assert.AreEqual(testAttribute2.D, 6.28);
        Assert.IsNotNull(testAttribute2.Objects);
        Assert.IsTrue(testAttribute2.Objects is object[]);
        Assert.AreEqual(((object[])testAttribute2.Objects).Length, 1);
        Assert.AreEqual(((object[])testAttribute2.Objects)[0], "Test");
        CollectionAssert.AreEqual(testAttribute2.Names, new[] { "Bob", "Ross" });

        object[]? nestedArray2 = (object[]?)testAttribute2.NestedArray;

        Assert.IsNotNull(nestedArray2);
        Assert.AreEqual(nestedArray2!.Length, 4);
        Assert.AreEqual(nestedArray2[0], 1);
        Assert.AreEqual(nestedArray2[1], "Hello");
        Assert.AreEqual(nestedArray2[2], 42);
        Assert.IsNull(nestedArray2[3]);

        Assert.AreEqual(testAttribute2.Animal, (Test_ObservablePropertyAttribute.Animal)67);
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

    partial class ModelWithCommandsWithCustomOptions
    {
        [RelayCommand]
        private Task Default()
        {
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task OfTDefault(string obj)
        {
            return Task.CompletedTask;
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private Task AllowConcurrentExecutions()
        {
            return Task.CompletedTask;
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private Task OfTAndAllowConcurrentExecutions(string obj)
        {
            return Task.CompletedTask;
        }

        [RelayCommand(FlowExceptionsToTaskScheduler = true)]
        private Task FlowExceptionsToTaskScheduler()
        {
            return Task.CompletedTask;
        }

        [RelayCommand(FlowExceptionsToTaskScheduler = true)]
        private Task OfTAndFlowExceptionsToTaskScheduler(string obj)
        {
            return Task.CompletedTask;
        }

        [RelayCommand(AllowConcurrentExecutions = true, FlowExceptionsToTaskScheduler = true)]
        private Task AllowConcurrentExecutionsAndFlowExceptionsToTaskScheduler()
        {
            return Task.CompletedTask;
        }

        [RelayCommand(AllowConcurrentExecutions = true, FlowExceptionsToTaskScheduler = true)]
        private Task OfTAndAllowConcurrentExecutionsAndFlowExceptionsToTaskScheduler(string obj)
        {
            return Task.CompletedTask;
        }
    }

    partial class ModelWithCommandWithNullableCanExecute
    {
        bool CanDoSomething1(string? parameter)
        {
            return !string.IsNullOrEmpty(parameter);
        }

        [RelayCommand(CanExecute = (nameof(CanDoSomething1)))]
        private void DoSomething1(string? parameter)
        {
        }

        bool CanDoSomething2(string parameter)
        {
            return !string.IsNullOrEmpty(parameter);
        }

        [RelayCommand(CanExecute = (nameof(CanDoSomething2)))]
        private void DoSomething2(string? parameter)
        {
        }

        bool CanDoSomething3((int A, string? B) parameter)
        {
            return !string.IsNullOrEmpty(parameter.B); 
        }

        [RelayCommand(CanExecute = (nameof(CanDoSomething3)))]
        private void DoSomething3((int A, string? B) parameter)
        {
        }
    }

    public partial class MyViewModelWithExplicitFieldAndPropertyAttributes
    {
        [RelayCommand]
        [field: Required]
        [field: MinLength(1)]
        [field: MaxLength(100)]
        [property: Required]
        [property: MinLength(1)]
        [property: MaxLength(100)]
        private void Foo()
        {
        }

        [RelayCommand]
        [property: JsonPropertyName("bar")]
        [property: XmlIgnore]
        private void Bar()
        {
        }

        [RelayCommand]
        [property: Test_ObservablePropertyAttribute.Test]
        private async Task BazAsync()
        {
            await Task.Yield();
        }

        [RelayCommand]
        [field: TestValidation(null, typeof(MyViewModelWithExplicitFieldAndPropertyAttributes), true, 6.28, new[] { "Bob", "Ross" }, NestedArray = new object[] { 1, "Hello", new int[] { 2, 3, 4 } }, Animal = Test_ObservablePropertyAttribute.Animal.Llama)]
        [property: TestValidation(null, typeof(MyViewModelWithExplicitFieldAndPropertyAttributes), true, 6.28, new[] { "Bob", "Ross" }, NestedArray = new object[] { 1, "Hello", new int[] { 2, 3, 4 } }, Animal = Test_ObservablePropertyAttribute.Animal.Llama)]
        private void FooBar()
        {
        }

        [RelayCommand]
        [field: Test_ObservablePropertyAttribute.Test]
        [property: Test_ObservablePropertyAttribute.Test]
        [property: Test_ObservablePropertyAttribute.PropertyInfo(null, typeof(MyViewModelWithExplicitFieldAndPropertyAttributes), true, 6.28, new[] { "Bob", "Ross" }, new object[] { "Test" }, NestedArray = new object[] { 1, "Hello", 42, null }, Animal = (Test_ObservablePropertyAttribute.Animal)67)]
        private void BarBaz()
        {
        }
    }

    // Copy of the attribute from Test_ObservablePropertyAttribute, to test nested types
    private sealed class TestValidationAttribute : ValidationAttribute
    {
        public TestValidationAttribute(object? o, Type t, bool flag, double d, string[] names)
        {
            O = o;
            T = t;
            Flag = flag;
            D = d;
            Names = names;
        }

        public object? O { get; }

        public Type T { get; }

        public bool Flag { get; }

        public double D { get; }

        public string[] Names { get; }

        public object? NestedArray { get; set; }

        public Test_ObservablePropertyAttribute.Animal Animal { get; set; }
    }
}

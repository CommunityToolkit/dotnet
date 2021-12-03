// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_ICommandAttribute
{
    [TestMethod]
    public async Task Test_ICommandAttribute_RelayCommand()
    {
        MyViewModel? model = new();

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
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_NoParameters_Property()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        model.IncrementCounter_NoParameters_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        model.IncrementCounter_NoParameters_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_WithParameter_Property()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        model.IncrementCounter_WithParameter_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        model.IncrementCounter_WithParameter_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_NoParameters_MethodWithNoParameters()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        model.IncrementCounter_NoParameters_MethodWithNoParametersCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        model.IncrementCounter_WithParameter_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_WithParameters_MethodWithNoParameters()
    {
        CanExecuteViewModel model = new();

        model.Flag = true;

        model.IncrementCounter_WithParameters_MethodWithNoParametersCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);

        model.Flag = false;

        model.IncrementCounter_WithParameter_PropertyCommand.Execute(null);

        Assert.AreEqual(model.Counter, 1);
    }

    [TestMethod]
    public void Test_ICommandAttribute_CanExecute_WithParameters_MethodWithMatchingParameter()
    {
        CanExecuteViewModel model = new();

        model.IncrementCounter_WithParameters_MethodWithMatchingParameterCommand.Execute(new User { Name = nameof(CanExecuteViewModel) });

        Assert.AreEqual(model.Counter, 1);

        model.IncrementCounter_WithParameter_PropertyCommand.Execute(new User());

        Assert.AreEqual(model.Counter, 1);
    }

    public sealed partial class MyViewModel
    {
        public int Counter { get; private set; }

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
    }
    
    public sealed partial class CanExecuteViewModel
    {
        public int Counter { get; private set; }

        public bool Flag { get; set; }

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
    }

    public sealed class User
    {
        public string? Name { get; set; }
    }
}

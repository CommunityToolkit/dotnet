// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public class Test_RelayCommand
{
    [TestMethod]
    public void Test_RelayCommand_AlwaysEnabled()
    {
        int ticks = 0;

        RelayCommand? command = new(() => ticks++);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        (object?, EventArgs?) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        command.NotifyCanExecuteChanged();

        Assert.AreSame(args.Item1, command);
        Assert.AreSame(args.Item2, EventArgs.Empty);

        command.Execute(null);

        Assert.AreEqual(ticks, 1);

        command.Execute(new object());

        Assert.AreEqual(ticks, 2);
    }

    [TestMethod]
    public void Test_RelayCommand_WithCanExecuteFunctionTrue()
    {
        int ticks = 0;

        RelayCommand? command = new(() => ticks++, () => true);

        Assert.IsTrue(command.CanExecute(null));
        Assert.IsTrue(command.CanExecute(new object()));

        command.Execute(null);

        Assert.AreEqual(ticks, 1);

        command.Execute(new object());

        Assert.AreEqual(ticks, 2);
    }

    [TestMethod]
    public void Test_RelayCommand_WithCanExecuteFunctionFalse()
    {
        int ticks = 0;

        RelayCommand? command = new(() => ticks++, () => false);

        Assert.IsFalse(command.CanExecute(null));
        Assert.IsFalse(command.CanExecute(new object()));

        command.Execute(null);

        // Logic is unconditionally invoked, the caller should check CanExecute first
        Assert.AreEqual(ticks, 1);

        command.Execute(new object());

        Assert.AreEqual(ticks, 2);
    }
}

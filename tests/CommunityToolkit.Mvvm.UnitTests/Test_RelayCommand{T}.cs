// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public class Test_RelayCommandOfT
{
    [TestMethod]
    public void Test_RelayCommandOfT_AlwaysEnabled()
    {
        string? text = string.Empty;

        RelayCommand<string>? command = new(s => text = s);

        Assert.IsTrue(command.CanExecute("Text"));
        Assert.IsTrue(command.CanExecute(null));

        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.CanExecute(new object()), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.CanExecute(42), "parameter");
        
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute(new object()), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute(42), "parameter");

        (object?, EventArgs?) args = default;

        command.CanExecuteChanged += (s, e) => args = (s, e);

        command.NotifyCanExecuteChanged();

        Assert.AreSame(args.Item1, command);
        Assert.AreSame(args.Item2, EventArgs.Empty);

        command.Execute((object)"Hello");

        Assert.AreEqual(text, "Hello");

        command.Execute(null);

        Assert.AreEqual(text, null);
    }

    [TestMethod]
    public void Test_RelayCommand_WithCanExecuteFunction()
    {
        string? text = string.Empty;

        RelayCommand<string>? command = new(s => text = s, s => s != null);

        Assert.IsTrue(command.CanExecute("Text"));
        Assert.IsFalse(command.CanExecute(null));

        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.CanExecute(new object()), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.CanExecute(42), "parameter");
        
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute(new object()), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute(42), "parameter");

        command.Execute((object)"Hello");

        Assert.AreEqual(text, "Hello");

        command.Execute(null);

        // Logic is unconditionally invoked, the caller should check CanExecute first
        Assert.IsNull(text);
    }

    [TestMethod]
    public void Test_RelayCommand_InvalidArgumentWithValueType()
    {
        int n = 0;

        RelayCommand<int>? command = new(i => n = i);

        // Special case
        Assert.IsFalse(command.CanExecute(null));

        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.CanExecute("Hello"), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.CanExecute(3.14f), "parameter");

        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute(null), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute("Hello"), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute(3.14f), "parameter");
    }

    [TestMethod]
    public void Test_RelayCommand_InvalidArgumentWithValueType_WithCanExecute()
    {
        int n = 0;

        RelayCommand<int>? command = new(i => n = i, i => i > 0);

        // Special case
        Assert.IsFalse(command.CanExecute(null));

        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.CanExecute("Hello"), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.CanExecute(3.14f), "parameter");

        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute(null), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute("Hello"), "parameter");
        ExceptionHelper.ThrowsArgumentExceptionWithParameterName(() => command.Execute(3.14f), "parameter");
    }
}

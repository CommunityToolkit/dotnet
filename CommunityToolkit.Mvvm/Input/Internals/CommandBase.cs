// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CommunityToolkit.Mvvm.Input.Internals;

public abstract class CommandBase : IRelayCommand
{
    /// <inheritdoc/>
    private event EventHandler? canExecuteChanged;

    /// <summary>
    /// It's "virtual" for cases when extended event handling is needed.
    /// For example at WPF, to punt the implementation off to the CommandManager class:
    /// <see  add { CommandManager.RequerySuggested += value; }/>
    /// <see  remove { CommandManager.RequerySuggested -= value; }/>
    /// </summary>
    public virtual event EventHandler? CanExecuteChanged
    {
        add { canExecuteChanged += value; }
        remove { canExecuteChanged -= value; }
    }

    /// <inheritdoc/>
    void IRelayCommand.NotifyCanExecuteChanged()
    {
        canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public abstract bool CanExecute(object? parameter);

    /// <inheritdoc/>
    public abstract void Execute(object? parameter);
}
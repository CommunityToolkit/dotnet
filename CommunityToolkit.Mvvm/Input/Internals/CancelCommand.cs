// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.ComponentModel;
using System.Windows.Input;

namespace CommunityToolkit.Mvvm.Input.Internals;

/// <summary>
/// A <see cref="ICommand"/> implementation wrapping <see cref="IAsyncRelayCommand"/> to support cancellation.
/// </summary>
internal sealed class CancelCommand : ICommand
{
    /// <summary>
    /// The wrapped <see cref="IAsyncRelayCommand"/> instance.
    /// </summary>
    private readonly IAsyncRelayCommand command;

    /// <summary>
    /// Creates a new <see cref="CancelCommand"/> instance.
    /// </summary>
    /// <param name="command">The <see cref="IAsyncRelayCommand"/> instance to wrap.</param>
    public CancelCommand(IAsyncRelayCommand command)
    {
        this.command = command;

        this.command.PropertyChanged += OnPropertyChanged;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <inheritdoc/>
    public bool CanExecute(object? parameter)
    {
        return this.command.CanBeCanceled;
    }

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        this.command.Cancel();
    }

    /// <inheritdoc cref="PropertyChangedEventHandler"/>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null or nameof(IAsyncRelayCommand.CanBeCanceled))
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

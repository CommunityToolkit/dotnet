// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityToolkit.Mvvm.Input;

/// <summary>
/// A command that mirrors the functionality of <see cref="RelayCommand"/>, with the addition of
/// accepting a <see cref="Func{TResult}"/> returning a <see cref="Task"/> as the execute
/// action, and providing an <see cref="ExecutionTask"/> property that notifies changes when
/// <see cref="ExecuteAsync"/> is invoked and when the returned <see cref="Task"/> completes.
/// </summary>
public sealed class AsyncRelayCommand : IAsyncRelayCommand
{
    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="ExecutionTask"/>.
    /// </summary>
    internal static readonly PropertyChangedEventArgs ExecutionTaskChangedEventArgs = new(nameof(ExecutionTask));

    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="CanBeCanceled"/>.
    /// </summary>
    internal static readonly PropertyChangedEventArgs CanBeCanceledChangedEventArgs = new(nameof(CanBeCanceled));

    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IsCancellationRequested"/>.
    /// </summary>
    internal static readonly PropertyChangedEventArgs IsCancellationRequestedChangedEventArgs = new(nameof(IsCancellationRequested));

    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IsRunning"/>.
    /// </summary>
    internal static readonly PropertyChangedEventArgs IsRunningChangedEventArgs = new(nameof(IsRunning));

    /// <summary>
    /// The <see cref="Func{TResult}"/> to invoke when <see cref="Execute"/> is used.
    /// </summary>
    private readonly Func<Task>? execute;

    /// <summary>
    /// The cancelable <see cref="Func{T,TResult}"/> to invoke when <see cref="Execute"/> is used.
    /// </summary>
    /// <remarks>Only one between this and <see cref="execute"/> is not <see langword="null"/>.</remarks>
    private readonly Func<CancellationToken, Task>? cancelableExecute;

    /// <summary>
    /// The optional action to invoke when <see cref="CanExecute"/> is used.
    /// </summary>
    private readonly Func<bool>? canExecute;

    /// <summary>
    /// Indicates whether or not concurrent executions of the command are allowed.
    /// </summary>
    private readonly bool allowConcurrentExecutions;

    /// <summary>
    /// The <see cref="CancellationTokenSource"/> instance to use to cancel <see cref="cancelableExecute"/>.
    /// </summary>
    /// <remarks>This is only used when <see cref="cancelableExecute"/> is not <see langword="null"/>.</remarks>
    private CancellationTokenSource? cancellationTokenSource;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<Task> execute)
    {
        ArgumentNullException.ThrowIfNull(execute);

        this.execute = execute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="allowConcurrentExecutions">Whether or not to allow concurrent executions of the command.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<Task> execute, bool allowConcurrentExecutions)
    {
        ArgumentNullException.ThrowIfNull(execute);

        this.execute = execute;
        this.allowConcurrentExecutions = allowConcurrentExecutions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class that can always execute.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> is <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute)
    {
        ArgumentNullException.ThrowIfNull(cancelableExecute);

        this.cancelableExecute = cancelableExecute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class that can always execute.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="allowConcurrentExecutions">Whether or not to allow concurrent executions of the command.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> is <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, bool allowConcurrentExecutions)
    {
        ArgumentNullException.ThrowIfNull(cancelableExecute);

        this.cancelableExecute = cancelableExecute;
        this.allowConcurrentExecutions = allowConcurrentExecutions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
    {
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentNullException.ThrowIfNull(canExecute);

        this.execute = execute;
        this.canExecute = canExecute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <param name="allowConcurrentExecutions">Whether or not to allow concurrent executions of the command.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, bool allowConcurrentExecutions)
    {
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentNullException.ThrowIfNull(canExecute);

        this.execute = execute;
        this.canExecute = canExecute;
        this.allowConcurrentExecutions = allowConcurrentExecutions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
    {
        ArgumentNullException.ThrowIfNull(cancelableExecute);
        ArgumentNullException.ThrowIfNull(canExecute);

        this.cancelableExecute = cancelableExecute;
        this.canExecute = canExecute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <param name="allowConcurrentExecutions">Whether or not to allow concurrent executions of the command.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute, bool allowConcurrentExecutions)
    {
        ArgumentNullException.ThrowIfNull(cancelableExecute);
        ArgumentNullException.ThrowIfNull(canExecute);

        this.cancelableExecute = cancelableExecute;
        this.canExecute = canExecute;
        this.allowConcurrentExecutions = allowConcurrentExecutions;
    }

    private Task? executionTask;

    /// <inheritdoc/>
    public Task? ExecutionTask
    {
        get => this.executionTask;
        private set
        {
            if (ReferenceEquals(this.executionTask, value))
            {
                return;
            }

            this.executionTask = value;

            PropertyChanged?.Invoke(this, ExecutionTaskChangedEventArgs);
            PropertyChanged?.Invoke(this, IsRunningChangedEventArgs);
            PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);

            if (value?.IsCompleted ?? true)
            {
                return;
            }

            static async void MonitorTask(AsyncRelayCommand @this, Task task)
            {
                try
                {
                    await task;
                }
                catch
                {
                }

                if (ReferenceEquals(@this.executionTask, task))
                {
                    @this.PropertyChanged?.Invoke(@this, ExecutionTaskChangedEventArgs);
                    @this.PropertyChanged?.Invoke(@this, IsRunningChangedEventArgs);
                    @this.PropertyChanged?.Invoke(@this, CanBeCanceledChangedEventArgs);
                }
            }

            MonitorTask(this, value!);
        }
    }

    /// <inheritdoc/>
    public bool CanBeCanceled => this.cancelableExecute is not null && IsRunning;

    /// <inheritdoc/>
    public bool IsCancellationRequested => this.cancellationTokenSource?.IsCancellationRequested == true;

    /// <inheritdoc/>
    public bool IsRunning => ExecutionTask?.IsCompleted == false;

    /// <inheritdoc/>
    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(object? parameter)
    {
        bool canExecute = this.canExecute?.Invoke() != false;

        return canExecute && (this.allowConcurrentExecutions || ExecutionTask?.IsCompleted != false);
    }

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        _ = ExecuteAsync(parameter);
    }

    /// <inheritdoc/>
    public Task ExecuteAsync(object? parameter)
    {
        if (CanExecute(parameter))
        {
            // Non cancelable command delegate
            if (this.execute is not null)
            {
                return ExecutionTask = this.execute();
            }

            // Cancel the previous operation, if one is pending
            this.cancellationTokenSource?.Cancel();

            CancellationTokenSource cancellationTokenSource = this.cancellationTokenSource = new();

            PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);

            // Invoke the cancelable command delegate with a new linked token
            return ExecutionTask = this.cancelableExecute!(cancellationTokenSource.Token);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Cancel()
    {
        this.cancellationTokenSource?.Cancel();

        PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);
        PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);
    }
}

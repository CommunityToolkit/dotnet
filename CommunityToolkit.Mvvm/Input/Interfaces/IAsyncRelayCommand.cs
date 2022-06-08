// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading.Tasks;

namespace CommunityToolkit.Mvvm.Input;

/// <summary>
/// An interface expanding <see cref="IRelayCommand"/> to support asynchronous operations.
/// </summary>
public interface IAsyncRelayCommand : IRelayCommand, INotifyPropertyChanged
{
    /// <summary>
    /// Gets the last scheduled <see cref="Task"/>, if available.
    /// This property notifies a change when the <see cref="Task"/> completes.
    /// </summary>
    Task? ExecutionTask { get; }

    /// <summary>
    /// Gets a value indicating whether a running operation for this command can currently be canceled.
    /// </summary>
    /// <remarks>
    /// The exact sequence of events that types implementing this interface should raise is as follows:
    /// <list type="bullet">
    /// <item>
    /// The command is initially not running: <see cref="IsRunning"/>, <see cref="CanBeCanceled"/>
    /// and <see cref="IsCancellationRequested"/> are <see langword="false"/>.
    /// </item>
    /// <item>
    /// The command starts running: <see cref="IsRunning"/> and <see cref="CanBeCanceled"/> switch to
    /// <see langword="true"/>. <see cref="IsCancellationRequested"/> is set to <see langword="false"/>.
    /// </item>
    /// <item>
    /// If the operation is canceled: <see cref="CanBeCanceled"/> switches to <see langword="false"/>
    /// and <see cref="IsCancellationRequested"/> switches to <see langword="true"/>.
    /// </item>
    /// <item>
    /// The operation completes: <see cref="IsRunning"/> and <see cref="CanBeCanceled"/> switch
    /// to <see langword="false"/>. The state of <see cref="IsCancellationRequested"/> is undefined.
    /// </item>
    /// </list>
    /// This only applies if the underlying logic for the command actually supports cancelation. If that is
    /// not the case, then <see cref="CanBeCanceled"/> and <see cref="IsCancellationRequested"/> will always remain
    /// <see langword="false"/> regardless of the current state of the command.
    /// </remarks>
    bool CanBeCanceled { get; }

    /// <summary>
    /// Gets a value indicating whether a cancelation request has been issued for the current operation.
    /// </summary>
    bool IsCancellationRequested { get; }

    /// <summary>
    /// Gets a value indicating whether the command currently has a pending operation being executed.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Provides a more specific version of <see cref="System.Windows.Input.ICommand.Execute"/>,
    /// also returning the <see cref="Task"/> representing the async operation being executed.
    /// </summary>
    /// <param name="parameter">The input parameter.</param>
    /// <returns>The <see cref="Task"/> representing the async operation being executed.</returns>
    /// <exception cref="System.ArgumentException">Thrown if <paramref name="parameter"/> is incompatible with the underlying command implementation.</exception>
    Task ExecuteAsync(object? parameter);

    /// <summary>
    /// Communicates a request for cancelation.
    /// </summary>
    /// <remarks>
    /// If the underlying command is not running, or if it does not support cancelation, this method will perform no action.
    /// Note that even with a successful cancelation, the completion of the current operation might not be immediate.
    /// </remarks>
    void Cancel();
}

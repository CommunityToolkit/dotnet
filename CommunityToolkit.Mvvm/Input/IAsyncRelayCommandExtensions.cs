// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
using CommunityToolkit.Mvvm.Input.Internals;

namespace CommunityToolkit.Mvvm.Input;

/// <summary>
/// Extensions for the <see cref="IAsyncRelayCommand"/> type.
/// </summary>
public static class IAsyncRelayCommandExtensions
{
    /// <summary>
    /// Creates an <see cref="ICommand"/> instance that can be used to cancel execution on the input command.
    /// The returned command will also notify when it can be executed based on the state of the wrapped command.
    /// </summary>
    /// <param name="command">The input <see cref="IAsyncRelayCommand"/> instance to create a cancellation command for.</param>
    /// <returns>An <see cref="ICommand"/> instance that can be used to monitor and signal cancellation for <paramref name="command"/>.</returns>
    /// <remarks>The returned instance is not guaranteed to be unique across multiple invocations with the same arguments.</remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="command"/> is <see langword="null"/>.</exception>
    public static ICommand CreateCancelCommand(this IAsyncRelayCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        // If the command is known not to ever allow cancellation, just reuse the same instance
        if (command is ICancellationAwareCommand { IsCancellationSupported: false })
        {
            return DisabledCommand.Instance;
        }

        // Create a new cancel command wrapping the input one
        return new CancelCommand(command);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA1063

namespace CommunityToolkit.Common.Deferred;

/// <summary>
/// Deferral handle provided by a <see cref="DeferredEventArgs"/>.
/// </summary>
public class EventDeferral : IDisposable
{
#if NET8_0_OR_GREATER
    private readonly TaskCompletionSource taskCompletionSource = new();
#else
    private readonly TaskCompletionSource<object?> taskCompletionSource = new();
#endif

    internal EventDeferral()
    {
    }

    /// <summary>
    /// Call when finished with the Deferral.
    /// </summary>
    public void Complete()
    {
#if NET8_0_OR_GREATER
        this.taskCompletionSource.TrySetResult();
#else
        this.taskCompletionSource.TrySetResult(null);
#endif
    }

    /// <summary>
    /// Waits for the <see cref="EventDeferral"/> to be completed by the event handler.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This is an internal only method to be used by EventHandler extension classes, public callers should call GetDeferral() instead on the DeferredEventArgs.")]
    public async Task WaitForCompletion(CancellationToken cancellationToken)
    {
        using (cancellationToken.Register(
#if NET8_0_OR_GREATER
            callback: static obj => Unsafe.As<EventDeferral>(obj!).taskCompletionSource.TrySetCanceled(),
#else
            callback: static obj => ((EventDeferral)obj).taskCompletionSource.TrySetCanceled(),
#endif
            state: this))
        {
#if NET8_0_OR_GREATER
            await this.taskCompletionSource.Task;
#else
            _ = await this.taskCompletionSource.Task;
#endif
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Complete();
    }
}

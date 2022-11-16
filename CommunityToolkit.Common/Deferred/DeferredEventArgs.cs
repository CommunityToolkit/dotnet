// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.ComponentModel;

#pragma warning disable CA1001

namespace CommunityToolkit.Common.Deferred;

/// <summary>
/// <see cref="EventArgs"/> which can retrieve a <see cref="EventDeferral"/> in order to process data asynchronously before an <see cref="EventHandler"/> completes and returns to the calling control.
/// </summary>
public class DeferredEventArgs : EventArgs
{
    /// <summary>
    /// Gets a new <see cref="DeferredEventArgs"/> to use in cases where no <see cref="EventArgs"/> wish to be provided.
    /// </summary>
    public static new DeferredEventArgs Empty => new();

    private readonly object eventDeferralLock = new();

    private EventDeferral? eventDeferral;

    /// <summary>
    /// Returns an <see cref="EventDeferral"/> which can be completed when deferred event is ready to continue.
    /// </summary>
    /// <returns><see cref="EventDeferral"/> instance.</returns>
    public EventDeferral GetDeferral()
    {
        lock (this.eventDeferralLock)
        {
            return this.eventDeferral ??= new EventDeferral();
        }
    }

    /// <summary>
    /// DO NOT USE - This is a support method used by <see cref="EventHandlerExtensions"/>. It is public only for
    /// additional usage within extensions for the UWP based TypedEventHandler extensions.
    /// </summary>
    /// <returns>Internal EventDeferral reference</returns>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This is an internal only method to be used by EventHandler extension classes, public callers should call GetDeferral() instead.")]
    public EventDeferral? GetCurrentDeferralAndReset()
    {
        lock (this.eventDeferralLock)
        {
            EventDeferral? eventDeferral = this.eventDeferral;

            this.eventDeferral = null;

            return eventDeferral;
        }
    }
}

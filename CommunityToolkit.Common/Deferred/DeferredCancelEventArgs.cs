// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

namespace CommunityToolkit.Common.Deferred;

/// <summary>
/// <see cref="DeferredEventArgs"/> which can also be canceled.
/// </summary>
public class DeferredCancelEventArgs : DeferredEventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether the event should be canceled.
    /// </summary>
    public bool Cancel { get; set; }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

namespace CommunityToolkit.Mvvm.Input.Internals;

/// <summary>
/// An interface for commands that know whether they support cancellation or not.
/// </summary>
internal interface ICancellationAwareCommand
{
    /// <summary>
    /// Gets whether or not the current command supports cancellation.
    /// </summary>
    bool IsCancellationSupported { get; }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

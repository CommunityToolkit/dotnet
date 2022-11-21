// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace CommunityToolkit.Mvvm.Collections.Internals;

/// <summary>
/// A helper type for the <see cref="ObservableGroup{TKey, TValue}"/> type.
/// </summary>
internal static class ObservableGroupHelper
{
    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IReadOnlyObservableGroup.Key"/>
    /// </summary>
    public static readonly PropertyChangedEventArgs KeyChangedEventArgs = new(nameof(IReadOnlyObservableGroup.Key));
}
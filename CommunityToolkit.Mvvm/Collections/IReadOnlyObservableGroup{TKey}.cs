// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// An interface for a grouped collection of items.
/// </summary>
/// <typeparam name="TKey">The type of the group key.</typeparam>
public interface IReadOnlyObservableGroup<out TKey> : IReadOnlyObservableGroup
    where TKey : notnull
{
    /// <summary>
    /// Gets the key for the current collection.
    /// </summary>
    new TKey Key { get; }
}

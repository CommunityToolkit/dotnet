// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// An interface for a grouped collection of items.
/// </summary>
public interface IReadOnlyObservableGroup : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the key for the current collection.
    /// </summary>
    object Key { get; }

    /// <summary>
    /// Gets the number of items currently in the grouped collection.
    /// </summary>
    int Count { get; }
}

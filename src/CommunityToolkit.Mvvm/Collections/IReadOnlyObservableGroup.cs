// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// An interface for a grouped collection of items.
/// </summary>
public interface IReadOnlyObservableGroup : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable
{
    /// <summary>
    /// Gets the key for the current collection.
    /// </summary>
    object Key { get; }

    /// <summary>
    /// Gets the number of items currently in the grouped collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element at the specified index in the current collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index in the read-only list.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
    object? this[int index] { get; }
}

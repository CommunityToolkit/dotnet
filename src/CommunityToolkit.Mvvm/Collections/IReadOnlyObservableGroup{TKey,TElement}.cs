// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// An interface for a grouped collection of items.
/// </summary>
/// <typeparam name="TKey">The type of the group key.</typeparam>
/// <typeparam name="TElement">The type of elements in the group.</typeparam>
public interface IReadOnlyObservableGroup<out TKey, out TElement> : IReadOnlyObservableGroup<TKey>, IReadOnlyList<TElement>, IGrouping<TKey, TElement>
    where TKey : notnull
{
    /// <summary>
    /// Gets the element at the specified index in the current collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index in the read-only list.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
    new TElement this[int index] { get; }
}

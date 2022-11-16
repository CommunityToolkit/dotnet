// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// An observable list of observable groups.
/// </summary>
/// <typeparam name="TKey">The type of the group keys.</typeparam>
/// <typeparam name="TElement">The type of elements in the collection.</typeparam>
public sealed class ObservableGroupedCollection<TKey, TElement> : ObservableCollection<ObservableGroup<TKey, TElement>>, ILookup<TKey, TElement>
    where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGroupedCollection{TKey, TValue}"/> class.
    /// </summary>
    public ObservableGroupedCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGroupedCollection{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="collection">The initial data to add in the grouped collection.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="collection"/> is <see langword="null"/>.</exception>
    public ObservableGroupedCollection(IEnumerable<IGrouping<TKey, TElement>> collection)
        : base(collection?.Select(static group => new ObservableGroup<TKey, TElement>(group))!)
    {
    }

    /// <inheritdoc/>
    IEnumerable<TElement> ILookup<TKey, TElement>.this[TKey key]
    {
        get
        {
            IEnumerable<TElement>? result = null;

            if (key is not null)
            {
                result = this.FirstGroupByKeyOrDefault(key);
            }

            return result ?? Enumerable.Empty<TElement>();
        }
    }

    /// <summary>
    /// Tries to get the underlying <see cref="List{T}"/> instance, if present.
    /// </summary>
    /// <param name="list">The resulting <see cref="List{T}"/>, if one was in use.</param>
    /// <returns>Whether or not a <see cref="List{T}"/> instance has been found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetList([NotNullWhen(true)] out List<ObservableGroup<TKey, TElement>>? list)
    {
        list = Items as List<ObservableGroup<TKey, TElement>>;

        return list is not null;
    }

    /// <inheritdoc/>
    bool ILookup<TKey, TElement>.Contains(TKey key)
    {
        return key is not null && this.FirstGroupByKey(key) is not null;
    }

    /// <inheritdoc/>
    IEnumerator<IGrouping<TKey, TElement>> IEnumerable<IGrouping<TKey, TElement>>.GetEnumerator()
    {
        return GetEnumerator();
    }
}

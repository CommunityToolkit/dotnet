// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// A read-only list of groups.
/// </summary>
/// <typeparam name="TKey">The type of the group keys.</typeparam>
/// <typeparam name="TElement">The type of elements in the collection.</typeparam>
public sealed class ReadOnlyObservableGroupedCollection<TKey, TElement> : ReadOnlyObservableCollection<ReadOnlyObservableGroup<TKey, TElement>>
    where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyObservableGroupedCollection{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="collection">The source collection to wrap.</param>
    public ReadOnlyObservableGroupedCollection(ObservableCollection<ObservableGroup<TKey, TElement>> collection)
        : base(new ObservableCollection<ReadOnlyObservableGroup<TKey, TElement>>(collection.Select(static g => new ReadOnlyObservableGroup<TKey, TElement>(g))))
    {
        collection.CollectionChanged += OnSourceCollectionChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyObservableGroupedCollection{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="collection">The source collection to wrap.</param>
    public ReadOnlyObservableGroupedCollection(ObservableCollection<ReadOnlyObservableGroup<TKey, TElement>> collection)
        : base(collection)
    {
    }

    /// <summary>
    /// Forwards the <see cref="INotifyCollectionChanged.CollectionChanged"/> event whenever it is raised by the wrapped collection.
    /// </summary>
    /// <param name="sender">The wrapped collection (an <see cref="ObservableCollection{T}"/> of <see cref="ReadOnlyObservableGroup{TKey, TValue}"/> instance).</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> arguments.</param>
    /// <exception cref="NotSupportedException">Thrown if a range operation is requested.</exception>
    private void OnSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Even if NotifyCollectionChangedEventArgs allows multiple items, the actual implementation
        // is only reporting the changes one by one. We consider only this case for now. If this is
        // added in a new version of .NET, this type will need to be updated accordingly in a new version.
        [DoesNotReturn]
        static void ThrowNotSupportedExceptionForRangeOperation()
        {
            throw new NotSupportedException(
                "ReadOnlyObservableGroupedCollection<TKey, TValue> doesn't support operations on multiple items at once.\n" +
                "If this exception was thrown, it likely means support for batched item updates has been added to the " +
                "underlying ObservableCollection<T> type, and this implementation doesn't support that feature yet.\n" +
                "Please consider opening an issue in https://aka.ms/toolkit/dotnet to report this.");
        }

        // The inner Items list is ObservableCollection<ReadOnlyObservableGroup<TKey, TValue>>, so doing a direct cast here will always succeed
        ObservableCollection<ReadOnlyObservableGroup<TKey, TElement>> items = (ObservableCollection<ReadOnlyObservableGroup<TKey, TElement>>)Items;

        switch (e.Action)
        {
            // Insert a single item for an "Add" operation, fail if multiple items are added
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems!.Count == 1)
                {
                    ObservableGroup<TKey, TElement> newItem = (ObservableGroup<TKey, TElement>)e.NewItems![0]!;

                    items.Insert(e.NewStartingIndex, new ReadOnlyObservableGroup<TKey, TElement>(newItem));
                }
                else if (e.NewItems!.Count > 1)
                {
                    ThrowNotSupportedExceptionForRangeOperation();
                }

                break;

            // Remove a single item at offset for a "Remove" operation, fail if multiple items are removed
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems!.Count == 1)
                {
                    items.RemoveAt(e.OldStartingIndex);
                }
                else if (e.OldItems!.Count > 1)
                {
                    ThrowNotSupportedExceptionForRangeOperation();
                }

                break;

            // Replace a single item at offset for a "Replace" operation, fail if multiple items are replaced
            case NotifyCollectionChangedAction.Replace:
                if (e.OldItems!.Count == 1 && e.NewItems!.Count == 1)
                {
                    ObservableGroup<TKey, TElement> replacedItem = (ObservableGroup<TKey, TElement>)e.NewItems![0]!;

                    items[e.OldStartingIndex] = new ReadOnlyObservableGroup<TKey, TElement>(replacedItem);
                }
                else if (e.OldItems!.Count > 1 || e.NewItems!.Count > 1)
                {
                    ThrowNotSupportedExceptionForRangeOperation();
                }

                break;

            // Move a single item between offsets for a "Move" operation, fail if multiple items are moved
            case NotifyCollectionChangedAction.Move:
                if (e.OldItems!.Count == 1 && e.NewItems!.Count == 1)
                {
                    items.Move(e.OldStartingIndex, e.NewStartingIndex);
                }
                else if (e.OldItems!.Count > 1 || e.NewItems!.Count > 1)
                {
                    ThrowNotSupportedExceptionForRangeOperation();
                }

                break;

            // A "Reset" operation is just forwarded normally
            case NotifyCollectionChangedAction.Reset:
                items.Clear();
                break;
            default:
                break;
        }
    }
}

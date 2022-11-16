// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// A read-only observable group. It associates a <see cref="Key"/> to a <see cref="ReadOnlyObservableCollection{T}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the group key.</typeparam>
/// <typeparam name="TElement">The type of elements in the group.</typeparam>
[DebuggerDisplay("Key = {Key}, Count = {Count}")]
public sealed class ReadOnlyObservableGroup<TKey, TElement> : ReadOnlyObservableCollection<TElement>, IReadOnlyObservableGroup<TKey, TElement>
    where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyObservableGroup{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="key">The key of the group.</param>
    /// <param name="collection">The collection of items to add in the group.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="collection"/> are <see langword="null"/>.</exception>
    public ReadOnlyObservableGroup(TKey key, ObservableCollection<TElement> collection)
        : base(collection)
    {
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        Key = key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyObservableGroup{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="group">The <see cref="ObservableGroup{TKey, TValue}"/> to wrap.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="group"/> is <see langword="null"/>.</exception>
    public ReadOnlyObservableGroup(ObservableGroup<TKey, TElement> group)
        : base(group)
    {
        Key = group.Key;
    }

    /// <inheritdoc/>
    public TKey Key { get; }

    /// <inheritdoc/>
    object IReadOnlyObservableGroup.Key => Key;

    /// <inheritdoc/>
    object? IReadOnlyObservableGroup.this[int index] => this[index];
}

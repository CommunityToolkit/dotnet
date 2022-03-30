// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Collections.Internals;

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// An observable group.
/// It associates a <see cref="Key"/> to an <see cref="ObservableCollection{T}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the group key.</typeparam>
/// <typeparam name="TElement">The type of elements in the group.</typeparam>
[DebuggerDisplay("Key = {Key}, Count = {Count}")]
public sealed class ObservableGroup<TKey, TElement> : ObservableCollection<TElement>, IReadOnlyObservableGroup<TKey, TElement>
    where TKey : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGroup{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="key">The key for the group.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="key"/> is <see langword="null"/>.</exception>
    public ObservableGroup(TKey key)
    {
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        this.key = key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGroup{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="grouping">The grouping to fill the group.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="grouping"/> is <see langword="null"/>.</exception>
    public ObservableGroup(IGrouping<TKey, TElement> grouping)
        : base(grouping)
    {
        this.key = grouping.Key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGroup{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="key">The key for the group.</param>
    /// <param name="collection">The initial collection of data to add to the group.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="collection"/> are <see langword="null"/>.</exception>
    public ObservableGroup(TKey key, IEnumerable<TElement> collection)
        : base(collection)
    {
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        this.key = key;
    }

    private TKey key;

    /// <summary>
    /// Gets or sets the key of the group.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/>.</exception>
    public TKey Key
    {
        get => this.key;
        set
        {
            ArgumentNullException.For<TKey>.ThrowIfNull(value);

            if (!EqualityComparer<TKey>.Default.Equals(this.key!, value))
            {
                this.key = value;

                OnPropertyChanged(ObservableGroupHelper.KeyChangedEventArgs);
            }
        }
    }

    /// <summary>
    /// Tries to get the underlying <see cref="List{T}"/> instance, if present.
    /// </summary>
    /// <param name="list">The resulting <see cref="List{T}"/>, if one was in use.</param>
    /// <returns>Whether or not a <see cref="List{T}"/> instance has been found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetList([NotNullWhen(true)] out List<TElement>? list)
    {
        list = Items as List<TElement>;

        return list is not null;
    }

    /// <inheritdoc/>
    object IReadOnlyObservableGroup.Key => Key;

    /// <inheritdoc/>
    object? IReadOnlyObservableGroup.this[int index] => this[index];
}

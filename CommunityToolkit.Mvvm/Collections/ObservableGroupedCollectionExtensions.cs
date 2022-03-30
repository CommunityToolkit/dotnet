// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Mvvm.Collections;

/// <summary>
/// The extensions methods to simplify the usage of <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
/// </summary>
public static class ObservableGroupedCollectionExtensions
{
    /// <summary>
    /// Return the first group with <paramref name="key"/> key.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group to query.</param>
    /// <returns>The first group matching <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="key"/> are <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The target group does not exist.</exception>
    public static ObservableGroup<TKey, TElement> FirstGroupByKey<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        ObservableGroup<TKey, TElement>? group = source.FirstGroupByKeyOrDefault(key);

        if (group is null)
        {
            [DoesNotReturn]
            static void ThrowArgumentExceptionForKeyNotFound()
            {
                throw new InvalidOperationException("The requested key was not present in the collection.");
            }

            ThrowArgumentExceptionForKeyNotFound();
        }

        return group;
    }

    /// <summary>
    /// Return the first group with <paramref name="key"/> key or null if not found.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group to query.</param>
    /// <returns>The first group matching <paramref name="key"/> or null.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="key"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement>? FirstGroupByKeyOrDefault<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        if (source.TryGetList(out List<ObservableGroup<TKey, TElement>>? list))
        {
            foreach (ObservableGroup<TKey, TElement>? group in list)
            {
                if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                {
                    return group;
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static ObservableGroup<TKey, TElement>? FirstOrDefaultFallback(ObservableGroupedCollection<TKey, TElement> source, TKey key)
        {
            return Enumerable.FirstOrDefault<ObservableGroup<TKey, TElement>>(source, group => EqualityComparer<TKey>.Default.Equals(group.Key, key));
        }

        return FirstOrDefaultFallback(source, key);
    }

    /// <summary>
    /// Adds a key-value <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="key"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> AddGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        ObservableGroup<TKey, TElement> group = new(key);

        source.Add(group);

        return group;
    }

    /// <summary>
    /// Adds a key-collection <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="grouping">The group of items to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="grouping"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> AddGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, IGrouping<TKey, TElement> grouping)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(grouping);

        ObservableGroup<TKey, TElement> group = new(grouping);

        source.Add(group);

        return group;
    }

    /// <summary>
    /// Adds a key-collection <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group where <paramref name="collection"/> will be added.</param>
    /// <param name="collection">The collection to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TElement}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/>, <paramref name="key"/> or <paramref name="collection"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> AddGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key, IEnumerable<TElement> collection)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(collection);

        ObservableGroup<TKey, TElement> group = new(key, collection);

        source.Add(group);

        return group;
    }

    /// <summary>
    /// Adds a key-value <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="key"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> InsertGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        if (source.TryGetList(out List<ObservableGroup<TKey, TElement>>? list))
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in list)
            {
                if (Comparer<TKey>.Default.Compare(key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(key);

            source.Insert(index, newGroup);

            return newGroup;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static ObservableGroup<TKey, TElement> InsertGroupFallback(ObservableGroupedCollection<TKey, TElement> source, TKey key)
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in source)
            {
                if (Comparer<TKey>.Default.Compare(key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(key);

            source.Insert(index, newGroup);

            return newGroup;
        }

        return InsertGroupFallback(source, key);
    }

    /// <summary>
    /// Adds a key-value <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="grouping">The group of items to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="grouping"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> InsertGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, IGrouping<TKey, TElement> grouping)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(grouping);

        if (source.TryGetList(out List<ObservableGroup<TKey, TElement>>? list))
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in list)
            {
                if (Comparer<TKey>.Default.Compare(grouping.Key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(grouping);

            source.Insert(index, newGroup);

            return newGroup;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static ObservableGroup<TKey, TElement> InsertGroupFallback(ObservableGroupedCollection<TKey, TElement> source, IGrouping<TKey, TElement> grouping)
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in source)
            {
                if (Comparer<TKey>.Default.Compare(grouping.Key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(grouping);

            source.Insert(index, newGroup);

            return newGroup;
        }

        return InsertGroupFallback(source, grouping);
    }

    /// <summary>
    /// Adds a key-value <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group where <paramref name="collection"/> will be added.</param>
    /// <param name="collection">The collection to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/>, <paramref name="key"/> or <paramref name="collection"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> InsertGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key, IEnumerable<TElement> collection)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(collection);

        if (source.TryGetList(out List<ObservableGroup<TKey, TElement>>? list))
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in list)
            {
                if (Comparer<TKey>.Default.Compare(key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(key, collection);

            source.Insert(index, newGroup);

            return newGroup;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static ObservableGroup<TKey, TElement> InsertGroupFallback(ObservableGroupedCollection<TKey, TElement> source, TKey key, IEnumerable<TElement> collection)
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in source)
            {
                if (Comparer<TKey>.Default.Compare(key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(key, collection);

            source.Insert(index, newGroup);

            return newGroup;
        }

        return InsertGroupFallback(source, key, collection);
    }

    /// <summary>
    /// Adds a key-value <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group to add.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> instance to insert <typeparamref name="TKey"/> at the right position.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/>, <paramref name="key"/> or <paramref name="comparer"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> InsertGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key, IComparer<TKey> comparer)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(comparer);

        if (source.TryGetList(out List<ObservableGroup<TKey, TElement>>? list))
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in list)
            {
                if (comparer.Compare(key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(key);

            source.Insert(index, newGroup);

            return newGroup;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static ObservableGroup<TKey, TElement> InsertGroupFallback(ObservableGroupedCollection<TKey, TElement> source, TKey key, IComparer<TKey> comparer)
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in source)
            {
                if (comparer.Compare(key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(key);

            source.Insert(index, newGroup);

            return newGroup;
        }

        return InsertGroupFallback(source, key, comparer);
    }

    /// <summary>
    /// Adds a key-value <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="grouping">The group of items to add.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> instance to insert <typeparamref name="TKey"/> at the right position.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/>, <paramref name="grouping"/> or <paramref name="comparer"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> InsertGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, IGrouping<TKey, TElement> grouping, IComparer<TKey> comparer)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(grouping);
        ArgumentNullException.ThrowIfNull(comparer);

        if (source.TryGetList(out List<ObservableGroup<TKey, TElement>>? list))
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in list)
            {
                if (comparer.Compare(grouping.Key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(grouping);

            source.Insert(index, newGroup);

            return newGroup;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static ObservableGroup<TKey, TElement> InsertGroupFallback(ObservableGroupedCollection<TKey, TElement> source, IGrouping<TKey, TElement> grouping, IComparer<TKey> comparer)
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in source)
            {
                if (comparer.Compare(grouping.Key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(grouping);

            source.Insert(index, newGroup);

            return newGroup;
        }

        return InsertGroupFallback(source, grouping, comparer);
    }

    /// <summary>
    /// Adds a key-value <see cref="ObservableGroup{TKey, TElement}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group where <paramref name="collection"/> will be added.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> instance to insert <typeparamref name="TKey"/> at the right position.</param>
    /// <param name="collection">The collection to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/>, <paramref name="key"/>, <paramref name="comparer"/> or <paramref name="collection"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> InsertGroup<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key, IComparer<TKey> comparer, IEnumerable<TElement> collection)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(collection);

        if (source.TryGetList(out List<ObservableGroup<TKey, TElement>>? list))
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in list)
            {
                if (comparer.Compare(key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(key, collection);

            source.Insert(index, newGroup);

            return newGroup;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static ObservableGroup<TKey, TElement> InsertGroupFallback(ObservableGroupedCollection<TKey, TElement> source, TKey key, IComparer<TKey> comparer, IEnumerable<TElement> collection)
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TElement> group in source)
            {
                if (comparer.Compare(key, group.Key) < 0)
                {
                    break;
                }

                index++;
            }

            ObservableGroup<TKey, TElement> newGroup = new(key, collection);

            source.Insert(index, newGroup);

            return newGroup;
        }

        return InsertGroupFallback(source, key, comparer, collection);
    }

    /// <summary>
    /// Add <paramref name="item"/> into the first group with <paramref name="key"/> key.
    /// If the group does not exist, it will be added.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group where the <paramref name="item"/> should be added.</param>
    /// <param name="item">The item to add.</param>
    /// <returns>The instance of the <see cref="ObservableGroup{TKey, TElement}"/> which will receive the value. It will either be an existing group or a new group.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="key"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> AddItem<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key, TElement item)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        ObservableGroup<TKey, TElement>? group = source.FirstGroupByKeyOrDefault(key);

        if (group is null)
        {
            group = new ObservableGroup<TKey, TElement>(key) { item };

            source.Add(group);
        }
        else
        {
            group.Add(item);
        }

        return group;
    }

    /// <summary>
    /// Insert <paramref name="item"/> into the first group with <paramref name="key"/> key.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group where to insert <paramref name="item"/>.</param>
    /// <param name="item">The item to add.</param>
    /// <returns>The instance of the <see cref="ObservableGroup{TKey, TElement}"/> which will receive the value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="key"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> InsertItem<TKey, TElement>(this ObservableGroupedCollection<TKey, TElement> source, TKey key, TElement item)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        ObservableGroup<TKey, TElement>? group = source.FirstGroupByKeyOrDefault(key);

        if (group is null)
        {
            group = source.InsertGroup(key, new[] { item });
        }
        else
        {
            if (group.TryGetList(out List<TElement>? list))
            {
                int index = 0;

                foreach (TElement element in list)
                {
                    if (Comparer<TElement>.Default.Compare(item, element) < 0)
                    {
                        break;
                    }

                    index++;
                }

                group.Insert(index, item);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            static void InsertItemFallback(ObservableCollection<TElement> source, TElement item)
            {
                int index = 0;

                foreach (TElement element in source)
                {
                    if (Comparer<TElement>.Default.Compare(item, element) < 0)
                    {
                        break;
                    }

                    index++;
                }

                source.Insert(index, item);
            }

            InsertItemFallback(group, item);
        }

        return group;
    }

    /// <summary>
    /// Insert <paramref name="item"/> into the first group with <paramref name="key"/> key.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TElement">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TElement}"/> instance.</param>
    /// <param name="key">The key of the group where to insert <paramref name="item"/>.</param>
    /// <param name="keyComparer">The <see cref="IComparer{T}"/> instance to compare keys.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="itemComparer">The <see cref="IComparer{T}"/> instance to compare elements.</param>
    /// <returns>The instance of the <see cref="ObservableGroup{TKey, TElement}"/> which will receive the value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/>, <paramref name="key"/>, <paramref name="keyComparer"/> or <paramref name="itemComparer"/> are <see langword="null"/>.</exception>
    public static ObservableGroup<TKey, TElement> InsertItem<TKey, TElement>(
        this ObservableGroupedCollection<TKey, TElement> source,
        TKey key,
        IComparer<TKey> keyComparer,
        TElement item,
        IComparer<TElement> itemComparer)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(keyComparer);
        ArgumentNullException.ThrowIfNull(itemComparer);

        ObservableGroup<TKey, TElement>? group = source.FirstGroupByKeyOrDefault(key);

        if (group is null)
        {
            group = source.InsertGroup(key, keyComparer, new[] { item });
        }
        else
        {
            if (group.TryGetList(out List<TElement>? list))
            {
                int index = 0;

                foreach (TElement element in list)
                {
                    if (itemComparer.Compare(item, element) < 0)
                    {
                        break;
                    }

                    index++;
                }

                group.Insert(index, item);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            static void InsertItemFallback(ObservableCollection<TElement> source, TElement item, IComparer<TElement> comparer)
            {
                int index = 0;

                foreach (TElement element in source)
                {
                    if (comparer.Compare(item, element) < 0)
                    {
                        break;
                    }

                    index++;
                }

                source.Insert(index, item);
            }

            InsertItemFallback(group, item, itemComparer);
        }

        return group;
    }

    /// <summary>
    /// Remove the first occurrence of the group with <paramref name="key"/> from the <paramref name="source"/> grouped collection.
    /// It will not do anything if the group does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="key"/> are <see langword="null"/>.</exception>
    public static void RemoveGroup<TKey, TValue>(this ObservableGroupedCollection<TKey, TValue> source, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        if (source.TryGetList(out List<ObservableGroup<TKey, TValue>>? list))
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TValue>? group in list)
            {
                if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                {
                    source.RemoveAt(index);

                    return;
                }

                index++;
            }
        }
        else
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void RemoveGroupFallback(ObservableGroupedCollection<TKey, TValue> source, TKey key)
            {
                int index = 0;

                foreach (ObservableGroup<TKey, TValue>? group in source)
                {
                    if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                    {
                        source.RemoveAt(index);
                        return;
                    }

                    index++;
                }
            }

            RemoveGroupFallback(source, key);
        }
    }

    /// <summary>
    /// Remove the first <paramref name="item"/> from the first group with <paramref name="key"/> from the <paramref name="source"/> grouped collection.
    /// It will not do anything if the group or the item does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where the <paramref name="item"/> should be removed.</param>
    /// <param name="item">The item to remove.</param>
    /// <param name="removeGroupIfEmpty">If true (default value), the group will be removed once it becomes empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> or <paramref name="key"/> are <see langword="null"/>.</exception>
    public static void RemoveItem<TKey, TValue>(this ObservableGroupedCollection<TKey, TValue> source, TKey key, TValue item, bool removeGroupIfEmpty = true)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.For<TKey>.ThrowIfNull(key);

        if (source.TryGetList(out List<ObservableGroup<TKey, TValue>>? list))
        {
            int index = 0;

            foreach (ObservableGroup<TKey, TValue>? group in list)
            {
                if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                {
                    if (group.Remove(item) &&
                        removeGroupIfEmpty &&
                        group.Count == 0)
                    {
                        source.RemoveAt(index);
                    }

                    return;
                }

                index++;
            }
        }
        else
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void RemoveItemFallback(ObservableGroupedCollection<TKey, TValue> source, TKey key, TValue item, bool removeGroupIfEmpty)
            {
                int index = 0;

                foreach (ObservableGroup<TKey, TValue>? group in source)
                {
                    if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                    {
                        if (group.Remove(item) &&
                            removeGroupIfEmpty &&
                            group.Count == 0)
                        {
                            source.RemoveAt(index);
                        }

                        return;
                    }

                    index++;
                }
            }

            RemoveItemFallback(source, key, item, removeGroupIfEmpty);
        }
    }
}

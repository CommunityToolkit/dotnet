// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests.Collections;

[TestClass]
public class Test_ReadOnlyObservableGroupedCollection
{
    [TestMethod]
    public void Test_ReadOnlyObservableGroupedCollection_Ctor_WithEmptySource_ShoudInitializeObject()
    {
        ObservableGroupedCollection<string, int> source = new();
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);

        Assert.AreEqual(readOnlyGroup.Count, 0);
        CollectionAssert.AreEqual(readOnlyGroup, Array.Empty<int>());
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroupedCollection_Ctor_WithObservableGroupedCollection_ShoudInitializeObject()
    {
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", new[] { 1, 3, 5 }),
            new IntGroup("B", new[] { 2, 4, 6 }),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);

        Assert.AreEqual(readOnlyGroup.Count, 2);

        Assert.AreEqual(readOnlyGroup[0].Key, "A");
        CollectionAssert.AreEquivalent(readOnlyGroup[0], new[] { 1, 3, 5 });

        Assert.AreEqual(readOnlyGroup[1].Key, "B");
        CollectionAssert.AreEquivalent(readOnlyGroup[1], new[] { 2, 4, 6 });
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroupedCollection_Ctor_WithListOfReadOnlyObservableGroupSource_ShoudInitializeObject()
    {
        ObservableCollection<ReadOnlyObservableGroup<string, int>> source = new()
        {
            new ReadOnlyObservableGroup<string, int>("A", new ObservableCollection<int> { 1, 3, 5 }),
            new ReadOnlyObservableGroup<string, int>("B", new ObservableCollection<int> { 2, 4, 6 }),
        };
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);

        Assert.AreEqual(readOnlyGroup.Count, 2);

        Assert.AreEqual(readOnlyGroup[0].Key, "A");
        CollectionAssert.AreEqual(readOnlyGroup[0], new[] { 1, 3, 5 });

        Assert.AreEqual(readOnlyGroup[1].Key, "B");
        CollectionAssert.AreEqual(readOnlyGroup[1], new[] { 2, 4, 6 });
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroupedCollection_IListImplementation_Properties_ShoudReturnExpectedValues()
    {
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", new[] { 1, 3, 5 }),
            new IntGroup("B", new[] { 2, 4, 6 }),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        IList list = readOnlyGroup;

        Assert.AreEqual(list.Count, 2);

        ReadOnlyObservableGroup<string, int> group0 = (ReadOnlyObservableGroup<string, int>)list[0]!;

        Assert.AreEqual(group0.Key, "A");
        CollectionAssert.AreEqual(group0, new[] { 1, 3, 5 });

        ReadOnlyObservableGroup<string, int> group1 = (ReadOnlyObservableGroup<string, int>)list[1]!;

        Assert.AreEqual(group1.Key, "B");
        CollectionAssert.AreEqual(group1, new[] { 2, 4, 6 });

        Assert.IsNotNull(list.SyncRoot);
        Assert.IsTrue(list.IsFixedSize);
        Assert.IsTrue(list.IsReadOnly);
        Assert.IsFalse(list.IsSynchronized);
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroupedCollection_IListImplementation_MutableMethods_ShoudThrow()
    {
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", new[] { 1, 3, 5 }),
            new IntGroup("B", new[] { 2, 4, 6 }),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        IList list = readOnlyGroup;

        ReadOnlyObservableGroup<string, int>? testGroup = new("test", new ObservableCollection<int>());

        _ = Assert.ThrowsException<NotSupportedException>(() => list.Add(testGroup));
        _ = Assert.ThrowsException<NotSupportedException>(() => list.Clear());
        _ = Assert.ThrowsException<NotSupportedException>(() => list.Insert(2, testGroup));
        _ = Assert.ThrowsException<NotSupportedException>(() => list.Remove(testGroup));
        _ = Assert.ThrowsException<NotSupportedException>(() => list.RemoveAt(2));
        _ = Assert.ThrowsException<NotSupportedException>(() => list[2] = testGroup);

        object[]? array = new object[5];

        // This line should not throw
        list.CopyTo(array, 0);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void Test_ReadOnlyObservableGroupedCollection_IListImplementation_IndexOf_ShoudReturnExpectedValue(int groupIndex)
    {
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", new[] { 1, 3, 5 }),
            new IntGroup("B", new[] { 2, 4, 6 }),
            new IntGroup("C", new[] { 7, 8, 9 }),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        IList list = readOnlyGroup;

        object? groupToSearch = groupIndex >= 0 ? list[groupIndex] : null;

        int index = list.IndexOf(groupToSearch);

        Assert.AreEqual(index, groupIndex);
    }

    [TestMethod]
    [DataRow(-1, false)]
    [DataRow(0, true)]
    [DataRow(1, true)]
    public void Test_ReadOnlyObservableGroupedCollection_IListImplementation_Contains_ShoudReturnExpectedValue(int groupIndex, bool expectedResult)
    {
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", new[] { 1, 3, 5 }),
            new IntGroup("B", new[] { 2, 4, 6 }),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        IList list = readOnlyGroup;

        object? groupToSearch = groupIndex >= 0 ? list[groupIndex] : null;

        bool result = list.Contains(groupToSearch);

        Assert.AreEqual(result, expectedResult);
    }

    [TestMethod]
    [DataRow(0, 0)]
    [DataRow(3, 3)]
    public void Test_ReadOnlyObservableGroupedCollection_AddGroupInSource_ShouldAddGroup(int sourceInitialItemsCount, int expectedInsertionIndex)
    {
        NotifyCollectionChangedEventArgs? collectionChangedEventArgs = null;
        int collectionChangedEventsCount = 0;
        bool isCountPropertyChangedEventRaised = false;
        int[] itemsList = new[] { 1, 2, 3 };
        ObservableGroupedCollection<string, int> source = new();
        for (int i = 0; i < sourceInitialItemsCount; i++)
        {
            source.Add(new ObservableGroup<string, int>($"group {i}", Enumerable.Empty<int>()));
        }

        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
        {
            collectionChangedEventArgs = e;
            collectionChangedEventsCount++;
        };
        ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

        source.Add(new ObservableGroup<string, int>("Add", itemsList));

        int expectedReadOnlyGroupCount = sourceInitialItemsCount + 1;

        Assert.AreEqual(readOnlyGroup.Count, expectedReadOnlyGroupCount);
        Assert.AreEqual("Add", readOnlyGroup[readOnlyGroup.Count - 1].Key);
        Assert.IsTrue(isCountPropertyChangedEventRaised);
        Assert.IsNotNull(collectionChangedEventArgs);
        Assert.AreEqual(collectionChangedEventsCount, 1);

        bool isAddEventValid = IsAddEventValid(collectionChangedEventArgs, itemsList, expectedInsertionIndex);

        Assert.IsTrue(isAddEventValid);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void Test_ReadOnlyObservableGroupedCollection_InsertGroupInSource_ShouldAddGroup(int insertionIndex)
    {
        NotifyCollectionChangedEventArgs? collectionChangedEventArgs = null;
        int collectionChangedEventsCount = 0;
        bool isCountPropertyChangedEventRaised = false;
        int[] itemsList = new[] { 1, 2, 3 };
        ObservableGroupedCollection<string, int> source = new()
        {
            new ObservableGroup<string, int>("Group0", new[] { 10, 20, 30 }),
            new ObservableGroup<string, int>("Group1", new[] { 40, 50, 60 })
        };
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
        {
            collectionChangedEventArgs = e;
            collectionChangedEventsCount++;
        };
        ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

        source.Insert(insertionIndex, new ObservableGroup<string, int>("Add", itemsList));

        Assert.AreEqual(readOnlyGroup.Count, 3);
        Assert.AreEqual("Add", readOnlyGroup[insertionIndex].Key);
        Assert.IsTrue(isCountPropertyChangedEventRaised);
        Assert.IsNotNull(collectionChangedEventArgs);
        Assert.AreEqual(collectionChangedEventsCount, 1);

        bool isAddEventValid = IsAddEventValid(collectionChangedEventArgs, itemsList, addIndex: insertionIndex);

        Assert.IsTrue(isAddEventValid);
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroupedCollection_RemoveGroupInSource_ShoudRemoveGroup()
    {
        NotifyCollectionChangedEventArgs? collectionChangedEventArgs = null;
        int collectionChangedEventsCount = 0;
        bool isCountPropertyChangedEventRaised = false;
        int[] aItemsList = new[] { 1, 2, 3 };
        int[] bItemsList = new[] { 2, 4, 6 };
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", aItemsList),
            new IntGroup("B", bItemsList),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
        {
            collectionChangedEventArgs = e;
            collectionChangedEventsCount++;
        };
        ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

        source.RemoveAt(1);

        Assert.AreEqual(readOnlyGroup.Count, 1);

        Assert.AreEqual(readOnlyGroup[0].Key, "A");
        CollectionAssert.AreEquivalent(readOnlyGroup[0], aItemsList);

        Assert.IsTrue(isCountPropertyChangedEventRaised);
        Assert.IsNotNull(collectionChangedEventArgs);
        Assert.AreEqual(collectionChangedEventsCount, 1);

        bool isRemoveEventValid = IsRemoveEventValid(collectionChangedEventArgs, bItemsList, 1);

        Assert.IsTrue(isRemoveEventValid);
    }

    [TestMethod]
    [DataRow(1, 0)]
    [DataRow(0, 1)]
    [DataRow(0, 2)]
    public void Test_ReadOnlyObservableGroupedCollection_MoveGroupInSource_ShoudMoveGroup(int oldIndex, int newIndex)
    {
        NotifyCollectionChangedEventArgs? collectionChangedEventArgs = null;
        int collectionChangedEventsCount = 0;
        bool isCountPropertyChangedEventRaised = false;
        int[] aItemsList = new[] { 1, 2, 3 };
        int[] bItemsList = new[] { 4, 5, 6 };
        int[] cItemsList = new[] { 7, 8, 9 };
        int[] dItemsList = new[] { 10, 11, 12 };
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", aItemsList),
            new IntGroup("B", bItemsList),
            new IntGroup("C", cItemsList),
            new IntGroup("D", dItemsList),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
        {
            collectionChangedEventArgs = e;
            collectionChangedEventsCount++;
        };
        ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

        source.Move(oldIndex, newIndex);

        Assert.AreEqual(groups.Count, readOnlyGroup.Count);

        Assert.AreEqual(readOnlyGroup[0].Key, "B");
        CollectionAssert.AreEquivalent(readOnlyGroup[0], bItemsList);

        List<IGrouping<string, int>> tempList = new(groups);
        IGrouping<string, int> tempItem = tempList[oldIndex];

        tempList.RemoveAt(oldIndex);
        tempList.Insert(newIndex, tempItem);

        for (int i = 0; i < tempList.Count; i++)
        {
            Assert.AreEqual(tempList[i].Key, readOnlyGroup[i].Key);
            CollectionAssert.AreEqual(tempList[i].ToArray(), readOnlyGroup[i].ToArray());
        }

        Assert.IsFalse(isCountPropertyChangedEventRaised);
        Assert.IsNotNull(collectionChangedEventArgs);
        Assert.AreEqual(collectionChangedEventsCount, 1);

        bool isMoveEventValid = IsMoveEventValid(collectionChangedEventArgs, groups[oldIndex], oldIndex, newIndex);

        Assert.IsTrue(isMoveEventValid);
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroupedCollection_ClearSource_ShoudClear()
    {
        NotifyCollectionChangedEventArgs? collectionChangedEventArgs = null;
        int collectionChangedEventsCount = 0;
        bool isCountPropertyChangedEventRaised = false;
        int[] aItemsList = new[] { 1, 2, 3 };
        int[] bItemsList = new[] { 2, 4, 6 };
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", aItemsList),
            new IntGroup("B", bItemsList),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
        {
            collectionChangedEventArgs = e;
            collectionChangedEventsCount++;
        };
        ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

        source.Clear();

        Assert.AreEqual(readOnlyGroup.Count, 0);

        Assert.IsTrue(isCountPropertyChangedEventRaised);
        Assert.IsNotNull(collectionChangedEventArgs);
        Assert.AreEqual(collectionChangedEventsCount, 1);

        bool isResetEventValid = IsResetEventValid(collectionChangedEventArgs);

        Assert.IsTrue(isResetEventValid);
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroupedCollection_ReplaceGroupInSource_ShoudReplaceGroup()
    {
        NotifyCollectionChangedEventArgs? collectionChangedEventArgs = null;
        int collectionChangedEventsCount = 0;
        bool isCountPropertyChangedEventRaised = false;
        int[] aItemsList = new[] { 1, 2, 3 };
        int[] bItemsList = new[] { 2, 4, 6 };
        int[] cItemsList = new[] { 7, 8, 9 };
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", aItemsList),
            new IntGroup("B", bItemsList),
        };
        ObservableGroupedCollection<string, int> source = new(groups);
        ReadOnlyObservableGroupedCollection<string, int> readOnlyGroup = new(source);
        ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
        {
            collectionChangedEventArgs = e;
            collectionChangedEventsCount++;
        };
        ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

        source[0] = new ObservableGroup<string, int>("C", cItemsList);

        Assert.AreEqual(readOnlyGroup.Count, 2);

        Assert.AreEqual(readOnlyGroup[0].Key, "C");
        CollectionAssert.AreEquivalent(readOnlyGroup[0], cItemsList);

        Assert.AreEqual(readOnlyGroup[1].Key, "B");
        CollectionAssert.AreEquivalent(readOnlyGroup[1], bItemsList);

        Assert.IsFalse(isCountPropertyChangedEventRaised);
        Assert.IsNotNull(collectionChangedEventArgs);
        Assert.AreEqual(collectionChangedEventsCount, 1);

        bool isReplaceEventValid = IsReplaceEventValid(collectionChangedEventArgs, aItemsList, cItemsList);

        Assert.IsTrue(isReplaceEventValid);
    }

    private static bool IsAddEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int addIndex)
    {
        IEnumerable<IEnumerable<int>>? newItems = args.NewItems?.Cast<IEnumerable<int>>();

        return
            args.Action == NotifyCollectionChangedAction.Add &&
            args.NewStartingIndex == addIndex &&
            args.OldItems == null &&
            newItems?.Count() == 1 &&
            Enumerable.SequenceEqual(newItems.ElementAt(0), expectedGroupItems);
    }

    private static bool IsRemoveEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int oldIndex)
    {
        IEnumerable<IEnumerable<int>>? oldItems = args.OldItems?.Cast<IEnumerable<int>>();

        return
            args.Action == NotifyCollectionChangedAction.Remove &&
            args.NewItems == null &&
            args.OldStartingIndex == oldIndex &&
            oldItems?.Count() == 1 &&
            Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedGroupItems);
    }

    private static bool IsMoveEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int oldIndex, int newIndex)
    {
        IEnumerable<IEnumerable<int>>? oldItems = args.OldItems?.Cast<IEnumerable<int>>();
        IEnumerable<IEnumerable<int>>? newItems = args.NewItems?.Cast<IEnumerable<int>>();

        return
            args.Action == NotifyCollectionChangedAction.Move &&
            args.OldStartingIndex == oldIndex &&
            args.NewStartingIndex == newIndex &&
            oldItems?.Count() == 1 &&
            Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedGroupItems) &&
            newItems?.Count() == 1 &&
            Enumerable.SequenceEqual(newItems.ElementAt(0), expectedGroupItems);
    }

    private static bool IsReplaceEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedRemovedItems, IEnumerable<int> expectedAddItems)
    {
        IEnumerable<IEnumerable<int>>? oldItems = args.OldItems?.Cast<IEnumerable<int>>();
        IEnumerable<IEnumerable<int>>? newItems = args.NewItems?.Cast<IEnumerable<int>>();

        return
            args.Action == NotifyCollectionChangedAction.Replace &&
            oldItems?.Count() == 1 &&
            Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedRemovedItems) &&
            newItems?.Count() == 1 &&
            Enumerable.SequenceEqual(newItems.ElementAt(0), expectedAddItems);
    }

    private static bool IsResetEventValid(NotifyCollectionChangedEventArgs args) => args.Action == NotifyCollectionChangedAction.Reset && args.NewItems == null && args.OldItems == null;

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ReadOnlyObservableGroupedCollection_Ctor_NullCollectionWithObservableGroups()
    {
        _ = new ReadOnlyObservableGroupedCollection<string, int>((ObservableCollection<ObservableGroup<string, int>>)null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ReadOnlyObservableGroupedCollection_Ctor_NullCollectionWithReadOnlyObservableGroups()
    {
        _ = new ReadOnlyObservableGroupedCollection<string, int>((ObservableCollection<ReadOnlyObservableGroup<string, int>>)null!);
    }
}

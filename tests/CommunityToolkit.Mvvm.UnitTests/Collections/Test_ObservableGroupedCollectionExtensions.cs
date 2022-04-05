// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests.Collections;

[TestClass]
public class Test_ObservableGroupedCollectionExtensions
{
    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_FirstGroupByKey_WhenGroupExists_ShouldReturnFirstGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });

        ObservableGroup<string, int> target = groupedCollection.AddGroup("B", new[] { 10 });

        _ = groupedCollection.AddGroup("B", new[] { 42 });

        ObservableGroup<string, int> result = groupedCollection.FirstGroupByKey("B");

        Assert.AreSame(result, target);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_ObservableGroupedCollectionExtensions_FirstGroupByKey_WhenGroupDoesNotExist_ShouldThrow()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });

        _ = groupedCollection.FirstGroupByKey("I do not exist");
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_FirstGroupByKeyOrDefault_WhenGroupExists_ShouldReturnFirstGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });

        ObservableGroup<string, int> target = groupedCollection.AddGroup("B", new[] { 10 });

        _ = groupedCollection.AddGroup("B", new[] { 42 });

        ObservableGroup<string, int>? result = groupedCollection.FirstGroupByKeyOrDefault("B");

        Assert.AreSame(result, target);
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_FirstGroupByKeyOrDefault_WhenGroupDoesNotExist_ShouldReturnNull()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });

        ObservableGroup<string, int>? result = groupedCollection.FirstGroupByKeyOrDefault("I do not exist");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_FirstGroupByKey_WhenGroupExistsAndIndexInRange_ShouldReturnFirstGroupValue()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });
        _ = groupedCollection.AddGroup("B", new[] { 10, 11, 12 });
        _ = groupedCollection.AddGroup("B", new[] { 42 });

        int result = groupedCollection.FirstGroupByKey("B")[2];

        Assert.AreEqual(result, 12);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(3)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_ObservableGroupedCollectionExtensions_FirstGroupByKey_WhenGroupExistsAndIndexOutOfRange_ShouldReturnThrow(int index)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });
        _ = groupedCollection.AddGroup("B", new[] { 10, 11, 12 });
        _ = groupedCollection.AddGroup("B", new[] { 42 });

        _ = groupedCollection.FirstGroupByKey("B")[index];
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_FirstGroupByKey_WhenGroupExistsAndIndexInRange_ShouldReturnValue()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });
        _ = groupedCollection.AddGroup("B", new[] { 10, 11, 12 });
        _ = groupedCollection.AddGroup("B", new[] { 42 });

        int result = groupedCollection.FirstGroupByKey("B")[2];

        Assert.AreEqual(result, 12);
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_AddGroup_WithItem_ShouldAddGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddGroup("new key", new[] { 23 });

        Assert.IsNotNull(addedGroup);
        Assert.AreEqual(addedGroup.Key, "new key");
        CollectionAssert.AreEqual(addedGroup, new[] { 23 });
        CollectionAssert.AreEqual(groupedCollection, new[] { addedGroup });
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_AddGroup_WithCollection_ShouldAddGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddGroup("new key", new[] { 23, 10, 42 });

        Assert.IsNotNull(addedGroup);
        Assert.AreEqual(addedGroup.Key, "new key");
        CollectionAssert.AreEqual(addedGroup, new[] { 23, 10, 42 });
        CollectionAssert.AreEqual(groupedCollection, new[] { addedGroup });
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_AddGroup_WithParamsCollection_ShouldAddGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddGroup("new key", new[] { 23, 10, 42 });

        Assert.IsNotNull(addedGroup);
        Assert.AreEqual(addedGroup.Key, "new key");
        CollectionAssert.AreEqual(addedGroup, new[] { 23, 10, 42 });
        CollectionAssert.AreEqual(groupedCollection, new[] { addedGroup });
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_InsertGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> a = groupedCollection.InsertGroup("A", new[] { 1, 2, 3 });
        ObservableGroup<string, int> d = groupedCollection.InsertGroup("D", new[] { 1, 2, 3 });
        ObservableGroup<string, int> e = groupedCollection.InsertGroup("E", new[] { 1, 2, 3 });
        ObservableGroup<string, int> b = groupedCollection.InsertGroup("B", new[] { 1, 2, 3 });
        ObservableGroup<string, int> z = groupedCollection.InsertGroup("Z", new[] { 1, 2, 3 });
        ObservableGroup<string, int> c = groupedCollection.InsertGroup("C", new[] { 1, 2, 3 });

        CollectionAssert.AllItemsAreNotNull(new[] { a, d, e, b, z, c });
        CollectionAssert.AreEqual(new[] { a, b, c, d, e, z }, groupedCollection);
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_InsertGroup_WithGrouping()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> a = groupedCollection.InsertGroup(new IntGroup("A", new[] { 1, 2, 3 }));
        ObservableGroup<string, int> d = groupedCollection.InsertGroup(new IntGroup("D", new[] { 1, 2, 3 }));
        ObservableGroup<string, int> e = groupedCollection.InsertGroup(new IntGroup("E", new[] { 1, 2, 3 }));
        ObservableGroup<string, int> b = groupedCollection.InsertGroup(new IntGroup("B", new[] { 1, 2, 3 }));
        ObservableGroup<string, int> z = groupedCollection.InsertGroup(new IntGroup("Z", new[] { 1, 2, 3 }));
        ObservableGroup<string, int> c = groupedCollection.InsertGroup(new IntGroup("C", new[] { 1, 2, 3 }));

        CollectionAssert.AllItemsAreNotNull(new[] { a, d, e, b, z, c });
        CollectionAssert.AreEqual(new[] { a, b, c, d, e, z }, groupedCollection);
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_AddItem_WhenTargetGroupDoesNotExists_ShouldCreateAndAddNewGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddItem("new key", 23);

        Assert.IsNotNull(addedGroup);
        Assert.AreEqual(addedGroup.Key, "new key");
        CollectionAssert.AreEqual(addedGroup, new[] { 23 });
        CollectionAssert.AreEqual(groupedCollection, new[] { addedGroup });
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_AddItem_WhenSingleTargetGroupAlreadyExists_ShouldAddItemToExistingGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });

        ObservableGroup<string, int> targetGroup = groupedCollection.AddGroup("B", new[] { 4, 5, 6 });

        _ = groupedCollection.AddGroup("C", new[] { 7, 8 });

        ObservableGroup<string, int> addedGroup = groupedCollection.AddItem("B", 23);

        Assert.AreSame(addedGroup, targetGroup);
        Assert.AreEqual(addedGroup.Key, "B");
        CollectionAssert.AreEqual(addedGroup, new[] { 4, 5, 6, 23 });

        Assert.AreEqual(groupedCollection.Count, 3);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });

        Assert.AreEqual(groupedCollection[1].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[1], new[] { 4, 5, 6, 23 });

        Assert.AreEqual(groupedCollection[2].Key, "C");
        CollectionAssert.AreEqual(groupedCollection[2], new[] { 7, 8 });
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_AddItem_WhenSeveralTargetGroupsAlreadyExist_ShouldAddItemToFirstExistingGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });

        ObservableGroup<string, int> targetGroup = groupedCollection.AddGroup("B", new[] { 4, 5, 6 });

        _ = groupedCollection.AddGroup("B", new[] { 7, 8, 9 });
        _ = groupedCollection.AddGroup("C", new[] { 10, 11 });

        ObservableGroup<string, int> addedGroup = groupedCollection.AddItem("B", 23);

        Assert.AreSame(addedGroup, targetGroup);
        Assert.AreEqual(addedGroup.Key, "B");
        CollectionAssert.AreEqual(addedGroup, new[] { 4, 5, 6, 23 });

        Assert.AreEqual(groupedCollection.Count, 4);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });

        Assert.AreEqual(groupedCollection[1].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[1], new[] { 4, 5, 6, 23 });

        Assert.AreEqual(groupedCollection[2].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[2], new[] { 7, 8, 9 });

        Assert.AreEqual(groupedCollection[3].Key, "C");
        CollectionAssert.AreEqual(groupedCollection[3], new[] { 10, 11 });
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_InsertItem()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> group1 = groupedCollection.InsertItem("A", 1);
        ObservableGroup<string, int> group2 = groupedCollection.InsertItem("A", 2);
        ObservableGroup<string, int> group6 = groupedCollection.InsertItem("A", 6);
        ObservableGroup<string, int> group4 = groupedCollection.InsertItem("A", 4);
        ObservableGroup<string, int> group3 = groupedCollection.InsertItem("A", 3);
        ObservableGroup<string, int> group99 = groupedCollection.InsertItem("A", 99);
        ObservableGroup<string, int> group8 = groupedCollection.InsertItem("B", 8);
        ObservableGroup<string, int> group7 = groupedCollection.InsertItem("B", 7);

        Assert.AreEqual(2, groupedCollection.Count);
        CollectionAssert.AllItemsAreNotNull(new[] { group1, group2, group6, group4, group3, group99, group8, group7 });

        Assert.AreSame(group1, group2);
        Assert.AreSame(group1, group6);
        Assert.AreSame(group1, group4);
        Assert.AreSame(group1, group3);
        Assert.AreSame(group1, group2);
        Assert.AreSame(group1, group99);
        Assert.AreNotSame(group1, group8);
        Assert.AreSame(group8, group7);

        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 6, 99 }, group1);
        CollectionAssert.AreEqual(new[] { 7, 8 }, group8);
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_InsertItem_WithComparer()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> group1 = groupedCollection.InsertItem("A", Comparer<string>.Default, 1, Comparer<int>.Default);
        ObservableGroup<string, int> group2 = groupedCollection.InsertItem("A", Comparer<string>.Default, 2, Comparer<int>.Default);
        ObservableGroup<string, int> group6 = groupedCollection.InsertItem("A", Comparer<string>.Default, 6, Comparer<int>.Default);
        ObservableGroup<string, int> group4 = groupedCollection.InsertItem("A", Comparer<string>.Default, 4, Comparer<int>.Default);
        ObservableGroup<string, int> group3 = groupedCollection.InsertItem("A", Comparer<string>.Default, 3, Comparer<int>.Default);
        ObservableGroup<string, int> group99 = groupedCollection.InsertItem("A", Comparer<string>.Default, 99, Comparer<int>.Default);
        ObservableGroup<string, int> group8 = groupedCollection.InsertItem("B", Comparer<string>.Default, 8, Comparer<int>.Default);
        ObservableGroup<string, int> group7 = groupedCollection.InsertItem("B", Comparer<string>.Default, 7, Comparer<int>.Default);

        Assert.AreEqual(2, groupedCollection.Count);
        CollectionAssert.AllItemsAreNotNull(new[] { group1, group2, group6, group4, group3, group99, group8, group7 });

        Assert.AreSame(group1, group2);
        Assert.AreSame(group1, group6);
        Assert.AreSame(group1, group4);
        Assert.AreSame(group1, group3);
        Assert.AreSame(group1, group2);
        Assert.AreSame(group1, group99);
        Assert.AreNotSame(group1, group8);
        Assert.AreSame(group8, group7);

        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 6, 99 }, group1);
        CollectionAssert.AreEqual(new[] { 7, 8 }, group8);
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_RemoveGroup_WhenGroupDoesNotExists_ShouldDoNothing()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });

        groupedCollection.RemoveGroup("I do not exist");

        Assert.AreEqual(groupedCollection.Count, 1);
        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_RemoveGroup_WhenSingleGroupExists_ShouldRemoveGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });
        _ = groupedCollection.AddGroup("B", new[] { 4, 5, 6 });

        groupedCollection.RemoveGroup("B");

        Assert.AreEqual(groupedCollection.Count, 1);
        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });
    }

    [TestMethod]
    public void Test_ObservableGroupedCollectionExtensions_RemoveGroup_WhenSeveralGroupsExist_ShouldRemoveFirstGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });
        _ = groupedCollection.AddGroup("B", new[] { 4, 5, 6 });
        _ = groupedCollection.AddGroup("B", new[] { 7, 8 });

        groupedCollection.RemoveGroup("B");

        Assert.AreEqual(groupedCollection.Count, 2);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });

        Assert.AreEqual(groupedCollection[1].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[1], new[] { 7, 8 });
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Test_ObservableGroupedCollectionExtensions_RemoveItem_WhenGroupDoesNotExist_ShouldDoNothing(bool removeGroupIfEmpty)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });
        _ = groupedCollection.AddGroup("B", new[] { 4, 5, 6 });

        groupedCollection.RemoveItem("I do not exist", 8, removeGroupIfEmpty);

        Assert.AreEqual(groupedCollection.Count, 2);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });

        Assert.AreEqual(groupedCollection[1].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[1], new[] { 4, 5, 6 });
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Test_ObservableGroupedCollectionExtensions_RemoveItem_WhenGroupExistsAndItemDoesNotExist_ShouldDoNothing(bool removeGroupIfEmpty)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });
        _ = groupedCollection.AddGroup("B", new[] { 4, 5, 6 });

        groupedCollection.RemoveItem("B", 8, removeGroupIfEmpty);

        Assert.AreEqual(groupedCollection.Count, 2);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });

        Assert.AreEqual(groupedCollection[1].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[1], new[] { 4, 5, 6 });
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Test_ObservableGroupedCollectionExtensions_RemoveItem_WhenGroupAndItemExist_ShouldRemoveItemFromGroup(bool removeGroupIfEmpty)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });
        _ = groupedCollection.AddGroup("B", new[] { 4, 5, 6 });

        groupedCollection.RemoveItem("B", 5, removeGroupIfEmpty);

        Assert.AreEqual(groupedCollection.Count, 2);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });

        Assert.AreEqual(groupedCollection[1].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[1], new[] { 4, 6 });
    }

    [TestMethod]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void Test_ObservableGroupedCollectionExtensions_RemoveItem_WhenRemovingLastItem_ShouldRemoveGroupIfRequired(bool removeGroupIfEmpty, bool expectGroupRemoved)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });
        _ = groupedCollection.AddGroup("B", new[] { 4 });

        groupedCollection.RemoveItem("B", 4, removeGroupIfEmpty);

        Assert.AreEqual(groupedCollection.Count, expectGroupRemoved ? 1 : 2);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });

        if (!expectGroupRemoved)
        {
            Assert.AreEqual(groupedCollection[1].Key, "B");
            Assert.AreEqual(groupedCollection[1].Count, 0);
        }
    }
}

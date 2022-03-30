// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using CommunityToolkit.Mvvm.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public class ObservableGroupedCollectionExtensionsTests
{
    [TestMethod]
    public void First_WhenGroupExists_ShouldReturnFirstGroup()
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
    public void First_WhenGroupDoesNotExist_ShouldThrow()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });

        _ = groupedCollection.FirstGroupByKey("I do not exist");
    }

    [TestMethod]
    public void FirstOrDefault_WhenGroupExists_ShouldReturnFirstGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });

        ObservableGroup<string, int> target = groupedCollection.AddGroup("B", new[] { 10 });

        _ = groupedCollection.AddGroup("B", new[] { 42 });

        ObservableGroup<string, int>? result = groupedCollection.FirstGroupByKeyOrDefault("B");

        Assert.AreSame(result, target);
    }

    [TestMethod]
    public void FirstOrDefault_WhenGroupDoesNotExist_ShouldReturnNull()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });

        ObservableGroup<string, int>? result = groupedCollection.FirstGroupByKeyOrDefault("I do not exist");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ElementAt_WhenGroupExistsAndIndexInRange_ShouldReturnFirstGroupValue()
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
    public void ElementAt_WhenGroupExistsAndIndexOutOfRange_ShouldReturnThrow(int index)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });
        _ = groupedCollection.AddGroup("B", new[] { 10, 11, 12 });
        _ = groupedCollection.AddGroup("B", new[] { 42 });

        _ = groupedCollection.FirstGroupByKey("B")[index];
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ElementAt_WhenGroupDoesNotExist_ShouldThrow()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });

        _ = groupedCollection.FirstGroupByKey("I do not exist")[0];
    }

    [TestMethod]
    public void ElementAtOrDefault_WhenGroupExistsAndIndexInRange_ShouldReturnValue()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 23 });
        _ = groupedCollection.AddGroup("B", new[] { 10, 11, 12 });
        _ = groupedCollection.AddGroup("B", new[] { 42 });

        int result = groupedCollection.FirstGroupByKey("B")[2];

        Assert.AreEqual(result, 12);
    }

    [TestMethod]
    public void AddGroup_WithItem_ShouldAddGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddGroup("new key", new[] { 23 });

        Assert.IsNotNull(addedGroup);
        Assert.AreEqual(addedGroup.Key, "new key");
        CollectionAssert.AreEqual(addedGroup, new[] { 23 });
        CollectionAssert.AreEqual(groupedCollection, new[] { addedGroup });
    }

    [TestMethod]
    public void AddGroup_WithCollection_ShouldAddGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddGroup("new key", new[] { 23, 10, 42 });

        Assert.IsNotNull(addedGroup);
        Assert.AreEqual(addedGroup.Key, "new key");
        CollectionAssert.AreEqual(addedGroup, new[] { 23, 10, 42 });
        CollectionAssert.AreEqual(groupedCollection, new[] { addedGroup });
    }

    [TestMethod]
    public void AddGroup_WithParamsCollection_ShouldAddGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddGroup("new key", new[] { 23, 10, 42 });

        Assert.IsNotNull(addedGroup);
        Assert.AreEqual(addedGroup.Key, "new key");
        CollectionAssert.AreEqual(addedGroup, new[] { 23, 10, 42 });
        CollectionAssert.AreEqual(groupedCollection, new[] { addedGroup });
    }

    [TestMethod]
    public void AddItem_WhenTargetGroupDoesNotExists_ShouldCreateAndAddNewGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddItem("new key", 23);

        Assert.IsNotNull(addedGroup);
        Assert.AreEqual(addedGroup.Key, "new key");
        CollectionAssert.AreEqual(addedGroup, new[] { 23 });
        CollectionAssert.AreEqual(groupedCollection, new[] { addedGroup });
    }

    [TestMethod]
    public void AddItem_WhenSingleTargetGroupAlreadyExists_ShouldAddItemToExistingGroup()
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
    public void AddItem_WhenSeveralTargetGroupsAlreadyExist_ShouldAddItemToFirstExistingGroup()
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
    public void RemoveGroup_WhenGroupDoesNotExists_ShouldDoNothing()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", new[] { 1, 2, 3 });

        groupedCollection.RemoveGroup("I do not exist");

        Assert.AreEqual(groupedCollection.Count, 1);
        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });
    }

    [TestMethod]
    public void RemoveGroup_WhenSingleGroupExists_ShouldRemoveGroup()
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
    public void RemoveGroup_WhenSeveralGroupsExist_ShouldRemoveFirstGroup()
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
    public void RemoveItem_WhenGroupDoesNotExist_ShouldDoNothing(bool removeGroupIfEmpty)
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
    public void RemoveItem_WhenGroupExistsAndItemDoesNotExist_ShouldDoNothing(bool removeGroupIfEmpty)
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
    public void RemoveItem_WhenGroupAndItemExist_ShouldRemoveItemFromGroup(bool removeGroupIfEmpty)
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
    public void RemoveItem_WhenRemovingLastItem_ShouldRemoveGroupIfRequired(bool removeGroupIfEmpty, bool expectGroupRemoved)
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

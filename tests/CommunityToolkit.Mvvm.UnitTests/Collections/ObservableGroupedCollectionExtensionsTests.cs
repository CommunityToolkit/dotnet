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

        _ = groupedCollection.AddGroup("A", 23);

        ObservableGroup<string, int> target = groupedCollection.AddGroup("B", 10);

        _ = groupedCollection.AddGroup("B", 42);

        ObservableGroup<string, int> result = groupedCollection.First("B");

        Assert.AreSame(result, target);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void First_WhenGroupDoesNotExist_ShouldThrow()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);

        _ = groupedCollection.First("I do not exist");
    }

    [TestMethod]
    public void FirstOrDefault_WhenGroupExists_ShouldReturnFirstGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);

        ObservableGroup<string, int> target = groupedCollection.AddGroup("B", 10);

        _ = groupedCollection.AddGroup("B", 42);

        ObservableGroup<string, int>? result = groupedCollection.FirstOrDefault("B");

        Assert.AreSame(result, target);
    }

    [TestMethod]
    public void FirstOrDefault_WhenGroupDoesNotExist_ShouldReturnNull()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);

        ObservableGroup<string, int>? result = groupedCollection.FirstOrDefault("I do not exist");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ElementAt_WhenGroupExistsAndIndexInRange_ShouldReturnFirstGroupValue()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);
        _ = groupedCollection.AddGroup("B", 10, 11, 12);
        _ = groupedCollection.AddGroup("B", 42);

        int result = groupedCollection.ElementAt("B", 2);

        Assert.AreEqual(result, 12);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(3)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ElementAt_WhenGroupExistsAndIndexOutOfRange_ShouldReturnThrow(int index)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);
        _ = groupedCollection.AddGroup("B", 10, 11, 12);
        _ = groupedCollection.AddGroup("B", 42);

        _ = groupedCollection.ElementAt("B", index);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ElementAt_WhenGroupDoesNotExist_ShouldThrow()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);

        _ = groupedCollection.ElementAt("I do not exist", 0);
    }

    [TestMethod]
    public void ElementAtOrDefault_WhenGroupExistsAndIndexInRange_ShouldReturnValue()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);
        _ = groupedCollection.AddGroup("B", 10, 11, 12);
        _ = groupedCollection.AddGroup("B", 42);

        int result = groupedCollection.ElementAt("B", 2);

        Assert.AreEqual(result, 12);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(3)]
    public void ElementAtOrDefault_WhenGroupExistsAndIndexOutOfRange_ShouldReturnDefaultValue(int index)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);
        _ = groupedCollection.AddGroup("B", 10, 11, 12);
        _ = groupedCollection.AddGroup("B", 42);

        int result = groupedCollection.ElementAtOrDefault("B", index);

        Assert.AreEqual(result, 0);
    }

    [TestMethod]
    public void ElementAtOrDefault_WhenGroupDoesNotExist_ShouldReturnDefaultValue()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 23);

        int result = groupedCollection.ElementAtOrDefault("I do not exist", 0);

        Assert.AreEqual(result, 0);
    }

    [TestMethod]
    public void AddGroup_WithItem_ShouldAddGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        ObservableGroup<string, int> addedGroup = groupedCollection.AddGroup("new key", 23);

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

        ObservableGroup<string, int> addedGroup = groupedCollection.AddGroup("new key", 23, 10, 42);

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

        _ = groupedCollection.AddGroup("A", 1, 2, 3);

        ObservableGroup<string, int> targetGroup = groupedCollection.AddGroup("B", 4, 5, 6);

        _ = groupedCollection.AddGroup("C", 7, 8);

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

        _ = groupedCollection.AddGroup("A", 1, 2, 3);

        ObservableGroup<string, int> targetGroup = groupedCollection.AddGroup("B", 4, 5, 6);

        _ = groupedCollection.AddGroup("B", 7, 8, 9);
        _ = groupedCollection.AddGroup("C", 10, 11);

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
    [ExpectedException(typeof(InvalidOperationException))]
    public void InsertItem_WhenGroupDoesNotExist_ShouldThrow()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 1, 2, 3);

        _ = groupedCollection.InsertItem("I do not exist", 0, 23);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(4)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void InsertItem_WhenIndexOutOfRange_ShouldThrow(int index)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 1, 2, 3);

        _ = groupedCollection.InsertItem("A", index, 23);
    }

    [TestMethod]
    [DataRow(0, new[] { 23, 1, 2, 3 })]
    [DataRow(1, new[] { 1, 23, 2, 3 })]
    [DataRow(3, new[] { 1, 2, 3, 23 })]
    public void InsertItem_WithValidIndex_WithSeveralGroups_ShoudInsertItemInFirstGroup(int index, int[] expecteGroupValues)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 4, 5);

        ObservableGroup<string, int> targetGroup = groupedCollection.AddGroup("B", 1, 2, 3);

        _ = groupedCollection.AddGroup("B", 6, 7);

        ObservableGroup<string, int> group = groupedCollection.InsertItem("B", index, 23);

        Assert.AreSame(group, targetGroup);

        Assert.AreEqual(groupedCollection.Count, 3);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 4, 5 });

        Assert.AreEqual(groupedCollection[1].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[1], expecteGroupValues);

        Assert.AreEqual(groupedCollection[2].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[2], new[] { 6, 7 });
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void SetItem_WhenGroupDoesNotExist_ShouldThrow()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 1, 2, 3);

        _ = groupedCollection.SetItem("I do not exist", 0, 23);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(3)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetItem_WhenIndexOutOfRange_ShouldThrow(int index)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 1, 2, 3);

        _ = groupedCollection.SetItem("A", index, 23);
    }

    [TestMethod]
    [DataRow(0, new[] { 23, 2, 3 })]
    [DataRow(1, new[] { 1, 23, 3 })]
    [DataRow(2, new[] { 1, 2, 23 })]
    public void SetItem_WithValidIndex_WithSeveralGroups_ShouldReplaceItemInFirstGroup(int index, int[] expectedGroupValues)
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 4, 5);

        ObservableGroup<string, int> targetGroup = groupedCollection.AddGroup("B", 1, 2, 3);

        _ = groupedCollection.AddGroup("B", 6, 7);

        ObservableGroup<string, int>? group = groupedCollection.SetItem("B", index, 23);

        Assert.AreSame(group, targetGroup);

        Assert.AreEqual(groupedCollection.Count, 3);

        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 4, 5 });

        Assert.AreEqual(groupedCollection[1].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[1], expectedGroupValues);

        Assert.AreEqual(groupedCollection[2].Key, "B");
        CollectionAssert.AreEqual(groupedCollection[2], new[] { 6, 7 });
    }

    [TestMethod]
    public void RemoveGroup_WhenGroupDoesNotExists_ShouldDoNothing()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 1, 2, 3);

        groupedCollection.RemoveGroup("I do not exist");

        Assert.AreEqual(groupedCollection.Count, 1);
        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });
    }

    [TestMethod]
    public void RemoveGroup_WhenSingleGroupExists_ShouldRemoveGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 1, 2, 3);
        _ = groupedCollection.AddGroup("B", 4, 5, 6);

        groupedCollection.RemoveGroup("B");

        Assert.AreEqual(groupedCollection.Count, 1);
        Assert.AreEqual(groupedCollection[0].Key, "A");
        CollectionAssert.AreEqual(groupedCollection[0], new[] { 1, 2, 3 });
    }

    [TestMethod]
    public void RemoveGroup_WhenSeveralGroupsExist_ShouldRemoveFirstGroup()
    {
        ObservableGroupedCollection<string, int> groupedCollection = new();

        _ = groupedCollection.AddGroup("A", 1, 2, 3);
        _ = groupedCollection.AddGroup("B", 4, 5, 6);
        _ = groupedCollection.AddGroup("B", 7, 8);

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

        _ = groupedCollection.AddGroup("A", 1, 2, 3);
        _ = groupedCollection.AddGroup("B", 4, 5, 6);

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

        _ = groupedCollection.AddGroup("A", 1, 2, 3);
        _ = groupedCollection.AddGroup("B", 4, 5, 6);

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

        _ = groupedCollection.AddGroup("A", 1, 2, 3);
        _ = groupedCollection.AddGroup("B", 4, 5, 6);

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

        _ = groupedCollection.AddGroup("A", 1, 2, 3);
        _ = groupedCollection.AddGroup("B", 4);

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

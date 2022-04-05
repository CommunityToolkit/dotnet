// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests.Collections;

[TestClass]
public class Test_ReadOnlyObservableGroup
{
    [TestMethod]
    public void Test_ReadOnlyObservableGroup_Ctor_WithKeyAndOBservableCollection_ShouldHaveExpectedInitialState()
    {
        ObservableCollection<int> source = new(new[] { 1, 2, 3 });
        ReadOnlyObservableGroup<string, int> group = new("key", source);

        Assert.AreEqual(group.Key, "key");
        CollectionAssert.AreEqual(group, new[] { 1, 2, 3 });
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroup_Ctor_ObservableGroup_ShouldHaveExpectedInitialState()
    {
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int> sourceGroup = new("key", source);
        ReadOnlyObservableGroup<string, int> group = new(sourceGroup);

        Assert.AreEqual(group.Key, "key");
        CollectionAssert.AreEqual(group, new[] { 1, 2, 3 });
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroup_Ctor_WithKeyAndCollection_ShouldHaveExpectedInitialState()
    {
        ObservableCollection<int> source = new() { 1, 2, 3 };
        ReadOnlyObservableGroup<string, int> group = new("key", source);

        Assert.AreEqual(group.Key, "key");
        CollectionAssert.AreEqual(group, new[] { 1, 2, 3 });
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroup_Add_ShouldRaiseEvent()
    {
        bool collectionChangedEventRaised = false;
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int> sourceGroup = new("key", source);
        ReadOnlyObservableGroup<string, int> group = new(sourceGroup);

        ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

        sourceGroup.Add(4);

        Assert.AreEqual(group.Key, "key");
        CollectionAssert.AreEqual(group, new[] { 1, 2, 3, 4 });

        Assert.IsTrue(collectionChangedEventRaised);
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroup_Update_ShouldRaiseEvent()
    {
        bool collectionChangedEventRaised = false;
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int> sourceGroup = new("key", source);
        ReadOnlyObservableGroup<string, int> group = new(sourceGroup);

        ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

        sourceGroup[1] = 4;

        Assert.AreEqual(group.Key, "key");
        CollectionAssert.AreEqual(group, new[] { 1, 4, 3 });

        Assert.IsTrue(collectionChangedEventRaised);
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroup_Remove_ShouldRaiseEvent()
    {
        bool collectionChangedEventRaised = false;
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int> sourceGroup = new("key", source);
        ReadOnlyObservableGroup<string, int> group = new(sourceGroup);

        ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

        _ = sourceGroup.Remove(1);

        Assert.AreEqual(group.Key, "key");
        CollectionAssert.AreEqual(group, new[] { 2, 3 });

        Assert.IsTrue(collectionChangedEventRaised);
    }

    [TestMethod]
    public void Test_ReadOnlyObservableGroup_Clear_ShouldRaiseEvent()
    {
        bool collectionChangedEventRaised = false;
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int> sourceGroup = new("key", source);
        ReadOnlyObservableGroup<string, int> group = new(sourceGroup);

        ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

        sourceGroup.Clear();

        Assert.AreEqual(group.Key, "key");
        CollectionAssert.AreEqual(group, Array.Empty<int>());

        Assert.IsTrue(collectionChangedEventRaised);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(3)]
    public void Test_ReadOnlyObservableGroup_IReadOnlyObservableGroup_ShouldReturnExpectedValues(int count)
    {
        ObservableGroup<string, int> sourceGroup = new("key", Enumerable.Range(0, count));
        ReadOnlyObservableGroup<string, int> group = new(sourceGroup);
        IReadOnlyObservableGroup iReadOnlyObservableGroup = group;

        Assert.AreEqual(iReadOnlyObservableGroup.Key, "key");
        Assert.AreEqual(iReadOnlyObservableGroup.Count, count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ReadOnlyObservableGroup_Ctor_NullKeyWithNotNullElements()
    {
        _ = new ReadOnlyObservableGroup<string, int>(null!, new ObservableCollection<int>());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ReadOnlyObservableGroup_Ctor_NotNullKeyWithNullElements()
    {
        _ = new ReadOnlyObservableGroup<string, int>("A", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ReadOnlyObservableGroup_Ctor_NullGroup()
    {
        _ = new ReadOnlyObservableGroup<string, int>(null!);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using CommunityToolkit.Mvvm.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests.Collections;

[TestClass]
public class Test_ObservableGroup
{
    [TestMethod]
    public void Test_ObservableGroup_Ctor_ShouldHaveExpectedState()
    {
        ObservableGroup<string, int> group = new("key");

        Assert.AreEqual("key", group.Key);
        Assert.AreEqual(0, group.Count);
    }

    [TestMethod]
    public void Test_ObservableGroup_Ctor_WithGrouping_ShouldHaveExpectedState()
    {
        IntGroup source = new("key", new[] { 1, 2, 3 });
        ObservableGroup<string, int> group = new(source);

        Assert.AreEqual("key", group.Key);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, group);
    }

    [TestMethod]
    public void Test_ObservableGroup_Ctor_WithCollection_ShouldHaveExpectedState()
    {
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int> group = new("key", source);

        Assert.AreEqual("key", group.Key);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, group);
    }

    [TestMethod]
    public void Test_ObservableGroup_Add_ShouldRaiseEvent()
    {
        bool collectionChangedEventRaised = false;
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int> group = new("key", source);

        group.CollectionChanged += (s, e) => collectionChangedEventRaised = true;

        group.Add(4);

        Assert.AreEqual("key", group.Key);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, group);
        Assert.IsTrue(collectionChangedEventRaised);
    }

    [TestMethod]
    public void Test_ObservableGroup_Update_ShouldRaiseEvent()
    {
        bool collectionChangedEventRaised = false;
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int> group = new("key", source);

        group.CollectionChanged += (s, e) => collectionChangedEventRaised = true;

        group[1] = 4;

        Assert.AreEqual("key", group.Key);
        CollectionAssert.AreEqual(new[] { 1, 4, 3 }, group);
        Assert.IsTrue(collectionChangedEventRaised);
    }

    [TestMethod]
    public void Test_ObservableGroup_Remove_ShouldRaiseEvent()
    {
        bool collectionChangedEventRaised = false;
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int>? group = new("key", source);

        group.CollectionChanged += (s, e) => collectionChangedEventRaised = true;

        _ = group.Remove(1);

        Assert.AreEqual("key", group.Key);
        CollectionAssert.AreEqual(new[] { 2, 3 }, group);
        Assert.IsTrue(collectionChangedEventRaised);
    }

    [TestMethod]
    public void Test_ObservableGroup_Clear_ShouldRaiseEvent()
    {
        bool collectionChangedEventRaised = false;
        int[] source = new[] { 1, 2, 3 };
        ObservableGroup<string, int>? group = new("key", source);

        group.CollectionChanged += (s, e) => collectionChangedEventRaised = true;

        group.Clear();

        Assert.AreEqual("key", group.Key);
        Assert.AreEqual(0, group.Count);
        Assert.IsTrue(collectionChangedEventRaised);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(3)]
    public void Test_ObservableGroup_IReadOnlyObservableGroup_ShouldReturnExpectedValues(int count)
    {
        ObservableGroup<string, int> group = new("key", Enumerable.Range(0, count));
        IReadOnlyObservableGroup iReadOnlyObservableGroup = group;

        Assert.AreEqual("key", iReadOnlyObservableGroup.Key);
        Assert.AreEqual(count, iReadOnlyObservableGroup.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ObservableGroup_Ctor_NullKey()
    {
        _ = new ObservableGroup<string, int>((string)null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ObservableGroup_Ctor_NullGroup()
    {
        _ = new ObservableGroup<string, int>((IGrouping<string, int>)null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ObservableGroup_Ctor_NullKeyWithNotNullElements()
    {
        _ = new ObservableGroup<string, int>(null!, new int[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ObservableGroup_Ctor_NotNullKeyWithNullElements()
    {
        _ = new ObservableGroup<string, int>("A", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ObservableGroup_Ctor_NullKeySetter()
    {
        ObservableGroup<string, int> group = new("A");

        group.Key = null!;
    }
}

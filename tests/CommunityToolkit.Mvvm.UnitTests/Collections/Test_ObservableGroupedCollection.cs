// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests.Collections;

[TestClass]
public class Test_ObservableGroupedCollection
{
    [TestMethod]
    public void Test_ObservableGroupedCollection_Ctor_ShouldHaveExpectedValues()
    {
        ObservableGroupedCollection<string, int> groupCollection = new();

        Assert.AreEqual(0, groupCollection.Count);
    }

    [TestMethod]
    public void Test_ObservableGroupedCollection_Ctor_WithGroups_ShouldHaveExpectedValues()
    {
        List<IGrouping<string, int>> groups = new()
        {
            new IntGroup("A", new[] { 1, 3, 5 }),
            new IntGroup("B", new[] { 2, 4, 6 }),
        };
        ObservableGroupedCollection<string, int> groupCollection = new(groups);

        Assert.AreEqual(2, groupCollection.Count);

        Assert.AreEqual("A", groupCollection[0].Key);
        CollectionAssert.AreEqual(new[] { 1, 3, 5 }, groupCollection[0]);

        Assert.AreEqual("B", groupCollection[1].Key);
        CollectionAssert.AreEqual(new[] { 2, 4, 6 }, groupCollection[1]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ObservableGroupedCollection_Ctor_NullCollection()
    {
        _ = new ObservableGroupedCollection<string, int>(null!);
    }
}

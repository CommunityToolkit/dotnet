// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collections
{
    [TestClass]
    public class ObservableGroupedCollectionTests
    {
        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_ShouldHaveExpectedValues()
        {
            var groupCollection = new ObservableGroupedCollection<string, int>();

            Assert.AreEqual(groupCollection.Count, 0);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithGroups_ShouldHaveExpectedValues()
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var groupCollection = new ObservableGroupedCollection<string, int>(groups);

            Assert.AreEqual(groupCollection.Count, 2);

            Assert.AreEqual(groupCollection.ElementAt(0).Key, "A");
            CollectionAssert.AreEqual(groupCollection.ElementAt(0), new[] { 1, 3, 5 });

            Assert.AreEqual(groupCollection.ElementAt(1).Key, "B");
            CollectionAssert.AreEqual(groupCollection.ElementAt(1), new[] { 2, 4, 6 });
        }
    }
}
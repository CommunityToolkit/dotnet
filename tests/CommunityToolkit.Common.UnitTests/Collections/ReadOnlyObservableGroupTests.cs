// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collections
{
    [TestClass]
    public class ReadOnlyObservableGroupTests
    {
        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithKeyAndOBservableCollection_ShouldHaveExpectedInitialState()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var group = new ReadOnlyObservableGroup<string, int>("key", source);

            Assert.AreEqual(group.Key, "key");
            CollectionAssert.AreEqual(group, new[] { 1, 2, 3 });
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_ObservableGroup_ShouldHaveExpectedInitialState()
        {
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);

            Assert.AreEqual(group.Key, "key");
            CollectionAssert.AreEqual(group, new[] { 1, 2, 3 });
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithKeyAndCollection_ShouldHaveExpectedInitialState()
        {
            var source = new[] { 1, 2, 3 };
            var group = new ReadOnlyObservableGroup<string, int>("key", source);

            Assert.AreEqual(group.Key, "key");
            CollectionAssert.AreEqual(group, new[] { 1, 2, 3 });
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Add_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            sourceGroup.Add(4);

            Assert.AreEqual(group.Key, "key");
            CollectionAssert.AreEqual(group, new[] { 1, 2, 3, 4 });

            Assert.IsTrue(collectionChangedEventRaised);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Update_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            sourceGroup[1] = 4;

            Assert.AreEqual(group.Key, "key");
            CollectionAssert.AreEqual(group, new[] { 1, 4, 3 });

            Assert.IsTrue(collectionChangedEventRaised);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Remove_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            sourceGroup.Remove(1);

            Assert.AreEqual(group.Key, "key");
            CollectionAssert.AreEqual(group, new[] { 2, 3 });

            Assert.IsTrue(collectionChangedEventRaised);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Clear_ShouldRaiseEvent()
        {
            var collectionChangedEventRaised = false;
            var source = new[] { 1, 2, 3 };
            var sourceGroup = new ObservableGroup<string, int>("key", source);
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            ((INotifyCollectionChanged)group).CollectionChanged += (s, e) => collectionChangedEventRaised = true;

            sourceGroup.Clear();

            Assert.AreEqual(group.Key, "key");
            CollectionAssert.AreEqual(group, Array.Empty<int>());

            Assert.IsTrue(collectionChangedEventRaised);
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(3)]
        public void IReadOnlyObservableGroup_ShouldReturnExpectedValues(int count)
        {
            var sourceGroup = new ObservableGroup<string, int>("key", Enumerable.Range(0, count));
            var group = new ReadOnlyObservableGroup<string, int>(sourceGroup);
            var iReadOnlyObservableGroup = (IReadOnlyObservableGroup)group;

            Assert.AreEqual(iReadOnlyObservableGroup.Key, "key");
            Assert.AreEqual(iReadOnlyObservableGroup.Count, count);
        }
    }
}
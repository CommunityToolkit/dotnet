// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public class Test_ObservableObject
{
    [TestMethod]
    public void Test_ObservableObject_Events()
    {
        SampleModel<int>? model = new();

        (PropertyChangingEventArgs, int) changing = default;
        (PropertyChangedEventArgs, int) changed = default;

        model.PropertyChanging += (s, e) =>
        {
            Assert.IsNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changing = (e, model.Data);
        };

        model.PropertyChanged += (s, e) =>
        {
            Assert.IsNotNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changed = (e, model.Data);
        };

        model.Data = 42;

        Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModel<int>.Data));
        Assert.AreEqual(changing.Item2, 0);
        Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel<int>.Data));
        Assert.AreEqual(changed.Item2, 42);
    }

    public class SampleModel<T> : ObservableObject
    {
        private T? data;

        public T? Data
        {
            get => data;
            set => SetProperty(ref data, value);
        }
    }

    [TestMethod]
    public void Test_ObservableObject_ProxyCrudWithProperty()
    {
        WrappingModelWithProperty? model = new(new Person { Name = "Alice" });

        (PropertyChangingEventArgs?, string?) changing = default;
        (PropertyChangedEventArgs?, string?) changed = default;

        model.PropertyChanging += (s, e) =>
        {
            Assert.AreSame(model, s);

            changing = (e, model.Name);
        };

        model.PropertyChanged += (s, e) =>
        {
            Assert.AreSame(model, s);

            changed = (e, model.Name);
        };

        model.Name = "Bob";

        Assert.AreEqual(changing.Item1?.PropertyName, nameof(WrappingModelWithProperty.Name));
        Assert.AreEqual(changing.Item2, "Alice");
        Assert.AreEqual(changed.Item1?.PropertyName, nameof(WrappingModelWithProperty.Name));
        Assert.AreEqual(changed.Item2, "Bob");
        Assert.AreEqual(model.PersonProxy.Name, "Bob");
    }

    public class Person
    {
        public string? Name { get; set; }
    }

    public class WrappingModelWithProperty : ObservableObject
    {
        private Person Person { get; }

        public WrappingModelWithProperty(Person person)
        {
            Person = person;
        }

        public Person PersonProxy => Person;

        public string? Name
        {
            get => Person.Name;
            set => SetProperty(Person.Name, value, Person, (person, name) => person.Name = name);
        }
    }

    [TestMethod]
    public void Test_ObservableObject_ProxyCrudWithField()
    {
        WrappingModelWithField? model = new(new Person { Name = "Alice" });

        (PropertyChangingEventArgs?, string?) changing = default;
        (PropertyChangedEventArgs?, string?) changed = default;

        model.PropertyChanging += (s, e) =>
        {
            Assert.AreSame(model, s);

            changing = (e, model.Name);
        };

        model.PropertyChanged += (s, e) =>
        {
            Assert.AreSame(model, s);

            changed = (e, model.Name);
        };

        model.Name = "Bob";

        Assert.AreEqual(changing.Item1?.PropertyName, nameof(WrappingModelWithField.Name));
        Assert.AreEqual(changing.Item2, "Alice");
        Assert.AreEqual(changed.Item1?.PropertyName, nameof(WrappingModelWithField.Name));
        Assert.AreEqual(changed.Item2, "Bob");
        Assert.AreEqual(model.PersonProxy.Name, "Bob");
    }

    public class WrappingModelWithField : ObservableObject
    {
        private readonly Person person;

        public WrappingModelWithField(Person person)
        {
            this.person = person;
        }

        public Person PersonProxy => this.person;

        public string? Name
        {
            get => this.person.Name;
            set => SetProperty(this.person.Name, value, this.person, (person, name) => person.Name = name);
        }
    }

    [TestMethod]
    public async Task Test_ObservableObject_NotifyTask()
    {
        static async Task TestAsync(Action<TaskCompletionSource<int>> callback)
        {
            SampleModelWithTask<int>? model = new();
            TaskCompletionSource<int>? tcs = new();
            Task<int>? task = tcs.Task;

            (PropertyChangingEventArgs?, Task<int>?) changing = default;
            (PropertyChangedEventArgs?, Task<int>?) changed = default;

            model.PropertyChanging += (s, e) =>
            {
                Assert.AreSame(model, s);

                changing = (e, model.Data);
            };

            model.PropertyChanged += (s, e) =>
            {
                Assert.AreSame(model, s);

                changed = (e, model.Data);
            };

            model.Data = task;

            Assert.IsFalse(task.IsCompleted);
            Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModelWithTask<int>.Data));
            Assert.IsNull(changing.Item2);
            Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModelWithTask<int>.Data));
            Assert.AreSame(changed.Item2, task);

            changed = default;

            callback(tcs);

            await Task.Delay(100); // Time for the notification to dispatch

            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel<int>.Data));
            Assert.AreSame(changed.Item2, task);
        }

        await TestAsync(tcs => tcs.SetResult(42));
        await TestAsync(tcs => tcs.SetException(new ArgumentException("Something went wrong")));
        await TestAsync(tcs => tcs.SetCanceled());
    }

    public class SampleModelWithTask<T> : ObservableObject
    {
        private TaskNotifier<T>? data;

        public Task<T>? Data
        {
            get => data;
            set => SetPropertyAndNotifyOnCompletion(ref data, value);
        }
    }

    [TestMethod]
    public async Task Test_ObservableObject_ThrowingTaskBubblesToUnobservedTaskException()
    {
        SampleModelWithTask model = new();

        static async Task TestMethodAsync(Action action)
        {
            await Task.Delay(100);

            action();
        }

        async void TestCallback(Action throwAction, Action completeAction)
        {
            model.Data = TestMethodAsync(throwAction);
            model.Data = null;

            await Task.Delay(200);

            completeAction();
        }

        bool success = await TaskSchedulerTestHelper.IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback);

        Assert.IsTrue(success);
    }

    [TestMethod]
    public async Task Test_ObservableObject_ThrowingTaskOfTBubblesToUnobservedTaskException()
    {
        SampleModelWithTask<int> model = new();

        static async Task<int> TestMethodAsync(Action action)
        {
            await Task.Delay(100);

            action();

            return 42;
        }

        async void TestCallback(Action throwAction, Action completeAction)
        {
            model.Data = TestMethodAsync(throwAction);
            model.Data = null;

            await Task.Delay(200);

            completeAction();
        }

        bool success = await TaskSchedulerTestHelper.IsExceptionBubbledUpToUnobservedTaskExceptionAsync(TestCallback);

        Assert.IsTrue(success);
    }

    public class SampleModelWithTask : ObservableObject
    {
        private TaskNotifier? taskNotifier;

        public Task? Data
        {
            get => taskNotifier;
            set => SetPropertyAndNotifyOnCompletion(ref taskNotifier, value);
        }
    }
}

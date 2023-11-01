// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable MVVMTK0033

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_ObservableRecipientAttribute
{
    [TestMethod]
    public void Test_ObservableRecipientAttribute_Events()
    {
        Person? model = new();
        List<PropertyChangedEventArgs>? args = new();

        model.PropertyChanged += (s, e) => args.Add(e);

        Assert.IsFalse(model.HasErrors);

        model.Name = "No";

        Assert.IsTrue(model.HasErrors);
        Assert.AreEqual(2, args.Count);
        Assert.AreEqual(nameof(Person.Name), args[0].PropertyName);
        Assert.AreEqual(nameof(INotifyDataErrorInfo.HasErrors), args[1].PropertyName);

        model.Name = "Valid";

        Assert.IsFalse(model.HasErrors);
        Assert.AreEqual(4, args.Count);
        Assert.AreEqual(nameof(Person.Name), args[2].PropertyName);
        Assert.AreEqual(nameof(INotifyDataErrorInfo.HasErrors), args[3].PropertyName);

        Assert.IsNotNull(typeof(Person).GetProperty("Messenger", BindingFlags.Instance | BindingFlags.NonPublic));
        Assert.AreEqual(0, typeof(Person).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Length);
    }

    [ObservableRecipient]
    public partial class Person : ObservableValidator
    {
        public Person()
        {
            Messenger = WeakReferenceMessenger.Default;
        }

        private string? name;

        [MinLength(4)]
        [MaxLength(20)]
        [Required]
        public string? Name
        {
            get => this.name;
            set => SetProperty(ref this.name, value, true);
        }

        public void TestCompile()
        {
            // Validates that the method Broadcast is correctly being generated
            Broadcast(0, 1, nameof(TestCompile));
        }
    }

    [TestMethod]
    public void Test_ObservableRecipientAttribute_AbstractConstructors()
    {
        ConstructorInfo[]? ctors = typeof(AbstractPerson).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.AreEqual(2, ctors.Length);
        Assert.IsTrue(ctors.All(static ctor => ctor.IsFamily));
    }

    [ObservableRecipient]
    public abstract partial class AbstractPerson : ObservableObject
    {
    }

    [TestMethod]
    public void Test_ObservableRecipientAttribute_NonAbstractConstructors()
    {
        ConstructorInfo[]? ctors = typeof(NonAbstractPerson).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.AreEqual(2, ctors.Length);
        Assert.IsTrue(ctors.All(static ctor => ctor.IsPublic));
    }

    [ObservableRecipient]
    public partial class NonAbstractPerson : ObservableObject
    {
    }

    [TestMethod]
    public void Test_ObservableRecipientAttribute_TrimmingAnnoations_IsActive()
    {
        bool shouldHaveTrimmingAnnotations =
#if NET6_0_OR_GREATER
            true;
#else
            false;
#endif

        MethodInfo isActivePropertySetter = typeof(TestRecipient).GetProperty(nameof(TestRecipient.IsActive))!.SetMethod!;

        if (shouldHaveTrimmingAnnotations)
        {
            Attribute[] attributes = isActivePropertySetter.GetCustomAttributes().ToArray();

            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual("System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute", attributes[0].GetType().ToString());
        }
        else
        {
            Attribute[] attributes = isActivePropertySetter.GetCustomAttributes().ToArray();

            Assert.AreEqual(0, attributes.Length);
        }
    }

    [TestMethod]
    public void Test_ObservableRecipientAttribute_TrimmingAnnoations_OnActivated()
    {
        bool shouldHaveTrimmingAnnotations =
#if NET6_0_OR_GREATER
            true;
#else
            false;
#endif

        MethodInfo onActivatedMethod = typeof(TestRecipient).GetMethod("OnActivated", BindingFlags.Instance | BindingFlags.NonPublic)!;
        IEnumerable<Attribute> attributes = onActivatedMethod.GetCustomAttributes();

        if (shouldHaveTrimmingAnnotations)
        {
            Assert.IsTrue(attributes.Any(static a => a.GetType().ToString() == "System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute"));
        }
        else
        {
            Assert.IsFalse(attributes.Any(static a => a.GetType().ToString() == "System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute"));
        }
    }

    [ObservableRecipient]
    public abstract partial class TestRecipient : ObservableObject
    {
    }

    [TestMethod]
    public void Test_ObservableRecipientAttribute_PersonWithCustomOnActivated()
    {
        PersonWithCustomOnActivated model = new();

        model.IsActive = true;

        Assert.IsTrue(model.Result);
    }

    [ObservableRecipient]
    public partial class PersonWithCustomOnActivated : ObservableObject
    {
        public bool Result { get; private set; }

        protected virtual partial void OnActivated()
        {
            Result = true;
        }
    }

    [TestMethod]
    public void Test_ObservableRecipientAttribute_PersonWithCustomOnDeactivated()
    {
        PersonWithCustomOnDeactivated model = new();

        model.IsActive = true;
        model.IsActive = false;

        Assert.IsTrue(model.Result);
    }

    [ObservableRecipient]
    public partial class PersonWithCustomOnDeactivated : ObservableObject
    {
        public bool Result { get; private set; }

        protected virtual partial void OnDeactivated()
        {
            Result = true;
        }
    }

    [TestMethod]
    public void Test_ObservableRecipientAttribute_PersonWithCustomOnActivatedAndOnDeactivated()
    {
        PersonWithCustomOnActivatedAndOnDeactivated model = new();

        model.IsActive = true;

        Assert.IsTrue(model.OnActivatedResult);

        model.IsActive = false;

        Assert.IsTrue(model.OnDeactivatedResult);
    }

    [ObservableRecipient]
    public partial class PersonWithCustomOnActivatedAndOnDeactivated : ObservableObject
    {
        public bool OnActivatedResult { get; private set; }

        public bool OnDeactivatedResult { get; private set; }

        protected virtual partial void OnActivated()
        {
            OnActivatedResult = true;
        }

        protected virtual partial void OnDeactivated()
        {
            OnDeactivatedResult = true;
        }
    }

    [TestMethod]
    public void Test_ObservableRecipientAttribute_SealedPersonWithCustomOnActivatedAndOnDeactivated()
    {
        SealedPersonWithCustomOnActivatedAndOnDeactivated model = new();

        model.IsActive = true;

        Assert.IsTrue(model.OnActivatedResult);

        model.IsActive = false;

        Assert.IsTrue(model.OnDeactivatedResult);
    }

    [ObservableRecipient]
    public sealed partial class SealedPersonWithCustomOnActivatedAndOnDeactivated : ObservableObject
    {
        public bool OnActivatedResult { get; private set; }

        public bool OnDeactivatedResult { get; private set; }

        private partial void OnActivated()
        {
            OnActivatedResult = true;
        }

        private partial void OnDeactivated()
        {
            OnDeactivatedResult = true;
        }
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/109
    [TestMethod]
    public void Test_ObservableRecipientAttribute_WorksWithBaseClassWithObservableObjectAttribute()
    {
        ViewModelWithOnlyObservableRecipientAttribute model = new();

        // This test method really only needs the two classes below to compile at all
        Assert.IsTrue(model is INotifyPropertyChanged); // From [ObservableObject]
        Assert.IsFalse(model.IsActive); // From [ObservableRecipient]
    }

    [ObservableObject]
    public partial class BaseViewModelWithObservableObjectAttribute
    {
    }

    [ObservableRecipient]
    public partial class ViewModelWithOnlyObservableRecipientAttribute : BaseViewModelWithObservableObjectAttribute
    {
    }
}

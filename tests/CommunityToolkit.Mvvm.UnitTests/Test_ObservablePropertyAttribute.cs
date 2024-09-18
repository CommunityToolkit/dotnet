// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Reflection;
#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ExternalAssembly;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable MVVMTK0032, MVVMTK0033, MVVMTK0034

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public partial class Test_ObservablePropertyAttribute
{
    [TestMethod]
    public void Test_ObservablePropertyAttribute_Events()
    {
        SampleModel model = new();

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

        Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModel.Data));
        Assert.AreEqual(changing.Item2, 0);
        Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel.Data));
        Assert.AreEqual(changed.Item2, 42);
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4225
    [TestMethod]
    public void Test_ObservablePropertyAttributeWithinRegion_Events()
    {
        SampleModel model = new();

        (PropertyChangingEventArgs, int) changing = default;
        (PropertyChangedEventArgs, int) changed = default;

        model.PropertyChanging += (s, e) =>
        {
            Assert.IsNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changing = (e, model.Counter);
        };

        model.PropertyChanged += (s, e) =>
        {
            Assert.IsNotNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changed = (e, model.Counter);
        };

        model.Counter = 42;

        Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModel.Counter));
        Assert.AreEqual(changing.Item2, 0);
        Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel.Counter));
        Assert.AreEqual(changed.Item2, 42);
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4225
    [TestMethod]
    public void Test_ObservablePropertyAttributeRightBelowRegion_Events()
    {
        SampleModel model = new();

        (PropertyChangingEventArgs, string?) changing = default;
        (PropertyChangedEventArgs, string?) changed = default;

        model.PropertyChanging += (s, e) =>
        {
            Assert.IsNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changing = (e, model.Name);
        };

        model.PropertyChanged += (s, e) =>
        {
            Assert.IsNotNull(changing.Item1);
            Assert.IsNull(changed.Item1);
            Assert.AreSame(model, s);
            Assert.IsNotNull(s);
            Assert.IsNotNull(e);

            changed = (e, model.Name);
        };

        model.Name = "Bob";

        Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModel.Name));
        Assert.AreEqual(changing.Item2, null);
        Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel.Name));
        Assert.AreEqual(changed.Item2, "Bob");
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedForAttribute_Events()
    {
        DependentPropertyModel model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        model.Name = "Bob";
        model.Surname = "Ross";

        CollectionAssert.AreEqual(new[] { nameof(model.Name), nameof(model.FullName), nameof(model.Alias), nameof(model.Surname), nameof(model.FullName), nameof(model.Alias) }, propertyNames);
    }

    [TestMethod]
    public void Test_ValidationAttributes()
    {
        PropertyInfo nameProperty = typeof(MyFormViewModel).GetProperty(nameof(MyFormViewModel.Name))!;

        Assert.IsNotNull(nameProperty.GetCustomAttribute<RequiredAttribute>());
        Assert.IsNotNull(nameProperty.GetCustomAttribute<MinLengthAttribute>());
        Assert.AreEqual(nameProperty.GetCustomAttribute<MinLengthAttribute>()!.Length, 1);
        Assert.IsNotNull(nameProperty.GetCustomAttribute<MaxLengthAttribute>());
        Assert.AreEqual(nameProperty.GetCustomAttribute<MaxLengthAttribute>()!.Length, 100);

        PropertyInfo ageProperty = typeof(MyFormViewModel).GetProperty(nameof(MyFormViewModel.Age))!;

        Assert.IsNotNull(ageProperty.GetCustomAttribute<RangeAttribute>());
        Assert.AreEqual(ageProperty.GetCustomAttribute<RangeAttribute>()!.Minimum, 0);
        Assert.AreEqual(ageProperty.GetCustomAttribute<RangeAttribute>()!.Maximum, 120);

        PropertyInfo emailProperty = typeof(MyFormViewModel).GetProperty(nameof(MyFormViewModel.Email))!;

        Assert.IsNotNull(emailProperty.GetCustomAttribute<EmailAddressAttribute>());

        PropertyInfo comboProperty = typeof(MyFormViewModel).GetProperty(nameof(MyFormViewModel.IfThisWorksThenThatsGreat))!;

        TestValidationAttribute testAttribute = comboProperty.GetCustomAttribute<TestValidationAttribute>()!;

        Assert.IsNotNull(testAttribute);
        Assert.IsNull(testAttribute.O);
        Assert.AreEqual(testAttribute.T, typeof(SampleModel));
        Assert.AreEqual(testAttribute.Flag, true);
        Assert.AreEqual(testAttribute.D, 6.28);
        CollectionAssert.AreEqual(testAttribute.Names, new[] { "Bob", "Ross" });

        object[]? nestedArray = (object[]?)testAttribute.NestedArray;

        Assert.IsNotNull(nestedArray);
        Assert.AreEqual(nestedArray!.Length, 3);
        Assert.AreEqual(nestedArray[0], 1);
        Assert.AreEqual(nestedArray[1], "Hello");
        Assert.IsTrue(nestedArray[2] is int[]);
        CollectionAssert.AreEqual((int[])nestedArray[2], new[] { 2, 3, 4 });

        Assert.AreEqual(testAttribute.Animal, Animal.Llama);
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4216
    [TestMethod]
    public void Test_ObservablePropertyWithValueNamedField()
    {
        ModelWithValueProperty model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        model.Value = "Hello world";

        Assert.AreEqual(model.Value, "Hello world");

        CollectionAssert.AreEqual(new[] { nameof(model.Value) }, propertyNames);
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4216
    [TestMethod]
    public void Test_ObservablePropertyWithValueNamedField_WithValidationAttributes()
    {
        ModelWithValuePropertyWithValidation model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        bool errorsChanged = false;

        model.ErrorsChanged += (s, e) => errorsChanged = true;

        model.Value = "Hello world";

        Assert.AreEqual(model.Value, "Hello world");

        // The [NotifyDataErrorInfo] attribute wasn't used, so the property shouldn't be validated
        Assert.IsFalse(errorsChanged);

        CollectionAssert.AreEqual(new[] { nameof(model.Value) }, propertyNames);
    }

    [TestMethod]
    public void Test_ObservablePropertyWithValueNamedField_WithValidationAttributesAndValidation()
    {
        ModelWithValuePropertyWithAutomaticValidation model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        List<DataErrorsChangedEventArgs> errors = new();

        model.ErrorsChanged += (s, e) => errors.Add(e);

        model.Value = "Bo";

        Assert.IsTrue(model.HasErrors);
        Assert.AreEqual(errors.Count, 1);
        Assert.AreEqual(errors[0].PropertyName, nameof(ModelWithValuePropertyWithAutomaticValidation.Value));

        model.Value = "Hello world";

        Assert.IsFalse(model.HasErrors);
        Assert.AreEqual(errors.Count, 2);
        Assert.AreEqual(errors[1].PropertyName, nameof(ModelWithValuePropertyWithAutomaticValidation.Value));
    }

    [TestMethod]
    public void Test_ObservablePropertyWithValueNamedField_WithValidationAttributesAndValidation_WithClassLevelAttribute()
    {
        ModelWithValuePropertyWithAutomaticValidationWithClassLevelAttribute model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        List<DataErrorsChangedEventArgs> errors = new();

        model.ErrorsChanged += (s, e) => errors.Add(e);

        model.Value = "Bo";

        Assert.IsTrue(model.HasErrors);
        Assert.AreEqual(errors.Count, 1);
        Assert.AreEqual(errors[0].PropertyName, nameof(ModelWithValuePropertyWithAutomaticValidationWithClassLevelAttribute.Value));

        model.Value = "Hello world";

        Assert.IsFalse(model.HasErrors);
        Assert.AreEqual(errors.Count, 2);
        Assert.AreEqual(errors[1].PropertyName, nameof(ModelWithValuePropertyWithAutomaticValidationWithClassLevelAttribute.Value));
    }

    [TestMethod]
    public void Test_ObservablePropertyWithValueNamedField_WithValidationAttributesAndValidation_InheritingClassLevelAttribute()
    {
        ModelWithValuePropertyWithAutomaticValidationInheritingClassLevelAttribute model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        List<DataErrorsChangedEventArgs> errors = new();

        model.ErrorsChanged += (s, e) => errors.Add(e);

        model.Value2 = "Bo";

        Assert.IsTrue(model.HasErrors);
        Assert.AreEqual(errors.Count, 1);
        Assert.AreEqual(errors[0].PropertyName, nameof(ModelWithValuePropertyWithAutomaticValidationInheritingClassLevelAttribute.Value2));

        model.Value2 = "Hello world";

        Assert.IsFalse(model.HasErrors);
        Assert.AreEqual(errors.Count, 2);
        Assert.AreEqual(errors[1].PropertyName, nameof(ModelWithValuePropertyWithAutomaticValidationInheritingClassLevelAttribute.Value2));
    }

    // See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4184
    [TestMethod]
    public void Test_GeneratedPropertiesWithValidationAttributesOverFields()
    {
        ViewModelWithValidatableGeneratedProperties model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        // Assign these fields directly to bypass the validation that is executed in the generated setters.
        // We only need those generated properties to be there to check whether they are correctly detected.
        model.first = "A";
        model.last = "This is a very long name that exceeds the maximum length of 60 for this property";

        Assert.IsFalse(model.HasErrors);

        model.RunValidation();

        Assert.IsTrue(model.HasErrors);

        ValidationResult[] validationErrors = model.GetErrors().ToArray();

        Assert.AreEqual(validationErrors.Length, 2);

        CollectionAssert.AreEqual(new[] { nameof(ViewModelWithValidatableGeneratedProperties.First) }, validationErrors[0].MemberNames.ToArray());
        CollectionAssert.AreEqual(new[] { nameof(ViewModelWithValidatableGeneratedProperties.Last) }, validationErrors[1].MemberNames.ToArray());
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedFor()
    {
        DependentPropertyModel model = new();

        List<string?> propertyNames = new();
        int canExecuteRequests = 0;

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
        model.MyCommand.CanExecuteChanged += (s, e) => canExecuteRequests++;

        model.Surname = "Ross";

        Assert.AreEqual(1, canExecuteRequests);

        CollectionAssert.AreEqual(new[] { nameof(model.Surname), nameof(model.FullName), nameof(model.Alias) }, propertyNames);
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedFor_GeneratedCommand()
    {
        DependentPropertyModel2 model = new();

        List<string?> propertyNames = new();
        int canExecuteRequests = 0;

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
        model.TestFromMethodCommand.CanExecuteChanged += (s, e) => canExecuteRequests++;

        model.Text = "Ross";

        Assert.AreEqual(1, canExecuteRequests);

        CollectionAssert.AreEqual(new[] { nameof(model.Text), nameof(model.FullName), nameof(model.Alias) }, propertyNames);
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedFor_IRelayCommandProperty()
    {
        DependentPropertyModel3 model = new();

        List<string?> propertyNames = new();
        int canExecuteRequests = 0;

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
        model.MyCommand.CanExecuteChanged += (s, e) => canExecuteRequests++;

        model.Text = "Ross";

        Assert.AreEqual(1, canExecuteRequests);

        CollectionAssert.AreEqual(new[] { nameof(model.Text), nameof(model.FullName), nameof(model.Alias) }, propertyNames);
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedFor_IAsyncRelayCommandOfTProperty()
    {
        DependentPropertyModel4 model = new();

        List<string?> propertyNames = new();
        int canExecuteRequests = 0;

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
        model.MyCommand.CanExecuteChanged += (s, e) => canExecuteRequests++;

        model.Text = "Ross";

        Assert.AreEqual(1, canExecuteRequests);

        CollectionAssert.AreEqual(new[] { nameof(model.Text), nameof(model.FullName), nameof(model.Alias) }, propertyNames);
    }

    [TestMethod]
    public void Test_OnPropertyChangingAndChangedPartialMethods()
    {
        ViewModelWithImplementedUpdateMethods model = new();

        model.Name = nameof(Test_OnPropertyChangingAndChangedPartialMethods);

        Assert.AreEqual(nameof(Test_OnPropertyChangingAndChangedPartialMethods), model.NameChangingValue);
        Assert.AreEqual(nameof(Test_OnPropertyChangingAndChangedPartialMethods), model.NameChangedValue);

        model.Number = 99;

        Assert.AreEqual(99, model.NumberChangedValue);
    }

    [TestMethod]
    public void Test_OnPropertyChangingAndChangedPartialMethods_WithPreviousValues()
    {
        ViewModelWithImplementedUpdateMethods2 model = new();

        Assert.AreEqual(null, model.Name);
        Assert.AreEqual(0, model.Number);

        CollectionAssert.AreEqual(Array.Empty<(string, string)>(), model.OnNameChangingValues);
        CollectionAssert.AreEqual(Array.Empty<(string, string)>(), model.OnNameChangedValues);
        CollectionAssert.AreEqual(Array.Empty<(int, int)>(), model.OnNumberChangingValues);
        CollectionAssert.AreEqual(Array.Empty<(int, int)>(), model.OnNumberChangedValues);

        model.Name = "Bob";

        CollectionAssert.AreEqual(new[] { ((string?)null, "Bob") }, model.OnNameChangingValues);
        CollectionAssert.AreEqual(new[] { ((string?)null, "Bob") }, model.OnNameChangedValues);

        Assert.AreEqual("Bob", model.Name);

        CollectionAssert.AreEqual(new[] { ((string?)null, "Bob") }, model.OnNameChangingValues);
        CollectionAssert.AreEqual(new[] { ((string?)null, "Bob") }, model.OnNameChangedValues);

        model.Name = "Alice";

        CollectionAssert.AreEqual(new[] { (null, "Bob"), ("Bob", "Alice") }, model.OnNameChangingValues);
        CollectionAssert.AreEqual(new[] { (null, "Bob"), ("Bob", "Alice") }, model.OnNameChangedValues);

        Assert.AreEqual("Alice", model.Name);

        CollectionAssert.AreEqual(new[] { (null, "Bob"), ("Bob", "Alice") }, model.OnNameChangingValues);
        CollectionAssert.AreEqual(new[] { (null, "Bob"), ("Bob", "Alice") }, model.OnNameChangedValues);

        model.Number = 42;

        CollectionAssert.AreEqual(new[] { (0, 42) }, model.OnNumberChangingValues);
        CollectionAssert.AreEqual(new[] { (0, 42) }, model.OnNumberChangedValues);

        Assert.AreEqual(42, model.Number);

        CollectionAssert.AreEqual(new[] { (0, 42) }, model.OnNumberChangingValues);
        CollectionAssert.AreEqual(new[] { (0, 42) }, model.OnNumberChangedValues);

        model.Number = 77;

        CollectionAssert.AreEqual(new[] { (0, 42), (42, 77) }, model.OnNumberChangingValues);
        CollectionAssert.AreEqual(new[] { (0, 42), (42, 77) }, model.OnNumberChangedValues);

        Assert.AreEqual(77, model.Number);

        CollectionAssert.AreEqual(new[] { (0, 42), (42, 77) }, model.OnNumberChangingValues);
        CollectionAssert.AreEqual(new[] { (0, 42), (42, 77) }, model.OnNumberChangedValues);
    }

    [TestMethod]
    public void Test_OnPropertyChangingAndChangedPartialMethodWithAdditionalValidation()
    {
        ViewModelWithImplementedUpdateMethodAndAdditionalValidation model = new();

        // The actual validation is performed inside the model itself.
        // This test validates that the order with which methods/events are generated is:
        //   - On<PROPERTY_NAME>Changing(value);
        //   - OnPropertyChanging();
        //   - field = value;
        //   - On<PROPERTY_NAME>Changed(value);
        //   - OnPropertyChanged();
        model.Name = "B";

        Assert.AreEqual("B", model.Name);
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedRecipients_WithObservableObject()
    {
        Test_NotifyPropertyChangedRecipients_Test(
           factory: static messenger => new BroadcastingViewModel(messenger),
           setter: static (model, value) => model.Name = value,
           propertyName: nameof(BroadcastingViewModel.Name));
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedRecipients_WithObservableRecipientAttribute()
    {
        Test_NotifyPropertyChangedRecipients_Test(
            factory: static messenger => new BroadcastingViewModelWithAttribute(messenger),
            setter: static (model, value) => model.Name = value,
            propertyName: nameof(BroadcastingViewModelWithAttribute.Name));
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedRecipients_WithInheritedObservableRecipientAttribute()
    {
        Test_NotifyPropertyChangedRecipients_Test(
            factory: static messenger => new BroadcastingViewModelWithInheritedAttribute(messenger),
            setter: static (model, value) => model.Name2 = value,
            propertyName: nameof(BroadcastingViewModelWithInheritedAttribute.Name2));
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedRecipients_WithObservableObject_WithClassLevelAttribute()
    {
        Test_NotifyPropertyChangedRecipients_Test(
           factory: static messenger => new BroadcastingViewModelWithClassLevelAttribute(messenger),
           setter: static (model, value) => model.Name = value,
           propertyName: nameof(BroadcastingViewModelWithClassLevelAttribute.Name));
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedRecipients_WithObservableRecipientAttribute_WithClassLevelAttribute()
    {
        Test_NotifyPropertyChangedRecipients_Test(
            factory: static messenger => new BroadcastingViewModelWithAttributeAndClassLevelAttribute(messenger),
            setter: static (model, value) => model.Name = value,
            propertyName: nameof(BroadcastingViewModelWithAttributeAndClassLevelAttribute.Name));
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedRecipients_WithInheritedObservableRecipientAttribute_WithClassLevelAttribute()
    {
        Test_NotifyPropertyChangedRecipients_Test(
            factory: static messenger => new BroadcastingViewModelWithInheritedClassLevelAttribute(messenger),
            setter: static (model, value) => model.Name2 = value,
            propertyName: nameof(BroadcastingViewModelWithInheritedClassLevelAttribute.Name2));
    }

    [TestMethod]
    public void Test_NotifyPropertyChangedRecipients_WithInheritedObservableRecipientAttributeAndClassLevelAttribute()
    {
        Test_NotifyPropertyChangedRecipients_Test(
            factory: static messenger => new BroadcastingViewModelWithInheritedAttributeAndClassLevelAttribute(messenger),
            setter: static (model, value) => model.Name2 = value,
            propertyName: nameof(BroadcastingViewModelWithInheritedAttributeAndClassLevelAttribute.Name2));
    }

    private void Test_NotifyPropertyChangedRecipients_Test<T>(Func<IMessenger, T> factory, Action<T, string?> setter, string propertyName)
        where T : notnull
    {
        IMessenger messenger = new StrongReferenceMessenger();

        T model = factory(messenger);

        List<(object Sender, PropertyChangedMessage<string?> Message)> messages = new();

        messenger.Register<PropertyChangedMessage<string?>>(model, (r, m) => messages.Add((r, m)));

        setter(model, "Bob");

        Assert.AreEqual(1, messages.Count);
        Assert.AreSame(model, messages[0].Sender);
        Assert.AreEqual(null, messages[0].Message.OldValue);
        Assert.AreEqual("Bob", messages[0].Message.NewValue);
        Assert.AreEqual(propertyName, messages[0].Message.PropertyName);

        setter(model, "Ross");

        Assert.AreEqual(2, messages.Count);
        Assert.AreSame(model, messages[1].Sender);
        Assert.AreEqual("Bob", messages[1].Message.OldValue);
        Assert.AreEqual("Ross", messages[1].Message.NewValue);
        Assert.AreEqual(propertyName, messages[0].Message.PropertyName);
    }

    [TestMethod]
    public void Test_ObservableProperty_ObservableRecipientDoesNotBroadcastByDefault()
    {
        IMessenger messenger = new StrongReferenceMessenger();
        RecipientWithNonBroadcastingProperty model = new(messenger);

        List<(object Sender, PropertyChangedMessage<string?> Message)> messages = new();

        messenger.Register<PropertyChangedMessage<string?>>(model, (r, m) => messages.Add((r, m)));

        model.Name = "Bob";
        model.Name = "Alice";
        model.Name = null;

        // The [NotifyPropertyChangedRecipients] attribute wasn't used, so no messages should have been sent
        Assert.AreEqual(messages.Count, 0);
    }

#if NET6_0_OR_GREATER
    // See https://github.com/CommunityToolkit/dotnet/issues/155
    [TestMethod]
    public void Test_ObservableProperty_NullabilityAnnotations_Simple()
    {
        // List<string?>?
        NullabilityInfoContext context = new();
        NullabilityInfo info = context.Create(typeof(NullableRepro).GetProperty(nameof(NullableRepro.NullableList))!);

        Assert.AreEqual(typeof(List<string>), info.Type);
        Assert.AreEqual(NullabilityState.Nullable, info.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, info.WriteState);
        Assert.AreEqual(1, info.GenericTypeArguments.Length);

        NullabilityInfo elementInfo = info.GenericTypeArguments[0];

        Assert.AreEqual(typeof(string), elementInfo.Type);
        Assert.AreEqual(NullabilityState.Nullable, elementInfo.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, elementInfo.WriteState);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/155
    [TestMethod]
    public void Test_ObservableProperty_NullabilityAnnotations_Complex()
    {
        // Foo<Foo<string?, int>.Bar<object?>?, StrongBox<Foo<int, string?>.Bar<object>?>?>?
        NullabilityInfoContext context = new();
        NullabilityInfo info = context.Create(typeof(NullableRepro).GetProperty(nameof(NullableRepro.NullableMess))!);

        Assert.AreEqual(typeof(Foo<Foo<string?, int>.Bar<object?>?, StrongBox<Foo<int, string?>.Bar<object>?>?>), info.Type);
        Assert.AreEqual(NullabilityState.Nullable, info.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, info.WriteState);
        Assert.AreEqual(2, info.GenericTypeArguments.Length);

        NullabilityInfo leftInfo = info.GenericTypeArguments[0];

        Assert.AreEqual(typeof(Foo<string?, int>.Bar<object?>), leftInfo.Type);
        Assert.AreEqual(NullabilityState.Nullable, leftInfo.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, leftInfo.WriteState);
        Assert.AreEqual(3, leftInfo.GenericTypeArguments.Length);

        NullabilityInfo leftInfo0 = leftInfo.GenericTypeArguments[0];

        Assert.AreEqual(typeof(string), leftInfo0.Type);
        Assert.AreEqual(NullabilityState.Nullable, leftInfo0.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, leftInfo0.WriteState);

        NullabilityInfo leftInfo1 = leftInfo.GenericTypeArguments[1];

        Assert.AreEqual(typeof(int), leftInfo1.Type);
        Assert.AreEqual(NullabilityState.NotNull, leftInfo1.ReadState);
        Assert.AreEqual(NullabilityState.NotNull, leftInfo1.WriteState);

        NullabilityInfo leftInfo2 = leftInfo.GenericTypeArguments[2];

        Assert.AreEqual(typeof(object), leftInfo2.Type);
        Assert.AreEqual(NullabilityState.Nullable, leftInfo2.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, leftInfo2.WriteState);

        NullabilityInfo rightInfo = info.GenericTypeArguments[1];

        Assert.AreEqual(typeof(StrongBox<Foo<int, string?>.Bar<object>?>), rightInfo.Type);
        Assert.AreEqual(NullabilityState.Nullable, rightInfo.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, rightInfo.WriteState);
        Assert.AreEqual(1, rightInfo.GenericTypeArguments.Length);

        NullabilityInfo rightInnerInfo = rightInfo.GenericTypeArguments[0];

        Assert.AreEqual(typeof(Foo<int, string?>.Bar<object>), rightInnerInfo.Type);
        Assert.AreEqual(NullabilityState.Nullable, rightInnerInfo.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, rightInnerInfo.WriteState);
        Assert.AreEqual(3, rightInnerInfo.GenericTypeArguments.Length);

        NullabilityInfo rightInfo0 = rightInnerInfo.GenericTypeArguments[0];

        Assert.AreEqual(typeof(int), rightInfo0.Type);
        Assert.AreEqual(NullabilityState.NotNull, rightInfo0.ReadState);
        Assert.AreEqual(NullabilityState.NotNull, rightInfo0.WriteState);

        NullabilityInfo rightInfo1 = rightInnerInfo.GenericTypeArguments[1];

        Assert.AreEqual(typeof(string), rightInfo1.Type);
        Assert.AreEqual(NullabilityState.Nullable, rightInfo1.ReadState);
        Assert.AreEqual(NullabilityState.Nullable, rightInfo1.WriteState);

        NullabilityInfo rightInfo2 = rightInnerInfo.GenericTypeArguments[2];

        Assert.AreEqual(typeof(object), rightInfo2.Type);
        //Assert.AreEqual(NullabilityState.NotNull, rightInfo2.ReadState);
        //Assert.AreEqual(NullabilityState.NotNull, rightInfo2.WriteState);

        // The commented out lines are to work around a bug in the NullabilityInfo API in .NET 6.
        // This has been fixed for .NET 7: https://github.com/dotnet/runtime/pull/63556. The test
        // cases above can be uncommented when the .NET 7 target (or a more recent version) is added.
    }
#endif

    // See https://github.com/CommunityToolkit/dotnet/issues/201
    [TestMethod]
    public void Test_ObservableProperty_InheritedMembersAsAttributeTargets()
    {
        ConcreteViewModel model = new();

        List<string?> propertyNames = new();
        List<object?> canExecuteChangedArgs = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
        model.DoSomethingCommand.CanExecuteChanged += (s, _) => canExecuteChangedArgs.Add(s);
        model.ManualCommand.CanExecuteChanged += (s, _) => canExecuteChangedArgs.Add(s);

        model.A = nameof(model.A);
        model.B = nameof(model.B);
        model.C = nameof(model.C);
        model.D = nameof(model.D);

        CollectionAssert.AreEqual(new[]
        {
            nameof(model.A),
            nameof(model.Content),
            nameof(model.B),
            nameof(model.SomeGeneratedProperty),
            nameof(model.C),
            nameof(model.D)
        }, propertyNames);

        CollectionAssert.AreEqual(new[] { model.DoSomethingCommand, model.ManualCommand }, canExecuteChangedArgs);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/224
    [TestMethod]
    public void Test_ObservableProperty_WithinGenericTypeWithMultipleTypeParameters()
    {
        ModelWithMultipleGenericParameters<int, string> model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        model.Value = true;
        model.TValue = 42;
        model.UValue = "Hello";
        model.List = new List<int>() { 420 };

        Assert.AreEqual(model.Value, true);
        Assert.AreEqual(model.TValue, 42);
        Assert.AreEqual(model.UValue, "Hello");
        CollectionAssert.AreEqual(new[] { 420 }, model.List);

        CollectionAssert.AreEqual(new[] { nameof(model.Value), nameof(model.TValue), nameof(model.UValue), nameof(model.List) }, propertyNames);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/222
    [TestMethod]
    public void Test_ObservableProperty_WithBaseViewModelWithObservableObjectAttributeInAnotherAssembly()
    {
        ModelWithObservablePropertyAndBaseClassInAnotherAssembly model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        Assert.AreEqual(model.OtherProperty, "Ok");

        model.MyProperty = "A";
        model.OtherProperty = "B";

        Assert.AreEqual(model.MyProperty, "A");
        Assert.AreEqual(model.OtherProperty, "B");

        CollectionAssert.AreEqual(new[] { nameof(model.MyProperty), nameof(model.OtherProperty) }, propertyNames);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/230
    [TestMethod]
    public void Test_ObservableProperty_ModelWithCultureAwarePropertyName()
    {
        ModelWithCultureAwarePropertyName model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        model.InputFolder = 42;

        Assert.AreEqual(model.InputFolder, 42);

        CollectionAssert.AreEqual(new[] { nameof(model.InputFolder) }, propertyNames);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/242
    [TestMethod]
    public void Test_ObservableProperty_ModelWithNotifyPropertyChangedRecipientsAndDisplayAttributeLast()
    {
        IMessenger messenger = new StrongReferenceMessenger();
        ModelWithNotifyPropertyChangedRecipientsAndDisplayAttributeLast model = new(messenger);

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        object newValue = new();
        bool isMessageReceived = false;

        messenger.Register<Test_ObservablePropertyAttribute, PropertyChangedMessage<object>>(this, (r, m) =>
        {
            if (m.Sender != model)
            {
                Assert.Fail();
            }

            if (m.NewValue != newValue)
            {
                Assert.Fail();
            }

            isMessageReceived = true;
        });

        model.SomeProperty = newValue;

        Assert.AreEqual(model.SomeProperty, newValue);
        Assert.IsTrue(isMessageReceived);

        CollectionAssert.AreEqual(new[] { nameof(model.SomeProperty) }, propertyNames);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/257
    [TestMethod]
    public void Test_ObservableProperty_InheritedModelWithCommandUsingInheritedObservablePropertyForCanExecute()
    {
        InheritedModelWithCommandUsingInheritedObservablePropertyForCanExecute model = new();

        Assert.IsFalse(model.SaveCommand.CanExecute(null));

        model.CanSave = true;

        Assert.IsTrue(model.SaveCommand.CanExecute(null));
    }

    [TestMethod]
    public void Test_ObservableProperty_ForwardsSpecialCasesDataAnnotationAttributes()
    {
        PropertyInfo propertyInfo = typeof(ModelWithAdditionalDataAnnotationAttributes).GetProperty(nameof(ModelWithAdditionalDataAnnotationAttributes.Name))!;

        DisplayAttribute? displayAttribute = (DisplayAttribute?)propertyInfo.GetCustomAttribute(typeof(DisplayAttribute));

        Assert.IsNotNull(displayAttribute);
        Assert.AreEqual(displayAttribute!.Name, "MyProperty");
        Assert.AreEqual(displayAttribute.ResourceType, typeof(List<double>));
        Assert.AreEqual(displayAttribute.Prompt, "Foo bar baz");

        KeyAttribute? keyAttribute = (KeyAttribute?)propertyInfo.GetCustomAttribute(typeof(KeyAttribute));

        Assert.IsNotNull(keyAttribute);

        EditableAttribute? editableAttribute = (EditableAttribute?)propertyInfo.GetCustomAttribute(typeof(EditableAttribute));

        Assert.IsNotNull(keyAttribute);
        Assert.IsTrue(editableAttribute!.AllowEdit);

        UIHintAttribute? uiHintAttribute = (UIHintAttribute?)propertyInfo.GetCustomAttribute(typeof(UIHintAttribute));

        Assert.IsNotNull(uiHintAttribute);
        Assert.AreEqual(uiHintAttribute!.UIHint, "MyControl");
        Assert.AreEqual(uiHintAttribute.PresentationLayer, "WPF");
        Assert.AreEqual(uiHintAttribute.ControlParameters.Count, 3);
        Assert.IsTrue(uiHintAttribute.ControlParameters.ContainsKey("Foo"));
        Assert.IsTrue(uiHintAttribute.ControlParameters.ContainsKey("Bar"));
        Assert.IsTrue(uiHintAttribute.ControlParameters.ContainsKey("Baz"));
        Assert.AreEqual(uiHintAttribute.ControlParameters["Foo"], 42);
        Assert.AreEqual(uiHintAttribute.ControlParameters["Bar"], 3.14);
        Assert.AreEqual(uiHintAttribute.ControlParameters["Baz"], "Hello");

        ScaffoldColumnAttribute? scaffoldColumnAttribute = (ScaffoldColumnAttribute?)propertyInfo.GetCustomAttribute(typeof(ScaffoldColumnAttribute));

        Assert.IsNotNull(scaffoldColumnAttribute);
        Assert.IsTrue(scaffoldColumnAttribute!.Scaffold);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/271
    [TestMethod]
    public void Test_ObservableProperty_ModelWithObservablePropertyInRootNamespace()
    {
        ModelWithObservablePropertyInRootNamespace model = new();

        List<string?> propertyNames = new();

        model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

        model.Number = 3.14f;

        // We mostly just need to verify this class compiles fine with the right generated code
        CollectionAssert.AreEqual(propertyNames, new[] { nameof(model.Number) });
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/272
    [TestMethod]
    public void Test_ObservableProperty_WithCommandReferencingGeneratedPropertyFromOtherAssembly()
    {
        ModelWithOverriddenCommandMethodFromExternalBaseModel model = new();

        Assert.IsFalse(model.HasSaved);
        Assert.IsFalse(model.SaveCommand.CanExecute(null));

        model.CanSave = true;

        Assert.IsTrue(model.SaveCommand.CanExecute(null));

        model.SaveCommand.Execute(null);

        Assert.IsTrue(model.HasSaved);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/413
    [TestMethod]
    public void Test_ObservableProperty_WithExplicitAttributeForProperty()
    {
        PropertyInfo nameProperty = typeof(MyViewModelWithExplicitPropertyAttributes).GetProperty(nameof(MyViewModelWithExplicitPropertyAttributes.Name))!;

        Assert.IsNotNull(nameProperty.GetCustomAttribute<RequiredAttribute>());
        Assert.IsNotNull(nameProperty.GetCustomAttribute<MinLengthAttribute>());
        Assert.AreEqual(nameProperty.GetCustomAttribute<MinLengthAttribute>()!.Length, 1);
        Assert.IsNotNull(nameProperty.GetCustomAttribute<MaxLengthAttribute>());
        Assert.AreEqual(nameProperty.GetCustomAttribute<MaxLengthAttribute>()!.Length, 100);

        PropertyInfo lastNameProperty = typeof(MyViewModelWithExplicitPropertyAttributes).GetProperty(nameof(MyViewModelWithExplicitPropertyAttributes.LastName))!;

        Assert.IsNotNull(lastNameProperty.GetCustomAttribute<JsonPropertyNameAttribute>());
        Assert.AreEqual(lastNameProperty.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name, "lastName");
        Assert.IsNotNull(lastNameProperty.GetCustomAttribute<XmlIgnoreAttribute>());

        PropertyInfo justOneSimpleAttributeProperty = typeof(MyViewModelWithExplicitPropertyAttributes).GetProperty(nameof(MyViewModelWithExplicitPropertyAttributes.JustOneSimpleAttribute))!;

        Assert.IsNotNull(justOneSimpleAttributeProperty.GetCustomAttribute<TestAttribute>());

        PropertyInfo someComplexValidationAttributeProperty = typeof(MyViewModelWithExplicitPropertyAttributes).GetProperty(nameof(MyViewModelWithExplicitPropertyAttributes.SomeComplexValidationAttribute))!;

        TestValidationAttribute testAttribute = someComplexValidationAttributeProperty.GetCustomAttribute<TestValidationAttribute>()!;

        Assert.IsNotNull(testAttribute);
        Assert.IsNull(testAttribute.O);
        Assert.AreEqual(testAttribute.T, typeof(MyViewModelWithExplicitPropertyAttributes));
        Assert.AreEqual(testAttribute.Flag, true);
        Assert.AreEqual(testAttribute.D, 6.28);
        CollectionAssert.AreEqual(testAttribute.Names, new[] { "Bob", "Ross" });

        object[]? nestedArray = (object[]?)testAttribute.NestedArray;

        Assert.IsNotNull(nestedArray);
        Assert.AreEqual(nestedArray!.Length, 3);
        Assert.AreEqual(nestedArray[0], 1);
        Assert.AreEqual(nestedArray[1], "Hello");
        Assert.IsTrue(nestedArray[2] is int[]);
        CollectionAssert.AreEqual((int[])nestedArray[2], new[] { 2, 3, 4 });

        Assert.AreEqual(testAttribute.Animal, Animal.Llama);

        PropertyInfo someComplexRandomAttribute = typeof(MyViewModelWithExplicitPropertyAttributes).GetProperty(nameof(MyViewModelWithExplicitPropertyAttributes.SomeComplexRandomAttribute))!;

        Assert.IsNotNull(someComplexRandomAttribute.GetCustomAttribute<TestAttribute>());

        PropertyInfoAttribute testAttribute2 = someComplexRandomAttribute.GetCustomAttribute<PropertyInfoAttribute>()!;

        Assert.IsNotNull(testAttribute2);
        Assert.IsNull(testAttribute2.O);
        Assert.AreEqual(testAttribute2.T, typeof(MyViewModelWithExplicitPropertyAttributes));
        Assert.AreEqual(testAttribute2.Flag, true);
        Assert.AreEqual(testAttribute2.D, 6.28);
        Assert.IsNotNull(testAttribute2.Objects);
        Assert.IsTrue(testAttribute2.Objects is object[]);
        Assert.AreEqual(((object[])testAttribute2.Objects).Length, 1);
        Assert.AreEqual(((object[])testAttribute2.Objects)[0], "Test");
        CollectionAssert.AreEqual(testAttribute2.Names, new[] { "Bob", "Ross" });

        object[]? nestedArray2 = (object[]?)testAttribute2.NestedArray;

        Assert.IsNotNull(nestedArray2);
        Assert.AreEqual(nestedArray2!.Length, 4);
        Assert.AreEqual(nestedArray2[0], 1);
        Assert.AreEqual(nestedArray2[1], "Hello");
        Assert.AreEqual(nestedArray2[2], 42);
        Assert.IsNull(nestedArray2[3]);

        Assert.AreEqual(testAttribute2.Animal, (Animal)67);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/446
    [TestMethod]
    public void Test_ObservableProperty_CommandNamesThatCantBeLowered()
    {
        ModelWithCommandNamesThatCantBeLowered model = new();

        // Just ensures this builds
        _ = model.中文Command;
        _ = model.c中文Command;

        FieldInfo? fieldInfo = typeof(ModelWithCommandNamesThatCantBeLowered).GetField($"_{nameof(ModelWithCommandNamesThatCantBeLowered.中文)}Command", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.AreSame(model.中文Command, fieldInfo?.GetValue(model));

        fieldInfo = typeof(ModelWithCommandNamesThatCantBeLowered).GetField($"_{nameof(ModelWithCommandNamesThatCantBeLowered.c中文)}Command", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.AreSame(model.c中文Command, fieldInfo?.GetValue(model));
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/375
    [TestMethod]
    public void Test_ObservableProperty_ModelWithObservablePropertyWithUnderscoreAndUppercase()
    {
        ModelWithObservablePropertyWithUnderscoreAndUppercase model = new();

        Assert.IsFalse(model.IsReadOnly);

        // Just ensures this builds and the property is generated with the expected name
        model.IsReadOnly = true;

        Assert.IsTrue(model.IsReadOnly);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/711
    [TestMethod]
    public void Test_ObservableProperty_ModelWithDependentPropertyAndPropertyChanging()
    {
        ModelWithDependentPropertyAndPropertyChanging model = new();

        List<string?> changingArgs = new();
        List<string?> changedArgs = new();

        model.PropertyChanging += (s, e) => changingArgs.Add(e.PropertyName);
        model.PropertyChanged += (s, e) => changedArgs.Add(e.PropertyName);

        model.Name = "Bob";

        CollectionAssert.AreEqual(new[] { nameof(ModelWithDependentPropertyAndPropertyChanging.Name), nameof(ModelWithDependentPropertyAndPropertyChanging.FullName) }, changingArgs);
        CollectionAssert.AreEqual(new[] { nameof(ModelWithDependentPropertyAndPropertyChanging.Name), nameof(ModelWithDependentPropertyAndPropertyChanging.FullName) }, changedArgs);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/711
    [TestMethod]
    public void Test_ObservableProperty_ModelWithDependentPropertyAndNoPropertyChanging()
    {
        ModelWithDependentPropertyAndNoPropertyChanging model = new();

        List<string?> changedArgs = new();

        model.PropertyChanged += (s, e) => changedArgs.Add(e.PropertyName);

        model.Name = "Alice";

        CollectionAssert.AreEqual(new[] { nameof(ModelWithDependentPropertyAndNoPropertyChanging.Name), nameof(ModelWithDependentPropertyAndNoPropertyChanging.FullName) }, changedArgs);
    }

#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_ObservableProperty_MemberNotNullAttributeIsPresent()
    {
        MemberNotNullAttribute? attribute = typeof(ModelWithNonNullableObservableProperty).GetProperty(nameof(ModelWithNonNullableObservableProperty.Name))!.SetMethod!.GetCustomAttribute<MemberNotNullAttribute>();

        Assert.IsNotNull(attribute);
        CollectionAssert.AreEqual(new[] { nameof(ModelWithNonNullableObservableProperty.name) }, attribute.Members);
    }
#endif

    // See https://github.com/CommunityToolkit/dotnet/issues/731
    [TestMethod]
    public void Test_ObservableProperty_ForwardedAttributesWithNegativeValues()
    {
        Assert.AreEqual(PositiveEnum.Something,
            typeof(ModelWithForwardedAttributesWithNegativeValues)
            .GetProperty(nameof(ModelWithForwardedAttributesWithNegativeValues.Test2))!
            .GetCustomAttribute<DefaultValueAttribute>()!
            .Value);

        Assert.AreEqual(NegativeEnum.Problem,
            typeof(ModelWithForwardedAttributesWithNegativeValues)
            .GetProperty(nameof(ModelWithForwardedAttributesWithNegativeValues.Test3))!
            .GetCustomAttribute<DefaultValueAttribute>()!
            .Value);
    }

    public abstract partial class BaseViewModel : ObservableObject
    {
        public string? Content { get; set; }

        [ObservableProperty]
        private string? someGeneratedProperty;

        [RelayCommand]
        private void DoSomething()
        {
        }

        public IRelayCommand ManualCommand { get; } = new RelayCommand(() => { });
    }

    public partial class ConcreteViewModel : BaseViewModel
    {
        // Inherited property
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Content))]
        private string? _a;

        // Inherited generated property
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SomeGeneratedProperty))]
        private string? _b;

        // Inherited generated command
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DoSomethingCommand))]
        private string? _c;

        // Inherited manual command
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ManualCommand))]
        private string? _d;
    }

    public partial class SampleModel : ObservableObject
    {
        /// <summary>
        /// This is a sample data field within <see cref="SampleModel"/> of type <see cref="int"/>.
        /// </summary>
        [ObservableProperty]
        private int data;

        #region More properties

        [ObservableProperty]
        private int counter;

        #endregion

        [ObservableProperty]
        private string? name;
    }

    [INotifyPropertyChanged]
    public sealed partial class DependentPropertyModel
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        [NotifyPropertyChangedFor(nameof(Alias))]
        private string? name;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName), nameof(Alias))]
        [NotifyCanExecuteChangedFor(nameof(MyCommand))]
        private string? surname;

        public string FullName => $"{Name} {Surname}";

        public string Alias => $"{Name?.ToLower()}{Surname?.ToLower()}";

        public RelayCommand MyCommand { get; } = new(() => { });
    }

    [INotifyPropertyChanged]
    public sealed partial class DependentPropertyModel2
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName), nameof(Alias))]
        [NotifyCanExecuteChangedFor(nameof(TestFromMethodCommand))]
        private string? text;

        public string FullName => "";

        public string Alias => "";

        public RelayCommand MyCommand { get; } = new(() => { });

        [RelayCommand]
        private void TestFromMethod()
        {
        }
    }

    [INotifyPropertyChanged]
    public sealed partial class DependentPropertyModel3
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName), nameof(Alias))]
        [NotifyCanExecuteChangedFor(nameof(MyCommand))]
        private string? text;

        public string FullName => "";

        public string Alias => "";

        public IRelayCommand MyCommand { get; } = new RelayCommand(() => { });
    }

    [INotifyPropertyChanged]
    public sealed partial class DependentPropertyModel4
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName), nameof(Alias))]
        [NotifyCanExecuteChangedFor(nameof(MyCommand))]
        private string? text;

        public string FullName => "";

        public string Alias => "";

        public IAsyncRelayCommand<string> MyCommand { get; } = new AsyncRelayCommand<string>(_ => Task.CompletedTask);
    }

    public partial class MyFormViewModel : ObservableValidator
    {
        [ObservableProperty]
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        private string? name;

        [ObservableProperty]
        [Range(0, 120)]
        private int age;

        [ObservableProperty]
        [EmailAddress]
        private string? email;

        [ObservableProperty]
        [TestValidation(null, typeof(SampleModel), true, 6.28, new[] { "Bob", "Ross" }, NestedArray = new object[] { 1, "Hello", new int[] { 2, 3, 4 } }, Animal = Animal.Llama)]
        private int ifThisWorksThenThatsGreat;
    }

    private sealed class TestValidationAttribute : ValidationAttribute
    {
        public TestValidationAttribute(object? o, Type t, bool flag, double d, string[] names)
        {
            O = o;
            T = t;
            Flag = flag;
            D = d;
            Names = names;
        }

        public object? O { get; }

        public Type T { get; }

        public bool Flag { get; }

        public double D { get; }

        public string[] Names { get; }

        public object? NestedArray { get; set; }

        public Animal Animal { get; set; }
    }

    public enum Animal
    {
        Cat,
        Dog,
        Llama
    }

    public partial class ModelWithValueProperty : ObservableObject
    {
        [ObservableProperty]
        private string? value;
    }

    public partial class ModelWithValuePropertyWithValidation : ObservableValidator
    {
        [ObservableProperty]
        [Required]
        [MinLength(5)]
        private string? value;
    }

    public partial class ModelWithValuePropertyWithAutomaticValidation : ObservableValidator
    {
        [ObservableProperty]
        [Required]
        [MinLength(5)]
        [NotifyDataErrorInfo]
        private string? value;
    }

    [NotifyDataErrorInfo]
    public partial class ModelWithValuePropertyWithAutomaticValidationWithClassLevelAttribute : ObservableValidator
    {
        [ObservableProperty]
        [Required]
        [MinLength(5)]
        private string? value;
    }

    public partial class ModelWithValuePropertyWithAutomaticValidationInheritingClassLevelAttribute : ModelWithValuePropertyWithAutomaticValidationWithClassLevelAttribute
    {
        [ObservableProperty]
        [Required]
        [MinLength(5)]
        private string? value2;
    }

    public partial class ViewModelWithValidatableGeneratedProperties : ObservableValidator
    {
        [Required]
        [MinLength(2)]
        [MaxLength(60)]
        [Display(Name = "FirstName")]
        [ObservableProperty]
        public string first = "Bob";

        [Display(Name = "LastName")]
        [Required]
        [MinLength(2)]
        [MaxLength(60)]
        [ObservableProperty]
        public string last = "Jones";

        public void RunValidation() => ValidateAllProperties();
    }

    public partial class ViewModelWithImplementedUpdateMethods : ObservableObject
    {
        [ObservableProperty]
        public string? name = "Bob";

        [ObservableProperty]
        public int number = 42;

        public string? NameChangingValue { get; private set; }

        public string? NameChangedValue { get; private set; }

        public int NumberChangedValue { get; private set; }

        partial void OnNameChanging(string? value)
        {
            NameChangingValue = value;
        }

        partial void OnNameChanged(string? value)
        {
            NameChangedValue = value;
        }

        partial void OnNumberChanged(int value)
        {
            NumberChangedValue = value;
        }
    }

    public partial class ViewModelWithImplementedUpdateMethods2 : ObservableObject
    {
        [ObservableProperty]
        public string? name;

        [ObservableProperty]
        public int number;

        public List<(string? Old, string? New)> OnNameChangingValues { get; } = new();

        public List<(string? Old, string? New)> OnNameChangedValues { get; } = new();

        public List<(int Old, int New)> OnNumberChangingValues { get; } = new();

        public List<(int Old, int New)> OnNumberChangedValues { get; } = new();

        partial void OnNameChanging(string? oldValue, string? newValue)
        {
            OnNameChangingValues.Add((oldValue, newValue));
        }

        partial void OnNameChanged(string? oldValue, string? newValue)
        {
            OnNameChangedValues.Add((oldValue, newValue));
        }

        partial void OnNumberChanging(int oldValue, int newValue)
        {
            OnNumberChangingValues.Add((oldValue, newValue));
        }

        partial void OnNumberChanged(int oldValue, int newValue)
        {
            OnNumberChangedValues.Add((oldValue, newValue));
        }
    }

    public partial class ViewModelWithImplementedUpdateMethodAndAdditionalValidation : ObservableObject
    {
        private int step;

        [ObservableProperty]
        public string? name = "A";

        partial void OnNameChanging(string? value)
        {
            Assert.AreEqual(0, this.step);

            this.step = 1;

            Assert.AreEqual("A", this.name);
            Assert.AreEqual("B", value);
        }

        partial void OnNameChanged(string? value)
        {
            Assert.AreEqual(2, this.step);

            this.step = 3;

            Assert.AreEqual("B", this.name);
            Assert.AreEqual("B", value);
        }

        protected override void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            base.OnPropertyChanging(e);

            Assert.AreEqual(1, this.step);

            this.step = 2;

            Assert.AreEqual("A", this.name);
            Assert.AreEqual(nameof(Name), e.PropertyName);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            Assert.AreEqual(3, this.step);

            Assert.AreEqual("B", this.name);
            Assert.AreEqual(nameof(Name), e.PropertyName);
        }
    }

    partial class BroadcastingViewModel : ObservableRecipient
    {
        public BroadcastingViewModel(IMessenger messenger)
            : base(messenger)
        {
        }

        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private string? name;
    }

    partial class RecipientWithNonBroadcastingProperty : ObservableRecipient
    {
        public RecipientWithNonBroadcastingProperty(IMessenger messenger)
            : base(messenger)
        {
        }

        [ObservableProperty]
        private string? name;
    }

    [ObservableRecipient]
    partial class BroadcastingViewModelWithAttribute : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private string? name;
    }

    partial class BroadcastingViewModelWithInheritedAttribute : BroadcastingViewModelWithAttribute
    {
        public BroadcastingViewModelWithInheritedAttribute(IMessenger messenger)
            : base(messenger)
        {
        }

        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private string? name2;
    }

    [NotifyPropertyChangedRecipients]
    partial class BroadcastingViewModelWithClassLevelAttribute : ObservableRecipient
    {
        public BroadcastingViewModelWithClassLevelAttribute(IMessenger messenger)
            : base(messenger)
        {
        }

        [ObservableProperty]
        private string? name;
    }

    partial class BroadcastingViewModelWithInheritedClassLevelAttribute : BroadcastingViewModelWithClassLevelAttribute
    {
        public BroadcastingViewModelWithInheritedClassLevelAttribute(IMessenger messenger)
            : base(messenger)
        {
        }

        [ObservableProperty]
        private string? name2;
    }

    [ObservableRecipient]
    [NotifyPropertyChangedRecipients]
    partial class BroadcastingViewModelWithAttributeAndClassLevelAttribute : ObservableObject
    {
        [ObservableProperty]
        private string? name;
    }

    partial class BroadcastingViewModelWithInheritedAttributeAndClassLevelAttribute : BroadcastingViewModelWithAttributeAndClassLevelAttribute
    {
        public BroadcastingViewModelWithInheritedAttributeAndClassLevelAttribute(IMessenger messenger)
            : base(messenger)
        {
        }

        [ObservableProperty]
        private string? name2;
    }

#if NET6_0_OR_GREATER
    private partial class NullableRepro : ObservableObject
    {
        [ObservableProperty]
        private List<string?>? nullableList;

        [ObservableProperty]
        private Foo<Foo<string?, int>.Bar<object?>?, StrongBox<Foo<int, string?>.Bar<object>?>?>? nullableMess;
    }

    private class Foo<T1, T2>
    {
        public class Bar<T>
        {
        }
    }
#endif

    partial class ModelWithObservablePropertyAndBaseClassInAnotherAssembly : ModelWithObservableObjectAttribute
    {
        [ObservableProperty]
        private string? _otherProperty;

        public ModelWithObservablePropertyAndBaseClassInAnotherAssembly()
        {
            OtherProperty = "Ok";
        }
    }

    interface IValueHolder
    {
        public bool Value { get; }
    }

    partial class ModelWithMultipleGenericParameters<T, U> : ObservableObject, IValueHolder
    {
        [ObservableProperty]
        private bool value;

        [ObservableProperty]
        private T? tValue;

        [ObservableProperty]
        private U? uValue;

        [ObservableProperty]
        private List<T>? list;
    }

    [ObservableObject]
    partial class ModelWithCultureAwarePropertyName
    {
        // This starts with "i" as it's one of the characters that can change when converted to uppercase.
        // For instance, when using the Turkish language pack, this would become "İnputFolder" if done wrong.
        [ObservableProperty]
        private int _inputFolder;
    }

    [ObservableRecipient]
    public sealed partial class ModelWithNotifyPropertyChangedRecipientsAndDisplayAttributeLast : ObservableValidator
    {
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        [Display(Name = "Foo bar baz")]
        private object? _someProperty;
    }

    public abstract partial class BaseModelWithObservablePropertyAttribute : ObservableObject
    {
        [ObservableProperty]
        private bool canSave;

        public abstract void Save();
    }

    public partial class InheritedModelWithCommandUsingInheritedObservablePropertyForCanExecute : BaseModelWithObservablePropertyAttribute
    {
        [RelayCommand(CanExecute = nameof(CanSave))]
        public override void Save()
        {
        }
    }

    public partial class ModelWithAdditionalDataAnnotationAttributes : ObservableValidator
    {
        [ObservableProperty]
        [Display(Name = "MyProperty", ResourceType = typeof(List<double>), Prompt = "Foo bar baz")]
        [Key]
        [Editable(true)]
        [UIHint("MyControl", "WPF", new object[] { "Foo", 42, "Bar", 3.14, "Baz", "Hello" })]
        [ScaffoldColumn(true)]
        private string? name;
    }

    public partial class ModelWithOverriddenCommandMethodFromExternalBaseModel : ModelWithObservablePropertyAndMethod
    {
        public bool HasSaved { get; private set; }

        [RelayCommand(CanExecute = nameof(CanSave))]
        public override void Save()
        {
            HasSaved = true;
        }
    }

    public partial class MyViewModelWithExplicitPropertyAttributes : ObservableValidator
    {
        [ObservableProperty]
        [property: Required]
        [property: MinLength(1)]
        [property: MaxLength(100)]
        private string? name;

        [ObservableProperty]
        [property: JsonPropertyName("lastName")]
        [property: XmlIgnore]
        private string? lastName;

        [ObservableProperty]
        [property: Test]
        private string? justOneSimpleAttribute;

        [ObservableProperty]
        [property: TestValidation(null, typeof(MyViewModelWithExplicitPropertyAttributes), true, 6.28, new[] { "Bob", "Ross" }, NestedArray = new object[] { 1, "Hello", new int[] { 2, 3, 4 } }, Animal = Animal.Llama)]
        private int someComplexValidationAttribute;

        [ObservableProperty]
        [property: Test]
        [property: PropertyInfo(null, typeof(MyViewModelWithExplicitPropertyAttributes), true, 6.28, new[] { "Bob", "Ross" }, new object[] { "Test" }, NestedArray = new object[] { 1, "Hello", 42, null }, Animal = (Animal)67)]
        private int someComplexRandomAttribute;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class TestAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class PropertyInfoAttribute : Attribute
    {
        public PropertyInfoAttribute(object? o, Type t, bool flag, double d, string[] names, object[] objects)
        {
            O = o;
            T = t;
            Flag = flag;
            D = d;
            Names = names;
            Objects = objects;
        }

        public object? O { get; }

        public Type T { get; }

        public bool Flag { get; }

        public double D { get; }

        public string[] Names { get; }

        public object? Objects { get; set; }

        public object? NestedArray { get; set; }

        public Animal Animal { get; set; }
    }

    private sealed partial class ModelWithCommandNamesThatCantBeLowered
    {
        [RelayCommand]
        public void 中文()
        {
        }

        [RelayCommand]
        public void c中文()
        {
        }
    }

    private partial class ModelWithObservablePropertyWithUnderscoreAndUppercase : ObservableObject
    {
        [ObservableProperty]
        private bool _IsReadOnly;
    }

#if NET6_0_OR_GREATER
    // See https://github.com/CommunityToolkit/dotnet/issues/645
    // This viewmodel is here only to double check no warnings are emitted when the attribute is present
    public partial class ModelWithNonNullableObservableProperty : ObservableObject
    {
        public ModelWithNonNullableObservableProperty()
        {
            Name = "Bob";
        }

        [ObservableProperty]
        internal string name;
    }
#endif

    private partial class ModelWithForwardedAttributesWithNegativeValues : ObservableObject
    {
        [ObservableProperty]
        private bool _test1;

        [ObservableProperty]
        [property: DefaultValue(PositiveEnum.Something)]
        private PositiveEnum _test2;

        [ObservableProperty]
        [property: DefaultValue(NegativeEnum.Problem)]
        private NegativeEnum _test3;

        [ObservableProperty]
        private int _test4;

        public ModelWithForwardedAttributesWithNegativeValues()
        {
            Test1 = true;
            Test2 = PositiveEnum.Else;
        }
    }

    public enum PositiveEnum
    {
        Something = 0,
        Else = 1
    }

    public enum NegativeEnum
    {
        Problem = -1,
        OK = 0
    }
    
    private sealed partial class ModelWithDependentPropertyAndPropertyChanging : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        private string? name;

        public string? FullName => "";
    }

    [INotifyPropertyChanged(IncludeAdditionalHelperMethods = false)]
    private sealed partial class ModelWithDependentPropertyAndNoPropertyChanging
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        private string? name;

        public string? FullName => "";
    }

#if NET6_0_OR_GREATER
    // See https://github.com/CommunityToolkit/dotnet/issues/939
    public partial class ModelWithSecondaryPropertySetFromGeneratedSetter_DoesNotWarn : ObservableObject
    {
        [ObservableProperty]
        [set: MemberNotNull(nameof(B))]
        private string a;

        // This type validates forwarding attributes on generated accessors. In particular, there should
        // be no nullability warning on this constructor (CS8618), thanks to 'MemberNotNullAttribute("B")'
        // being forwarded to the generated setter in the generated property (see linked issue).
        public ModelWithSecondaryPropertySetFromGeneratedSetter_DoesNotWarn()
        {
            A = "";
        }

        public string B { get; private set; }

        [MemberNotNull(nameof(B))]
        partial void OnAChanged(string? oldValue, string newValue)
        {
            B = "";
        }
    }
#endif
}

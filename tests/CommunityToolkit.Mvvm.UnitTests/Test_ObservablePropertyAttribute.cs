// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    public void Test_AlsoNotifyChangeForAttribute_Events()
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

        model.Value = "Hello world";

        Assert.AreEqual(model.Value, "Hello world");

        CollectionAssert.AreEqual(new[] { nameof(model.Value) }, propertyNames);
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
    public void Test_AlsoNotifyChangeFor()
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
    public void Test_AlsoNotifyChangeFor_GeneratedCommand()
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
    public void Test_AlsoNotifyChangeFor_IRelayCommandProperty()
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
    public void Test_AlsoNotifyChangeFor_IAsyncRelayCommandOfTProperty()
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
    public void Test_OnPropertyChangingAndChangedPartialMethodWithAdditionalValidation()
    {
        ViewModelWithImplementedUpdateMethodAndAdditionalValidation model = new();

        // The actual validation is performed inside the model itself.
        // This test validates that the order with which methods/events are generated is:
        //   - On<PROPERTY_NAME>Changing(value);
        //   - OnProperyChanging();
        //   - field = value;
        //   - On<PROPERTY_NAME>Changed(value);
        //   - OnProperyChanged();
        model.Name = "B";

        Assert.AreEqual("B", model.Name);
    }

    [TestMethod]
    public void Test_AlsoBroadcastChange_WithObservableObject()
    {
        Test_AlsoBroadcastChange_Test(
           factory: static messenger => new BroadcastingViewModel(messenger),
           setter: static (model, value) => model.Name = value,
           propertyName: nameof(BroadcastingViewModel.Name));
    }

    [TestMethod]
    public void Test_AlsoBroadcastChange_WithObservableRecipientAttribute()
    {
        Test_AlsoBroadcastChange_Test(
            factory: static messenger => new BroadcastingViewModelWithAttribute(messenger),
            setter: static (model, value) => model.Name = value,
            propertyName: nameof(BroadcastingViewModelWithAttribute.Name));
    }

    [TestMethod]
    public void Test_AlsoBroadcastChange_WithInheritedObservableRecipientAttribute()
    {
        Test_AlsoBroadcastChange_Test(
            factory: static messenger => new BroadcastingViewModelWithInheritedAttribute(messenger),
            setter: static (model, value) => model.Name2 = value,
            propertyName: nameof(BroadcastingViewModelWithInheritedAttribute.Name2));
    }

    private void Test_AlsoBroadcastChange_Test<T>(Func<IMessenger, T> factory, Action<T, string?> setter, string propertyName)
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
        [AlsoNotifyChangeFor(nameof(FullName))]
        [AlsoNotifyChangeFor(nameof(Alias))]
        private string? name;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(FullName), nameof(Alias))]
        [AlsoNotifyCanExecuteFor(nameof(MyCommand))]
        private string? surname;

        public string FullName => $"{Name} {Surname}";

        public string Alias => $"{Name?.ToLower()}{Surname?.ToLower()}";

        public RelayCommand MyCommand { get; } = new(() => { });
    }

    [INotifyPropertyChanged]
    public sealed partial class DependentPropertyModel2
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(FullName), nameof(Alias))]
        [AlsoNotifyCanExecuteFor(nameof(TestFromMethodCommand))]
        private string? text;

        public string FullName => "";

        public string Alias => "";

        public RelayCommand MyCommand { get; } = new(() => { });

        [ICommand]
        private void TestFromMethod()
        {
        }
    }

    [INotifyPropertyChanged]
    public sealed partial class DependentPropertyModel3
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(FullName), nameof(Alias))]
        [AlsoNotifyCanExecuteFor(nameof(MyCommand))]
        private string? text;

        public string FullName => "";

        public string Alias => "";

        public IRelayCommand MyCommand { get; } = new RelayCommand(() => { });
    }

    [INotifyPropertyChanged]
    public sealed partial class DependentPropertyModel4
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(FullName), nameof(Alias))]
        [AlsoNotifyCanExecuteFor(nameof(MyCommand))]
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
        [AlsoBroadcastChange]
        private string? name;
    }

    [ObservableRecipient]
    partial class BroadcastingViewModelWithAttribute : ObservableObject
    {
        [ObservableProperty]
        [AlsoBroadcastChange]
        private string? name;
    }

    partial class BroadcastingViewModelWithInheritedAttribute : BroadcastingViewModelWithAttribute
    {
        public BroadcastingViewModelWithInheritedAttribute(IMessenger messenger)
            : base(messenger)
        {
        }

        [ObservableProperty]
        [AlsoBroadcastChange]
        private string? name2;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS0618

namespace CommunityToolkit.Mvvm.UnitTests;

[TestClass]
public class Test_ObservableValidator
{
    [TestMethod]
    public void Test_ObservableValidator_HasErrors()
    {
        Person model = new();
        List<PropertyChangedEventArgs> args = new();

        model.PropertyChanged += (s, e) => args.Add(e);

        Assert.IsFalse(model.HasErrors);

        model.Name = "No";

        // Verify that errors were correctly reported as changed, and that all the relevant
        // properties were broadcast as well (both the changed property and HasErrors). We need
        // this last one to raise notifications too so that users can bind to that in the UI.
        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(2, args);
        Assert.AreEqual(nameof(Person.Name), args[0].PropertyName);
        Assert.AreEqual(nameof(INotifyDataErrorInfo.HasErrors), args[1].PropertyName);

        model.Name = "Valid";

        Assert.IsFalse(model.HasErrors);
        Assert.HasCount(4, args);
        Assert.AreEqual(nameof(Person.Name), args[2].PropertyName);
        Assert.AreEqual(nameof(INotifyDataErrorInfo.HasErrors), args[3].PropertyName);
    }

    [TestMethod]
    public void Test_ObservableValidator_ErrorsChanged()
    {
        Person model = new();

        List<(object? Sender, DataErrorsChangedEventArgs Args)> errors = new();

        model.ErrorsChanged += (s, e) => errors.Add((s, e));

        model.Name = "Foo";

        Assert.HasCount(1, errors);
        Assert.AreSame(errors[0].Sender, model);
        Assert.AreEqual(nameof(Person.Name), errors[0].Args.PropertyName);

        errors.Clear();

        model.Name = "Bar";

        Assert.HasCount(1, errors);
        Assert.AreSame(errors[0].Sender, model);
        Assert.AreEqual(nameof(Person.Name), errors[0].Args.PropertyName);

        errors.Clear();

        model.Name = "Valid";

        Assert.HasCount(1, errors);
        Assert.AreSame(errors[0].Sender, model);
        Assert.AreEqual(nameof(Person.Name), errors[0].Args.PropertyName);

        errors.Clear();

        model.Name = "This is fine";

        Assert.IsEmpty(errors);
    }

    [TestMethod]
    public void Test_ObservableValidator_GetErrors()
    {
        Person? model = new();

        Assert.AreEqual(0, model.GetErrors(null).Count());
        Assert.AreEqual(0, model.GetErrors(string.Empty).Count());
        Assert.AreEqual(0, model.GetErrors("ThereIsntAPropertyWithThisName").Count());
        Assert.AreEqual(0, model.GetErrors(nameof(Person.Name)).Count());

        model.Name = "Foo";

        ValidationResult[]? errors = model.GetErrors(nameof(Person.Name)).ToArray();

        Assert.HasCount(1, errors);
        Assert.AreEqual(nameof(Person.Name), errors[0].MemberNames.First());

        Assert.AreEqual(0, model.GetErrors("ThereIsntAPropertyWithThisName").Count());

        errors = model.GetErrors(null).ToArray();

        Assert.HasCount(1, errors);
        Assert.AreEqual(nameof(Person.Name), errors[0].MemberNames.First());

        errors = model.GetErrors(string.Empty).ToArray();

        Assert.HasCount(1, errors);
        Assert.AreEqual(nameof(Person.Name), errors[0].MemberNames.First());

        model.Age = -1;

        errors = model.GetErrors(null).ToArray();

        Assert.HasCount(2, errors);
        Assert.IsTrue(errors.Any(e => e.MemberNames.First().Equals(nameof(Person.Name))));
        Assert.IsTrue(errors.Any(e => e.MemberNames.First().Equals(nameof(Person.Age))));

        model.Age = 26;

        errors = model.GetErrors(null).ToArray();

        Assert.HasCount(1, errors);
        Assert.IsTrue(errors.Any(e => e.MemberNames.First().Equals(nameof(Person.Name))));
        Assert.IsFalse(errors.Any(e => e.MemberNames.First().Equals(nameof(Person.Age))));
    }

    [TestMethod]
    [DataRow("", false)]
    [DataRow("No", false)]
    [DataRow("This text is really, really too long for the target property", false)]
    [DataRow("1234", true)]
    [DataRow("01234567890123456789", true)]
    [DataRow("Hello world", true)]
    public void Test_ObservableValidator_ValidateReturn(string value, bool isValid)
    {
        Person? model = new() { Name = value };

        Assert.AreEqual(model.HasErrors, !isValid);

        if (isValid)
        {
            Assert.IsFalse(model.GetErrors(nameof(Person.Name)).Any());
        }
        else
        {
            Assert.IsTrue(model.GetErrors(nameof(Person.Name)).Any());
        }
    }

    [TestMethod]
    public void Test_ObservableValidator_TrySetProperty()
    {
        Person? model = new();
        List<DataErrorsChangedEventArgs>? events = new();

        model.ErrorsChanged += (s, e) => events.Add(e);

        // Set a correct value, this should update the property
        Assert.IsTrue(model.TrySetName("Hello", out IReadOnlyCollection<ValidationResult>? errors));
        Assert.IsEmpty(errors);
        Assert.IsEmpty(events);
        Assert.AreEqual("Hello", model.Name);
        Assert.IsFalse(model.HasErrors);

        // Invalid value #1, this should be ignored
        Assert.IsFalse(model.TrySetName(null, out errors));
        Assert.IsNotEmpty(errors);
        Assert.IsEmpty(events);
        Assert.AreEqual("Hello", model.Name);
        Assert.IsFalse(model.HasErrors);

        // Invalid value #2, same as above
        Assert.IsFalse(model.TrySetName("This string is too long for the target property in this model and should fail", out errors));
        Assert.IsNotEmpty(errors);
        Assert.IsEmpty(events);
        Assert.AreEqual("Hello", model.Name);
        Assert.IsFalse(model.HasErrors);

        // Correct value, this should update the property
        Assert.IsTrue(model.TrySetName("Hello world", out errors));
        Assert.IsEmpty(errors);
        Assert.IsEmpty(events);
        Assert.AreEqual("Hello world", model.Name);
        Assert.IsFalse(model.HasErrors);

        // Actually set an invalid value to show some errors
        model.Name = "No";

        // Errors should now be present
        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(1, events);
        Assert.IsTrue(model.GetErrors(nameof(Person.Name)).Any());
        Assert.IsTrue(model.HasErrors);

        // Trying to set a correct property should clear the errors
        Assert.IsTrue(model.TrySetName("This is fine", out errors));
        Assert.IsEmpty(errors);
        Assert.HasCount(2, events);
        Assert.IsFalse(model.HasErrors);
        Assert.AreEqual("This is fine", model.Name);
    }

    [TestMethod]
    public void Test_ObservableValidator_ValidateProperty()
    {
        ComparableModel? model = new();
        List<DataErrorsChangedEventArgs>? events = new();

        model.ErrorsChanged += (s, e) => events.Add(e);

        // Set a correct value for both properties, first A then B
        model.A = 42;
        model.B = 30;

        Assert.IsEmpty(events);
        Assert.IsFalse(model.HasErrors);

        // Make B greater than A, hence invalidating A
        model.B = 50;

        Assert.HasCount(1, events);
        Assert.AreEqual(nameof(ComparableModel.A), events.Last().PropertyName);
        Assert.IsTrue(model.HasErrors);

        events.Clear();

        // Make A greater than B, hence making it valid again
        model.A = 51;

        Assert.HasCount(1, events);
        Assert.AreEqual(nameof(ComparableModel.A), events.Last().PropertyName);
        Assert.AreEqual(0, model.GetErrors(nameof(ComparableModel.A)).Count());
        Assert.IsFalse(model.HasErrors);

        events.Clear();

        // Make A smaller than B, hence invalidating it
        model.A = 49;

        Assert.HasCount(1, events);
        Assert.AreEqual(nameof(ComparableModel.A), events.Last().PropertyName);
        Assert.AreEqual(1, model.GetErrors(nameof(ComparableModel.A)).Count());
        Assert.IsTrue(model.HasErrors);

        events.Clear();

        // Lower B, hence making A valid again
        model.B = 20;

        Assert.HasCount(1, events);
        Assert.AreEqual(nameof(ComparableModel.A), events.Last().PropertyName);
        Assert.AreEqual(0, model.GetErrors(nameof(ComparableModel.A)).Count());
        Assert.IsFalse(model.HasErrors);
    }

    [TestMethod]
    public void Test_ObservableValidator_ClearErrors()
    {
        Person? model = new();
        List<DataErrorsChangedEventArgs>? events = new();

        model.ErrorsChanged += (s, e) => events.Add(e);

        model.Age = -2;

        Assert.IsTrue(model.HasErrors);
        Assert.IsTrue(model.GetErrors(nameof(Person.Age)).Any());

        model.ClearErrors(nameof(Person.Age));

        Assert.IsFalse(model.HasErrors);
        Assert.HasCount(2, events);

        model.Age = 200;
        model.Name = "Bo";
        events.Clear();

        Assert.IsTrue(model.HasErrors);
        Assert.IsTrue(model.GetErrors(nameof(Person.Age)).Any());
        Assert.IsTrue(model.GetErrors(nameof(Person.Name)).Any());

        model.ClearErrors(null);
        Assert.IsFalse(model.HasErrors);
        Assert.IsFalse(model.GetErrors(nameof(Person.Age)).Any());
        Assert.IsFalse(model.GetErrors(nameof(Person.Name)).Any());
        Assert.HasCount(2, events);
        Assert.AreEqual(nameof(Person.Age), events[0].PropertyName);
        Assert.AreEqual(nameof(Person.Name), events[1].PropertyName);
    }

    [TestMethod]
    public void Test_ObservableValidator_ValidateAllProperties()
    {
        PersonWithDeferredValidation? model = new();
        List<DataErrorsChangedEventArgs>? events = new();

        model.ErrorsChanged += (s, e) => events.Add(e);

        model.ValidateAllProperties();

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(2, events);

        // Note: we can't use an index here because the order used to return properties
        // from reflection APIs is an implementation detail and might change at any time.
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Name)));
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));

        events.Clear();

        model.Name = "James";
        model.Age = 42;

        model.ValidateAllProperties();

        Assert.IsFalse(model.HasErrors);
        Assert.HasCount(2, events);
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Name)));
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));

        events.Clear();

        model.Age = -10;

        model.ValidateAllProperties();

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(1, events);
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));
    }

    [TestMethod]
    public void Test_ObservableValidator_ValidateAllProperties_WithFallback()
    {
        PersonWithDeferredValidation? model = new();
        List<DataErrorsChangedEventArgs>? events = new();

        MethodInfo[] staticMethods = typeof(ObservableValidator).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo validationMethod = staticMethods.Single(static m => m.Name.Contains("GetValidationActionFallback"));
        Func<Type, Action<object>> validationFunc = (Func<Type, Action<object>>)validationMethod.CreateDelegate(typeof(Func<Type, Action<object>>));
        Action<object> validationAction = validationFunc(model.GetType());

        model.ErrorsChanged += (s, e) => events.Add(e);

        validationAction(model);

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(2, events);

        // Note: we can't use an index here because the order used to return properties
        // from reflection APIs is an implementation detail and might change at any time.
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Name)));
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));

        events.Clear();

        model.Name = "James";
        model.Age = 42;

        validationAction(model);

        Assert.IsFalse(model.HasErrors);
        Assert.HasCount(2, events);
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Name)));
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));

        events.Clear();

        model.Age = -10;

        validationAction(model);

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(1, events);
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/235
    [TestMethod]
    public void Test_ObservableValidator_ValidateAllProperties_WithinPartialClassDeclaration()
    {
        PersonWithPartialDeclaration model = new();
        List<DataErrorsChangedEventArgs> events = new();

        model.ErrorsChanged += (s, e) => events.Add(e);

        model.ValidateAllProperties();

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(2, events);

        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(PersonWithPartialDeclaration.Name)));
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(PersonWithPartialDeclaration.Number)));

        events.Clear();

        model.Name = "Bob";
        model.Number = 42;

        model.ValidateAllProperties();

        Assert.IsFalse(model.HasErrors);
        Assert.HasCount(2, events);
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(PersonWithPartialDeclaration.Name)));
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(PersonWithPartialDeclaration.Number)));
    }

    [TestMethod]
    public void Test_ObservableValidator_CustomValidation()
    {
        Dictionary<object, object?>? items = new() { [nameof(CustomValidationModel.A)] = 42 };
        CustomValidationModel? model = new(items);
        List<DataErrorsChangedEventArgs>? events = new();

        model.ErrorsChanged += (s, e) => events.Add(e);

        model.A = 10;

        Assert.IsFalse(model.HasErrors);
        Assert.IsEmpty(events);
    }

    [TestMethod]
    public void Test_ObservableValidator_CustomValidationWithInjectedService()
    {
        ValidationWithServiceModel? model = new(new FancyService());
        List<DataErrorsChangedEventArgs>? events = new();

        model.ErrorsChanged += (s, e) => events.Add(e);

        model.Name = "This is a valid name";

        Assert.IsFalse(model.HasErrors);
        Assert.IsEmpty(events);

        model.Name = "This is invalid238!!";

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(1, events);
        Assert.AreEqual(nameof(ValidationWithServiceModel.Name), events[0].PropertyName);
        Assert.HasCount(1, model.GetErrors(nameof(ValidationWithServiceModel.Name)).ToArray());

        model.Name = "This is valid but it is too long so the validation will fail anyway";

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(2, events);
        Assert.AreEqual(nameof(ValidationWithServiceModel.Name), events[1].PropertyName);
        Assert.HasCount(1, model.GetErrors(nameof(ValidationWithServiceModel.Name)).ToArray());

        model.Name = "This is both too long and it also contains invalid characters, a real disaster!";

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(3, events);
        Assert.AreEqual(nameof(ValidationWithServiceModel.Name), events[2].PropertyName);
        Assert.HasCount(2, model.GetErrors(nameof(ValidationWithServiceModel.Name)).ToArray());
    }

    [TestMethod]
    public void Test_ObservableValidator_ValidationWithFormattedDisplayName()
    {
        ValidationWithDisplayName? model = new();

        Assert.IsTrue(model.HasErrors);

        // We need to order because there is no guaranteed order on the members of a type
        ValidationResult[] allErrors = model.GetErrors().OrderBy(error => error.ErrorMessage).ToArray();

        Assert.HasCount(2, allErrors);

        Assert.AreEqual(1, allErrors[0].MemberNames.Count());
        Assert.AreEqual(nameof(ValidationWithDisplayName.StringMayNotBeEmpty), allErrors[0].MemberNames.Single());
        Assert.AreEqual($"FIRST: {nameof(ValidationWithDisplayName.StringMayNotBeEmpty)}.", allErrors[0].ErrorMessage);

        Assert.AreEqual(1, allErrors[1].MemberNames.Count());
        Assert.AreEqual(nameof(ValidationWithDisplayName.AnotherRequiredField), allErrors[1].MemberNames.Single());
        Assert.AreEqual($"SECOND: {nameof(ValidationWithDisplayName.AnotherRequiredField)}.", allErrors[1].ErrorMessage);
    }

    // See: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4272
    [TestMethod]
    [DataRow(typeof(ObservableValidatorBase))]
    [DataRow(typeof(ObservableValidatorDerived))]
    public void Test_ObservableRecipient_ValidationOnNonValidatableProperties(Type type)
    {
        ObservableValidatorBase viewmodel = (ObservableValidatorBase)Activator.CreateInstance(type)!;

        viewmodel.ValidateAllProperties();
    }

    // See: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4272
    [TestMethod]
    [DataRow(typeof(ObservableValidatorBase))]
    [DataRow(typeof(ObservableValidatorDerived))]
    public void Test_ObservableRecipient_ValidationOnNonValidatableProperties_WithFallback(Type type)
    {
        ObservableValidatorBase viewmodel = (ObservableValidatorBase)Activator.CreateInstance(type)!;

        MethodInfo[] staticMethods = typeof(ObservableValidator).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo validationMethod = staticMethods.Single(static m => m.Name.Contains("GetValidationActionFallback"));
        Func<Type, Action<object>> validationFunc = (Func<Type, Action<object>>)validationMethod.CreateDelegate(typeof(Func<Type, Action<object>>));

        validationFunc(viewmodel.GetType())(viewmodel);
    }

    [TestMethod]
    public void Test_ObservableValidator_VerifyTrimmingAnnotation()
    {
#if NET6_0_OR_GREATER
        System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute? attribute =
            typeof(ComponentModel.__Internals.__ObservableValidatorExtensions)
            .GetCustomAttribute<System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute>();

        Assert.IsNotNull(attribute);
        Assert.AreEqual(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, attribute.MemberTypes);
#else
        IEnumerable<Attribute> attributes = typeof(ComponentModel.__Internals.__ObservableValidatorExtensions).GetCustomAttributes();

        Assert.IsFalse(attributes.Any(static a => a.GetType().Name is "DynamicallyAccessedMembersAttribute"));
#endif
    }

    [TestMethod]
    public void Test_ObservableRecipient_AbstractTypesDoNotTriggerCodeGeneration()
    {
        MethodInfo? createAllPropertiesValidatorMethod = typeof(ComponentModel.__Internals.__ObservableValidatorExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(static m => m.Name == "CreateAllPropertiesValidator")
            .Where(static m => m.GetParameters() is { Length: 1 } parameters && parameters[0].ParameterType == typeof(AbstractModelWithValidatableProperty))
            .FirstOrDefault();

        // We need to validate that no methods are generated for abstract types, so we just check this method doesn't exist
        Assert.IsNull(createAllPropertiesValidatorMethod);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/246
    [TestMethod]
    public void Test_ObservableValidator_WithGenericTypeParameters()
    {
        GenericPerson<string> model = new();

        model.Name = "Bob";

        model.ValidateAllProperties();

        Assert.IsTrue(model.HasErrors);

        ValidationResult[] errors = model.GetErrors(nameof(model.Value)).ToArray();

        Assert.IsNotNull(errors);
        Assert.HasCount(1, errors);

        CollectionAssert.AreEqual(errors[0].MemberNames.ToArray(), new[] { nameof(model.Value) });

        model.Value = "Ross";

        model.ValidateAllProperties();

        Assert.IsFalse(model.HasErrors);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/691
    [TestMethod]
    public void Test_ObservableValidator_ValidateAllProperties_IncludeInheritedProperties()
    {
        DerivedModelWithValidatableProperties model = new();
        List<DataErrorsChangedEventArgs> events = new();

        model.ErrorsChanged += (s, e) => events.Add(e);

        model.ValidateAllProperties();

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(2, events);

        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(DerivedModelWithValidatableProperties.Name)));
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(DerivedModelWithValidatableProperties.Number)));

        events.Clear();

        model.Number = 42;

        model.ValidateAllProperties();

        Assert.IsTrue(model.HasErrors);
        Assert.HasCount(2, events);

        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(DerivedModelWithValidatableProperties.Name)));
        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(DerivedModelWithValidatableProperties.Number)));

        Assert.AreEqual(1, model.GetErrors(nameof(DerivedModelWithValidatableProperties.Name)).Count());
        Assert.AreEqual(0, model.GetErrors(nameof(DerivedModelWithValidatableProperties.Number)).Count());

        events.Clear();

        model.Name = "Bob";
        model.Number = 80;

        model.ValidateAllProperties();

        Assert.IsFalse(model.HasErrors);
        Assert.HasCount(1, events);

        Assert.IsTrue(events.Any(e => e.PropertyName == nameof(DerivedModelWithValidatableProperties.Name)));
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/881
    [TestMethod]
    public void Test_ObservableValidator_HasErrors_IncludeNonAutogenerateAttribute()
    {
        DerivedModelWithValidatableProperties model = new();

        DisplayAttribute? displayAttribute = model.GetType().GetProperty(nameof(ObservableValidator.HasErrors))?.GetCustomAttribute<DisplayAttribute>();

        Assert.IsNotNull(displayAttribute);
        Assert.IsFalse(displayAttribute.AutoGenerateField);
    }

    public class Person : ObservableValidator
    {
        private string? name;

        [MinLength(4)]
        [MaxLength(20)]
        [Required]
        public string? Name
        {
            get => this.name;
            set => SetProperty(ref this.name, value, true);
        }

        public bool TrySetName(string? value, out IReadOnlyCollection<ValidationResult> errors)
        {
            return TrySetProperty(ref this.name, value, out errors, nameof(Name));
        }

        private int age;

        [Range(0, 100)]
        public int Age
        {
            get => this.age;
            set => SetProperty(ref this.age, value, true);
        }

        public new void ClearErrors(string? propertyName)
        {
            base.ClearErrors(propertyName);
        }
    }

    public class PersonWithDeferredValidation : ObservableValidator
    {
        [MinLength(4)]
        [MaxLength(20)]
        [Required]
        public string? Name { get; set; }

        [Range(18, 100)]
        public int Age { get; set; }

        // Extra property with no validation
        public float Foo { get; set; } = float.NaN;

        public new void ValidateAllProperties()
        {
            base.ValidateAllProperties();
        }
    }

    /// <summary>
    /// Test model for linked properties, to test <see cref="ObservableValidator.ValidateProperty(object?, string?)"/> instance.
    /// See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3665 for the original request for this feature.
    /// </summary>
    public class ComparableModel : ObservableValidator
    {
        private int a;

        [Range(10, 100)]
        [GreaterThan(nameof(B))]
        public int A
        {
            get => this.a;
            set => SetProperty(ref this.a, value, true);
        }

        private int b;

        [Range(20, 80)]
        public int B
        {
            get => this.b;
            set
            {
                _ = SetProperty(ref this.b, value, true);
                ValidateProperty(A, nameof(A));
            }
        }
    }

    public sealed class GreaterThanAttribute : ValidationAttribute
    {
        public GreaterThanAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            object otherValue = instance.GetType().GetProperty(PropertyName)!.GetValue(instance)!;

            if (((IComparable)value!).CompareTo(otherValue) > 0)
            {
                return ValidationResult.Success!;
            }

            return new("The current value is smaller than the other one");
        }
    }

    /// <summary>
    /// Test model for custom validation properties.
    /// See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3729 for the original request for this feature.
    /// </summary>
    public class CustomValidationModel : ObservableValidator
    {
        public CustomValidationModel(IDictionary<object, object?> items)
            : base(items)
        {
        }

        private int a;

        [CustomValidation(typeof(CustomValidationModel), nameof(ValidateA))]
        public int A
        {
            get => this.a;
            set => SetProperty(ref this.a, value, true);
        }

        public static ValidationResult ValidateA(int x, ValidationContext context)
        {
            Assert.AreEqual(nameof(A), context.MemberName);

            if ((int)context.Items[nameof(A)]! == 42)
            {
                return ValidationResult.Success!;
            }

            return new("Missing the magic number");
        }
    }

    public interface IFancyService
    {
        bool Validate(string name);
    }

    public class FancyService : IFancyService
    {
        public bool Validate(string name)
        {
            return Regex.IsMatch(name, @"^[A-Za-z ]+$");
        }
    }

    /// <summary>
    /// Test model for custom validation with an injected service.
    /// See https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3750 for the original request for this feature.
    /// </summary>
    public class ValidationWithServiceModel : ObservableValidator
    {
        private readonly IFancyService service;

        public ValidationWithServiceModel(IFancyService service)
        {
            this.service = service;
        }

        private string? name;

        [MaxLength(25, ErrorMessage = "The name is too long")]
        [CustomValidation(typeof(ValidationWithServiceModel), nameof(ValidateName))]
        public string? Name
        {
            get => this.name;
            set => SetProperty(ref this.name, value, true);
        }

        public static ValidationResult ValidateName(string name, ValidationContext context)
        {
            bool isValid = ((ValidationWithServiceModel)context.ObjectInstance).service.Validate(name);

            if (isValid)
            {
                return ValidationResult.Success!;
            }

            return new("The name contains invalid characters");
        }
    }

    /// <summary>
    /// Test model for validation with a formatted display name string on each property.
    /// This is issue #1 from https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3763.
    /// </summary>
    public class ValidationWithDisplayName : ObservableValidator
    {
        public ValidationWithDisplayName()
        {
            ValidateAllProperties();
        }

        private string? stringMayNotBeEmpty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "FIRST: {0}.")]
        public string? StringMayNotBeEmpty
        {
            get => this.stringMayNotBeEmpty;
            set => SetProperty(ref this.stringMayNotBeEmpty, value, true);
        }

        private string? anotherRequiredField;

        [Required(AllowEmptyStrings = false, ErrorMessage = "SECOND: {0}.")]
        public string? AnotherRequiredField
        {
            get => this.anotherRequiredField;
            set => SetProperty(ref this.anotherRequiredField, value, true);
        }
    }

    public class ObservableValidatorBase : ObservableValidator
    {
        public int? MyDummyInt { get; set; } = 0;

        public new void ValidateAllProperties()
        {
            base.ValidateAllProperties();
        }
    }

    public class ObservableValidatorDerived : ObservableValidatorBase
    {
        public string? Name { get; set; }

        public int SomeRandomproperty { get; set; }
    }

    public partial class PersonWithPartialDeclaration : ObservableValidator
    {
        [Required]
        [MinLength(1)]
        public string? Name { get; set; }

        public new void ValidateAllProperties()
        {
            base.ValidateAllProperties();
        }
    }

    public partial class PersonWithPartialDeclaration
    {
        [Range(10, 1000)]
        public int Number { get; set; }
    }

    public abstract class AbstractModelWithValidatableProperty : ObservableValidator
    {
        [Required]
        [MinLength(2)]
        public string? Name { get; set; }
    }

    public class DerivedModelWithValidatableProperties : AbstractModelWithValidatableProperty
    {
        [Range(10, 1000)]
        public int Number { get; set; }

        public new void ValidateAllProperties()
        {
            base.ValidateAllProperties();
        }
    }

    public class GenericPerson<T> : ObservableValidator
    {
        [Required]
        [MinLength(1)]
        public string? Name { get; set; }

        [Required]
        public T? Value { get; set; }

        public new void ValidateAllProperties()
        {
            base.ValidateAllProperties();
        }
    }
}

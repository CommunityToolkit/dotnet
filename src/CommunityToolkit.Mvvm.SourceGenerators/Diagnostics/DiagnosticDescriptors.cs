// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.CodeAnalysis;

#pragma warning disable IDE0090 // Use 'new DiagnosticDescriptor(...)'

namespace CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
internal static class DiagnosticDescriptors
{
    /// <summary>
    /// The diagnostic id for <see cref="InheritFromObservableObjectInsteadOfUsingINotifyPropertyChangedAttributeWarning"/>.
    /// </summary>
    public const string InheritFromObservableObjectInsteadOfUsingINotifyPropertyChangedAttributeId = "MVVMTK0032";

    /// <summary>
    /// The diagnostic id for <see cref="InheritFromObservableObjectInsteadOfUsingObservableObjectAttributeWarning"/>.
    /// </summary>
    public const string InheritFromObservableObjectInsteadOfUsingObservableObjectAttributeId = "MVVMTK0033";

    /// <summary>
    /// The diagnostic id for <see cref="FieldReferenceForObservablePropertyFieldWarning"/>.
    /// </summary>
    public const string FieldReferenceForObservablePropertyFieldId = "MVVMTK0034";

    /// <summary>
    /// The diagnostic id for <see cref="AsyncVoidReturningRelayCommandMethod"/>.
    /// </summary>
    public const string AsyncVoidReturningRelayCommandMethodId = "MVVMTK0039";

    /// <summary>
    /// The diagnostic id for <see cref="UseObservablePropertyOnPartialProperty"/>.
    /// </summary>
    public const string UseObservablePropertyOnPartialPropertyId = "MVVMTK0042";

    /// <summary>
    /// The diagnostic id for <see cref="WinRTObservablePropertyOnFieldsIsNotAotCompatible"/>.
    /// </summary>
    public const string WinRTObservablePropertyOnFieldsIsNotAotCompatibleId = "MVVMTK0045";

    /// <summary>
    /// The diagnostic id for <see cref="UseObservablePropertyOnSemiAutoProperty"/>.
    /// </summary>
    public const string UseObservablePropertyOnSemiAutoPropertyId = "MVVMTK0056";

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a duplicate declaration of <see cref="INotifyPropertyChanged"/> would happen.
    /// <para>
    /// Format: <c>"Cannot apply [INotifyPropertyChangedAttribute] to type {0}, as it already declares the INotifyPropertyChanged interface"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0001",
        title: $"Duplicate {nameof(INotifyPropertyChanged)} definition",
        messageFormat: $"Cannot apply [INotifyPropertyChanged] to type {{0}}, as it already declares the {nameof(INotifyPropertyChanged)} interface",
        category: typeof(INotifyPropertyChangedGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot apply [INotifyPropertyChanged] to a type that already declares the {nameof(INotifyPropertyChanged)} interface.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0001");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a duplicate declaration of <see cref="INotifyPropertyChanged"/> would happen.
    /// <para>
    /// Format: <c>"Cannot apply [ObservableObjectAttribute] to type {0}, as it already declares the INotifyPropertyChanged interface"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0002",
        title: $"Duplicate {nameof(INotifyPropertyChanged)} definition",
        messageFormat: $"Cannot apply [ObservableObject] to type {{0}}, as it already declares the {nameof(INotifyPropertyChanged)} interface",
        category: typeof(ObservableObjectGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot apply [ObservableObject] to a type that already declares the {nameof(INotifyPropertyChanged)} interface.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0002");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a duplicate declaration of <see cref="INotifyPropertyChanging"/> would happen.
    /// <para>
    /// Format: <c>"Cannot apply [ObservableObjectAttribute] to type {0}, as it already declares the INotifyPropertyChanging interface"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0003",
        title: $"Duplicate {nameof(INotifyPropertyChanging)} definition",
        messageFormat: $"Cannot apply [ObservableObject] to type {{0}}, as it already declares the {nameof(INotifyPropertyChanging)} interface",
        category: typeof(ObservableObjectGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot apply [ObservableObject] to a type that already declares the {nameof(INotifyPropertyChanging)} interface.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0003");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a duplicate declaration of <see cref="INotifyPropertyChanging"/> would happen.
    /// <para>
    /// Format: <c>"Cannot apply [ObservableRecipientAttribute] to type {0}, as it already inherits from the ObservableRecipient class"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateObservableRecipientError = new DiagnosticDescriptor(
        id: "MVVMTK0004",
        title: "Duplicate ObservableRecipient definition",
        messageFormat: $"Cannot apply [ObservableRecipient] to type {{0}}, as it already inherits from the ObservableRecipient class",
        category: typeof(ObservableRecipientGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot apply [ObservableRecipient] to a type that already inherits from the ObservableRecipient class.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0004");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when there is a missing base functionality to enable <c>ObservableRecipientAttribute</c>.
    /// <para>
    /// Format: <c>"Cannot apply [ObservableRecipientAttribute] to type {0}, as it lacks necessary base functionality (it should either inherit from ObservableObject, or be annotated with [ObservableObjectAttribute] or [INotifyPropertyChangedAttribute])"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MissingBaseObservableObjectFunctionalityError = new DiagnosticDescriptor(
        id: "MVVMTK0005",
        title: "Missing base ObservableObject functionality",
        messageFormat: $"Cannot apply [ObservableRecipient] to type {{0}}, as it lacks necessary base functionality (it should either inherit from ObservableObject, or be annotated with [ObservableObject] or [INotifyPropertyChanged])",
        category: typeof(ObservableRecipientGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot apply [ObservableRecipient] to a type that lacks necessary base functionality (it should either inherit from ObservableObject, or be annotated with [ObservableObject] or [INotifyPropertyChanged]).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0005");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the target type doesn't inherit from the <c>ObservableValidator</c> class.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} cannot be used to generate an observable property, as it has {3} validation attribute(s) but is declared in a type that doesn't inherit from ObservableValidator"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MissingObservableValidatorInheritanceForValidationAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0006",
        title: "Missing ObservableValidator inheritance",
        messageFormat: "The {0} {1}.{2} cannot be used to generate an observable property, as it has {3} validation attribute(s) but is declared in a type that doesn't inherit from ObservableValidator",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [ObservableProperty] to fields or properties with validation attributes if they are declared in a type that doesn't inherit from ObservableValidator.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0006");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an annotated method to generate a command for has an invalid signature.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidRelayCommandMethodSignatureError = new DiagnosticDescriptor(
        id: "MVVMTK0007",
        title: "Invalid RelayCommand method signature",
        messageFormat: "The method {0}.{1} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [RelayCommand] to methods with a signature that doesn't match any of the existing relay command types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0007");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an unsupported C# language version is being used.
    /// </summary>
    public static readonly DiagnosticDescriptor UnsupportedCSharpLanguageVersionError = new DiagnosticDescriptor(
        id: "MVVMTK0008",
        title: "Unsupported C# language version",
        messageFormat: "The source generator features from the MVVM Toolkit require consuming projects to set the C# language version to at least C# 8.0",
        category: typeof(UnsupportedCSharpLanguageVersionAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The source generator features from the MVVM Toolkit require consuming projects to set the C# language version to at least C# 8.0. Make sure to add <LangVersion>8.0</LangVersion> (or above) to your .csproj file.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0008");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a specified <c>CanExecute</c> name has no matching member.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a valid member, but "{0}" has no matches in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidCanExecuteMemberNameError = new DiagnosticDescriptor(
        id: "MVVMTK0009",
        title: "Invalid RelayCommand.CanExecute member name",
        messageFormat: "The CanExecute name must refer to a valid member, but \"{0}\" has no matches in type {1}",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The CanExecute name in [RelayCommand] must refer to a valid member in its parent type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0009");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a specified <c>CanExecute</c> name maps to multiple members.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a single member, but "{0}" has multiple matches in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MultipleCanExecuteMemberNameMatchesError = new DiagnosticDescriptor(
        id: "MVVMTK0010",
        title: "Multiple RelayCommand.CanExecute member name matches",
        messageFormat: "The CanExecute name must refer to a single member, but \"{0}\" has multiple matches in type {1}",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot set the CanExecute name in [RelayCommand] to one that has multiple matches in its parent type (it must refer to a single compatible member).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0010");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a a specified <c>CanExecute</c> name maps to an invalid member.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a compatible member, but no valid members were found for "{0}" in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidCanExecuteMemberError = new DiagnosticDescriptor(
        id: "MVVMTK0011",
        title: "No valid RelayCommand.CanExecute member match",
        messageFormat: "The CanExecute name must refer to a compatible member, but no valid members were found for \"{0}\" in type {1}",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The CanExecute name in [RelayCommand] must refer to a compatible member (either a property or a method) to be used in a generated command.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0011");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>RelayCommandAttribute.AllowConcurrentExecutions</c> is being set for a non-asynchronous method.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be annotated with the [RelayCommand] attribute specifying a concurrency control option, as it maps to a non-asynchronous command type"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidConcurrentExecutionsParameterError = new DiagnosticDescriptor(
        id: "MVVMTK0012",
        title: "Invalid concurrency control option usage",
        messageFormat: "The method {0}.{1} cannot be annotated with the [RelayCommand] attribute specifying a concurrency control option, as it maps to a non-asynchronous command type",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply the [RelayCommand] attribute specifying a concurrency control option to methods mapping to non-asynchronous command types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0012");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>RelayCommandAttribute.IncludeCancelCommandParameter</c> is being set for an invalid method.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be annotated with the [RelayCommand] attribute specifying to include a cancel command, as it does not map to an asynchronous command type taking a cancellation token"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidIncludeCancelCommandParameterError = new DiagnosticDescriptor(
        id: "MVVMTK0013",
        title: "Invalid include cancel command setting usage",
        messageFormat: "The method {0}.{1} cannot be annotated with the [RelayCommand] attribute specifying to include a cancel command, as it does not map to an asynchronous command type taking a cancellation token",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply the [RelayCommand] attribute specifying to include a cancel command to methods not mapping to an asynchronous command type accepting a cancellation token.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0013");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a generated property created with <c>[ObservableProperty]</c> would collide with the source field.
    /// <para>
    /// Format: <c>"The field {0}.{1} cannot be used to generate an observable property, as its name would collide with the field name (instance fields should use the "lowerCamel", "_lowerCamel" or "m_lowerCamel" pattern)</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor ObservablePropertyNameCollisionError = new DiagnosticDescriptor(
        id: "MVVMTK0014",
        title: "Name collision for generated property",
        messageFormat: "The field {0}.{1} cannot be used to generate an observable property, as its name would collide with the field name (instance fields should use the \"lowerCamel\", \"_lowerCamel\" or \"m_lowerCamel\" pattern)",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The name of fields annotated with [ObservableProperty] should use \"lowerCamel\", \"_lowerCamel\" or \"m_lowerCamel\" pattern to avoid collisions with the generated properties.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0014");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the specified target for <c>[NotifyPropertyChangedFor]</c> is not valid.
    /// <para>
    /// Format: <c>"The target(s) of [NotifyPropertyChangedFor] must be a (different) accessible property, but "{0}" has no (other) matches in type {1}</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor NotifyPropertyChangedForInvalidTargetError = new DiagnosticDescriptor(
        id: "MVVMTK0015",
        title: "Invalid target name for [NotifyPropertyChangedFor]",
        messageFormat: "The target(s) of [NotifyPropertyChangedFor] must be a (different) accessible property, but \"{0}\" has no (other) matches in type {1}",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The target(s) of [NotifyPropertyChangedFor] must be a (different) accessible property in its parent type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0015");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the specified target for <c>[NotifyCanExecuteChangedFor]</c> is not valid.
    /// <para>
    /// Format: <c>"The target(s) of [NotifyCanExecuteChangedFor] must be an accessible <c>IRelayCommand</c> property, but "{0}" has no matches in type {1}</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor NotifyCanExecuteChangedForInvalidTargetError = new DiagnosticDescriptor(
        id: "MVVMTK0016",
        title: "Invalid target name for [NotifyCanExecuteChangedFor]",
        messageFormat: "The target(s) of [NotifyCanExecuteChangedFor] must be an accessible IRelayCommand property, but \"{0}\" has no matches in type {1}",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The target(s) of [NotifyCanExecuteChangedFor] must be an accessible IRelayCommand property in its parent type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0016");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[INotifyPropertyChanged]</c> is applied to a type with an attribute already.
    /// <para>
    /// Format: <c>"Cannot apply [INotifyPropertyChanged] to type {0}, as it already has this attribute or [ObservableObject] applied to it (including base types)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidAttributeCombinationForINotifyPropertyChangedAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0017",
        title: "Invalid target type for [INotifyPropertyChanged]",
        messageFormat: "Cannot apply [INotifyPropertyChanged] to type {0}, as it already has this attribute or [ObservableObject] applied to it (including base types)",
        category: typeof(INotifyPropertyChangedGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [INotifyPropertyChanged] to a type that already has this attribute or [ObservableObject] applied to it (including base types).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0017");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableObject]</c> is applied to a type with an attribute already.
    /// <para>
    /// Format: <c>"Cannot apply [ObservableObject] to type {0}, as it already has this attribute or [INotifyPropertyChanged] applied to it (including base types)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidAttributeCombinationForObservableObjectAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0018",
        title: "Invalid target type for [ObservableObject]",
        messageFormat: "Cannot apply [ObservableObject] to type {0}, as it already has this attribute or [INotifyPropertyChanged] applied to it (including base types)",
        category: typeof(ObservableObjectGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [ObservableObject] to a type that already has this attribute or [INotifyPropertyChanged] applied to it (including base types).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0018");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableProperty]</c> is applied to a field or property in an invalid type.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} cannot be used to generate an observable property, as its containing type doesn't inherit from ObservableObject, nor does it use [ObservableObject] or [INotifyPropertyChanged]"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidContainingTypeForObservablePropertyMemberError = new DiagnosticDescriptor(
        id: "MVVMTK0019",
        title: "Invalid containing type for [ObservableProperty] field or property",
        messageFormat: "The {0} {1}.{2} cannot be used to generate an observable property, as its containing type doesn't inherit from ObservableObject, nor does it use [ObservableObject] or [INotifyPropertyChanged]",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Fields and properties annotated with [ObservableProperty] must be contained in a type that inherits from ObservableObject or that is annotated with [ObservableObject] or [INotifyPropertyChanged] (including base types).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0019");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableProperty]</c> is applied to a field or property in an invalid type.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} needs to be annotated with [ObservableProperty] in order to enable using [NotifyPropertyChangedFor], [NotifyCanExecuteChangedFor], [NotifyPropertyChangedRecipients] and [NotifyDataErrorInfo]"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor FieldWithOrphanedDependentObservablePropertyAttributesError = new DiagnosticDescriptor(
        id: "MVVMTK0020",
        title: "Invalid use of attributes dependent on [ObservableProperty]",
        messageFormat: "The {0} {1}.{2} needs to be annotated with [ObservableProperty] in order to enable using [NotifyPropertyChangedFor], [NotifyCanExecuteChangedFor], [NotifyPropertyChangedRecipients] and [NotifyDataErrorInfo]",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Fields and properties not annotated with [ObservableProperty] cannot use [NotifyPropertyChangedFor], [NotifyCanExecuteChangedFor], [NotifyPropertyChangedRecipients] and [NotifyDataErrorInfo].",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0020");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableRecipient]</c> is applied to a type with an attribute already.
    /// <para>
    /// Format: <c>"Cannot apply [ObservableRecipient] to type {0}, as it already inherits this attribute from a base type"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidAttributeCombinationForObservableRecipientAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0021",
        title: "Invalid target type for [ObservableRecipient]",
        messageFormat: "Cannot apply [ObservableRecipient] to type {0}, as it already inherits this attribute from a base type",
        category: typeof(ObservableRecipientGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [ObservableRecipient] to a type that already inherits this attribute from a base type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0021");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[NotifyPropertyChangedRecipients]</c> is applied to a field or property in an invalid type.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} cannot be annotated with [NotifyPropertyChangedRecipients], as its containing type doesn't inherit from ObservableRecipient, nor does it use [ObservableRecipient]"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidContainingTypeForNotifyPropertyChangedRecipientsMemberError = new DiagnosticDescriptor(
        id: "MVVMTK0022",
        title: "Invalid containing type for [ObservableProperty] field or property",
        messageFormat: "The {0} {1}.{2} cannot be annotated with [NotifyPropertyChangedRecipients], as its containing type doesn't inherit from ObservableRecipient, nor does it use [ObservableRecipient]",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Fields and properties annotated with [NotifyPropertyChangedRecipients] must be contained in a type that inherits from ObservableRecipient or that is annotated with [ObservableRecipient] (including base types).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0022");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a specified <c>[RelayCommand]</c> method has any overloads.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a single member, but "{0}" has multiple matches in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MultipleRelayCommandMethodOverloadsError = new DiagnosticDescriptor(
        id: "MVVMTK0023",
        title: "Multiple overloads for method annotated with RelayCommand",
        messageFormat: "The method {0}.{1} cannot be annotated with [RelayCommand], has it has multiple overloads (command methods must be unique within their containing type)",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Methods with multiple overloads cannot be annotated with [RelayCommand], as command methods must be unique within their containing type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0023");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a generated property created with <c>[ObservableProperty]</c> would cause conflicts with other generated members.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} cannot be used to generate an observable property, as its name or type would cause conflicts with other generated members"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidObservablePropertyError = new DiagnosticDescriptor(
        id: "MVVMTK0024",
        title: "Invalid generated property declaration",
        messageFormat: "The {0} {1}.{2} cannot be used to generate an observable property, as its name or type would cause conflicts with other generated members",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The fields and properties annotated with [ObservableProperty] cannot result in a property name or have a type that would cause conflicts with other generated members.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0024");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the target type doesn't inherit from the <c>ObservableValidator</c> class.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} cannot be annotated with [NotifyDataErrorInfo], as it is declared in a type that doesn't inherit from ObservableValidator"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MissingObservableValidatorInheritanceForNotifyDataErrorInfoError = new DiagnosticDescriptor(
        id: "MVVMTK0025",
        title: "Missing ObservableValidator inheritance",
        messageFormat: "The {0} {1}.{2} cannot be annotated with [NotifyDataErrorInfo], as it is declared in a type that doesn't inherit from ObservableValidator",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [NotifyDataErrorInfo] to fields and properties that are declared in a type that doesn't inherit from ObservableValidator.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0025");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the target field or property uses [NotifyDataErrorInfo] but has no validation attributes.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} cannot be annotated with [NotifyDataErrorInfo], as it doesn't have any validation attributes to use during validation"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MissingValidationAttributesForNotifyDataErrorInfoError = new DiagnosticDescriptor(
        id: "MVVMTK0026",
        title: "Missing validation attributes",
        messageFormat: "The {0} {1}.{2} cannot be annotated with [NotifyDataErrorInfo], as it doesn't have any validation attributes to use during validation",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [NotifyDataErrorInfo] to fields and properties that don't have any validation attributes to use during validation.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0026");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[NotifyPropertyChangedRecipients]</c> is applied to an invalid type.
    /// <para>
    /// Format: <c>"The type {0} cannot be annotated with [NotifyPropertyChangedRecipients], as it doesn't inherit from ObservableRecipient, nor does it use [ObservableRecipient]"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidTypeForNotifyPropertyChangedRecipientsError = new DiagnosticDescriptor(
        id: "MVVMTK0027",
        title: "Invalid type for [NotifyPropertyChangedRecipients] attribute",
        messageFormat: "The type {0} cannot be annotated with [NotifyPropertyChangedRecipients], as it doesn't inherit from ObservableRecipient, nor does it use [ObservableRecipient]",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Types annotated with [NotifyPropertyChangedRecipients] must inherit from ObservableRecipient or be annotated with [ObservableRecipient] (including base types).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0027");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[NotifyDataErrorInfo]</c> is applied to an invalid type.
    /// <para>
    /// Format: <c>"The type {0} cannot be annotated with [NotifyDataErrorInfo], as it doesn't inherit from ObservableValidator"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidTypeForNotifyDataErrorInfoError = new DiagnosticDescriptor(
        id: "MVVMTK0028",
        title: "Invalid type for [NotifyDataErrorInfo] attribute",
        messageFormat: "The type {0} cannot be annotated with [NotifyDataErrorInfo], as it doesn't inherit from ObservableValidator",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Types annotated with [NotifyDataErrorInfo] must inherit from ObservableValidator.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0028");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[NotifyPropertyChangedRecipients]</c> is applied to a field or property in a class with <c>[NotifyPropertyChangedRecipients]</c> used at the class-level.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} is annotated with [NotifyPropertyChangedRecipients], but that is not needed since its containing type already uses or inherits [NotifyPropertyChangedRecipients] at the class-level"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor UnnecessaryNotifyPropertyChangedRecipientsAttributeOnMemberWarning = new DiagnosticDescriptor(
        id: "MVVMTK0029",
        title: "Unnecessary [NotifyPropertyChangedRecipients] field or property annotation",
        messageFormat: "The {0} {1}.{2} is annotated with [NotifyPropertyChangedRecipients], but that is not needed since its containing type already uses or inherits [NotifyPropertyChangedRecipients] at the class-level",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Annotating a field or property with [NotifyPropertyChangedRecipients] is not necessary if the containing type has or inherits [NotifyPropertyChangedRecipients] at the class-level.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0029");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[NotifyDataErrorInfo]</c> is applied to a field or property in a class with <c>[NotifyDataErrorInfo]</c> used at the class-level.
    /// <para>
    /// Format: <c>"The {0} {1}.{2} is annotated with [NotifyDataErrorInfo], but that is not needed since its containing type already uses or inherits [NotifyDataErrorInfo] at the class-level"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor UnnecessaryNotifyDataErrorInfoAttributeOnMemberWarning = new DiagnosticDescriptor(
        id: "MVVMTK0030",
        title: "Unnecessary [NotifyDataErrorInfo] field or property annotation",
        messageFormat: "The {0} {1}.{2} is annotated with [NotifyDataErrorInfo], but that is not needed since its containing type already uses or inherits [NotifyDataErrorInfo] at the class-level",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Annotating a field or property with [NotifyDataErrorInfo] is not necessary if the containing type has or inherits [NotifyDataErrorInfo] at the class-level.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0030");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>RelayCommandAttribute.FlowExceptionsToTaskScheduler</c> is being set for a non-asynchronous method.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be annotated with the [RelayCommand] attribute specifying an exception flow option, as it maps to a non-asynchronous command type"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidFlowExceptionsToTaskSchedulerParameterError = new DiagnosticDescriptor(
        id: "MVVMTK0031",
        title: "Invalid task scheduler exception flow option usage",
        messageFormat: "The method {0}.{1} cannot be annotated with the [RelayCommand] attribute specifying a task scheduler exception flow option, as it maps to a non-asynchronous command type",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply the [RelayCommand] attribute specifying a task scheduler exception flow option to methods mapping to non-asynchronous command types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0031");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[INotifyPropertyChanged]</c> is used on a type that could inherit from <c>ObservableObject</c> instead.
    /// <para>
    /// Format: <c>"The type {0} is using the [INotifyPropertyChanged] attribute while having no base type, and it should instead inherit from ObservableObject"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InheritFromObservableObjectInsteadOfUsingINotifyPropertyChangedAttributeWarning = new DiagnosticDescriptor(
        id: InheritFromObservableObjectInsteadOfUsingINotifyPropertyChangedAttributeId,
        title: "Inherit from ObservableObject instead of using [INotifyPropertyChanged]",
        messageFormat: "The type {0} is using the [INotifyPropertyChanged] attribute while having no base type, and it should instead inherit from ObservableObject",
        category: typeof(INotifyPropertyChangedGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
            "Classes with no base types should prefer inheriting from ObservableObject instead of using attributes to generate INotifyPropertyChanged code, as that will " +
            "reduce the binary size of the application (the attributes are only meant to support cases where the annotated types are already inheriting from a different type).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0032");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableObject]</c> is used on a type that could inherit from <c>ObservableObject</c> instead.
    /// <para>
    /// Format: <c>"The type {0} is using the [ObservableObject] attribute while having no base type, and it should instead inherit from ObservableObject"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InheritFromObservableObjectInsteadOfUsingObservableObjectAttributeWarning = new DiagnosticDescriptor(
        id: InheritFromObservableObjectInsteadOfUsingObservableObjectAttributeId,
        title: "Inherit from ObservableObject instead of using [ObservableObject]",
        messageFormat: "The type {0} is using the [ObservableObject] attribute while having no base type, and it should instead inherit from ObservableObject",
        category: typeof(ObservableObjectGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
            "Classes with no base types should prefer inheriting from ObservableObject instead of using attributes to generate INotifyPropertyChanged code, as that will " +
            "reduce the binary size of the application (the attributes are only meant to support cases where the annotated types are already inheriting from a different type).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0033");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a field with <c>[ObservableProperty]</c> is being directly referenced.
    /// <para>
    /// Format: <c>"The field {0} is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor FieldReferenceForObservablePropertyFieldWarning = new DiagnosticDescriptor(
        id: FieldReferenceForObservablePropertyFieldId,
        title: "Direct field reference to [ObservableProperty] backing field",
        messageFormat: "The field {0} is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Fields with [ObservableProperty] should not be directly referenced, and the generated properties should be used instead.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0034");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a field with <c>[ObservableProperty]</c> is using an invalid attribute targeting the property.
    /// <para>
    /// Format: <c>"The field {0} annotated with [ObservableProperty] is using attribute "{1}" which was not recognized as a valid type (are you missing a using directive?)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyTargetedAttributeOnObservablePropertyField = new DiagnosticDescriptor(
        id: "MVVMTK0035",
        title: "Invalid property targeted attribute type",
        messageFormat: "The field {0} annotated with [ObservableProperty] is using attribute \"{1}\" which was not recognized as a valid type (are you missing a using directive?)",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All attributes targeting the generated property for a field annotated with [ObservableProperty] must correctly be resolved to valid types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0035");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a method with <c>[RelayCommand]</c> is using an invalid attribute targeting the field or property.
    /// <para>
    /// Format: <c>"The method {0} annotated with [RelayCommand] is using attribute "{1}" which was not recognized as a valid type (are you missing a using directive?)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidFieldOrPropertyTargetedAttributeOnRelayCommandMethod = new DiagnosticDescriptor(
        id: "MVVMTK0036",
        title: "Invalid field or property targeted attribute type",
        messageFormat: "The method {0} annotated with [RelayCommand] is using attribute \"{1}\" which was not recognized as a valid type (are you missing a using directive?)",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All attributes targeting the generated field or property for a method annotated with [RelayCommand] must correctly be resolved to valid types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0036");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a field with <c>[ObservableProperty]</c> is using an invalid attribute expression targeting the property.
    /// <para>
    /// Format: <c>"The field {0} annotated with [ObservableProperty] is using attribute "{1}" with an invalid expression (are you passing any incorrect parameters to the attribute constructor?)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyTargetedAttributeExpressionOnObservablePropertyField = new DiagnosticDescriptor(
        id: "MVVMTK0037",
        title: "Invalid property targeted attribute expression",
        messageFormat: "The field {0} annotated with [ObservableProperty] is using attribute \"{1}\" with an invalid expression (are you passing any incorrect parameters to the attribute constructor?)",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All attributes targeting the generated property for a field annotated with [ObservableProperty] must be using valid expressions.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a method with <c>[RelayCommand]</c> is using an invalid attribute targeting the field or property.
    /// <para>
    /// Format: <c>"The method {0} annotated with [RelayCommand] is using attribute "{1}" with an invalid expression (are you passing any incorrect parameters to the attribute constructor?)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidFieldOrPropertyTargetedAttributeExpressionOnRelayCommandMethod = new DiagnosticDescriptor(
        id: "MVVMTK0038",
        title: "Invalid field or property targeted attribute expression",
        messageFormat: "The method {0} annotated with [RelayCommand] is using attribute \"{1}\" with an invalid expression (are you passing any incorrect parameters to the attribute constructor?)",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All attributes targeting the generated field or property for a method annotated with [RelayCommand] must be using valid expressions.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a method with <c>[RelayCommand]</c> is async void.
    /// <para>
    /// Format: <c>"The method {0} annotated with [RelayCommand] is async void (make sure to return a Task type instead)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor AsyncVoidReturningRelayCommandMethod = new DiagnosticDescriptor(
        id: AsyncVoidReturningRelayCommandMethodId,
        title: "Async void returning method annotated with RelayCommand",
        messageFormat: "The method {0} annotated with [RelayCommand] is async void (make sure to return a Task type instead)",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All asynchronous methods annotated with [RelayCommand] should return a Task type, to benefit from the additional support provided by AsyncRelayCommand and AsyncRelayCommand<T>.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0039");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableProperty]</c> is used on a generated field of an auto-property.
    /// <para>
    /// Format: <c>"The backing field for property {0}.{1} cannot be annotated with [ObservableProperty] (the attribute can only be used directly on fields, and the generator will then handle generating the corresponding property)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor AutoPropertyBackingFieldObservableProperty = new DiagnosticDescriptor(
        id: "MVVMTK0040",
        title: "[ObservableProperty] on auto-property backing field",
        messageFormat: "The backing field for property {0}.{1} cannot be annotated with [ObservableProperty] (the attribute can only be used directly on fields, and the generator will then handle generating the corresponding property)",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The backing fields of auto-properties cannot be annotated with [ObservableProperty] (the attribute can only be used directly on fields, and the generator will then handle generating the corresponding property).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0040");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for the C# language version not being sufficient for <c>[ObservableProperty]</c> on partial properties.
    /// <para>
    /// Format: <c>"Using [ObservableProperty] on partial properties requires the C# language version to be set to 'preview', as support for the 'field' keyword is needed by the source generators to emit valid code (add <LangVersion>preview</LangVersion> to your .csproj/.props file)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor CSharpLanguageVersionIsNotPreviewForObservableProperty = new DiagnosticDescriptor(
        id: "MVVMTK0041",
        title: "C# language version is not 'preview'",
        messageFormat: """Using [ObservableProperty] on partial properties requires the C# language version to be set to 'preview', as support for the 'field' keyword is needed by the source generators to emit valid code (add <LangVersion>preview</LangVersion> to your .csproj/.props file)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The C# language version must be set to 'preview' when using [ObservableProperty] on partial properties for the source generators to emit valid code (the <LangVersion>preview</LangVersion> option must be set in the .csproj/.props file).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0041");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[ObservableProperty]</c> on a field should be converted to a partial property.
    /// <para>
    /// Format: <c>"The field {0}.{1} using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor UseObservablePropertyOnPartialProperty = new DiagnosticDescriptor(
        id: UseObservablePropertyOnPartialPropertyId,
        title: "Prefer using [ObservableProperty] on partial properties",
        messageFormat: """The field {0}.{1} using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fields using [ObservableProperty] can be converted to partial properties instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0042");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableProperty]</c> is applied to a property with an invalid declaration.
    /// <para>
    /// Format: <c>"The property {0}.{1} cannot be used to generate an observable property, as its declaration is not valid (it must be an instance (non static) partial property with a getter and a setter that is not init-only)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationForObservableProperty = new DiagnosticDescriptor(
        id: "MVVMTK0043",
        title: "Invalid property declaration for [ObservableProperty]",
        messageFormat: "The property {0}.{1} cannot be used to generate an observable property, as its declaration is not valid (it must be an instance (non static) partial property with a getter and a setter that is not init-only)",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [ObservableProperty] must be instance (non static) partial properties with a getter and a setter that is not init-only.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0043");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableProperty]</c> is applied to a property when an unsupported version of Roslyn is used.
    /// <para>
    /// Format: <c>"The property {0}.{1} cannot be used to generate an observable property, as the current Roslyn version being used is not high enough (remove [ObservableProperty] or target a field instead, or upgrade to at least Visual Studio 2022 version 17.12 and the .NET 9 SDK)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor UnsupportedRoslynVersionForObservablePartialPropertySupport = new DiagnosticDescriptor(
        id: "MVVMTK0044",
        title: "Unsupported Roslyn version for using [ObservableProperty] on partial properties",
        messageFormat: "The property {0}.{1} cannot be used to generate an observable property, as the current Roslyn version being used is not high enough (remove [ObservableProperty] or target a field instead, or upgrade to at least Visual Studio 2022 version 17.12 and the .NET 9 SDK)",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Using [ObservableProperty] with (partial) properties requires a higher version of Roslyn (remove [ObservableProperty] or target a field instead, or upgrade to at least Visual Studio 2022 version 17.12 and the .NET 9 SDK).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0044");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[ObservableProperty]</c> is used on a field in WinRT scenarios.
    /// <para>
    /// Format: <c>"The field {0}.{1} using [ObservableProperty] will generate code that is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and a partial property should be used instead (as it allows the CsWinRT generators to correctly produce the necessary WinRT marshalling code)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor WinRTObservablePropertyOnFieldsIsNotAotCompatible = new DiagnosticDescriptor(
        id: WinRTObservablePropertyOnFieldsIsNotAotCompatibleId,
        title: "Using [ObservableProperty] on fields is not AOT compatible for WinRT",
        messageFormat: """The field {0}.{1} using [ObservableProperty] will generate code that is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and a partial property should be used instead (as it allows the CsWinRT generators to correctly produce the necessary WinRT marshalling code)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Fields using [ObservableProperty] will generate code that is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and partial properties should be used instead (as they allow the CsWinRT generators to correctly produce the necessary WinRT marshalling code).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0045");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[RelayCommand]</c> is used on a method in types where <c>[GeneratedBindableCustomProperty]</c> is used.
    /// <para>
    /// Format: <c>"The method {0} using [RelayCommand] within a type also using [GeneratedBindableCustomProperty] and including the generated property, which is not supported, and a manually declared command property should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated command property that is produced by the MVVM Toolkit generator)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatible = new DiagnosticDescriptor(
        id: "MVVMTK0046",
        title: "Using [RelayCommand] is not compatible with [GeneratedBindableCustomProperty]",
        messageFormat: """The method {0} using [RelayCommand] within a type also using [GeneratedBindableCustomProperty] and including the generated property, which is not supported, and a manually declared command property should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated command property that is produced by the MVVM Toolkit generator)""",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Using [RelayCommand] on methods within a type also using [GeneratedBindableCustomProperty] and including the generated property is not supported, and a manually declared command property should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated command property that is produced by the MVVM Toolkit generator).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0046");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[GeneratedBindableCustomProperty]</c> is used on a type that also uses <c>[ObservableProperty]</c> on any declared or inherited fields.
    /// <para>
    /// Format: <c>"The type {0} using [GeneratedBindableCustomProperty] is also using [ObservableProperty] on its declared (or inherited) field {1}.{2}, and including the generated property: combining the two generators is not supported, and partial properties should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated property that is produced by the MVVM Toolkit generator)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor WinRTGeneratedBindableCustomPropertyWithBaseObservablePropertyOnField = new DiagnosticDescriptor(
        id: "MVVMTK0047",
        title: "Using [GeneratedBindableCustomProperty] is not compatible with [ObservableProperty] on fields",
        messageFormat: """The type {0} using [GeneratedBindableCustomProperty] is also using [ObservableProperty] on its declared (or inherited) field {1}.{2}, and including the generated property: combining the two generators is not supported, and partial properties should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated property that is produced by the MVVM Toolkit generator)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Using [GeneratedBindableCustomProperty] on types that also use [ObservableProperty] on any declared (or inherited) fields and including the generated property is not supported, and partial properties should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated property that is produced by the MVVM Toolkit generator).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0047");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[GeneratedBindableCustomProperty]</c> is used on a type that also uses <c>[RelayCommand]</c> on any declared or inherited methods.
    /// <para>
    /// Format: <c>"The type {0} using [GeneratedBindableCustomProperty] is also using [RelayCommand] on its inherited method {1} and including the generated property: combining the two generators is not supported, and a manually declared command property should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated property that is produced by the MVVM Toolkit generator)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor WinRTGeneratedBindableCustomPropertyWithBaseRelayCommand = new DiagnosticDescriptor(
        id: "MVVMTK0048",
        title: "Using [GeneratedBindableCustomProperty] is not compatible with [RelayCommand]",
        messageFormat: """The type {0} using [GeneratedBindableCustomProperty] is also using [RelayCommand] on its inherited method {1} and including the generated property: combining the two generators is not supported, and a manually declared command property should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated property that is produced by the MVVM Toolkit generator)""",
        category: typeof(RelayCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Using [GeneratedBindableCustomProperty] on types that also use [RelayCommand] on any inherited methods and including the generated property is not supported, and a manually declared command property should be used instead (the [GeneratedBindableCustomProperty] generator cannot see the generated property that is produced by the MVVM Toolkit generator).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0048");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[INotifyPropertyChanged]</c> is used on a type in WinRT scenarios.
    /// <para>
    /// Format: <c>"The type {0} is using the [INotifyPropertyChanged] attribute, with is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and it should derive from ObservableObject or manually implement INotifyPropertyChanged instead (as it allows the CsWinRT generators to correctly produce the necessary WinRT marshalling code)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor WinRTUsingINotifyPropertyChangedAttribute = new DiagnosticDescriptor(
        id: "MVVMTK0049",
        title: "Using [INotifyPropertyChanged] is not AOT compatible for WinRT",
        messageFormat: "The type {0} is using the [INotifyPropertyChanged] attribute, with is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and it should derive from ObservableObject or manually implement INotifyPropertyChanged instead (as it allows the CsWinRT generators to correctly produce the necessary WinRT marshalling code)",
        category: typeof(INotifyPropertyChangedGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Using the [INotifyPropertyChanged] attribute on types is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and they should derive from ObservableObject or manually implement INotifyPropertyChanged instead (as it allows the CsWinRT generators to correctly produce the necessary WinRT marshalling code).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0049");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableObject]</c> is used on a type in WinRT scenarios.
    /// <para>
    /// Format: <c>"The type {0} is using the [ObservableObject] attribute, with is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and it should derive from ObservableObject instead (as it allows the CsWinRT generators to correctly produce the necessary WinRT marshalling code)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor WinRTUsingObservableObjectAttribute = new DiagnosticDescriptor(
        id: "MVVMTK0050",
        title: "Using [ObservableObject] is not AOT compatible for WinRT",
        messageFormat: "The type {0} is using the [ObservableObject] attribute, with is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and it should derive from ObservableObject instead (as it allows the CsWinRT generators to correctly produce the necessary WinRT marshalling code)",
        category: typeof(ObservableObjectGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Using the [ObservableObject] attribute on types is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps), and they should derive from ObservableObject instead (as it allows the CsWinRT generators to correctly produce the necessary WinRT marshalling code).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0050");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[ObservableProperty]</c> is used on a field in WinRT scenarios.
    /// <para>
    /// Format: <c>"This project produced one or more 'MVVMTK0045' warnings due to [ObservableProperty] being used on fields, which is not AOT compatible in WinRT scenarios, but it can't enable partial properties and the associated code fixer because 'LangVersion' is not set to 'preview' (setting 'LangVersion=preview' is required to use [ObservableProperty] on partial properties and address these warnings)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor WinRTObservablePropertyOnFieldsIsNotAotCompatibleCompilationEndInfo = new DiagnosticDescriptor(
        id: "MVVMTK0051",
        title: "Using [ObservableProperty] with WinRT and AOT requires 'LangVersion=preview'",
        messageFormat: """This project produced one or more 'MVVMTK0045' warnings due to [ObservableProperty] being used on fields, which is not AOT compatible in WinRT scenarios, but it can't enable partial properties and the associated code fixer because 'LangVersion' is not set to 'preview' (setting 'LangVersion=preview' is required to use [ObservableProperty] on partial properties and address these warnings)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "This project producing one or more 'MVVMTK0045' warnings due to [ObservableProperty] being used on fields, which is not AOT compatible in WinRT scenarios, should set 'LangVersion' to 'preview' to enable partial properties and the associated code fixer (setting 'LangVersion=preview' is required to use [ObservableProperty] on partial properties and address these warnings).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0051",
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[ObservableProperty]</c> is used on a property that is not an incomplete partial definition.
    /// <para>
    /// Format: <c>"The property {0}.{1} is not an incomplete partial definition ([ObservableProperty] must be used on partial property definitions with no implementation part)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidObservablePropertyDeclarationIsNotIncompletePartialDefinition = new DiagnosticDescriptor(
        id: "MVVMTK0052",
        title: "Using [ObservableProperty] on an invalid property declaration (not incomplete partial definition)",
        messageFormat: """The property {0}.{1} is not an incomplete partial definition ([ObservableProperty] must be used on partial property definitions with no implementation part)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A property using [ObservableProperty] is not an incomplete partial definition part ([ObservableProperty] must be used on partial property definitions with no implementation part).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0052");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[ObservableProperty]</c> is used on a property that returns a ref value.
    /// <para>
    /// Format: <c>"The property {0}.{1} returns a ref value ([ObservableProperty] must be used on properties returning a type by value)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidObservablePropertyDeclarationReturnsByRef = new DiagnosticDescriptor(
        id: "MVVMTK0053",
        title: "Using [ObservableProperty] on a property that returns byref",
        messageFormat: """The property {0}.{1} returns a ref value ([ObservableProperty] must be used on properties returning a type by value)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A property using [ObservableProperty] returns a value by reference ([ObservableProperty] must be used on properties returning a type by value).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0053");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[ObservableProperty]</c> is used on a property that returns a byref-like value.
    /// <para>
    /// Format: <c>"The property {0}.{1} returns a byref-like value ([ObservableProperty] must be used on properties of a non byref-like type)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidObservablePropertyDeclarationReturnsRefLikeType = new DiagnosticDescriptor(
        id: "MVVMTK0054",
        title: "Using [ObservableProperty] on a property that returns byref-like",
        messageFormat: """The property {0}.{1} returns a byref-like value ([ObservableProperty] must be used on properties of a non byref-like type)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A property using [ObservableProperty] returns a byref-like value ([ObservableProperty] must be used on properties of a non byref-like type).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0054");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when <c>[ObservableProperty]</c> is used on a property that returns a pointer type.
    /// <para>
    /// Format: <c>"The property {0}.{1} returns a pointer or function pointer value ([ObservableProperty] must be used on properties of a non pointer-like type)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidObservablePropertyDeclarationReturnsPointerLikeType = new DiagnosticDescriptor(
        id: "MVVMTK0055",
        title: "Using [ObservableProperty] on a property that returns pointer-like",
        messageFormat: """The property {0}.{1} returns a pointer or function pointer value ([ObservableProperty] must be used on properties of a non pointer-like type)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A property using [ObservableProperty] returns a pointer-like value ([ObservableProperty] must be used on properties of a non pointer-like type).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0055");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for when a semi-auto property can be converted to use <c>[ObservableProperty]</c> instead.
    /// <para>
    /// Format: <c>"The semi-auto property {0}.{1} can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor UseObservablePropertyOnSemiAutoProperty = new DiagnosticDescriptor(
        id: UseObservablePropertyOnSemiAutoPropertyId,
        title: "Prefer using [ObservableProperty] over semi-auto properties",
        messageFormat: """The semi-auto property {0}.{1} can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)""",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Semi-auto properties should be converted to partial properties using [ObservableProperty] when possible, which is recommended (doing so makes the code less verbose and results in more optimized code).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0056");
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#pragma warning disable IDE0090 // Use 'new DiagnosticDescriptor(...)'

namespace CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
internal static class DiagnosticDescriptors
{
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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the target type doesn't inherit from the <c>ObservableValidator</c> class.
    /// <para>
    /// Format: <c>"The field {0}.{1} cannot be used to generate an observable property, as it has {2} validation attribute(s) but is declared in a type that doesn't inherit from ObservableValidator"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MissingObservableValidatorInheritanceForValidationAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0006",
        title: "Missing ObservableValidator inheritance",
        messageFormat: "The field {0}.{1} cannot be used to generate an observable property, as it has {2} validation attribute(s) but is declared in a type that doesn't inherit from ObservableValidator",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [ObservableProperty] to fields with validation attributes if they are declared in a type that doesn't inherit from ObservableValidator.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an unsupported C# language version is being used.
    /// </summary>
    public static readonly DiagnosticDescriptor UnsupportedCSharpLanguageVersionError = new DiagnosticDescriptor(
        id: "MVVMTK0008",
        title: "Unsupported C# language version",
        messageFormat: "The source generator features from the MVVM Toolkit require consuming projects to set the C# language version to at least C# 8.0",
        category: typeof(CSharpParseOptions).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The source generator features from the MVVM Toolkit require consuming projects to set the C# language version to at least C# 8.0. Make sure to add <LangVersion>8.0</LangVersion> (or above) to your .csproj file.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableProperty]</c> is applied to a field in an invalid type.
    /// <para>
    /// Format: <c>"The field {0}.{1} cannot be used to generate an observable property, as its containing type doesn't inherit from ObservableObject, nor does it use [ObservableObject] or [INotifyPropertyChanged]"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidContainingTypeForObservablePropertyFieldError = new DiagnosticDescriptor(
        id: "MVVMTK0019",
        title: "Invalid containing type for [ObservableProperty] field",
        messageFormat: "The field {0}.{1} cannot be used to generate an observable property, as its containing type doesn't inherit from ObservableObject, nor does it use [ObservableObject] or [INotifyPropertyChanged]",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Fields annotated with [ObservableProperty] must be contained in a type that inherits from ObservableObject or that is annotated with [ObservableObject] or [INotifyPropertyChanged] (including base types).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[ObservableProperty]</c> is applied to a field in an invalid type.
    /// <para>
    /// Format: <c>"The field {0}.{1} needs to be annotated with [ObservableProperty] in order to enable using [NotifyPropertyChangedFor], [NotifyCanExecuteChangedFor], [NotifyPropertyChangedRecipients] and [NotifyDataErrorInfo]"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor FieldWithOrphanedDependentObservablePropertyAttributesError = new DiagnosticDescriptor(
        id: "MVVMTK0020",
        title: "Invalid use of attributes dependent on [ObservableProperty]",
        messageFormat: "The field {0}.{1} needs to be annotated with [ObservableProperty] in order to enable using [NotifyPropertyChangedFor], [NotifyCanExecuteChangedFor], [NotifyPropertyChangedRecipients] and [NotifyDataErrorInfo]",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Fields not annotated with [ObservableProperty] cannot use [NotifyPropertyChangedFor], [NotifyCanExecuteChangedFor], [NotifyPropertyChangedRecipients] and [NotifyDataErrorInfo].",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[NotifyPropertyChangedRecipients]</c> is applied to a field in an invalid type.
    /// <para>
    /// Format: <c>"The field {0}.{1} cannot be annotated with [NotifyPropertyChangedRecipients], as its containing type doesn't inherit from ObservableRecipient, nor does it use [ObservableRecipient]"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidContainingTypeForNotifyPropertyChangedRecipientsFieldError = new DiagnosticDescriptor(
        id: "MVVMTK0022",
        title: "Invalid containing type for [ObservableProperty] field",
        messageFormat: "The field {0}.{1} cannot be annotated with [NotifyPropertyChangedRecipients], as its containing type doesn't inherit from ObservableRecipient, nor does it use [ObservableRecipient]",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Fields annotated with [NotifyPropertyChangedRecipients] must be contained in a type that inherits from ObservableRecipient or that is annotated with [ObservableRecipient] (including base types).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a generated property created with <c>[ObservableProperty]</c> would cause conflicts with other generated members.
    /// <para>
    /// Format: <c>"The field {0}.{1} cannot be used to generate an observable property, as its name or type would cause conflicts with other generated members"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidObservablePropertyError = new DiagnosticDescriptor(
        id: "MVVMTK0024",
        title: "Invalid generated property declaration",
        messageFormat: "The field {0}.{1} cannot be used to generate an observable property, as its name or type would cause conflicts with other generated members",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The fields annotated with [ObservableProperty] cannot result in a property name or have a type that would cause conflicts with other generated members.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the target type doesn't inherit from the <c>ObservableValidator</c> class.
    /// <para>
    /// Format: <c>"The field {0}.{1} cannot be annotated with [NotifyDataErrorInfo], as it is declared in a type that doesn't inherit from ObservableValidator"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MissingObservableValidatorInheritanceForNotifyDataErrorInfoError = new DiagnosticDescriptor(
        id: "MVVMTK0025",
        title: "Missing ObservableValidator inheritance",
        messageFormat: "The field {0}.{1} cannot be annotated with [NotifyDataErrorInfo], as it is declared in a type that doesn't inherit from ObservableValidator",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [NotifyDataErrorInfo] to fields that are declared in a type that doesn't inherit from ObservableValidator.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the target field uses [NotifyDataErrorInfo] but has no validation attributes.
    /// <para>
    /// Format: <c>"The field {0}.{1} cannot be annotated with [NotifyDataErrorInfo], as it doesn't have any validation attributes to use during validation"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MissingValidationAttributesForNotifyDataErrorInfoError = new DiagnosticDescriptor(
        id: "MVVMTK0026",
        title: "Missing validation attributes",
        messageFormat: "The field {0}.{1} cannot be annotated with [NotifyDataErrorInfo], as it doesn't have any validation attributes to use during validation",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [NotifyDataErrorInfo] to fields that don't have any validation attributes to use during validation.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        description: "Types annotated with [NotifyDataErrorInfo] must inherit from ObservableRecipient.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[NotifyPropertyChangedRecipients]</c> is applied to a field in a class with <c>[NotifyPropertyChangedRecipients]</c> used at the class-level.
    /// <para>
    /// Format: <c>"The field {0}.{1} is annotated with [NotifyPropertyChangedRecipients], but that is not needed since its containing type already uses or inherits [NotifyPropertyChangedRecipients] at the class-level"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor UnnecessaryNotifyPropertyChangedRecipientsAttributeOnFieldWarning = new DiagnosticDescriptor(
        id: "MVVMTK0029",
        title: "Unnecessary [NotifyPropertyChangedRecipients] field annotation",
        messageFormat: "The field {0}.{1} is annotated with [NotifyPropertyChangedRecipients], but that is not needed since its containing type already uses or inherits [NotifyPropertyChangedRecipients] at the class-level",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Annotating a field with [NotifyPropertyChangedRecipients] is not necessary if the containing type has or inherits [NotifyPropertyChangedRecipients] at the class-level.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[NotifyDataErrorInfo]</c> is applied to a field in a class with <c>[NotifyDataErrorInfo]</c> used at the class-level.
    /// <para>
    /// Format: <c>"The field {0}.{1} is annotated with [NotifyDataErrorInfo], but that is not needed since its containing type already uses or inherits [NotifyDataErrorInfo] at the class-level"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor UnnecessaryNotifyDataErrorInfoAttributeOnFieldWarning = new DiagnosticDescriptor(
        id: "MVVMTK0030",
        title: "Unnecessary [NotifyDataErrorInfo] field annotation",
        messageFormat: "The field {0}.{1} is annotated with [NotifyDataErrorInfo], but that is not needed since its containing type already uses or inherits [NotifyDataErrorInfo] at the class-level",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Annotating a field with [NotifyDataErrorInfo] is not necessary if the containing type has or inherits [NotifyDataErrorInfo] at the class-level.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

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
        helpLinkUri: "https://aka.ms/mvvmtoolkit");
}

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
    public static readonly DiagnosticDescriptor MissingObservableValidatorInheritanceError = new DiagnosticDescriptor(
        id: "MVVMTK0006",
        title: "Missing ObservableValidator inheritance",
        messageFormat: "The field {0}.{1} cannot be used to generate an observable property, as it has {2} validation attribute(s) but is declared in a type that doesn't inherit from ObservableValidator",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot apply [ObservableProperty] to fields with validation attributes if they are declared in a type that doesn't inherit from ObservableValidator.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an annotated method to generate a command for has an invalid signature.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidICommandMethodSignatureError = new DiagnosticDescriptor(
        id: "MVVMTK0007",
        title: "Invalid ICommand method signature",
        messageFormat: "The method {0}.{1} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types",
        category: typeof(ICommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [ICommand] to methods with a signature that doesn't match any of the existing relay command types.",
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
    public static readonly DiagnosticDescriptor InvalidCanExecuteMemberName = new DiagnosticDescriptor(
        id: "MVVMTK0009",
        title: "Invalid ICommand.CanExecute member name",
        messageFormat: "The CanExecute name must refer to a valid member, but \"{0}\" has no matches in type {1}",
        category: typeof(ICommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The CanExecute name in [ICommand] must refer to a valid member in its parent type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a specified <c>CanExecute</c> name maps to multiple members.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a single member, but "{0}" has multiple matches in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MultipleCanExecuteMemberNameMatches = new DiagnosticDescriptor(
        id: "MVVMTK0010",
        title: "Multiple ICommand.CanExecute member name matches",
        messageFormat: "The CanExecute name must refer to a single member, but \"{0}\" has multiple matches in type {1}",
        category: typeof(ICommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot set the CanExecute name in [ICommand] to one that has multiple matches in its parent type (it must refer to a single compatible member).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a a specified <c>CanExecute</c> name maps to an invalid member.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a compatible member, but no valid members were found for "{0}" in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidCanExecuteMember = new DiagnosticDescriptor(
        id: "MVVMTK0011",
        title: "No valid ICommand.CanExecute member match",
        messageFormat: "The CanExecute name must refer to a compatible member, but no valid members were found for \"{0}\" in type {1}",
        category: typeof(ICommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The CanExecute name in [ICommand] must refer to a compatible member (either a property or a method) to be used in a generated command.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>ICommandAttribute.AllowConcurrentExecutions</c> is being set for a non-asynchronous method.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be annotated with the [ICommand] attribute specifying a concurrency control setting, as it maps to a non-asynchronous command type"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidConcurrentExecutionsParameterError = new DiagnosticDescriptor(
        id: "MVVMTK0012",
        title: "Invalid concurrency control setting usage",
        messageFormat: "The method {0}.{1} cannot be annotated with the [ICommand] attribute specifying a concurrency control setting, as it maps to a non-asynchronous command type",
        category: typeof(ICommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply the [ICommand] attribute specifying a concurrency control setting to methods mapping to non-asynchronous command types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>ICommandAttribute.IncludeCancelCommandParameter</c> is being set for an invalid method.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be annotated with the [ICommand] attribute specifying to include a cancel command, as it does not map to an asynchronous command type taking a cancellation token"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidIncludeCancelCommandParameterError = new DiagnosticDescriptor(
        id: "MVVMTK0013",
        title: "Invalid concurrency control setting usage",
        messageFormat: "The method {0}.{1} cannot be annotated with the [ICommand] attribute specifying to include a cancel command, as it does not map to an asynchronous command type taking a cancellation token",
        category: typeof(ICommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply the [ICommand] attribute specifying to include a cancel command to methods not mapping to an asynchronous command type accepting a cancellation token.",
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
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the specified target for <c>[AlsoNotifyChangeFor]</c> is not valid.
    /// <para>
    /// Format: <c>"The target(s) of [AlsoNotifyChangeFor] must be a (different) accessible property, but "{0}" has no (other) matches in type {1}</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor AlsoNotifyChangeForInvalidTargetError = new DiagnosticDescriptor(
        id: "MVVMTK0015",
        title: "Invalid target name for [AlsoNotifyChangeFor]",
        messageFormat: "The target(s) of [AlsoNotifyChangeFor] must be a (different) accessible property, but \"{0}\" has no (other) matches in type {1}",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The target(s) of [AlsoNotifyChangeFor] must be a (different) accessible property in its parent type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the specified target for <c>[AlsoNotifyCanExecuteFor]</c> is not valid.
    /// <para>
    /// Format: <c>"The target(s) of [AlsoNotifyCanExecuteFor] must be an accessible <c>IRelayCommand</c> property, but "{0}" has no matches in type {1}</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor AlsoNotifyCanExecuteForInvalidTargetError = new DiagnosticDescriptor(
        id: "MVVMTK0016",
        title: "Invalid target name for [AlsoNotifyCanExecuteFor]",
        messageFormat: "The target(s) of [AlsoNotifyCanExecuteFor] must be an accessible IRelayCommand property, but \"{0}\" has no matches in type {1}",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The target(s) of [AlsoNotifyCanExecuteFor] must be an accessible IRelayCommand property in its parent type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>[INotifyPropertyChanged]</c> is applied to a type with an attribute already.
    /// <para>
    /// Format: <c>"Cannot apply [INotifyPropertyChanged] to type {0}, as it already has this attribute or [ObservableObject] applied to it (including base types)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidAttributeCombinationForINotifyPropertyChangedAttributeError = new DiagnosticDescriptor(
        id: "MVVMTK0017",
        title: $"Invalid target type for [INotifyPropertyChanged]",
        messageFormat: $"Cannot apply [INotifyPropertyChanged] to type {{0}}, as it already has this attribute or [ObservableObject] applied to it (including base types)",
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
        title: $"Invalid target type for [ObservableObject]",
        messageFormat: $"Cannot apply [ObservableObject] to type {{0}}, as it already has this attribute or [INotifyPropertyChanged] applied to it (including base types)",
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
        title: $"Invalid containing type for [ObservableProperty] field",
        messageFormat: $"The field {{0}}.{{1}} cannot be used to generate an observable property, as its containing type doesn't inherit from ObservableObject, nor does it use [ObservableObject] or [INotifyPropertyChanged]",
        category: typeof(ObservablePropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Fields annotated with [ObservableProperty] must be contained in a type that inherits from ObservableObject or that is annotated with [ObservableObject] or [INotifyPropertyChanged] (including base types).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit");
}

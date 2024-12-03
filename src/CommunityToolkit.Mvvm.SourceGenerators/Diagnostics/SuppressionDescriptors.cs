// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

#pragma warning disable IDE0090 // Use 'new SuppressionDescriptor(...)'

namespace CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="SuppressionDescriptors"/> instances for suppressed diagnostics by analyzers in this project.
/// </summary>
internal static class SuppressionDescriptors
{
    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a field using [ObservableProperty] with an attribute list targeting a property.
    /// </summary>
    public static readonly SuppressionDescriptor PropertyAttributeListForObservablePropertyField = new SuppressionDescriptor(
        id: "MVVMTKSPR0001",
        suppressedDiagnosticId: "CS0657",
        justification: "Fields using [ObservableProperty] can use [property:], [set:] and [set:] attribute lists to forward attributes to the generated properties");

    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a field using [ObservableProperty] with an attribute list targeting a get or set accessor.
    /// </summary>
    public static readonly SuppressionDescriptor PropertyAttributeListForObservablePropertyFieldAccessors = new SuppressionDescriptor(
        id: "MVVMTKSPR0001",
        suppressedDiagnosticId: "CS0658",
        justification: "Fields using [ObservableProperty] can use [property:], [set:] and [set:] attribute lists to forward attributes to the generated properties");

    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a method using [RelayCommand] with an attribute list targeting a field or property.
    /// </summary>
    public static readonly SuppressionDescriptor FieldOrPropertyAttributeListForRelayCommandMethod = new SuppressionDescriptor(
        id: "MVVMTKSPR0002",
        suppressedDiagnosticId: "CS0657",
        justification: "Methods using [RelayCommand] can use [field:] and [property:] attribute lists to forward attributes to the generated fields and properties");
}

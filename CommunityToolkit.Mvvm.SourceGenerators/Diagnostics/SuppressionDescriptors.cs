// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="SuppressionDescriptors"/> instances for suppressed diagnostics by analyzers in this project.
/// </summary>
internal static class SuppressionDescriptors
{
    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a field using [ObservableProperty] with on attribute list targeting a property.
    /// </summary>
    public static readonly SuppressionDescriptor PropertyAttributeListForObservablePropertyField = new(
        id: "MVVMTKSPR0001",
        suppressedDiagnosticId: "CS0657",
        justification: "Fields using [ObservableProperty] can use [property:] attribute lists to forward attributes to the generated properties");
}

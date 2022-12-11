// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates a warning when a class is using a code generation attribute when it could inherit instead.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ClassUsingAttributeInsteadOfInheritanceAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The mapping of target attributes that will trigger the analyzer.
    /// </summary>
    private static readonly ImmutableDictionary<string, string> GeneratorAttributeNamesToFullyQualifiedNamesMap = ImmutableDictionary.CreateRange(new[]
    {
        new KeyValuePair<string, string>("ObservableObjectAttribute", "CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute"),
        new KeyValuePair<string, string>("INotifyPropertyChangedAttribute", "CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute"),
    });

    /// <summary>
    /// The mapping of diagnostics for each target attribute.
    /// </summary>
    private static readonly ImmutableDictionary<string, DiagnosticDescriptor> GeneratorAttributeNamesToDiagnosticsMap = ImmutableDictionary.CreateRange(new[]
    {
        new KeyValuePair<string, DiagnosticDescriptor>("ObservableObjectAttribute", InheritFromObservableObjectInsteadOfUsingObservableObjectAttributeWarning),
        new KeyValuePair<string, DiagnosticDescriptor>("INotifyPropertyChangedAttribute", InheritFromObservableObjectInsteadOfUsingINotifyPropertyChangedAttributeWarning),
    });

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        InheritFromObservableObjectInsteadOfUsingObservableObjectAttributeWarning,
        InheritFromObservableObjectInsteadOfUsingINotifyPropertyChangedAttributeWarning);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(static context =>
        {
            // We're looking for class declarations
            if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsRecord: false, IsStatic: false, IsImplicitlyDeclared: false } classSymbol)
            {
                return;
            }

            foreach (AttributeData attribute in context.Symbol.GetAttributes())
            {
                // Same logic as in FieldWithOrphanedDependentObservablePropertyAttributesAnalyzer to find target attributes
                if (attribute.AttributeClass is { Name: string attributeName } attributeClass &&
                    GeneratorAttributeNamesToFullyQualifiedNamesMap.TryGetValue(attributeName, out string? fullyQualifiedAttributeName) &&
                    context.Compilation.GetTypeByMetadataName(fullyQualifiedAttributeName) is INamedTypeSymbol attributeSymbol &&
                    SymbolEqualityComparer.Default.Equals(attributeClass, attributeSymbol))
                {
                    // The type is annotated with either [ObservableObject] or [INotifyPropertyChanged].
                    // Next, we need to check whether it isn't already inheriting from another type.
                    if (classSymbol.BaseType is { SpecialType: SpecialType.System_Object })
                    {
                        // This type is using the attribute when it could just inherit from ObservableObject, which is preferred
                        context.ReportDiagnostic(Diagnostic.Create(GeneratorAttributeNamesToDiagnosticsMap[attributeClass.Name], context.Symbol.Locations.FirstOrDefault(), context.Symbol));
                    }
                }
            }
        }, SymbolKind.NamedType);
    }
}

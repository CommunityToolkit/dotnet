// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
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
    /// The key for the name of the target type to update.
    /// </summary>
    internal const string TypeNameKey = "TypeName";

    /// <summary>
    /// The key for the name of the attribute that was found and should be removed.
    /// </summary>
    internal const string AttributeTypeNameKey = "AttributeTypeName";

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
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Try to get all necessary type symbols
            if (!context.Compilation.TryBuildNamedTypeSymbolMap(GeneratorAttributeNamesToFullyQualifiedNamesMap, out ImmutableDictionary<string, INamedTypeSymbol>? typeSymbols))
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // We're looking for class declarations that don't have any base type
                if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsRecord: false, IsStatic: false, IsImplicitlyDeclared: false, BaseType.SpecialType: SpecialType.System_Object } classSymbol)
                {
                    return;
                }

                foreach (AttributeData attribute in context.Symbol.GetAttributes())
                {
                    // Same logic as in FieldWithOrphanedDependentObservablePropertyAttributesAnalyzer to find target attributes
                    if (attribute.AttributeClass is { Name: string attributeName } attributeClass &&
                        typeSymbols.TryGetValue(attributeName, out INamedTypeSymbol? attributeSymbol) &&
                        SymbolEqualityComparer.Default.Equals(attributeClass, attributeSymbol))
                    {
                        // The type is annotated with either [ObservableObject] or [INotifyPropertyChanged], and we already validated
                        // that it has no other base type, so emit a diagnostic to suggest inheriting from ObservableObject instead.
                        context.ReportDiagnostic(Diagnostic.Create(
                            GeneratorAttributeNamesToDiagnosticsMap[attributeClass.Name],
                            context.Symbol.Locations.FirstOrDefault(),
                            ImmutableDictionary.Create<string, string?>()
                                .Add(TypeNameKey, classSymbol.Name)
                                .Add(AttributeTypeNameKey, attributeName),
                            context.Symbol));
                    }
                }
            }, SymbolKind.NamedType);
        });
    }
}

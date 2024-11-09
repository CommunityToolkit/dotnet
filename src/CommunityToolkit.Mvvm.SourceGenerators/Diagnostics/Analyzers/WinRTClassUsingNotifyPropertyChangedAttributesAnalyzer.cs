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
/// A diagnostic analyzer that generates a warning when <c>[ObservableObject]</c> and <c>[INotifyPropertyChanged]</c> are used on a class in WinRT scenarios.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WinRTClassUsingNotifyPropertyChangedAttributesAnalyzer : DiagnosticAnalyzer
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
        new KeyValuePair<string, DiagnosticDescriptor>("ObservableObjectAttribute", WinRTUsingObservableObjectAttribute),
        new KeyValuePair<string, DiagnosticDescriptor>("INotifyPropertyChangedAttribute", WinRTUsingINotifyPropertyChangedAttribute),
    });

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        WinRTUsingObservableObjectAttribute,
        WinRTUsingINotifyPropertyChangedAttribute);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // This analyzer is only enabled when CsWinRT is in AOT mode
            if (!context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.IsCsWinRTAotOptimizerEnabled(context.Compilation))
            {
                return;
            }

            // Try to get all necessary type symbols
            if (!context.Compilation.TryBuildNamedTypeSymbolMap(GeneratorAttributeNamesToFullyQualifiedNamesMap, out ImmutableDictionary<string, INamedTypeSymbol>? typeSymbols))
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // We're looking for class declarations that don't have any base type (same as the other analyzer for non-WinRT scenarios), but inverted for base types.
                // That is, we only want to warn in cases where the other analyzer would not warn. Otherwise, warnings from that one are already more than sufficient.
                if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsRecord: false, IsStatic: false, IsImplicitlyDeclared: false, BaseType.SpecialType: not SpecialType.System_Object } classSymbol)
                {
                    return;
                }

                foreach (AttributeData attribute in context.Symbol.GetAttributes())
                {
                    // Warn if either attribute is used, as it's not compatible with AOT
                    if (attribute.AttributeClass is { Name: string attributeName } attributeClass &&
                        typeSymbols.TryGetValue(attributeName, out INamedTypeSymbol? attributeSymbol) &&
                        SymbolEqualityComparer.Default.Equals(attributeClass, attributeSymbol))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            GeneratorAttributeNamesToDiagnosticsMap[attributeClass.Name],
                            context.Symbol.Locations.FirstOrDefault(),
                            context.Symbol));
                    }
                }
            }, SymbolKind.NamedType);
        });
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error whenever a field has an orphaned attribute that depends on <c>[ObservableProperty]</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FieldWithOrphanedDependentObservablePropertyAttributesAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The mapping of target attributes that will trigger the analyzer.
    /// </summary>
    private static readonly ImmutableDictionary<string, string> GeneratorAttributeNamesToFullyQualifiedNamesMap = ImmutableDictionary.CreateRange(new[]
    {
        new KeyValuePair<string, string>("NotifyCanExecuteChangedForAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedForAttribute"),
        new KeyValuePair<string, string>("NotifyDataErrorInfoAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyDataErrorInfoAttribute"),
        new KeyValuePair<string, string>("NotifyPropertyChangedForAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedForAttribute"),
        new KeyValuePair<string, string>("NotifyPropertyChangedRecipientsAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedRecipientsAttribute")
    });

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(FieldWithOrphanedDependentObservablePropertyAttributesError);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Defer the registration so it can be skipped if C# 8.0 or more is not available.
        // That is because in that case source generators are not supported at all anyway.
        context.RegisterCompilationStartAction(static context =>
        {
            if (!context.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
            {
                return;
            }

            // Try to get all necessary type symbols to map
            if (!context.Compilation.TryBuildNamedTypeSymbolMap(GeneratorAttributeNamesToFullyQualifiedNamesMap, out ImmutableDictionary<string, INamedTypeSymbol>? typeSymbols))
            {
                return;
            }

            // We also need the symbol for [ObservableProperty], separately
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                ImmutableArray<AttributeData> attributes = context.Symbol.GetAttributes();

                // If the symbol has no attributes, there's nothing left to do
                if (attributes.IsEmpty)
                {
                    return;
                }

                foreach (AttributeData dependentAttribute in attributes)
                {
                    // Go over each attribute on the target symbol, and check if any of them matches one of the trigger attributes.
                    // The logic here is the same as the one in UnsupportedCSharpLanguageVersionAnalyzer.
                    if (dependentAttribute.AttributeClass is { Name: string attributeName } dependentAttributeClass &&
                        typeSymbols.TryGetValue(attributeName, out INamedTypeSymbol? dependentAttributeSymbol) &&
                        SymbolEqualityComparer.Default.Equals(dependentAttributeClass, dependentAttributeSymbol))
                    {
                        // If the attribute matches, iterate over the attributes to try to find [ObservableProperty]
                        foreach (AttributeData attribute in attributes)
                        {
                            if (attribute.AttributeClass is { Name: "ObservablePropertyAttribute" } attributeSymbol &&
                                SymbolEqualityComparer.Default.Equals(attributeSymbol, observablePropertySymbol))
                            {
                                // If [ObservableProperty] is found, then this field is valid in that it doesn't have orphaned dependent attributes
                                return;
                            }
                        }

                        context.ReportDiagnostic(Diagnostic.Create(
                            FieldWithOrphanedDependentObservablePropertyAttributesError,
                            context.Symbol.Locations.FirstOrDefault(),
                            context.Symbol.Kind.ToFieldOrPropertyKeyword(),
                            context.Symbol.ContainingType,
                            context.Symbol.Name));

                        // Just like in UnsupportedCSharpLanguageVersionAnalyzer, stop if a diagnostic has been emitted for the current symbol
                        return;
                    }
                }
            }, SymbolKind.Field, SymbolKind.Property);
        });
    }
}

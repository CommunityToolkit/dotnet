// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !ROSLYN_4_12_0_OR_GREATER

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error whenever <c>[ObservableProperty]</c> is used on a property, if the Roslyn version in use is not high enough.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnsupportedRoslynVersionForPartialPropertyAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(UnsupportedRoslynVersionForObservablePartialPropertySupport);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the symbol for [ObservableProperty]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // We're intentionally only looking for properties here
                if (context.Symbol is not IPropertySymbol propertySymbol)
                {
                    return;
                }

                // If the property has [ObservableProperty], emit an error in all cases
                if (propertySymbol.HasAttributeWithType(observablePropertySymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        UnsupportedRoslynVersionForObservablePartialPropertySupport,
                        propertySymbol.Locations.FirstOrDefault(),
                        propertySymbol.ContainingType,
                        propertySymbol));
                }
            }, SymbolKind.Property);
        });
    }
}

#endif

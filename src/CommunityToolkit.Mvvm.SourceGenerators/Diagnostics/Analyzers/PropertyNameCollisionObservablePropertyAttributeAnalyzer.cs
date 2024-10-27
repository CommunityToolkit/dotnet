// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error when a generated property from <c>[ObservableProperty]</c> would collide with the field name.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PropertyNameCollisionObservablePropertyAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(ObservablePropertyNameCollisionError);

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
                // Ensure we do have a valid field
                if (context.Symbol is not IFieldSymbol fieldSymbol)
                {
                    return;
                }

                // We only care if the field has [ObservableProperty]
                if (!fieldSymbol.HasAttributeWithType(observablePropertySymbol))
                {
                    return;
                }

                // Emit the diagnostic if there is a name collision
                if (fieldSymbol.Name == ObservablePropertyGenerator.Execute.GetGeneratedPropertyName(fieldSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        ObservablePropertyNameCollisionError,
                        fieldSymbol.Locations.FirstOrDefault(),
                        fieldSymbol.ContainingType,
                        fieldSymbol.Name));
                }
            }, SymbolKind.Field);
        });
    }
}

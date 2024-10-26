// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error when an auto-property is using <c>[field: ObservableProperty]</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AutoPropertyWithFieldTargetedObservablePropertyAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(AutoPropertyBackingFieldObservableProperty);

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
                // Get the property symbol and the type symbol for the containing type
                if (context.Symbol is not IPropertySymbol { ContainingType: INamedTypeSymbol typeSymbol } propertySymbol)
                {
                    return;
                }

                foreach (ISymbol memberSymbol in typeSymbol.GetMembers())
                {
                    // We're only looking for fields with an associated property
                    if (memberSymbol is not IFieldSymbol { AssociatedSymbol: IPropertySymbol associatedPropertySymbol })
                    {
                        continue;
                    }

                    // Check that this field is in fact the backing field for the target auto-property
                    if (!SymbolEqualityComparer.Default.Equals(associatedPropertySymbol, propertySymbol))
                    {
                        continue;
                    }

                    // If the field isn't using [ObservableProperty], this analyzer isn't applicable
                    if (!memberSymbol.TryGetAttributeWithType(observablePropertySymbol, out AttributeData? attributeData))
                    {
                        return;
                    }

                    // Report the diagnostic on the attribute location
                    context.ReportDiagnostic(Diagnostic.Create(
                        AutoPropertyBackingFieldObservableProperty,
                        attributeData.GetLocation(),
                        typeSymbol,
                        propertySymbol));
                }
            }, SymbolKind.Property);
        });
    }
}

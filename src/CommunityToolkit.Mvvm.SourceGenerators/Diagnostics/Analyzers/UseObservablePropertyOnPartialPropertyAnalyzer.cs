// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_11_0_OR_GREATER

using System.Collections.Immutable;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates a suggestion whenever <c>[ObservableProperty]</c> is used on a field when a partial property could be used instead.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseObservablePropertyOnPartialPropertyAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(UseObservablePropertyOnPartialProperty);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Using [ObservableProperty] on partial properties is only supported when using C# preview.
            // As such, if that is not the case, return immediately, as no diagnostic should be produced.
            if (!context.Compilation.IsLanguageVersionPreview())
            {
                return;
            }

            // Get the symbol for [ObservableProperty]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // We're intentionally only looking for fields here
                if (context.Symbol is not IFieldSymbol fieldSymbol)
                {
                    return;
                }

                // Check that we are in fact using [ObservableProperty]
                if (!fieldSymbol.TryGetAttributeWithType(observablePropertySymbol, out AttributeData? observablePropertyAttribute))
                {
                    return;
                }

                // Emit the diagnostic for this field to suggest changing to a partial property instead
                context.ReportDiagnostic(Diagnostic.Create(
                    UseObservablePropertyOnPartialProperty,
                    observablePropertyAttribute.GetLocation(),
                    ImmutableDictionary.Create<string, string?>()
                        .Add(FieldReferenceForObservablePropertyFieldAnalyzer.FieldNameKey, fieldSymbol.Name)
                        .Add(FieldReferenceForObservablePropertyFieldAnalyzer.PropertyNameKey, ObservablePropertyGenerator.Execute.GetGeneratedPropertyName(fieldSymbol)),
                    fieldSymbol.ContainingType,
                    fieldSymbol));
            }, SymbolKind.Field);
        });
    }
}

#endif

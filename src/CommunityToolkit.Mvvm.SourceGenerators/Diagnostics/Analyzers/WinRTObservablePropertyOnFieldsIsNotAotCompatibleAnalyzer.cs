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
/// A diagnostic analyzer that generates an error when <c>[ObservableProperty]</c> is used on a field in a scenario where it wouldn't be AOT compatible.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(WinRTObservablePropertyOnFieldsIsNotAotCompatible);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // This analyzer is only enabled in cases where CsWinRT is producing AOT-compatible code
            if (!context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.IsCsWinRTAotOptimizerEnabled(context.Compilation))
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
                // Ensure we do have a valid field
                if (context.Symbol is not IFieldSymbol fieldSymbol)
                {
                    return;
                }

                // Emit a diagnostic if the field is using the [ObservableProperty] attribute
                if (fieldSymbol.TryGetAttributeWithType(observablePropertySymbol, out AttributeData? observablePropertyAttribute))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        WinRTObservablePropertyOnFieldsIsNotAotCompatible,
                        observablePropertyAttribute.GetLocation(),
                        ImmutableDictionary.Create<string, string?>()
                            .Add(FieldReferenceForObservablePropertyFieldAnalyzer.FieldNameKey, fieldSymbol.Name)
                            .Add(FieldReferenceForObservablePropertyFieldAnalyzer.PropertyNameKey, ObservablePropertyGenerator.Execute.GetGeneratedPropertyName(fieldSymbol)),
                        fieldSymbol.ContainingType,
                        fieldSymbol.Name));
                }
            }, SymbolKind.Field);
        });
    }
}

#endif

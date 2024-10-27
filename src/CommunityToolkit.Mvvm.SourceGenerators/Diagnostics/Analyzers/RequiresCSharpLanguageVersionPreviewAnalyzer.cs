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
/// A diagnostic analyzer that generates errors when a property using <c>[ObservableProperty]</c> on a partial property is in a project with the C# language version not set to preview.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RequiresCSharpLanguageVersionPreviewAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [CSharpLanguageVersionIsNotPreviewForObservableProperty];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // If the language version is set to preview, we'll never emit diagnostics
            if (context.Compilation.IsLanguageVersionPreview())
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
                // We only want to target partial property definitions (also include non-partial ones for diagnostics)
                if (context.Symbol is not IPropertySymbol { PartialDefinitionPart: null })
                {
                    return;
                }

                // If the property is using [ObservableProperty], emit the diagnostic
                if (context.Symbol.TryGetAttributeWithType(observablePropertySymbol, out AttributeData? observablePropertyAttribute))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        CSharpLanguageVersionIsNotPreviewForObservableProperty,
                        observablePropertyAttribute.GetLocation(),
                        context.Symbol));
                }
            }, SymbolKind.Property);
        });
    }
}

#endif
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_11_0_OR_GREATER

using System.Collections.Immutable;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                if (context.Symbol is not IPropertySymbol { PartialDefinitionPart: null } partialProperty)
                {
                    return;
                }

                // Make sure to skip the warning if the property is not actually partial
                if (partialProperty.DeclaringSyntaxReferences is [var syntaxReference])
                {
                    // Make sure we can find the syntax node, and that it's a property declaration
                    if (syntaxReference.GetSyntax(context.CancellationToken) is PropertyDeclarationSyntax propertyDeclarationSyntax)
                    {
                        // If the property is not partial, ignore it, as we'll already have a warning from the other analyzer here
                        if (!propertyDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
                        {
                            return;
                        }
                    }
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
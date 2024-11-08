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
/// A diagnostic analyzer that generates an error when <c>[RelayCommand]</c> is used on a method inside a type with <c>[GeneratedBindableCustomProperty]</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatibleAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatible);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // This analyzer is only enabled when CsWinRT is also used
            if (!context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.IsUsingWindowsRuntimePack())
            {
                return;
            }

            // Get the symbols for [RelayCommand] and [GeneratedBindableCustomProperty]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.Input.RelayCommandAttribute") is not INamedTypeSymbol relayCommandSymbol ||
                context.Compilation.GetTypeByMetadataName("WinRT.GeneratedBindableCustomPropertyAttribute") is not INamedTypeSymbol generatedBindableCustomPropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // Ensure we do have a valid method with a containing type we can reference
                if (context.Symbol is not IMethodSymbol { ContainingType: INamedTypeSymbol typeSymbol } methodSymbol)
                {
                    return;
                }

                // If the method is not using [RelayCommand], we can skip it
                if (!methodSymbol.TryGetAttributeWithType(relayCommandSymbol, out AttributeData? relayCommandAttribute))
                {
                    return;
                }

                // If the containing type is using [GeneratedBindableCustomProperty], emit a warning
                if (typeSymbol.HasAttributeWithType(generatedBindableCustomPropertySymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatible,
                        relayCommandAttribute.GetLocation(),
                        methodSymbol));
                }
            }, SymbolKind.Method);
        });
    }
}

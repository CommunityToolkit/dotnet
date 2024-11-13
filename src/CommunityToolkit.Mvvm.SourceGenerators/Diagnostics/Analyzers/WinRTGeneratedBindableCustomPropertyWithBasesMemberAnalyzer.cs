// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_11_0_OR_GREATER

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error when <c>[GeneratedBindableCustomProperty]</c> is used on types with invalid generated base members.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        WinRTGeneratedBindableCustomPropertyWithBaseObservablePropertyOnField,
        WinRTGeneratedBindableCustomPropertyWithBaseRelayCommand);

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

            // Get the symbols for [ObservableProperty], [RelayCommand] and [GeneratedBindableCustomProperty]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol ||
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.Input.RelayCommandAttribute") is not INamedTypeSymbol relayCommandSymbol ||
                context.Compilation.GetTypeByMetadataName("WinRT.GeneratedBindableCustomPropertyAttribute") is not INamedTypeSymbol generatedBindableCustomPropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // Ensure we do have a valid type
                if (context.Symbol is not INamedTypeSymbol typeSymbol)
                {
                    return;
                }

                // We only care about it if it's using [GeneratedBindableCustomProperty]
                if (!typeSymbol.HasAttributeWithType(generatedBindableCustomPropertySymbol))
                {
                    return;
                }

                // Warn on all [ObservableProperty] fields
                foreach (IFieldSymbol fieldSymbol in FindObservablePropertyFields(typeSymbol, observablePropertySymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        WinRTGeneratedBindableCustomPropertyWithBaseObservablePropertyOnField,
                        typeSymbol.Locations.FirstOrDefault(),
                        typeSymbol,
                        fieldSymbol.ContainingType,
                        fieldSymbol.Name));
                }

                // Warn on all [RelayCommand] methods
                foreach (IMethodSymbol methodSymbol in FindRelayCommandMethods(typeSymbol, relayCommandSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        WinRTGeneratedBindableCustomPropertyWithBaseRelayCommand,
                        typeSymbol.Locations.FirstOrDefault(),
                        typeSymbol,
                        methodSymbol));
                }
            }, SymbolKind.NamedType);
        });
    }

    /// <summary>
    /// Finds all methods in the base types that have the <c>[RelayCommand]</c> attribute.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="INamedTypeSymbol"/> instance to inspect.</param>
    /// <param name="relayCommandSymbol">The symbol for the <c>[RelayCommand]</c></param>
    /// <returns>All <see cref="IMethodSymbol"/> instances for matching members.</returns>
    private static IEnumerable<IMethodSymbol> FindRelayCommandMethods(INamedTypeSymbol typeSymbol, INamedTypeSymbol relayCommandSymbol)
    {
        // Check whether the base type (if any) is from the same assembly, and stop if it isn't. We do not
        // want to include methods from the same type, as those will already be caught by another analyzer.
        if (!SymbolEqualityComparer.Default.Equals(typeSymbol.ContainingAssembly, typeSymbol.BaseType?.ContainingAssembly))
        {
            yield break;
        }

        foreach (ISymbol memberSymbol in typeSymbol.BaseType.GetAllMembersFromSameAssembly())
        {
            if (memberSymbol is IMethodSymbol methodSymbol &&
                methodSymbol.HasAttributeWithType(relayCommandSymbol))
            {
                yield return methodSymbol;
            }
        }
    }

    /// <summary>
    /// Finds all fields in the base types that have the <c>[ObservableProperty]</c> attribute.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="INamedTypeSymbol"/> instance to inspect.</param>
    /// <param name="observablePropertySymbol">The symbol for the <c>[ObservableProperty]</c></param>
    /// <returns>All <see cref="IFieldSymbol"/> instances for matching members.</returns>
    private static IEnumerable<IFieldSymbol> FindObservablePropertyFields(INamedTypeSymbol typeSymbol, INamedTypeSymbol observablePropertySymbol)
    {
        // Skip the base type if not from the same assembly, same as above
        if (!SymbolEqualityComparer.Default.Equals(typeSymbol.ContainingAssembly, typeSymbol.BaseType?.ContainingAssembly))
        {
            yield break;
        }

        foreach (ISymbol memberSymbol in typeSymbol.BaseType.GetAllMembersFromSameAssembly())
        {
            if (memberSymbol is IFieldSymbol fieldSymbol &&
                fieldSymbol.HasAttributeWithType(observablePropertySymbol))
            {
                yield return fieldSymbol;
            }
        }
    }
}

#endif

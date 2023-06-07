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
/// A diagnostic analyzer that generates a warning when using <c>[RelayCommand]</c> over an <see langword="async"/> <see cref="void"/> method.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncVoidReturningRelayCommandMethodAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(AsyncVoidReturningRelayCommandMethod);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the symbol for [RelayCommand]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.Input.RelayCommandAttribute") is not INamedTypeSymbol relayCommandSymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // We're only looking for async void methods
                if (context.Symbol is not IMethodSymbol { IsAsync: true, ReturnsVoid: true } methodSymbol)
                {
                    return;
                }

                // We only care about methods annotated with [RelayCommand]
                if (!methodSymbol.HasAttributeWithType(relayCommandSymbol))
                {
                    return;
                }

                // Warn on async void methods using [RelayCommand] (they should return a Task instead)
                context.ReportDiagnostic(Diagnostic.Create(
                    AsyncVoidReturningRelayCommandMethod,
                    context.Symbol.Locations.FirstOrDefault(),
                    context.Symbol));
            }, SymbolKind.Method);
        });
    }
}

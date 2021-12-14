// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file is ported and adapted from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;

/// <summary>
/// Extension methods for <see cref="GeneratorExecutionContext"/>, specifically for reporting diagnostics.
/// </summary>
internal static class DiagnosticExtensions
{
    /// <summary>
    /// Adds a new diagnostics to the current compilation.
    /// </summary>
    /// <param name="context">The <see cref="GeneratorExecutionContext"/> instance currently in use.</param>
    /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
    /// <param name="symbol">The source <see cref="ISymbol"/> to attach the diagnostics to.</param>
    /// <param name="args">The optional arguments for the formatted message to include.</param>
    public static void ReportDiagnostic(
        this GeneratorExecutionContext context,
        DiagnosticDescriptor descriptor,
        ISymbol symbol,
        params object[] args)
    {
        context.ReportDiagnostic(Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args));
    }

    /// <summary>
    /// Adds a new diagnostics to the current compilation.
    /// </summary>
    /// <param name="context">The <see cref="GeneratorExecutionContext"/> instance currently in use.</param>
    /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
    /// <param name="node">The source <see cref="SyntaxNode"/> to attach the diagnostics to.</param>
    /// <param name="args">The optional arguments for the formatted message to include.</param>
    public static void ReportDiagnostic(
        this GeneratorExecutionContext context,
        DiagnosticDescriptor descriptor,
        SyntaxNode node,
        params object[] args)
    {
        context.ReportDiagnostic(Diagnostic.Create(descriptor, node.GetLocation(), args));
    }

    /// <summary>
    /// Adds a new diagnostics to the target builder.
    /// </summary>
    /// <param name="diagnostics">The collection of produced <see cref="Diagnostic"/> instances.</param>
    /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
    /// <param name="symbol">The source <see cref="ISymbol"/> to attach the diagnostics to.</param>
    /// <param name="args">The optional arguments for the formatted message to include.</param>
    public static void Add(
        this ImmutableArray<Diagnostic>.Builder diagnostics,
        DiagnosticDescriptor descriptor,
        ISymbol symbol,
        params object[] args)
    {
        diagnostics.Add(Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args));
    }

    /// <summary>
    /// Adds a new diagnostics to the target builder.
    /// </summary>
    /// <param name="diagnostics">The collection of produced <see cref="Diagnostic"/> instances.</param>
    /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
    /// <param name="node">The source <see cref="SyntaxNode"/> to attach the diagnostics to.</param>
    /// <param name="args">The optional arguments for the formatted message to include.</param>
    public static void Add(
        this ImmutableArray<Diagnostic>.Builder diagnostics,
        DiagnosticDescriptor descriptor,
        SyntaxNode node,
        params object[] args)
    {
        diagnostics.Add(Diagnostic.Create(descriptor, node.GetLocation(), args));
    }

    /// <summary>
    /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
    /// <param name="diagnostics">The input <see cref="IncrementalValuesProvider{TValues}"/> sequence of diagnostics.</param>
    public static void ReportDiagnostics(this IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ImmutableArray<Diagnostic>> diagnostics)
    {
        context.RegisterSourceOutput(diagnostics, static (context, diagnostics) =>
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        });
    }

    /// <summary>
    /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
    /// <param name="isSupported">An <see cref="IncrementalValueProvider{TValue}"/> instance indicating support for a feature.</param>
    /// <param name="diagnostic">The <see cref="Diagnostic"/> to emit if <paramref name="isSupported"/> fails.</param>
    public static void ReportDiagnosticsIsNotSupported(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<bool> isSupported,
        Diagnostic diagnostic)
    {
        context.RegisterSourceOutput(isSupported, (context, isSupported) =>
        {
            if (!isSupported)
            {
                context.ReportDiagnostic(diagnostic);
            }
        });
    }
}

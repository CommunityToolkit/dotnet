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
/// Extension methods specifically for creating diagnostics.
/// </summary>
internal static class DiagnosticExtensions
{
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
        diagnostics.Add(descriptor.CreateDiagnostic(symbol, args));
    }

    /// <summary>
    /// Creates a new <see cref="Diagnostic"/> instance with the specified parameters.
    /// </summary>
    /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
    /// <param name="symbol">The source <see cref="ISymbol"/> to attach the diagnostics to.</param>
    /// <param name="args">The optional arguments for the formatted message to include.</param>
    /// <returns>The resulting <see cref="Diagnostic"/> instance.</returns>
    public static Diagnostic CreateDiagnostic(this DiagnosticDescriptor descriptor, ISymbol symbol, params object[] args)
    {
        return Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args);
    }
}

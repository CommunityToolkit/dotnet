// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// <inheritdoc cref="GeneratorAttributeSyntaxContext" path="/summary/node()"/>
/// </summary>
/// <param name="syntaxContext">The original <see cref="GeneratorAttributeSyntaxContext"/> value.</param>
/// <param name="globalOptions">The original <see cref="AnalyzerConfigOptions"/> value.</param>
internal readonly struct GeneratorAttributeSyntaxContextWithOptions(
    GeneratorAttributeSyntaxContext syntaxContext,
    AnalyzerConfigOptions globalOptions)
{
    /// <inheritdoc cref="GeneratorAttributeSyntaxContext.TargetNode"/>
    public SyntaxNode TargetNode { get; } = syntaxContext.TargetNode;

    /// <inheritdoc cref="GeneratorAttributeSyntaxContext.TargetSymbol"/>
    public ISymbol TargetSymbol { get; } = syntaxContext.TargetSymbol;

    /// <inheritdoc cref="GeneratorAttributeSyntaxContext.SemanticModel"/>
    public SemanticModel SemanticModel { get; } = syntaxContext.SemanticModel;

    /// <inheritdoc cref="GeneratorAttributeSyntaxContext.Attributes"/>
    public ImmutableArray<AttributeData> Attributes { get; } = syntaxContext.Attributes;

    /// <inheritdoc cref="AnalyzerConfigOptionsProvider.GlobalOptions"/>
    public AnalyzerConfigOptions GlobalOptions { get; } = globalOptions;
}

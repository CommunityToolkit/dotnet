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
/// A diagnostic analyzer that generates an error when a class level <c>[NotifyDataErrorInfo]</c> use is detected.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidClassLevelNotifyDataErrorInfoAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(InvalidTypeForNotifyDataErrorInfoError);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(static context =>
        {
            // We're looking for all class declarations
            if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsImplicitlyDeclared: false } classSymbol)
            {
                return;
            }

            // Only inspect classes that are using [NotifyDataErrorInfo]
            if (!classSymbol.HasAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyDataErrorInfoAttribute"))
            {
                return;
            }

            // If the containing type is not valid, emit a diagnostic
            if (!classSymbol.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator"))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    InvalidTypeForNotifyDataErrorInfoError,
                    classSymbol.Locations.FirstOrDefault(),
                    classSymbol));
            }
        }, SymbolKind.NamedType);
    }
}

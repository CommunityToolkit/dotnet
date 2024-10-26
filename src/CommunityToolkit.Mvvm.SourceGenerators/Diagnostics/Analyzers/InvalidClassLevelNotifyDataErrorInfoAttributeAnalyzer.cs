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
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the symbols for [NotifyDataErrorInfo] and ObservableValidator
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyDataErrorInfoAttribute") is not INamedTypeSymbol notifyDataErrorInfoAttributeSymbol ||
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator") is not INamedTypeSymbol observableValidatorSymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // We're looking for all class declarations
                if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsImplicitlyDeclared: false } classSymbol)
                {
                    return;
                }

                // Emit a diagnostic for types that use [NotifyDataErrorInfo] but don't inherit from ObservableValidator
                if (classSymbol.HasAttributeWithType(notifyDataErrorInfoAttributeSymbol) &&
                    !classSymbol.InheritsFromType(observableValidatorSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        InvalidTypeForNotifyDataErrorInfoError,
                        classSymbol.Locations.FirstOrDefault(),
                        classSymbol));
                }
            }, SymbolKind.NamedType);
        });
    }
}

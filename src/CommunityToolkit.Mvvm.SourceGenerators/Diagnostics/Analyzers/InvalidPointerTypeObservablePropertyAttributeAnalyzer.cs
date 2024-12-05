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
/// A diagnostic analyzer that generates an error whenever <c>[ObservableProperty]</c> is used with pointer types.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPointerTypeObservablePropertyAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(InvalidObservablePropertyDeclarationReturnsPointerLikeType);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the [ObservableProperty] symbol
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // Ensure that we have a valid target symbol to analyze
                if (context.Symbol is not (IFieldSymbol or IPropertySymbol))
                {
                    return;
                }

                // If the property is not using [ObservableProperty], there's nothing to do
                if (!context.Symbol.TryGetAttributeWithType(observablePropertySymbol, out AttributeData? observablePropertyAttribute))
                {
                    return;
                }

                // Emit a diagnostic if the type is a pointer type
                if (context.Symbol is
                    IPropertySymbol { Type.TypeKind: TypeKind.Pointer or TypeKind.FunctionPointer } or
                    IFieldSymbol { Type.TypeKind: TypeKind.Pointer or TypeKind.FunctionPointer })
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        InvalidObservablePropertyDeclarationReturnsPointerLikeType,
                        observablePropertyAttribute.GetLocation(),
                        context.Symbol.ContainingType,
                        context.Symbol.Name));
                }
            }, SymbolKind.Field, SymbolKind.Property);
        });
    }
}

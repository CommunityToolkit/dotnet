// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_12_0_OR_GREATER

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates a suggestion whenever <c>[ObservableProperty]</c> is used on a field when a partial property could be used instead.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseObservablePropertyOnPartialPropertyAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(UseObservablePropertyOnPartialProperty);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Using [ObservableProperty] on partial properties is only supported when using C# preview.
            // As such, if that is not the case, return immediately, as no diagnostic should be produced.
            if (!context.Compilation.IsLanguageVersionPreview())
            {
                return;
            }

            // If CsWinRT is in AOT-optimization mode, disable this analyzer, as the WinRT one will produce a warning instead
            if (context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.IsCsWinRTAotOptimizerEnabled(context.Compilation))
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
                // We're intentionally only looking for fields here
                if (context.Symbol is not IFieldSymbol fieldSymbol)
                {
                    return;
                }

                // Check that we are in fact using [ObservableProperty]
                if (!fieldSymbol.HasAttributeWithType(observablePropertySymbol))
                {
                    return;
                }

                // It's not really meant to be used this way, but technically speaking the generator also supports
                // static fields. So for those users leveraging that (for whatever reason), make sure to skip those.
                // Partial properties using [ObservableProperty] cannot be static, and we never want the code fixer
                // to prompt the user, run, and then result in code that will fail to compile.
                if (fieldSymbol.IsStatic)
                {
                    return;
                }

                // Emit the diagnostic for this field to suggest changing to a partial property instead
                context.ReportDiagnostic(Diagnostic.Create(
                    UseObservablePropertyOnPartialProperty,
                    fieldSymbol.Locations.FirstOrDefault(),
                    ImmutableDictionary.Create<string, string?>()
                        .Add(FieldReferenceForObservablePropertyFieldAnalyzer.FieldNameKey, fieldSymbol.Name)
                        .Add(FieldReferenceForObservablePropertyFieldAnalyzer.PropertyNameKey, ObservablePropertyGenerator.Execute.GetGeneratedPropertyName(fieldSymbol)),
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name));
            }, SymbolKind.Field);
        });
    }
}

#endif

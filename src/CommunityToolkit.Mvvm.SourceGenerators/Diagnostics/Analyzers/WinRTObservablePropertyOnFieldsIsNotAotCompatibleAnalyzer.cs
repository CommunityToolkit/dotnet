// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_12_0_OR_GREATER

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error when <c>[ObservableProperty]</c> is used on a field in a scenario where it wouldn't be AOT compatible.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        WinRTObservablePropertyOnFieldsIsNotAotCompatible,
        WinRTObservablePropertyOnFieldsIsNotAotCompatibleCompilationEndInfo);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // This analyzer is only enabled in cases where CsWinRT is producing AOT-compatible code
            if (!context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.IsCsWinRTAotOptimizerEnabled(context.Compilation))
            {
                return;
            }

            // Get the symbol for [ObservableProperty]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol)
            {
                return;
            }

            // Track whether we produced any diagnostics, for the compilation end scenario
            AttributeData? firstObservablePropertyAttribute = null;

            context.RegisterSymbolAction(context =>
            {
                // Ensure we do have a valid field
                if (context.Symbol is not IFieldSymbol fieldSymbol)
                {
                    return;
                }

                // Emit a diagnostic if the field is using the [ObservableProperty] attribute
                if (fieldSymbol.TryGetAttributeWithType(observablePropertySymbol, out AttributeData? observablePropertyAttribute))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        WinRTObservablePropertyOnFieldsIsNotAotCompatible,
                        fieldSymbol.Locations.FirstOrDefault(),
                        ImmutableDictionary.Create<string, string?>()
                            .Add(FieldReferenceForObservablePropertyFieldAnalyzer.FieldNameKey, fieldSymbol.Name)
                            .Add(FieldReferenceForObservablePropertyFieldAnalyzer.PropertyNameKey, ObservablePropertyGenerator.Execute.GetGeneratedPropertyName(fieldSymbol)),
                        fieldSymbol.ContainingType,
                        fieldSymbol.Name));

                    // Notify that we did produce at least one diagnostic. Note: callbacks can run in parallel, so the order
                    // is not guaranteed. As such, there's no point in using an interlocked compare exchange operation here,
                    // since we couldn't rely on the value being written actually being the "first" occurrence anyway.
                    // So we can just do a normal volatile read for better performance.
                    Volatile.Write(ref firstObservablePropertyAttribute, observablePropertyAttribute);
                }
            }, SymbolKind.Field);

            // If C# preview is already in use, we can stop here. The last diagnostic is only needed when partial properties
            // cannot be used, to inform developers that they'll need to bump the language version to enable the code fixer.
            if (context.Compilation.IsLanguageVersionPreview())
            {
                return;
            }

            context.RegisterCompilationEndAction(context =>
            {
                // If we have produced at least one diagnostic, also emit the info message
                if (Volatile.Read(ref firstObservablePropertyAttribute) is { } observablePropertyAttribute)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        WinRTObservablePropertyOnFieldsIsNotAotCompatibleCompilationEndInfo,
                        observablePropertyAttribute.GetLocation()));
                }
            });
        });
    }
}

#endif

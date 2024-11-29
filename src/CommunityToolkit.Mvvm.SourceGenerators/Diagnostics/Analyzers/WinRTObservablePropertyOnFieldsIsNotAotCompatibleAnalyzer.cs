// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_12_0_OR_GREATER

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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

                    // Track the attribute data to use as target for the diagnostic. This method takes care of effectively
                    // sorting all incoming values, so that the final one is deterministic across runs. This ensures that
                    // the actual location will be the same across recompilations, instead of jumping around all over the
                    // place. This also makes it possible to more easily suppress it, since its location would not change.
                    SetOrUpdateAttributeDataBySourceLocation(ref firstObservablePropertyAttribute, observablePropertyAttribute);
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

    /// <summary>
    /// Sets or updates the <see cref="AttributeData"/> instance to use for compilation diagnostics, sorting by source location.
    /// </summary>
    /// <param name="oldAttributeDataLocation">The location of the previous value to potentially overwrite.</param>
    /// <param name="newAttributeData">Thew new <see cref="AttributeData"/> instance.</param>
    private static void SetOrUpdateAttributeDataBySourceLocation([NotNull] ref AttributeData? oldAttributeDataLocation, AttributeData newAttributeData)
    {
        bool hasReplacedOriginalValue;

        do
        {
            AttributeData? oldAttributeData = Volatile.Read(ref oldAttributeDataLocation);

            // If the old attribute data is null, it means this is the first time we called this method with a new
            // attribute candidate. In that case, there is nothing to check: we should always store the new instance.
            if (oldAttributeData is not null)
            {
                // Sort by file paths, alphabetically
                int filePathRelativeSortIndex = string.Compare(
                    newAttributeData.ApplicationSyntaxReference?.SyntaxTree.FilePath,
                    oldAttributeData.ApplicationSyntaxReference?.SyntaxTree.FilePath,
                    StringComparison.OrdinalIgnoreCase);

                // Also sort by location (this is a tie-breaker if two values are from the same file)
                bool isTextSpanLowerSorted =
                    (newAttributeData.ApplicationSyntaxReference?.Span.Start ?? 0) <
                    (oldAttributeData.ApplicationSyntaxReference?.Span.Start ?? 0);

                // The new candidate can be dropped if it's from a file that's alphabetically sorted after
                // the old value, or whether the location is after the previous one, within the same file.
                if (filePathRelativeSortIndex == 1 || (filePathRelativeSortIndex == 0 && !isTextSpanLowerSorted))
                {
                    break;
                }
            }

            // Attempt to actually replace the old value, without taking locks
            AttributeData? originalValue = Interlocked.CompareExchange(
                location1: ref oldAttributeDataLocation,
                value: newAttributeData,
                comparand: oldAttributeData);

            // This call might have raced against other threads, since all symbol actions can run in parallel.
            // If the original value is the old value we read at the start of the method, it means no other
            // thread raced against this one, so we are done. If it's different, then we failed to write the
            // new candidate. We can discard the work done in this iteration and simply try again.
            hasReplacedOriginalValue = oldAttributeData == originalValue;
        }
        while (!hasReplacedOriginalValue);
#pragma warning disable CS8777 // The loop always ensures that 'oldAttributeDataLocation' is not null on exit
    }
#pragma warning restore CS8777
}

#endif

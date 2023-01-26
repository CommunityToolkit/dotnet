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
/// A diagnostic analyzer that generates an error when a class level <c>[NotifyPropertyChangedRecipients]</c> use is detected.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidClassLevelNotifyPropertyChangedRecipientsAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(InvalidTypeForNotifyPropertyChangedRecipientsError);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the symbols for [NotifyPropertyChangedRecipients], ObservableRecipient and [ObservableRecipient]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedRecipientsAttribute") is not INamedTypeSymbol notifyPropertyChangedRecipientsAttributeSymbol ||
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient") is not INamedTypeSymbol observableRecipientSymbol ||
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipientAttribute") is not INamedTypeSymbol observableRecipientAttributeSymbol)
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

                // Emit a diagnostic for types that use [NotifyPropertyChangedRecipients] but are neither inheriting from ObservableRecipient nor using [ObservableRecipient]
                if (classSymbol.HasAttributeWithType(notifyPropertyChangedRecipientsAttributeSymbol) &&
                    !classSymbol.InheritsFromType(observableRecipientSymbol) &&
                    !classSymbol.HasOrInheritsAttributeWithType(observableRecipientAttributeSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        InvalidTypeForNotifyPropertyChangedRecipientsError,
                        classSymbol.Locations.FirstOrDefault(),
                        classSymbol));
                }
            }, SymbolKind.NamedType);
        });
    }
}

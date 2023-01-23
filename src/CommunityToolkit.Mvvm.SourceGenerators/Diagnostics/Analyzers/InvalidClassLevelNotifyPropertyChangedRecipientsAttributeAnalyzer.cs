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

        context.RegisterSymbolAction(static context =>
        {
            // We're looking for all class declarations
            if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsImplicitlyDeclared: false } classSymbol)
            {
                return;
            }

            // Only inspect classes that are using [NotifyPropertyChangedRecipients]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedRecipientsAttribute") is not INamedTypeSymbol notifyPropertyChangedRecipientsAttributeSymbol ||
                !classSymbol.HasAttributeWithType(notifyPropertyChangedRecipientsAttributeSymbol))
            {
                return;
            }

            // If the containing type is not valid, emit a diagnostic
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient") is INamedTypeSymbol observableRecipientSymbol &&
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipientAttribute") is INamedTypeSymbol observableRecipientAttributeSymbol &&
                !classSymbol.InheritsFromType(observableRecipientSymbol) &&
                !classSymbol.HasOrInheritsAttributeWithType(observableRecipientAttributeSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    InvalidTypeForNotifyPropertyChangedRecipientsError,
                    classSymbol.Locations.FirstOrDefault(),
                    classSymbol));
            }
        }, SymbolKind.NamedType);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error whenever a source-generator attribute is used with not high enough C# version enabled.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnsupportedCSharpLanguageVersionAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The mapping of target attributes that will trigger the analyzer.
    /// </summary>
    private static readonly ImmutableDictionary<string, string> GeneratorAttributeNamesToFullyQualifiedNamesMap = ImmutableDictionary.CreateRange(new[]
    {
        new KeyValuePair<string, string>("INotifyPropertyChangedAttribute", "CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute"),
        new KeyValuePair<string, string>("NotifyCanExecuteChangedForAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedForAttribute"),
        new KeyValuePair<string, string>("NotifyDataErrorInfoAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyDataErrorInfoAttribute"),
        new KeyValuePair<string, string>("NotifyPropertyChangedForAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedForAttribute"),
        new KeyValuePair<string, string>("NotifyPropertyChangedRecipientsAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedRecipientsAttribute"),
        new KeyValuePair<string, string>("ObservableObjectAttribute", "CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute"),
        new KeyValuePair<string, string>("ObservablePropertyAttribute", "CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute"),
        new KeyValuePair<string, string>("ObservableRecipientAttribute", "CommunityToolkit.Mvvm.ComponentModel.ObservableRecipientAttribute"),
        new KeyValuePair<string, string>("RelayCommandAttribute", "CommunityToolkit.Mvvm.Input.RelayCommandAttribute"),
    });

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(UnsupportedCSharpLanguageVersionError);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        // Defer the callback registration to when the compilation starts, so we can execute more
        // preliminary checks and skip registering any kind of symbol analysis at all if not needed.
        context.RegisterCompilationStartAction(static context =>
        {
            // Check that the language version is not high enough, otherwise no diagnostic should ever be produced
            if (context.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
            {
                return;
            }

            context.RegisterSymbolAction(static context =>
            {
                // The possible attribute targets are only fields, classes and methods
                if (context.Symbol is not (IFieldSymbol or INamedTypeSymbol { TypeKind: TypeKind.Class, IsImplicitlyDeclared: false } or IMethodSymbol))
                {
                    return;
                }

                ImmutableArray<AttributeData> attributes = context.Symbol.GetAttributes();

                // If the symbol has no attributes, there's nothing left to do
                if (attributes.IsEmpty)
                {
                    return;
                }

                foreach (AttributeData attribute in attributes)
                {
                    // Go over each attribute on the target symbol, and check if the attribute type name is a candidate.
                    // If it is, double check by actually resolving the symbol from the compilation and comparing against it.
                    // This minimizes the calls to CompilationGetTypeByMetadataName(string) to only cases where it's almost
                    // guaranteed we'll actually get a match. If we do have one, then we can emit the diagnostic for the symbol.
                    if (attribute.AttributeClass is { Name: string attributeName } attributeClass &&
                        GeneratorAttributeNamesToFullyQualifiedNamesMap.TryGetValue(attributeName, out string? fullyQualifiedAttributeName) &&
                        context.Compilation.GetTypeByMetadataName(fullyQualifiedAttributeName) is INamedTypeSymbol attributeSymbol &&
                        SymbolEqualityComparer.Default.Equals(attributeClass, attributeSymbol))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(UnsupportedCSharpLanguageVersionError, context.Symbol.Locations.FirstOrDefault()));
                    }
                }
            }, SymbolKind.Field, SymbolKind.NamedType, SymbolKind.Method);
        });
    }
}

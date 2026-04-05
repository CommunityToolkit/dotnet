// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that reports types requiring generated ObservableValidator hooks that are not partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ObservableValidatorValidationGeneratorPartialTypeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(ObservableValidatorTypeMustBePartial);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            ConcurrentDictionary<(SyntaxTree SyntaxTree, TextSpan Span), byte> reportedLocations = new();

            context.RegisterSymbolAction(context =>
            {
                if (context.Symbol is not INamedTypeSymbol typeSymbol ||
                    typeSymbol.TypeKind != TypeKind.Class)
                {
                    return;
                }

                if (!typeSymbol.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator") ||
                    !HasLocalValidatableProperties(typeSymbol))
                {
                    return;
                }

                foreach (var typeDeclaration in typeSymbol.GetNonPartialTypeDeclarationNodes())
                {
                    if (reportedLocations.TryAdd((typeDeclaration.SyntaxTree, typeDeclaration.Span), 0))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            ObservableValidatorTypeMustBePartial,
                            typeDeclaration.Identifier.GetLocation(),
                            typeDeclaration.Identifier.ValueText));
                    }
                }
            }, SymbolKind.NamedType);
        });
    }

    /// <summary>
    /// Checks whether a target type has any locally declared validatable members.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="INamedTypeSymbol"/> instance to inspect.</param>
    /// <returns>Whether <paramref name="typeSymbol"/> has locally declared validatable members.</returns>
    private static bool HasLocalValidatableProperties(INamedTypeSymbol typeSymbol)
    {
        foreach (ISymbol memberSymbol in typeSymbol.GetMembers())
        {
            if (memberSymbol is IPropertySymbol propertySymbol)
            {
                if (propertySymbol.IsStatic ||
                    propertySymbol.IsIndexer ||
                    !propertySymbol.CanBeReferencedByName)
                {
                    continue;
                }

                if (propertySymbol.GetAttributes().Any(static a => a.AttributeClass?.InheritsFromFullyQualifiedMetadataName(
                    "System.ComponentModel.DataAnnotations.ValidationAttribute") == true))
                {
                    return true;
                }
            }
            else if (memberSymbol is IFieldSymbol fieldSymbol &&
                     fieldSymbol.GetAttributes().Any(static a => a.AttributeClass?.HasFullyQualifiedMetadataName(
                         "CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") == true) &&
                     fieldSymbol.GetAttributes().Any(static a => a.AttributeClass?.InheritsFromFullyQualifiedMetadataName(
                         "System.ComponentModel.DataAnnotations.ValidationAttribute") == true))
            {
                return true;
            }
        }

        return false;
    }
}

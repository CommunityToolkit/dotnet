// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error whenever <c>[ObservableProperty]</c> is used on an invalid property declaration.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyLevelObservablePropertyAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(InvalidPropertyDeclarationForObservableProperty);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the symbol for [ObservableProperty]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // We're intentionally only looking for properties here
                if (context.Symbol is not IPropertySymbol propertySymbol)
                {
                    return;
                }

                // If the property isn't using [ObservableProperty], there's nothing to do
                if (!propertySymbol.TryGetAttributeWithType(observablePropertySymbol, out AttributeData? observablePropertyAttribute))
                {
                    return;
                }

                // Check that the property has valid syntax
                foreach (SyntaxReference propertyReference in propertySymbol.DeclaringSyntaxReferences)
                {
                    SyntaxNode propertyNode = propertyReference.GetSyntax(context.CancellationToken);

                    if (!IsValidCandidateProperty(propertyNode, out _))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            InvalidPropertyDeclarationForObservableProperty,
                            observablePropertyAttribute.GetLocation(),
                            propertySymbol.ContainingType,
                            propertySymbol));
                    }
                }
            }, SymbolKind.Property);
        });
    }

    /// <summary>
    /// Checks whether a given property declaration has valid syntax.
    /// </summary>
    /// <param name="node">The input node to validate.</param>
    /// <param name="containingTypeNode">The resulting node for the containing type of the property, if valid.</param>
    /// <returns>Whether <paramref name="node"/> is a valid property.</returns>
    internal static bool IsValidCandidateProperty(SyntaxNode node, out TypeDeclarationSyntax? containingTypeNode)
    {
        // The node must be a property declaration with two accessors
        if (node is not PropertyDeclarationSyntax { AccessorList.Accessors: { Count: 2 } accessors, AttributeLists.Count: > 0 } property)
        {
            containingTypeNode = null;

            return false;
        }

        // The property must be partial (we'll check that it's a declaration from its symbol)
        if (!property.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            containingTypeNode = null;

            return false;
        }

        // The accessors must be a get and a set (with any accessibility)
        if (accessors[0].Kind() is not (SyntaxKind.GetAccessorDeclaration or SyntaxKind.SetAccessorDeclaration) ||
            accessors[1].Kind() is not (SyntaxKind.GetAccessorDeclaration or SyntaxKind.SetAccessorDeclaration))
        {
            containingTypeNode = null;

            return false;
        }

        containingTypeNode = (TypeDeclarationSyntax?)property.Parent;

        return true;
    }
}

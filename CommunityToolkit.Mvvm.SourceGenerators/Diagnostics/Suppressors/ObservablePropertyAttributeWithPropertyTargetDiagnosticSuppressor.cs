// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// <para>
/// A diagnostic suppressor to suppress CS0657 warnings for fields with [ObservableProperty] using a [property:] attribute list.
/// </para>
/// <para>
/// That is, this diagnostic suppressor will suppress the following diagnostic:
/// <code>
/// public class MyViewModel : ObservableObject
/// {
///     [ObservableProperty]
///     [property: JsonPropertyName("Name")]
///     private string? name;
/// }
/// </code>
/// </para>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ObservablePropertyAttributeWithPropertyTargetDiagnosticSuppressor : DiagnosticSuppressor
{
    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a field using [ObservableProperty] with on attribute list targeting a property.
    /// </summary>
    private static readonly SuppressionDescriptor PropertyAttributeListForObservablePropertyField = new(
        id: "MVVMTKSPR0001",
        suppressedDiagnosticId: "CS0657",
        justification: "Fields using [ObservableProperty] can use [property:] attribute lists to forward attributes to the generated properties");

    /// <inheritdoc/>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(PropertyAttributeListForObservablePropertyField);

    /// <inheritdoc/>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SyntaxNode? syntaxNode = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);

            // Check that the target is effectively [property:] over a field declaration with at least one variable, which is the only case we are interested in
            if (syntaxNode is AttributeTargetSpecifierSyntax { Parent.Parent: FieldDeclarationSyntax { Declaration.Variables.Count: > 0 } fieldDeclaration } attributeTarget &&
                attributeTarget.Identifier.IsKind(SyntaxKind.PropertyKeyword))
            {
                SemanticModel semanticModel = context.GetSemanticModel(syntaxNode.SyntaxTree);

                // Get the field symbol from the first variable declaration
                ISymbol? declaredSymbol = semanticModel.GetDeclaredSymbol(fieldDeclaration.Declaration.Variables[0], context.CancellationToken);

                // Check if the field is using [ObservableProperty], in which case we should suppress the warning
                if (declaredSymbol is IFieldSymbol fieldSymbol &&
                    semanticModel.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is INamedTypeSymbol observablePropertySymbol &&
                    fieldSymbol.GetAttributes().Select(attribute => attribute.AttributeClass).Contains(observablePropertySymbol, SymbolEqualityComparer.Default))
                {
                    context.ReportSuppression(Suppression.Create(PropertyAttributeListForObservablePropertyField, diagnostic));
                }
            }
        }
    }
}

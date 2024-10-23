// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.SuppressionDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// <para>
/// A diagnostic suppressor to suppress CS0657 warnings for fields with [ObservableProperty] using a [property:] attribute list (or [set:] or [get:]).
/// </para>
/// <para>
/// That is, this diagnostic suppressor will suppress the following diagnostic:
/// <code>
/// public partial class MyViewModel : ObservableObject
/// {
///     [ObservableProperty]
///     [property: JsonPropertyName("Name")]
///     private string? name;
/// }
/// </code>
/// </para>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ObservablePropertyAttributeWithSupportedTargetDiagnosticSuppressor : DiagnosticSuppressor
{
    /// <inheritdoc/>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(PropertyAttributeListForObservablePropertyField, PropertyAttributeListForObservablePropertyFieldAccessors);

    /// <inheritdoc/>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SyntaxNode? syntaxNode = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);

            // Check that the target is effectively [property:] over a field declaration with at least one variable, which is the only case we are interested in
            if (syntaxNode is AttributeTargetSpecifierSyntax { Parent.Parent: FieldDeclarationSyntax { Declaration.Variables.Count: > 0 } fieldDeclaration } attributeTarget &&
                (attributeTarget.Identifier.IsKind(SyntaxKind.PropertyKeyword) || attributeTarget.Identifier.IsKind(SyntaxKind.GetKeyword) || attributeTarget.Identifier.IsKind(SyntaxKind.SetKeyword)))
            {
                SemanticModel semanticModel = context.GetSemanticModel(syntaxNode.SyntaxTree);

                // Get the field symbol from the first variable declaration
                ISymbol? declaredSymbol = semanticModel.GetDeclaredSymbol(fieldDeclaration.Declaration.Variables[0], context.CancellationToken);

                // Check if the field is using [ObservableProperty], in which case we should suppress the warning
                if (declaredSymbol is IFieldSymbol fieldSymbol &&
                    semanticModel.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is INamedTypeSymbol observablePropertySymbol &&
                    fieldSymbol.HasAttributeWithType(observablePropertySymbol))
                {
                    // Emit the right suppression based on the attribute modifier. For 'property:', Roslyn
                    // will emit the 'CS0657' warning, whereas for 'get:' or 'set:', it will emit 'CS0658'.
                    if (attributeTarget.Identifier.IsKind(SyntaxKind.PropertyKeyword))
                    {
                        context.ReportSuppression(Suppression.Create(PropertyAttributeListForObservablePropertyField, diagnostic));
                    }
                    else
                    {
                        context.ReportSuppression(Suppression.Create(PropertyAttributeListForObservablePropertyFieldAccessors, diagnostic));
                    }
                }
            }
        }
    }
}

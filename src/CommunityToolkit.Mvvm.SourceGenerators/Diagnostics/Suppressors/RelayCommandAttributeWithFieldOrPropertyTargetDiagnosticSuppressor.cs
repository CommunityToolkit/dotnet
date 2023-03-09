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
/// A diagnostic suppressor to suppress CS0657 warnings for methods with [RelayCommand] using a [field:] or [property:] attribute list.
/// </para>
/// <para>
/// That is, this diagnostic suppressor will suppress the following diagnostic:
/// <code>
/// public partial class MyViewModel : ObservableObject
/// {
///     [RelayCommand]
///     [field: JsonIgnore]
///     [property: SomeOtherAttribute]
///     private void DoSomething()
///     {
///     }
/// }
/// </code>
/// </para>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RelayCommandAttributeWithFieldOrPropertyTargetDiagnosticSuppressor : DiagnosticSuppressor
{
    /// <inheritdoc/>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(FieldOrPropertyAttributeListForRelayCommandMethod);

    /// <inheritdoc/>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SyntaxNode? syntaxNode = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);

            // Check that the target is effectively [field:] or [property:] over a method declaration, which is the case we're looking for
            if (syntaxNode is AttributeTargetSpecifierSyntax { Parent.Parent: MethodDeclarationSyntax methodDeclaration, Identifier: SyntaxToken(SyntaxKind.FieldKeyword or SyntaxKind.PropertyKeyword) })
            {
                SemanticModel semanticModel = context.GetSemanticModel(syntaxNode.SyntaxTree);

                // Get the method symbol from the first variable declaration
                ISymbol? declaredSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                // Check if the method is using [RelayCommand], in which case we should suppress the warning
                if (declaredSymbol is IMethodSymbol methodSymbol &&
                    semanticModel.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.Input.RelayCommandAttribute") is INamedTypeSymbol relayCommandSymbol &&
                    methodSymbol.HasAttributeWithType(relayCommandSymbol))
                {
                    context.ReportSuppression(Suppression.Create(FieldOrPropertyAttributeListForRelayCommandMethod, diagnostic));
                }
            }
        }
    }
}

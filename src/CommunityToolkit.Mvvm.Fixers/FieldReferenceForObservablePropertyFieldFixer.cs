// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.SourceGenerators;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CommunityToolkit.Mvvm.Fixers;

/// <summary>
/// A code fixer that automatically updates references to fields with <c>[ObservableProperty]</c> to reference the generated property instead.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class FieldReferenceForObservablePropertyFieldFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.FieldReferenceForObservablePropertyFieldId);

    /// <inheritdoc/>
    public override FixAllProvider? GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];
        TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

        // Retrieve the properties passed by the analyzer
        if (diagnostic.Properties[FieldReferenceForObservablePropertyFieldAnalyzer.FieldNameKey] is not string fieldName ||
            diagnostic.Properties[FieldReferenceForObservablePropertyFieldAnalyzer.PropertyNameKey] is not string propertyName)
        {
            return;
        }

        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (SyntaxNode syntaxNode in root!.FindNode(diagnosticSpan).DescendantNodesAndSelf())
        {
            // Find the first descendant node from the source of the diagnostic that is an identifier with the target name
            if (syntaxNode is IdentifierNameSyntax { Identifier.Text: string identifierName } identifierNameSyntax &&
                identifierName == fieldName)
            {
                // Register the code fix to update the field reference to use the generated property instead
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Reference property",
                        createChangedDocument: token => UpdateReference(context.Document, identifierNameSyntax, propertyName, token),
                        equivalenceKey: "Reference property"),
                    diagnostic);

                return;
            }
        }
    }

    /// <summary>
    /// Applies the code fix to a target identifier and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="fieldReference">The <see cref="IdentifierNameSyntax"/> corresponding to the field reference to update.</param>
    /// <param name="propertyName">The name of the generated property.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="fieldReference"/> being replaced with a property reference.</returns>
    private static async Task<Document> UpdateReference(Document document, IdentifierNameSyntax fieldReference, string propertyName, CancellationToken cancellationToken)
    {
        IdentifierNameSyntax propertyReference = SyntaxFactory.IdentifierName(propertyName);
        SyntaxNode originalRoot = await fieldReference.SyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
        SyntaxTree updatedTree = originalRoot.ReplaceNode(fieldReference, propertyReference).SyntaxTree;

        return document.WithSyntaxRoot(await updatedTree.GetRootAsync(cancellationToken).ConfigureAwait(false));
    }
}

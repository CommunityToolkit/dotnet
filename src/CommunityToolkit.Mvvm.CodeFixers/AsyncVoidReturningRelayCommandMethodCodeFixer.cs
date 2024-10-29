// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.CodeFixers;

/// <summary>
/// A code fixer that automatically updates the return type of <see langword="async"/> <see cref="void"/> methods using <c>[RelayCommand]</c> to return a <see cref="Task"/> instead.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class AsyncVoidReturningRelayCommandMethodCodeFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(AsyncVoidReturningRelayCommandMethodId);

    /// <inheritdoc/>
    public override FixAllProvider? GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];
        TextSpan diagnosticSpan = context.Span;

        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        // Get the method declaration from the target diagnostic
        if (root!.FindNode(diagnosticSpan) is MethodDeclarationSyntax methodDeclaration)
        {
            // Register the code fix to update the return type to be Task instead
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Change return type to Task",
                    createChangedDocument: token => ChangeReturnType(context.Document, root, methodDeclaration, token),
                    equivalenceKey: "Change return type to Task"),
                diagnostic);
        }
    }

    /// <summary>
    /// Applies the code fix to a target method declaration and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="methodDeclaration">The <see cref="MethodDeclarationSyntax"/> to update.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An updated document with the applied code fix, and the return type of the method being <see cref="Task"/>.</returns>
    private static async Task<Document> ChangeReturnType(Document document, SyntaxNode root, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        // Get the semantic model (bail if it's not available)
        if (await document.GetSemanticModelAsync(cancellationToken) is not SemanticModel semanticModel)
        {
            return document;
        }

        // Also bail if we can't resolve the Task symbol (this should really never happen)
        if (semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task") is not INamedTypeSymbol taskSymbol)
        {
            return document;
        }

        // Create the new syntax node for the return, and configure it to automatically add "using System.Threading.Tasks;" if needed
        SyntaxNode typeSyntax = SyntaxGenerator.GetGenerator(document).TypeExpression(taskSymbol).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);

        // Replace the void return type with the newly created Task type expression
        return document.WithSyntaxRoot(root.ReplaceNode(methodDeclaration.ReturnType, typeSyntax));
    }
}

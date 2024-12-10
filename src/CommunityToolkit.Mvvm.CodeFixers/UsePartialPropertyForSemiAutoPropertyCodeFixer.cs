// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_12_0_OR_GREATER

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.CodeFixers;

/// <summary>
/// A code fixer that converts semi-auto properties to partial properties using <c>[ObservableProperty]</c>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class UsePartialPropertyForSemiAutoPropertyCodeFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(UseObservablePropertyOnSemiAutoPropertyId);

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

        // Get the property declaration from the target diagnostic
        if (root!.FindNode(diagnosticSpan) is PropertyDeclarationSyntax propertyDeclaration)
        {
            // Register the code fix to update the semi-auto property to a partial property
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use a partial property",
                    createChangedDocument: token => ConvertToPartialProperty(context.Document, root, propertyDeclaration),
                    equivalenceKey: "Use a partial property"),
                diagnostic);
        }
    }

    /// <summary>
    /// Applies the code fix to a target identifier and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/> for the property being updated.</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="propertyDeclaration"/> being replaced with a partial property.</returns>
    private static async Task<Document> ConvertToPartialProperty(Document document, SyntaxNode root, PropertyDeclarationSyntax propertyDeclaration)
    {
        await Task.CompletedTask;

        // Get a new property that is partial and with semicolon token accessors
        PropertyDeclarationSyntax updatedPropertyDeclaration =
            propertyDeclaration
            .AddModifiers(Token(SyntaxKind.PartialKeyword))
            .AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("ObservableProperty")))))
            .WithAdditionalAnnotations(Formatter.Annotation)
            .WithAccessorList(AccessorList(List(
            [
                // Keep the accessors (so we can easily keep all trivia, modifiers, attributes, etc.) but make them semicolon only
                propertyDeclaration.AccessorList!.Accessors[0]
                .WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithAdditionalAnnotations(Formatter.Annotation),
                propertyDeclaration.AccessorList!.Accessors[1]
                .WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithAdditionalAnnotations(Formatter.Annotation)
            ])));

        // Create an editor to perform all mutations
        SyntaxEditor editor = new(root, document.Project.Solution.Workspace.Services);

        editor.ReplaceNode(propertyDeclaration, updatedPropertyDeclaration);

        // Find the parent type for the property
        TypeDeclarationSyntax typeDeclaration = propertyDeclaration.FirstAncestorOrSelf<TypeDeclarationSyntax>()!;

        // Make sure it's partial (we create the updated node in the function to preserve the updated property declaration).
        // If we created it separately and replaced it, the whole tree would also be replaced, and we'd lose the new property.
        if (!typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            editor.ReplaceNode(typeDeclaration, static (node, generator) => generator.WithModifiers(node, generator.GetModifiers(node).WithPartial(true)));
        }

        return document.WithSyntaxRoot(editor.GetChangedRoot());
    }
}

#endif

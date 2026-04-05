// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace CommunityToolkit.Mvvm.CodeFixers;

/// <summary>
/// A code fixer that makes types partial for generated ObservableValidator validation support.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class MakeObservableValidatorTypePartialCodeFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.ObservableValidatorTypeMustBePartialId);

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

        if (root?.FindNode(diagnosticSpan).FirstAncestorOrSelf<TypeDeclarationSyntax>() is TypeDeclarationSyntax typeDeclaration)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Make type partial",
                    createChangedDocument: token => MakeTypePartialAsync(context.Document, root, typeDeclaration),
                    equivalenceKey: "Make type partial"),
                diagnostic);
        }
    }

    /// <summary>
    /// Applies the code fix to a target type declaration and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="typeDeclaration">The <see cref="TypeDeclarationSyntax"/> to update.</param>
    /// <returns>An updated document with the applied code fix.</returns>
    private static Task<Document> MakeTypePartialAsync(Document document, SyntaxNode root, TypeDeclarationSyntax typeDeclaration)
    {
        SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
        TypeDeclarationSyntax updatedTypeDeclaration = (TypeDeclarationSyntax)generator.WithModifiers(typeDeclaration, generator.GetModifiers(typeDeclaration).WithPartial(true));

        return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(typeDeclaration, updatedTypeDeclaration)));
    }
}

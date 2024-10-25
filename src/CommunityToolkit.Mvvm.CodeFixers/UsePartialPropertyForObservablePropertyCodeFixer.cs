// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.CodeFixers;

/// <summary>
/// A code fixer that converts fields using <c>[ObservableProperty]</c> to partial properties.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class UsePartialPropertyForObservablePropertyCodeFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(UseObservablePropertyOnPartialPropertyId);

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

        // Retrieve the properties passed by the analyzer
        if (diagnostic.Properties[FieldReferenceForObservablePropertyFieldAnalyzer.FieldNameKey] is not string fieldName ||
            diagnostic.Properties[FieldReferenceForObservablePropertyFieldAnalyzer.PropertyNameKey] is not string propertyName)
        {
            return;
        }

        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        // Get the field declaration from the target diagnostic (we only support individual fields, with a single declaration)
        if (root!.FindNode(diagnosticSpan).FirstAncestorOrSelf<FieldDeclarationSyntax>() is { Declaration.Variables: [{ Identifier.Text: string identifierName }] } fieldDeclaration &&
            identifierName == fieldName)
        {
            // Register the code fix to update the class declaration to inherit from ObservableObject instead
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use a partial property",
                    createChangedDocument: token => ConvertToPartialProperty(context.Document, root, fieldDeclaration, fieldName, propertyName, context.CancellationToken),
                    equivalenceKey: "Use a partial property"),
                diagnostic);
        }
    }

    /// <summary>
    /// Applies the code fix to a target identifier and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/> for the field being updated.</param>
    /// <param name="fieldName">The name of the annotated field.</param>
    /// <param name="propertyName">The name of the generated property.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="fieldDeclaration"/> being replaced with a partial property.</returns>
    private static async Task<Document> ConvertToPartialProperty(
        Document document,
        SyntaxNode root,
        FieldDeclarationSyntax fieldDeclaration,
        string fieldName,
        string propertyName,
        CancellationToken cancellationToken)
    {
        // Get all attributes that were on the field. Here we only include those targeting either
        // the field, or the property. Those targeting the accessors will be moved there directly.
        AttributeListSyntax[] propertyAttributes =
            fieldDeclaration
            .AttributeLists
            .Where(list => list.Target is null || list.Target.Kind() is SyntaxKind.FieldKeyword or SyntaxKind.PropertyKeyword)
            .ToArray();

        // Separately, also get all attributes for the property getters
        AttributeListSyntax[] getterAttributes =
            fieldDeclaration
            .AttributeLists
            .Where(list => list.Target?.Kind() is SyntaxKind.GetKeyword)
            .ToArray();

        // Also do the same for the setters
        AttributeListSyntax[] setterAttributes =
            fieldDeclaration
            .AttributeLists
            .Where(list => list.Target?.Kind() is SyntaxKind.SetKeyword)
            .ToArray();

        // Create the following property declaration:
        //
        // <PROPERTY_ATTRIBUTES>
        // public partial <PROPERTY_TYPE> <PROPERTY_NAME>
        // {
        //     <GETTER_ATTRIBUTES>
        //     get;
        //
        //     <GETTER_ATTRIBUTES>
        //     set;
        // }
        PropertyDeclarationSyntax propertyDeclaration =
            PropertyDeclaration(fieldDeclaration.Declaration.Type, Identifier(propertyName))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword))
            .AddAttributeLists(propertyAttributes)
            .AddAccessorListAccessors(
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .AddAttributeLists(getterAttributes),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .AddAttributeLists(setterAttributes));

        SyntaxTree updatedTree = root.ReplaceNode(fieldDeclaration, propertyDeclaration).SyntaxTree;

        return document.WithSyntaxRoot(await updatedTree.GetRootAsync(cancellationToken).ConfigureAwait(false));
    }
}

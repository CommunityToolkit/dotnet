// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.CodeFixers;

/// <summary>
/// A code fixer that automatically updates types using <c>[ObservableObject]</c> or <c>[INotifyPropertyChanged]</c>
/// that have no base type to inherit from <c>ObservableObject</c> instead.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class ClassUsingAttributeInsteadOfInheritanceCodeFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        InheritFromObservableObjectInsteadOfUsingINotifyPropertyChangedAttributeId,
        InheritFromObservableObjectInsteadOfUsingObservableObjectAttributeId);

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

        // Retrieve the property passed by the analyzer
        if (diagnostic.Properties[ClassUsingAttributeInsteadOfInheritanceAnalyzer.TypeNameKey] is not string typeName ||
            diagnostic.Properties[ClassUsingAttributeInsteadOfInheritanceAnalyzer.AttributeTypeNameKey] is not string attributeTypeName)
        {
            return;
        }

        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        foreach (SyntaxNode syntaxNode in root!.FindNode(diagnosticSpan).DescendantNodesAndSelf())
        {
            // Find the first descendant node from the source of the diagnostic that is a class declaration with the target name
            if (syntaxNode is ClassDeclarationSyntax { Identifier.Text: string identifierName } classDeclaration &&
                identifierName == typeName)
            {
                // Register the code fix to update the class declaration to inherit from ObservableObject instead
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Inherit from ObservableObject",
                        createChangedDocument: token => UpdateReference(context.Document, root, classDeclaration, attributeTypeName),
                        equivalenceKey: "Inherit from ObservableObject"),
                    diagnostic);

                return;
            }
        }
    }

    /// <summary>
    /// Applies the code fix to a target class declaration and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> to update.</param>
    /// <param name="attributeTypeName">The name of the attribute that should be removed.</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="classDeclaration"/> inheriting from <c>ObservableObject</c>.</returns>
    private static Task<Document> UpdateReference(Document document, SyntaxNode root, ClassDeclarationSyntax classDeclaration, string attributeTypeName)
    {
        // Insert ObservableObject always in first position in the base list. The type might have
        // some interfaces in the base list, so we just copy them back after ObservableObject.
        SyntaxGenerator generator = SyntaxGenerator.GetGenerator(document);
        ClassDeclarationSyntax updatedClassDeclaration = (ClassDeclarationSyntax)generator.AddBaseType(classDeclaration, IdentifierName("ObservableObject"));

        // Find the attribute list and attribute to remove
        foreach (AttributeListSyntax attributeList in updatedClassDeclaration.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                if (attribute.Name is IdentifierNameSyntax { Identifier.Text: string identifierName } &&
                    (identifierName == attributeTypeName || (identifierName + "Attribute") == attributeTypeName))
                {
                    // We found the attribute to remove and the list to update
                    updatedClassDeclaration = (ClassDeclarationSyntax)generator.RemoveNode(updatedClassDeclaration, attribute);

                    break;
                }
            }
        }

        return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(classDeclaration, updatedClassDeclaration)));
    }
}

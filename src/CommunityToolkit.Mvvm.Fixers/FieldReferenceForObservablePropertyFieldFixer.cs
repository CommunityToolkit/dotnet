// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.SourceGenerators;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommunityToolkit.Mvvm.Fixers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class FieldReferenceForObservablePropertyFieldFixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.FieldReferenceForObservablePropertyFieldId);

    public override FixAllProvider? GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];
        Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

        string? propertyName = diagnostic.Properties[FieldReferenceForObservablePropertyFieldAnalyzer.PropertyNameKey];
        string? fieldName = diagnostic.Properties[FieldReferenceForObservablePropertyFieldAnalyzer.FieldNameKey];

        if (propertyName == null || fieldName == null)
        {
            return;
        }

        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        Debug.Assert(root != null);

        IdentifierNameSyntax fieldReference = root!.FindNode(diagnosticSpan).DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().FirstOrDefault(i => i.ToString() == fieldName);

        if (fieldReference == null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Reference property",
                createChangedDocument: c => UpdateReference(context.Document, fieldReference, propertyName, c),
                equivalenceKey: "Reference property"),
            diagnostic);

    }

    private async Task<Document> UpdateReference(Document document, IdentifierNameSyntax fieldReference, string propertyName, CancellationToken cancellationToken)
    {
        IdentifierNameSyntax propertyReference = SyntaxFactory.IdentifierName(propertyName);
        SyntaxNode originalRoot = await fieldReference.SyntaxTree.GetRootAsync(cancellationToken);
        SyntaxTree updatedTree = originalRoot.ReplaceNode(fieldReference, propertyReference).SyntaxTree;
        return document.WithSyntaxRoot(await updatedTree.GetRootAsync(cancellationToken));
    }
}

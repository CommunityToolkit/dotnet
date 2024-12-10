// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_12_0_OR_GREATER

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.SourceGenerators;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
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
/// A code fixer that converts fields using <c>[ObservableProperty]</c> to partial properties.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class UsePartialPropertyForObservablePropertyCodeFixer : CodeFixProvider
{
    /// <summary>
    /// The mapping of well-known MVVM Toolkit attributes.
    /// </summary>
    private static readonly ImmutableDictionary<string, string> MvvmToolkitAttributeNamesToFullyQualifiedNamesMap = ImmutableDictionary.CreateRange(new[]
    {
        new KeyValuePair<string, string>("NotifyCanExecuteChangedForAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedForAttribute"),
        new KeyValuePair<string, string>("NotifyDataErrorInfoAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyDataErrorInfoAttribute"),
        new KeyValuePair<string, string>("NotifyPropertyChangedForAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedForAttribute"),
        new KeyValuePair<string, string>("NotifyPropertyChangedRecipientsAttribute", "CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedRecipientsAttribute"),
        new KeyValuePair<string, string>("ObservablePropertyAttribute", "CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute")
    });

    /// <summary>
    /// The mapping of well-known data annotation attributes.
    /// </summary>
    private static readonly ImmutableDictionary<string, string> DataAnnotationsAttributeNamesToFullyQualifiedNamesMap = ImmutableDictionary.CreateRange(new[]
    {
        new KeyValuePair<string, string>("UIHintAttribute", "System.ComponentModel.DataAnnotations.UIHintAttribute"),
        new KeyValuePair<string, string>("ScaffoldColumnAttribute", "System.ComponentModel.DataAnnotations.ScaffoldColumnAttribute"),
        new KeyValuePair<string, string>("DisplayAttribute", "System.ComponentModel.DataAnnotations.DisplayAttribute"),
        new KeyValuePair<string, string>("EditableAttribute", "System.ComponentModel.DataAnnotations.EditableAttribute"),
        new KeyValuePair<string, string>("KeyAttribute", "System.ComponentModel.DataAnnotations.KeyAttribute")
    });

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        UseObservablePropertyOnPartialPropertyId,
        WinRTObservablePropertyOnFieldsIsNotAotCompatibleId);

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

        // This code fixer needs the semantic model, so check that first
        if (!context.Document.SupportsSemanticModel)
        {
            return;
        }

        SemanticModel semanticModel = (await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false))!;

        // If the language is not preview, we cannot apply this code fix (as it would generate invalid C# code)
        if (!semanticModel.Compilation.IsLanguageVersionPreview())
        {
            return;
        }

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
            // Register the code fix to convert the field declaration to a partial property
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use a partial property",
                    createChangedDocument: token => ConvertToPartialProperty(context.Document, root, fieldDeclaration, semanticModel, fieldName, propertyName, context.CancellationToken),
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
    /// <param name="semanticModel">The semantic model for <paramref name="document"/>.</param>
    /// <param name="fieldName">The name of the annotated field.</param>
    /// <param name="propertyName">The name of the generated property.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="fieldDeclaration"/> being replaced with a partial property.</returns>
    private static async Task<Document> ConvertToPartialProperty(
        Document document,
        SyntaxNode root,
        FieldDeclarationSyntax fieldDeclaration,
        SemanticModel semanticModel,
        string fieldName,
        string propertyName,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        // Try to get all necessary type symbols to process the attributes
        if (!semanticModel.Compilation.TryBuildNamedTypeSymbolMap(MvvmToolkitAttributeNamesToFullyQualifiedNamesMap, out ImmutableDictionary<string, INamedTypeSymbol>? toolkitTypeSymbols) ||
            !semanticModel.Compilation.TryBuildNamedTypeSymbolMap(DataAnnotationsAttributeNamesToFullyQualifiedNamesMap, out ImmutableDictionary<string, INamedTypeSymbol>? annotationTypeSymbols))
        {
            return document;
        }

        // Also query [ValidationAttribute]
        if (semanticModel.Compilation.GetTypeByMetadataName("System.ComponentModel.DataAnnotations.ValidationAttribute") is not INamedTypeSymbol validationAttributeSymbol)
        {
            return document;
        }

        // Get all attributes that were on the field. Here we only include those targeting either
        // the field, or the property. Those targeting the accessors will be moved there directly.
        List<AttributeListSyntax> propertyAttributes =
            fieldDeclaration
            .AttributeLists
            .Where(list => list.Target is null || list.Target.Identifier.Kind() is not (SyntaxKind.GetKeyword or SyntaxKind.SetKeyword))
            .ToList();

        // Fixup attribute lists as following:
        //   1) If they have the 'field:' target, keep it (it's no longer the default)
        //   2) If they have the 'property:' target, remove it (it's not needed anymore)
        //   3) If they are from the MVVM Toolkit, remove the target (they'll apply to the property)
        //   4) If they have no target and they are either a validation attribute, or any of the well-known
        //     data annotation attributes (which are automatically forwarded), leave them without a target.
        //   5) If they have no target, add 'field:' to preserve the original behavior
        //   5) Otherwise, leave them without changes (this will carry over invalid targets as-is)
        for (int i = 0; i < propertyAttributes.Count; i++)
        {
            AttributeListSyntax attributeListSyntax = propertyAttributes[i];

            // Special case: the list has no attributes. Just remove it entirely.
            if (attributeListSyntax.Attributes is [])
            {
                propertyAttributes.RemoveAt(i--);

                continue;
            }

            // Case 1
            if (attributeListSyntax.Target?.Identifier.IsKind(SyntaxKind.FieldKeyword) is true)
            {
                continue;
            }

            // Case 2
            if (attributeListSyntax.Target?.Identifier.IsKind(SyntaxKind.PropertyKeyword) is true)
            {
                propertyAttributes[i] = attributeListSyntax.WithTarget(null);

                continue;
            }

            if (attributeListSyntax.Attributes.Count == 1)
            {
                // Make sure we can retrieve the symbol for the attribute type
                if (!semanticModel.GetSymbolInfo(attributeListSyntax.Attributes[0], cancellationToken).TryGetAttributeTypeSymbol(out INamedTypeSymbol? attributeSymbol))
                {
                    return document;
                }

                // Case 3
                if (toolkitTypeSymbols.ContainsValue(attributeSymbol))
                {
                    propertyAttributes[i] = attributeListSyntax.WithTarget(null);

                    continue;
                }

                // Case 4
                if (annotationTypeSymbols.ContainsValue(attributeSymbol) || attributeSymbol.InheritsFromType(validationAttributeSymbol))
                {
                    continue;
                }

                // Case 5
                if (attributeListSyntax.Target is null)
                {
                    propertyAttributes[i] = attributeListSyntax.WithTarget(AttributeTargetSpecifier(Token(SyntaxKind.FieldKeyword)));

                    continue;
                }
            }
            else
            {
                // If we have multiple attributes in the current list, we need additional logic here.
                // We could have any number of attributes here, so we split them into three buckets:
                //   - MVVM Toolkit attributes: these should be moved over with no target
                //   - Data annotation or validation attributes: these should be moved over with the same target
                //   - Any other attributes: these should be moved over with the 'field' target
                List<AttributeSyntax> mvvmToolkitAttributes = [];
                List<AttributeSyntax> annotationOrValidationAttributes = [];
                List<AttributeSyntax> fieldAttributes = [];

                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    // Like for the single attribute case, make sure we can get the symbol for the attribute
                    if (!semanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).TryGetAttributeTypeSymbol(out INamedTypeSymbol? attributeSymbol))
                    {
                        return document;
                    }

                    bool isAnnotationOrValidationAttribute = annotationTypeSymbols.ContainsValue(attributeSymbol) || attributeSymbol.InheritsFromType(validationAttributeSymbol);

                    // Split the attributes into the buckets. Note that we have a special rule for annotation and validation
                    // attributes when no target is specified. In that case, we will merge them with the MVVM Toolkit items.
                    // This allows us to try to keep the attributes in the same attribute list, rather than splitting them.
                    if (toolkitTypeSymbols.ContainsValue(attributeSymbol) || (isAnnotationOrValidationAttribute && attributeListSyntax.Target is null))
                    {
                        mvvmToolkitAttributes.Add(attributeSyntax);
                    }
                    else if (isAnnotationOrValidationAttribute)
                    {
                        annotationOrValidationAttributes.Add(attributeSyntax);
                    }
                    else
                    {
                        fieldAttributes.Add(attributeSyntax);
                    }
                }

                // We need to start inserting the new lists right before the one we're currently
                // processing. We'll be removing it when we're done, the buckets will replace it.
                int insertionIndex = i;

                // Helper to process and insert the new synthesized attribute lists into the target collection
                void InsertAttributeListIfNeeded(List<AttributeSyntax> attributes, AttributeTargetSpecifierSyntax? attributeTarget)
                {
                    if (attributes is [])
                    {
                        return;
                    }

                    AttributeListSyntax attributeList = AttributeList(SeparatedList(attributes)).WithTarget(attributeTarget);

                    // Only if this is the first non empty list we're adding, carry over the original trivia
                    if (insertionIndex == i)
                    {
                        attributeList = attributeList.WithTriviaFrom(attributeListSyntax);
                    }

                    // Finally, insert the new list into the final tree
                    propertyAttributes.Insert(insertionIndex++, attributeList);
                }

                InsertAttributeListIfNeeded(mvvmToolkitAttributes, attributeTarget: null);
                InsertAttributeListIfNeeded(annotationOrValidationAttributes, attributeTarget: attributeListSyntax.Target);
                InsertAttributeListIfNeeded(fieldAttributes, attributeTarget: AttributeTargetSpecifier(Token(SyntaxKind.FieldKeyword)));

                // Remove the attribute list that we have just split into buckets
                propertyAttributes.RemoveAt(insertionIndex);

                // Move the current loop iteration to the last inserted item.
                // We decrement by 1 because the new loop iteration will add 1.
                i = insertionIndex - 1;
            }
        }

        // Separately, also get all attributes for the property getters
        AttributeListSyntax[] getterAttributes =
            fieldDeclaration
            .AttributeLists
            .Where(list => list.Target?.Identifier.Kind() is SyntaxKind.GetKeyword)
            .Select(list => list.WithTarget(null).WithAdditionalAnnotations(Formatter.Annotation))
            .ToArray();

        // Also do the same for the setters
        AttributeListSyntax[] setterAttributes =
            fieldDeclaration
            .AttributeLists
            .Where(list => list.Target?.Identifier.Kind() is SyntaxKind.SetKeyword)
            .Select(list => list.WithTarget(null).WithAdditionalAnnotations(Formatter.Annotation))
            .ToArray();

        // Create the following property declaration:
        //
        // <PROPERTY_ATTRIBUTES>
        // <PROPERTY_MODIFIERS> <PROPERTY_TYPE> <PROPERTY_NAME>
        // {
        //     <GETTER_ATTRIBUTES>
        //     get;
        //
        //     <GETTER_ATTRIBUTES>
        //     set;
        // }
        PropertyDeclarationSyntax propertyDeclaration =
            PropertyDeclaration(fieldDeclaration.Declaration.Type, Identifier(propertyName))
            .WithModifiers(GetPropertyModifiers(fieldDeclaration))
            .AddAttributeLists(propertyAttributes.ToArray())
            .WithAdditionalAnnotations(Formatter.Annotation)
            .AddAccessorListAccessors(
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .AddAttributeLists(getterAttributes)
                .WithAdditionalAnnotations(Formatter.Annotation),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .AddAttributeLists(setterAttributes)
                .WithAdditionalAnnotations(Formatter.Annotation));

        // If the field has an initializer, preserve that on the property
        if (fieldDeclaration.Declaration.Variables[0].Initializer is { } fieldInitializer)
        {
            propertyDeclaration = propertyDeclaration.WithInitializer(fieldInitializer).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        // Create an editor to perform all mutations. This allows to keep track of multiple
        // replacements for nodes on the same original tree, which otherwise wouldn't work.
        SyntaxEditor editor = new(root, document.Project.Solution.Workspace.Services);

        editor.ReplaceNode(fieldDeclaration, propertyDeclaration);

        // Get the field declaration from the target diagnostic (we only support individual fields, with a single declaration)
        foreach (SyntaxNode descendantNode in root.DescendantNodes())
        {
            // We only care about identifier nodes
            if (descendantNode is not IdentifierNameSyntax identifierSyntax)
            {
                continue;
            }

            // Pre-filter to only match the field name we just replaced
            if (identifierSyntax.Identifier.Text != fieldName)
            {
                continue;
            }

            // Make sure the identifier actually refers to the field being replaced
            if (semanticModel.GetSymbolInfo(identifierSyntax, cancellationToken).Symbol is not IFieldSymbol fieldSymbol)
            {
                continue;
            }

            // Special case for 'this.<FIELD_NAME>' accesses: we want to drop the 'this.' prefix
            if (identifierSyntax.Parent is MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } thisExpressionSyntax)
            {
                editor.ReplaceNode(thisExpressionSyntax, IdentifierName(propertyName));
            }
            else
            {
                // Replace the field reference with a reference to the new property
                editor.ReplaceNode(identifierSyntax, IdentifierName(propertyName));
            }
        }

        return document.WithSyntaxRoot(editor.GetChangedRoot());
    }

    /// <summary>
    /// Gets all modifiers that need to be added to a generated property.
    /// </summary>
    /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/> for the field being updated.</param>
    /// <returns>The list of necessary modifiers for <paramref name="fieldDeclaration"/>.</returns>
    private static SyntaxTokenList GetPropertyModifiers(FieldDeclarationSyntax fieldDeclaration)
    {
        SyntaxTokenList propertyModifiers = TokenList(Token(SyntaxKind.PublicKeyword));

        // Add the 'required' modifier if the field also had it
        if (fieldDeclaration.Modifiers.Any(SyntaxKind.RequiredKeyword))
        {
            propertyModifiers = propertyModifiers.Add(Token(SyntaxKind.RequiredKeyword));
        }

        // Always add 'partial' last
        return propertyModifiers.Add(Token(SyntaxKind.PartialKeyword));
    }
}

#endif

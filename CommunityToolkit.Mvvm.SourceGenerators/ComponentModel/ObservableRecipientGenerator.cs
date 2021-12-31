// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for the <c>ObservableRecipientAttribute</c> type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class ObservableRecipientGenerator : TransitiveMembersGenerator<ObservableRecipientInfo>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableRecipientGenerator"/> class.
    /// </summary>
    public ObservableRecipientGenerator()
        : base("global::CommunityToolkit.Mvvm.ComponentModel.ObservableRecipientAttribute")
    {
    }

    /// <inheritdoc/>
    protected override IncrementalValuesProvider<(INamedTypeSymbol Symbol, ObservableRecipientInfo Info)> GetInfo(
        IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<(INamedTypeSymbol Symbol, AttributeData AttributeData)> source)
    {
        static ObservableRecipientInfo GetInfo(INamedTypeSymbol typeSymbol, AttributeData attributeData, bool isRequiresUnreferencedCodeAttributeAvailable)
        {
            string typeName = typeSymbol.Name;
            bool hasExplicitConstructors = !(typeSymbol.InstanceConstructors.Length == 1 && typeSymbol.InstanceConstructors[0] is { Parameters.IsEmpty: true, IsImplicitlyDeclared: true });
            bool isAbstract = typeSymbol.IsAbstract;
            bool isObservableValidator = typeSymbol.InheritsFrom("global::CommunityToolkit.Mvvm.ComponentModel.ObservableValidator");

            return new(
                typeName,
                hasExplicitConstructors,
                isAbstract,
                isObservableValidator,
                isRequiresUnreferencedCodeAttributeAvailable);
        }

        // Check whether [RequiresUnreferencedCode] is available
        IncrementalValueProvider<bool> isRequiresUnreferencedCodeAttributeAvailable =
            context.CompilationProvider
            .Select(static (item, _) => item.GetTypeByMetadataName("System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute") is { DeclaredAccessibility: Accessibility.Public });

        return
            source
            .Combine(isRequiresUnreferencedCodeAttributeAvailable)
            .Select(static (item, _) => (item.Left.Symbol, GetInfo(item.Left.Symbol, item.Left.AttributeData, item.Right)));
    }

    /// <inheritdoc/>
    protected override bool ValidateTargetType(INamedTypeSymbol typeSymbol, ObservableRecipientInfo info, out ImmutableArray<Diagnostic> diagnostics)
    {
        ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();

        // Check if the type already inherits from ObservableRecipient
        if (typeSymbol.InheritsFrom("global::CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient"))
        {
            builder.Add(DuplicateObservableRecipientError, typeSymbol, typeSymbol);

            diagnostics = builder.ToImmutable();

            return false;
        }

        // In order to use [ObservableRecipient], the target type needs to inherit from ObservableObject,
        // or be annotated with [ObservableObject] or [INotifyPropertyChanged] (with additional helpers).
        if (!typeSymbol.InheritsFrom("global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject") &&
            !typeSymbol.GetAttributes().Any(static a => a.AttributeClass?.HasFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute") == true) &&
            !typeSymbol.GetAttributes().Any(static a =>
                a.AttributeClass?.HasFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute") == true &&
                !a.HasNamedArgument("IncludeAdditionalHelperMethods", false)))
        {
            builder.Add(MissingBaseObservableObjectFunctionalityError, typeSymbol, typeSymbol);

            diagnostics = builder.ToImmutable();

            return false;
        }

        diagnostics = builder.ToImmutable();

        return true;
    }

    /// <inheritdoc/>
    protected override ImmutableArray<MemberDeclarationSyntax> FilterDeclaredMembers(ObservableRecipientInfo info, ImmutableArray<MemberDeclarationSyntax> memberDeclarations)
    {
        ImmutableArray<MemberDeclarationSyntax>.Builder builder = ImmutableArray.CreateBuilder<MemberDeclarationSyntax>();

        // If the target type has no constructors, generate constructors as well
        if (!info.HasExplicitConstructors)
        {
            foreach (ConstructorDeclarationSyntax ctor in memberDeclarations.OfType<ConstructorDeclarationSyntax>())
            {
                string text = ctor.NormalizeWhitespace().ToFullString();
                string replaced = text.Replace("ObservableRecipient", info.TypeName);

                // Adjust the visibility of the constructors based on whether the target type is abstract.
                // If that is not the case, the constructors have to be declared as public and not protected.
                if (!info.IsAbstract)
                {
                    replaced = replaced.Replace("protected", "public");
                }

                builder.Add((ConstructorDeclarationSyntax)ParseMemberDeclaration(replaced)!);
            }
        }

        MemberDeclarationSyntax RemoveRequiresUnreferencedCodeAttributeIfNeeded(MemberDeclarationSyntax member)
        {
            if (!info.IsRequiresUnreferencedCodeAttributeAvailable &&
                member is PropertyDeclarationSyntax { Identifier.ValueText: "IsActive" } or MethodDeclarationSyntax { Identifier.ValueText: "OnActivated" })
            {
                SyntaxNode attributeNode =
                    member
                    .DescendantNodes()
                    .OfType<AttributeListSyntax>()
                    .First(node => node.Attributes[0].Name is QualifiedNameSyntax { Right: IdentifierNameSyntax { Identifier.ValueText: "RequiresUnreferencedCode" } });

                return member.RemoveNode(attributeNode, SyntaxRemoveOptions.KeepExteriorTrivia)!;
            }

            return member;
        }

        // Skip the SetProperty overloads if the target type inherits from ObservableValidator, to avoid conflicts
        if (info.IsObservableValidator)
        {
            foreach (MemberDeclarationSyntax member in memberDeclarations.Where(static member => member is not ConstructorDeclarationSyntax))
            {
                if (member is not MethodDeclarationSyntax { Identifier.ValueText: "SetProperty" })
                {
                    builder.Add(RemoveRequiresUnreferencedCodeAttributeIfNeeded(member));
                }
            }

            return builder.ToImmutable();
        }

        // If the target type has at least one custom constructor, only generate methods
        foreach (MemberDeclarationSyntax member in memberDeclarations.Where(static member => member is not ConstructorDeclarationSyntax))
        {
            builder.Add(RemoveRequiresUnreferencedCodeAttributeIfNeeded(member));
        }

        return builder.ToImmutable();
    }
}

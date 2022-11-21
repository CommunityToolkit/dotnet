// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
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
        : base("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipientAttribute")
    {
    }

    /// <inheritdoc/>
    private protected override ObservableRecipientInfo? ValidateTargetTypeAndGetInfo(INamedTypeSymbol typeSymbol, AttributeData attributeData, Compilation compilation, out ImmutableArray<DiagnosticInfo> diagnostics)
    {
        diagnostics = ImmutableArray<DiagnosticInfo>.Empty;

        ObservableRecipientInfo? info = null;

        // Check if the type already inherits from ObservableRecipient
        if (typeSymbol.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient"))
        {
            diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(DuplicateObservableRecipientError, typeSymbol, typeSymbol));

            goto End;
        }

        // Check if the type already inherits [ObservableRecipient]
        if (typeSymbol.InheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableRecipientAttribute"))
        {
            diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(InvalidAttributeCombinationForObservableRecipientAttributeError, typeSymbol, typeSymbol));

            goto End;
        }

        // In order to use [ObservableRecipient], the target type needs to inherit from ObservableObject,
        // or be annotated with [ObservableObject] or [INotifyPropertyChanged] (with additional helpers).
        if (!typeSymbol.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableObject") &&
            !typeSymbol.HasOrInheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute") &&
            !typeSymbol.HasOrInheritsAttribute(static a =>
                a.AttributeClass?.HasFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute") == true &&
                !a.HasNamedArgument("IncludeAdditionalHelperMethods", false)))
        {
            diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(MissingBaseObservableObjectFunctionalityError, typeSymbol, typeSymbol));

            goto End;
        }

        // Gather all necessary info to propagate down the pipeline
        string typeName = typeSymbol.Name;
        bool hasExplicitConstructors = !(typeSymbol.InstanceConstructors.Length == 1 && typeSymbol.InstanceConstructors[0] is { Parameters.IsEmpty: true, IsImplicitlyDeclared: true });
        bool isAbstract = typeSymbol.IsAbstract;
        bool isObservableValidator = typeSymbol.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator");
        bool isRequiresUnreferencedCodeAttributeAvailable = compilation.HasAccessibleTypeWithMetadataName("System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute");
        bool hasOnActivatedMethod = typeSymbol.GetMembers().Any(m => m is IMethodSymbol { Parameters.IsEmpty: true, Name: "OnActivated" });
        bool hasOnDeactivatedMethod = typeSymbol.GetMembers().Any(m => m is IMethodSymbol { Parameters.IsEmpty: true, Name: "OnDeactivated" });

        info = new ObservableRecipientInfo(
            typeName,
            hasExplicitConstructors,
            isAbstract,
            isObservableValidator,
            isRequiresUnreferencedCodeAttributeAvailable,
            hasOnActivatedMethod,
            hasOnDeactivatedMethod);

        End:
        return info;
    }

    /// <inheritdoc/>
    protected override ImmutableArray<MemberDeclarationSyntax> FilterDeclaredMembers(ObservableRecipientInfo info, ImmutableArray<MemberDeclarationSyntax> memberDeclarations)
    {
        using ImmutableArrayBuilder<MemberDeclarationSyntax> builder = ImmutableArrayBuilder<MemberDeclarationSyntax>.Rent();

        // If the target type has no constructors, generate constructors as well
        if (!info.HasExplicitConstructors)
        {
            foreach (ConstructorDeclarationSyntax originalConstructor in memberDeclarations.OfType<ConstructorDeclarationSyntax>())
            {
                ConstructorDeclarationSyntax modifiedConstructor = originalConstructor.WithIdentifier(Identifier(info.TypeName));

                // Adjust the visibility of the constructors based on whether the target type is abstract.
                // If that is not the case, the constructors have to be declared as public and not protected.
                if (!info.IsAbstract)
                {
                    modifiedConstructor = modifiedConstructor.WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));
                }

                builder.Add(modifiedConstructor);
            }
        }

        MemberDeclarationSyntax FixupFilteredMemberDeclaration(MemberDeclarationSyntax member)
        {
            // Make OnActivated partial if the type already has the method
            if (info.HasOnActivatedMethod &&
                member is MethodDeclarationSyntax { Identifier.ValueText: "OnActivated" } onActivatdMethod)
            {
                SyntaxNode attributeNode =
                    member
                    .DescendantNodes()
                    .OfType<AttributeListSyntax>()
                    .First(node => node.Attributes[0].Name is QualifiedNameSyntax { Right: IdentifierNameSyntax { Identifier.ValueText: "RequiresUnreferencedCode" } });

                return
                    onActivatdMethod
                    .RemoveNode(attributeNode, SyntaxRemoveOptions.KeepExteriorTrivia)!
                    .AddModifiers(Token(SyntaxKind.PartialKeyword))
                    .WithBody(null)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            }

            // Make OnDeactivated partial if the type already has the method
            if (info.HasOnDeactivatedMethod &&
                member is MethodDeclarationSyntax { Identifier.ValueText: "OnDeactivated" } onDeactivatedMethod)
            {
                return
                    onDeactivatedMethod
                    .AddModifiers(Token(SyntaxKind.PartialKeyword))
                    .WithBody(null)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            }

            // Remove [RequiresUnreferencedCode] if the attribute is not available
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
                    builder.Add(FixupFilteredMemberDeclaration(member));
                }
            }

            return builder.ToImmutable();
        }

        // If the target type has at least one custom constructor, only generate methods
        foreach (MemberDeclarationSyntax member in memberDeclarations.Where(static member => member is not ConstructorDeclarationSyntax))
        {
            builder.Add(FixupFilteredMemberDeclaration(member));
        }

        return builder.ToImmutable();
    }
}

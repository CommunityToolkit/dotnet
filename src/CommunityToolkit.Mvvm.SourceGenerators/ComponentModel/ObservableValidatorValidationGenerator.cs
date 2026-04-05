// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for hook-based <c>ObservableValidator</c> validation.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class ObservableValidatorValidationGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ValidationInfo> explicitValidationInfo =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is TypeDeclarationSyntax typeDeclaration && typeDeclaration.HasOrPotentiallyHasBaseTypes(),
                static (context, token) =>
                {
                    if (!context.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
                    {
                        return default;
                    }

                    INamedTypeSymbol typeSymbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!;

                    if (!context.Node.IsFirstSyntaxDeclarationForSymbol(typeSymbol))
                    {
                        return default;
                    }

                    token.ThrowIfCancellationRequested();

                    if (!Execute.IsObservableValidator(typeSymbol))
                    {
                        return default;
                    }

                    token.ThrowIfCancellationRequested();

                    return Execute.GetInfo(typeSymbol, token);
                })
            .Where(static item => item is not null)!;

        IncrementalValuesProvider<(ValidationTypeInfo Left, PropertyValidationInfo Right)> explicitValidationMembers =
            explicitValidationInfo.SelectMany(static (item, _) =>
                item.Properties.Select(property => (new ValidationTypeInfo(item.Hierarchy, item.TypeName), property)));

        IncrementalValuesProvider<(ValidationTypeInfo Left, PropertyValidationInfo Right)> generatedPropertyValidationMembers =
            context.ForAttributeWithMetadataNameAndOptions(
                "CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute",
                ObservablePropertyGenerator.Execute.IsCandidatePropertyDeclaration,
                static (context, token) =>
                {
                    MemberDeclarationSyntax memberSyntax = ObservablePropertyGenerator.Execute.GetCandidateMemberDeclaration(context.TargetNode);

                    if (!ObservablePropertyGenerator.Execute.IsCandidateValidForCompilation(memberSyntax, context.SemanticModel) ||
                        !ObservablePropertyGenerator.Execute.IsCandidateSymbolValid(context.TargetSymbol) ||
                        context.TargetSymbol is not IFieldSymbol fieldSymbol)
                    {
                        return default;
                    }

                    token.ThrowIfCancellationRequested();

                    return Execute.GetGeneratedObservablePropertyValidationInfo(memberSyntax, fieldSymbol, context.SemanticModel, context.GlobalOptions, token);
                })
            .Where(static item => item.Left is not null)!;

        IncrementalValuesProvider<(ValidationTypeInfo Left, PropertyValidationInfo Right)> validationMembers =
            explicitValidationMembers
            .Collect()
            .Combine(generatedPropertyValidationMembers.Collect())
            .SelectMany(static (item, _) => item.Left.Concat(item.Right));

        IncrementalValuesProvider<ValidationInfo> validationInfo =
            validationMembers
            .GroupBy(static item => item.Left, static item => item.Right)
            .Select(static (item, _) => new ValidationInfo(item.Key.Hierarchy, item.Key.TypeName, item.Right));

        IncrementalValueProvider<bool> isHeaderFileNeeded =
            validationInfo
            .Collect()
            .Select(static (item, _) => item.Length > 0);

        IncrementalValueProvider<bool> isDynamicallyAccessedMembersAttributeAvailable =
            context.CompilationProvider
            .Select(static (item, _) => item.HasAccessibleTypeWithMetadataName("System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute"));

        IncrementalValueProvider<(bool Condition, bool State)> headerFileInfo =
            isHeaderFileNeeded
            .Combine(isDynamicallyAccessedMembersAttributeAvailable)
            .Select(static (item, _) => (item.Left, item.Right));

        context.RegisterConditionalImplementationSourceOutput(headerFileInfo, static (context, item) =>
        {
            CompilationUnitSyntax compilationUnit = Execute.GetHeaderSyntax(item);

            context.AddSource("__ObservableValidatorExtensions.g.cs", compilationUnit);
        });

        context.RegisterSourceOutput(validationInfo, static (context, item) =>
        {
            CompilationUnitSyntax compilationUnit = Execute.GetSyntax(item);

            context.AddSource($"{item.Hierarchy.FilenameHint}.ObservableValidator.g.cs", compilationUnit);
        });
    }
}

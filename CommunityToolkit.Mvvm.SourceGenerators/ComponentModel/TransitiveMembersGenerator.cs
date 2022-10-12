// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for a given attribute type.
/// </summary>
/// <typeparam name="TInfo">The type of info gathered for each target type to process.</typeparam>
public abstract partial class TransitiveMembersGenerator<TInfo> : IIncrementalGenerator
{
    /// <summary>
    /// The fully qualified metadata name of the attribute type to look for.
    /// </summary>
    private readonly string fullyQualifiedAttributeMetadataName;

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> instance to compare intermediate models.
    /// </summary>
    /// <remarks>
    /// This is needed to cache extracted info on attributes used to annotate target types.
    /// </remarks>
    private readonly IEqualityComparer<TInfo> comparer;

    /// <summary>
    /// The preloaded <see cref="ClassDeclarationSyntax"/> instance with members to generate.
    /// </summary>
    private readonly ClassDeclarationSyntax classDeclaration;

    /// <summary>
    /// The sequence of member declarations for sealed types.
    /// </summary>
    private ImmutableArray<MemberDeclarationSyntax> sealedMemberDeclarations;

    /// <summary>
    /// The resulting sequence of member declarations for non sealed types.
    /// </summary>
    private ImmutableArray<MemberDeclarationSyntax> nonSealedMemberDeclarations;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransitiveMembersGenerator{TInfo}"/> class.
    /// </summary>
    /// <param name="fullyQualifiedAttributeMetadataName">The fully qualified metadata name of the attribute type to look for.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> instance to compare intermediate models.</param>
    private protected TransitiveMembersGenerator(string fullyQualifiedAttributeMetadataName, IEqualityComparer<TInfo>? comparer = null)
    {
        this.fullyQualifiedAttributeMetadataName = fullyQualifiedAttributeMetadataName;
        this.comparer = comparer ?? EqualityComparer<TInfo>.Default;
        this.classDeclaration = Execute.LoadClassDeclaration(fullyQualifiedAttributeMetadataName);

        Execute.ProcessMemberDeclarations(
            GetType(),
            this.classDeclaration.Members.ToImmutableArray(),
            out this.sealedMemberDeclarations,
            out this.nonSealedMemberDeclarations);
    }

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Gather all generation info, and any diagnostics
        IncrementalValuesProvider<Result<(HierarchyInfo Hierarchy, bool IsSealed, TInfo? Info)>> generationInfoWithErrors =
            context.SyntaxProvider
            .ForAttributeWithMetadataName(
                this.fullyQualifiedAttributeMetadataName,
                static (node, _) => node is ClassDeclarationSyntax classDeclaration && classDeclaration.HasOrPotentiallyHasAttributes(),
                (context, token) =>
                {
                    if (!context.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
                    {
                        return default;
                    }

                    INamedTypeSymbol typeSymbol = (INamedTypeSymbol)context.TargetSymbol;

                    // Gather all generation info, and any diagnostics
                    TInfo? info = ValidateTargetTypeAndGetInfo(typeSymbol, context.Attributes[0], context.SemanticModel.Compilation, out ImmutableArray<DiagnosticInfo> diagnostics);

                    // If there are any diagnostics, there's no need to compute the hierarchy info at all, just return them
                    if (diagnostics.Length > 0)
                    {
                        return new Result<(HierarchyInfo, bool, TInfo?)>(default, diagnostics);
                    }

                    HierarchyInfo hierarchy = HierarchyInfo.From(typeSymbol);

                    return new Result<(HierarchyInfo, bool, TInfo?)>((hierarchy, typeSymbol.IsSealed, info), diagnostics);
                })
            .Where(static item => item is not null)!;

        // Emit the diagnostic, if needed
        context.ReportDiagnostics(generationInfoWithErrors.Select(static (item, _) => item.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, bool IsSealed, TInfo Info)> generationInfo =
            generationInfoWithErrors
            .Where(static item => item.Errors.IsEmpty)
            .Select(static (item, _) => item.Value)!
            .WithComparers(HierarchyInfo.Comparer.Default, EqualityComparer<bool>.Default, this.comparer);

        // Generate the required members
        context.RegisterSourceOutput(generationInfo, (context, item) =>
        {
            ImmutableArray<MemberDeclarationSyntax> sourceMemberDeclarations = item.IsSealed ? this.sealedMemberDeclarations : this.nonSealedMemberDeclarations;
            ImmutableArray<MemberDeclarationSyntax> filteredMemberDeclarations = FilterDeclaredMembers(item.Info, sourceMemberDeclarations);
            CompilationUnitSyntax compilationUnit = item.Hierarchy.GetCompilationUnit(filteredMemberDeclarations, this.classDeclaration.BaseList);

            context.AddSource($"{item.Hierarchy.FilenameHint}.g.cs", compilationUnit.GetText(Encoding.UTF8));
        });
    }

    /// <summary>
    /// Validates the target type being processes, gets the info if possible and produces all necessary diagnostics.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="INamedTypeSymbol"/> instance currently being processed.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> instance for the attribute used over <paramref name="typeSymbol"/>.</param>
    /// <param name="compilation">The compilation that <paramref name="typeSymbol"/> belongs to.</param>
    /// <param name="diagnostics">The resulting diagnostics, if any.</param>
    /// <returns>The extracted info for the current type, if possible.</returns>
    /// <remarks>If <paramref name="diagnostics"/> is empty, the returned info will always be ignored and no sources will be produced.</remarks>
    private protected abstract TInfo? ValidateTargetTypeAndGetInfo(INamedTypeSymbol typeSymbol, AttributeData attributeData, Compilation compilation, out ImmutableArray<DiagnosticInfo> diagnostics);

    /// <summary>
    /// Filters the <see cref="MemberDeclarationSyntax"/> nodes to generate from the input parsed tree.
    /// </summary>
    /// <param name="info">The <typeparamref name="TInfo"/> instance with the current processing info.</param>
    /// <param name="memberDeclarations">The input sequence of <see cref="MemberDeclarationSyntax"/> instances to generate.</param>
    /// <returns>A sequence of <see cref="MemberDeclarationSyntax"/> nodes to emit in the generated file.</returns>
    protected abstract ImmutableArray<MemberDeclarationSyntax> FilterDeclaredMembers(TInfo info, ImmutableArray<MemberDeclarationSyntax> memberDeclarations);
}

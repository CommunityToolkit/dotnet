// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for a given attribute type.
/// </summary>
/// <typeparam name="TInfo">The type of info gathered for each target type to process.</typeparam>
public abstract partial class TransitiveMembersGenerator<TInfo> : IIncrementalGenerator
{
    /// <summary>
    /// The fully qualified name of the attribute type to look for.
    /// </summary>
    private readonly string attributeType;

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
    /// <param name="attributeType">The fully qualified name of the attribute type to look for.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> instance to compare intermediate models.</param>
    private protected TransitiveMembersGenerator(string attributeType, IEqualityComparer<TInfo>? comparer = null)
    {
        this.attributeType = attributeType;
        this.comparer = comparer ?? EqualityComparer<TInfo>.Default;
        this.classDeclaration = Execute.LoadClassDeclaration(attributeType);

        Execute.ProcessMemberDeclarations(
            GetType(),
            this.classDeclaration.Members.ToImmutableArray(),
            out this.sealedMemberDeclarations,
            out this.nonSealedMemberDeclarations);
    }

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all class declarations
        IncrementalValuesProvider<INamedTypeSymbol> typeSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                static (context, _) => (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!);

        // Filter the types with the target attribute
        IncrementalValuesProvider<(INamedTypeSymbol Symbol, AttributeData AttributeData)> typeSymbolsWithAttributeData =
            typeSymbols
            .Select((item, _) => (
                Symbol: item,
                Attribute: item.GetAttributes().FirstOrDefault(a => a.AttributeClass?.HasFullyQualifiedName(this.attributeType) == true)))
            .Where(static item => item.Attribute is not null)!;

        // Transform the input data
        IncrementalValuesProvider<(INamedTypeSymbol Symbol, TInfo Info)> typeSymbolsWithInfo = GetInfo(context, typeSymbolsWithAttributeData);

        // Filter by language version
        context.FilterWithLanguageVersion(ref typeSymbolsWithInfo, LanguageVersion.CSharp8, UnsupportedCSharpLanguageVersionError);

        // Gather all generation info, and any diagnostics
        IncrementalValuesProvider<Result<(HierarchyInfo Hierarchy, bool IsSealed, TInfo Info)>> generationInfoWithErrors =
            typeSymbolsWithInfo.Select((item, _) =>
            {
                if (ValidateTargetType(item.Symbol, item.Info, out ImmutableArray<Diagnostic> diagnostics))
                {
                    return new Result<(HierarchyInfo, bool, TInfo)>(
                        (HierarchyInfo.From(item.Symbol), item.Symbol.IsSealed, item.Info),
                        ImmutableArray<Diagnostic>.Empty);
                }

                return new Result<(HierarchyInfo, bool, TInfo)>(default, diagnostics);
            });

        // Emit the diagnostic, if needed
        context.ReportDiagnostics(generationInfoWithErrors.Select(static (item, _) => item.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, bool IsSealed, TInfo Info)> generationInfo =
            generationInfoWithErrors
            .Where(static item => item.Errors.IsEmpty)
            .Select(static (item, _) => item.Value)
            .WithComparers(HierarchyInfo.Comparer.Default, EqualityComparer<bool>.Default, this.comparer);

        // Generate the required members
        context.RegisterSourceOutput(generationInfo, (context, item) =>
        {
            ImmutableArray<MemberDeclarationSyntax> sourceMemberDeclarations = item.IsSealed ? this.sealedMemberDeclarations : this.nonSealedMemberDeclarations;
            ImmutableArray<MemberDeclarationSyntax> filteredMemberDeclarations = FilterDeclaredMembers(item.Info, sourceMemberDeclarations);
            CompilationUnitSyntax compilationUnit = item.Hierarchy.GetCompilationUnit(filteredMemberDeclarations, this.classDeclaration.BaseList);

            context.AddSource(item.Hierarchy.FilenameHint, compilationUnit.ToFullString());
        });
    }

    /// <summary>
    /// Gathers info from a source <see cref="IncrementalValuesProvider{TValues}"/> input.
    /// </summary>
    /// <param name="context">The <see cref="IncrementalGeneratorInitializationContext"/> instance in use.</param>
    /// <param name="source">The source <see cref="IncrementalValuesProvider{TValues}"/> input.</param>
    /// <returns>A transformed <see cref="IncrementalValuesProvider{TValues}"/> instance with the gathered data.</returns>
    protected abstract IncrementalValuesProvider<(INamedTypeSymbol Symbol, TInfo Info)> GetInfo(
        IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<(INamedTypeSymbol Symbol, AttributeData AttributeData)> source);

    /// <summary>
    /// Validates a target type being processed.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="INamedTypeSymbol"/> instance for the target type.</param>
    /// <param name="info">The <typeparamref name="TInfo"/> instance with the current processing info.</param>
    /// <param name="diagnostics">The resulting diagnostics from the processing operation.</param>
    /// <returns>Whether or not the target type is valid and can be processed normally.</returns>
    protected abstract bool ValidateTargetType(INamedTypeSymbol typeSymbol, TInfo info, out ImmutableArray<Diagnostic> diagnostics);

    /// <summary>
    /// Filters the <see cref="MemberDeclarationSyntax"/> nodes to generate from the input parsed tree.
    /// </summary>
    /// <param name="info">The <typeparamref name="TInfo"/> instance with the current processing info.</param>
    /// <param name="memberDeclarations">The input sequence of <see cref="MemberDeclarationSyntax"/> instances to generate.</param>
    /// <returns>A sequence of <see cref="MemberDeclarationSyntax"/> nodes to emit in the generated file.</returns>
    protected abstract ImmutableArray<MemberDeclarationSyntax> FilterDeclaredMembers(TInfo info, ImmutableArray<MemberDeclarationSyntax> memberDeclarations);
}

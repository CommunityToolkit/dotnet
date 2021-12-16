// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for a given attribute type.
/// </summary>
/// <typeparam name="TInfo">The type of info gathered from parsed <see cref="AttributeData"/> instances.</typeparam>
public abstract partial class TransitiveMembersGenerator2<TInfo> : IIncrementalGenerator
    where TInfo : class
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
    /// Initializes a new instance of the <see cref="TransitiveMembersGenerator"/> class.
    /// </summary>
    /// <param name="attributeType">The fully qualified name of the attribute type to look for.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> instance to compare intermediate models.</param>
    private protected TransitiveMembersGenerator2(string attributeType, IEqualityComparer<TInfo>? comparer = null)
    {
        this.attributeType = attributeType;
        this.comparer = comparer ?? EqualityComparer<TInfo>.Default;

        string attributeTypeName = attributeType.Split('.').Last();
        string filename = $"CommunityToolkit.Mvvm.SourceGenerators.EmbeddedResources.{attributeTypeName.Replace("Attribute", string.Empty)}.cs";

        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
        using StreamReader reader = new(stream);

        string observableObjectSource = reader.ReadToEnd();
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(observableObjectSource);

        this.classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
    }

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Validate the language version
        IncrementalValueProvider<bool> isGeneratorSupported =
            context.ParseOptionsProvider
            .Select(static (item, _) => item is CSharpParseOptions { LanguageVersion: >= LanguageVersion.CSharp9 });

        // Get all class declarations
        IncrementalValuesProvider<INamedTypeSymbol> typeSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                static (context, _) => (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!)
            .Combine(isGeneratorSupported)
            .Where(static item => item.Right)
            .Select(static (item, _) => item.Left);

        // Filter the types with the target attribute
        IncrementalValuesProvider<(INamedTypeSymbol Symbol, TInfo Info)> typeSymbolsWithInfo =
            typeSymbols
            .Select((item, _) => (
                Symbol: item,
                Attribute: item.GetAttributes().FirstOrDefault(a => a.AttributeClass?.HasFullyQualifiedName(this.attributeType) == true)))
            .Where(static item => item.Attribute is not null)!
            .Select((item, _) => (item.Symbol, GetInfo(item.Attribute!)));

        // Gather all generation info, and any diagnostics
        IncrementalValuesProvider<Result<(HierarchyInfo Hierarchy, TInfo Info)>> generationInfoWithErrors =
            typeSymbolsWithInfo.Select((item, _) =>
            {
                if (ValidateTargetType(item.Symbol, item.Info, out ImmutableArray<Diagnostic> diagnostics))
                {
                    return new Result<(HierarchyInfo, TInfo)>(
                        (HierarchyInfo.From(item.Symbol), item.Info),
                        ImmutableArray<Diagnostic>.Empty);
                }

                return new Result<(HierarchyInfo Hierarchy, TInfo Info)>(default, diagnostics);
            });

        // Emit the diagnostic, if needed
        context.ReportDiagnostics(generationInfoWithErrors.Select(static (item, _) => item.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, TInfo Info)> generationInfo =
            generationInfoWithErrors
            .Where(static item => item.Errors.IsEmpty)
            .Select(static (item, _) => (item.Value.Hierarchy, item.Value.Info))
            .WithComparers(HierarchyInfo.Comparer.Default, this.comparer);

        // Generate the required members
        context.RegisterSourceOutput(generationInfo, (context, item) =>
        {
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations = FilterDeclaredMembers(item.Info, this.classDeclaration);
            CompilationUnitSyntax compilationUnit = item.Hierarchy.GetCompilationUnit(memberDeclarations, this.classDeclaration.BaseList);

            context.AddSource(
                hintName: $"{item.Hierarchy.FilenameHint}.cs",
                sourceText: SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
        });
    }

    /// <summary>
    /// Gets an info model from a retrieved <see cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="attributeData">The input <see cref="AttributeData"/> to get info from.</param>
    /// <returns>A <typeparamref name="TInfo"/> instance with data extracted from <paramref name="attributeData"/>.</returns>
    protected abstract TInfo GetInfo(AttributeData attributeData);

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
    /// <param name="classDeclaration">The input <see cref="ClassDeclarationSyntax"/> with the reference trees to copy.</param>
    /// <returns>A sequence of <see cref="MemberDeclarationSyntax"/> nodes to emit in the generated file.</returns>
    protected abstract ImmutableArray<MemberDeclarationSyntax> FilterDeclaredMembers(TInfo info, ClassDeclarationSyntax classDeclaration);
}

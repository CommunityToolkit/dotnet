// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Input.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for generating command properties from annotated methods.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class RelayCommandGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all method declarations with at least one attribute
        IncrementalValuesProvider<IMethodSymbol> methodSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax { Parent: ClassDeclarationSyntax, AttributeLists.Count: > 0 },
                static (context, _) => (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!);

        // Filter the methods using [RelayCommand]
        IncrementalValuesProvider<(IMethodSymbol Symbol, AttributeData Attribute)> methodSymbolsWithAttributeData =
            methodSymbols
            .Select(static (item, _) => (
                item,
                Attribute: item.GetAttributes().FirstOrDefault(a => a.AttributeClass?.HasFullyQualifiedName("global::CommunityToolkit.Mvvm.Input.RelayCommandAttribute") == true)))
            .Where(static item => item.Attribute is not null)!;

        // Filter by language version
        context.FilterWithLanguageVersion(ref methodSymbolsWithAttributeData, LanguageVersion.CSharp8, UnsupportedCSharpLanguageVersionError);

        // Gather info for all annotated command methods
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, Result<CommandInfo?> Info)> commandInfoWithErrors =
            methodSymbolsWithAttributeData
            .Select(static (item, _) =>
            {
                HierarchyInfo hierarchy = HierarchyInfo.From(item.Symbol.ContainingType);
                CommandInfo? commandInfo = Execute.GetInfo(item.Symbol, item.Attribute, out ImmutableArray<Diagnostic> diagnostics);

                return (hierarchy, new Result<CommandInfo?>(commandInfo, diagnostics));
            });

        // Output the diagnostics
        context.ReportDiagnostics(commandInfoWithErrors.Select(static (item, _) => item.Info.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, CommandInfo Info)> commandInfo =
            commandInfoWithErrors
            .Where(static item => item.Info.Value is not null)
            .Select(static (item, _) => (item.Hierarchy, item.Info.Value!))
            .WithComparers(HierarchyInfo.Comparer.Default, CommandInfo.Comparer.Default);

        // Generate the commands
        context.RegisterSourceOutput(commandInfo, static (context, item) =>
        {
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations = Execute.GetSyntax(item.Info);
            CompilationUnitSyntax compilationUnit = item.Hierarchy.GetCompilationUnit(memberDeclarations);

            context.AddSource($"{item.Hierarchy.FilenameHint}.{item.Info.MethodName}.g.cs", compilationUnit.GetText(Encoding.UTF8));
        });
    }
}

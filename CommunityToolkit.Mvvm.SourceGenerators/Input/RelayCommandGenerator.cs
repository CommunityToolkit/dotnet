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
        // Gather info for all annotated command methods (starting from method declarations with at least one attribute)
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, Result<CommandInfo?> Info)> commandInfoWithErrors =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax { Parent: ClassDeclarationSyntax, AttributeLists.Count: > 0 },
                static (context, token) =>
                {
                    if (!context.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
                    {
                        return default;
                    }

                    IMethodSymbol methodSymbol = (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!;

                    // Filter the methods using [RelayCommand]
                    if (!methodSymbol.TryGetAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.Input.RelayCommandAttribute", out AttributeData? attribute))
                    {
                        return default;
                    }

                    // Produce the incremental models
                    HierarchyInfo hierarchy = HierarchyInfo.From(methodSymbol.ContainingType);
                    CommandInfo? commandInfo = Execute.GetInfo(methodSymbol, attribute, out ImmutableArray<Diagnostic> diagnostics);

                    return (Hierarchy: hierarchy, new Result<CommandInfo?>(commandInfo, diagnostics));
                })
            .Where(static item => item.Hierarchy is not null);

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

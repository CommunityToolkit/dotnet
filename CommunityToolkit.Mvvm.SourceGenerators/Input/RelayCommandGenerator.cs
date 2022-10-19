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
            .ForAttributeWithMetadataName(
                "CommunityToolkit.Mvvm.Input.RelayCommandAttribute",
                static (node, _) => node is MethodDeclarationSyntax { Parent: ClassDeclarationSyntax, AttributeLists.Count: > 0 },
                static (context, token) =>
                {
                    if (!context.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
                    {
                        return default;
                    }

                    IMethodSymbol methodSymbol = (IMethodSymbol)context.TargetSymbol;

                    // Get the hierarchy info for the target symbol, and try to gather the command info
                    HierarchyInfo? hierarchy = HierarchyInfo.From(methodSymbol.ContainingType);

                    _ = Execute.TryGetInfo(methodSymbol, context.Attributes[0], out CommandInfo? commandInfo, out ImmutableArray<DiagnosticInfo> diagnostics);

                    return (Hierarchy: hierarchy, new Result<CommandInfo?>(commandInfo, diagnostics));
                })
            .Where(static item => item.Hierarchy is not null)!;

        // Output the diagnostics
        context.ReportDiagnostics(commandInfoWithErrors.Select(static (item, _) => item.Info.Errors.AsImmutableArray()));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, Result<CommandInfo> Info)> commandInfo =
            commandInfoWithErrors
            .Where(static item => item.Info.Value is not null)!;

        // Generate the commands
        context.RegisterSourceOutput(commandInfo, static (context, item) =>
        {
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations = Execute.GetSyntax(item.Info.Value);
            CompilationUnitSyntax compilationUnit = item.Hierarchy.GetCompilationUnit(memberDeclarations);

            context.AddSource($"{item.Hierarchy.FilenameHint}.{item.Info.Value.MethodName}.g.cs", compilationUnit.GetText(Encoding.UTF8));
        });
    }
}

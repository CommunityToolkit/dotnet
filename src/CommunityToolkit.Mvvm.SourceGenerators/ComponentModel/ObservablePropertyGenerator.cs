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

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for the <c>ObservablePropertyAttribute</c> type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class ObservablePropertyGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Gather info for all annotated fields
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, Result<PropertyInfo?> Info)> propertyInfoWithErrors =
            context.ForAttributeWithMetadataNameAndOptions(
                "CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute",
                Execute.IsCandidatePropertyDeclaration,
                static (context, token) =>
                {
                    MemberDeclarationSyntax memberSyntax = Execute.GetCandidateMemberDeclaration(context.TargetNode);

                    // Validate that the candidate is valid for the current compilation
                    if (!Execute.IsCandidateValidForCompilation(memberSyntax, context.SemanticModel))
                    {
                        return default;
                    }

                    // Validate the symbol as well before doing any work
                    if (!Execute.IsCandidateSymbolValid(context.TargetSymbol))
                    {
                        return default;
                    }

                    token.ThrowIfCancellationRequested();

                    // Get the hierarchy info for the target symbol, and try to gather the property info
                    HierarchyInfo hierarchy = HierarchyInfo.From(context.TargetSymbol.ContainingType);

                    token.ThrowIfCancellationRequested();

                    _ = Execute.TryGetInfo(
                        memberSyntax,
                        context.TargetSymbol,
                        context.SemanticModel,
                        context.GlobalOptions,
                        token,
                        out PropertyInfo? propertyInfo,
                        out ImmutableArray<DiagnosticInfo> diagnostics);

                    token.ThrowIfCancellationRequested();

                    return (Hierarchy: hierarchy, new Result<PropertyInfo?>(propertyInfo, diagnostics));
                })
            .Where(static item => item.Hierarchy is not null);

        // Output the diagnostics
        context.ReportDiagnostics(propertyInfoWithErrors.Select(static (item, _) => item.Info.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, Result<PropertyInfo> Info)> propertyInfo =
            propertyInfoWithErrors
            .Where(static item => item.Info.Value is not null)!;

        // Split and group by containing type
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, EquatableArray<PropertyInfo> Properties)> groupedPropertyInfo =
            propertyInfo
            .GroupBy(static item => item.Left, static item => item.Right.Value);

        // Generate the requested properties and methods
        context.RegisterSourceOutput(groupedPropertyInfo, static (context, item) =>
        {
            // Generate all member declarations for the current type
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations =
                item.Properties
                .Select(Execute.GetPropertySyntax)
                .Concat(item.Properties.Select(Execute.GetOnPropertyChangeMethodsSyntax).SelectMany(static l => l))
                .ToImmutableArray();

            // Insert all members into the same partial type declaration
            CompilationUnitSyntax compilationUnit = item.Hierarchy.GetCompilationUnit(memberDeclarations);

            context.AddSource($"{item.Hierarchy.FilenameHint}.g.cs", compilationUnit);
        });

        // Gather all property changing names
        IncrementalValueProvider<EquatableArray<string>> propertyChangingNames =
            propertyInfo
            .SelectMany(static (item, _) => item.Info.Value.PropertyChangingNames)
            .Collect()
            .Select(static (item, _) => item.Distinct().ToImmutableArray().AsEquatableArray());

        // Generate the cached property changing names
        context.RegisterSourceOutput(propertyChangingNames, static (context, item) =>
        {
            CompilationUnitSyntax? compilationUnit = Execute.GetKnownPropertyChangingArgsSyntax(item);

            if (compilationUnit is not null)
            {
                context.AddSource("__KnownINotifyPropertyChangingArgs.g.cs", compilationUnit);
            }
        });

        // Gather all property changed names
        IncrementalValueProvider<EquatableArray<string>> propertyChangedNames =
            propertyInfo
            .SelectMany(static (item, _) => item.Info.Value.PropertyChangedNames)
            .Collect()
            .Select(static (item, _) => item.Distinct().ToImmutableArray().AsEquatableArray());

        // Generate the cached property changed names
        context.RegisterSourceOutput(propertyChangedNames, static (context, item) =>
        {
            CompilationUnitSyntax? compilationUnit = Execute.GetKnownPropertyChangedArgsSyntax(item);

            if (compilationUnit is not null)
            {
                context.AddSource("__KnownINotifyPropertyChangedArgs.g.cs", compilationUnit);
            }
        });
    }
}

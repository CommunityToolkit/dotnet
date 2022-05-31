// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

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
        // Get all field declarations with at least one attribute
        IncrementalValuesProvider<IFieldSymbol> fieldSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is FieldDeclarationSyntax { Parent: ClassDeclarationSyntax or RecordDeclarationSyntax, AttributeLists.Count: > 0 },
                static (context, _) => ((FieldDeclarationSyntax)context.Node).Declaration.Variables.Select(v => (IFieldSymbol)context.SemanticModel.GetDeclaredSymbol(v)!))
            .SelectMany(static (item, _) => item);

        // Filter the fields using [ObservableProperty]
        IncrementalValuesProvider<IFieldSymbol> fieldSymbolsWithAttribute =
            fieldSymbols
            .Where(static item => item.HasAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute"));

        // Get diagnostics for fields using [NotifyPropertyChangedFor], [NotifyCanExecuteChangedFor], [AlsoBroadcastChange] and [AlsoValidateProperty], but not [ObservableProperty]
        IncrementalValuesProvider<Diagnostic> fieldSymbolsWithOrphanedDependentAttributeWithErrors =
            fieldSymbols
            .Where(static item =>
                (item.HasAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.NotifyPropertyChangedForAttribute") ||
                 item.HasAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.NotifyCanExecuteChangedForAttribute") ||
                 item.HasAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.AlsoBroadcastChangeAttribute") ||
                 item.HasAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.AlsoValidatePropertyAttribute")) &&
                 !item.HasAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute"))
            .Select(static (item, _) => Execute.GetDiagnosticForFieldWithOrphanedDependentAttributes(item));

        // Output the diagnostics
        context.ReportDiagnostics(fieldSymbolsWithOrphanedDependentAttributeWithErrors);

        // Filter by language version
        context.FilterWithLanguageVersion(ref fieldSymbolsWithAttribute, LanguageVersion.CSharp8, UnsupportedCSharpLanguageVersionError);

        // Gather info for all annotated fields
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, Result<PropertyInfo?> Info)> propertyInfoWithErrors =
            fieldSymbolsWithAttribute
            .Select(static (item, _) =>
            {
                HierarchyInfo hierarchy = HierarchyInfo.From(item.ContainingType);
                PropertyInfo? propertyInfo = Execute.TryGetInfo(item, out ImmutableArray<Diagnostic> diagnostics);

                return (hierarchy, new Result<PropertyInfo?>(propertyInfo, diagnostics));
            });

        // Output the diagnostics
        context.ReportDiagnostics(propertyInfoWithErrors.Select(static (item, _) => item.Info.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, PropertyInfo Info)> propertyInfo =
            propertyInfoWithErrors
            .Select(static (item, _) => (item.Hierarchy, Info: item.Info.Value))
            .Where(static item => item.Info is not null)!
            .WithComparers(HierarchyInfo.Comparer.Default, PropertyInfo.Comparer.Default);

        // Split and group by containing type
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, ImmutableArray<PropertyInfo> Properties)> groupedPropertyInfo =
            propertyInfo
            .GroupBy(HierarchyInfo.Comparer.Default)
            .WithComparers(HierarchyInfo.Comparer.Default, PropertyInfo.Comparer.Default.ForImmutableArray());

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

            context.AddSource($"{item.Hierarchy.FilenameHint}.g.cs", compilationUnit.GetText(Encoding.UTF8));
        });

        // Gather all property changing names
        IncrementalValueProvider<ImmutableArray<string>> propertyChangingNames =
            propertyInfo
            .SelectMany(static (item, _) => item.Info.PropertyChangingNames)
            .Collect()
            .Select(static (item, _) => item.Distinct().ToImmutableArray())
            .WithComparer(EqualityComparer<string>.Default.ForImmutableArray());

        // Generate the cached property changing names
        context.RegisterSourceOutput(propertyChangingNames, static (context, item) =>
        {
            CompilationUnitSyntax? compilationUnit = Execute.GetKnownPropertyChangingArgsSyntax(item);

            if (compilationUnit is not null)
            {
                context.AddSource("__KnownINotifyPropertyChangingArgs.g.cs", compilationUnit.GetText(Encoding.UTF8));
            }
        });

        // Gather all property changed names
        IncrementalValueProvider<ImmutableArray<string>> propertyChangedNames =
            propertyInfo
            .SelectMany(static (item, _) => item.Info.PropertyChangedNames)
            .Collect()
            .Select(static (item, _) => item.Distinct().ToImmutableArray())
            .WithComparer(EqualityComparer<string>.Default.ForImmutableArray());

        // Generate the cached property changed names
        context.RegisterSourceOutput(propertyChangedNames, static (context, item) =>
        {
            CompilationUnitSyntax? compilationUnit = Execute.GetKnownPropertyChangedArgsSyntax(item);

            if (compilationUnit is not null)
            {
                context.AddSource("__KnownINotifyPropertyChangedArgs.g.cs", compilationUnit.GetText(Encoding.UTF8));
            }
        });
    }
}

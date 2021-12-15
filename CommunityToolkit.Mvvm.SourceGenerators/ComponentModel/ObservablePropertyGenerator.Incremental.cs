// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for the <c>ObservablePropertyAttribute</c> type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class ObservablePropertyGenerator2 : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Validate the language version. Note that we're emitting this diagnostic in each generator (excluding the one
        // only emitting the nullability annotation attributes if missing) so that the diagnostic is emitted only when
        // users are using one of these generators, and not by default as soon as they add a reference to the MVVM Toolkit.
        // This ensures that users not using any of the source generators won't be broken when upgrading to this new version.
        IncrementalValueProvider<bool> isGeneratorSupported =
            context.ParseOptionsProvider
            .Select(static (item, _) => item is CSharpParseOptions { LanguageVersion: >= LanguageVersion.CSharp9 });

        // Emit the diagnostic, if needed
        context.ReportDiagnosticsIsNotSupported(isGeneratorSupported, Diagnostic.Create(UnsupportedCSharpLanguageVersionError, null));

        // Get all field declarations with at least one attribute
        IncrementalValuesProvider<IFieldSymbol> fieldSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is FieldDeclarationSyntax { Parent: ClassDeclarationSyntax or RecordDeclarationSyntax, AttributeLists.Count: > 0 },
                static (context, _) => ((FieldDeclarationSyntax)context.Node).Declaration.Variables.Select(v => (IFieldSymbol)context.SemanticModel.GetDeclaredSymbol(v)!))
            .Combine(isGeneratorSupported)
            .Where(static item => item.Right)
            .SelectMany(static (item, _) => item.Left);

        // Filter the fields using [ObservableProperty]
        IncrementalValuesProvider<IFieldSymbol> fieldSymbolsWithAttribute =
            fieldSymbols
            .Where(static item => item.GetAttributes().Any(a => a.AttributeClass?.HasFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") == true));

        // Gather info for all annotated fields
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, Result<PropertyInfo> Info)> propertyInfoWithErrors =
            fieldSymbolsWithAttribute
            .Select(static (item, _) =>
            {
                HierarchyInfo hierarchy = HierarchyInfo.From(item.ContainingType);
                PropertyInfo propertyInfo = Execute.GetInfo(item);

                return (hierarchy, new Result<PropertyInfo>(propertyInfo, ImmutableArray<Diagnostic>.Empty));
            });

        // Output the diagnostics
        context.ReportDiagnostics(propertyInfoWithErrors.Select(static (item, _) => item.Info.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, PropertyInfo Info)> propertyInfo =
            propertyInfoWithErrors
            .Select(static (item, _) => (item.Hierarchy, item.Info.Value))
            .WithComparers(HierarchyInfo.Comparer.Default, PropertyInfo.Comparer.Default);

        // Split and group by containing type
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, ImmutableArray<PropertyInfo> Properties)> groupedPropertyInfo =
            propertyInfo
            .Collect()
            .GroupBy(HierarchyInfo.Comparer.Default)
            .WithComparers(HierarchyInfo.Comparer.Default, PropertyInfo.Comparer.Default.ForImmutableArray());

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
                context.AddSource(
                    hintName: "__KnownINotifyPropertyChangingArgs.cs",
                    sourceText: SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
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
        context.RegisterSourceOutput(propertyChangingNames, static (context, item) =>
        {
            CompilationUnitSyntax? compilationUnit = Execute.GetKnownPropertyChangedArgsSyntax(item);

            if (compilationUnit is not null)
            {
                context.AddSource(
                    hintName: "__KnownINotifyPropertyChangedArgs.cs",
                    sourceText: SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
            }
        });
    }
}

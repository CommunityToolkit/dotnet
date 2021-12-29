// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.SourceGenerators.Input.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for message registration without relying on compiled LINQ expressions.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class IMessengerRegisterAllGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all class declarations
        IncrementalValuesProvider<INamedTypeSymbol> typeSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                static (context, _) => (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!);

        // Get the target IRecipient<TMessage> interfaces and filter out other types
        IncrementalValuesProvider<(INamedTypeSymbol Type, ImmutableArray<INamedTypeSymbol> Interfaces)> typeAndInterfaceSymbols =
            typeSymbols
            .Select(static (item, _) => (item, Interfaces: Execute.GetInterfaces(item)))
            .Where(static item => !item.Interfaces.IsEmpty);

        // Get the recipient info for all target types
        IncrementalValuesProvider<RecipientInfo> recipientInfo =
            typeAndInterfaceSymbols
            .Select(static (item, _) => Execute.GetInfo(item.Type, item.Interfaces))
            .WithComparer(RecipientInfo.Comparer.Default);

        // Check whether the header file is needed
        IncrementalValueProvider<bool> isHeaderFileNeeded =
            recipientInfo
            .Collect()
            .Select(static (item, _) => item.Length > 0);

        // Generate the header file with the attributes
        context.RegisterImplementationSourceOutput(isHeaderFileNeeded, static (context, item) =>
        {
            CompilationUnitSyntax compilationUnit = Execute.GetSyntax();

            context.AddSource(
                hintName: "__IMessengerExtensions.cs",
                sourceText: SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
        });

        // Generate the class with all registration methods
        context.RegisterImplementationSourceOutput(recipientInfo, static (context, item) =>
        {
            CompilationUnitSyntax compilationUnit = Execute.GetSyntax(item);

            context.AddSource(
                hintName: $"{item.FilenameHint}.cs",
                sourceText: SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
        });
    }
}

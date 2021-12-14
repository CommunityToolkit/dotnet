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
        // Validate the language version
        IncrementalValueProvider<bool> isGeneratorSupported =
            context.ParseOptionsProvider
            .Select(static (item, _) => item is CSharpParseOptions { LanguageVersion: >= LanguageVersion.CSharp8 });

        // Get all class declarations
        IncrementalValuesProvider<INamedTypeSymbol> typeSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                static (context, _) => (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!)
            .Combine(isGeneratorSupported)
            .Where(static item => item.Right)
            .Select(static (item, _) => item.Left);

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

        // Get the generated methods for all recipients
        IncrementalValueProvider<ImmutableArray<MemberDeclarationSyntax>> methodDeclarations =
            recipientInfo
            .Select(static (item, _) => Execute.GetSyntax(item))
            .SelectMany(static (item, _) => item)
            .Collect();

        // Generate the class with all registration methods
        context.RegisterImplementationSourceOutput(methodDeclarations, static (context, item) =>
        {
            CompilationUnitSyntax compilationUnit = Execute.GetSyntax(item);

            context.AddSource(
                hintName: "__IMessengerExtensions.cs",
                sourceText: SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
        });
    }
}

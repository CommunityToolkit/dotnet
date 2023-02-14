// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Messaging.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        // Get the recipient info for all target types
        IncrementalValuesProvider<RecipientInfo> recipientInfo =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax classDeclaration && classDeclaration.HasOrPotentiallyHasBaseTypes(),
                static (context, token) =>
                {
                    if (!context.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
                    {
                        return default;
                    }

                    INamedTypeSymbol typeSymbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!;

                    // The type must be a non-abstract, non-generic type (just like with the ObservableValidator generator)
                    if (typeSymbol is not { IsAbstract: false, IsGenericType: false })
                    {
                        return default;
                    }

                    // This pipeline step also needs to filter out duplicate recipient definitions (it might happen if a
                    // recipient has partial declarations). To do this, all pairs of class declarations and associated
                    // symbols are gathered, and then only the pair where the class declaration is the first syntax
                    // reference for the associated symbol is kept.
                    if (!context.Node.IsFirstSyntaxDeclarationForSymbol(typeSymbol))
                    {
                        return default;
                    }

                    ImmutableArray<INamedTypeSymbol> interfaceSymbols = Execute.GetInterfaces(typeSymbol);

                    // Check that the type implements at least one IRecipient<TMessage> interface
                    if (interfaceSymbols.IsEmpty)
                    {
                        return default;
                    }

                    return Execute.GetInfo(typeSymbol, interfaceSymbols);
                })
            .Where(static item => item is not null)!;

        // Check whether the header file is needed
        IncrementalValueProvider<bool> isHeaderFileNeeded =
            recipientInfo
            .Collect()
            .Select(static (item, _) => item.Length > 0);

        // Check whether [DynamicallyAccessedMembers] is available
        IncrementalValueProvider<bool> isDynamicallyAccessedMembersAttributeAvailable =
            context.CompilationProvider
            .Select(static (item, _) => item.HasAccessibleTypeWithMetadataName("System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute"));

        // Gather the conditional flag and attribute availability
        IncrementalValueProvider<(bool IsHeaderFileNeeded, bool IsDynamicallyAccessedMembersAttributeAvailable)> headerFileInfo =
            isHeaderFileNeeded
            .Combine(isDynamicallyAccessedMembersAttributeAvailable);

        // Generate the header file with the attributes
        context.RegisterConditionalImplementationSourceOutput(headerFileInfo, static (context, item) =>
        {
            CompilationUnitSyntax compilationUnit = Execute.GetSyntax(item);

            context.AddSource("__IMessengerExtensions.g.cs", compilationUnit);
        });

        // Generate the class with all registration methods
        context.RegisterImplementationSourceOutput(recipientInfo, static (context, item) =>
        {
            CompilationUnitSyntax compilationUnit = Execute.GetSyntax(item);

            context.AddSource($"{item.FilenameHint}.g.cs", compilationUnit);
        });
    }
}

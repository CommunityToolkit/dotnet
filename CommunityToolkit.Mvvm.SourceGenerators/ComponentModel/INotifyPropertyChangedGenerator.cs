// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Input.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for the <c>INotifyPropertyChangedAttribute</c> type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class INotifyPropertyChangedGenerator : TransitiveMembersGenerator<INotifyPropertyChangedInfo>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="INotifyPropertyChangedGenerator"/> class.
    /// </summary>
    public INotifyPropertyChangedGenerator()
        : base("global::CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute")
    {
    }

    /// <inheritdoc/>
    protected override IncrementalValuesProvider<(INamedTypeSymbol Symbol, INotifyPropertyChangedInfo Info)> GetInfo(
        IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<(INamedTypeSymbol Symbol, AttributeData AttributeData)> source)
    {
        static INotifyPropertyChangedInfo GetInfo(INamedTypeSymbol typeSymbol, AttributeData attributeData)
        {
            bool includeAdditionalHelperMethods = attributeData.GetNamedArgument<bool>("IncludeAdditionalHelperMethods", true);

            return new(includeAdditionalHelperMethods);
        }

        return source.Select(static (item, _) => (item.Symbol, GetInfo(item.Symbol, item.AttributeData)));
    }

    /// <inheritdoc/>
    protected override bool ValidateTargetType(INamedTypeSymbol typeSymbol, INotifyPropertyChangedInfo info, out ImmutableArray<Diagnostic> diagnostics)
    {
        ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();

        // Check if the type already implements INotifyPropertyChanged
        if (typeSymbol.AllInterfaces.Any(i => i.HasFullyQualifiedName("global::System.ComponentModel.INotifyPropertyChanged")))
        {
            builder.Add(DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError, typeSymbol, typeSymbol);

            diagnostics = builder.ToImmutable();

            return false;
        }

        diagnostics = builder.ToImmutable();

        return true;
    }

    /// <inheritdoc/>
    protected override ImmutableArray<MemberDeclarationSyntax> FilterDeclaredMembers(INotifyPropertyChangedInfo info, ImmutableArray<MemberDeclarationSyntax> memberDeclarations)
    {
        // If requested, only include the event and the basic methods to raise it, but not the additional helpers
        if (!info.IncludeAdditionalHelperMethods)
        {
            return memberDeclarations.Where(static member => member
                is EventFieldDeclarationSyntax
                or MethodDeclarationSyntax { Identifier.ValueText: "OnPropertyChanged" }).ToImmutableArray();
        }

        return memberDeclarations;
    }
}

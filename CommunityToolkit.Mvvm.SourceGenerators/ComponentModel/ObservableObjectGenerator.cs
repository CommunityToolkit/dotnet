// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for the <c>ObservableObjectAttribute</c> type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class ObservableObjectGenerator : TransitiveMembersGenerator<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableObjectGenerator"/> class.
    /// </summary>
    public ObservableObjectGenerator()
        : base("CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute")
    {
    }

    /// <inheritdoc/>
    private protected override int ValidateTargetTypeAndGetInfo(INamedTypeSymbol typeSymbol, AttributeData attributeData, Compilation compilation, out ImmutableArray<DiagnosticInfo> diagnostics)
    {
        diagnostics = ImmutableArray<DiagnosticInfo>.Empty;

        // Check if the type already implements INotifyPropertyChanged...
        if (typeSymbol.AllInterfaces.Any(i => i.HasFullyQualifiedName("global::System.ComponentModel.INotifyPropertyChanged")))
        {
            diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError, typeSymbol, typeSymbol));

            goto End;
        }

        // ...or INotifyPropertyChanging
        if (typeSymbol.AllInterfaces.Any(i => i.HasFullyQualifiedName("global::System.ComponentModel.INotifyPropertyChanging")))
        {
            diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError, typeSymbol, typeSymbol));

            goto End;
        }

        // Check if the type uses [INotifyPropertyChanged] or [ObservableObject] already (in the type hierarchy too)
        if (typeSymbol.InheritsAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute") ||
            typeSymbol.HasOrInheritsAttributeWithFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute"))
        {
            diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(InvalidAttributeCombinationForObservableObjectAttributeError, typeSymbol, typeSymbol));

            goto End;
        }

        End:
        return 0;
    }

    /// <inheritdoc/>
    protected override ImmutableArray<MemberDeclarationSyntax> FilterDeclaredMembers(int info, ImmutableArray<MemberDeclarationSyntax> memberDeclarations)
    {
        return memberDeclarations;
    }
}

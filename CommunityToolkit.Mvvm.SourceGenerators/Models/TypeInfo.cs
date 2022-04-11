// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators.Models;

/// <summary>
/// A model describing a type info in a type hierarchy.
/// </summary>
/// <param name="QualifiedName">The qualified name for the type.</param>
/// <param name="Kind">The type of the type in the hierarchy.</param>
internal sealed record TypeInfo(string QualifiedName, TypeKind Kind)
{
    /// <summary>
    /// Creates a <see cref="TypeDeclarationSyntax"/> instance for the current info.
    /// </summary>
    /// <returns>A <see cref="TypeDeclarationSyntax"/> instance for the current info.</returns>
    public TypeDeclarationSyntax GetSyntax()
    {
        // Create the partial type declaration with the kind.
        // This code produces a class declaration as follows:
        //
        // <TYPE_KIND> <TYPE_NAME>
        // {
        // }
        return Kind switch
        {
            TypeKind.Struct => StructDeclaration(QualifiedName),
            TypeKind.Interface => InterfaceDeclaration(QualifiedName),
            _ => ClassDeclaration(QualifiedName)
        };
    }
}

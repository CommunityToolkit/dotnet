// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="SyntaxKind"/> type.
/// </summary>
internal static class SyntaxKindExtensions
{
    /// <summary>
    /// Converts a <see cref="SyntaxKind"/> value to either "field" or "property" based on the kind.
    /// </summary>
    /// <param name="kind">The input <see cref="SyntaxKind"/> value.</param>
    /// <returns>Either "field" or "property" based on <paramref name="kind"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="kind"/> is neither <see cref="SyntaxKind.FieldDeclaration"/> nor <see cref="SyntaxKind.PropertyDeclaration"/>.</exception>
    public static string ToFieldOrPropertyKeyword(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.FieldDeclaration => "field",
            SyntaxKind.PropertyDeclaration => "property",
            _ => throw new ArgumentException($"Unsupported syntax kind '{kind}' for field or property keyword conversion."),
        };
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="Accessibility"/> type.
/// </summary>
internal static class AccessibilityExtensions
{
    /// <summary>
    /// Converts a given <see cref="Accessibility"/> value to the equivalent <see cref="SyntaxTokenList"/>."/>
    /// </summary>
    /// <param name="accessibility">The input <see cref="Accessibility"/> value to convert.</param>
    /// <returns>The <see cref="SyntaxTokenList"/> representing the modifiers for <paramref name="accessibility"/>.</returns>
    public static SyntaxTokenList ToSyntaxTokenList(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.NotApplicable => TokenList(),
            Accessibility.Private => TokenList(Token(SyntaxKind.PrivateKeyword)),
            Accessibility.ProtectedAndInternal => TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ProtectedKeyword)),
            Accessibility.Protected => TokenList(Token(SyntaxKind.ProtectedKeyword)),
            Accessibility.Internal => TokenList(Token(SyntaxKind.InternalKeyword)),
            Accessibility.ProtectedOrInternal => TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.InternalKeyword)),
            Accessibility.Public => TokenList(Token(SyntaxKind.PublicKeyword)),
            _ => TokenList()
        };
    }
}

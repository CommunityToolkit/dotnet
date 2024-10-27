// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="SyntaxTokenList"/> type.
/// </summary>
internal static class SyntaxTokenListExtensions
{
    /// <summary>
    /// Checks whether a given <see cref="SyntaxTokenList"/> value contains any accessibility modifiers.
    /// </summary>
    /// <param name="syntaxList">The input <see cref="SyntaxTokenList"/> value to check.</param>
    /// <returns>Whether <paramref name="syntaxList"/> contains any accessibility modifiers.</returns>
    public static bool ContainsAnyAccessibilityModifiers(this SyntaxTokenList syntaxList)
    {
        foreach (SyntaxToken token in syntaxList)
        {
            if (SyntaxFacts.IsAccessibilityModifier(token.Kind()))
            {
                return true;
            }
        }

        return false;
    }
}

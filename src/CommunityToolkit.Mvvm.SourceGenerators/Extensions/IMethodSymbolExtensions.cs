// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="IMethodSymbol"/> type.
/// </summary>
internal static class IMethodSymbolExtensions
{
    /// <summary>
    /// Checks whether all input symbols are <see cref="IMethodSymbol"/>-s in the same override hierarchy.
    /// </summary>
    /// <param name="symbols">The input <see cref="ISymbol"/> set to check.</param>
    /// <returns>Whether all input symbols are <see cref="IMethodSymbol"/>-s in the same override hierarchy.</returns>
    public static bool AreAllInSameOverriddenMethodHierarchy(this ImmutableArray<ISymbol> symbols)
    {
        IMethodSymbol? baseSymbol = null;

        // Look for the base method
        foreach (ISymbol currentSymbol in symbols)
        {
            // If any input symbol is not a method, we can stop right away
            if (currentSymbol is not IMethodSymbol methodSymbol)
            {
                return false;
            }

            if (methodSymbol.IsVirtual)
            {
                // If we already found a base method, all methods can't possibly be in the same hierarchy
                if (baseSymbol is not null)
                {
                    return false;
                }

                baseSymbol = methodSymbol;
            }
        }

        // If we didn't find any, stop here
        if (baseSymbol is null)
        {
            return false;
        }

        // Verify all methods are in the same tree
        foreach (ISymbol currentSymbol in symbols)
        {
            IMethodSymbol methodSymbol = (IMethodSymbol)currentSymbol;

            // Ignore the base method
            if (SymbolEqualityComparer.Default.Equals(methodSymbol, baseSymbol))
            {
                continue;
            }

            // If the current method isn't an override, then fail
            if (methodSymbol.OverriddenMethod is not { } overriddenMethod)
            {
                return false;
            }

            // The current method must be overriding another one in the set
            if (!symbols.Any(symbol => SymbolEqualityComparer.Default.Equals(symbol, overriddenMethod)))
            {
                return false;
            }
        }

        return true;
    }
}

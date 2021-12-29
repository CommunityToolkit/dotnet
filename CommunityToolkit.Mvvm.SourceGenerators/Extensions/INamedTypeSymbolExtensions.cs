// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="INamedTypeSymbol"/> type.
/// </summary>
internal static class INamedTypeSymbolExtensions
{
    /// <summary>
    /// Gets a valid filename for a given <see cref="INamedTypeSymbol"/> instance.
    /// </summary>
    /// <param name="symbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <returns>The full metadata name for <paramref name="symbol"/> that is also a valid filename.</returns>
    public static string GetFullMetadataNameForFileName(this INamedTypeSymbol symbol)
    {
        static StringBuilder BuildFrom(ISymbol? symbol, StringBuilder builder)
        {
            return symbol switch
            {
                INamespaceSymbol ns when ns.IsGlobalNamespace => builder,
                INamespaceSymbol ns when ns.ContainingNamespace is { IsGlobalNamespace: false }
                    => BuildFrom(ns.ContainingNamespace, builder.Insert(0, $".{ns.MetadataName}")),
                ITypeSymbol ts when ts.ContainingType is ISymbol pt => BuildFrom(pt, builder.Insert(0, $"+{ts.MetadataName}")),
                ITypeSymbol ts when ts.ContainingNamespace is ISymbol pn => BuildFrom(pn, builder.Insert(0, $".{ts.MetadataName}")),
                ISymbol => BuildFrom(symbol.ContainingSymbol, builder.Insert(0, symbol.MetadataName)),
                _ => builder
            };
        }

        // Build the full metadata name by concatenating the metadata names of all symbols from the input
        // one to the outermost namespace, if any. Additionally, the ` and + symbols need to be replaced
        // to avoid errors when generating code. This is a known issue with source generators not accepting
        // those characters at the moment, see: https://github.com/dotnet/roslyn/issues/58476.
        return BuildFrom(symbol, new StringBuilder(256)).ToString().Replace('`', '-').Replace('+', '.');
    }

    /// <summary>
    /// Checks whether or not a given <see cref="INamedTypeSymbol"/> inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="INamedTypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> inherits from <paramref name="name"/>.</returns>
    public static bool InheritsFrom(this INamedTypeSymbol typeSymbol, string name)
    {
        INamedTypeSymbol? baseType = typeSymbol.BaseType;

        while (baseType != null)
        {
            if (baseType.HasFullyQualifiedName(name))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
}

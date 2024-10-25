// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="SymbolKindExtensions"/> type.
/// </summary>
internal static class SymbolKindExtensions
{
    /// <summary>
    /// Converts a <see cref="SymbolKind"/> value to either "field" or "property" based on the kind.
    /// </summary>
    /// <param name="kind">The input <see cref="SymbolKind"/> value.</param>
    /// <returns>Either "field" or "property" based on <paramref name="kind"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="kind"/> is neither <see cref="SymbolKind.Field"/> nor <see cref="SymbolKind.Property"/>.</exception>
    public static string ToFieldOrPropertyKeyword(this SymbolKind kind)
    {
        return kind switch
        {
            SymbolKind.Field => "field",
            SymbolKind.Property => "property",
            _ => throw new ArgumentException($"Unsupported symbol kind '{kind}' for field or property keyword conversion."),
        };
    }
}

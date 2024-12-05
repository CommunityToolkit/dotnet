// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="SyntaxKind"/> type.
/// </summary>
internal static class SyntaxKindExtensions
{
    /// <summary>
    /// Converts an <see cref="ImmutableArray{T}"/> of <see cref="SyntaxKind"/> values to one of their underlying type.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray{T}"/> value.</param>
    /// <returns>The resulting <see cref="ImmutableArray{T}"/> of <see cref="ushort"/> values.</returns>
    public static ImmutableArray<ushort> AsUnderlyingType(this ImmutableArray<SyntaxKind> array)
    {
        ushort[]? underlyingArray = (ushort[]?)(object?)Unsafe.As<ImmutableArray<SyntaxKind>, SyntaxKind[]?>(ref array);

        return Unsafe.As<ushort[]?, ImmutableArray<ushort>>(ref underlyingArray);
    }

    /// <summary>
    /// Converts an <see cref="ImmutableArray{T}"/> of <see cref="ushort"/> values to one of their real type.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray{T}"/> value.</param>
    /// <returns>The resulting <see cref="ImmutableArray{T}"/> of <see cref="SyntaxKind"/> values.</returns>
    public static ImmutableArray<SyntaxKind> FromUnderlyingType(this ImmutableArray<ushort> array)
    {
        SyntaxKind[]? typedArray = (SyntaxKind[]?)(object?)Unsafe.As<ImmutableArray<ushort>, ushort[]?>(ref array);

        return Unsafe.As<SyntaxKind[]?, ImmutableArray<SyntaxKind>>(ref typedArray);
    }

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

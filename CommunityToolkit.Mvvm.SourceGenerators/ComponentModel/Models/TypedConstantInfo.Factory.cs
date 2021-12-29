// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <inheritdoc/>
partial record TypedConstantInfo
{
    /// <summary>
    /// Creates a new <see cref="TypedConstantInfo"/> instance from a given <see cref="TypedConstant"/> value.
    /// </summary>
    /// <param name="arg">The input <see cref="TypedConstant"/> value.</param>
    /// <returns>A <see cref="TypedConstantInfo"/> instance representing <paramref name="arg"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the input argument is not valid.</exception>
    public static TypedConstantInfo From(TypedConstant arg)
    {
        if (arg.IsNull)
        {
            return new Null();
        }

        if (arg.Kind == TypedConstantKind.Array)
        {
            string elementTypeName = ((IArrayTypeSymbol)arg.Type!).ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            ImmutableArray<TypedConstantInfo> items = arg.Values.Select(From).ToImmutableArray();

            return new Array(elementTypeName, items);
        }

        return (arg.Kind, arg.Value) switch
        {
            (TypedConstantKind.Primitive, string text) => new Primitive.String(text),
            (TypedConstantKind.Primitive, bool flag) => new Primitive.Boolean(flag),
            (TypedConstantKind.Primitive, object value) => value switch
            {
                byte b => new Primitive.Of<byte>(b),
                char c => new Primitive.Of<char>(c),
                double d => new Primitive.Of<double>(d),
                float f => new Primitive.Of<float>(f),
                int i => new Primitive.Of<int>(i),
                long l => new Primitive.Of<long>(l),
                sbyte sb => new Primitive.Of<sbyte>(sb),
                short sh => new Primitive.Of<short>(sh),
                uint ui => new Primitive.Of<uint>(ui),
                ulong ul => new Primitive.Of<ulong>(ul),
                ushort ush => new Primitive.Of<ushort>(ush),
                _ => throw new ArgumentException("Invalid primitive type")
            },
            (TypedConstantKind.Type, ITypeSymbol type) => new Type(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
            (TypedConstantKind.Enum, object value) => new Enum(arg.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), value),
            _ => throw new ArgumentException("Invalid typed constant type"),
        };
    }
}

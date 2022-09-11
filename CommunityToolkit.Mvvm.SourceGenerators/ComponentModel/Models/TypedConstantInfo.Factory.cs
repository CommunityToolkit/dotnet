// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

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

    /// <summary>
    /// Creates a new <see cref="TypedConstantInfo"/> instance from a given <see cref="IOperation"/> value.
    /// </summary>
    /// <param name="arg">The input <see cref="IOperation"/> value.</param>
    /// <returns>A <see cref="TypedConstantInfo"/> instance representing <paramref name="arg"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the input argument is not valid.</exception>
    public static TypedConstantInfo From(IOperation arg)
    {
        if (arg.ConstantValue.HasValue)
        {
            // Enum values are constant but need to be checked explicitly in this case
            if (arg.Type?.TypeKind is TypeKind.Enum)
            {
                return new Enum(arg.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), arg.ConstantValue.Value!);
            }

            // Handle all other constant literals normally
            return arg.ConstantValue.Value switch
            {
                null => new Null(),
                string text => new Primitive.String(text),
                bool flag => new Primitive.Boolean(flag),
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
            };
        }

        if (arg is ITypeOfOperation typeOfOperation)
        {
            return new Type(typeOfOperation.TypeOperand.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        if (arg is IArrayCreationOperation arrayCreationOperation)
        {
            string? elementTypeName = ((IArrayTypeSymbol?)arg.Type)?.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // If the element type is not available (since the attribute wasn't checked), just default to object
            elementTypeName ??= "object";

            ImmutableArray<TypedConstantInfo> items = ImmutableArray<TypedConstantInfo>.Empty; // TODO

            return new Array(elementTypeName, items);
        }

        throw new ArgumentException("Invalid attribute argument value");
    }
}

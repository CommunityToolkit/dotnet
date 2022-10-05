// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    /// <param name="operation">The input <see cref="IOperation"/> value.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> that was used to retrieve <paramref name="operation"/>.</param>
    /// <param name="expression">The <see cref="ExpressionSyntax"/> that <paramref name="operation"/> was retrieved from.</param>
    /// <param name="token">The cancellation token for the current operation.</param>
    /// <returns>A <see cref="TypedConstantInfo"/> instance representing <paramref name="operation"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the input argument is not valid.</exception>
    public static TypedConstantInfo From(
        IOperation operation,
        SemanticModel semanticModel,
        ExpressionSyntax expression,
        CancellationToken token)
    {
        if (operation.ConstantValue.HasValue)
        {
            // Enum values are constant but need to be checked explicitly in this case
            if (operation.Type?.TypeKind is TypeKind.Enum)
            {
                return new Enum(operation.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), operation.ConstantValue.Value!);
            }

            // Handle all other constant literals normally
            return operation.ConstantValue.Value switch
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

        if (operation is ITypeOfOperation typeOfOperation)
        {
            return new Type(typeOfOperation.TypeOperand.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        if (operation is IArrayCreationOperation)
        {
            string? elementTypeName = ((IArrayTypeSymbol?)operation.Type)?.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // If the element type is not available (since the attribute wasn't checked), just default to object
            elementTypeName ??= "object";

            InitializerExpressionSyntax? initializerExpression =
                (expression as ImplicitArrayCreationExpressionSyntax)?.Initializer
                ?? (expression as ArrayCreationExpressionSyntax)?.Initializer;

            // No initializer found, just return an empty array
            if (initializerExpression is null)
            {
                return new Array(elementTypeName, ImmutableArray<TypedConstantInfo>.Empty);
            }

            using ImmutableArrayBuilder<TypedConstantInfo>.Lease items = ImmutableArrayBuilder<TypedConstantInfo>.Rent();

            // Enumerate all array elements and extract serialized info for them
            foreach (ExpressionSyntax initializationExpression in initializerExpression.Expressions)
            {
                if (semanticModel.GetOperation(initializationExpression, token) is not IOperation initializationOperation)
                {
                    throw new ArgumentException("Failed to retrieve an operation for the current array element");
                }

                items.Add(From(initializationOperation, semanticModel, initializationExpression, token));
            }

            return new Array(elementTypeName, items.ToImmutable());
        }

        throw new ArgumentException("Invalid attribute argument value");
    }
}

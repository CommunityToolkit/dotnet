// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
    public static TypedConstantInfo Create(TypedConstant arg)
    {
        if (arg.IsNull)
        {
            return new Null();
        }

        if (arg.Kind == TypedConstantKind.Array)
        {
            string elementTypeName = ((IArrayTypeSymbol)arg.Type!).ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            ImmutableArray<TypedConstantInfo> items = arg.Values.Select(Create).ToImmutableArray();

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
    /// <param name="info">The resulting <see cref="TypedConstantInfo"/> instance, if available</param>
    /// <returns>Whether a resulting <see cref="TypedConstantInfo"/> instance could be created.</returns>
    /// <exception cref="ArgumentException">Thrown if the input argument is not valid.</exception>
    public static bool TryCreate(
        IOperation operation,
        SemanticModel semanticModel,
        ExpressionSyntax expression,
        CancellationToken token,
        [NotNullWhen(true)] out TypedConstantInfo? info)
    {
        if (operation.ConstantValue.HasValue)
        {
            // Enum values are constant but need to be checked explicitly in this case
            if (operation.Type?.TypeKind is TypeKind.Enum)
            {
                info = new Enum(operation.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), operation.ConstantValue.Value!);

                return true;
            }

            // Handle all other constant literals normally
            info = operation.ConstantValue.Value switch
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

            return true;
        }

        if (operation is ITypeOfOperation typeOfOperation)
        {
            info = new Type(typeOfOperation.TypeOperand.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

            return true;
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
                info = new Array(elementTypeName, ImmutableArray<TypedConstantInfo>.Empty);

                return true;
            }

            using ImmutableArrayBuilder<TypedConstantInfo> items = ImmutableArrayBuilder<TypedConstantInfo>.Rent();

            // Enumerate all array elements and extract serialized info for them
            foreach (ExpressionSyntax initializationExpression in initializerExpression.Expressions)
            {
                if (semanticModel.GetOperation(initializationExpression, token) is not IOperation initializationOperation)
                {
                    goto Failure;
                }

                if (!TryCreate(initializationOperation, semanticModel, initializationExpression, token, out TypedConstantInfo? elementInfo))
                {
                    goto Failure;
                }

                items.Add(elementInfo);
            }

            info = new Array(elementTypeName, items.ToImmutable());

            return true;
        }

        Failure:
        info = null;

        return false;
    }
}

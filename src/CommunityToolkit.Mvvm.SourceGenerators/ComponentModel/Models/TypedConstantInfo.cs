// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model representing a typed constant item.
/// </summary>
/// <remarks>This model is fully serializable and comparable.</remarks>
internal abstract partial record TypedConstantInfo
{
    /// <summary>
    /// Gets an <see cref="ExpressionSyntax"/> instance representing the current constant.
    /// </summary>
    /// <returns>The <see cref="ExpressionSyntax"/> instance representing the current constant.</returns>
    public abstract ExpressionSyntax GetSyntax();

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing an array.
    /// </summary>
    /// <param name="ElementTypeName">The type name for array elements.</param>
    /// <param name="Items">The sequence of contained elements.</param>
    public sealed record Array(string ElementTypeName, EquatableArray<TypedConstantInfo> Items) : TypedConstantInfo
    {
        /// <inheritdoc/>
        public override ExpressionSyntax GetSyntax()
        {
            return
                ArrayCreationExpression(
                ArrayType(IdentifierName(ElementTypeName))
                .AddRankSpecifiers(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))))
                .WithInitializer(InitializerExpression(SyntaxKind.ArrayInitializerExpression)
                .AddExpressions(Items.Select(static c => c.GetSyntax()).ToArray()));
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing a primitive value.
    /// </summary>
    public abstract record Primitive : TypedConstantInfo
    {
        /// <summary>
        /// A <see cref="TypedConstantInfo"/> type representing a <see cref="string"/> value.
        /// </summary>
        /// <param name="Value">The input <see cref="string"/> value.</param>
        public sealed record String(string Value) : TypedConstantInfo
        {
            /// <inheritdoc/>
            public override ExpressionSyntax GetSyntax()
            {
                return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(Value));
            }
        }

        /// <summary>
        /// A <see cref="TypedConstantInfo"/> type representing a <see cref="bool"/> value.
        /// </summary>
        /// <param name="Value">The input <see cref="bool"/> value.</param>
        public sealed record Boolean(bool Value) : TypedConstantInfo
        {
            /// <inheritdoc/>
            public override ExpressionSyntax GetSyntax()
            {
                return LiteralExpression(Value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);
            }
        }

        /// <summary>
        /// A <see cref="TypedConstantInfo"/> type representing a generic primitive value.
        /// </summary>
        /// <typeparam name="T">The primitive type.</typeparam>
        /// <param name="Value">The input primitive value.</param>
        public sealed record Of<T>(T Value) : TypedConstantInfo
            where T : unmanaged, IEquatable<T>
        {
            /// <inheritdoc/>
            public override ExpressionSyntax GetSyntax()
            {
                return LiteralExpression(SyntaxKind.NumericLiteralExpression, Value switch
                {
                    byte b => Literal(b),
                    char c => Literal(c),

                    // For doubles, we need to manually format it and always add the trailing "D" suffix.
                    // This ensures that the correct type is produced if the expression was assigned to
                    // an object (eg. the literal was used in an attribute object parameter/property).
                    double d => Literal(d.ToString("R", CultureInfo.InvariantCulture) + "D", d),

                    // For floats, Roslyn will automatically add the "F" suffix, so no extra work is needed
                    float f => Literal(f),
                    int i => Literal(i),
                    long l => Literal(l),
                    sbyte sb => Literal(sb),
                    short sh => Literal(sh),
                    uint ui => Literal(ui),
                    ulong ul => Literal(ul),
                    ushort ush => Literal(ush),
                    _ => throw new ArgumentException("Invalid primitive type")
                });
            }
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing a type.
    /// </summary>
    /// <param name="TypeName">The input type name.</param>
    public sealed record Type(string TypeName) : TypedConstantInfo
    {
        /// <inheritdoc/>
        public override ExpressionSyntax GetSyntax()
        {
            return TypeOfExpression(IdentifierName(TypeName));
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing an enum value.
    /// </summary>
    /// <param name="TypeName">The enum type name.</param>
    /// <param name="Value">The boxed enum value.</param>
    public sealed record Enum(string TypeName, object Value) : TypedConstantInfo
    {
        /// <inheritdoc/>
        public override ExpressionSyntax GetSyntax()
        {
            return
                CastExpression(
                    IdentifierName(TypeName),
                    LiteralExpression(SyntaxKind.NumericLiteralExpression, ParseToken(Value.ToString())));
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing a <see langword="null"/> value.
    /// </summary>
    public sealed record Null : TypedConstantInfo
    {
        /// <inheritdoc/>
        public override ExpressionSyntax GetSyntax()
        {
            return LiteralExpression(SyntaxKind.NullLiteralExpression);
        }
    }
}

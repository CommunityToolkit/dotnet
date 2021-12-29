// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model representing a typed constant item.
/// </summary>
/// <remarks>This model is fully serializeable and comparable.</remarks>
internal abstract partial record TypedConstantInfo
{
    /// <summary>
    /// Gets an <see cref="ExpressionSyntax"/> instance representing the current constant.
    /// </summary>
    /// <returns>The <see cref="ExpressionSyntax"/> instance representing the current constant.</returns>
    public abstract ExpressionSyntax GetSyntax();

    /// <summary>
    /// Checks whether the current instance is the same as an input one.
    /// </summary>
    /// <param name="other">The <see cref="TypedConstantInfo"/> instance to compare to.</param>
    /// <returns>Whether or not the two instances are the same.</returns>
    /// <remarks>This method differs from <see cref="Equals(TypedConstantInfo?)"/> in that it checks for deep equality.</remarks>
    protected abstract bool IsEqualTo(TypedConstantInfo other);

    /// <summary>
    /// Adds the current instance to an incremental <see cref="HashCode"/> value.
    /// </summary>
    /// <param name="hashCode">The target <see cref="HashCode"/> value.</param>
    protected abstract void AddToHashCode(ref HashCode hashCode);

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing an array.
    /// </summary>
    /// <param name="ElementTypeName">The type name for array elements.</param>
    /// <param name="Items">The sequence of contained elements.</param>
    public sealed record Array(string ElementTypeName, ImmutableArray<TypedConstantInfo> Items) : TypedConstantInfo
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

        /// <inheritdoc/>
        protected override bool IsEqualTo(TypedConstantInfo other)
        {
            if (other is Array array &&
                ElementTypeName == array.ElementTypeName &&
                Items.Length == array.Items.Length)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (!Items[i].IsEqualTo(array.Items[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override void AddToHashCode(ref HashCode hashCode)
        {
            hashCode.Add(ElementTypeName);

            foreach (TypedConstantInfo item in Items)
            {
                item.AddToHashCode(ref hashCode);
            }
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

            /// <inheritdoc/>
            protected override bool IsEqualTo(TypedConstantInfo other)
            {
                return
                    other is String @string &&
                    Value == @string.Value;
            }

            /// <inheritdoc/>
            protected override void AddToHashCode(ref HashCode hashCode)
            {
                hashCode.Add(Value);
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

            /// <inheritdoc/>
            protected override bool IsEqualTo(TypedConstantInfo other)
            {
                return
                    other is Boolean @bool &&
                    Value == @bool.Value;
            }

            /// <inheritdoc/>
            protected override void AddToHashCode(ref HashCode hashCode)
            {
                hashCode.Add(Value);
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
                    double d => Literal(d),
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

            /// <inheritdoc/>
            protected override bool IsEqualTo(TypedConstantInfo other)
            {
                return
                    other is Of<T> box &&
                    Value.Equals(box.Value);
            }

            /// <inheritdoc/>
            protected override void AddToHashCode(ref HashCode hashCode)
            {
                hashCode.Add(Value);
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

        /// <inheritdoc/>
        protected override bool IsEqualTo(TypedConstantInfo other)
        {
            return
                other is Type type &&
                TypeName == type.TypeName;
        }

        /// <inheritdoc/>
        protected override void AddToHashCode(ref HashCode hashCode)
        {
            hashCode.Add(TypeName);
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

        /// <inheritdoc/>
        protected override bool IsEqualTo(TypedConstantInfo other)
        {
            return
                other is Enum @enum &&
                TypeName == @enum.TypeName &&
                Value.Equals(@enum.Value);
        }

        /// <inheritdoc/>
        protected override void AddToHashCode(ref HashCode hashCode)
        {
            hashCode.Add(TypeName);
            hashCode.Add(Value);
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

        /// <inheritdoc/>
        protected override bool IsEqualTo(TypedConstantInfo other)
        {
            return other is Null;
        }

        /// <inheritdoc/>
        protected override void AddToHashCode(ref HashCode hashCode)
        {
            hashCode.Add((object?)null);
        }
    }
}

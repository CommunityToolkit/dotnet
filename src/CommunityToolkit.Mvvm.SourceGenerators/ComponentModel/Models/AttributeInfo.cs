// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model representing an attribute declaration.
/// </summary>
/// <param name="TypeName">The type name of the attribute.</param>
/// <param name="ConstructorArgumentInfo">The <see cref="TypedConstantInfo"/> values for all constructor arguments for the attribute.</param>
/// <param name="NamedArgumentInfo">The <see cref="TypedConstantInfo"/> values for all named arguments for the attribute.</param>
internal sealed record AttributeInfo(
    string TypeName,
    EquatableArray<TypedConstantInfo> ConstructorArgumentInfo,
    EquatableArray<(string Name, TypedConstantInfo Value)> NamedArgumentInfo)
{
    /// <summary>
    /// Creates a new <see cref="AttributeInfo"/> instance from a given <see cref="AttributeData"/> value.
    /// </summary>
    /// <param name="attributeData">The input <see cref="AttributeData"/> value.</param>
    /// <returns>A <see cref="AttributeInfo"/> instance representing <paramref name="attributeData"/>.</returns>
    public static AttributeInfo Create(AttributeData attributeData)
    {
        string typeName = attributeData.AttributeClass!.GetFullyQualifiedName();

        using ImmutableArrayBuilder<TypedConstantInfo> constructorArguments = ImmutableArrayBuilder<TypedConstantInfo>.Rent();
        using ImmutableArrayBuilder<(string, TypedConstantInfo)> namedArguments = ImmutableArrayBuilder<(string, TypedConstantInfo)>.Rent();

        // Get the constructor arguments
        foreach (TypedConstant typedConstant in attributeData.ConstructorArguments)
        {
            constructorArguments.Add(TypedConstantInfo.Create(typedConstant));
        }

        // Get the named arguments
        foreach (KeyValuePair<string, TypedConstant> namedConstant in attributeData.NamedArguments)
        {
            namedArguments.Add((namedConstant.Key, TypedConstantInfo.Create(namedConstant.Value)));
        }

        return new(
            typeName,
            constructorArguments.ToImmutable(),
            namedArguments.ToImmutable());
    }

    /// <summary>
    /// Creates a new <see cref="AttributeInfo"/> instance from a given syntax node.
    /// </summary>
    /// <param name="typeSymbol">The symbol for the attribute type.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current run.</param>
    /// <param name="arguments">The sequence of <see cref="AttributeArgumentSyntax"/> instances to process.</param>
    /// <param name="token">The cancellation token for the current operation.</param>
    /// <param name="info">The resulting <see cref="AttributeInfo"/> instance, if available</param>
    /// <returns>Whether a resulting <see cref="AttributeInfo"/> instance could be created.</returns>
    public static bool TryCreate(
        INamedTypeSymbol typeSymbol,
        SemanticModel semanticModel,
        IEnumerable<AttributeArgumentSyntax> arguments,
        CancellationToken token,
        [NotNullWhen(true)] out AttributeInfo? info)
    {
        string typeName = typeSymbol.GetFullyQualifiedName();

        using ImmutableArrayBuilder<TypedConstantInfo> constructorArguments = ImmutableArrayBuilder<TypedConstantInfo>.Rent();
        using ImmutableArrayBuilder<(string, TypedConstantInfo)> namedArguments = ImmutableArrayBuilder<(string, TypedConstantInfo)>.Rent();

        foreach (AttributeArgumentSyntax argument in arguments)
        {
            // The attribute expression has to have an available operation to extract information from
            if (semanticModel.GetOperation(argument.Expression, token) is not IOperation operation)
            {
                continue;
            }

            // Try to get the info for the current argument
            if (!TypedConstantInfo.TryCreate(operation, semanticModel, argument.Expression, token, out TypedConstantInfo? argumentInfo))
            {
                info = null;

                return false;
            }

            // Try to get the identifier name if the current expression is a named argument expression. If it
            // isn't, then the expression is a normal attribute constructor argument, so no extra work is needed.
            if (argument.NameEquals is { Name.Identifier.ValueText: string argumentName })
            {
                namedArguments.Add((argumentName, argumentInfo));
            }
            else
            {
                constructorArguments.Add(argumentInfo);
            }
        }

        info = new AttributeInfo(
            typeName,
            constructorArguments.ToImmutable(),
            namedArguments.ToImmutable());

        return true;
    }

    /// <summary>
    /// Gets an <see cref="AttributeSyntax"/> instance representing the current value.
    /// </summary>
    /// <returns>The <see cref="ExpressionSyntax"/> instance representing the current value.</returns>
    public AttributeSyntax GetSyntax()
    {
        // Gather the constructor arguments
        IEnumerable<AttributeArgumentSyntax> arguments =
            ConstructorArgumentInfo
            .Select(static arg => AttributeArgument(arg.GetSyntax()));

        // Gather the named arguments
        IEnumerable<AttributeArgumentSyntax> namedArguments =
            NamedArgumentInfo.Select(static arg =>
                AttributeArgument(arg.Value.GetSyntax())
                .WithNameEquals(NameEquals(IdentifierName(arg.Name))));

        return Attribute(IdentifierName(TypeName), AttributeArgumentList(SeparatedList(arguments.Concat(namedArguments))));
    }
}

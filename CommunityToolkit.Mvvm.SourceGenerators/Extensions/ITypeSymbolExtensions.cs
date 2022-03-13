// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="ITypeSymbol"/> type.
/// </summary>
internal static class ITypeSymbolExtensions
{
    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> inherits from <paramref name="name"/>.</returns>
    public static bool InheritsFromFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
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

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> implements an interface with a specied name.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for interface implementation.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an interface with the specified name.</returns>
    public static bool HasInterfaceWithFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
    {
        foreach (INamedTypeSymbol interfaceType in typeSymbol.AllInterfaces)
        {
            if (interfaceType.HasFullyQualifiedName(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits a specified attribute.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="predicate">The predicate used to match available attributes.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute matching <paramref name="predicate"/>.</returns>
    public static bool HasOrInheritsAttribute(this ITypeSymbol typeSymbol, Func<AttributeData, bool> predicate)
    {
        for (ITypeSymbol? currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.GetAttributes().Any(predicate))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits a specified attribute.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The name of the attribute to look for.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute with the specified type name.</returns>
    public static bool HasOrInheritsAttributeWithFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
    {
        for (ITypeSymbol? currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.HasAttributeWithFullyQualifiedName(name))
            {
                return true;
            }
        }

        return false;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="Compilation"/> type.
/// </summary>
internal static class CompilationExtensions
{
    /// <summary>
    /// <para>
    /// Checks whether or not a type with a specified metadata name is accessible from a given <see cref="Compilation"/> instance.
    /// </para>
    /// <para>
    /// This method enumerates candidate type symbols to find a match in the following order:
    /// <list type="number">
    ///   <item><description>
    ///     If only one type with the given name is found within the compilation and its referenced assemblies, check its accessibility.
    ///   </description></item>
    ///   <item><description>
    ///     If the current <paramref name="compilation"/> defines the symbol, check its accessibility.
    ///   </description></item>
    ///   <item><description>
    ///     Otherwise, check whether the type exists and is accessible from any of the referenced assemblies.
    ///   </description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/> to consider for analysis.</param>
    /// <param name="fullyQualifiedMetadataName">The fully-qualified metadata type name to find.</param>
    /// <returns>Whether a type with the specified metadata name can be accessed from the given compilation.</returns>
    public static bool HasAccessibleTypeWithMetadataName(this Compilation compilation, string fullyQualifiedMetadataName)
    {
        // Try to get the unique type with this name
        INamedTypeSymbol? type = compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);

        // If there is only a single matching symbol, check its accessibility
        if (type is not null)
        {
            return type.CanBeAccessedFrom(compilation.Assembly);
        }

        // Otherwise, try to get the unique type with this name originally defined in 'compilation'
        type ??= compilation.Assembly.GetTypeByMetadataName(fullyQualifiedMetadataName);

        if (type is not null)
        {
            return type.CanBeAccessedFrom(compilation.Assembly);
        }

        // Otherwise, check whether the type is defined and accessible from any of the referenced assemblies
        foreach (IModuleSymbol module in compilation.Assembly.Modules)
        {
            foreach (IAssemblySymbol referencedAssembly in module.ReferencedAssemblySymbols)
            {
                if (referencedAssembly.GetTypeByMetadataName(fullyQualifiedMetadataName) is not INamedTypeSymbol currentType)
                {
                    continue;
                }

                switch (currentType.GetEffectiveAccessibility())
                {
                    case Accessibility.Public:
                    case Accessibility.Internal when referencedAssembly.GivesAccessTo(compilation.Assembly):
                        return true;
                    default:
                        continue;
                }
            }
        }

        return false;
    }
}

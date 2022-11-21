// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for necessary nullability attributes.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class NullabilityAttributesGenerator : IIncrementalGenerator
{
    /// <summary>
    /// The <c>System.Diagnostics.CodeAnalysis.NotNullAttribute</c> metadata name.
    /// </summary>
    private const string NotNullAttributeMetadataName = "System.Diagnostics.CodeAnalysis.NotNullAttribute";

    /// <summary>
    /// The <c>System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute</c> metadata name.
    /// </summary>
    private const string NotNullIfNotNullAttributeMetadataName = "System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Check that the target attributes are not available in the consuming project. To ensure that this
        // works fine both in .NET (Core) and .NET Standard implementations, we also need to check that the
        // target types are declared as public (we assume that in this case those types are from the BCL).
        // This avoids issues on .NET Standard with Roslyn also seeing internal types from referenced assemblies.

        // Check whether [NotNull] is not available
        IncrementalValueProvider<bool> isNotNullAttributeNotAvailable =
            context.CompilationProvider
            .Select(static (item, _) => !item.HasAccessibleTypeWithMetadataName(NotNullAttributeMetadataName));

        // Generate the [NotNull] type
        context.RegisterConditionalSourceOutput(isNotNullAttributeNotAvailable, static context =>
        {
            string source = LoadAttributeSourceWithMetadataName(NotNullAttributeMetadataName);

            context.AddSource($"{NotNullAttributeMetadataName}.g.cs", source);
        });

        // Check whether [NotNullIfNotNull] is not available
        IncrementalValueProvider<bool> isNotNullIfNotNullAttributeNotAvailable =
            context.CompilationProvider
            .Select(static (item, _) => !item.HasAccessibleTypeWithMetadataName(NotNullIfNotNullAttributeMetadataName));

        // Generate the [NotNullIfNotNull] type
        context.RegisterConditionalSourceOutput(isNotNullIfNotNullAttributeNotAvailable, static context =>
        {
            string source = LoadAttributeSourceWithMetadataName(NotNullIfNotNullAttributeMetadataName);

            context.AddSource($"{NotNullIfNotNullAttributeMetadataName}.g.cs", source);
        });
    }

    /// <summary>
    /// Gets the generated source for a specified attribute.
    /// </summary>
    private static string LoadAttributeSourceWithMetadataName(string typeFullName)
    {
        string typeName = typeFullName.Split('.').Last();
        string filename = $"{typeName}.cs";

        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
        StreamReader reader = new(stream);

        return reader.ReadToEnd();
    }
}

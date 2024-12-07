// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error when <c>[RelayCommand]</c> is used on a method inside a type with <c>[GeneratedBindableCustomProperty]</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatibleAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatible);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // This analyzer is only enabled when CsWinRT is also used
            if (!context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.IsUsingWindowsRuntimePack())
            {
                return;
            }

            // Get the symbols for [RelayCommand] and [GeneratedBindableCustomProperty]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.Input.RelayCommandAttribute") is not INamedTypeSymbol relayCommandSymbol ||
                context.Compilation.GetTypeByMetadataName("WinRT.GeneratedBindableCustomPropertyAttribute") is not INamedTypeSymbol generatedBindableCustomPropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // Ensure we do have a valid method with a containing type we can reference
                if (context.Symbol is not IMethodSymbol { ContainingType: INamedTypeSymbol typeSymbol } methodSymbol)
                {
                    return;
                }

                // If the method is not using [RelayCommand], we can skip it
                if (!methodSymbol.TryGetAttributeWithType(relayCommandSymbol, out AttributeData? relayCommandAttribute))
                {
                    return;
                }

                // If the containing type is not using [GeneratedBindableCustomProperty], we can also skip it
                if (!typeSymbol.TryGetAttributeWithType(generatedBindableCustomPropertySymbol, out AttributeData? generatedBindableCustomPropertyAttribute))
                {
                    return;
                }

                (_, string propertyName) = RelayCommandGenerator.Execute.GetGeneratedFieldAndPropertyNames(methodSymbol);

                if (DoesGeneratedBindableCustomPropertyAttributeIncludePropertyName(generatedBindableCustomPropertyAttribute, propertyName))
                {
                    // Actually warn if the generated command would've been included by the generator
                    context.ReportDiagnostic(Diagnostic.Create(
                        WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatible,
                        methodSymbol.GetLocationFromAttributeDataOrDefault(relayCommandAttribute),
                        methodSymbol));
                }
            }, SymbolKind.Method);
        });
    }

    /// <summary>
    /// Checks whether a generated property with a given name would be included by the [GeneratedBindableCustomProperty] generator.
    /// </summary>
    /// <param name="attributeData">The input <see cref="AttributeData"/> value for the [GeneratedBindableCustomProperty] attribute.</param>
    /// <param name="propertyName">The target generated property name to check.</param>
    /// <returns>Whether <paramref name="propertyName"/> would be included by the [GeneratedBindableCustomProperty] generator.</returns>
    internal static bool DoesGeneratedBindableCustomPropertyAttributeIncludePropertyName(AttributeData attributeData, string propertyName)
    {
        // Make sure we have a valid list of property names to explicitly include.
        // If that is not the case, we consider all properties as included by default.
        if (attributeData.ConstructorArguments is not [{ IsNull: false, Kind: TypedConstantKind.Array, Type: IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_String }, Values: var names }, ..])
        {
            return true;
        }

        // Simply match the input collection of target property names
        foreach (TypedConstant propertyValue in names)
        {
            if (propertyValue is { IsNull: false, Type.SpecialType: SpecialType.System_String, Value: string targetName } && targetName == propertyName)
            {
                return true;
            }
        }

        // No matches, we can consider the property as not included
        return false;
    }
}

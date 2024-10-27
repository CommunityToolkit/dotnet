// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates an error when a field or property with <c>[ObservableProperty]</c> is not valid (special cases)
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidGeneratedPropertyObservablePropertyAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(InvalidObservablePropertyError);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the symbol for [ObservableProperty] and the event args we need
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol ||
                context.Compilation.GetTypeByMetadataName("System.ComponentModel.PropertyChangedEventArgs") is not INamedTypeSymbol propertyChangedEventArgsSymbol ||
                context.Compilation.GetTypeByMetadataName("System.ComponentModel.PropertyChangingEventArgs") is not INamedTypeSymbol propertyChangingEventArgsSymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // Validate that we do have a field or a property
                if (context.Symbol is not (IFieldSymbol or IPropertySymbol))
                {
                    return;
                }

                // Ensure we do have the [ObservableProperty] attribute
                if (!context.Symbol.HasAttributeWithType(observablePropertySymbol))
                {
                    return;
                }

                ITypeSymbol propertyType = ObservablePropertyGenerator.Execute.GetPropertyType(context.Symbol);
                string propertyName = ObservablePropertyGenerator.Execute.GetGeneratedPropertyName(context.Symbol);

                // Same logic as 'IsGeneratedPropertyInvalid' in the generator
                if (propertyName == "Property")
                {
                    // Check for collisions with the generated helpers and the property, only happens with these 3 types
                    if (propertyType.SpecialType == SpecialType.System_Object ||
                        propertyType.HasOrInheritsFromType(propertyChangedEventArgsSymbol) ||
                        propertyType.HasOrInheritsFromType(propertyChangingEventArgsSymbol))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            InvalidObservablePropertyError,
                            context.Symbol.Locations.FirstOrDefault(),
                            context.Symbol.Kind.ToFieldOrPropertyKeyword(),
                            context.Symbol.ContainingType,
                            context.Symbol.Name));
                    }
                }
            }, SymbolKind.Field, SymbolKind.Property);
        });
    }
}

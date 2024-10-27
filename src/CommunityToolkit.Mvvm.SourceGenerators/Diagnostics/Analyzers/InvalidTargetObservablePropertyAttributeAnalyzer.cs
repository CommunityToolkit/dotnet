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
/// A diagnostic analyzer that generates an error when a field or property with <c>[ObservableProperty]</c> is not a valid target.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidTargetObservablePropertyAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(InvalidContainingTypeForObservablePropertyMemberError);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the required symbols for the analyzer
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol ||
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableObject") is not INamedTypeSymbol observableObjectSymbol ||
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute") is not INamedTypeSymbol observableObjectAttributeSymbol ||
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute") is not INamedTypeSymbol notifyPropertyChangedAttributeSymbol)
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
                if (!context.Symbol.TryGetAttributeWithType(observablePropertySymbol, out AttributeData? attributeDataobservablePropertyAttribute))
                {
                    return;
                }

                // Same logic as in 'IsTargetTypeValid' in the generator
                bool isObservableObject = context.Symbol.ContainingType.InheritsFromType(observableObjectSymbol);
                bool hasObservableObjectAttribute = context.Symbol.ContainingType.HasOrInheritsAttributeWithType(observableObjectAttributeSymbol);
                bool hasINotifyPropertyChangedAttribute = context.Symbol.ContainingType.HasOrInheritsAttributeWithType(notifyPropertyChangedAttributeSymbol);

                // Emit the diagnostic if the target is not valid
                if (!isObservableObject && !hasObservableObjectAttribute && !hasINotifyPropertyChangedAttribute)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                       InvalidContainingTypeForObservablePropertyMemberError,
                       context.Symbol.Locations.FirstOrDefault(),
                       context.Symbol.Kind.ToFieldOrPropertyKeyword(),
                       context.Symbol.ContainingType,
                       context.Symbol.Name));
                }
            }, SymbolKind.Field, SymbolKind.Property);
        });
    }
}

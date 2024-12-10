// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_12_0_OR_GREATER

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates a suggestion whenever <c>[ObservableProperty]</c> is used on a semi-auto property when a partial property could be used instead.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseObservablePropertyOnSemiAutoPropertyAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(UseObservablePropertyOnSemiAutoProperty);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Using [ObservableProperty] on partial properties is only supported when using C# preview.
            // As such, if that is not the case, return immediately, as no diagnostic should be produced.
            if (!context.Compilation.IsLanguageVersionPreview())
            {
                return;
            }

            // Get the symbol for [ObservableProperty] and ObservableObject
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is not INamedTypeSymbol observablePropertySymbol ||
                context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableObject") is not INamedTypeSymbol observableObjectSymbol)
            {
                return;
            }

            // Get the symbol for the SetProperty<T> method as well
            if (!TryGetSetPropertyMethodSymbol(observableObjectSymbol, out IMethodSymbol? setPropertySymbol))
            {
                return;
            }

            context.RegisterSymbolStartAction(context =>
            {
                // We only care about types that could derive from ObservableObject
                if (context.Symbol is not INamedTypeSymbol { IsStatic: false, IsReferenceType: true, BaseType.SpecialType: not SpecialType.System_Object } typeSymbol)
                {
                    return;
                }

                // If the type does not derive from ObservableObject, ignore it
                if (!typeSymbol.InheritsFromType(observableObjectSymbol))
                {
                    return;
                }

                Dictionary<IPropertySymbol, bool[]> propertyMap = new(SymbolEqualityComparer.Default);

                // Crawl all members to discover properties that might be of interest
                foreach (ISymbol memberSymbol in typeSymbol.GetMembers())
                {
                    // We're only looking for properties that might be valid candidates for conversion
                    if (memberSymbol is not IPropertySymbol
                        {
                            IsStatic: false,
                            IsPartialDefinition: false,
                            PartialDefinitionPart: null,
                            PartialImplementationPart: null,
                            ReturnsByRef: false,
                            ReturnsByRefReadonly: false,
                            Type.IsRefLikeType: false,
                            GetMethod: not null,
                            SetMethod.IsInitOnly: false
                        } propertySymbol)
                    {
                        continue;
                    }

                    // We can safely ignore properties that already have [ObservableProperty]
                    if (typeSymbol.HasAttributeWithType(observablePropertySymbol))
                    {
                        continue;
                    }

                    // Track the property for later
                    propertyMap.Add(propertySymbol, new bool[2]);
                }

                // We want to process both accessors, where we specifically need both the syntax
                // and their semantic model to verify what they're doing. We can use a code callback.
                context.RegisterOperationBlockAction(context =>
                {
                    // Make sure the current symbol is a property accessor
                    if (context.OwningSymbol is not IMethodSymbol { MethodKind: MethodKind.PropertyGet or MethodKind.PropertySet, AssociatedSymbol: IPropertySymbol propertySymbol })
                    {
                        return;
                    }

                    // If so, check that we are actually processing one of the properties we care about
                    if (!propertyMap.TryGetValue(propertySymbol, out bool[]? validFlags))
                    {
                        return;
                    }

                    // Handle the 'get' logic
                    if (SymbolEqualityComparer.Default.Equals(propertySymbol.GetMethod, context.OwningSymbol))
                    {
                        // We expect a top-level block operation, that immediately returns an expression
                        if (context.OperationBlocks is not [IBlockOperation { Operations: [IReturnOperation returnOperation] }])
                        {
                            return;
                        }

                        // Next, we expect the return to produce a field reference
                        if (returnOperation is not { ReturnedValue: IFieldReferenceOperation fieldReferenceOperation })
                        {
                            return;
                        }

                        // The field has to be implicitly declared and not constant (and not static)
                        if (fieldReferenceOperation.Field is not { IsImplicitlyDeclared: true, IsStatic: false } fieldSymbol)
                        {
                            return;
                        }

                        // Validate tha the field is indeed 'field' (it will be associated with the property)
                        if (!SymbolEqualityComparer.Default.Equals(fieldSymbol.AssociatedSymbol, propertySymbol))
                        {
                            return;
                        }

                        // The 'get' accessor is valid
                        validFlags[0] = true;
                    }
                    else if (SymbolEqualityComparer.Default.Equals(propertySymbol.SetMethod, context.OwningSymbol))
                    {
                        // We expect a top-level block operation, that immediately performs an invocation
                        if (context.OperationBlocks is not [IBlockOperation { Operations: [IExpressionStatementOperation { Operation: IInvocationOperation invocationOperation }] }])
                        {
                            return;
                        }

                        // Brief filtering of the target method, also get the original definition
                        if (invocationOperation.TargetMethod is not { Name: "SetProperty", IsGenericMethod: true, IsStatic: false } methodSymbol)
                        {
                            return;
                        }

                        // First, check that we're calling 'ObservableObject.SetProperty'
                        if (!SymbolEqualityComparer.Default.Equals(methodSymbol.ConstructedFrom, setPropertySymbol))
                        {
                            return;
                        }

                        // We matched the method, now let's validate the arguments
                        if (invocationOperation.Arguments is not [{ } locationArgument, { } valueArgument, { } propertyNameArgument])
                        {
                            return;
                        }

                        // The field has to be implicitly declared and not constant (and not static)
                        if (locationArgument.Value is not IFieldReferenceOperation { Field: { IsImplicitlyDeclared: true, IsStatic: false } fieldSymbol })
                        {
                            return;
                        }

                        // Validate tha the field is indeed 'field' (it will be associated with the property)
                        if (!SymbolEqualityComparer.Default.Equals(fieldSymbol.AssociatedSymbol, propertySymbol))
                        {
                            return;
                        }

                        // The value is just the 'value' keyword
                        if (valueArgument.Value is not IParameterReferenceOperation { Syntax: IdentifierNameSyntax { Identifier.Text: "value" } })
                        {
                            return;
                        }

                        // The property name should be the default value
                        if (propertyNameArgument is not { IsImplicit: true, ArgumentKind: ArgumentKind.DefaultValue })
                        {
                            return;
                        }

                        // The 'set' accessor is valid
                        validFlags[1] = true;
                    }
                });

                // Finally, we can consume this information when we finish processing the symbol
                context.RegisterSymbolEndAction(context =>
                {
                    // Emit a diagnostic for each property that was a valid match
                    foreach (KeyValuePair<IPropertySymbol, bool[]> pair in propertyMap)
                    {
                        if (pair.Value is [true, true])
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                UseObservablePropertyOnSemiAutoProperty,
                                pair.Key.Locations.FirstOrDefault(),
                                pair.Key.ContainingType,
                                pair.Key.Name));
                        }
                    }
                });
            }, SymbolKind.NamedType);
        });
    }

    /// <summary>
    /// Tries to get the symbol for the target <c>SetProperty</c> method this analyzer looks for.
    /// </summary>
    /// <param name="observableObjectSymbol">The symbol for <c>ObservableObject</c>.</param>
    /// <param name="setPropertySymbol">The resulting method symbol, if found (this should always be the case).</param>
    /// <returns>Whether <paramref name="setPropertySymbol"/> could be resolved correctly.</returns>
    private static bool TryGetSetPropertyMethodSymbol(INamedTypeSymbol observableObjectSymbol, [NotNullWhen(true)] out IMethodSymbol? setPropertySymbol)
    {
        foreach (ISymbol symbol in observableObjectSymbol.GetMembers("SetProperty"))
        {
            // We're guaranteed to only match methods here
            IMethodSymbol methodSymbol = (IMethodSymbol)symbol;

            // Match the exact signature we need (there's several overloads)
            if (methodSymbol.Parameters is not
                [
                    { Kind: SymbolKind.TypeParameter, RefKind: RefKind.Ref },
                    { Kind: SymbolKind.TypeParameter, RefKind: RefKind.None },
                    { Type.SpecialType: SpecialType.System_String }
                ])
            {
                setPropertySymbol = methodSymbol;

                return true;
            }
        }

        setPropertySymbol = null;

        return false;
    }
}

#endif
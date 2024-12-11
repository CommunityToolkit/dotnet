// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if ROSLYN_4_12_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates a suggestion whenever <c>[ObservableProperty]</c> is used on a semi-auto property when a partial property could be used instead.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseObservablePropertyOnSemiAutoPropertyAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The number of pooled flags per stack (ie. how many properties we expect on average per type).
    /// </summary>
    private const int NumberOfPooledFlagsPerStack = 20;

    /// <summary>
    /// Shared pool for <see cref="Dictionary{TKey, TValue}"/> instances.
    /// </summary>
    [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1008", Justification = "This is a pool of (empty) dictionaries, it is not actually storing compilation data.")]
    private static readonly ObjectPool<Dictionary<IPropertySymbol, bool[]>> PropertyMapPool = new(static () => new Dictionary<IPropertySymbol, bool[]>(SymbolEqualityComparer.Default));

    /// <summary>
    /// Shared pool for <see cref="Stack{T}"/>-s of flags, one per type being processed.
    /// </summary>
    private static readonly ObjectPool<Stack<bool[]>> PropertyFlagsStackPool = new(CreatePropertyFlagsStack);

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

                Dictionary<IPropertySymbol, bool[]> propertyMap = PropertyMapPool.Allocate();
                Stack<bool[]> propertyFlagsStack = PropertyFlagsStackPool.Allocate();

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

                    // Take an array from the stack or create a new one otherwise
                    bool[] flags = propertyFlagsStack.Count > 0
                        ? propertyFlagsStack.Pop()
                        : new bool[2];

                    // Track the property for later
                    propertyMap.Add(propertySymbol, flags);
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

                // We also need to track getters which have no body, and we need syntax for that
                context.RegisterSyntaxNodeAction(context =>
                {
                    // Let's just make sure we do have a property symbol
                    if (context.ContainingSymbol is not IPropertySymbol { GetMethod: not null } propertySymbol)
                    {
                        return;
                    }

                    // Lookup the property to get its flags
                    if (!propertyMap.TryGetValue(propertySymbol, out bool[]? validFlags))
                    {
                        return;
                    }

                    // We expect two accessors, skip if otherwise (the setter will be validated by the other callback)
                    if (context.Node is not PropertyDeclarationSyntax { AccessorList.Accessors: [{ } firstAccessor, { } secondAccessor] })
                    {
                        return;
                    }

                    // Check that either of them is a semicolon token 'get;' accessor (it can be in either position)
                    if (firstAccessor.IsKind(SyntaxKind.GetAccessorDeclaration) && firstAccessor.SemicolonToken.IsKind(SyntaxKind.SemicolonToken) ||
                        secondAccessor.IsKind(SyntaxKind.GetAccessorDeclaration) && secondAccessor.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
                    {
                        validFlags[0] = true;
                    }
                }, SyntaxKind.PropertyDeclaration);

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

                    // Before clearing the dictionary, move back all values to the stack
                    foreach (bool[] propertyFlags in propertyMap.Values)
                    {
                        // Make sure the array is cleared before returning it
                        propertyFlags.AsSpan().Clear();

                        propertyFlagsStack.Push(propertyFlags);
                    }

                    // We are now done processing the symbol, we can return the dictionary.
                    // Note that we must clear it before doing so to avoid leaks and issues.
                    propertyMap.Clear();

                    PropertyMapPool.Free(propertyMap);

                    // Also do the same for the stack, except we don't need to clean it (since it roots no compilation objects)
                    PropertyFlagsStackPool.Free(propertyFlagsStack);
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

    /// <summary>
    /// Produces a new <see cref="Stack{T}"/> instance to pool.
    /// </summary>
    /// <returns>The resulting <see cref="Stack{T}"/> instance to use.</returns>
    private static Stack<bool[]> CreatePropertyFlagsStack()
    {
        static IEnumerable<bool[]> EnumerateFlags()
        {
            for (int i = 0; i < NumberOfPooledFlagsPerStack; i++)
            {
                yield return new bool[2];
            }
        }

        return new(EnumerateFlags());
    }
}

#endif
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates a warning when accessing a field instead of a generated observable property.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FieldReferenceForObservablePropertyFieldAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(FieldReferenceForObservablePropertyFieldWarning);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterOperationAction(static context =>
        {
            // We're only looking for references to fields that could potentially be observable properties
            if (context.Operation is not IFieldReferenceOperation { Field: IFieldSymbol { IsStatic: false, IsConst: false, IsImplicitlyDeclared: false, ContainingType: INamedTypeSymbol } fieldSymbol })
            {
                return;
            }

            foreach (AttributeData attribute in fieldSymbol.GetAttributes())
            {
                // Look for the [ObservableProperty] attribute (there can only ever be one per field)
                if (attribute.AttributeClass is { Name: "ObservablePropertyAttribute" } attributeClass &&
                    context.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is INamedTypeSymbol attributeSymbol &&
                    SymbolEqualityComparer.Default.Equals(attributeClass, attributeSymbol))
                {
                    // Emit a warning to redirect users to access the generated property instead
                    context.ReportDiagnostic(Diagnostic.Create(FieldReferenceForObservablePropertyFieldWarning, context.Operation.Syntax.GetLocation(), fieldSymbol));

                    return;
                }
            }
        }, OperationKind.FieldReference);
    }
}

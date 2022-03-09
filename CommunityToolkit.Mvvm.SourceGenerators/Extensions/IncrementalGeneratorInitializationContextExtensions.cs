// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for <see cref="IncrementalGeneratorInitializationContext"/>.
/// </summary>
internal static class IncrementalGeneratorInitializationContextExtensions
{
    /// <summary>
    /// Implements a gate for a language version over items in an input <see cref="IncrementalValuesProvider{TValues}"/> source.
    /// </summary>
    /// <typeparam name="T">The type of items in the input <see cref="IncrementalValuesProvider{TValues}"/> source.</typeparam>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> value being used.</param>
    /// <param name="source">The source <see cref="IncrementalValuesProvider{TValues}"/> instance.</param>
    /// <param name="languageVersion">The minimum language version to gate for.</param>
    /// <param name="diagnosticDescriptor">The <see cref="DiagnosticDescriptor"/> to emit if the gate detects invalid usage.</param>
    /// <remarks>
    /// Items in <paramref name="source"/> will be filtered out if the gate fails. If it passes, items will remain untouched.
    /// </remarks>
    public static void FilterWithLanguageVersion<T>(
        this IncrementalGeneratorInitializationContext context,
        ref IncrementalValuesProvider<T> source,
        LanguageVersion languageVersion,
        DiagnosticDescriptor diagnosticDescriptor)
    {
        // Check whether the target language version is supported
        IncrementalValueProvider<bool> isGeneratorSupported =
            context.ParseOptionsProvider
            .Select((item, _) => item is CSharpParseOptions options && options.LanguageVersion >= languageVersion);

        // Combine each data item with the supported flag
        IncrementalValuesProvider<(T Data, bool IsGeneratorSupported)> dataWithSupportedInfo =
            source
            .Combine(isGeneratorSupported);

        // Get a marker node to show whether an invalid attribute is used
        IncrementalValueProvider<bool> isUnsupportedAttributeUsed =
            dataWithSupportedInfo
            .Select(static (item, _) => item.IsGeneratorSupported)
            .Where(static item => !item)
            .Collect()
            .Select(static (item, _) => item.Length > 0);

        // Report them to the output
        context.RegisterSourceOutput(isUnsupportedAttributeUsed, (context, diagnostic) =>
        {
            if (diagnostic)
            {
                context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, null));
            }
        });

        // Only let data through if the minimum language version is supported
        source =
            dataWithSupportedInfo
            .Where(static item => item.IsGeneratorSupported)
            .Select(static (item, _) => item.Data);
    }

    /// <summary>
    /// Conditionally invokes <see cref="IncrementalGeneratorInitializationContext.RegisterImplementationSourceOutput{TSource}(IncrementalValueProvider{TSource}, Action{SourceProductionContext, TSource})"/>
    /// if the value produced by the input <see cref="IncrementalValueProvider{TValue}"/> is <see langword="true"/>, and also supplying a given input state.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> value being used.</param>
    /// <param name="source">The source <see cref="IncrementalValueProvider{TValues}"/> instance.</param>
    /// <param name="action">The conditional <see cref="Action{T}"/> to invoke.</param>
    public static void RegisterConditionalImplementationSourceOutput<T>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<(bool Condition, T State)> source,
        Action<SourceProductionContext, T> action)
    {
        context.RegisterImplementationSourceOutput(source, (context, item) =>
        {
            if (item.Condition)
            {
                action(context, item.State);
            }
        });
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
#if NET472
using System.ComponentModel.DataAnnotations;
#endif

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers;

/// <summary>
/// A custom <see cref="CSharpAnalyzerTest{TAnalyzer, TVerifier}"/> that uses a specific C# language version to parse code.
/// </summary>
/// <typeparam name="TAnalyzer">The type of the analyzer to test.</typeparam>
internal sealed class CSharpAnalyzerWithLanguageVersionTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    /// The C# language version to use to parse code.
    /// </summary>
    private readonly LanguageVersion languageVersion;

    /// <summary>
    /// Creates a new <see cref="CSharpAnalyzerWithLanguageVersionTest{TAnalyzer}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="languageVersion">The C# language version to use to parse code.</param>
    private CSharpAnalyzerWithLanguageVersionTest(LanguageVersion languageVersion)
    {
        this.languageVersion = languageVersion;
    }

    /// <inheritdoc/>
    protected override ParseOptions CreateParseOptions()
    {
        return new CSharpParseOptions(this.languageVersion, DocumentationMode.Diagnose);
    }

    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync"/>
    /// <param name="languageVersion">The language version to use to run the test.</param>
    public static Task VerifyAnalyzerAsync(string source, LanguageVersion languageVersion, params DiagnosticResult[] expected)
    {
        CSharpAnalyzerWithLanguageVersionTest<TAnalyzer> test = new(languageVersion) { TestCode = source };

#if NET8_0_OR_GREATER
        test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
#elif NET6_0_OR_GREATER
        test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net60;
#else
        test.TestState.ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default;
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(RequiredAttribute).Assembly.Location));
#endif
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(ObservableObject).Assembly.Location));

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync(CancellationToken.None);
    }

    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync"/>
    /// <param name="languageVersion">The language version to use to run the test.</param>
    public static Task VerifyAnalyzerAsync(string source, LanguageVersion languageVersion, (string PropertyName, object PropertyValue)[] editorconfig)
    {
        CSharpAnalyzerWithLanguageVersionTest<TAnalyzer> test = new(languageVersion) { TestCode = source };

#if NET8_0_OR_GREATER
        test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
#elif NET6_0_OR_GREATER
        test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net60;
#else
        test.TestState.ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default;
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(RequiredAttribute).Assembly.Location));
#endif
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(ObservableObject).Assembly.Location));

        // Add any editorconfig properties, if present
        if (editorconfig.Length > 0)
        {
            test.SolutionTransforms.Add((solution, projectId) =>
                solution.AddAnalyzerConfigDocument(
                    DocumentId.CreateNewId(projectId),
                    "MvvmToolkitAnalyzers.editorconfig",
                    SourceText.From($"""
                        is_global = true
                        {string.Join(Environment.NewLine, editorconfig.Select(static p => $"build_property.{p.PropertyName} = {p.PropertyValue}"))}
                        """,
                        Encoding.UTF8),
                filePath: "/MvvmToolkitAnalyzers.editorconfig"));
        }

        return test.RunAsync(CancellationToken.None);
    }
}

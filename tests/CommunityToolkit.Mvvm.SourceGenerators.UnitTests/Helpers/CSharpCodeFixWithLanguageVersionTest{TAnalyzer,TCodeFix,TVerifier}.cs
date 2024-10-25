// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers;

/// <summary>
/// A custom <see cref="CSharpCodeFixTest{TAnalyzer, TCodeFix, TVerifier}"/> that uses a specific C# language version to parse code.
/// </summary>
/// <typeparam name="TAnalyzer">The type of the analyzer to produce diagnostics.</typeparam>
/// <typeparam name="TCodeFix">The type of code fix to test.</typeparam>
/// <typeparam name="TVerifier">The type of verifier to use to validate the code fixer.</typeparam>
internal sealed class CSharpCodeFixWithLanguageVersionTest<TAnalyzer, TCodeFix, TVerifier> : CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
    where TVerifier : IVerifier, new()
{
    /// <summary>
    /// The C# language version to use to parse code.
    /// </summary>
    private readonly LanguageVersion languageVersion;

    /// <summary>
    /// Creates a new <see cref="CSharpCodeFixWithLanguageVersionTest{TAnalyzer, TCodeFix, TVerifier}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="languageVersion">The C# language version to use to parse code.</param>
    public CSharpCodeFixWithLanguageVersionTest(LanguageVersion languageVersion)
    {
        this.languageVersion = languageVersion;
    }

    /// <inheritdoc/>
    protected override ParseOptions CreateParseOptions()
    {
        return new CSharpParseOptions(this.languageVersion, DocumentationMode.Diagnose);
    }
}

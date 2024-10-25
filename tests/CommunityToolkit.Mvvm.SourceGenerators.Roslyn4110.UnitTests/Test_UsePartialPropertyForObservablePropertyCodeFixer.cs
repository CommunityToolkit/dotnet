// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers.CSharpCodeFixWithLanguageVersionTest<
    CommunityToolkit.Mvvm.SourceGenerators.UseObservablePropertyOnPartialPropertyAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.UsePartialPropertyForObservablePropertyCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.Mvvm.SourceGenerators.UseObservablePropertyOnPartialPropertyAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.UsePartialPropertyForObservablePropertyCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public class Test_UsePartialPropertyForObservablePropertyCodeFixer
{
    [TestMethod]
    public async Task SimpleFieldWithNoReferences()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                private int i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                public partial int I { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(5,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(5, 6, 5, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(6, 24, 6, 25).WithArguments("C.I")
        });

        await test.RunAsync();
    }
}

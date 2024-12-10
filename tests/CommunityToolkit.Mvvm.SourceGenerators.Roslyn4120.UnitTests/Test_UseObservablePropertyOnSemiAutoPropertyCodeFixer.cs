// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers.CSharpCodeFixWithLanguageVersionTest<
    CommunityToolkit.Mvvm.SourceGenerators.UseObservablePropertyOnSemiAutoPropertyAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.UsePartialPropertyForSemiAutoPropertyCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.Mvvm.SourceGenerators.UseObservablePropertyOnSemiAutoPropertyAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.UsePartialPropertyForSemiAutoPropertyCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public class Test_UseObservablePropertyOnSemiAutoPropertyCodeFixer
{
    [TestMethod]
    public async Task SimpleProperty()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                public string Name
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                [ObservableProperty]
                public partial string Name { get; set; }
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
            // /0/Test0.cs(7,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Name can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 19, 7, 23).WithArguments("MyApp.SampleViewModel", "Name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(6, 24, 6, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }
}

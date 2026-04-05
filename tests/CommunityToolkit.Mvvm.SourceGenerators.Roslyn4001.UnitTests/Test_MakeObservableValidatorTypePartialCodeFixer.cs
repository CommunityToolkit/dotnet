// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    CommunityToolkit.Mvvm.SourceGenerators.ObservableValidatorValidationGeneratorPartialTypeAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.MakeObservableValidatorTypePartialCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.Mvvm.SourceGenerators.ObservableValidatorValidationGeneratorPartialTypeAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.MakeObservableValidatorTypePartialCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public class MakeObservableValidatorTypePartialCodeFixer
{
    [TestMethod]
    public async Task TopLevelType()
    {
        string original = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            class SampleViewModel : ObservableValidator
            {
                [Required]
                public string Name { get; set; }
            }
            """;

        string @fixed = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            partial class SampleViewModel : ObservableValidator
            {
                [Required]
                public string Name { get; set; }
            }
            """;

        CSharpCodeFixTest test = new()
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.Add(CSharpCodeFixVerifier.Diagnostic("MVVMTK0057").WithSpan(6, 7, 6, 22).WithArguments("SampleViewModel"));

        await test.RunAsync();
    }
}

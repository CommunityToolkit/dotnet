// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public class Test_UnsupportedRoslynVersionForPartialPropertyAnalyzer
{
    [TestMethod]
    public async Task UnsupportedRoslynVersionForPartialPropertyAnalyzer_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0044:ObservableProperty|}]            
                    public string Bar { get; set; }
                }
            }
            """;

        await Test_SourceGeneratorsDiagnostics.VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UnsupportedRoslynVersionForPartialPropertyAnalyzer>(source, LanguageVersion.CSharp8);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

partial class Test_SourceGeneratorsDiagnostics
{
    [TestMethod]
    public async Task RequireCSharpLanguageVersionPreviewAnalyzer_LanguageVersionIsNotPreview_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0041:ObservableProperty|}]            
                    public string Bar { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<RequiresCSharpLanguageVersionPreviewAnalyzer>(source, LanguageVersion.CSharp12);
    }

    [TestMethod]
    public async Task RequireCSharpLanguageVersionPreviewAnalyzer_LanguageVersionIsPreview_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    public string Bar { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<RequiresCSharpLanguageVersionPreviewAnalyzer>(source, languageVersion: LanguageVersion.Preview);
    }
}

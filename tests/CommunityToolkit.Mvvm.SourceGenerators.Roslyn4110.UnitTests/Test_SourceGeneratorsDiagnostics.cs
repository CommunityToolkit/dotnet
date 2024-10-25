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
                    public string Name { get; set; }
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
                    public string Name { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<RequiresCSharpLanguageVersionPreviewAnalyzer>(source, languageVersion: LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task UseObservablePropertyOnPartialPropertyAnalyzer_LanguageVersionIsNotPreview_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    private string name;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UseObservablePropertyOnPartialPropertyAnalyzer>(source, LanguageVersion.CSharp12);
    }

    [TestMethod]
    public async Task UseObservablePropertyOnPartialPropertyAnalyzer_LanguageVersionIsPreview_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0042:ObservableProperty|}]            
                    private string name;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UseObservablePropertyOnPartialPropertyAnalyzer>(source, LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task UseObservablePropertyOnPartialPropertyAnalyzer_LanguageVersionIsPreview_OnPartialProperty_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    public string Name { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UseObservablePropertyOnPartialPropertyAnalyzer>(source, LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task InvalidPropertyLevelObservablePropertyAttributeAnalyzer_OnValidPropertyDeclaration_DoesNotWarn1()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    public partial string {|CS9248:Name|} { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidPropertyLevelObservablePropertyAttributeAnalyzer>(source, LanguageVersion.Preview, [], ["CS0103", "CS9248"]);
    }

    [TestMethod]
    public async Task InvalidPropertyLevelObservablePropertyAttributeAnalyzer_OnValidPropertyDeclaration_DoesNotWarn2()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    internal partial string {|CS9248:Name|} { get; private set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidPropertyLevelObservablePropertyAttributeAnalyzer>(source, LanguageVersion.Preview, [], ["CS0103", "CS9248"]);
    }

    [TestMethod]
    public async Task InvalidPropertyLevelObservablePropertyAttributeAnalyzer_OnValidPropertyDeclaration_DoesNotWarn3()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    protected internal partial string {|CS9248:Name|} { get; private protected set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidPropertyLevelObservablePropertyAttributeAnalyzer>(source, LanguageVersion.Preview, [], ["CS0103", "CS9248"]);
    }

    [TestMethod]
    public async Task InvalidPropertyLevelObservablePropertyAttributeAnalyzer_OnNonPartialProperty_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0043:ObservableProperty|}]            
                    public string Name { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidPropertyLevelObservablePropertyAttributeAnalyzer>(source, LanguageVersion.Preview, [], ["CS9248"]);
    }

    [TestMethod]
    public async Task InvalidPropertyLevelObservablePropertyAttributeAnalyzer_OnReadOnlyProperty_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0043:ObservableProperty|}]            
                    public partial string {|CS9248:Name|} { get; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidPropertyLevelObservablePropertyAttributeAnalyzer>(source, LanguageVersion.Preview, [], ["CS9248"]);
    }

    [TestMethod]
    public async Task InvalidPropertyLevelObservablePropertyAttributeAnalyzer_OnWriteOnlyProperty_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0043:ObservableProperty|}]            
                    public partial string {|CS9248:Name|} { set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidPropertyLevelObservablePropertyAttributeAnalyzer>(source, LanguageVersion.Preview, [], ["CS9248"]);
    }

    [TestMethod]
    public async Task InvalidPropertyLevelObservablePropertyAttributeAnalyzer_OnInitOnlyProperty_Warns()
    {
#if NET6_0_OR_GREATER
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0043:ObservableProperty|}]            
                    public partial string {|CS9248:Name|} { get; init; }
                }
            }
            """;
#else
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0043:ObservableProperty|}]
                    public partial string {|CS9248:Name|} { get; {|CS0518:init|}; }
                }
            }
            """;
#endif

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidPropertyLevelObservablePropertyAttributeAnalyzer>(source, LanguageVersion.Preview, [], ["CS0518", "CS9248"]);
    }
}

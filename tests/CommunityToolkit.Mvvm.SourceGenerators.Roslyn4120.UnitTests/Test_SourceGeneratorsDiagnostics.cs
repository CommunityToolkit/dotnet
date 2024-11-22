// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

partial class Test_SourceGeneratorsDiagnostics
{
    [TestMethod]
    public async Task RequireCSharpLanguageVersionPreviewAnalyzer_LanguageVersionIsNotPreview_DoesnNotWarn()
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

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<RequiresCSharpLanguageVersionPreviewAnalyzer>(source, LanguageVersion.CSharp12);
    }

    [TestMethod]
    public async Task RequireCSharpLanguageVersionPreviewAnalyzer_LanguageVersionIsNotPreview_Partial_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0041:ObservableProperty|}]            
                    public partial string Name { get; set; }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<RequiresCSharpLanguageVersionPreviewAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,

            // /0/Test0.cs(8,31): error CS8703: The modifier 'partial' is not valid for this item in C# 12.0. Please use language version 'preview' or greater.
            DiagnosticResult.CompilerError("CS8703").WithSpan(8, 31, 8, 35).WithArguments("partial", "12.0", "preview"),
            // /0/Test0.cs(8,31): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 31, 8, 35).WithArguments("MyApp.SampleViewModel.Name"));
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
    public async Task RequireCSharpLanguageVersionPreviewAnalyzer_LanguageVersionIsPreview_Partial_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    public partial string Name { get; set; }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<RequiresCSharpLanguageVersionPreviewAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.Preview,

            // /0/Test0.cs(8,31): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 31, 8, 35).WithArguments("MyApp.SampleViewModel.Name"));
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
                    [ObservableProperty]            
                    private string {|MVVMTK0042:name|};
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

    [TestMethod]
    public async Task UseObservablePropertyOnPartialPropertyAnalyzer_LanguageVersionIsPreview_OnStaticField_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    private static string name;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UseObservablePropertyOnPartialPropertyAnalyzer>(source, LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task UseObservablePropertyOnPartialPropertyAnalyzer_CsWinRTAotOptimizerEnabled_Auto_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    private static string name;
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<UseObservablePropertyOnPartialPropertyAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.Preview,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "auto")]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_NotTargetingWindows_DoesNotWarn()
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

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,
            editorconfig: []);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_OptIn_DoesNotWarn()
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

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true)]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_False_DoesNotWarn()
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

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "false")]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_Auto_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    private string {|MVVMTK0045:name|};
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.Preview,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "auto")]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_Auto_NotCSharpPreview_Warns_WithCompilationWarning()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [{|MVVMTK0051:ObservableProperty|}]            
                    private string {|MVVMTK0045:name|};
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "auto")]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_True_NoXaml_Level1_DoesNotWarn()
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

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.Preview,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "true"), ("CsWinRTAotWarningLevel", 1)]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_True_NoXaml_Level2_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    private string {|MVVMTK0045:name|};
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.Preview,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "true"), ("CsWinRTAotWarningLevel", 2)]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_True_NoXaml_1_Component_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    private string {|MVVMTK0045:name|};
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.Preview,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "true"), ("CsWinRTAotWarningLevel", 1), ("CsWinRTComponent", true)]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_True_UwpXaml_Level1_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    private string {|MVVMTK0045:name|};
                }
            }

            namespace Windows.UI.Xaml.Controls
            {
                public class Button;
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.Preview,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "true"), ("CsWinRTAotWarningLevel", 1)]);
    }

    [TestMethod]
    public async Task WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer_TargetingWindows_CsWinRTAotOptimizerEnabled_True_WinUIXaml_Level1_Warns()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [ObservableProperty]            
                    private string {|MVVMTK0045:name|};
                }
            }

            namespace Microsoft.UI.Xaml.Controls
            {
                public class Button;
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTObservablePropertyOnFieldsIsNotAotCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.Preview,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true), ("CsWinRTAotOptimizerEnabled", "true"), ("CsWinRTAotWarningLevel", 1)]);
    }

    [TestMethod]
    public async Task WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer_NotTargetingWindows_DoesNotWarn()
    {
        const string source = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;
            using CommunityToolkit.Mvvm.Input;
            using WinRT;
            
            namespace MyApp
            {
                [GeneratedBindableCustomProperty]
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    private string name;

                    [RelayCommand]
                    private void DoStuff()
                    {
                    }
                }
            }

            namespace WinRT
            {
                public class GeneratedBindableCustomPropertyAttribute : Attribute;
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,
            editorconfig: []);
    }

    [TestMethod]
    public async Task WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer_TargetingWindows_DoesNotWarn()
    {
        const string source = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;
            using CommunityToolkit.Mvvm.Input;
            using WinRT;
            
            namespace MyApp
            {
                [GeneratedBindableCustomProperty]
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    private string name;

                    [RelayCommand]
                    private void DoStuff()
                    {
                    }
                }
            }

            namespace WinRT
            {
                public class GeneratedBindableCustomPropertyAttribute : Attribute;
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true)]);
    }

    [TestMethod]
    public async Task WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer_TargetingWindows_BaseType_Field_Warns()
    {
        const string source = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;
            using WinRT;
            
            namespace MyApp
            {
                [GeneratedBindableCustomProperty]
                public partial class {|MVVMTK0047:SampleViewModel|} : BaseViewModel
                {                    
                }

                public partial class BaseViewModel : ObservableObject
                {
                    [ObservableProperty]
                    private string name;
                }
            }

            namespace WinRT
            {
                public class GeneratedBindableCustomPropertyAttribute : Attribute;
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true)]);
    }

    [TestMethod]
    public async Task WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer_TargetingWindows_BaseType_Method_Warns()
    {
        const string source = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;
            using CommunityToolkit.Mvvm.Input;
            using WinRT;
            
            namespace MyApp
            {
                [GeneratedBindableCustomProperty]
                public partial class {|MVVMTK0048:SampleViewModel|} : BaseViewModel
                {                    
                }

                public partial class BaseViewModel : ObservableObject
                {            
                    [RelayCommand]
                    private void DoStuff()
                    {
                    }
                }
            }

            namespace WinRT
            {
                public class GeneratedBindableCustomPropertyAttribute : Attribute;
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTGeneratedBindableCustomPropertyWithBasesMemberAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp12,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true)]);
    }
}

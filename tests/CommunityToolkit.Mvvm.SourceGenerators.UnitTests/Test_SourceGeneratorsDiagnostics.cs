// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public partial class Test_SourceGeneratorsDiagnostics
{
    [TestMethod]
    public void DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError_Explicit()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                }
            }
            """;

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0001");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError_Inherited()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace CommunityToolkit.Mvvm.ComponentModel
            {
                public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                    public event PropertyChangingEventHandler? PropertyChanging;
                }
            }

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel : ObservableObject
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0001");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError_Explicit()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0002");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError_Inherited()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace CommunityToolkit.Mvvm.ComponentModel
            {
                public abstract class ObservableObject : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                }
            }

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel : ObservableObject
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0002");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError_Explicit()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel : INotifyPropertyChanging
                {
                    public event PropertyChangingEventHandler? PropertyChanging;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0003");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError_Inherited()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public abstract class MyBaseViewModel : INotifyPropertyChanging
                {
                    public event PropertyChangingEventHandler? PropertyChanging;
                }

                [ObservableObject]
                public partial class SampleViewModel : MyBaseViewModel
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0003");
    }

    [TestMethod]
    public void DuplicateObservableRecipientError()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace CommunityToolkit.Mvvm.ComponentModel
            {
                public abstract class ObservableRecipient : ObservableObject
                {
                }
            }

            namespace MyApp
            {
                [ObservableRecipient]
                public partial class SampleViewModel : ObservableRecipient
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableRecipientGenerator>(source, "MVVMTK0004");
    }

    [TestMethod]
    public void MissingBaseObservableObjectFunctionalityError()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableRecipient]
                public partial class SampleViewModel
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableRecipientGenerator>(source, "MVVMTK0005");
    }

    [TestMethod]
    public void MissingObservableValidatorInheritanceForValidationAttributeError()
    {
        string source = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel
                {
                    [ObservableProperty]
                    [Required]
                    private string name;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0006");
    }

    [TestMethod]
    public void InvalidRelayCommandMethodSignatureError()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand]
                    private string GreetUser() => "Hello world!";
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0007");
    }

    [TestMethod]
    public async Task UnsupportedCSharpLanguageVersion_FromINotifyPropertyChangedGenerator()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class {|MVVMTK0008:SampleViewModel|}
                {
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UnsupportedCSharpLanguageVersionAnalyzer>(source, LanguageVersion.CSharp7_3);
    }

    [TestMethod]
    public async Task UnsupportedCSharpLanguageVersion_FromObservableObjectGenerator()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class {|MVVMTK0008:SampleViewModel|}
                {
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UnsupportedCSharpLanguageVersionAnalyzer>(source, LanguageVersion.CSharp7_3);
    }

    [TestMethod]
    public async Task UnsupportedCSharpLanguageVersion_FromObservablePropertyGenerator()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class {|MVVMTK0008:SampleViewModel|}
                {
                    [ObservableProperty]
                    private string {|MVVMTK0008:name|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UnsupportedCSharpLanguageVersionAnalyzer>(source, LanguageVersion.CSharp7_3);
    }

    [TestMethod]
    public async Task UnsupportedCSharpLanguageVersion_FromObservablePropertyGenerator_MultipleAttributes()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class {|MVVMTK0008:SampleViewModel|}
                {
                    [ObservableProperty]
                    string {|MVVMTK0008:name|};

                    [ObservableProperty]
                    [NotifyPropertyChangedFor(nameof(Bar))]
                    int {|MVVMTK0008:number|};

                    string Bar { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UnsupportedCSharpLanguageVersionAnalyzer>(source, LanguageVersion.CSharp7_3);
    }

    [TestMethod]
    public async Task UnsupportedCSharpLanguageVersion_FromObservableValidatorValidateAllPropertiesGenerator()
    {
        string source = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableValidator
                {
                    [Required]
                    public string Name { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UnsupportedCSharpLanguageVersionAnalyzer>(source, LanguageVersion.CSharp7_3);
    }

    [TestMethod]
    public async Task UnsupportedCSharpLanguageVersion_FromRelayCommandGenerator()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand]
                    private void {|MVVMTK0008:GreetUser|}(object value)
                    {
                    }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UnsupportedCSharpLanguageVersionAnalyzer>(source, LanguageVersion.CSharp7_3);
    }

    [TestMethod]
    public async Task UnsupportedCSharpLanguageVersion_FromIMessengerRegisterAllGenerator()
    {
        string source = """
            using CommunityToolkit.Mvvm.Messaging;

            namespace MyApp
            {
                public class MyMessage
                {
                }

                public partial class SampleViewModel : IRecipient<MyMessage>
                {
                    public void Receive(MyMessage message)
                    {
                    }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<UnsupportedCSharpLanguageVersionAnalyzer>(source, LanguageVersion.CSharp7_3);
    }

    [TestMethod]
    public void InvalidCanExecuteMemberName()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private bool Foo => true;

                    [RelayCommand(CanExecute = "Bar")]
                    private void GreetUser()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0009");
    }

    [TestMethod]
    public void MultipleCanExecuteMemberNameMatches()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private bool Foo => true;

                    private bool Foo() => true;

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0010");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_NonReadableProperty()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private bool Foo { set { } }

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_PropertyWithInvalidType()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private string Foo => "Hi!";

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_MethodWithInvalidType()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private string Foo() => "Hi!";

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_MethodWithIncompatibleInputType_MissingInput()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private bool Foo(string name) => true;

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_MethodWithIncompatibleInputType_NonMatchingInputType()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private bool Foo(int age) => true;

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser(string name)
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_MethodWithIncompatibleInputType_TooManyInputs()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private bool Foo(string name, int age) => true;

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser(string name)
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidRelayCommandAllowConcurrentExecutionsOption()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand(AllowConcurrentExecutions = false)]
                    private void GreetUser(User user)
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0012");
    }

    [TestMethod]
    public void InvalidRelayCommandIncludeCancelCommandSettings_SynchronousMethod()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand(IncludeCancelCommand = true)]
                    private void GreetUser(User user)
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0013");
    }

    [TestMethod]
    public void InvalidRelayCommandIncludeCancelCommandSettings_AsynchronousMethodWithNoCancellationToken()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand(IncludeCancelCommand = true)]
                    private async Task DoWorkAsync()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0013");
    }

    [TestMethod]
    public void InvalidRelayCommandIncludeCancelCommandSettings_AsynchronousMethodWithParameterAndNoCancellationToken()
    {
        string source = """
            using System.Threading.Tasks;
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand(IncludeCancelCommand = true)]
                    private async Task GreetUserAsync(User user)
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0013");
    }

    [TestMethod]
    public async Task NameCollisionForGeneratedObservableProperty_PascalCaseField_Warns()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    private string {|MVVMTK0014:Name|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<PropertyNameCollisionObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task NameCollisionForGeneratedObservableProperty_CamelCaseField_DoesNotWarn()
    {
        string source = """
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

        // Using C# 9 here because the generated code will emit [MemberNotNull] on the property setter, which requires C# 9
        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<PropertyNameCollisionObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp9);
    }

    [TestMethod]
    public async Task NameCollisionForGeneratedObservableProperty_PascalCaseProperty_DoesNotWarn()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    private string Name { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<PropertyNameCollisionObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public void NotifyPropertyChangedForInvalidTargetError_Null()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedFor(null)]
                    private string name;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0015");
    }

    [TestMethod]
    public void NotifyPropertyChangedForInvalidTargetError_SamePropertyAsGeneratedOneFromSelf()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedFor(nameof(Name))]
                    private string name;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0015");
    }

    [TestMethod]
    public void NotifyPropertyChangedForInvalidTargetError_Missing()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedFor("FooBar")]
                    private string name;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0015");
    }

    [TestMethod]
    public void NotifyPropertyChangedForInvalidTargetError_InvalidType()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedFor(nameof(Foo))]
                    private string name;

                    public void Foo()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0015");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_Null()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyCanExecuteChangedFor(null)]
                    private string name;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_Missing()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyCanExecuteChangedFor("FooBar")]
                    private string name;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_InvalidMemberType()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyCanExecuteChangedFor(nameof(Foo))]
                    private string name;

                    public void Foo()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_InvalidPropertyType()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyCanExecuteChangedFor(nameof(Foo))]
                    private string name;

                    public string Foo { get; }
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_InvalidCommandType()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyCanExecuteChangedFor(nameof(FooCommand))]
                    private string name;

                    public ICommand FooCommand { get; }
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForINotifyPropertyChangedAttributeError_InheritingINotifyPropertyChangedAttribute()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class A
                {
                }

                [INotifyPropertyChanged]
                public partial class B : A
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0017");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForINotifyPropertyChangedAttributeError_InheritingObservableObjectAttribute()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class A
                {
                }

                [INotifyPropertyChanged]
                public partial class B : A
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0017");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForINotifyPropertyChangedAttributeError_WithAlsoObservableObjectAttribute()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                [ObservableObject]
                public partial class A
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0017");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForObservableObjectAttributeError_InheritingINotifyPropertyChangedAttribute()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class A
                {
                }

                [ObservableObject]
                public partial class B : A
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0018");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForObservableObjectAttributeError_InheritingObservableObjectAttribute()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class A
                {
                }

                [ObservableObject]
                public partial class B : A
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0018");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForObservableObjectAttributeError_WithAlsoINotifyPropertyChangedAttribute()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                [ObservableObject]
                public partial class A
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0018");
    }

    [TestMethod]
    public async Task InvalidContainingTypeForObservableProperty_OnField_Warns()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : INotifyPropertyChanged
                {
                    [ObservableProperty]
                    public int {|MVVMTK0019:number|};

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidTargetObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task InvalidContainingTypeForObservableProperty_OnField_InValidType_DoesNotWarn()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public int number;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidTargetObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task InvalidContainingTypeForObservableProperty_OnPartialProperty_Warns()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : INotifyPropertyChanged
                {
                    [ObservableProperty]
                    public int {|MVVMTK0019:Number|} { get; set; }

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidTargetObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task InvalidContainingTypeForObservableProperty_OnPartialProperty_InValidType_DoesNotWarn()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public int Number { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidTargetObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task FieldWithOrphanedDependentObservablePropertyAttributesError_NotifyPropertyChangedFor()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [NotifyPropertyChangedFor("")]
                    public int {|MVVMTK0020:number|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<FieldWithOrphanedDependentObservablePropertyAttributesAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task FieldWithOrphanedDependentObservablePropertyAttributesError_NotifyCanExecuteChangedFor()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [NotifyCanExecuteChangedFor("")]
                    public int {|MVVMTK0020:number|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<FieldWithOrphanedDependentObservablePropertyAttributesAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task FieldWithOrphanedDependentObservablePropertyAttributesError_NotifyPropertyChangedRecipients()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [NotifyPropertyChangedRecipients]
                    public int {|MVVMTK0020:number|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<FieldWithOrphanedDependentObservablePropertyAttributesAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task FieldWithOrphanedDependentObservablePropertyAttributesError_MultipleUsesStillGenerateOnlyASingleDiagnostic()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [NotifyPropertyChangedFor("")]
                    [NotifyPropertyChangedFor("")]
                    [NotifyPropertyChangedFor("")]
                    [NotifyCanExecuteChangedFor("")]
                    [NotifyCanExecuteChangedFor("")]
                    [NotifyPropertyChangedRecipients]
                    public int {|MVVMTK0020:number|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<FieldWithOrphanedDependentObservablePropertyAttributesAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public void InvalidAttributeCombinationForObservableRecipientAttributeError()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableRecipient]
                public partial class A : ObservableObject
                {
                }

                [ObservableRecipient]
                public partial class B : A
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservableRecipientGenerator>(source, "MVVMTK0021");
    }

    [TestMethod]
    public void InvalidContainingTypeForNotifyPropertyChangedRecipientsFieldError_ObservableObject()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedRecipients]
                    public int number;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0022");
    }

    [TestMethod]
    public void MultipleRelayCommandMethodOverloads_WithOverloads()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand]
                    private void GreetUser()
                    {
                    }

                    [RelayCommand]
                    private void GreetUser(object value)
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0023");
    }

    [TestMethod]
    public void MultipleRelayCommandMethodOverloads_WithOverloadInBaseType()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class BaseViewModel
                {
                    [RelayCommand]
                    private void GreetUser()
                    {
                    }
                }

                public partial class SampleViewModel : BaseViewModel
                {
                    [RelayCommand]
                    private void GreetUser(object value)
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0023");
    }

    [TestMethod]
    public async Task InvalidObservablePropertyError_Object()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public object {|MVVMTK0024:property|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidGeneratedPropertyObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp9);
    }

    [TestMethod]
    public async Task InvalidObservablePropertyError_Object_WithProperty()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public object {|MVVMTK0024:Property|} { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidGeneratedPropertyObservablePropertyAttributeAnalyzer>(source, LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task InvalidObservablePropertyError_PropertyChangingEventArgs()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public PropertyChangingEventArgs {|MVVMTK0024:property|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidGeneratedPropertyObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp9);
    }

    [TestMethod]
    public async Task InvalidObservablePropertyError_PropertyChangedEventArgs()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public PropertyChangedEventArgs {|MVVMTK0024:property|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidGeneratedPropertyObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp9);
    }

    [TestMethod]
    public async Task InvalidObservablePropertyError_CustomTypeDerivedFromPropertyChangedEventArgs()
    {
        string source = """
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public class MyPropertyChangedEventArgs : PropertyChangedEventArgs
                {
                    public MyPropertyChangedEventArgs(string propertyName)
                        : base(propertyName)
                    {
                    }
                }

                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public MyPropertyChangedEventArgs {|MVVMTK0024:property|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidGeneratedPropertyObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp9);
    }

    [TestMethod]
    public void MissingObservableValidatorInheritanceForNotifyDataErrorInfoError()
    {
        string source = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel
                {
                    [ObservableProperty]
                    [Required]
                    [NotifyDataErrorInfo]
                    private string name;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0006", "MVVMTK0025");
    }

    [TestMethod]
    public void MissingValidationAttributesForNotifyDataErrorInfoError()
    {
        string source = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableValidator
                {
                    [ObservableProperty]
                    [NotifyDataErrorInfo]
                    private string name;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0026");
    }

    [TestMethod]
    public async Task InvalidTypeForNotifyPropertyChangedRecipientsError()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [NotifyPropertyChangedRecipients]
                public partial class {|MVVMTK0027:MyViewModel|} : ObservableObject
                {
                    [ObservableProperty]
                    public int number;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidClassLevelNotifyPropertyChangedRecipientsAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task InvalidTypeForNotifyDataErrorInfoError()
    {
        string source = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [NotifyDataErrorInfo]
                public partial class {|MVVMTK0028:SampleViewModel|} : ObservableObject
                {
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<InvalidClassLevelNotifyDataErrorInfoAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public void UnnecessaryNotifyPropertyChangedRecipientsWarning_SameType()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [NotifyPropertyChangedRecipients]
                public partial class MyViewModel : ObservableRecipient
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedRecipients]
                    public int number;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0029");
    }

    [TestMethod]
    public void UnnecessaryNotifyPropertyChangedRecipientsWarning_BaseType()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [NotifyPropertyChangedRecipients]
                public class MyBaseViewModel : ObservableRecipient
                {
                }

                public partial class MyViewModel : MyBaseViewModel
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedRecipients]
                    public int number;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0029");
    }

    [TestMethod]
    public void UnnecessaryNotifyDataErrorInfoWarning_SameType()
    {
        string source = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [NotifyDataErrorInfo]
                public partial class MyViewModel : ObservableValidator
                {
                    [ObservableProperty]
                    [Required]
                    [NotifyDataErrorInfo]
                    public int number;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0030");
    }

    [TestMethod]
    public void UnnecessaryNotifyDataErrorInfoWarning_BaseType()
    {
        string source = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [NotifyDataErrorInfo]
                public class MyBaseViewModel : ObservableValidator
                {
                }

                public partial class MyViewModel : MyBaseViewModel
                {
                    [ObservableProperty]
                    [Required]
                    [NotifyDataErrorInfo]
                    public int number;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0030");
    }

    [TestMethod]
    public void InvalidRelayCommandFlowExceptionsToTaskSchedulerOption()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand(FlowExceptionsToTaskScheduler = false)]
                    private void GreetUser(User user)
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0031");
    }

    [TestMethod]
    public async Task UsingINotifyPropertyChangedAttributeOnClassWithNoBaseType()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class {|MVVMTK0032:SampleViewModel|}
                {
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<ClassUsingAttributeInsteadOfInheritanceAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task UsingObservableObjectAttributeOnClassWithNoBaseType()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class {|MVVMTK0033:SampleViewModel|}
                {
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<ClassUsingAttributeInsteadOfInheritanceAnalyzer>(source, LanguageVersion.CSharp8);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/518
    [TestMethod]
    public async Task FieldReferenceToFieldWithObservablePropertyAttribute_Assignment()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class MyViewModel : ObservableObject
            {
                [ObservableProperty]
                int number;

                void M1()
                {
                    {|MVVMTK0034:number|} = 1;
                }

                void M2()
                {
                    {|MVVMTK0034:this.number|} = 1;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<FieldReferenceForObservablePropertyFieldAnalyzer>(source, LanguageVersion.CSharp8);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/518
    [TestMethod]
    public async Task FieldReferenceToFieldWithObservablePropertyAttribute_Read()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class MyViewModel : ObservableObject
            {
                [ObservableProperty]
                int number;

                void M()
                {
                    var temp = {|MVVMTK0034:number|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<FieldReferenceForObservablePropertyFieldAnalyzer>(source, LanguageVersion.CSharp8);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/518
    [TestMethod]
    public async Task FieldReferenceToFieldWithObservablePropertyAttribute_Ref()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class MyViewModel : ObservableObject
            {
                [ObservableProperty]
                int number;

                void M()
                {
                    ref var x = ref {|MVVMTK0034:number|};
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<FieldReferenceForObservablePropertyFieldAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task FieldReferenceToFieldWithObservablePropertyAttribute_DoesNotEmitWarningFromWithinConstructor()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class MyViewModel : ObservableObject
            {
                [ObservableProperty]
                int number;

                public MyViewModel()
                {
                    number = 42;
                    var temp = number;
                    ref var x = ref number;
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<FieldReferenceForObservablePropertyFieldAnalyzer>(source, LanguageVersion.CSharp8);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/563
    [TestMethod]
    public void InvalidPropertyTargetedAttributeOnObservablePropertyField_MissingUsingDirective()
    {
        string source = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [property: MyTest]
                    public int number;
                }
            }

            namespace MyAttributes
            {
                [AttributeUsage(AttributeTargets.Property)]
                public class MyTestAttribute : Attribute
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0035");
    }

    [TestMethod]
    public void InvalidPropertyTargetedAttributeOnObservablePropertyField_TypoInAttributeName()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [property: Fbuifbweif]
                    public int number;
                }
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0035");
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/683
    [TestMethod]
    public void InvalidPropertyTargetedAttributeOnObservablePropertyField_InvalidExpression()
    {
        string source = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            public partial class MyViewModel : ObservableObject
            {
                [ObservableProperty]
                [property: Test(TestAttribute.M)]
                private int number;
            }

            public class TestAttribute : Attribute
            {
                public static string M => "";
            }
            """;

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0037");
    }

    [TestMethod]
    public void InvalidPropertyTargetedAttributeOnRelayCommandMethod_MissingUsingDirective()
    {
        string source = """
            using System;
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [RelayCommand]
                    [property: MyTest]
                    private void Test()
                    {
                    }
                }
            }

            namespace MyAttributes
            {
                [AttributeUsage(AttributeTargets.Property)]
                public class MyTestAttribute : Attribute
                {
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0036");
    }

    [TestMethod]
    public void InvalidPropertyTargetedAttributeOnRelayCommandMethod_TypoInAttributeName()
    {
        string source = """
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [RelayCommand]
                    [property: Fbuifbweif]
                    private void Test()
                    {
                    }
                }
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0036");
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/683
    [TestMethod]
    public void InvalidPropertyTargetedAttributeOnRelayCommandMethod_InvalidExpressionOnFieldAttribute()
    {
        string source = """
            using System;
            using CommunityToolkit.Mvvm.Input;

            public partial class MyViewModel
            {
                [RelayCommand]
                [field: Test(TestAttribute.M)]
                private void Test()
                {
                }
            }

            public class TestAttribute : Attribute
            {
                public static string M => "";
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0038");
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/683
    [TestMethod]
    public void InvalidPropertyTargetedAttributeOnRelayCommandMethod_InvalidExpressionOnPropertyAttribute()
    {
        string source = """
            using System;
            using CommunityToolkit.Mvvm.Input;

            public partial class MyViewModel
            {
                [RelayCommand]
                [property: Test(TestAttribute.M)]
                private void Test()
                {
                }
            }

            public class TestAttribute : Attribute
            {
                public static string M => "";
            }
            """;

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0038");
    }

    [TestMethod]
    public async Task AsyncVoidReturningRelayCommandMethodAnalyzer()
    {
        string source = """
            using System;
            using CommunityToolkit.Mvvm.Input;

            public partial class MyViewModel
            {
                [RelayCommand]
                private async void {|MVVMTK0039:TestAsync|}()
                {
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<AsyncVoidReturningRelayCommandMethodAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task FieldTargetedObservablePropertyAttribute_InstanceAutoProperty()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [field: {|MVVMTK0040:ObservableProperty|}]
                    public string Name { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<AutoPropertyWithFieldTargetedObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task FieldTargetedObservablePropertyAttribute_StaticAutoProperty()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [field: {|MVVMTK0040:ObservableProperty|}]
                    public static string Name { get; set; }
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<AutoPropertyWithFieldTargetedObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp8);
    }

    [TestMethod]
    public async Task FieldTargetedObservablePropertyAttribute_RecordPrimaryConstructorParameter()
    {
        string source = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial record SampleViewModel([field: {|MVVMTK0040:ObservableProperty|}] string Name);
            }

            namespace System.Runtime.CompilerServices
            {
                internal static class IsExternalInit
                {
                }
            }
            """;

        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<AutoPropertyWithFieldTargetedObservablePropertyAttributeAnalyzer>(source, LanguageVersion.CSharp9);
    }

    [TestMethod]
    public async Task WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatibleAnalyzer_NotTargetingWindows_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            using CommunityToolkit.Mvvm.Input;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [RelayCommand]
                    private void DoStuff()
                    {
                    }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp10,
            editorconfig: []);
    }

    [TestMethod]
    public async Task WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatibleAnalyzer_TargetingWindows_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.Mvvm.ComponentModel;
            using CommunityToolkit.Mvvm.Input;
            
            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {            
                    [RelayCommand]
                    private void DoStuff()
                    {
                    }
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp10,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true)]);
    }

    [TestMethod]
    public async Task WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatibleAnalyzer_TargetingWindows_Bindable_Warns()
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
                    [{|MVVMTK0046:RelayCommand|}]
                    private void DoStuff()
                    {
                    }
                }
            }

            namespace WinRT
            {
                public class GeneratedBindableCustomPropertyAttribute : Attribute
                {
                }
            }
            """;

        await CSharpAnalyzerWithLanguageVersionTest<WinRTRelayCommandIsNotGeneratedBindableCustomPropertyCompatibleAnalyzer>.VerifyAnalyzerAsync(
            source,
            LanguageVersion.CSharp10,
            editorconfig: [("_MvvmToolkitIsUsingWindowsRuntimePack", true)]);
    }

    /// <summary>
    /// Verifies the diagnostic errors for a given analyzer, and that all available source generators can run successfully with the input source (including subsequent compilation).
    /// </summary>
    /// <typeparam name="TAnalyzer">The type of the analyzer to test.</typeparam>
    /// <param name="markdownSource">The input source to process with diagnostic annotations.</param>
    /// <param name="languageVersion">The language version to use to parse code and run tests.</param>
    internal static async Task VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<TAnalyzer>(string markdownSource, LanguageVersion languageVersion)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        await VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<TAnalyzer>(markdownSource, languageVersion, [], []);
    }

    /// <summary>
    /// Verifies the diagnostic errors for a given analyzer, and that all available source generators can run successfully with the input source (including subsequent compilation).
    /// </summary>
    /// <typeparam name="TAnalyzer">The type of the analyzer to test.</typeparam>
    /// <param name="markdownSource">The input source to process with diagnostic annotations.</param>
    /// <param name="languageVersion">The language version to use to parse code and run tests.</param>
    /// <param name="generatorDiagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    /// <param name="ignoredDiagnosticIds">The list of diagnostic ids to ignore in the final compilation.</param>
    internal static async Task VerifyAnalyzerDiagnosticsAndSuccessfulGeneration<TAnalyzer>(string markdownSource, LanguageVersion languageVersion, string[] generatorDiagnosticsIds, string[] ignoredDiagnosticIds)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        await CSharpAnalyzerWithLanguageVersionTest<TAnalyzer>.VerifyAnalyzerAsync(markdownSource, languageVersion);

        IIncrementalGenerator[] generators =
        {
            new IMessengerRegisterAllGenerator(),
            new ObservableObjectGenerator(),
            new INotifyPropertyChangedGenerator(),
            new ObservablePropertyGenerator(),
            new ObservableRecipientGenerator(),
            new ObservableValidatorValidateAllPropertiesGenerator(),
            new RelayCommandGenerator()
        };

        // Transform diagnostic annotations back to normal C# (eg. "{|MVVMTK0008:Foo()|}" ---> "Foo()")
        string source = Regex.Replace(markdownSource, @"{\|((?:,?\w+)+):(.+?)\|}", m => m.Groups[2].Value);

        VerifyGeneratedDiagnostics(CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(languageVersion)), generators, generatorDiagnosticsIds, ignoredDiagnosticIds);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="source">The input source to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    internal static void VerifyGeneratedDiagnostics<TGenerator>(string source, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        IIncrementalGenerator generator = new TGenerator();

        VerifyGeneratedDiagnostics(CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8)), new[] { generator }, diagnosticsIds, []);
    }

    /// <summary>
    /// Verifies the output of one or more source generators.
    /// </summary>
    /// <param name="syntaxTree">The input source tree to process.</param>
    /// <param name="generators">The generators to apply to the input syntax tree.</param>
    /// <param name="generatorDiagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    /// <param name="ignoredDiagnosticIds">The list of diagnostic ids to ignore in the final compilation.</param>
    internal static void VerifyGeneratedDiagnostics(SyntaxTree syntaxTree, IIncrementalGenerator[] generators, string[] generatorDiagnosticsIds, string[] ignoredDiagnosticIds)
    {
        // Ensure CommunityToolkit.Mvvm and System.ComponentModel.DataAnnotations are loaded
        Type observableObjectType = typeof(ObservableObject);
        Type validationAttributeType = typeof(ValidationAttribute);

        // Get all assembly references for the loaded assemblies (easy way to pull in all necessary dependencies)
        IEnumerable<MetadataReference> references =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where !assembly.IsDynamic
            let reference = MetadataReference.CreateFromFile(assembly.Location)
            select reference;

        // Create a syntax tree with the input source
        CSharpCompilation compilation = CSharpCompilation.Create(
            "original",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generators).WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

        // Run all source generators on the input source code
        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        string[] resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToArray();

        CollectionAssert.AreEquivalent(generatorDiagnosticsIds, resultingIds, $"resultingIds: {string.Join(", ", resultingIds)}");

        // If the compilation was supposed to succeed, ensure that no further errors were generated
        if (resultingIds.Length == 0)
        {
            // Compute diagnostics for the final compiled output (just include errors)
            List<Diagnostic> outputCompilationDiagnostics = outputCompilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToList();

            // Filtered diagnostics
            List<Diagnostic> filteredDiagnostics = outputCompilationDiagnostics.Where(diagnostic => !ignoredDiagnosticIds.Contains(diagnostic.Id)).ToList();

            Assert.IsTrue(filteredDiagnostics.Count == 0, $"resultingIds: {string.Join(", ", filteredDiagnostics)}");
        }

        GC.KeepAlive(observableObjectType);
        GC.KeepAlive(validationAttributeType);
    }
}

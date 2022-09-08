// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public class Test_SourceGeneratorsDiagnostics
{
    [TestMethod]
    public void DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError_Explicit()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                }
            }";

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0001");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError_Inherited()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0001");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError_Explicit()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                }
            }";

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0002");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError_Inherited()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0002");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError_Explicit()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel : INotifyPropertyChanging
                {
                    public event PropertyChangingEventHandler? PropertyChanging;
                }
            }";

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0003");
    }

    [TestMethod]
    public void DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError_Inherited()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0003");
    }

    [TestMethod]
    public void DuplicateObservableRecipientError()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservableRecipientGenerator>(source, "MVVMTK0004");
    }

    [TestMethod]
    public void MissingBaseObservableObjectFunctionalityError()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableRecipient]
                public partial class SampleViewModel
                {
                }
            }";

        VerifyGeneratedDiagnostics<ObservableRecipientGenerator>(source, "MVVMTK0005");
    }

    [TestMethod]
    public void MissingObservableValidatorInheritanceForValidationAttributeError()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0006");
    }

    [TestMethod]
    public void InvalidRelayCommandMethodSignatureError()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand]
                    private string GreetUser() => ""Hello world!"";
                }
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0007");
    }

    [TestMethod]
    public void UnsupportedCSharpLanguageVersion_FromINotifyPropertyChangedGenerator()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel
                {
                }
            }";

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)),
            "MVVMTK0008");

        VerifySuccessfulGeneration(source);
    }

    [TestMethod]
    public void UnsupportedCSharpLanguageVersion_FromObservableObjectGenerator()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel
                {
                }
            }";

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)),
            "MVVMTK0008");

        VerifySuccessfulGeneration(source);
    }

    [TestMethod]
    public void UnsupportedCSharpLanguageVersion_FromObservablePropertyGenerator()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel
                {
                    [ObservableProperty]
                    private string name;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)),
            "MVVMTK0008");

        VerifySuccessfulGeneration(source);
    }

    [TestMethod]
    public void UnsupportedCSharpLanguageVersion_FromObservableValidatorValidateAllPropertiesGenerator()
    {
        string source = @"
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableValidator
                {
                    [Required]
                    public string Name { get; set; }
                }
            }";

        VerifyGeneratedDiagnostics<ObservableValidatorValidateAllPropertiesGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)),
            "MVVMTK0008");

        VerifySuccessfulGeneration(source);
    }

    [TestMethod]
    public void UnsupportedCSharpLanguageVersion_FromRelayCommandGenerator()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [RelayCommand]
                    private void GreetUser(object value)
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)),
            "MVVMTK0008");

        VerifySuccessfulGeneration(source);
    }

    [TestMethod]
    public void UnsupportedCSharpLanguageVersion_FromIMessengerRegisterAllGenerator()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<IMessengerRegisterAllGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)),
            "MVVMTK0008");

        VerifySuccessfulGeneration(source);
    }

    [TestMethod]
    public void InvalidCanExecuteMemberName()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private bool Foo => true;

                    [RelayCommand(CanExecute = ""Bar"")]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0009");
    }

    [TestMethod]
    public void MultipleCanExecuteMemberNameMatches()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0010");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_NonReadableProperty()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_PropertyWithInvalidType()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private string Foo => ""Hi!"";

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_MethodWithInvalidType()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    private string Foo() => ""Hi!"";

                    [RelayCommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_MethodWithIncompatibleInputType_MissingInput()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_MethodWithIncompatibleInputType_NonMatchingInputType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidCanExecuteMember_MethodWithIncompatibleInputType_TooManyInputs()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidRelayCommandAllowConcurrentExecutionsOption()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0012");
    }

    [TestMethod]
    public void InvalidRelayCommandIncludeCancelCommandSettings_SynchronousMethod()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0013");
    }

    [TestMethod]
    public void InvalidRelayCommandIncludeCancelCommandSettings_AsynchronousMethodWithNoCancellationToken()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0013");
    }

    [TestMethod]
    public void InvalidRelayCommandIncludeCancelCommandSettings_AsynchronousMethodWithParameterAndNoCancellationToken()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0013");
    }

    [TestMethod]
    public void NameCollisionForGeneratedObservableProperty()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    private string Name;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0014");
    }

    [TestMethod]
    public void NotifyPropertyChangedForInvalidTargetError_Null()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedFor(null)]
                    private string name;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0015");
    }

    [TestMethod]
    public void NotifyPropertyChangedForInvalidTargetError_SamePropertyAsGeneratedOneFromSelf()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedFor(nameof(Name))]
                    private string name;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0015");
    }

    [TestMethod]
    public void NotifyPropertyChangedForInvalidTargetError_Missing()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedFor(""FooBar"")]
                    private string name;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0015");
    }

    [TestMethod]
    public void NotifyPropertyChangedForInvalidTargetError_InvalidType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0015");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_Null()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyCanExecuteChangedFor(null)]
                    private string name;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_Missing()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyCanExecuteChangedFor(""FooBar"")]
                    private string name;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_InvalidMemberType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_InvalidPropertyType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void NotifyCanExecuteChangedForInvalidTargetError_InvalidCommandType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0016");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForINotifyPropertyChangedAttributeError_InheritingINotifyPropertyChangedAttribute()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0017");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForINotifyPropertyChangedAttributeError_InheritingObservableObjectAttribute()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0017");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForINotifyPropertyChangedAttributeError_WithAlsoObservableObjectAttribute()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                [ObservableObject]
                public partial class A
                {
                }
            }";

        VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0017");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForObservableObjectAttributeError_InheritingINotifyPropertyChangedAttribute()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0018");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForObservableObjectAttributeError_InheritingObservableObjectAttribute()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0018");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForObservableObjectAttributeError_WithAlsoINotifyPropertyChangedAttribute()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                [ObservableObject]
                public partial class A
                {
                }
            }";

        VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0018");
    }

    [TestMethod]
    public void InvalidContainingTypeForObservablePropertyFieldError()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : INotifyPropertyChanged
                {
                    [ObservableProperty]
                    public int number;

                    public event PropertyChangedEventHandler PropertyChanged;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0019");
    }

    [TestMethod]
    public void FieldWithOrphanedDependentObservablePropertyAttributesError_NotifyPropertyChangedFor()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [NotifyPropertyChangedFor("")]
                    public int number;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0020");
    }

    [TestMethod]
    public void FieldWithOrphanedDependentObservablePropertyAttributesError_NotifyCanExecuteChangedFor()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [NotifyCanExecuteChangedFor("")]
                    public int number;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0020");
    }

    [TestMethod]
    public void FieldWithOrphanedDependentObservablePropertyAttributesError_NotifyPropertyChangedRecipients()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel
                {
                    [NotifyPropertyChangedRecipients]
                    public int number;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0020");
    }

    [TestMethod]
    public void FieldWithOrphanedDependentObservablePropertyAttributesError_MultipleUsesStillGenerateOnlyASingleDiagnostic()
    {
        string source = @"
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
                    public int number;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0020");
    }

    [TestMethod]
    public void InvalidAttributeCombinationForObservableRecipientAttributeError()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservableRecipientGenerator>(source, "MVVMTK0021");
    }

    [TestMethod]
    public void InvalidContainingTypeForNotifyPropertyChangedRecipientsFieldError_ObservableObject()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [NotifyPropertyChangedRecipients]
                    public int number;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0022");
    }

    [TestMethod]
    public void MultipleRelayCommandMethodOverloads_WithOverloads()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0023");
    }

    [TestMethod]
    public void MultipleRelayCommandMethodOverloads_WithOverloadInBaseType()
    {
        string source = @"
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
        }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0023");
    }

    [TestMethod]
    public void InvalidObservablePropertyError_Object()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public object property;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0024");
    }

    [TestMethod]
    public void InvalidObservablePropertyError_PropertyChangingEventArgs()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public PropertyChangingEventArgs property;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0024");
    }

    [TestMethod]
    public void InvalidObservablePropertyError_PropertyChangedEventArgs()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public PropertyChangedEventArgs property;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0024");
    }

    [TestMethod]
    public void InvalidObservablePropertyError_CustomTypeDerivedFromPropertyChangedEventArgs()
    {
        string source = @"
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
                    public MyPropertyChangedEventArgs property;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0024");
    }

    [TestMethod]
    public void MissingObservableValidatorInheritanceForNotifyDataErrorInfoError()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0006", "MVVMTK0025");
    }

    [TestMethod]
    public void MissingValidationAttributesForNotifyDataErrorInfoError()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0026");
    }

    [TestMethod]
    public void InvalidTypeForNotifyPropertyChangedRecipientsError()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [NotifyPropertyChangedRecipients]
                public partial class MyViewModel : ObservableObject
                {
                    [ObservableProperty]
                    public int number;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0027");
    }

    [TestMethod]
    public void InvalidTypeForNotifyDataErrorInfoError()
    {
        string source = @"
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [NotifyDataErrorInfo]
                public partial class SampleViewModel : ObservableObject
                {
                    [ObservableProperty]
                    [Required]
                    private string name;
                }
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0006", "MVVMTK0028");
    }

    [TestMethod]
    public void UnnecessaryNotifyPropertyChangedRecipientsWarning_SameType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0029");
    }

    [TestMethod]
    public void UnnecessaryNotifyPropertyChangedRecipientsWarning_BaseType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0029");
    }

    [TestMethod]
    public void UnnecessaryNotifyDataErrorInfoWarning_SameType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0030");
    }

    [TestMethod]
    public void UnnecessaryNotifyDataErrorInfoWarning_BaseType()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0030");
    }

    [TestMethod]
    public void InvalidRelayCommandFlowExceptionsToTaskSchedulerOption()
    {
        string source = @"
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
            }";

        VerifyGeneratedDiagnostics<RelayCommandGenerator>(source, "MVVMTK0031");
    }

    [TestMethod]
    public void ValidObservablePropertyGeneratorScenarios()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel
                {
                    [ObservableProperty]
                    string name;

                    [ObservableProperty]
                    [NotifyPropertyChangedFor(nameof(Name))]
                    int number;
                }
            }";

        VerifySuccessfulGeneration(source);
    }

    /// <summary>
    /// Verifies that all available source generators can run successfully with the input source (including subsequent compilation).
    /// </summary>
    /// <param name="source">The input source to process.</param>
    private static void VerifySuccessfulGeneration(string source)
    {
        IIncrementalGenerator[] generators =
        {
            new IMessengerRegisterAllGenerator(),
            new NullabilityAttributesGenerator(),
            new ObservableObjectGenerator(),
            new INotifyPropertyChangedGenerator(),
            new ObservablePropertyGenerator(),
            new ObservableRecipientGenerator(),
            new ObservableValidatorValidateAllPropertiesGenerator(),
            new RelayCommandGenerator()
        };

        VerifyGeneratedDiagnostics(CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8)), generators, Array.Empty<string>());
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="source">The input source to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static void VerifyGeneratedDiagnostics<TGenerator>(string source, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        IIncrementalGenerator generator = new TGenerator();

        VerifyGeneratedDiagnostics(CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8)), new[] { generator }, diagnosticsIds);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="syntaxTree">The input source to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static void VerifyGeneratedDiagnostics<TGenerator>(SyntaxTree syntaxTree, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        IIncrementalGenerator generator = new TGenerator();

        VerifyGeneratedDiagnostics(syntaxTree, new[] { generator }, diagnosticsIds);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="syntaxTree">The input source tree to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static void VerifyGeneratedDiagnostics(SyntaxTree syntaxTree, IIncrementalGenerator[] generators, string[] generatorDiagnosticsIds)
    {
        Type observableObjectType = typeof(ObservableObject);
        Type validationAttributeType = typeof(ValidationAttribute);

        IEnumerable<MetadataReference> references =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where !assembly.IsDynamic
            let reference = MetadataReference.CreateFromFile(assembly.Location)
            select reference;

        CSharpCompilation compilation = CSharpCompilation.Create(
            "original",
            new SyntaxTree[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generators).WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();

        CollectionAssert.AreEquivalent(generatorDiagnosticsIds, resultingIds.ToArray(), $"resultingIds: {string.Join(", ", resultingIds)}");

        // If the compilation was supposed to succeed, ensure that no further errors were generated
        if (resultingIds.Count == 0)
        {
            // Compute diagnostics for the final compiled output (just include errors)
            List<Diagnostic> outputCompilationDiagnostics = outputCompilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToList();

            Assert.IsTrue(outputCompilationDiagnostics.Count == 0, $"resultingIds:{Environment.NewLine} {string.Join(Environment.NewLine, outputCompilationDiagnostics)}");
        }

        GC.KeepAlive(observableObjectType);
        GC.KeepAlive(validationAttributeType);
    }
}

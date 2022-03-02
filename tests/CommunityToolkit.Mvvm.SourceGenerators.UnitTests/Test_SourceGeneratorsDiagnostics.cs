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
    public void MissingObservableValidatorInheritanceError()
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
    public void InvalidICommandMethodSignatureError()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [ICommand]
                    private string GreetUser() => ""Hello world!"";
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0007");
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
    }

    [TestMethod]
    public void UnsupportedCSharpLanguageVersion_FromObservableValidatorValidateAllPropertiesGenerator()
    {
        string source = @"
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public partial class SampleViewModel : ObservableValidator
                {
                    [Required]
                    public string Name { get; set; }
                }
            }";

        // This is explicitly allowed in C# < 8.0, as it doesn't use any new features
        VerifyGeneratedDiagnostics<ObservableValidatorValidateAllPropertiesGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)));
    }

    [TestMethod]
    public void UnsupportedCSharpLanguageVersion_FromICommandGenerator()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [ICommand]
                    private void GreetUser(object value)
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)),
            "MVVMTK0008");
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

        // This is explicitly allowed in C# < 8.0, as it doesn't use any new features
        VerifyGeneratedDiagnostics<IMessengerRegisterAllGenerator>(
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3)));
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

                    [ICommand(CanExecute = ""Bar"")]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0009");
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

                    [ICommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0010");
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

                    [ICommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0011");
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

                    [ICommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0011");
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

                    [ICommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0011");
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

                    [ICommand(CanExecute = nameof(Foo))]
                    private void GreetUser()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0011");
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

                    [ICommand(CanExecute = nameof(Foo))]
                    private void GreetUser(string name)
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0011");
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

                    [ICommand(CanExecute = nameof(Foo))]
                    private void GreetUser(string name)
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0011");
    }

    [TestMethod]
    public void InvalidICommandAllowConcurrentExecutionsSettings()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [ICommand(AllowConcurrentExecutions = false)]
                    private void GreetUser(User user)
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0012");
    }

    [TestMethod]
    public void InvalidICommandIncludeCancelCommandSettings_SynchronousMethod()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [ICommand(IncludeCancelCommand = true)]
                    private void GreetUser(User user)
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0013");
    }

    [TestMethod]
    public void InvalidICommandIncludeCancelCommandSettings_AsynchronousMethodWithNoCancellationToken()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [ICommand(IncludeCancelCommand = true)]
                    private async Task DoWorkAsync()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0013");
    }

    [TestMethod]
    public void InvalidICommandIncludeCancelCommandSettings_AsynchronousMethodWithParameterAndNoCancellationToken()
    {
        string source = @"
            using CommunityToolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [ICommand(IncludeCancelCommand = true)]
                    private async Task GreetUserAsync(User user)
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0013");
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
        VerifyGeneratedDiagnostics<TGenerator>(CSharpSyntaxTree.ParseText(source), diagnosticsIds);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="syntaxTree">The input source tree to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static void VerifyGeneratedDiagnostics<TGenerator>(SyntaxTree syntaxTree, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
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

        IIncrementalGenerator generator = new TGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator).WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();

        Assert.IsTrue(resultingIds.SetEquals(diagnosticsIds));

        GC.KeepAlive(observableObjectType);
        GC.KeepAlive(validationAttributeType);
    }
}

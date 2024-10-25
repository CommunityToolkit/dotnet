// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    CommunityToolkit.Mvvm.SourceGenerators.AsyncVoidReturningRelayCommandMethodAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.AsyncVoidReturningRelayCommandMethodCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.Mvvm.SourceGenerators.AsyncVoidReturningRelayCommandMethodAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.AsyncVoidReturningRelayCommandMethodCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public class Test_AsyncVoidReturningRelayCommandMethodCodeFixer
{
    [TestMethod]
    public async Task AsyncVoidMethod_FileContainsSystemThreadingTasksUsingDirective()
    {
        string original = """
            using System.Threading.Tasks;
            using CommunityToolkit.Mvvm.Input;

            partial class C
            {
                [RelayCommand]
                private async void Foo()
                {
                }
            }
            """;

        string @fixed = """
            using System.Threading.Tasks;
            using CommunityToolkit.Mvvm.Input;

            partial class C
            {
                [RelayCommand]
                private async Task Foo()
                {
                }
            }
            """;

        CSharpCodeFixTest test = new()
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        test.TestState.AdditionalReferences.Add(typeof(RelayCommand).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,24): error MVVMTK0039: The method C.Foo() annotated with [RelayCommand] is async void (make sure to return a Task type instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 24, 7, 27).WithArguments("C.Foo()")
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task AsyncVoidMethod_FileDoesNotContainSystemThreadingTasksUsingDirective()
    {
        string original = """
            using CommunityToolkit.Mvvm.Input;

            partial class C
            {
                [RelayCommand]
                private async void Foo()
                {
                }
            }
            """;

        string @fixed = """
            using System.Threading.Tasks;
            using CommunityToolkit.Mvvm.Input;

            partial class C
            {
                [RelayCommand]
                private async Task Foo()
                {
                }
            }
            """;

        CSharpCodeFixTest test = new()
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        test.TestState.AdditionalReferences.Add(typeof(RelayCommand).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,24): error MVVMTK0039: The method C.Foo() annotated with [RelayCommand] is async void (make sure to return a Task type instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 24, 6, 27).WithArguments("C.Foo()")
        });

        await test.RunAsync();
    }
}

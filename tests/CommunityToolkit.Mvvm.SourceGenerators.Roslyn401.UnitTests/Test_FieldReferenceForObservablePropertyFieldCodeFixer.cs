// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    CommunityToolkit.Mvvm.SourceGenerators.FieldReferenceForObservablePropertyFieldAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.FieldReferenceForObservablePropertyFieldCodeFixer,
    Microsoft.CodeAnalysis.Testing.Verifiers.MSTestVerifier>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.Mvvm.SourceGenerators.FieldReferenceForObservablePropertyFieldAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.FieldReferenceForObservablePropertyFieldCodeFixer,
    Microsoft.CodeAnalysis.Testing.Verifiers.MSTestVerifier>;

namespace CommunityToolkit.Mvvm.SourceGenerators.Roslyn401.UnitTests;

[TestClass]
public class Test_FieldReferenceForObservablePropertyFieldCodeFixer
{
    [TestMethod]
    public async Task SimpleMemberAccess()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            class C : ObservableObject
            {
                [ObservableProperty]
                private int i;

                void M()
                {
                    _ = i;
                    i = 1;
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            class C : ObservableObject
            {
                [ObservableProperty]
                private int i;

                void M()
                {
                    _ = I;
                    I = 1;
                }
            }
            """;

        CSharpCodeFixTest test = new()
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(10,13): warning MVVMTK0034: The field C.i is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(10, 13, 10, 14).WithArguments("C.i"),

            // /0/Test0.cs(11,9): warning MVVMTK0034: The field C.i is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(11, 9, 11, 10).WithArguments("C.i")
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(10,13): error CS0103: The name 'I' does not exist in the current context
            DiagnosticResult.CompilerError("CS0103").WithSpan(10, 13, 10, 14).WithArguments("I"),

            // /0/Test0.cs(11,9): error CS0103: The name 'I' does not exist in the current context
            DiagnosticResult.CompilerError("CS0103").WithSpan(11, 9, 11, 10).WithArguments("I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task QualifiedMemberAccess()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            class C : ObservableObject
            {
                [ObservableProperty]
                private int i;

                void M()
                {
                    _ = this.i;
                    this.i = 1;
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            class C : ObservableObject
            {
                [ObservableProperty]
                private int i;

                void M()
                {
                    _ = this.I;
                    this.I = 1;
                }
            }
            """;

        CSharpCodeFixTest test = new()
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(10,13): warning MVVMTK0034: The field C.i is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(10, 13, 10, 19).WithArguments("C.i"),

            // /0/Test0.cs(11,9): warning MVVMTK0034: The field C.i is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(11, 9, 11, 15).WithArguments("C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(10,18): error CS1061: 'C' does not contain a definition for 'I' and no accessible extension method 'I' accepting a first argument of type 'C' could be found (are you missing a using directive or an assembly reference?)
            DiagnosticResult.CompilerError("CS1061").WithSpan(10, 18, 10, 19).WithArguments("C", "I"),

            // /0/Test0.cs(11,14): error CS1061: 'C' does not contain a definition for 'I' and no accessible extension method 'I' accepting a first argument of type 'C' could be found (are you missing a using directive or an assembly reference?)
            DiagnosticResult.CompilerError("CS1061").WithSpan(11, 14, 11, 15).WithArguments("C", "I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task ExternalQualifiedMemberAccess()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            class C : ObservableObject
            {
                [ObservableProperty]
                public int i;
            }

            class D
            {
                void M()
                {
                    var c = new C();
                    c.i = 1;
                    _ = c.i;
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            class C : ObservableObject
            {
                [ObservableProperty]
                public int i;
            }

            class D
            {
                void M()
                {
                    var c = new C();
                    c.I = 1;
                    _ = c.I;
                }
            }
            """;

        CSharpCodeFixTest test = new()
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(14,9): warning MVVMTK0034: The field C.i is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(14, 9, 14, 12).WithArguments("C.i"),

            // /0/Test0.cs(15,13): warning MVVMTK0034: The field C.i is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(15, 13, 15, 16).WithArguments("C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(14,11): error CS1061: 'C' does not contain a definition for 'I' and no accessible extension method 'I' accepting a first argument of type 'C' could be found (are you missing a using directive or an assembly reference?)
            DiagnosticResult.CompilerError("CS1061").WithSpan(14, 11, 14, 12).WithArguments("C", "I"),

            // /0/Test0.cs(15,15): error CS1061: 'C' does not contain a definition for 'I' and no accessible extension method 'I' accepting a first argument of type 'C' could be found (are you missing a using directive or an assembly reference?)
            DiagnosticResult.CompilerError("CS1061").WithSpan(15, 15, 15, 16).WithArguments("C", "I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task ExternalConditionalMemberAccess()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            class C : ObservableObject
            {
                [ObservableProperty]
                public int i;
            }

            class D
            {
                void M()
                {
                    var c = new C();
                    _ = c?.i;
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            class C : ObservableObject
            {
                [ObservableProperty]
                public int i;
            }

            class D
            {
                void M()
                {
                    var c = new C();
                    _ = c?.I;
                }
            }
            """;

        CSharpCodeFixTest test = new()
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(14,15): warning MVVMTK0034: The field C.i is annotated with [ObservableProperty] and should not be directly referenced (use the generated property instead)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(14, 15, 14, 17).WithArguments("C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(14,15): error CS1061: 'C' does not contain a definition for 'I' and no accessible extension method 'I' accepting a first argument of type 'C' could be found (are you missing a using directive or an assembly reference?)
            DiagnosticResult.CompilerError("CS1061").WithSpan(14, 15, 14, 17).WithArguments("C", "I"),
        });

        await test.RunAsync();
    }
}

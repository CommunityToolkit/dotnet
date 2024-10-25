// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers.CSharpCodeFixWithLanguageVersionTest<
    CommunityToolkit.Mvvm.SourceGenerators.UseObservablePropertyOnPartialPropertyAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.UsePartialPropertyForObservablePropertyCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.Mvvm.SourceGenerators.UseObservablePropertyOnPartialPropertyAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.UsePartialPropertyForObservablePropertyCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public class Test_UsePartialPropertyForObservablePropertyCodeFixer
{
    [TestMethod]
    public async Task SimpleFieldWithNoReferences()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                private int i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                public partial int I { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(5,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(5, 6, 5, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(6, 24, 6, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithNoReferences_WithAdditionalAttributes1()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                [NotifyPropertyChangedFor("hello")]
                private int i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                [NotifyPropertyChangedFor("hello")]
                public partial int I { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(5,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(5, 6, 5, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(7, 24, 7, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithNoReferences_WithAdditionalAttributes2()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                [NotifyPropertyChangedFor("hello1")]
                [NotifyCanExecuteChangedFor("hello2")]
                private int i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                [NotifyPropertyChangedFor("hello1")]
                [NotifyCanExecuteChangedFor("hello2")]
                public partial int I { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(5,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(5, 6, 5, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 24, 8, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithNoReferences_WithAdditionalAttributes3()
    {
        string original = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                [field: MinLength(1)]
                [property: MinLength(2)]
                private int i;
            }
            """;

        string @fixed = """
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                [field: MinLength(1)]
                [MinLength(2)]
                public partial int I { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 6, 6, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(9,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(9, 24, 9, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithNoReferences_WithAdditionalAttributes4()
    {
        string original = """
            using System;
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                [Test("This is on the field")]
                [field: Test("This is also on a the field, but using 'field:'")]
                [property: Test("This is on the property")]
                [get: Test("This is on the getter")]
                [set: Test("This is also on the setter")]
                [set: Test("This is a second one on the setter")]
                [ignored: Test("This should be ignored, but still carried over")]
                private int i;
            }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class TestAttribute(string text) : Attribute;
            """;

        string @fixed = """
            using System;
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                [field: Test("This is on the field")]
                [field: Test("This is also on a the field, but using 'field:'")]
                [Test("This is on the property")]
                [ignored: Test("This should be ignored, but still carried over")]
                public partial int I { [Test("This is on the getter")]
                    get; [Test("This is also on the setter")]
                [Test("This is a second one on the setter")]
                    set;
                }
            }
            
            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class TestAttribute(string text) : Attribute;
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 6, 7, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(12,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(12, 24, 12, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithNoReferences_WithSimpleComment()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                // This is a comment
                [ObservableProperty]
                private int i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                // This is a comment
                [ObservableProperty]
                public partial int I { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 6, 6, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(7, 24, 7, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithNoReferences_WithTwoLineComment()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                // This is a comment.
                // This is more comment.
                [ObservableProperty]
                private int i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                // This is a comment.
                // This is more comment.
                [ObservableProperty]
                public partial int I { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 6, 7, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 24, 8, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithNoReferences_WithXmlComment()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                /// <summary>
                /// Blah blah blah.
                /// </summary>
                /// <remarks>Blah blah blah.</remarks>
                [ObservableProperty]
                private int i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                /// <summary>
                /// Blah blah blah.
                /// </summary>
                /// <remarks>Blah blah blah.</remarks>
                [ObservableProperty]
                public partial int I { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.TestState.AdditionalReferences.Add(typeof(ObservableObject).Assembly);
        test.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(9,6): info MVVMTK0042: The field C.C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(9, 6, 9, 24).WithArguments("C", "C.i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(10,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(10, 24, 10, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }
}

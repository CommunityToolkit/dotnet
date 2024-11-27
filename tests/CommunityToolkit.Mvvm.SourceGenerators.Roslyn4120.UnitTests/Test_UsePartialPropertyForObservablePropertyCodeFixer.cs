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
            // /0/Test0.cs(6,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 17, 6, 18).WithArguments("C", "i"),
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
            // /0/Test0.cs(7,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 17, 7, 18).WithArguments("C", "i"),
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
            // /0/Test0.cs(8,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(8, 17, 8, 18).WithArguments("C", "i"),
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
            // /0/Test0.cs(9,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(9, 17, 9, 18).WithArguments("C", "i"),
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
            // /0/Test0.cs(15,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(15, 17, 15, 18).WithArguments("C", "i"),
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
            // /0/Test0.cs(7,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 17, 7, 18).WithArguments("C", "i"),
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
            // /0/Test0.cs(8,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(8, 17, 8, 18).WithArguments("C", "i"),
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
            // /0/Test0.cs(10,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(10, 17, 10, 18).WithArguments("C", "i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(10,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(10, 24, 10, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithSomeReferences()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                private int i;

                public void M()
                {
                    i = 42;
                }

                public int N() => i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                public partial int I { get; set; }

                public void M()
                {
                    I = 42;
                }
            
                public int N() => I;
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
            // /0/Test0.cs(6,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 17, 6, 18).WithArguments("C", "i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(6, 24, 6, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleField_WithInitializer1()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                private int i = 42;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                public partial int I { get; set; } = 42;
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
            // /0/Test0.cs(6,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 17, 6, 18).WithArguments("C", "i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(6, 24, 6, 25).WithArguments("C.I"),

            // /0/Test0.cs(6,24): error CS8050: Only auto-implemented properties can have initializers.
            DiagnosticResult.CompilerError("CS8050").WithSpan(6, 24, 6, 25),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleField_WithInitializer2()
    {
        string original = """
            using System.Collections.Generic;
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                private ICollection<string> items = ["A", "B", "C"];
            }
            """;

        string @fixed = """
            using System.Collections.Generic;
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                public partial ICollection<string> Items { get; set; } = ["A", "B", "C"];
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
            // /0/Test0.cs(7,33): info MVVMTK0042: The field C.items using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 33, 7, 38).WithArguments("C", "items"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,40): error CS8050: Only auto-implemented properties can have initializers.
            DiagnosticResult.CompilerError("CS8050").WithSpan(7, 40, 7, 45),

            // /0/Test0.cs(7,40): error CS9248: Partial property 'C.Items' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(7, 40, 7, 45).WithArguments("C.Items"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleField_WithInitializer3()
    {
        string original = """
            using System.Collections.Generic;
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                private ICollection<string> items = new List<string> { "A", "B", "C" };
            }
            """;

        string @fixed = """
            using System.Collections.Generic;
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                public partial ICollection<string> Items { get; set; } = new List<string> { "A", "B", "C" };
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
            // /0/Test0.cs(7,33): info MVVMTK0042: The field C.items using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 33, 7, 38).WithArguments("C", "items"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,40): error CS8050: Only auto-implemented properties can have initializers.
            DiagnosticResult.CompilerError("CS8050").WithSpan(7, 40, 7, 45),

            // /0/Test0.cs(7,40): error CS9248: Partial property 'C.Items' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(7, 40, 7, 45).WithArguments("C.Items"),
        });

        await test.RunAsync();
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/971
    [TestMethod]
    public async Task SimpleField_WithNoReferences_WithRequiredModifier()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                internal required string foo;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                public required partial string Foo { get; set; }
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
            // /0/Test0.cs(6,30): info MVVMTK0042: The field C.foo using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 30, 6, 33).WithArguments("C", "foo"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,36): error CS9248: Partial property 'C.Foo' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(6, 36, 6, 39).WithArguments("C.Foo"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFieldWithSomeReferences_WithSomeThisExpressions()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            partial class C : ObservableObject
            {
                [ObservableProperty]
                private int i;

                public void M()
                {
                    i = 42;
                    this.i = 42;
                }

                public int N() => i;

                public int P() => this.i + Q(i) + Q(this.i);

                private int Q(int i) => this.i + i;
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;
            
            partial class C : ObservableObject
            {
                [ObservableProperty]
                public partial int I { get; set; }
            
                public void M()
                {
                    I = 42;
                    I = 42;
                }
            
                public int N() => I;
            
                public int P() => I + Q(I) + Q(I);
            
                private int Q(int i) => I + i;
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
            // /0/Test0.cs(6,17): info MVVMTK0042: The field C.i using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 17, 6, 18).WithArguments("C", "i"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(6,24): error CS9248: Partial property 'C.I' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(6, 24, 6, 25).WithArguments("C.I"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFields_WithMultipleAttributes_SingleProperty()
    {
        string original = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            public partial class Class1 : ObservableObject
            {
                [ObservableProperty, NotifyPropertyChangedFor("Age")] private string name = String.Empty;
            }
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;
            
            public partial class Class1 : ObservableObject
            {
                [ObservableProperty, NotifyPropertyChangedFor("Age")]
                public partial string Name { get; set; } = String.Empty;
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
            // /0/Test0.cs(6,74): info MVVMTK0042: The field Class1.name using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 74, 6, 78).WithArguments("Class1", "name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,27): error CS8050: Only auto-implemented properties, or properties that use the 'field' keyword, can have initializers.
            DiagnosticResult.CompilerError("CS8050").WithSpan(7, 27, 7, 31),

            // /0/Test0.cs(7,27): error CS9248: Partial property 'Class1.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(7, 27, 7, 31).WithArguments("Class1.Name"),
        });

        await test.RunAsync();
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/1007
    [TestMethod]
    public async Task SimpleFields_WithMultipleAttributes_WithNoBlankLines()
    {
        string original = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            public partial class Class1 : ObservableObject
            {
                [ObservableProperty, NotifyPropertyChangedFor("Age")] private string name = String.Empty;
                [ObservableProperty] private int age;
            }
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;
            
            public partial class Class1 : ObservableObject
            {
                [ObservableProperty, NotifyPropertyChangedFor("Age")]
                public partial string Name { get; set; } = String.Empty;

                [ObservableProperty]
                public partial int Age { get; set; }
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
            // /0/Test0.cs(6,74): info MVVMTK0042: The field Class1.name using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(6, 74, 6, 78).WithArguments("Class1", "name"),
            
            // /0/Test0.cs(7,38): info MVVMTK0042: The field Class1.age using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 38, 7, 41).WithArguments("Class1", "age"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(7,27): error CS8050: Only auto-implemented properties, or properties that use the 'field' keyword, can have initializers.
            DiagnosticResult.CompilerError("CS8050").WithSpan(7, 27, 7, 31),

            // /0/Test0.cs(7,27): error CS9248: Partial property 'Class1.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(7, 27, 7, 31).WithArguments("Class1.Name"),

            // /0/Test0.cs(10,24): error CS9248: Partial property 'Class1.Age' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(10, 24, 10, 27).WithArguments("Class1.Age"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFields_WithMultipleAttributes_WithMixedBuckets_1()
    {
        string original = """
            using System;
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            public partial class Class1 : ObservableObject
            {
                // Leading trivia
                [ObservableProperty, NotifyPropertyChangedFor("A"), Display]
                [NotifyPropertyChangedFor("B")]
                private string _name;
            }
            """;

        string @fixed = """
            using System;
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;
            
            public partial class Class1 : ObservableObject
            {
                // Leading trivia
                [ObservableProperty, NotifyPropertyChangedFor("A"), Display]
                [NotifyPropertyChangedFor("B")]
                public partial string Name { get; set; }
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
            // /0/Test0.cs(10,20): info MVVMTK0042: The field Class1._name using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(10, 20, 10, 25).WithArguments("Class1", "_name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(10,27): error CS9248: Partial property 'Class1.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(10, 27, 10, 31).WithArguments("Class1.Name"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleFields_WithMultipleAttributes_WithMixedBuckets_2()
    {
        string original = """
            using System;
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;

            public partial class Class1 : ObservableObject
            {
                // Leading trivia
                [NotifyPropertyChangedFor("B")]
                [ObservableProperty, NotifyPropertyChangedFor("A"), Display, Test]
                [NotifyPropertyChangedFor("C")]
                [property: UIHint("name"), Test]
                private string name;
            }

            public class TestAttribute : Attribute;
            """;

        string @fixed = """
            using System;
            using System.ComponentModel.DataAnnotations;
            using CommunityToolkit.Mvvm.ComponentModel;
            
            public partial class Class1 : ObservableObject
            {
                // Leading trivia
                [NotifyPropertyChangedFor("B")]
                [ObservableProperty, NotifyPropertyChangedFor("A"), Display]
                [field: Test]
                [NotifyPropertyChangedFor("C")]
                [UIHint("name"), Test]
                public partial string Name { get; set; }
            }

            public class TestAttribute : Attribute;
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
            // /0/Test0.cs(12,20): info MVVMTK0042: The field Class1.name using [ObservableProperty] can be converted to a partial property instead, which is recommended (doing so improves the developer experience and allows other generators and analyzers to correctly see the generated property as well)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(12, 20, 12, 24).WithArguments("Class1", "name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(13,27): error CS9248: Partial property 'Class1.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(13, 27, 13, 31).WithArguments("Class1.Name"),
        });

        await test.RunAsync();
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers.CSharpCodeFixWithLanguageVersionTest<
    CommunityToolkit.Mvvm.SourceGenerators.UseObservablePropertyOnSemiAutoPropertyAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.UsePartialPropertyForSemiAutoPropertyCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.Mvvm.SourceGenerators.UseObservablePropertyOnSemiAutoPropertyAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.UsePartialPropertyForSemiAutoPropertyCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace CommunityToolkit.Mvvm.SourceGenerators.UnitTests;

[TestClass]
public class Test_UseObservablePropertyOnSemiAutoPropertyCodeFixer
{
    [TestMethod]
    public async Task SimpleProperty()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                public string Name
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                [ObservableProperty]
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
            // /0/Test0.cs(7,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Name can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 19, 7, 23).WithArguments("MyApp.SampleViewModel", "Name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 31).WithArguments("MyApp.SampleViewModel.Name"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_WithLeadingTrivia()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                /// <summary>
                /// This is a property.
                /// </summary>
                public string Name
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                /// <summary>
                /// This is a property.
                /// </summary>
                [ObservableProperty]
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
            // /0/Test0.cs(10,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Name can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(10, 19, 10, 23).WithArguments("MyApp.SampleViewModel", "Name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 31).WithArguments("MyApp.SampleViewModel.Name"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_WithLeadingTrivia_AndAttributes()
    {
        string original = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                /// <summary>
                /// This is a property.
                /// </summary>
                [Test("Targeting property")]
                [field: Test("Targeting field")]
                public string Name
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }
            }

            public class TestAttribute(string text) : Attribute;
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                /// <summary>
                /// This is a property.
                /// </summary>
                [ObservableProperty]
                [Test("Targeting property")]
                [field: Test("Targeting field")]
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
            // /0/Test0.cs(13,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Name can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(13, 19, 13, 23).WithArguments("MyApp.SampleViewModel", "Name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 31).WithArguments("MyApp.SampleViewModel.Name"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_Multiple()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                public string FirstName
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }

                public string LastName
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                [ObservableProperty]
                public partial string FirstName { get; set; }

                [ObservableProperty]
                public partial string LastName { get; set; }
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
            // /0/Test0.cs(7,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.FirstName can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 19, 7, 28).WithArguments("MyApp.SampleViewModel", "FirstName"),

            // /0/Test0.cs(13,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.LastName can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(13, 19, 13, 27).WithArguments("MyApp.SampleViewModel", "LastName"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 31).WithArguments("MyApp.SampleViewModel.Name"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_WithinPartialType()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                public string Name
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                [ObservableProperty]
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
            // /0/Test0.cs(7,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Name can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 19, 7, 23).WithArguments("MyApp.SampleViewModel", "Name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 31).WithArguments("MyApp.SampleViewModel.Name"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_WithinPartialType_Multiple()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                public string FirstName
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }

                public string LastName
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                [ObservableProperty]
                public partial string FirstName { get; set; }

                [ObservableProperty]
                public partial string LastName { get; set; }
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
            // /0/Test0.cs(7,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.FirstName can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(7, 19, 7, 28).WithArguments("MyApp.SampleViewModel", "FirstName"),

            // /0/Test0.cs(13,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.LastName can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(13, 19, 13, 27).WithArguments("MyApp.SampleViewModel", "LastName"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 31).WithArguments("MyApp.SampleViewModel.Name"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_WithinPartialType_Multiple_MixedScenario()
    {
        string original = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                [Test("This is an attribute")]
                public string Prop1
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }

                // Single comment
                public string Prop2
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }

                /// <summary>
                /// This is a property.
                /// </summary>
                public string Prop3
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }

                /// <summary>
                /// This is another property.
                /// </summary>
                [Test("Another attribute")]
                public string Prop4
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }

                // Some other single comment
                [Test("Yet another attribute")]
                public string Prop5
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }
            }

            public class TestAttribute(string text) : Attribute;
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : ObservableObject
            {
                [ObservableAttribute]
                [Test("This is an attribute")]
                public partial string Prop1 { get; set; }

                // Single comment
                [ObservableAttribute]
                public partial string Prop2 { get; set; }

                /// <summary>
                /// This is a property.
                /// </summary>
                [ObservableAttribute]
                public partial string Prop3 { get; set; }

                /// <summary>
                /// This is another property.
                /// </summary>
                [ObservableAttribute]
                [Test("Another attribute")]
                public partial string Prop4 { get; set; }

                // Some other single comment
                [ObservableAttribute]
                [Test("Yet another attribute")]
                public partial string Prop5 { get; set; }
            }

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
            // /0/Test0.cs(9,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Prop1 can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(9, 19, 9, 24).WithArguments("MyApp.SampleViewModel", "Prop1"),

            // /0/Test0.cs(16,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Prop2 can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(16, 19, 16, 24).WithArguments("MyApp.SampleViewModel", "Prop2"),

            // /0/Test0.cs(25,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Prop3 can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(25, 19, 25, 24).WithArguments("MyApp.SampleViewModel", "Prop3"),

            // /0/Test0.cs(35,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Prop4 can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(35, 19, 35, 24).WithArguments("MyApp.SampleViewModel", "Prop4"),

            // /0/Test0.cs(43,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Prop5 can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(43, 19, 43, 24).WithArguments("MyApp.SampleViewModel", "Prop5"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 31).WithArguments("MyApp.SampleViewModel.Name"),
        });

        await test.RunAsync();
    }
}

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
    public async Task SimplePropertyWithBlockAccessorSyntax()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                public string Name
                {
                    get
                    {
                        return field;
                    }
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
    public async Task SimplePropertyWithNestedBlockSyntax()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                public string Name
                {
                    get
                    {
                        {
                            return field;
                        }
                    }
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
    public async Task SimpleProperty_WithSemicolonTokenGetAccessor()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                public string Name
                {
                    get;
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
    public async Task SimpleProperty_WithMissingUsingDirective()
    {
        string original = """
            namespace MyApp;

            public class SampleViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
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

            public partial class SampleViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
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
            // /0/Test0.cs(5,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Name can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(5, 19, 5, 23).WithArguments("MyApp.SampleViewModel", "Name"),
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
            // /0/Test0.cs(11,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(11, 27, 11, 31).WithArguments("MyApp.SampleViewModel.Name"),
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
            // /0/Test0.cs(13,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Name can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(13, 19, 13, 23).WithArguments("MyApp.SampleViewModel", "Name"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(14,27): error CS9248: Partial property 'SampleViewModel.Name' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(14, 27, 14, 31).WithArguments("MyApp.SampleViewModel.Name"),
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
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.FirstName' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 36).WithArguments("MyApp.SampleViewModel.FirstName"),

            // /0/Test0.cs(11,27): error CS9248: Partial property 'SampleViewModel.LastName' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(11, 27, 11, 35).WithArguments("MyApp.SampleViewModel.LastName"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_Multiple_OnlyTriggersOnFirstOne()
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

                private string _lastName;

                public string LastName
                {
                    get => _lastName;
                    set => SetProperty(ref _lastName, value);
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

                private string _lastName;

                public string LastName
                {
                    get => _lastName;
                    set => SetProperty(ref _lastName, value);
                }
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
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.FirstName' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 36).WithArguments("MyApp.SampleViewModel.FirstName"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_Multiple_OnlyTriggersOnSecondOne()
    {
        string original = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public class SampleViewModel : ObservableObject
            {
                private string _firstName;

                public string FirstName
                {
                    get => _firstName;
                    set => SetProperty(ref _firstName, value);
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
                private string _firstName;

                public string FirstName
                {
                    get => _firstName;
                    set => SetProperty(ref _firstName, value);
                }

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
            // /0/Test0.cs(15,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.LastName can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(15, 19, 15, 27).WithArguments("MyApp.SampleViewModel", "LastName"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(16,27): error CS9248: Partial property 'SampleViewModel.LastName' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(16, 27, 16, 35).WithArguments("MyApp.SampleViewModel.LastName"),
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
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.FirstName' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 36).WithArguments("MyApp.SampleViewModel.FirstName"),

            // /0/Test0.cs(11,27): error CS9248: Partial property 'SampleViewModel.LastName' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(11, 27, 11, 35).WithArguments("MyApp.SampleViewModel.LastName"),
        });

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_Multiple_WithMissingUsingDirective()
    {
        string original = """
            namespace MyApp;

            public partial class SampleViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
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

                public string PhoneNumber
                {
                    get;
                    set => SetProperty(ref field, value);
                }
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            namespace MyApp;

            public partial class SampleViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
            {
                [ObservableProperty]
                public partial string FirstName { get; set; }

                [ObservableProperty]
                public partial string LastName { get; set; }

                [ObservableProperty]
                public partial string PhoneNumber { get; set; }
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
            // /0/Test0.cs(5,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.FirstName can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(5, 19, 5, 28).WithArguments("MyApp.SampleViewModel", "FirstName"),

            // /0/Test0.cs(11,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.LastName can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(11, 19, 11, 27).WithArguments("MyApp.SampleViewModel", "LastName"),

            // /0/Test0.cs(17,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.PhoneNumber can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(17, 19, 17, 30).WithArguments("MyApp.SampleViewModel", "PhoneNumber"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(8,27): error CS9248: Partial property 'SampleViewModel.FirstName' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(8, 27, 8, 36).WithArguments("MyApp.SampleViewModel.FirstName"),

            // /0/Test0.cs(11,27): error CS9248: Partial property 'SampleViewModel.LastName' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(11, 27, 11, 35).WithArguments("MyApp.SampleViewModel.LastName"),

            // /0/Test0.cs(14,27): error CS9248: Partial property 'SampleViewModel.PhoneNumber' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(14, 27, 14, 38).WithArguments("MyApp.SampleViewModel.PhoneNumber"),
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

                [Test("Attribute without trivia")]
                public string Prop6
                {
                    get => field;
                    set => SetProperty(ref field, value);
                }

                public string Prop7
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
                [ObservableProperty]
                [Test("This is an attribute")]
                public partial string Prop1 { get; set; }

                // Single comment
                [ObservableProperty]
                public partial string Prop2 { get; set; }

                /// <summary>
                /// This is a property.
                /// </summary>
                [ObservableProperty]
                public partial string Prop3 { get; set; }

                /// <summary>
                /// This is another property.
                /// </summary>
                [ObservableProperty]
                [Test("Another attribute")]
                public partial string Prop4 { get; set; }

                // Some other single comment
                [ObservableProperty]
                [Test("Yet another attribute")]
                public partial string Prop5 { get; set; }

                [ObservableProperty]
                [Test("Attribute without trivia")]
                public partial string Prop6 { get; set; }

                [ObservableProperty]
                public partial string Prop7 { get; set; }
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

            // /0/Test0.cs(50,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Prop6 can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(50, 19, 50, 24).WithArguments("MyApp.SampleViewModel", "Prop6"),

            // /0/Test0.cs(56,19): info MVVMTK0056: The semi-auto property MyApp.SampleViewModel.Prop7 can be converted to a partial property using [ObservableProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(56, 19, 56, 24).WithArguments("MyApp.SampleViewModel", "Prop7"),
        });

        test.FixedState.ExpectedDiagnostics.AddRange(new[]
        {
            // /0/Test0.cs(10,27): error CS9248: Partial property 'SampleViewModel.Prop1' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(10, 27, 10, 32).WithArguments("MyApp.SampleViewModel.Prop1"),

            // /0/Test0.cs(14,27): error CS9248: Partial property 'SampleViewModel.Prop2' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(14, 27, 14, 32).WithArguments("MyApp.SampleViewModel.Prop2"),

            // /0/Test0.cs(20,27): error CS9248: Partial property 'SampleViewModel.Prop3' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(20, 27, 20, 32).WithArguments("MyApp.SampleViewModel.Prop3"),

            // /0/Test0.cs(27,27): error CS9248: Partial property 'SampleViewModel.Prop4' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(27, 27, 27, 32).WithArguments("MyApp.SampleViewModel.Prop4"),

            // /0/Test0.cs(32,27): error CS9248: Partial property 'SampleViewModel.Prop5' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(32, 27, 32, 32).WithArguments("MyApp.SampleViewModel.Prop5"),

            // /0/Test0.cs(36,27): error CS9248: Partial property 'SampleViewModel.Prop6' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(36, 27, 36, 32).WithArguments("MyApp.SampleViewModel.Prop6"),

            // /0/Test0.cs(39,27): error CS9248: Partial property 'SampleViewModel.Prop7' must have an implementation part.
            DiagnosticResult.CompilerError("CS9248").WithSpan(39, 27, 39, 32).WithArguments("MyApp.SampleViewModel.Prop7"),
        });

        await test.RunAsync();
    }
}

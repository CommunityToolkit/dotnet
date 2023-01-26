// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    CommunityToolkit.Mvvm.SourceGenerators.ClassUsingAttributeInsteadOfInheritanceAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.ClassUsingAttributeInsteadOfInheritanceCodeFixer,
    Microsoft.CodeAnalysis.Testing.Verifiers.MSTestVerifier>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.Mvvm.SourceGenerators.ClassUsingAttributeInsteadOfInheritanceAnalyzer,
    CommunityToolkit.Mvvm.CodeFixers.ClassUsingAttributeInsteadOfInheritanceCodeFixer,
    Microsoft.CodeAnalysis.Testing.Verifiers.MSTestVerifier>;

namespace CommunityToolkit.Mvvm.SourceGenerators.Roslyn401.UnitTests;

[TestClass]
public class ClassUsingAttributeInsteadOfInheritanceCodeFixer
{
    [TestMethod]
    [DataRow("INotifyPropertyChanged", "MVVMTK0032")]
    [DataRow("ObservableObject", "MVVMTK0033")]
    public async Task SingleAttributeList(string attributeTypeName, string diagnosticId)
    {
        string original = $$"""
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [{{attributeTypeName}}]
            class C
            {
            }
            """;

        string @fixed = """
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            class C : ObservableObject
            {
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
            // /0/Test0.cs(5,15): warning <DIAGNOSTIC_ID>: The type C is using the <ATTRIBUTE_TYPE> attribute while having no base type, and it should instead inherit from ObservableObject
            CSharpCodeFixVerifier.Diagnostic(diagnosticId).WithSpan(5, 7, 5, 8).WithArguments("C")
        });

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("INotifyPropertyChanged", "MVVMTK0032")]
    [DataRow("ObservableObject", "MVVMTK0033")]
    public async Task SingleAttributeList_WithOtherInterface(string attributeTypeName, string diagnosticId)
    {
        string original = $$"""
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [{{attributeTypeName}}]
            class C : IDisposable
            {
                public void Dispose()
                {
                }
            }
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            class C : ObservableObject, IDisposable
            {
                public void Dispose()
                {
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
            // /0/Test0.cs(5,15): warning <DIAGNOSTIC_ID>: The type C is using the <ATTRIBUTE_TYPE> attribute while having no base type, and it should instead inherit from ObservableObject
            CSharpCodeFixVerifier.Diagnostic(diagnosticId).WithSpan(6, 7, 6, 8).WithArguments("C")
        });

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("INotifyPropertyChanged", "MVVMTK0032")]
    [DataRow("ObservableObject", "MVVMTK0033")]
    public async Task MultipleAttributeLists_OneBeforeTarget(string attributeTypeName, string diagnosticId)
    {
        string original = $$"""
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [Test]
            [{{attributeTypeName}}]
            class C
            {
            }

            class TestAttribute : Attribute
            {
            }
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [Test]
            class C : ObservableObject
            {
            }

            class TestAttribute : Attribute
            {
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
            // /0/Test0.cs(5,15): warning <DIAGNOSTIC_ID>: The type C is using the <ATTRIBUTE_TYPE> attribute while having no base type, and it should instead inherit from ObservableObject
            CSharpCodeFixVerifier.Diagnostic(diagnosticId).WithSpan(7, 7, 7, 8).WithArguments("C")
        });

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("INotifyPropertyChanged", "MVVMTK0032")]
    [DataRow("ObservableObject", "MVVMTK0033")]
    public async Task MultipleAttributeLists_OneAfterTarget(string attributeTypeName, string diagnosticId)
    {
        string original = $$"""
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [{{attributeTypeName}}]
            [Test]
            class C
            {
            }

            class TestAttribute : Attribute
            {
            }
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [Test]
            class C : ObservableObject
            {
            }

            class TestAttribute : Attribute
            {
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
            // /0/Test0.cs(5,15): warning <DIAGNOSTIC_ID>: The type C is using the <ATTRIBUTE_TYPE> attribute while having no base type, and it should instead inherit from ObservableObject
            CSharpCodeFixVerifier.Diagnostic(diagnosticId).WithSpan(7, 7, 7, 8).WithArguments("C")
        });

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("INotifyPropertyChanged", "MVVMTK0032")]
    [DataRow("ObservableObject", "MVVMTK0033")]
    public async Task MultipleAttributeLists_OneBeforeAndOneAfterTarget(string attributeTypeName, string diagnosticId)
    {
        string original = $$"""
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [Test]
            [{{attributeTypeName}}]
            [Test]
            class C
            {
            }

            [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
            class TestAttribute : Attribute
            {
            }
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [Test]
            [Test]
            class C : ObservableObject
            {
            }

            [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
            class TestAttribute : Attribute
            {
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
            // /0/Test0.cs(5,15): warning <DIAGNOSTIC_ID>: The type C is using the <ATTRIBUTE_TYPE> attribute while having no base type, and it should instead inherit from ObservableObject
            CSharpCodeFixVerifier.Diagnostic(diagnosticId).WithSpan(8, 7, 8, 8).WithArguments("C")
        });

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("INotifyPropertyChanged", "MVVMTK0032")]
    [DataRow("ObservableObject", "MVVMTK0033")]
    public async Task MultipleAttributesInAttributeList(string attributeTypeName, string diagnosticId)
    {
        string original = $$"""
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [Test, {{attributeTypeName}}]
            class C
            {
            }

            class TestAttribute : Attribute
            {
            }
            """;

        string @fixed = """
            using System;
            using CommunityToolkit.Mvvm.ComponentModel;

            // This is some trivia
            [Test]
            class C : ObservableObject
            {
            }

            class TestAttribute : Attribute
            {
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
            // /0/Test0.cs(5,15): warning <DIAGNOSTIC_ID>: The type C is using the <ATTRIBUTE_TYPE> attribute while having no base type, and it should instead inherit from ObservableObject
            CSharpCodeFixVerifier.Diagnostic(diagnosticId).WithSpan(6, 7, 6, 8).WithArguments("C")
        });

        await test.RunAsync();
    }
}

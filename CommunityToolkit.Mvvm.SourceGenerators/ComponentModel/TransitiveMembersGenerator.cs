// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <summary>
/// A source generator for a given attribute type.
/// </summary>
public abstract partial class TransitiveMembersGenerator : ISourceGenerator
{
    /// <summary>
    /// The fully qualified name of the attribute type to look for.
    /// </summary>
    private readonly string attributeTypeFullName;

    /// <summary>
    /// The name of the attribute type to look for.
    /// </summary>
    private readonly string attributeTypeName;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransitiveMembersGenerator"/> class.
    /// </summary>
    /// <param name="attributeTypeFullName">The fully qualified name of the attribute type to look for.</param>
    protected TransitiveMembersGenerator(string attributeTypeFullName)
    {
        this.attributeTypeFullName = attributeTypeFullName;
        this.attributeTypeName = attributeTypeFullName.Split('.').Last();
    }

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when the generation failed for a given type.
    /// </summary>
    protected abstract DiagnosticDescriptor TargetTypeErrorDescriptor { get; }

    /// <inheritdoc/>
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(this.attributeTypeFullName));
    }

    /// <inheritdoc/>
    public void Execute(GeneratorExecutionContext context)
    {
        // Get the syntax receiver with the candidate nodes
        if (context.SyntaxContextReceiver is not SyntaxReceiver syntaxReceiver ||
            syntaxReceiver.GatheredInfo.Count == 0)
        {
            return;
        }

        // Validate the language version
        if (context.ParseOptions is not CSharpParseOptions { LanguageVersion: >= LanguageVersion.CSharp9 })
        {
            context.ReportDiagnostic(Diagnostic.Create(UnsupportedCSharpLanguageVersionError, null));
        }

        // Load the syntax tree with the members to generate
        SyntaxTree sourceSyntaxTree = LoadSourceSyntaxTree();

        foreach (SyntaxReceiver.Item item in syntaxReceiver.GatheredInfo)
        {
            if (!ValidateTargetType(context, item.AttributeData, item.ClassDeclaration, item.ClassSymbol, out DiagnosticDescriptor? descriptor))
            {
                context.ReportDiagnostic(descriptor, item.AttributeSyntax, item.ClassSymbol);

                continue;
            }

            try
            {
                OnExecute(context, item.AttributeData, item.ClassDeclaration, item.ClassSymbol, sourceSyntaxTree);
            }
            catch
            {
                context.ReportDiagnostic(TargetTypeErrorDescriptor, item.AttributeSyntax, item.ClassSymbol);
            }
        }
    }

    /// <summary>
    /// Loads the source syntax tree for the current generator.
    /// </summary>
    /// <returns>The syntax tree with the elements to emit in the generated code.</returns>
    private SyntaxTree LoadSourceSyntaxTree()
    {
        string filename = $"CommunityToolkit.Mvvm.SourceGenerators.EmbeddedResources.{this.attributeTypeName.Replace("Attribute", string.Empty)}.cs";

        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
        StreamReader reader = new(stream);

        string observableObjectSource = reader.ReadToEnd();

        return CSharpSyntaxTree.ParseText(observableObjectSource);
    }

    /// <summary>
    /// Processes a given target type.
    /// </summary>
    /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> for the current attribute being processed.</param>
    /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
    /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
    /// <param name="sourceSyntaxTree">The <see cref="Microsoft.CodeAnalysis.SyntaxTree"/> for the target parsed source.</param>
    private void OnExecute(
        GeneratorExecutionContext context,
        AttributeData attributeData,
        ClassDeclarationSyntax classDeclaration,
        INamedTypeSymbol classDeclarationSymbol,
        SyntaxTree sourceSyntaxTree)
    {
        ClassDeclarationSyntax sourceDeclaration = sourceSyntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();

        // Create the class declaration for the user type. This will produce a tree as follows:
        //
        // <MODIFIERS> <CLASS_NAME> : <BASE_TYPES>
        // {
        //     <MEMBERS>
        // }
        ClassDeclarationSyntax? classDeclarationSyntax =
            ClassDeclaration(classDeclaration.Identifier.Text)
            .WithModifiers(classDeclaration.Modifiers)
            .WithBaseList(sourceDeclaration.BaseList)
            .AddMembers(OnLoadDeclaredMembers(context, attributeData, classDeclaration, classDeclarationSymbol, sourceDeclaration).ToArray());

        TypeDeclarationSyntax typeDeclarationSyntax = classDeclarationSyntax;

        // Add all parent types in ascending order, if any
        foreach (TypeDeclarationSyntax? parentType in classDeclaration.Ancestors().OfType<TypeDeclarationSyntax>())
        {
            typeDeclarationSyntax = parentType
                .WithMembers(SingletonList<MemberDeclarationSyntax>(typeDeclarationSyntax))
                .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                .WithBaseList(null)
                .WithAttributeLists(List<AttributeListSyntax>())
                .WithoutTrivia();
        }

        // Create the compilation unit with the namespace and target member.
        // From this, we can finally generate the source code to output.
        string? namespaceName = classDeclarationSymbol.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: NameAndContainingTypesAndNamespaces));

        // Create the final compilation unit to generate (with the full type declaration)
        string? source =
            CompilationUnit()
            .AddMembers(NamespaceDeclaration(IdentifierName(namespaceName))
            .WithLeadingTrivia(TriviaList(
                Comment("// <auto-generated/>"),
                Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true))))
            .AddMembers(typeDeclarationSyntax))
            .NormalizeWhitespace()
            .ToFullString();

        // Add the partial type
        context.AddSource($"{classDeclarationSymbol.GetFullMetadataNameForFileName()}.cs", SourceText.From(source, Encoding.UTF8));
    }

    /// <summary>
    /// Loads the <see cref="MemberDeclarationSyntax"/> nodes to generate from the input parsed tree.
    /// </summary>
    /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> for the current attribute being processed.</param>
    /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
    /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
    /// <param name="sourceDeclaration">The parsed <see cref="ClassDeclarationSyntax"/> instance with the source nodes.</param>
    /// <returns>A sequence of <see cref="MemberDeclarationSyntax"/> nodes to emit in the generated file.</returns>
    private IEnumerable<MemberDeclarationSyntax> OnLoadDeclaredMembers(
        GeneratorExecutionContext context,
        AttributeData attributeData,
        ClassDeclarationSyntax classDeclaration,
        INamedTypeSymbol classDeclarationSymbol,
        ClassDeclarationSyntax sourceDeclaration)
    {
        IEnumerable<MemberDeclarationSyntax> generatedMembers = FilterDeclaredMembers(context, attributeData, classDeclaration, classDeclarationSymbol, sourceDeclaration);

        // Add the attributes on each member
        return generatedMembers.Select(member =>
        {
            // [GeneratedCode] is always present
            member = member
            .WithoutLeadingTrivia()
            .AddAttributeLists(AttributeList(SingletonSeparatedList(
                Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode"))
                .AddArgumentListArguments(
                    AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(GetType().FullName))),
                    AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(GetType().Assembly.GetName().Version.ToString())))))))
            .WithLeadingTrivia(member.GetLeadingTrivia());

            // [DebuggerNonUserCode] is not supported over interfaces, events or fields
            if (member.Kind() is not SyntaxKind.InterfaceDeclaration and not SyntaxKind.EventFieldDeclaration and not SyntaxKind.FieldDeclaration)
            {
                member = member.AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))));
            }

            // [ExcludeFromCodeCoverage] is not supported on interfaces and fields
            if (member.Kind() is not SyntaxKind.InterfaceDeclaration and not SyntaxKind.FieldDeclaration)
            {
                member = member.AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))));
            }

            // If the target class is sealed, make protected members private and remove the virtual modifier
            if (classDeclarationSymbol.IsSealed)
            {
                return member
                    .ReplaceModifier(SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword)
                    .RemoveModifier(SyntaxKind.VirtualKeyword);
            }

            return member;
        });
    }

    /// <summary>
    /// Validates a target type being processed.
    /// </summary>
    /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> for the current attribute being processed.</param>
    /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
    /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
    /// <param name="descriptor">The resulting <see cref="DiagnosticDescriptor"/> to emit in case the target type isn't valid.</param>
    /// <returns>Whether or not the target type is valid and can be processed normally.</returns>
    protected abstract bool ValidateTargetType(
        GeneratorExecutionContext context,
        AttributeData attributeData,
        ClassDeclarationSyntax classDeclaration,
        INamedTypeSymbol classDeclarationSymbol,
        [NotNullWhen(false)] out DiagnosticDescriptor? descriptor);

    /// <summary>
    /// Filters the <see cref="MemberDeclarationSyntax"/> nodes to generate from the input parsed tree.
    /// </summary>
    /// <param name="context">The input <see cref="GeneratorExecutionContext"/> instance to use.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> for the current attribute being processed.</param>
    /// <param name="classDeclaration">The <see cref="ClassDeclarationSyntax"/> node to process.</param>
    /// <param name="classDeclarationSymbol">The <see cref="INamedTypeSymbol"/> for <paramref name="classDeclaration"/>.</param>
    /// <param name="sourceDeclaration">The parsed <see cref="ClassDeclarationSyntax"/> instance with the source nodes.</param>
    /// <returns>A sequence of <see cref="MemberDeclarationSyntax"/> nodes to emit in the generated file.</returns>
    protected virtual IEnumerable<MemberDeclarationSyntax> FilterDeclaredMembers(
        GeneratorExecutionContext context,
        AttributeData attributeData,
        ClassDeclarationSyntax classDeclaration,
        INamedTypeSymbol classDeclarationSymbol,
        ClassDeclarationSyntax sourceDeclaration)
    {
        return sourceDeclaration.Members;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <inheritdoc/>
partial class ObservableValidatorValidateAllPropertiesGenerator
{
    /// <summary>
    /// A container for all the logic for <see cref="ObservableValidatorValidateAllPropertiesGenerator"/>.
    /// </summary>
    private static class Execute
    {
        /// <summary>
        /// Checks whether a given type inherits from <c>ObservableValidator</c>.
        /// </summary>
        /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance to check.</param>
        /// <returns>Whether <paramref name="typeSymbol"/> inherits from <c>ObservableValidator</c>.</returns>
        public static bool IsObservableValidator(INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.InheritsFromFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableValidator");
        }

        /// <summary>
        /// Gets the <see cref="ValidationInfo"/> instance from an input symbol.
        /// </summary>
        /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance to inspect.</param>
        /// <returns>The resulting <see cref="ValidationInfo"/> instance for <paramref name="typeSymbol"/>.</returns>
        public static ValidationInfo GetInfo(INamedTypeSymbol typeSymbol)
        {
            using ImmutableArrayBuilder<string> propertyNames = ImmutableArrayBuilder<string>.Rent();

            foreach (ISymbol memberSymbol in typeSymbol.GetMembers())
            {
                if (memberSymbol is { IsStatic: true } or not (IPropertySymbol { IsIndexer: false } or IFieldSymbol))
                {
                    continue;
                }

                ImmutableArray<AttributeData> attributes = memberSymbol.GetAttributes();

                // Also include fields that are annotated with [ObservableProperty]. This is necessary because
                // all generators run in an undefined order and looking at the same original compilation, so the
                // current one wouldn't be able to see generated properties from other generators directly.
                if (memberSymbol is IFieldSymbol &&
                    !attributes.Any(static a => a.AttributeClass?.HasFullyQualifiedMetadataName(
                        "CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") == true))
                {
                    continue;
                }

                // Skip the current member if there are no validation attributes applied to it
                if (!attributes.Any(a => a.AttributeClass?.InheritsFromFullyQualifiedMetadataName(
                    "System.ComponentModel.DataAnnotations.ValidationAttribute") == true))
                {
                    continue;
                }

                // Get the target property name either directly or matching the generated one
                string propertyName = memberSymbol switch
                {
                    IPropertySymbol propertySymbol => propertySymbol.Name,
                    IFieldSymbol fieldSymbol => ObservablePropertyGenerator.Execute.GetGeneratedPropertyName(fieldSymbol),
                    _ => throw new InvalidOperationException("Invalid symbol type")
                };

                propertyNames.Add(propertyName);
            }

            return new(
                typeSymbol.GetFullyQualifiedMetadataName(),
                typeSymbol.GetFullyQualifiedName(),
                propertyNames.ToImmutable());
        }

        /// <summary>
        /// Gets the head <see cref="CompilationUnitSyntax"/> instance.
        /// </summary>
        /// <param name="isDynamicallyAccessedMembersAttributeAvailable">Indicates whether <c>[DynamicallyAccessedMembers]</c> should be generated.</param>
        /// <returns>The head <see cref="CompilationUnitSyntax"/> instance with the type attributes.</returns>
        public static CompilationUnitSyntax GetSyntax(bool isDynamicallyAccessedMembersAttributeAvailable)
        {
            using ImmutableArrayBuilder<AttributeListSyntax> attributes = ImmutableArrayBuilder<AttributeListSyntax>.Rent();

            // Prepare the base attributes with are always present:
            //
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.DebuggerNonUserCode]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            // [global::System.Obsolete("This type is not intended to be used directly by user code")]
            attributes.Add(
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode")).AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservableValidatorValidateAllPropertiesGenerator).FullName))),
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservableValidatorValidateAllPropertiesGenerator).Assembly.GetName().Version.ToString())))))));
            attributes.Add(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))));
            attributes.Add(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))));
            attributes.Add(
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("global::System.ComponentModel.EditorBrowsable")).AddArgumentListArguments(
                    AttributeArgument(ParseExpression("global::System.ComponentModel.EditorBrowsableState.Never"))))));
            attributes.Add(
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("global::System.Obsolete")).AddArgumentListArguments(
                    AttributeArgument(LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        Literal("This type is not intended to be used directly by user code")))))));

            if (isDynamicallyAccessedMembersAttributeAvailable)
            {
                // Conditionally add the attribute to inform trimming, if the type is available:
                //
                // [global::System.CodeDom.Compiler.DynamicallyAccessedMembersAttribute(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods)]
                attributes.Add(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute")).AddArgumentListArguments(
                        AttributeArgument(ParseExpression("global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods"))))));
            }

            // This code produces a compilation unit as follows:
            //
            // // <auto-generated/>
            // #pragma warning disable
            // namespace CommunityToolkit.Mvvm.ComponentModel.__Internals
            // {
            //     <ATTRIBUTES>
            //     internal static partial class __ObservableValidatorExtensions
            //     {
            //     }
            // }
            return
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("CommunityToolkit.Mvvm.ComponentModel.__Internals")).WithLeadingTrivia(TriviaList(
                    Comment("// <auto-generated/>"),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)))).AddMembers(
                ClassDeclaration("__ObservableValidatorExtensions").AddModifiers(
                    Token(SyntaxKind.InternalKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.PartialKeyword))
                .AddAttributeLists(attributes.ToArray())))
                .NormalizeWhitespace();
        }

        /// <summary>
        /// Gets the <see cref="CompilationUnitSyntax"/> instance for the input recipient.
        /// </summary>
        /// <param name="validationInfo">The input <see cref="ValidationInfo"/> instance to process.</param>
        /// <returns>The generated <see cref="CompilationUnitSyntax"/> instance for <paramref name="validationInfo"/>.</returns>
        public static CompilationUnitSyntax GetSyntax(ValidationInfo validationInfo)
        {
            // Create a static factory method creating a delegate that can be used to validate all properties in a given class.
            // This pattern is used so that the library doesn't have to use MakeGenericType(...) at runtime, nor use unsafe casts
            // over the created delegate to be able to cache it as an Action<object> instance. This pattern enables the same
            // functionality and with almost identical performance (not noticeable in this context anyway), but while preserving
            // full runtime type safety (as a safe cast is used to validate the input argument), and with less reflection needed.
            // Note that we're deliberately creating a new delegate instance here and not using code that could see the C# compiler
            // create a static class to cache a reusable delegate, because each generated method will only be called at most once,
            // as the returned delegate will be cached by the MVVM Toolkit itself. So this ensures the the produced code is minimal,
            // and that there will be no unnecessary static fields and objects being created and possibly never collected.
            // This code will produce a syntax tree as follows:
            //
            // // <auto-generated/>
            // #pragma warning disable
            // namespace CommunityToolkit.Mvvm.ComponentModel.__Internals
            // {
            //     partial class __ObservableValidatorExtensions
            //     {
            //         [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            //         [global::System.Obsolete("This method is not intended to be called directly by user code")]
            //         public static global::System.Action<object> CreateAllPropertiesValidator(<INSTANCE_TYPE> _)
            //         {
            //             static void ValidateAllProperties(object obj)
            //             {
            //                 var instance = (<INSTANCE_TYPE>)obj;
            //                 <BODY>
            //             }
            //
            //             return ValidateAllProperties;
            //         }
            //     }
            // }
            return
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("CommunityToolkit.Mvvm.ComponentModel.__Internals")).WithLeadingTrivia(TriviaList(
                    Comment("// <auto-generated/>"),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)))).AddMembers(
                ClassDeclaration("__ObservableValidatorExtensions").AddModifiers(Token(SyntaxKind.PartialKeyword)).AddMembers(
                MethodDeclaration(
                    GenericName("global::System.Action").AddTypeArgumentListArguments(PredefinedType(Token(SyntaxKind.ObjectKeyword))),
                    Identifier("CreateAllPropertiesValidator")).AddAttributeLists(
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.ComponentModel.EditorBrowsable")).AddArgumentListArguments(
                            AttributeArgument(ParseExpression("global::System.ComponentModel.EditorBrowsableState.Never"))))),
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.Obsolete")).AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal("This method is not intended to be called directly by user code"))))))).AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword)).AddParameterListParameters(
                        Parameter(Identifier("_")).WithType(IdentifierName(validationInfo.TypeName)))
                    .WithBody(Block(
                        LocalFunctionStatement(
                            PredefinedType(Token(SyntaxKind.VoidKeyword)),
                            Identifier("ValidateAllProperties"))
                        .AddModifiers(Token(SyntaxKind.StaticKeyword))
                        .AddParameterListParameters(
                            Parameter(Identifier("obj")).WithType(PredefinedType(Token(SyntaxKind.ObjectKeyword))))
                        .WithBody(Block(
                            LocalDeclarationStatement(
                                VariableDeclaration(IdentifierName("var")) // Cannot use Token(SyntaxKind.VarKeyword) here (throws an ArgumentException)
                                .AddVariables(
                                    VariableDeclarator(Identifier("instance"))
                                    .WithInitializer(EqualsValueClause(
                                        CastExpression(
                                            IdentifierName(validationInfo.TypeName),
                                            IdentifierName("obj")))))))
                            .AddStatements(EnumerateValidationStatements(validationInfo).ToArray())),
                        ReturnStatement(IdentifierName("ValidateAllProperties")))))))
                .NormalizeWhitespace();
        }

        /// <summary>
        /// Gets a sequence of statements to validate declared properties.
        /// </summary>
        /// <param name="validationInfo">The input <see cref="ValidationInfo"/> instance to process.</param>
        /// <returns>The sequence of <see cref="StatementSyntax"/> instances to validate declared properties.</returns>
        private static ImmutableArray<StatementSyntax> EnumerateValidationStatements(ValidationInfo validationInfo)
        {
            using ImmutableArrayBuilder<StatementSyntax> statements = ImmutableArrayBuilder<StatementSyntax>.Rent();

            // This loop produces a sequence of statements as follows:
            //
            // __ObservableValidatorHelper.ValidateProperty(instance, instance.<PROPERTY_0>, nameof(instance.<PROPERTY_0>));
            // __ObservableValidatorHelper.ValidateProperty(instance, instance.<PROPERTY_0>, nameof(instance.<PROPERTY_0>));
            // ...
            // __ObservableValidatorHelper.ValidateProperty(instance, instance.<PROPERTY_1>, nameof(instance.<PROPERTY_1>));
            foreach (string propertyName in validationInfo.PropertyNames)
            {
                statements.Add(
                    ExpressionStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("__ObservableValidatorHelper"),
                            IdentifierName("ValidateProperty")))
                    .AddArgumentListArguments(
                        Argument(IdentifierName("instance")),
                        Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("instance"),
                                IdentifierName(propertyName))),
                        Argument(
                            InvocationExpression(IdentifierName("nameof"))
                            .AddArgumentListArguments(Argument(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("instance"),
                                    IdentifierName(propertyName))))))));
            }

            return statements.ToImmutable();
        }
    }
}

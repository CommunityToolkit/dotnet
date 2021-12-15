// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
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
using System.Collections.Immutable;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Models;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <inheritdoc/>
partial class ObservablePropertyGenerator2
{
    /// <summary>
    /// A container for all the logic for <see cref="ObservablePropertyGenerator2"/>.
    /// </summary>
    private static class Execute
    {
        /// <summary>
        /// Processes a given field.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <returns>The resulting <see cref="PropertyInfo"/> instance for <paramref name="fieldSymbol"/>.</returns>
        public static PropertyInfo GetInfo(IFieldSymbol fieldSymbol)
        {
            // Check whether the containing type implements INotifyPropertyChanging and whether it inherits from ObservableValidator
            bool isObservableObject = fieldSymbol.ContainingType.InheritsFrom("global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject");
            bool isObservableValidator = fieldSymbol.ContainingType.InheritsFrom("global::CommunityToolkit.Mvvm.ComponentModel.ObservableValidator");
            bool isNotifyPropertyChanging = fieldSymbol.ContainingType.AllInterfaces.Any(static i => i.HasFullyQualifiedName("global::System.ComponentModel.INotifyPropertyChanging"));
            bool hasObservableObjectAttribute = fieldSymbol.ContainingType.GetAttributes().Any(static a => a.AttributeClass?.HasFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute") == true);
            bool shouldNotifyPropertyChanging = isObservableObject || isNotifyPropertyChanging || hasObservableObjectAttribute;

            // Get the property type and name
            string typeName = fieldSymbol.Type.GetFullyQualifiedName();
            bool isNullableReferenceType = fieldSymbol.Type is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated };
            string fieldName = fieldSymbol.Name;
            string propertyName = GetGeneratedPropertyName(fieldSymbol);

            ImmutableArray<string>.Builder propertyChangedNames = ImmutableArray.CreateBuilder<string>();
            ImmutableArray<string>.Builder propertyChangingNames = ImmutableArray.CreateBuilder<string>();
            ImmutableArray<string>.Builder notifiedCommandNames = ImmutableArray.CreateBuilder<string>();
            ImmutableArray<AttributeInfo>.Builder validationAttributes = ImmutableArray.CreateBuilder<AttributeInfo>();

            // Gather attributes info
            foreach (AttributeData attributeData in fieldSymbol.GetAttributes())
            {
                // Add dependent property notifications, if needed
                if (attributeData.AttributeClass?.HasFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.AlsoNotifyChangeForAttribute") == true)
                {
                    foreach (string dependentPropertyName in attributeData.GetConstructorArguments<string>())
                    {
                        propertyChangedNames.Add(dependentPropertyName);
                    }
                }
                else if (attributeData.AttributeClass?.HasFullyQualifiedName("global::CommunityToolkit.Mvvm.ComponentModel.AlsoNotifyCanExecuteForAttribute") == true)
                {
                    // Add dependent relay command notifications, if needed
                    foreach (string commandName in attributeData.GetConstructorArguments<string>())
                    {
                        notifiedCommandNames.Add(commandName);
                    }
                }
                else if (attributeData.AttributeClass?.InheritsFrom("global::System.ComponentModel.DataAnnotations.ValidationAttribute") == true)
                {
                    // Track the current validation attribute
                    validationAttributes.Add(AttributeInfo.From(attributeData));
                }
            }

            return new(
                typeName,
                isNullableReferenceType,
                fieldName,
                propertyName,
                propertyChangingNames.ToImmutable(),
                propertyChangedNames.ToImmutable(),
                notifiedCommandNames.ToImmutable(),
                validationAttributes.ToImmutable());
        }

        /// <summary>
        /// Gets a <see cref="CompilationUnitSyntax"/> instance with the cached args for property changing notifications.
        /// </summary>
        /// <param name="names">The sequence of property names to cache args for.</param>
        /// <returns>A <see cref="CompilationUnitSyntax"/> instance with the sequence of cached args, if any.</returns>
        public static CompilationUnitSyntax? GetKnownPropertyChangingArgsSyntax(ImmutableArray<string> names)
        {
            return GetKnownPropertyChangingOrChangedArgsSyntax(
                "__KnownINotifyPropertyChangingArgs",
                "global::System.ComponentModel.PropertyChangingEventArgs",
                names);
        }

        /// <summary>
        /// Gets a <see cref="CompilationUnitSyntax"/> instance with the cached args for property changed notifications.
        /// </summary>
        /// <param name="names">The sequence of property names to cache args for.</param>
        /// <returns>A <see cref="CompilationUnitSyntax"/> instance with the sequence of cached args, if any.</returns>
        public static CompilationUnitSyntax? GetKnownPropertyChangedArgsSyntax(ImmutableArray<string> names)
        {
            return GetKnownPropertyChangingOrChangedArgsSyntax(
                "__KnownINotifyPropertyChangedArgs",
                "global::System.ComponentModel.PropertyChangedEventArgs",
                names);
        }

        /// <summary>
        /// Gets a <see cref="CompilationUnitSyntax"/> instance with the cached args of a specified type.
        /// </summary>
        /// <param name="ContainingTypeName">The name of the generated type.</param>
        /// <param name="ArgsTypeName">The argument type name.</param>
        /// <param name="names">The sequence of property names to cache args for.</param>
        /// <returns>A <see cref="CompilationUnitSyntax"/> instance with the sequence of cached args, if any.</returns>
        private static CompilationUnitSyntax? GetKnownPropertyChangingOrChangedArgsSyntax(
            string ContainingTypeName,
            string ArgsTypeName,
            ImmutableArray<string> names)
        {
            if (names.IsEmpty)
            {
                return null;
            }

            // This code takes a class symbol and produces a compilation unit as follows:
            //
            // // <auto-generated/>
            // #pragma warning disable
            // namespace CommunityToolkit.Mvvm.ComponentModel.__Internals
            // {
            //     [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            //     [global::System.Diagnostics.DebuggerNonUserCode]
            //     [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            //     [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            //     [global::System.Obsolete("This type is not intended to be used directly by user code")]
            //     internal static class <CONTAINING_TYPE_NAME>
            //     {
            //         <FIELDS>
            //     }
            // }
            return
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("CommunityToolkit.Mvvm.ComponentModel.__Internals")).WithLeadingTrivia(TriviaList(
                    Comment("// <auto-generated/>"),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)))).AddMembers(
                ClassDeclaration(ContainingTypeName).AddModifiers(
                    Token(SyntaxKind.InternalKeyword),
                    Token(SyntaxKind.StaticKeyword)).AddAttributeLists(
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode"))
                            .AddArgumentListArguments(
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator2).FullName))),
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservablePropertyGenerator2).Assembly.GetName().Version.ToString())))))),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))),
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.ComponentModel.EditorBrowsable")).AddArgumentListArguments(
                            AttributeArgument(ParseExpression("global::System.ComponentModel.EditorBrowsableState.Never"))))),
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.Obsolete")).AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal("This type is not intended to be used directly by user code")))))))
                    .AddMembers(names.Select(name => CreateFieldDeclaration(ArgsTypeName, name)).ToArray())))
                .NormalizeWhitespace(eol: "\n");
        }

        /// <summary>
        /// Creates a field declaration for a cached property changing/changed name.
        /// </summary>
        /// <param name="typeName">The field type name (either <see cref="PropertyChangedEventArgs"/> or <see cref="PropertyChangingEventArgs"/>).</param>
        /// <param name="propertyName">The name of the cached property name.</param>
        /// <returns>A <see cref="FieldDeclarationSyntax"/> instance for the input cached property name.</returns>
        private static FieldDeclarationSyntax CreateFieldDeclaration(string typeName, string propertyName)
        {
            // Create a static field with a cached property changed/changing argument for a specified property.
            // This code produces a field declaration as follows:
            //
            // [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            // [global::System.Obsolete("This field is not intended to be referenced directly by user code")]
            // public static readonly <ARG_TYPE> <PROPERTY_NAME> = new("<PROPERTY_NAME>");
            return
                FieldDeclaration(
                VariableDeclaration(IdentifierName(typeName))
                .AddVariables(
                    VariableDeclarator(Identifier(propertyName))
                    .WithInitializer(EqualsValueClause(
                        ImplicitObjectCreationExpression()
                        .AddArgumentListArguments(Argument(
                            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(propertyName))))))))
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.ComponentModel.EditorBrowsable")).AddArgumentListArguments(
                        AttributeArgument(ParseExpression("global::System.ComponentModel.EditorBrowsableState.Never"))))),
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.Obsolete")).AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal("This field is not intended to be referenced directly by user code")))))));
        }

        /// <summary>
        /// Get the generated property name for an input field.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <returns>The generated property name for <paramref name="fieldSymbol"/>.</returns>
        private static string GetGeneratedPropertyName(IFieldSymbol fieldSymbol)
        {
            string propertyName = fieldSymbol.Name;

            if (propertyName.StartsWith("m_"))
            {
                propertyName = propertyName.Substring(2);
            }
            else if (propertyName.StartsWith("_"))
            {
                propertyName = propertyName.TrimStart('_');
            }

            return $"{char.ToUpper(propertyName[0])}{propertyName.Substring(1)}";
        }
    }
}

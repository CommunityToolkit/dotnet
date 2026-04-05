// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using CommunityToolkit.Mvvm.SourceGenerators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <inheritdoc/>
partial class ObservableValidatorValidationGenerator
{
    /// <summary>
    /// A container for all the logic for <see cref="ObservableValidatorValidationGenerator"/>.
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
        /// <param name="token">The cancellation token for the current operation.</param>
        /// <returns>The resulting <see cref="ValidationInfo"/> instance for <paramref name="typeSymbol"/>, if available.</returns>
        public static ValidationInfo? GetInfo(INamedTypeSymbol typeSymbol, CancellationToken token)
        {
            if (!typeSymbol.IsTypeHierarchyPartial())
            {
                return default;
            }

            using ImmutableArrayBuilder<PropertyValidationInfo> properties = ImmutableArrayBuilder<PropertyValidationInfo>.Rent();

            foreach (ISymbol memberSymbol in typeSymbol.GetMembers())
            {
                if (memberSymbol is not IPropertySymbol propertySymbol ||
                    propertySymbol.IsStatic ||
                    propertySymbol.IsIndexer ||
                    !propertySymbol.CanBeReferencedByName)
                {
                    continue;
                }

                token.ThrowIfCancellationRequested();

                if (!propertySymbol.GetAttributes().Any(static a => a.AttributeClass?.InheritsFromFullyQualifiedMetadataName(
                    "System.ComponentModel.DataAnnotations.ValidationAttribute") == true))
                {
                    continue;
                }

                properties.Add(new(propertySymbol.Name, HasDisplayAttribute(propertySymbol.GetAttributes())));
            }

            token.ThrowIfCancellationRequested();

            if (properties.WrittenSpan.IsEmpty)
            {
                return default;
            }

            return new(
                HierarchyInfo.From(typeSymbol),
                typeSymbol.GetFullyQualifiedName(),
                properties.ToImmutable());
        }

        /// <summary>
        /// Gets the validation info for a field-backed observable property, if applicable.
        /// </summary>
        /// <param name="memberSyntax">The source member declaration for the annotated field.</param>
        /// <param name="fieldSymbol">The field symbol annotated with <c>[ObservableProperty]</c>.</param>
        /// <param name="semanticModel">The semantic model for the current run.</param>
        /// <param name="options">The analyzer config options in use.</param>
        /// <param name="token">The cancellation token for the current operation.</param>
        /// <returns>The validation info for <paramref name="fieldSymbol"/>, if available.</returns>
        public static (ValidationTypeInfo Left, PropertyValidationInfo Right) GetGeneratedObservablePropertyValidationInfo(
            MemberDeclarationSyntax memberSyntax,
            IFieldSymbol fieldSymbol,
            SemanticModel semanticModel,
            AnalyzerConfigOptions options,
            CancellationToken token)
        {
            if (!fieldSymbol.ContainingType.IsTypeHierarchyPartial() ||
                !IsObservableValidator(fieldSymbol.ContainingType) ||
                !ObservablePropertyGenerator.Execute.TryGetInfo(memberSyntax, fieldSymbol, semanticModel, options, token, out PropertyInfo? propertyInfo, out _ ) ||
                propertyInfo is not { HasValidationAttributes: true })
            {
                return default;
            }

            return (
                new ValidationTypeInfo(HierarchyInfo.From(fieldSymbol.ContainingType), fieldSymbol.ContainingType.GetFullyQualifiedName()),
                new PropertyValidationInfo(propertyInfo.PropertyName, HasDisplayAttribute(propertyInfo.ForwardedAttributes)));
        }

        /// <summary>
        /// Gets the <see cref="CompilationUnitSyntax"/> instance for the input recipient.
        /// </summary>
        /// <param name="validationInfo">The input <see cref="ValidationInfo"/> instance to process.</param>
        /// <returns>The generated <see cref="CompilationUnitSyntax"/> instance for <paramref name="validationInfo"/>.</returns>
        public static CompilationUnitSyntax GetSyntax(ValidationInfo validationInfo)
        {
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations = GetMemberDeclarations(validationInfo);

            return validationInfo.Hierarchy.GetCompilationUnit(memberDeclarations)
                .AddUsings(
                    UsingDirective(ParseName("System.Collections.Generic")),
                    UsingDirective(ParseName("System.ComponentModel.DataAnnotations")),
                    UsingDirective(ParseName("System.Reflection")))
                .NormalizeWhitespace();
        }

        /// <summary>
        /// Gets the compatibility header <see cref="CompilationUnitSyntax"/> instance.
        /// </summary>
        /// <param name="isDynamicallyAccessedMembersAttributeAvailable">Indicates whether <c>[DynamicallyAccessedMembers]</c> should be generated.</param>
        /// <returns>The header <see cref="CompilationUnitSyntax"/> instance with the compatibility type.</returns>
        public static CompilationUnitSyntax GetHeaderSyntax(bool isDynamicallyAccessedMembersAttributeAvailable)
        {
            using ImmutableArrayBuilder<AttributeListSyntax> attributes = ImmutableArrayBuilder<AttributeListSyntax>.Rent();

            attributes.Add(
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode")).AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservableValidatorValidationGenerator).FullName))),
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservableValidatorValidationGenerator).Assembly.GetName().Version!.ToString()))))))
                .WithOpenBracketToken(Token(TriviaList(
                    Comment("/// <summary>"),
                    Comment("/// A compatibility helper type for validation-related generated code."),
                    Comment("/// </summary>")), SyntaxKind.OpenBracketToken, TriviaList())));
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
                attributes.Add(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute")).AddArgumentListArguments(
                            AttributeArgument(ParseExpression("global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods"))))));
            }

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
        /// Creates the generated members for a target type.
        /// </summary>
        /// <param name="validationInfo">The validation info for the target type.</param>
        /// <returns>The generated members for <paramref name="validationInfo"/>.</returns>
        private static ImmutableArray<MemberDeclarationSyntax> GetMemberDeclarations(ValidationInfo validationInfo)
        {
            using ImmutableArrayBuilder<MemberDeclarationSyntax> members = ImmutableArrayBuilder<MemberDeclarationSyntax>.Rent();

            foreach (PropertyValidationInfo propertyInfo in validationInfo.Properties)
            {
                string validationHelperName = GetValidationAttributesHelperName(propertyInfo.PropertyName);

                members.Add(ParseMemberDeclaration($$"""
                    private static readonly IEnumerable<ValidationAttribute> {{validationHelperName}} =
                        typeof({{validationInfo.TypeName}}).GetProperty(nameof({{propertyInfo.PropertyName}}))!.GetCustomAttributes<ValidationAttribute>();
                    """)!);

                if (propertyInfo.HasDisplayAttribute)
                {
                    string displayHelperName = GetDisplayAttributeHelperName(propertyInfo.PropertyName);

                    members.Add(ParseMemberDeclaration($$"""
                        private static readonly DisplayAttribute {{displayHelperName}} =
                            typeof({{validationInfo.TypeName}}).GetProperty(nameof({{propertyInfo.PropertyName}}))!.GetCustomAttribute<DisplayAttribute>()!;
                        """)!);
                }
            }

            members.Add(ParseMemberDeclaration(GetTryValidatePropertyCoreSource(validationInfo))!);
            members.Add(ParseMemberDeclaration(GetValidateAllPropertiesCoreSource(validationInfo))!);

            return members.ToImmutable();
        }

        /// <summary>
        /// Creates the source for the generated <c>TryValidatePropertyCore</c> override.
        /// </summary>
        /// <param name="validationInfo">The validation info for the current type.</param>
        /// <returns>The generated member source.</returns>
        private static string GetTryValidatePropertyCoreSource(ValidationInfo validationInfo)
        {
            StringBuilder builder = new();

            builder.AppendLine("/// <inheritdoc/>");
            builder.AppendLine("protected override ValidationStatus TryValidatePropertyCore(object? value, string propertyName, ICollection<ValidationResult> errors)");
            builder.AppendLine("{");
            builder.AppendLine("    return (propertyName) switch");
            builder.AppendLine("    {");

            foreach (PropertyValidationInfo propertyInfo in validationInfo.Properties)
            {
                string validationHelperName = GetValidationAttributesHelperName(propertyInfo.PropertyName);
                string displayNameExpression = propertyInfo.HasDisplayAttribute
                    ? $"{GetDisplayAttributeHelperName(propertyInfo.PropertyName)}.GetName() ?? {SymbolDisplay.FormatLiteral(propertyInfo.PropertyName, true)}"
                    : SymbolDisplay.FormatLiteral(propertyInfo.PropertyName, true);

                builder.AppendLine($"        nameof({propertyInfo.PropertyName}) => TryValidateValue(value, nameof({propertyInfo.PropertyName}), {displayNameExpression}, {validationHelperName}, errors),");
            }

            builder.AppendLine("        _ => base.TryValidatePropertyCore(value, propertyName, errors)");
            builder.AppendLine("    };");
            builder.AppendLine("}");

            return builder.ToString();
        }

        /// <summary>
        /// Creates the source for the generated <c>ValidateAllPropertiesCore</c> override.
        /// </summary>
        /// <param name="validationInfo">The validation info for the current type.</param>
        /// <returns>The generated member source.</returns>
        private static string GetValidateAllPropertiesCoreSource(ValidationInfo validationInfo)
        {
            StringBuilder builder = new();

            builder.AppendLine("/// <inheritdoc/>");
            builder.AppendLine("protected override void ValidateAllPropertiesCore()");
            builder.AppendLine("{");

            foreach (PropertyValidationInfo propertyInfo in validationInfo.Properties)
            {
                builder.AppendLine($"    ValidateProperty({propertyInfo.PropertyName}, nameof({propertyInfo.PropertyName}));");
            }

            builder.AppendLine("    base.ValidateAllPropertiesCore();");
            builder.AppendLine("}");

            return builder.ToString();
        }

        /// <summary>
        /// Creates a stable helper name for a given property.
        /// </summary>
        /// <param name="propertyName">The property name to process.</param>
        /// <returns>A stable helper name for <paramref name="propertyName"/>.</returns>
        private static string GetValidationAttributesHelperName(string propertyName)
        {
            return GetHelperName(propertyName, "ValidationAttributes");
        }

        /// <summary>
        /// Creates a stable display helper name for a given property.
        /// </summary>
        /// <param name="propertyName">The property name to process.</param>
        /// <returns>A stable display helper name for <paramref name="propertyName"/>.</returns>
        private static string GetDisplayAttributeHelperName(string propertyName)
        {
            return GetHelperName(propertyName, "DisplayAttribute");
        }

        /// <summary>
        /// Creates a stable helper name for a given property and suffix.
        /// </summary>
        /// <param name="propertyName">The property name to process.</param>
        /// <param name="suffix">The suffix to append.</param>
        /// <returns>A stable helper name for <paramref name="propertyName"/>.</returns>
        private static string GetHelperName(string propertyName, string suffix)
        {
            StringBuilder builder = new("__");

            foreach (char c in propertyName)
            {
                builder.Append(char.IsLetterOrDigit(c) ? c : '_');
            }

            if (builder.Length == 2 || !char.IsLetter(builder[2]) && builder[2] != '_')
            {
                builder.Insert(2, '_');
            }

            builder.Append(suffix);

            return builder.ToString();
        }

        /// <summary>
        /// Checks whether a property has a <c>DisplayAttribute</c> declared.
        /// </summary>
        /// <param name="attributes">The attributes declared on the property.</param>
        /// <returns>Whether the property has a display attribute.</returns>
        private static bool HasDisplayAttribute(ImmutableArray<AttributeData> attributes)
        {
            foreach (AttributeData attributeData in attributes)
            {
                if (attributeData.AttributeClass?.HasFullyQualifiedMetadataName("System.ComponentModel.DataAnnotations.DisplayAttribute") == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether a generated property has a forwarded <c>DisplayAttribute</c>.
        /// </summary>
        /// <param name="attributes">The forwarded property attributes.</param>
        /// <returns>Whether the property has a display attribute.</returns>
        private static bool HasDisplayAttribute(EquatableArray<AttributeInfo> attributes)
        {
            foreach (AttributeInfo attributeInfo in attributes)
            {
                if (attributeInfo.TypeName == "global::System.ComponentModel.DataAnnotations.DisplayAttribute")
                {
                    return true;
                }
            }

            return false;
        }
    }
}

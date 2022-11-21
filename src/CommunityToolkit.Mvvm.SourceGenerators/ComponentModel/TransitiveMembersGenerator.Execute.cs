// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <inheritdoc/>
partial class TransitiveMembersGenerator<TInfo>
{
    /// <summary>
    /// A container for all the logic for <see cref="TransitiveMembersGenerator{TInfo}"/>.
    /// </summary>
    internal static class Execute
    {
        /// <summary>
        /// Loads the source <see cref="ClassDeclarationSyntax"/> instance to get member declarations from.
        /// </summary>
        /// <param name="attributeType">The fully qualified name of the attribute type to look for.</param>
        /// <returns>The source <see cref="ClassDeclarationSyntax"/> instance to get member declarations from.</returns>
        public static ClassDeclarationSyntax LoadClassDeclaration(string attributeType)
        {
            string attributeTypeName = attributeType.Split('.').Last();
            string filename = $"{attributeTypeName.Replace("Attribute", string.Empty)}.cs";

            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);
            using StreamReader reader = new(stream);

            string observableObjectSource = reader.ReadToEnd();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(observableObjectSource);

            return syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        }

        /// <summary>
        /// Processes the sequence of member declarations to generate.
        /// </summary>
        /// <param name="generatorType">The type of generator being used.</param>
        /// <param name="memberDeclarations">The input sequence of member declarations to generate.</param>
        /// <param name="sealedMemberDeclarations">The resulting sequence of member declarations for sealed types.</param>
        /// <param name="nonSealedMemberDeclarations">The resulting sequence of member declarations for non sealed types.</param>
        public static void ProcessMemberDeclarations(
            Type generatorType,
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations,
            out ImmutableArray<MemberDeclarationSyntax> sealedMemberDeclarations,
            out ImmutableArray<MemberDeclarationSyntax> nonSealedMemberDeclarations)
        {
            ImmutableArray<MemberDeclarationSyntax> annotatedMemberDeclarations = memberDeclarations.Select(member =>
            {
                // [GeneratedCode] is always present
                member =
                    member
                    .WithoutLeadingTrivia()
                    .AddAttributeLists(AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(generatorType.FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(generatorType.Assembly.GetName().Version.ToString())))))))
                    .WithLeadingTrivia(member.GetLeadingTrivia());

                // [ExcludeFromCodeCoverage] is not supported on interfaces and fields
                if (member.Kind() is not SyntaxKind.InterfaceDeclaration and not SyntaxKind.FieldDeclaration)
                {
                    member = member.AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))));
                }

                return member;
            }).ToImmutableArray();

            // If the target class is sealed, make protected members private and remove the virtual modifier
            sealedMemberDeclarations = annotatedMemberDeclarations.Select(static member =>
            {
                // Constructors become public for sealed types
                if (member is ConstructorDeclarationSyntax)
                {
                    return member.ReplaceModifier(SyntaxKind.ProtectedKeyword, SyntaxKind.PublicKeyword);
                }

                // Other members become private
                return
                    member
                    .ReplaceModifier(SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword)
                    .RemoveModifier(SyntaxKind.VirtualKeyword);
            }).ToImmutableArray();

            nonSealedMemberDeclarations = annotatedMemberDeclarations;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
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
        /// Checks whether or not nullability attributes are currently available.
        /// </summary>
        /// <param name="compilation">The input <see cref="Compilation"/> instance.</param>
        /// <returns>Whether or not nullability attributes are currently available.</returns>
        public static bool IsNullabilitySupported(Compilation compilation)
        {
            return
                compilation.HasAccessibleTypeWithMetadataName("System.Diagnostics.CodeAnalysis.NotNullAttribute") &&
                compilation.HasAccessibleTypeWithMetadataName("System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute");
        }

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

                // [DebuggerNonUserCode] is not supported on interfaces, fields and event
                if (member.Kind() is not (SyntaxKind.InterfaceDeclaration or SyntaxKind.FieldDeclaration or SyntaxKind.EventFieldDeclaration))
                {
                    member = member.AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))));
                }

                // [ExcludeFromCodeCoverage] is not supported on interfaces and fields
                if (member.Kind() is not (SyntaxKind.InterfaceDeclaration or SyntaxKind.FieldDeclaration))
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

        /// <summary>
        /// Adjusts the nullability annotations for generated members, dropping attributes if needed.
        /// </summary>
        /// <param name="memberDeclarations">The input sequence of member declarations to generate.</param>
        /// <param name="isNullabilitySupported">Whether nullability attributes are supported.</param>
        /// <returns>The updated collection of member declarations to generate.</returns>
        public static ImmutableArray<MemberDeclarationSyntax> AdjustMemberDeclarationNullabilityAnnotations(
            ImmutableArray<MemberDeclarationSyntax> memberDeclarations,
            bool isNullabilitySupported)
        {
            // If nullability attributes are supported, there is nothing else to do
            if (isNullabilitySupported)
            {
                return memberDeclarations;
            }

            using ImmutableArrayBuilder<MemberDeclarationSyntax> builder = ImmutableArrayBuilder<MemberDeclarationSyntax>.Rent();

            NullabilityAdjustmentSyntaxRewriter syntaxRewriter = new();

            // Iterate over all members and adjust the method declarations, if needed
            foreach (MemberDeclarationSyntax memberDeclaration in memberDeclarations)
            {
                if (memberDeclaration is MethodDeclarationSyntax methodDeclaration)
                {
                    builder.Add((MethodDeclarationSyntax)syntaxRewriter.Visit(methodDeclaration));
                }
                else
                {
                    builder.Add(memberDeclaration);
                }
            }

            return builder.ToImmutable();
        }

        /// <summary>
        /// A custom syntax rewriter that removes nullability attributes from method parameters.
        /// </summary>
        private sealed class NullabilityAdjustmentSyntaxRewriter : CSharpSyntaxRewriter
        {
            /// <inheritdoc/>
            public override SyntaxNode? VisitParameter(ParameterSyntax node)
            {
                SyntaxNode? updatedNode = base.VisitParameter(node);

                // If the node is a parameter node with a single attribute being either [NotNull] or [NotNullIfNotNull], drop it.
                // This expression will match all parameters with the following format:
                //
                // ([global::<NAMESPACE>.<ATTRIBUTE_NAME>] <TYPE> <PARAMETER_NAME>)
                //
                // Where <ATTRIBUTE_NAME> is either "NotNull" or "NotNullIfNotNull". This relies on parameters following this structure
                // for nullability annotations, but that is fine in this context given the only source files are the embedded ones.
                if (updatedNode is ParameterSyntax { AttributeLists: [{ Attributes: [{ Name: QualifiedNameSyntax { Right.Identifier.Text: "NotNull" or "NotNullIfNotNull"} }] }] } parameterNode)
                {
                    return parameterNode.WithAttributeLists(default);
                }

                return updatedNode;
            }
        }
    }
}

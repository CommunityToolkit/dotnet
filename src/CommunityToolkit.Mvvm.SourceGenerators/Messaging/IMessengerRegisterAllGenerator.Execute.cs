// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using CommunityToolkit.Mvvm.SourceGenerators.Messaging.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <inheritdoc/>
partial class IMessengerRegisterAllGenerator
{
    /// <summary>
    /// A container for all the logic for <see cref="IMessengerRegisterAllGenerator"/>.
    /// </summary>
    private static class Execute
    {
        /// <summary>
        /// Gets the <c>IRecipient&lt;TMessage&gt;</c> interfaces from <paramref name="typeSymbol"/>, if any.
        /// </summary>
        /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance to inspect.</param>
        /// <returns>An array of interface type symbols.</returns>
        public static ImmutableArray<INamedTypeSymbol> GetInterfaces(INamedTypeSymbol typeSymbol)
        {
            using ImmutableArrayBuilder<INamedTypeSymbol> iRecipientInterfaces = ImmutableArrayBuilder<INamedTypeSymbol>.Rent();

            foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.AllInterfaces)
            {
                if (interfaceSymbol.HasFullyQualifiedMetadataName("CommunityToolkit.Mvvm.Messaging.IRecipient`1"))
                {
                    iRecipientInterfaces.Add(interfaceSymbol);
                }
            }

            return iRecipientInterfaces.ToImmutable();
        }

        /// <summary>
        /// Gets the <see cref="RecipientInfo"/> instance from the given info.
        /// </summary>
        /// <param name="typeSymbol">The type symbol for the target type being inspected.</param>
        /// <param name="interfaceSymbols">The input array of interface type symbols being handled.</param>
        /// <returns>A <see cref="RecipientInfo"/> instance for the current type being inspected.</returns>
        public static RecipientInfo GetInfo(INamedTypeSymbol typeSymbol, ImmutableArray<INamedTypeSymbol> interfaceSymbols)
        {
            using ImmutableArrayBuilder<string> names = ImmutableArrayBuilder<string>.Rent();

            foreach (INamedTypeSymbol interfaceSymbol in interfaceSymbols)
            {
                names.Add(interfaceSymbol.TypeArguments[0].GetFullyQualifiedName());
            }

            return new(
                typeSymbol.GetFullyQualifiedMetadataName(),
                typeSymbol.GetFullyQualifiedName(),
                names.ToImmutable());
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
            // /// <summary>
            // /// A helper type with generated registration stubs for types implementing <see cref="global::CommunityToolkit.Mvvm.Messaging.IRecipient{T}"/>.
            // /// </summary>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.DebuggerNonUserCode]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            // [global::System.Obsolete("This type is not intended to be used directly by user code")]
            attributes.Add(
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode")).AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(IMessengerRegisterAllGenerator).FullName))),
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(IMessengerRegisterAllGenerator).Assembly.GetName().Version.ToString()))))))
                .WithOpenBracketToken(Token(TriviaList(
                    Comment("/// <summary>"),
                    Comment("/// A helper type with generated registration stubs for types implementing <see cref=\"global::CommunityToolkit.Mvvm.Messaging.IRecipient{T}\"/>."),
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
                // Conditionally add the attribute to inform trimming, if the type is available:
                //
                // [global::System.CodeDom.Compiler.DynamicallyAccessedMembersAttribute(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods)]
                attributes.Add(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute")).AddArgumentListArguments(
                        AttributeArgument(ParseExpression("global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods"))))));
            }

            // Create it in the standard Messaging namespace
            // so that it resolves naturally from the user's code.
            // This code produces a compilation unit as follows:
            //
            // // <auto-generated/>
            // #pragma warning disable
            // namespace CommunityToolkit.Mvvm.Messaging
            // {
            //     <ATTRIBUTES>
            //     internal static partial class __IMessengerExtensions
            //     {
            //     }
            // }
            return
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("CommunityToolkit.Mvvm.Messaging")).WithLeadingTrivia(TriviaList(
                    Comment("// <auto-generated/>"),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)))).AddMembers(
                ClassDeclaration("__IMessengerExtensions").AddModifiers(
                    Token(SyntaxKind.InternalKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.PartialKeyword))
                .AddAttributeLists(attributes.ToArray())))
                .NormalizeWhitespace();
        }

        /// <summary>
        /// Gets the <see cref="CompilationUnitSyntax"/> instance for the input recipient.
        /// </summary>
        /// <param name="recipientInfo">The input <see cref="RecipientInfo"/> instance to process.</param>
        /// <returns>The generated <see cref="CompilationUnitSyntax"/> instance for <paramref name="recipientInfo"/>.</returns>
        public static CompilationUnitSyntax GetSyntax(RecipientInfo recipientInfo)
        {
            // Create a static method to register all messages for a given recipient type.
            // This pattern is used so that the library doesn't have to
            // use GetType(...) and GetMethod(...) at runtime.
            // This pattern eliminates the need for reflection entirely
            // because type resolution occurs at "compile time" rather than at runtime.
            // This is the first overload being generated: a non-generic method doing the registration
            // with no tokens, which is the most common scenario and will help particularly with AOT.
            // This code will produce a syntax tree as follows:
            //
            // /// <summary>
            // /// Registers all declared message handlers for a given <see cref="<RECIPIENT_TYPE>"/> recipient, using the default channel.
            // /// </summary>
            // /// <inheritdoc cref="global::CommunityToolkit.Mvvm.Messaging.IMessengerExtensions.RegisterAll(global::CommunityToolkit.Mvvm.Messaging.IMessenger,object)"/>
            // public static void RegisterAll(this global::CommunityToolkit.Mvvm.Messaging.IMessenger messenger, <RECIPIENT_TYPE> recipient)
            // {
            //     <BODY>
            // }
            MethodDeclarationSyntax defaultChannelMethodDeclaration =
                MethodDeclaration(
                    PredefinedType(Token(SyntaxKind.VoidKeyword)),
                    Identifier("RegisterAll"))
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword))
                .WithLeadingTrivia(TriviaList(
                    Comment("/// <summary>"),
                    Comment($"/// Registers all declared message handlers for a given <see cref=\"{recipientInfo.TypeName}\"/> recipient, using the default channel."),
                    Comment("/// </summary>"),
                    Comment("/// <inheritdoc cref=\"global::CommunityToolkit.Mvvm.Messaging.IMessengerExtensions.RegisterAll(global::CommunityToolkit.Mvvm.Messaging.IMessenger,object)\"/>")))
                .AddParameterListParameters(
                    Parameter(Identifier("messenger"))
                        .AddModifiers(Token(SyntaxKind.ThisKeyword))
                        .WithType(IdentifierName("global::CommunityToolkit.Mvvm.Messaging.IMessenger")),
                    Parameter(Identifier("recipient")).WithType(IdentifierName(recipientInfo.TypeName)))
                .WithBody(Block(EnumerateRegistrationStatements(recipientInfo).ToArray()));

            // Create a generic version that will support all other cases with custom tokens.
            // This code will produce a syntax tree as follows:
            //
            // /// <summary>
            // /// Registers all declared message handlers for a given <see cref="<RECIPIENT_TYPE>"/> recipient.
            // /// </summary>
            // /// <inheritdoc cref="global::CommunityToolkit.Mvvm.Messaging.IMessengerExtensions.RegisterAll{TToken}(global::CommunityToolkit.Mvvm.Messaging.IMessenger,object,TToken)"/>
            // public static void RegisterAll<TToken>(global::CommunityToolkit.Mvvm.Messaging.IMessenger messenger, <RECIPIENT_TYPE> recipient, TToken token)
            //     where TToken : notnull, global::System.IEquatable<TToken>
            // {
            //     <BODY>
            // }
            MethodDeclarationSyntax customChannelMethodDeclaration =
                MethodDeclaration(
                    PredefinedType(Token(SyntaxKind.VoidKeyword)),
                    Identifier("RegisterAll"))
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword))
                .WithLeadingTrivia(TriviaList(
                    Comment("/// <summary>"),
                    Comment($"/// Registers all declared message handlers for a given <see cref=\"{recipientInfo.TypeName}\"/> recipient."),
                    Comment("/// </summary>"),
                    Comment("/// <inheritdoc cref=\"global::CommunityToolkit.Mvvm.Messaging.IMessengerExtensions.RegisterAll{TToken}(global::CommunityToolkit.Mvvm.Messaging.IMessenger,object,TToken)\"/>")))
                .AddTypeParameterListParameters(TypeParameter("TToken"))
                .AddConstraintClauses(
                    TypeParameterConstraintClause("TToken")
                    .AddConstraints(
                        TypeConstraint(IdentifierName("notnull")),
                        TypeConstraint(GenericName("global::System.IEquatable").AddTypeArgumentListArguments(IdentifierName("TToken")))))
                .AddParameterListParameters(
                    Parameter(Identifier("messenger"))
                        .AddModifiers(Token(SyntaxKind.ThisKeyword))
                        .WithType(IdentifierName("global::CommunityToolkit.Mvvm.Messaging.IMessenger")),
                    Parameter(Identifier("recipient")).WithType(IdentifierName(recipientInfo.TypeName)),
                    Parameter(Identifier("token")).WithType(IdentifierName("TToken")))
                .WithBody(Block(EnumerateRegistrationStatementsWithTokens(recipientInfo).ToArray()));

            // This code produces a compilation unit as follows:
            //
            // // <auto-generated/>
            // #pragma warning disable
            // #nullable enable
            // namespace CommunityToolkit.Mvvm.Messaging
            // {
            //     /// <inheritdoc/>
            //     partial class __IMessengerExtensions
            //     {
            //         <GENERATED_MEMBERS>
            //     }
            // }
            return
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("CommunityToolkit.Mvvm.Messaging")).WithLeadingTrivia(TriviaList(
                    Comment("// <auto-generated/>"),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)),
                    Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))).AddMembers(
                ClassDeclaration("__IMessengerExtensions").AddModifiers(
                    Token(TriviaList(Comment("/// <inheritdoc/>")), SyntaxKind.PartialKeyword, TriviaList()))
                .AddMembers(defaultChannelMethodDeclaration, customChannelMethodDeclaration)))
                .NormalizeWhitespace();
        }

        /// <summary>
        /// Gets a sequence of statements to register declared message handlers.
        /// </summary>
        /// <param name="recipientInfo">The input <see cref="RecipientInfo"/> instance to process.</param>
        /// <returns>The sequence of <see cref="StatementSyntax"/> instances to register message handlers.</returns>
        private static ImmutableArray<StatementSyntax> EnumerateRegistrationStatements(RecipientInfo recipientInfo)
        {
            using ImmutableArrayBuilder<StatementSyntax> statements = ImmutableArrayBuilder<StatementSyntax>.Rent();

            // This loop produces a sequence of statements as follows:
            //
            // messenger.Register<<TYPE_0>>(recipient);
            // messenger.Register<<TYPE_1>>(recipient);
            // ...
            // messenger.Register<<TYPE_N>>(recipient);
            foreach (string messageType in recipientInfo.MessageTypes)
            {
                statements.Add(
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("messenger"),
                                GenericName(Identifier("Register"))
                                .AddTypeArgumentListArguments(IdentifierName(messageType))))
                        .AddArgumentListArguments(Argument(IdentifierName("recipient")))));
            }

            return statements.ToImmutable();
        }

        /// <summary>
        /// Gets a sequence of statements to register declared message handlers with a custom token.
        /// </summary>
        /// <param name="recipientInfo">The input <see cref="RecipientInfo"/> instance to process.</param>
        /// <returns>The sequence of <see cref="StatementSyntax"/> instances to register message handlers.</returns>
        private static ImmutableArray<StatementSyntax> EnumerateRegistrationStatementsWithTokens(RecipientInfo recipientInfo)
        {
            using ImmutableArrayBuilder<StatementSyntax> statements = ImmutableArrayBuilder<StatementSyntax>.Rent();

            // This loop produces a sequence of statements as follows:
            //
            // messenger.Register<<TYPE_0>, TToken>(recipient, token);
            // messenger.Register<<TYPE_1>, TToken>(recipient, token);
            // ...
            // messenger.Register<<TYPE_N>, TToken>(recipient, token);
            foreach (string messageType in recipientInfo.MessageTypes)
            {
                statements.Add(
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("messenger"),
                                GenericName(Identifier("Register"))
                                .AddTypeArgumentListArguments(IdentifierName(messageType), IdentifierName("TToken"))))
                        .AddArgumentListArguments(Argument(IdentifierName("recipient")), Argument(IdentifierName("token")))));
            }

            return statements.ToImmutable();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

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
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.DebuggerNonUserCode]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            // [global::System.Obsolete("This type is not intended to be used directly by user code")]
            attributes.Add(
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName($"global::System.CodeDom.Compiler.GeneratedCode")).AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(IMessengerRegisterAllGenerator).FullName))),
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(IMessengerRegisterAllGenerator).Assembly.GetName().Version.ToString())))))));
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
            // namespace CommunityToolkit.Mvvm.Messaging.__Internals
            // {
            //     <ATTRIBUTES>
            //     internal static partial class __IMessengerExtensions
            //     {
            //     }
            // }
            return
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("CommunityToolkit.Mvvm.Messaging.__Internals")).WithLeadingTrivia(TriviaList(
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
            // Create a static factory method to register all messages for a given recipient type.
            // This follows the same pattern used in ObservableValidatorValidateAllPropertiesGenerator,
            // with the same advantages mentioned there (type safety, more AOT-friendly, etc.).
            // This is the first overload being generated: a non-generic method doing the registration
            // with no tokens, which is the most common scenario and will help particularly with AOT.
            // This code will produce a syntax tree as follows:
            //
            // [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            // [global::System.Obsolete("This method is not intended to be called directly by user code")]
            // public static global::System.Action<global::CommunityToolkit.Mvvm.Messaging.IMessenger, object> CreateAllMessagesRegistrator(<RECIPIENT_TYPE> _)
            // {
            //     static void RegisterAll(global::CommunityToolkit.Mvvm.Messaging.IMessenger messenger, object obj)
            //     {
            //         var recipient = (<INSTANCE_TYPE>)obj;
            //         <BODY>
            //     }
            //
            //     return RegisterAll;
            // }
            MethodDeclarationSyntax defaultChannelMethodDeclaration =
                MethodDeclaration(
                    GenericName("global::System.Action").AddTypeArgumentListArguments(
                        IdentifierName("global::CommunityToolkit.Mvvm.Messaging.IMessenger"),
                        PredefinedType(Token(SyntaxKind.ObjectKeyword))),
                    Identifier("CreateAllMessagesRegistrator")).AddAttributeLists(
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
                        Parameter(Identifier("_")).WithType(IdentifierName(recipientInfo.TypeName)))
                    .WithBody(Block(
                        LocalFunctionStatement(
                            PredefinedType(Token(SyntaxKind.VoidKeyword)),
                            Identifier("RegisterAll"))
                        .AddModifiers(Token(SyntaxKind.StaticKeyword))
                        .AddParameterListParameters(
                            Parameter(Identifier("messenger")).WithType(IdentifierName("global::CommunityToolkit.Mvvm.Messaging.IMessenger")),
                            Parameter(Identifier("obj")).WithType(PredefinedType(Token(SyntaxKind.ObjectKeyword))))
                        .WithBody(Block(
                            LocalDeclarationStatement(
                                VariableDeclaration(IdentifierName("var"))
                                .AddVariables(
                                    VariableDeclarator(Identifier("recipient"))
                                    .WithInitializer(EqualsValueClause(
                                        CastExpression(
                                            IdentifierName(recipientInfo.TypeName),
                                            IdentifierName("obj")))))))
                            .AddStatements(EnumerateRegistrationStatements(recipientInfo).ToArray())),
                        ReturnStatement(IdentifierName("RegisterAll"))));

            // Create a generic version that will support all other cases with custom tokens.
            // Note: the generic overload has a different name to simplify the lookup with reflection.
            // This code will produce a syntax tree as follows:
            //
            // [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
            // [global::System.Obsolete("This method is not intended to be called directly by user code")]
            // public static global::System.Action<global::CommunityToolkit.Mvvm.Messaging.IMessenger, object, TToken> CreateAllMessagesRegistratorWithToken<TToken>(<RECIPIENT_TYPE> _)
            //     where TToken : global::System.IEquatable<TToken>
            // {
            //     static void RegisterAll(global::CommunityToolkit.Mvvm.Messaging.IMessenger messenger, object obj, TToken token)
            //     {
            //         var recipient = (<INSTANCE_TYPE>)obj;
            //         <BODY>
            //     }
            //
            //     return RegisterAll;
            // }
            MethodDeclarationSyntax customChannelMethodDeclaration =
                MethodDeclaration(
                    GenericName("global::System.Action").AddTypeArgumentListArguments(
                        IdentifierName("global::CommunityToolkit.Mvvm.Messaging.IMessenger"),
                        PredefinedType(Token(SyntaxKind.ObjectKeyword)),
                        IdentifierName("TToken")),
                    Identifier("CreateAllMessagesRegistratorWithToken")).AddAttributeLists(
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
                        Parameter(Identifier("_")).WithType(IdentifierName(recipientInfo.TypeName)))
                    .AddTypeParameterListParameters(TypeParameter("TToken"))
                    .AddConstraintClauses(
                        TypeParameterConstraintClause("TToken")
                        .AddConstraints(TypeConstraint(GenericName("global::System.IEquatable").AddTypeArgumentListArguments(IdentifierName("TToken")))))
                    .WithBody(Block(
                        LocalFunctionStatement(
                            PredefinedType(Token(SyntaxKind.VoidKeyword)),
                            Identifier("RegisterAll"))
                        .AddModifiers(Token(SyntaxKind.StaticKeyword))
                        .AddParameterListParameters(
                            Parameter(Identifier("messenger")).WithType(IdentifierName("global::CommunityToolkit.Mvvm.Messaging.IMessenger")),
                            Parameter(Identifier("obj")).WithType(PredefinedType(Token(SyntaxKind.ObjectKeyword))),
                            Parameter(Identifier("token")).WithType(IdentifierName("TToken")))
                        .WithBody(Block(
                            LocalDeclarationStatement(
                                VariableDeclaration(IdentifierName("var"))
                                .AddVariables(
                                    VariableDeclarator(Identifier("recipient"))
                                    .WithInitializer(EqualsValueClause(
                                        CastExpression(
                                            IdentifierName(recipientInfo.TypeName),
                                            IdentifierName("obj")))))))
                            .AddStatements(EnumerateRegistrationStatementsWithTokens(recipientInfo).ToArray())),
                        ReturnStatement(IdentifierName("RegisterAll"))));

            // This code produces a compilation unit as follows:
            //
            // // <auto-generated/>
            // #pragma warning disable
            // namespace CommunityToolkit.Mvvm.Messaging.__Internals
            // {
            //     partial class __IMessengerExtensions
            //     {
            //         <GENERATED_MEMBERS>
            //     }
            // }
            return
                CompilationUnit().AddMembers(
                NamespaceDeclaration(IdentifierName("CommunityToolkit.Mvvm.Messaging.__Internals")).WithLeadingTrivia(TriviaList(
                    Comment("// <auto-generated/>"),
                    Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)))).AddMembers(
                ClassDeclaration("__IMessengerExtensions").AddModifiers(Token(SyntaxKind.PartialKeyword))
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

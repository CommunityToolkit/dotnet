// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Diagnostics;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
using CommunityToolkit.Mvvm.SourceGenerators.Input.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CommunityToolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.Mvvm.SourceGenerators;

/// <inheritdoc/>
partial class ICommandGenerator
{
    /// <summary>
    /// A container for all the logic for <see cref="ICommandGenerator"/>.
    /// </summary>
    private static class Execute
    {
        /// <summary>
        /// Processes a given target method.
        /// </summary>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance the method was annotated with.</param>
        /// <param name="diagnostics">The resulting diagnostics from the processing operation.</param>
        /// <returns>The resulting <see cref="CommandInfo"/> instance for <paramref name="methodSymbol"/>, if available.</returns>
        public static CommandInfo? GetInfo(IMethodSymbol methodSymbol, AttributeData attributeData, out ImmutableArray<Diagnostic> diagnostics)
        {
            ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();

            // Get the command field and property names
            (string fieldName, string propertyName) = GetGeneratedFieldAndPropertyNames(methodSymbol);

            // Get the command type symbols
            if (!TryMapCommandTypesFromMethod(
                methodSymbol,
                builder,
                out string? commandInterfaceType,
                out string? commandClassType,
                out string? delegateType,
                out bool supportsCancellation,
                out ImmutableArray<string> commandTypeArguments,
                out ImmutableArray<string> delegateTypeArguments))
            {
                goto Failure;
            }

            // Check the switch to allow concurrent executions
            if (!TryGetAllowConcurrentExecutionsSwitch(
                methodSymbol,
                attributeData,
                commandClassType,
                builder,
                out bool allowConcurrentExecutions))
            {
                goto Failure;   
            }

            // Get the CanExecute expression type, if any
            if (!TryGetCanExecuteExpressionType(
                methodSymbol,
                attributeData,
                commandTypeArguments,
                builder,
                out string? canExecuteMemberName,
                out CanExecuteExpressionType? canExecuteExpressionType))
            {
                goto Failure;
            }

            // Get the option to include a cancel command, if any
            if (!TryGetIncludeCancelCommandSwitch(
                methodSymbol,
                attributeData,
                commandClassType,
                supportsCancellation,
                builder,
                out bool generateCancelCommand))
            {
                goto Failure;
            }

            diagnostics = builder.ToImmutable();

            return new(
                methodSymbol.Name,
                fieldName,
                propertyName,
                commandInterfaceType,
                commandClassType,
                delegateType,
                commandTypeArguments,
                delegateTypeArguments,
                canExecuteMemberName,
                canExecuteExpressionType,
                allowConcurrentExecutions,
                generateCancelCommand);

            Failure:
            diagnostics = builder.ToImmutable();

            return null;
        }

        /// <summary>
        /// Creates the <see cref="MemberDeclarationSyntax"/> instances for a specified command.
        /// </summary>
        /// <param name="commandInfo">The input <see cref="CommandInfo"/> instance with the info to generate the command.</param>
        /// <returns>The <see cref="MemberDeclarationSyntax"/> instances for the input command.</returns>
        public static ImmutableArray<MemberDeclarationSyntax> GetSyntax(CommandInfo commandInfo)
        {
            // Prepare all necessary type names with type arguments
            string commandInterfaceTypeXmlName = commandInfo.CommandTypeArguments.IsEmpty
                ? commandInfo.CommandInterfaceType
                : commandInfo.CommandInterfaceType + "{T}";
            string commandClassTypeName = commandInfo.CommandTypeArguments.IsEmpty
                ? commandInfo.CommandClassType
                : $"{commandInfo.CommandClassType}<{string.Join(", ", commandInfo.CommandTypeArguments)}>";
            string commandInterfaceTypeName = commandInfo.CommandTypeArguments.IsEmpty
                ? commandInfo.CommandInterfaceType
                : $"{commandInfo.CommandInterfaceType}<{string.Join(", ", commandInfo.CommandTypeArguments)}>";
            string delegateTypeName = commandInfo.DelegateTypeArguments.IsEmpty
                ? commandInfo.DelegateType
                : $"{commandInfo.DelegateType}<{string.Join(", ", commandInfo.DelegateTypeArguments)}>";

            // Construct the generated field as follows:
            //
            // /// <summary>The backing field for <see cref="<COMMAND_PROPERTY_NAME>"/></summary>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // private <COMMAND_TYPE>? <COMMAND_FIELD_NAME>;
            FieldDeclarationSyntax fieldDeclaration =
                FieldDeclaration(
                VariableDeclaration(NullableType(IdentifierName(commandClassTypeName)))
                .AddVariables(VariableDeclarator(Identifier(commandInfo.FieldName))))
                .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(Comment($"/// <summary>The backing field for <see cref=\"{commandInfo.PropertyName}\"/>.</summary>")), SyntaxKind.OpenBracketToken, TriviaList())));

            // Prepares the argument to pass the underlying method to invoke
            ImmutableArray<ArgumentSyntax>.Builder commandCreationArguments = ImmutableArray.CreateBuilder<ArgumentSyntax>();

            // The first argument is the execute method, which is always present
            commandCreationArguments.Add(
                Argument(
                    ObjectCreationExpression(IdentifierName(delegateTypeName))
                    .AddArgumentListArguments(Argument(IdentifierName(commandInfo.MethodName)))));

            // Get the can execute expression, if available
            ExpressionSyntax? canExecuteExpression = commandInfo.CanExecuteExpressionType switch
            {
                // Create a lambda expression ignoring the input value:
                //
                // new <RELAY_COMMAND_TYPE>(<METHOD_EXPRESSION>, _ => <CAN_EXECUTE_METHOD>());
                CanExecuteExpressionType.MethodInvocationLambdaWithDiscard =>
                    SimpleLambdaExpression(
                        Parameter(Identifier(TriviaList(), SyntaxKind.UnderscoreToken, "_", "_", TriviaList())))
                    .WithExpressionBody(InvocationExpression(IdentifierName(commandInfo.CanExecuteMemberName!))),

                // Create a lambda expression returning the property value:
                //
                // new <RELAY_COMMAND_TYPE>(<METHOD_EXPRESSION>, () => <CAN_EXECUTE_PROPERTY>);
                CanExecuteExpressionType.PropertyAccessLambda =>
                    ParenthesizedLambdaExpression()
                    .WithExpressionBody(IdentifierName(commandInfo.CanExecuteMemberName!)),

                // Create a lambda expression again, but discarding the input value:
                //
                // new <RELAY_COMMAND_TYPE>(<METHOD_EXPRESSION>, _ => <CAN_EXECUTE_PROPERTY>);
                CanExecuteExpressionType.PropertyAccessLambdaWithDiscard =>
                    SimpleLambdaExpression(
                        Parameter(Identifier(TriviaList(), SyntaxKind.UnderscoreToken, "_", "_", TriviaList())))
                    .WithExpressionBody(IdentifierName(commandInfo.CanExecuteMemberName!)),

                    // Create a method groupd expression, which will become:
                    //
                    // new <RELAY_COMMAND_TYPE>(<METHOD_EXPRESSION>, <CAN_EXECUTE_METHOD>);
                    CanExecuteExpressionType.MethodGroup => IdentifierName(commandInfo.CanExecuteMemberName!),
                _ => null
            };

            // Add the can execute expression to the arguments, if available
            if (canExecuteExpression is not null)
            {
                commandCreationArguments.Add(Argument(canExecuteExpression));
            }

            // Enable concurrent executions, if requested
            if (commandInfo.AllowConcurrentExecutions)
            {
                commandCreationArguments.Add(Argument(LiteralExpression(SyntaxKind.TrueLiteralExpression)));
            }

            // Construct the generated property as follows (the explicit delegate cast is needed to avoid overload resolution conflicts):
            //
            // /// <summary>Gets an <see cref="<COMMAND_INTERFACE_TYPE>" instance wrapping <see cref="<METHOD_NAME>"/> and <see cref="<OPTIONAL_CAN_EXECUTE>"/>.</summary>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.DebuggerNonUserCode]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // public <COMMAND_TYPE> <COMMAND_PROPERTY_NAME> => <COMMAND_FIELD_NAME> ??= new <RELAY_COMMAND_TYPE>(<COMMAND_CREATION_ARGUMENTS>);
            PropertyDeclarationSyntax propertyDeclaration =
                PropertyDeclaration(
                    IdentifierName(commandInterfaceTypeName),
                    Identifier(commandInfo.PropertyName))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(Comment(
                        $"/// <summary>Gets an <see cref=\"{commandInterfaceTypeXmlName}\"/> instance wrapping <see cref=\"{commandInfo.MethodName}\"/>.</summary>")),
                        SyntaxKind.OpenBracketToken,
                        TriviaList())),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        AssignmentExpression(
                            SyntaxKind.CoalesceAssignmentExpression,
                            IdentifierName(commandInfo.FieldName),
                            ObjectCreationExpression(IdentifierName(commandClassTypeName))
                            .AddArgumentListArguments(commandCreationArguments.ToArray()))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            // Conditionally declare the additional members for the cancel commands
            if (commandInfo.IncludeCancelCommand)
            {
                // Prepare all necessary member and type names
                string cancelCommandFieldName = $"{commandInfo.FieldName.Substring(0, commandInfo.FieldName.Length - "Command".Length)}CancelCommand";
                string cancelCommandPropertyName = $"{commandInfo.PropertyName.Substring(0, commandInfo.PropertyName.Length - "Command".Length)}CancelCommand";

                // Construct the generated field for the cancel command as follows:
                //
                // /// <summary>The backing field for <see cref="<COMMAND_PROPERTY_NAME>"/></summary>
                // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
                // private global::System.Windows.Input.ICommand? <CANCEL_COMMAND_FIELD_NAME>;
                FieldDeclarationSyntax cancelCommandFieldDeclaration =
                    FieldDeclaration(
                    VariableDeclaration(NullableType(IdentifierName("global::System.Windows.Input.ICommand")))
                    .AddVariables(VariableDeclarator(Identifier(cancelCommandFieldName))))
                    .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                    .AddAttributeLists(
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                            .AddArgumentListArguments(
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).FullName))),
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).Assembly.GetName().Version.ToString()))))))
                        .WithOpenBracketToken(Token(TriviaList(Comment($"/// <summary>The backing field for <see cref=\"{cancelCommandPropertyName}\"/>.</summary>")), SyntaxKind.OpenBracketToken, TriviaList())));

                // Construct the generated property as follows (the explicit delegate cast is needed to avoid overload resolution conflicts):
                //
                // /// <summary>Gets an <see cref="global::System.Windows.Input.ICommand" instance that can be used to cancel <see cref="<COMMAND_PROPERTY_NAME>"/>.</summary>
                // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
                // [global::System.Diagnostics.DebuggerNonUserCode]
                // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                // public global::System.Windows.Input.ICommand <CANCEL_COMMAND_PROPERTY_NAME> => <CANCEL_COMMAND_FIELD_NAME> ??= global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommandExtensions.CreateCancelCommand(<COMMAND_PROPERTY_NAME>);
                PropertyDeclarationSyntax cancelCommandPropertyDeclaration =
                    PropertyDeclaration(
                        IdentifierName("global::System.Windows.Input.ICommand"),
                        Identifier(cancelCommandPropertyName))
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddAttributeLists(
                        AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                            .AddArgumentListArguments(
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).FullName))),
                                AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ICommandGenerator).Assembly.GetName().Version.ToString()))))))
                        .WithOpenBracketToken(Token(TriviaList(Comment(
                            $"/// <summary>Gets an <see cref=\"global::System.Windows.Input.ICommand\"/> instance that can be used to cancel <see cref=\"{commandInfo.PropertyName}\"/>.</summary>")),
                            SyntaxKind.OpenBracketToken,
                            TriviaList())),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode")))),
                        AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            AssignmentExpression(
                                SyntaxKind.CoalesceAssignmentExpression,
                                IdentifierName(cancelCommandFieldName),
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommandExtensions"),
                                        IdentifierName("CreateCancelCommand")))
                                .AddArgumentListArguments(Argument(IdentifierName(commandInfo.PropertyName))))))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                return ImmutableArray.Create<MemberDeclarationSyntax>(fieldDeclaration, propertyDeclaration, cancelCommandFieldDeclaration, cancelCommandPropertyDeclaration);
            }

            return ImmutableArray.Create<MemberDeclarationSyntax>(fieldDeclaration, propertyDeclaration);
        }

        /// <summary>
        /// Get the generated field and property names for the input method.
        /// </summary>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <returns>The generated field and property names for <paramref name="methodSymbol"/>.</returns>
        private static (string FieldName, string PropertyName) GetGeneratedFieldAndPropertyNames(IMethodSymbol methodSymbol)
        {
            string propertyName = methodSymbol.Name;

            if (methodSymbol.ReturnType.HasFullyQualifiedName("global::System.Threading.Tasks.Task") &&
                methodSymbol.Name.EndsWith("Async"))
            {
                propertyName = propertyName.Substring(0, propertyName.Length - "Async".Length);
            }

            propertyName += "Command";

            string fieldName = $"{char.ToLower(propertyName[0])}{propertyName.Substring(1)}";

            return (fieldName, propertyName);
        }

        /// <summary>
        /// Gets the type symbols for the input method, if supported.
        /// </summary>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="commandInterfaceType">The command interface type name.</param>
        /// <param name="commandClassType">The command class type name.</param>
        /// <param name="delegateType">The delegate type name for the wrapped method.</param>
        /// <param name="supportsCancellation">Indicates whether or not the resulting command supports cancellation.</param>
        /// <param name="commandTypeArguments">The type arguments for <paramref name="commandInterfaceType"/> and <paramref name="commandClassType"/>, if any.</param>
        /// <param name="delegateTypeArguments">The type arguments for <paramref name="delegateType"/>, if any.</param>
        /// <returns>Whether or not <paramref name="methodSymbol"/> was valid and the requested types have been set.</returns>
        private static bool TryMapCommandTypesFromMethod(
            IMethodSymbol methodSymbol,
            ImmutableArray<Diagnostic>.Builder diagnostics,
            [NotNullWhen(true)] out string? commandInterfaceType,
            [NotNullWhen(true)] out string? commandClassType,
            [NotNullWhen(true)] out string? delegateType,
            out bool supportsCancellation,
            out ImmutableArray<string> commandTypeArguments,
            out ImmutableArray<string> delegateTypeArguments)
        {
            // Map <void, void> to IRelayCommand, RelayCommand, Action
            if (methodSymbol.ReturnsVoid && methodSymbol.Parameters.Length == 0)
            {
                commandInterfaceType = "global::CommunityToolkit.Mvvm.Input.IRelayCommand";
                commandClassType = "global::CommunityToolkit.Mvvm.Input.RelayCommand";
                delegateType = "global::System.Action";
                supportsCancellation = false;
                commandTypeArguments = ImmutableArray<string>.Empty;
                delegateTypeArguments = ImmutableArray<string>.Empty;

                return true;
            }

            // Map <T, void> to IRelayCommand<T>, RelayCommand<T>, Action<T>
            if (methodSymbol.ReturnsVoid &&
                methodSymbol.Parameters.Length == 1 &&
                methodSymbol.Parameters[0] is IParameterSymbol { RefKind: RefKind.None, Type: { IsRefLikeType: false, TypeKind: not TypeKind.Pointer and not TypeKind.FunctionPointer } } parameter)
            {
                commandInterfaceType = "global::CommunityToolkit.Mvvm.Input.IRelayCommand";
                commandClassType = "global::CommunityToolkit.Mvvm.Input.RelayCommand";
                delegateType = "global::System.Action";
                supportsCancellation = false;
                commandTypeArguments = ImmutableArray.Create(parameter.Type.GetFullyQualifiedName());
                delegateTypeArguments = ImmutableArray.Create(parameter.Type.GetFullyQualifiedName());

                return true;
            }

            if (methodSymbol.ReturnType.HasFullyQualifiedName("global::System.Threading.Tasks.Task"))
            {
                // Map <void, Task> to IAsyncRelayCommand, AsyncRelayCommand, Func<Task>
                if (methodSymbol.Parameters.Length == 0)
                {
                    commandInterfaceType = "global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommand";
                    commandClassType = "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommand";
                    delegateType = "global::System.Func";
                    supportsCancellation = false;
                    commandTypeArguments = ImmutableArray<string>.Empty;
                    delegateTypeArguments = ImmutableArray.Create("global::System.Threading.Tasks.Task");

                    return true;
                }

                if (methodSymbol.Parameters.Length == 1 &&
                    methodSymbol.Parameters[0] is IParameterSymbol { RefKind: RefKind.None, Type: { IsRefLikeType: false, TypeKind: not TypeKind.Pointer and not TypeKind.FunctionPointer } } singleParameter)
                {
                    // Map <CancellationToken, Task> to IAsyncRelayCommand, AsyncRelayCommand, Func<CancellationToken, Task>
                    if (singleParameter.Type.HasFullyQualifiedName("global::System.Threading.CancellationToken"))
                    {
                        commandInterfaceType = "global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommand";
                        commandClassType = "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommand";
                        delegateType = "global::System.Func";
                        supportsCancellation = true;
                        commandTypeArguments = ImmutableArray<string>.Empty;
                        delegateTypeArguments = ImmutableArray.Create(singleParameter.Type.GetFullyQualifiedName(), "global::System.Threading.Tasks.Task");

                        return true;
                    }

                    // Map <T, Task> to IAsyncRelayCommand<T>, AsyncRelayCommand<T>, Func<T, Task>
                    commandInterfaceType = "global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommand";
                    commandClassType = "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommand";
                    delegateType = "global::System.Func";
                    supportsCancellation = false;
                    commandTypeArguments = ImmutableArray.Create(singleParameter.Type.GetFullyQualifiedName());
                    delegateTypeArguments = ImmutableArray.Create(singleParameter.Type.GetFullyQualifiedName(), "global::System.Threading.Tasks.Task");

                    return true;
                }

                // Map <T, CancellationToken, Task> to IAsyncRelayCommand<T>, AsyncRelayCommand<T>, Func<T, CancellationToken, Task>
                if (methodSymbol.Parameters.Length == 2 &&
                    methodSymbol.Parameters[0] is IParameterSymbol { RefKind: RefKind.None, Type: { IsRefLikeType: false, TypeKind: not TypeKind.Pointer and not TypeKind.FunctionPointer } } firstParameter &&
                    methodSymbol.Parameters[1] is IParameterSymbol { RefKind: RefKind.None, Type: { IsRefLikeType: false, TypeKind: not TypeKind.Pointer and not TypeKind.FunctionPointer } } secondParameter &&
                    secondParameter.Type.HasFullyQualifiedName("global::System.Threading.CancellationToken"))
                {
                    commandInterfaceType = "global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommand";
                    commandClassType = "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommand";
                    delegateType = "global::System.Func";
                    supportsCancellation = true;
                    commandTypeArguments = ImmutableArray.Create(firstParameter.Type.GetFullyQualifiedName());
                    delegateTypeArguments = ImmutableArray.Create(firstParameter.Type.GetFullyQualifiedName(), secondParameter.Type.GetFullyQualifiedName(), "global::System.Threading.Tasks.Task");

                    return true;
                }
            }

            diagnostics.Add(InvalidICommandMethodSignatureError, methodSymbol, methodSymbol.ContainingType, methodSymbol);

            commandInterfaceType = null;
            commandClassType = null;
            delegateType = null;
            supportsCancellation = false;
            commandTypeArguments = ImmutableArray<string>.Empty;
            delegateTypeArguments = ImmutableArray<string>.Empty;

            return false;
        }

        /// <summary>
        /// Checks whether or not the user has requested to configure the handling of concurrent executions.
        /// </summary>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance the method was annotated with.</param>
        /// <param name="commandClassType">The command class type name.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="allowConcurrentExecutions">Whether or not concurrent executions have been enabled.</param>
        /// <returns>Whether or not a value for <paramref name="allowConcurrentExecutions"/> could be retrieved successfully.</returns>
        private static bool TryGetAllowConcurrentExecutionsSwitch(
            IMethodSymbol methodSymbol,
            AttributeData attributeData,
            string commandClassType,
            ImmutableArray<Diagnostic>.Builder diagnostics,
            out bool allowConcurrentExecutions)
        {
            // Try to get the custom switch for concurrent executions (the default is false)
            if (!attributeData.TryGetNamedArgument("AllowConcurrentExecutions", out allowConcurrentExecutions))
            {
                allowConcurrentExecutions = false;

                return true;
            }

            // If the current type is an async command type and concurrent execution is disabled, pass that value to the constructor.
            // If concurrent executions are allowed, there is no need to add any additional argument, as that is the default value.
            if (commandClassType is "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommand")
            {
                return true;
            }
            else
            {
                diagnostics.Add(InvalidConcurrentExecutionsParameterError, methodSymbol, methodSymbol.ContainingType, methodSymbol);

                return false;
            }
        }

        /// <summary>
        /// Checks whether or not the user has requested to also generate a cancel command.
        /// </summary>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance the method was annotated with.</param>
        /// <param name="commandClassType">The command class type name.</param>
        /// <param name="supportsCancellation">Indicates whether or not the command supports cancellation.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="generateCancelCommand">Whether or not concurrent executions have been enabled.</param>
        /// <returns>Whether or not a value for <paramref name="generateCancelCommand"/> could be retrieved successfully.</returns>
        private static bool TryGetIncludeCancelCommandSwitch(
            IMethodSymbol methodSymbol,
            AttributeData attributeData,
            string commandClassType,
            bool supportsCancellation,
            ImmutableArray<Diagnostic>.Builder diagnostics,
            out bool generateCancelCommand)
        {
            // Try to get the custom switch for cancel command generation (the default is false)
            if (!attributeData.TryGetNamedArgument("IncludeCancelCommand", out generateCancelCommand))
            {
                generateCancelCommand = false;

                return true;
            }

            // If the current type is an async command type and cancellation is supported, pass that value to the constructor.
            // Otherwise, the current attribute use is not valid, so a diagnostic message should be produced.
            if (commandClassType is "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommand" &&
                supportsCancellation)
            {
                return true;
            }
            else
            {
                diagnostics.Add(InvalidIncludeCancelCommandParameterError, methodSymbol, methodSymbol.ContainingType, methodSymbol);

                return false;
            }
        }

        /// <summary>
        /// Tries to get the expression type for the "CanExecute" property, if available.
        /// </summary>
        /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to process.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> instance for <paramref name="methodSymbol"/>.</param>
        /// <param name="commandTypeArguments">The command type arguments, if any.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="canExecuteMemberName">The resulting can execute member name, if available.</param>
        /// <param name="canExecuteExpressionType">The resulting expression type, if available.</param>
        /// <returns>Whether or not a value for <paramref name="canExecuteMemberName"/> and <paramref name="canExecuteExpressionType"/> could be determined (may include <see langword="null"/>).</returns>
        private static bool TryGetCanExecuteExpressionType(
            IMethodSymbol methodSymbol,
            AttributeData attributeData,
            ImmutableArray<string> commandTypeArguments,
            ImmutableArray<Diagnostic>.Builder diagnostics,
            out string? canExecuteMemberName,
            out CanExecuteExpressionType? canExecuteExpressionType)
        {
            // Get the can execute member, if any
            if (!attributeData.TryGetNamedArgument("CanExecute", out string? memberName))
            {
                canExecuteMemberName = null;
                canExecuteExpressionType = null;

                return true;
            }

            if (memberName is null)
            {
                diagnostics.Add(InvalidCanExecuteMemberName, methodSymbol, memberName ?? string.Empty, methodSymbol.ContainingType);

                goto Failure;
            }

            ImmutableArray<ISymbol> canExecuteSymbols = methodSymbol.ContainingType!.GetMembers(memberName);

            if (canExecuteSymbols.IsEmpty)
            {
                // Special case for when the target member is a generated property from [ObservableProperty]
                if (TryGetCanExecuteMemberFromGeneratedProperty(memberName, methodSymbol.ContainingType, commandTypeArguments, out canExecuteExpressionType))
                {
                    canExecuteMemberName = memberName;

                    return true;
                }

                diagnostics.Add(InvalidCanExecuteMemberName, methodSymbol, memberName, methodSymbol.ContainingType);
            }
            else if (canExecuteSymbols.Length > 1)
            {
                diagnostics.Add(MultipleCanExecuteMemberNameMatches, methodSymbol, memberName, methodSymbol.ContainingType);
            }
            else if (TryGetCanExecuteExpressionFromSymbol(canExecuteSymbols[0], commandTypeArguments, out canExecuteExpressionType))
            {
                canExecuteMemberName = memberName;

                return true;
            }
            else
            {
                diagnostics.Add(InvalidCanExecuteMember, methodSymbol, memberName, methodSymbol.ContainingType);
            }

            Failure:
            canExecuteMemberName = null;
            canExecuteExpressionType = null;

            return false;
        }

        /// <summary>
        /// Gets the expression type for the can execute logic, if possible.
        /// </summary>
        /// <param name="canExecuteSymbol">The can execute member symbol (either a method or a property).</param>
        /// <param name="commandTypeArguments">The type arguments for the command interface, if any.</param>
        /// <param name="canExecuteExpressionType">The resulting can execute expression type, if available.</param>
        /// <returns>Whether or not <paramref name="canExecuteExpressionType"/> was set and the input symbol was valid.</returns>
        private static bool TryGetCanExecuteExpressionFromSymbol(
            ISymbol canExecuteSymbol,
            ImmutableArray<string> commandTypeArguments,
            [NotNullWhen(true)] out CanExecuteExpressionType? canExecuteExpressionType)
        {
            if (canExecuteSymbol is IMethodSymbol canExecuteMethodSymbol)
            {
                // The return type must always be a bool
                if (!canExecuteMethodSymbol.ReturnType.HasFullyQualifiedName("bool"))
                {
                    goto Failure;
                }

                // Parameterless methods are always valid
                if (canExecuteMethodSymbol.Parameters.IsEmpty)
                {
                    // If the command is generic, the input value is ignored
                    if (commandTypeArguments.Length > 0)
                    {
                        canExecuteExpressionType = CanExecuteExpressionType.MethodInvocationLambdaWithDiscard;
                    }
                    else
                    {
                        canExecuteExpressionType = CanExecuteExpressionType.MethodGroup;
                    }

                    return true;
                }

                // If the method has parameters, it has to have a single one matching the command type
                if (canExecuteMethodSymbol.Parameters.Length == 1 &&
                    commandTypeArguments.Length == 1 &&
                    canExecuteMethodSymbol.Parameters[0].Type.HasFullyQualifiedName(commandTypeArguments[0]))
                {
                    // Create a method group expression again
                    canExecuteExpressionType = CanExecuteExpressionType.MethodGroup;

                    return true;
                }
            }
            else if (canExecuteSymbol is IPropertySymbol { GetMethod: not null } canExecutePropertySymbol)
            {
                // The property type must always be a bool
                if (!canExecutePropertySymbol.Type.HasFullyQualifiedName("bool"))
                {
                    goto Failure;
                }

                if (commandTypeArguments.Length > 0)
                {
                    canExecuteExpressionType = CanExecuteExpressionType.PropertyAccessLambdaWithDiscard;
                }
                else
                {
                    canExecuteExpressionType = CanExecuteExpressionType.PropertyAccessLambda;
                }

                return true;
            }

            Failure:
            canExecuteExpressionType = null;

            return false;
        }

        /// <summary>
        /// Gets the expression type for the can execute logic, if possible.
        /// </summary>
        /// <param name="memberName">The member name passed to <c>[ICommand(CanExecute = ...)]</c>.</param>
        /// <param name="containingType">The containing type for the method annotated with <c>[ICommand]</c>.</param>
        /// <param name="commandTypeArguments">The type arguments for the command interface, if any.</param>
        /// <param name="canExecuteExpressionType">The resulting can execute expression type, if available.</param>
        /// <returns>Whether or not <paramref name="canExecuteExpressionType"/> was set and the input symbol was valid.</returns>
        private static bool TryGetCanExecuteMemberFromGeneratedProperty(
            string memberName,
            INamedTypeSymbol containingType,
            ImmutableArray<string> commandTypeArguments,
            [NotNullWhen(true)] out CanExecuteExpressionType? canExecuteExpressionType)
        {
            foreach (ISymbol memberSymbol in containingType.GetMembers())
            {
                // Only look for instance fields of bool type
                if (memberSymbol is not IFieldSymbol fieldSymbol ||
                    fieldSymbol is { IsStatic: true } ||
                    !fieldSymbol.Type.HasFullyQualifiedName("bool"))
                {
                    continue;
                }

                ImmutableArray<AttributeData> attributes = memberSymbol.GetAttributes();

                // Only filter fields with the [ObservableProperty] attribute
                if (memberSymbol is IFieldSymbol &&
                    !attributes.Any(static a => a.AttributeClass?.HasFullyQualifiedName(
                        "global::CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") == true))
                {
                    continue;
                }

                // Get the target property name either directly or matching the generated one
                string propertyName = ObservablePropertyGenerator.Execute.GetGeneratedPropertyName(fieldSymbol);

                // If the generated property name matches, get the right expression type
                if (memberName == propertyName)
                {
                    if (commandTypeArguments.Length > 0)
                    {
                        canExecuteExpressionType = CanExecuteExpressionType.PropertyAccessLambdaWithDiscard;
                    }
                    else
                    {
                        canExecuteExpressionType = CanExecuteExpressionType.PropertyAccessLambda;
                    }

                    return true;
                }
            }

            canExecuteExpressionType = null;

            return false;
        }
    }
}

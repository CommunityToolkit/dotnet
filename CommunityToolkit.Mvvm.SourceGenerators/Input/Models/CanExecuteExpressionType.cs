// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

namespace CommunityToolkit.Mvvm.SourceGenerators.Input.Models;

/// <summary>
/// A type describing the type of expression for the "CanExecute" property of a command.
/// </summary>
public enum CanExecuteExpressionType
{
    /// <summary>
    /// A method invocation lambda with discard: <c>_ => Method()</c>.
    /// </summary>
    MethodInvocationLambdaWithDiscard,

    /// <summary>
    /// A property access lambda: <c>() => Property</c>.
    /// </summary>
    PropertyAccessLambda,

    /// <summary>
    /// A property access lambda with discard: <c>_ => Property</c>.
    /// </summary>
    PropertyAccessLambdaWithDiscard,

    /// <summary>
    /// A method group expression: <c>Method</c>.
    /// </summary>
    MethodGroup
}

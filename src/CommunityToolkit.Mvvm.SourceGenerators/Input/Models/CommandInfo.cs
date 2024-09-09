// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;
using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace CommunityToolkit.Mvvm.SourceGenerators.Input.Models;

/// <summary>
/// A model with gathered info on a given command method.
/// </summary>
/// <param name="MethodName">The name of the target method.</param>
/// <param name="FieldName">The resulting field name for the generated command.</param>
/// <param name="PropertyName">The resulting property name for the generated command.</param>
/// <param name="CommandInterfaceType">The command interface type name.</param>
/// <param name="CommandClassType">The command class type name.</param>
/// <param name="DelegateType">The delegate type name for the wrapped method.</param>
/// <param name="CommandTypeArguments">The type arguments for <paramref name="CommandInterfaceType"/> and <paramref name="CommandClassType"/>, if any.</param>
/// <param name="DelegateTypeArguments">The type arguments for <paramref name="DelegateType"/>, if any.</param>
/// <param name="CanExecuteMemberName">The member name for the can execute check, if available.</param>
/// <param name="CanExecuteExpressionType">The can execute expression type, if available.</param>
/// <param name="AllowConcurrentExecutions">Whether or not concurrent executions have been enabled.</param>
/// <param name="FlowExceptionsToTaskScheduler">Whether or not exceptions should flow to the task scheduler.</param>
/// <param name="IncludeCancelCommand">Whether or not to also generate a cancel command.</param>
/// <param name="ForwardedAttributes">The sequence of forwarded attributes for the generated members.</param>
internal sealed record CommandInfo(
    string MethodName,
    string FieldName,
    string PropertyName,
    string CommandInterfaceType,
    string CommandClassType,
    string DelegateType,
    EquatableArray<string> CommandTypeArguments,
    EquatableArray<string> DelegateTypeArguments,
    string? CanExecuteMemberName,
    CanExecuteExpressionType? CanExecuteExpressionType,
    bool AllowConcurrentExecutions,
    bool FlowExceptionsToTaskScheduler,
    bool IncludeCancelCommand,
    EquatableArray<AttributeInfo> ForwardedAttributes);

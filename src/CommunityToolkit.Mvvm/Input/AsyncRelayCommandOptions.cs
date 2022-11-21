// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Mvvm.Input;

/// <summary>
/// Options to customize the behavior of <see cref="AsyncRelayCommand"/> and <see cref="AsyncRelayCommand{T}"/> instances.
/// </summary>
[Flags]
public enum AsyncRelayCommandOptions
{
    /// <summary>
    /// No option is specified. The <see cref="AsyncRelayCommand"/> and <see cref="AsyncRelayCommand{T}"/> types will use their default behavior:
    /// <list type="bullet">
    ///     <item>Concurrent execution is disallowed: a command is disabled if there is a pending asynchronous execution running.</item>
    ///     <item>
    ///         <para>
    ///             Exceptions are thrown on the calling context: calling <see cref="AsyncRelayCommand.Execute(object?)"/> will await the
    ///             returned <see cref="System.Threading.Tasks.Task"/> for the operation, and propagate the exception on the calling context.        
    ///         </para>
    ///         <para>This behavior is consistent with synchronous commands, where exceptions in <see cref="RelayCommand.Execute(object?)"/> behave the same.</para>
    ///     </item>
    /// </list>
    /// </summary>
    None = 0,

    /// <summary>
    /// <para>Concurrent executions are allowed. This option makes it so that the same command can be invoked concurrently multiple times.</para>
    /// <para>
    /// Note that additional considerations should be taken into account in this case:
    /// <list type="bullet">
    ///     <item>If the command supports cancellation, previous invocations will automatically be canceled if a new one is started.</item>
    ///     <item>The <see cref="AsyncRelayCommand.ExecutionTask"/> property will always represent the operation that was started last.</item>
    /// </list>
    /// </para>
    /// </summary>
    AllowConcurrentExecutions = 1 << 0,

    /// <summary>
    /// <para>Exceptions are not thrown on the calling context, and are propagated to <see cref="System.Threading.Tasks.TaskScheduler.UnobservedTaskException"/> instead.</para>
    /// <para>
    /// This affects how calls to <see cref="AsyncRelayCommand.Execute(object?)"/> behave. When this option is used, if an operation fails, that exception will not
    /// be rethrown on the calling context (as it is not awaited there). Instead, it will flow to <see cref="System.Threading.Tasks.TaskScheduler.UnobservedTaskException"/>.
    /// </para>
    /// <para>
    /// This option enables more advanced scenarios, where the <see cref="AsyncRelayCommand.ExecutionTask"/> property can be used to inspect the state of an operation
    /// that was queued. That is, even if the operation failed or was canceled, the details of that can be retrieved at a later time by accessing this property.
    /// </para>
    /// </summary>
    FlowExceptionsToTaskScheduler = 1 << 1
}

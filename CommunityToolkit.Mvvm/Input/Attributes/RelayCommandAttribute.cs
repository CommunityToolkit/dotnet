// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Windows.Input;

namespace CommunityToolkit.Mvvm.Input;

/// <summary>
/// An attribute that can be used to automatically generate <see cref="ICommand"/> properties from declared methods. When this attribute
/// is used to decorate a method, a generator will create a command property with the corresponding <see cref="IRelayCommand"/> interface
/// depending on the signature of the method. If an invalid method signature is used, the generator will report an error.
/// <para>
/// In order to use this attribute, the containing type doesn't need to implement any interfaces. The generated properties will be lazily
/// assigned but their value will never change, so there is no need to support property change notifications or other additional functionality.
/// </para>
/// <para>
/// This attribute can be used as follows:
/// <code>
/// partial class MyViewModel
/// {
///     [RelayCommand]
///     private void GreetUser(User? user)
///     {
///         Console.WriteLine($"Hello {user.Name}!");
///     }
/// }
/// </code>
/// And with this, code analogous to this will be generated:
/// <code>
/// partial class MyViewModel
/// {
///     private RelayCommand? greetUserCommand;
///
///     public IRelayCommand GreetUserCommand => greetUserCommand ??= new RelayCommand(GreetUser);
/// }
/// </code>
/// </para>
/// <para>
/// The following signatures are supported for annotated methods:
/// <code>
/// void Method();
/// </code>
/// Will generate an <see cref="IRelayCommand"/> property (using a <see cref="RelayCommand"/> instance).
/// <code>
/// void Method(T?);
/// </code>
/// Will generate an <see cref="IRelayCommand{T}"/> property (using a <see cref="RelayCommand{T}"/> instance).
/// <code>
/// Task Method();
/// Task Method(CancellationToken);
/// Task&lt;T&gt; Method();
/// Task&lt;T&gt; Method(CancellationToken);
/// </code>
/// Will both generate an <see cref="IAsyncRelayCommand"/> property (using an <see cref="AsyncRelayCommand{T}"/> instance).
/// <code>
/// Task Method(T?);
/// Task Method(T?, CancellationToken);
/// Task&lt;T&gt; Method(T?);
/// Task&lt;T&gt; Method(T?, CancellationToken);
/// </code>
/// Will both generate an <see cref="IAsyncRelayCommand{T}"/> property (using an <see cref="AsyncRelayCommand{T}"/> instance).
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class RelayCommandAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the property or method that will be invoked to check whether the
    /// generated command can be executed at any given time. The referenced member needs to return
    /// a <see cref="bool"/> value, and has to have a signature compatible with the target command.
    /// </summary>
    public string? CanExecute { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether or not to allow concurrent executions for an asynchronous command.
    /// <para>
    /// When set for an attribute used on a method that would result in an <see cref="AsyncRelayCommand"/> or an
    /// <see cref="AsyncRelayCommand{T}"/> property to be generated, this will modify the behavior of these commands
    /// when an execution is invoked while a previous one is still running. It is the same as creating an instance of
    /// these command types with a constructor such as <see cref="AsyncRelayCommand(Func{System.Threading.Tasks.Task}, AsyncRelayCommandOptions)"/>
    /// and using the <see cref="AsyncRelayCommandOptions.AllowConcurrentExecutions"/> value.
    /// </para>
    /// </summary>
    /// <remarks>Using this property is not valid if the target command doesn't map to an asynchronous command.</remarks>
    public bool AllowConcurrentExecutions { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether or not to exceptions should be propagated to <see cref="System.Threading.Tasks.TaskScheduler.UnobservedTaskException"/>.
    /// <para>
    /// When set for an attribute used on a method that would result in an <see cref="AsyncRelayCommand"/> or an
    /// <see cref="AsyncRelayCommand{T}"/> property to be generated, this will modify the behavior of these commands
    /// in case an exception is thrown by the underlying operation. It is the same as creating an instance of
    /// these command types with a constructor such as <see cref="AsyncRelayCommand(Func{System.Threading.Tasks.Task}, AsyncRelayCommandOptions)"/>
    /// and using the <see cref="AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler"/> value.
    /// </para>
    /// </summary>
    /// <remarks>Using this property is not valid if the target command doesn't map to an asynchronous command.</remarks>
    public bool FlowExceptionsToTaskScheduler { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether a cancel command should also be generated for an asynchronous command.
    /// <para>
    /// When set to <see langword="true"/>, this additional code will be generated:
    /// <code>
    /// partial class MyViewModel
    /// {
    ///     private ICommand? loginUserCancelCommand;
    ///
    ///     public ICommand LoginUserCancelCommand => loginUserCancelCommand ??= LoginUserCommand.CreateCancelCommand();
    /// }
    /// </code>
    /// Where <c>LoginUserCommand</c> is an <see cref="IAsyncRelayCommand"/> defined in the class (or generated by this attribute as well).
    /// </para>
    /// </summary>
    /// <remarks>Using this property is not valid if the target command doesn't map to a cancellable asynchronous command.</remarks>
    public bool IncludeCancelCommand { get; init; }
}

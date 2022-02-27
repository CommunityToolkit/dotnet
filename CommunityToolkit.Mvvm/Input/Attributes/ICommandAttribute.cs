// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
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
///     [ICommand]
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
/// </code>
/// Will both generate an <see cref="IAsyncRelayCommand"/> property (using an <see cref="AsyncRelayCommand{T}"/> instance).
/// <code>
/// Task Method(T?);
/// Task Method(T?, CancellationToken);
/// </code>
/// Will both generate an <see cref="IAsyncRelayCommand{T}"/> property (using an <see cref="AsyncRelayCommand{T}"/> instance).
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[Conditional("MVVMTOOLKIT_KEEP_SOURCE_GENERATOR_ATTRIBUTES")]
public sealed class ICommandAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the property or method that will be invoked to check whether the
    /// generated command can be executed at any given time. The referenced member needs to return
    /// a <see cref="bool"/> value, and has to have a signature compatible with the target command.
    /// </summary>
    public string? CanExecute { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether or not to allow concurrent executions for an asynchronous command.
    /// When set for an attribute used on a method that would result in an <see cref="AsyncRelayCommand"/> or an
    /// <see cref="AsyncRelayCommand{T}"/> property to be generated, this will modify the behavior of these commands
    /// when an execution is invoked while a previous one is still running. It is the same as creating an instance of
    /// these command types with a constructor such as <see cref="AsyncRelayCommand(Func{System.Threading.Tasks.Task}, bool)"/>.
    /// </summary>
    public bool AllowConcurrentExecutions { get; init; }
}

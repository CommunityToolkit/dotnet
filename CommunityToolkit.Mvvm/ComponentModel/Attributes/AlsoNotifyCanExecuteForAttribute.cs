// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// An attribute that can be used to support <see cref="IRelayCommand"/> properties in generated properties. When this attribute is
/// used, the generated property setter will also call <see cref="IRelayCommand.NotifyCanExecuteChanged"/> for the properties specified
/// in the attribute data, causing the validation logic for the command to be executed again. This can be useful to keep the code compact
/// when there are one or more dependent commands that should also be notified when a property is updated. If this attribute is used in
/// a field without <see cref="ObservablePropertyAttribute"/>, it is ignored (just like <see cref="AlsoNotifyChangeForAttribute"/>).
/// <para>
/// In order to use this attribute, the target property has to implement the <see cref="IRelayCommand"/> interface.
/// </para>
/// <para>
/// This attribute can be used as follows:
/// <code>
/// partial class MyViewModel : ObservableObject
/// {
///     [ObservableProperty]
///     [AlsoNotifyCanExecuteFor(nameof(GreetUserCommand))]
///     private string name;
///
///     public IRelayCommand GreetUserCommand { get; }
/// }
/// </code>
/// </para>
/// And with this, code analogous to this will be generated:
/// <code>
/// partial class MyViewModel
/// {
///     public string Name
///     {
///         get => name;
///         set
///         {
///             if (SetProperty(ref name, value))
///             {
///                 GreetUserCommand.NotifyCanExecuteChanged();
///             }
///         }
///     }
/// }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
[Conditional("MVVMTOOLKIT_KEEP_SOURCE_GENERATOR_ATTRIBUTES")]
public sealed class AlsoNotifyCanExecuteForAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlsoNotifyCanExecuteForAttribute"/> class.
    /// </summary>
    /// <param name="commandName">The name of the command to also notify when the annotated property changes.</param>
    public AlsoNotifyCanExecuteForAttribute(string commandName)
    {
        CommandNames = new[] { commandName };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlsoNotifyCanExecuteForAttribute"/> class.
    /// </summary>
    /// <param name="commandName">The name of the property to also notify when the annotated property changes.</param>
    /// <param name="otherCommandNames">
    /// The other command names to also notify when the annotated property changes. This parameter can optionally
    /// be used to indicate a series of dependent commands from the same attribute, to keep the code more compact.
    /// </param>
    public AlsoNotifyCanExecuteForAttribute(string commandName, params string[] otherCommandNames)
    {
        CommandNames = new[] { commandName }.Concat(otherCommandNames).ToArray();
    }

    /// <summary>
    /// Gets the command names to also notify when the annotated property changes.
    /// </summary>
    public string[] CommandNames { get; }
}

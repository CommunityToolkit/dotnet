// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// An attribute that can be used to support <see cref="ObservablePropertyAttribute"/> in generated properties.
/// When this attribute is used, the generated property setter will be private.
/// This can be useful to prevent an unwanted property changes.
/// If this attribute is used in a field without <see cref="ObservablePropertyAttribute"/>, it is ignored.
/// <para>
/// This attribute can be used as follows:
/// <code>
/// partial class MyViewModel : ObservableObject
/// {
///     [ObservableProperty]
///     [WithPrivateSetter]
///     private string name;
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
///         private set => SetProperty(ref name, value);
///     }
/// }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class WithPrivateSetterAttribute : Attribute
{
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// An attribute that indicates that a given partial property should be implemented by the source generator.
/// In order to use this attribute, the containing type has to inherit from <see cref="ObservableObject"/>, or it
/// must be using <see cref="ObservableObjectAttribute"/> or <see cref="INotifyPropertyChangedAttribute"/>.
/// If the containing type also implements the <see cref="INotifyPropertyChanging"/> (that is, if it either inherits from
/// <see cref="ObservableObject"/> or is using <see cref="ObservableObjectAttribute"/>), then the generated code will
/// also invoke <see cref="ObservableObject.OnPropertyChanging(PropertyChangingEventArgs)"/> to signal that event.
/// <para>
/// This attribute can be used as follows:
/// <code>
/// partial class MyViewModel : ObservableObject
/// {
///     [ObservableProperty]
///     public partial string name { get; set; }
///
///     [ObservableProperty]
///     public partial bool IsEnabled { get; set; }
/// }
/// </code>
/// </para>
/// And with this, code analogous to this will be generated:
/// <code>
/// partial class MyViewModel
/// {
///     public partial string Name
///     {
///         get => field;
///         set => SetProperty(ref field, value);
///     }
///
///     public partial bool IsEnabled
///     {
///         get => field;
///         set => SetProperty(ref field, value);
///     }
/// }
/// </code>
/// </summary>
/// <remarks>
/// <para>
/// In order to use this attribute on partial properties, the .NET 9 SDK is required, and C# preview must
/// be used. If that is not available, this attribute can be used to annotate fields instead, like so:
/// <code>
/// partial class MyViewModel : ObservableObject
/// {
///     [ObservableProperty]
///     private string name;
///
///     [ObservableProperty]
///     private bool isEnabled;
/// }
/// </code>
/// </para>
/// <para>
/// The generated properties will automatically use the <c>UpperCamelCase</c> format for their names,
/// which will be derived from the field names. The generator can also recognize fields using either
/// the <c>_lowerCamel</c> or <c>m_lowerCamel</c> naming scheme. Otherwise, the first character in the
/// source field name will be converted to uppercase (eg. <c>isEnabled</c> to <c>IsEnabled</c>).
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ObservablePropertyAttribute : Attribute
{
}

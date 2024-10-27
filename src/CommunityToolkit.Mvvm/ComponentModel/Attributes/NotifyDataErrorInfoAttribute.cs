// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// An attribute that can be used to support <see cref="ObservablePropertyAttribute"/> in generated properties, when applied to
/// partial properties contained in a type that is inheriting from <see cref="ObservableValidator"/> and using any validation attributes.
/// When this attribute is used, the generated property setter will also call <see cref="ObservableValidator.ValidateProperty(object?, string)"/>.
/// This allows generated properties to opt-in into validation behavior without having to fallback into a full explicit observable property.
/// <para>
/// This attribute can be used as follows:
/// <code>
/// partial class MyViewModel : ObservableValidator
/// {
///     [ObservableProperty]
///     [NotifyDataErrorInfo]
///     [Required]
///     [MinLength(2)]
///     public partial string Username { get; set; }
/// }
/// </code>
/// </para>
/// And with this, code analogous to this will be generated:
/// <code>
/// partial class MyViewModel
/// {
///     public partial string Username
///     {
///         get => field;
///         set => SetProperty(ref field, value, validate: true);
///     }
/// }
/// </code>
/// </summary>
/// <remarks>
/// <para>
/// This attribute can also be used on a class, which will enable the validation on all generated properties contained in it.
/// </para>
/// <para>
/// Just like <see cref="ObservablePropertyAttribute"/>, this attribute can also be used on fields as well.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NotifyDataErrorInfoAttribute : Attribute
{
}

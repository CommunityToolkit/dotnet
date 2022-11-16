// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;

namespace CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// An attribute that can be used to support <see cref="ObservablePropertyAttribute"/> in generated properties, when applied to fields
/// contained in a type that is either inheriting from <see cref="ObservableRecipient"/>, or annotated with <see cref="ObservableRecipientAttribute"/>.
/// When this attribute is used, the generated property setter will also call <see cref="ObservableRecipient.Broadcast{T}(T, T, string?)"/>.
/// This allows generated properties to opt-in into broadcasting behavior without having to fallback into a full explicit observable property.
/// <para>
/// This attribute can be used as follows:
/// <code>
/// partial class MyViewModel : ObservableRecipient
/// {
///     [ObservableProperty]
///     [NotifyPropertyChangedRecipients]
///     private string username;
/// }
/// </code>
/// </para>
/// <para>
/// And with this, code analogous to this will be generated:
/// <code>
/// partial class MyViewModel
/// {
///     public string Username
///     {
///         get => username;
///         set => SetProperty(ref username, value, broadcast: true);
///     }
/// }
/// </code>
/// </para>
/// <para>
/// This attribute can also be added to a class, and if so it will affect all generated properties in that type and inherited types.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NotifyPropertyChangedRecipientsAttribute : Attribute
{
}

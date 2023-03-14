// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model representing an generated property
/// </summary>
/// <param name="TypeNameWithNullabilityAnnotations">The type name for the generated property, including nullability annotations.</param>
/// <param name="FieldName">The field name.</param>
/// <param name="PropertyName">The generated property name.</param>
/// <param name="PropertyChangingNames">The sequence of property changing properties to notify.</param>
/// <param name="PropertyChangedNames">The sequence of property changed properties to notify.</param>
/// <param name="NotifiedCommandNames">The sequence of commands to notify.</param>
/// <param name="NotifyPropertyChangedRecipients">Whether or not the generated property also broadcasts changes.</param>
/// <param name="NotifyDataErrorInfo">Whether or not the generated property also validates its value.</param>
/// <param name="IsOldPropertyValueDirectlyReferenced">Whether the old property value is being directly referenced.</param>
/// <param name="IsReferenceTypeOrUnconstraindTypeParameter">Indicates whether the property is of a reference type or an unconstrained type parameter.</param>
/// <param name="IncludeMemberNotNullOnSetAccessor">Indicates whether to include nullability annotations on the setter.</param>
/// <param name="ForwardedAttributes">The sequence of forwarded attributes for the generated property.</param>
internal sealed record PropertyInfo(
    string TypeNameWithNullabilityAnnotations,
    string FieldName,
    string PropertyName,
    EquatableArray<string> PropertyChangingNames,
    EquatableArray<string> PropertyChangedNames,
    EquatableArray<string> NotifiedCommandNames,
    bool NotifyPropertyChangedRecipients,
    bool NotifyDataErrorInfo,
    bool IsOldPropertyValueDirectlyReferenced,
    bool IsReferenceTypeOrUnconstraindTypeParameter,
    bool IncludeMemberNotNullOnSetAccessor,
    EquatableArray<AttributeInfo> ForwardedAttributes);

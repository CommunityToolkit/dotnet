// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.Mvvm.SourceGenerators.Extensions;
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
/// <param name="ForwardedAttributes">The sequence of forwarded attributes for the generated property.</param>
internal sealed record PropertyInfo(
    string TypeNameWithNullabilityAnnotations,
    string FieldName,
    string PropertyName,
    ImmutableArray<string> PropertyChangingNames,
    ImmutableArray<string> PropertyChangedNames,
    ImmutableArray<string> NotifiedCommandNames,
    bool NotifyPropertyChangedRecipients,
    bool NotifyDataErrorInfo,
    ImmutableArray<AttributeInfo> ForwardedAttributes)
{
    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> implementation for <see cref="PropertyInfo"/>.
    /// </summary>
    public sealed class Comparer : Comparer<PropertyInfo, Comparer>
    {
        /// <inheritdoc/>
        protected override void AddToHashCode(ref HashCode hashCode, PropertyInfo obj)
        {
            hashCode.Add(obj.TypeNameWithNullabilityAnnotations);
            hashCode.Add(obj.FieldName);
            hashCode.Add(obj.PropertyName);
            hashCode.AddRange(obj.PropertyChangingNames);
            hashCode.AddRange(obj.PropertyChangedNames);
            hashCode.AddRange(obj.NotifiedCommandNames);
            hashCode.Add(obj.NotifyPropertyChangedRecipients);
            hashCode.Add(obj.NotifyDataErrorInfo);
            hashCode.AddRange(obj.ForwardedAttributes, AttributeInfo.Comparer.Default);
        }

        /// <inheritdoc/>
        protected override bool AreEqual(PropertyInfo x, PropertyInfo y)
        {
            return
                x.TypeNameWithNullabilityAnnotations == y.TypeNameWithNullabilityAnnotations &&
                x.FieldName == y.FieldName &&
                x.PropertyName == y.PropertyName &&
                x.PropertyChangingNames.SequenceEqual(y.PropertyChangingNames) &&
                x.PropertyChangedNames.SequenceEqual(y.PropertyChangedNames) &&
                x.NotifiedCommandNames.SequenceEqual(y.NotifiedCommandNames) &&
                x.NotifyPropertyChangedRecipients == y.NotifyPropertyChangedRecipients &&
                x.NotifyDataErrorInfo == y.NotifyDataErrorInfo &&
                x.ForwardedAttributes.SequenceEqual(y.ForwardedAttributes, AttributeInfo.Comparer.Default);
        }
    }
}

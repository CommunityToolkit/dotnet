// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model representing generated child <c>PropertyChanged</c> forwarding for an observable property.
/// </summary>
/// <param name="PropertyChangedNames">The dependent property names to notify when the child instance changes.</param>
/// <param name="NotifiedCommandNames">The dependent command names to notify when the child instance changes.</param>
internal sealed record ChildPropertyChangedSubscriptionInfo(
    EquatableArray<string> PropertyChangedNames,
    EquatableArray<string> NotifiedCommandNames);

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model with gathered info on a locally declared validatable property.
/// </summary>
/// <param name="PropertyName">The name of the property to validate.</param>
internal sealed record PropertyValidationInfo(string PropertyName);

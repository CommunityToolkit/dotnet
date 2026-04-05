// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;
using CommunityToolkit.Mvvm.SourceGenerators.Models;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model with gathered info on all validatable properties in a given type.
/// </summary>
/// <param name="Hierarchy">The hierarchy for the current type.</param>
/// <param name="TypeName">The fully qualified type name of the target type.</param>
/// <param name="Properties">The locally declared validatable properties for the current type.</param>
internal sealed record ValidationInfo(HierarchyInfo Hierarchy, string TypeName, EquatableArray<PropertyValidationInfo> Properties);

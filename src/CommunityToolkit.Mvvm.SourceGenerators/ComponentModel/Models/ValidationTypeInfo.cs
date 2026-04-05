// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.SourceGenerators.Models;

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model with identity information for a type requiring generated validation hooks.
/// </summary>
/// <param name="Hierarchy">The hierarchy for the target type.</param>
/// <param name="TypeName">The fully qualified type name for the target type.</param>
internal sealed record ValidationTypeInfo(HierarchyInfo Hierarchy, string TypeName);

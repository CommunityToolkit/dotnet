// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Mvvm.SourceGenerators.Input.Models;

/// <summary>
/// A model with gathered info on a given <c>INotifyPropertyChangedAttribute</c> instance.
/// </summary>
/// <param name="IncludeAdditionalHelperMethods">Whether to also generate helper methods in the target type.</param>
/// <param name="IsSealed">Whether the target type is sealed.</param>
public sealed record INotifyPropertyChangedInfo(bool IncludeAdditionalHelperMethods, bool IsSealed);

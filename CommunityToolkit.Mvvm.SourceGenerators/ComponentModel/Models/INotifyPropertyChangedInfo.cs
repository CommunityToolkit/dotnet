// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model with gathered info on a given <c>INotifyPropertyChangedAttribute</c> instance.
/// </summary>
/// <param name="IncludeAdditionalHelperMethods">Whether to also generate helper methods in the target type.</param>
public sealed record INotifyPropertyChangedInfo(bool IncludeAdditionalHelperMethods);

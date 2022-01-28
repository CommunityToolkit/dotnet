// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CommunityToolkit.Mvvm.ExternalAssembly;

/// <summary>
/// Test viewmodel for https://github.com/CommunityToolkit/dotnet/issues/222.
/// </summary>
[ObservableObject]
public partial class ModelWithObservableObjectAttribute
{
    [ObservableProperty]
    private string? _myProperty;
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

#pragma warning disable MVVMTK0033

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
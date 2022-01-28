// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

// Test viewmodel for https://github.com/CommunityToolkit/dotnet/issues/271
internal partial class ModelWithObservablePropertyInRootNamespace : ObservableObject
{
    [ObservableProperty]
    private float number;
}
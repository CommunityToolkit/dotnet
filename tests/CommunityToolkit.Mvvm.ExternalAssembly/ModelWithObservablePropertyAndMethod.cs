// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CommunityToolkit.Mvvm.ExternalAssembly;

/// <summary>
/// Test viewmodel for https://github.com/CommunityToolkit/dotnet/issues/222.
/// </summary>
public abstract partial class ModelWithObservablePropertyAndMethod : ObservableObject
{
    [ObservableProperty]
    private bool canSave;

    /// <summary>
    /// Base method to then generate a command.
    /// </summary>
    public abstract void Save();
}

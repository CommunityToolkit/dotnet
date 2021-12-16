// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model with gathered info on a given <c>ObservableRecipientAttribute</c> instance.
/// </summary>
/// <param name="TypeName">The type name of the type being annotated.</param>
/// <param name="HasExplicitConstructors">Whether or not the target type has explicit constructors.</param>
/// <param name="IsAbstract">Whether or not the target type is abstract.</param>
/// <param name="IsObservableValidator">Whether or not the target type inherits from <c>ObservableValidator</c>.</param>
public sealed record ObservableRecipientInfo(
    string TypeName,
    bool HasExplicitConstructors,
    bool IsAbstract,
    bool IsObservableValidator);

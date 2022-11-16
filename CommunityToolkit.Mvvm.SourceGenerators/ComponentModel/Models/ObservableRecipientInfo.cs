// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

namespace CommunityToolkit.Mvvm.SourceGenerators.ComponentModel.Models;

/// <summary>
/// A model with gathered info on a given <c>ObservableRecipientAttribute</c> instance.
/// </summary>
/// <param name="TypeName">The type name of the type being annotated.</param>
/// <param name="HasExplicitConstructors">Whether or not the target type has explicit constructors.</param>
/// <param name="IsAbstract">Whether or not the target type is abstract.</param>
/// <param name="IsObservableValidator">Whether or not the target type inherits from <c>ObservableValidator</c>.</param>
/// <param name="IsRequiresUnreferencedCodeAttributeAvailable">Whether or not the <c>RequiresUnreferencedCodeAttribute</c> type is available.</param>
/// <param name="HasOnActivatedMethod">Whether the target type has a custom <c>OnActivated()</c> method.</param>
/// <param name="HasOnDeactivatedMethod">Whether the target type has a custom <c>OnDeactivated()</c> method.</param>
public sealed record ObservableRecipientInfo(
    string TypeName,
    bool HasExplicitConstructors,
    bool IsAbstract,
    bool IsObservableValidator,
    bool IsRequiresUnreferencedCodeAttributeAvailable,
    bool HasOnActivatedMethod,
    bool HasOnDeactivatedMethod);

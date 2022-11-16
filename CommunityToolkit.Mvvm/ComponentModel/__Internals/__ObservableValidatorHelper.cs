// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommunityToolkit.Mvvm.ComponentModel.__Internals;

/// <summary>
/// An internal helper to support the source generator APIs related to <see cref="ObservableValidator"/>.
/// This type is not intended to be used directly by user code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete("This type is not intended to be used directly by user code")]
public static class __ObservableValidatorHelper
{
    /// <summary>
    /// Invokes <see cref="ObservableValidator.ValidateProperty(object?, string?)"/> externally on a target instance.
    /// </summary>
    /// <param name="instance">The target <see cref="ObservableValidator"/> instance.</param>
    /// <param name="value">The value to test for the specified property.</param>
    /// <param name="propertyName">The name of the property to validate.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This method is not intended to be called directly by user code")]
    [UnconditionalSuppressMessage(
        "ReflectionAnalysis",
        "IL2026:RequiresUnreferencedCode",
        Justification = "This helper is called by generated code from public APIs that have the proper annotations already (and we don't want generated code to produce warnings that developers cannot fix).")]
    public static void ValidateProperty(ObservableValidator instance, object? value, string propertyName)
    {
        instance.ValidateProperty(value, propertyName);
    }
}

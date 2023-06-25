// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// A container for all shared <see cref="AppContext"/> configuration switches for the MVVM Toolkit.
/// </summary>
/// <remarks>
/// <para>
/// This type uses a very specific setup for configuration switches to ensure ILLink can work the best.
/// This mirrors the architecture of feature switches in the runtime as well, and it's needed so that
/// no static constructor is generated for the type.
/// </para>
/// <para>
/// For more info, see <see href="https://github.com/dotnet/runtime/blob/main/docs/workflow/trimming/feature-switches.md#adding-new-feature-switch"/>.
/// </para>
/// </remarks>
internal static class Configuration
{
    /// <summary>
    /// The configuration property name for <see cref="IsINotifyPropertyChangingDisabled"/>.
    /// </summary>
    private const string IsINotifyPropertyChangingDisabledPropertyName = "MVVMTOOLKIT_DISABLE_INOTIFYPROPERTYCHANGING";

    /// <summary>
    /// The backing field for <see cref="IsINotifyPropertyChangingDisabled"/>.
    /// </summary>
    private static int isINotifyPropertyChangingDisabledConfigurationValue;

    /// <summary>
    /// Indicates whether or not support for <see cref="INotifyPropertyChanging"/> is disabled.
    /// </summary>
    public static bool IsINotifyPropertyChangingDisabled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetConfigurationValue(IsINotifyPropertyChangingDisabledPropertyName, ref isINotifyPropertyChangingDisabledConfigurationValue);
    }

    /// <summary>
    /// Gets a configuration value for a specified property.
    /// </summary>
    /// <param name="propertyName">The property name to retrieve the value for.</param>
    /// <param name="cachedResult">The cached result for the target configuration value.</param>
    /// <returns>The value of the specified configuration setting.</returns>
    private static bool GetConfigurationValue(string propertyName, ref int cachedResult)
    {
        // The cached switch value has 3 states:
        //   0: unknown.
        //   1: true
        //   -1: false
        //
        // This method doesn't need to worry about concurrent accesses to the cached result,
        // as even if the configuration value is retrieved twice, that'll always be the same.
        if (cachedResult < 0)
        {
            return false;
        }

        if (cachedResult > 0)
        {
            return true;
        }

        // Get the configuration switch value, or its default
        if (!AppContext.TryGetSwitch(propertyName, out bool isEnabled))
        {
            isEnabled = GetDefaultConfigurationValue(propertyName);
        }

        // Update the cached result
        cachedResult = isEnabled ? 1 : -1;

        return isEnabled;
    }

    /// <summary>
    /// Gets the default configuration value for a given feature switch.
    /// </summary>
    /// <param name="propertyName">The property name to retrieve the value for.</param>
    /// <returns>The default value for the target <paramref name="propertyName"/>.</returns>
    private static bool GetDefaultConfigurationValue(string propertyName)
    {
        // Disable INotifyPropertyChanging support (defaults to false)
        if (propertyName == IsINotifyPropertyChangingDisabledPropertyName)
        {
            return false;
        }

        return false;
    }
}

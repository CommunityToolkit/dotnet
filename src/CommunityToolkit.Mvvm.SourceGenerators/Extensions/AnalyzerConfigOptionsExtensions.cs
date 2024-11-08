// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.Mvvm.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="AnalyzerConfigOptions"/> type.
/// </summary>
internal static class AnalyzerConfigOptionsExtensions
{
    /// <summary>
    /// Checks whether the Windows runtime pack is being used (ie. if the target framework is <c>net8.0-windows10.0.17763.0</c> or above).
    /// </summary>
    /// <param name="options">The input <see cref="AnalyzerConfigOptions"/> instance.</param>
    /// <returns>Whether the Windows runtime pack is being used.</returns>
    public static bool IsUsingWindowsRuntimePack(this AnalyzerConfigOptions options)
    {
        return options.GetMSBuildBooleanPropertyValue("_MvvmToolkitIsUsingWindowsRuntimePack");
    }

    /// <summary>
    /// Checks whether CsWinRT is configured in AOT support mode.
    /// </summary>
    /// <param name="options">The input <see cref="AnalyzerConfigOptions"/> instance.</param>
    /// <param name="compilation">The input <see cref="Compilation"/> instance in use.</param>
    /// <returns>Whether CsWinRT is configured in AOT support mode.</returns>
    public static bool IsCsWinRTAotOptimizerEnabled(this AnalyzerConfigOptions options, Compilation compilation)
    {
        // If the runtime pack isn't being used, CsWinRT won't be used either. Technically speaking it's possible
        // to reference CsWinRT without targeting Windows, but that's not a scenario that is supported anyway.
        if (!options.IsUsingWindowsRuntimePack())
        {
            return false;
        }

        if (options.TryGetMSBuildStringPropertyValue("CsWinRTAotOptimizerEnabled", out string? csWinRTAotOptimizerEnabled))
        {
            // If the generators are in opt-in mode, we will not show warnings
            if (string.Equals(csWinRTAotOptimizerEnabled, "OptIn", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // The automatic mode will generate marshalling code for all possible scenarios
            if (string.Equals(csWinRTAotOptimizerEnabled, "Auto", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // The default value of "true" will run in automatic mode for some scenarios, which we have to check
            if (bool.TryParse(csWinRTAotOptimizerEnabled, out bool isCsWinRTAotOptimizerEnabled) && isCsWinRTAotOptimizerEnabled)
            {
                // The CsWinRT generator will be enabled for AOT scenarios in the following cases:
                //   - The project is producing a WinRT component
                //   - The 'CsWinRTAotWarningLevel' is set to '2', ie. all marshalling code even for built-in types should be produced
                //   - The app is either UWP XAML or WinUI 3 (which is detected by the presence of the 'Button' type
                // For additional reference, see the source code at https://github.com/microsoft/CsWinRT.
                return
                    options.GetMSBuildBooleanPropertyValue("CsWinRTComponent") ||
                    options.GetMSBuildInt32PropertyValue("CsWinRTAotWarningLevel") == 2 ||
                    compilation.GetTypeByMetadataName("Microsoft.UI.Xaml.Controls.Button") is not null ||
                    compilation.GetTypeByMetadataName("Windows.UI.Xaml.Controls.Button") is not null;
                       
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the boolean value of a given MSBuild property from an input <see cref="AnalyzerConfigOptions"/> instance.
    /// </summary>
    /// <param name="options">The input <see cref="AnalyzerConfigOptions"/> instance.</param>
    /// <param name="propertyName">The name of the target MSBuild property.</param>
    /// <param name="defaultValue">The default value to return if the property is not found or cannot be parsed.</param>
    /// <returns>The value of the target MSBuild property.</returns>
    public static bool GetMSBuildBooleanPropertyValue(this AnalyzerConfigOptions options, string propertyName, bool defaultValue = false)
    {
        if (options.TryGetMSBuildStringPropertyValue(propertyName, out string? propertyValue))
        {
            if (bool.TryParse(propertyValue, out bool booleanPropertyValue))
            {
                return booleanPropertyValue;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets the integer value of a given MSBuild property from an input <see cref="AnalyzerConfigOptions"/> instance.
    /// </summary>
    /// <param name="options">The input <see cref="AnalyzerConfigOptions"/> instance.</param>
    /// <param name="propertyName">The name of the target MSBuild property.</param>
    /// <param name="defaultValue">The default value to return if the property is not found or cannot be parsed.</param>
    /// <returns>The value of the target MSBuild property.</returns>
    public static int GetMSBuildInt32PropertyValue(this AnalyzerConfigOptions options, string propertyName, int defaultValue = 0)
    {
        if (options.TryGetMSBuildStringPropertyValue(propertyName, out string? propertyValue))
        {
            if (int.TryParse(propertyValue, out int int32PropertyValue))
            {
                return int32PropertyValue;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Tries to get a <see cref="string"/> value of a given MSBuild property from an input <see cref="AnalyzerConfigOptions"/> instance.
    /// </summary>
    /// <param name="options">The input <see cref="AnalyzerConfigOptions"/> instance.</param>
    /// <param name="propertyName">The name of the target MSBuild property.</param>
    /// <param name="propertyValue">The resulting property value.</param>
    /// <returns>Whether the property value was retrieved..</returns>
    public static bool TryGetMSBuildStringPropertyValue(this AnalyzerConfigOptions options, string propertyName, [NotNullWhen(true)] out string? propertyValue)
    {
        return options.TryGetValue($"build_property.{propertyName}", out propertyValue);
    }
}

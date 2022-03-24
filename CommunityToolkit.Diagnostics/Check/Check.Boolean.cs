// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
public static partial class Check
{
    /// <summary>
    /// Checks that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTrue(bool value)
    {
        return value;
    }

    /// <summary>
    /// Checks that the input value must be <see langword="false"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFalse(bool value)
    {
        return !value;
    }
}

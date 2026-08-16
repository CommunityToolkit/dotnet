// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
public static partial class Guard
{
    /// <summary>
    /// Asserts that the input value is defined in the specified enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The type of enumeration to validate.</typeparam>
    /// <param name="value">The input <typeparamref name="TEnum"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not defined in <typeparamref name="TEnum"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsDefined<TEnum>(TEnum value, [CallerArgumentExpression(nameof(value))] string name = "")
        where TEnum : struct, Enum
    {
        if (EnumCache<TEnum>.Values.Contains(value))
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForEnumNotDefined(value, name);
    }
}

/// <summary>
/// Provides a cached set of all defined values for a specific enumeration type.
/// </summary>
/// <typeparam name="TEnum">The type of enumeration whose defined values are cached.</typeparam>
internal static class EnumCache<TEnum> where TEnum : struct, Enum
{
    static EnumCache()
    {
#if NET5_0_OR_GREATER
        TEnum[] values = Enum.GetValues<TEnum>();
#else
        TEnum[] values = (TEnum[])Enum.GetValues(typeof(TEnum));
#endif
        Values = values.ToFrozenSet();
    }

    public static readonly FrozenSet<TEnum> Values;
}

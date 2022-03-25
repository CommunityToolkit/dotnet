// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Check
{
    /// <summary>
    /// Checks that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="target">The target <see cref="int"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is less than or equal to <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCloseTo(int value, int target, uint delta)
    {
        uint difference;

        if (value >= target)
        {
            difference = (uint)(value - target);
        }
        else
        {
            difference = (uint)(target - value);
        }

        return difference <= delta;
    }

    /// <summary>
    /// Checks that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="target">The target <see cref="int"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is more than <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotCloseTo(int value, int target, uint delta)
    {
        uint difference;

        if (value >= target)
        {
            difference = (uint)(value - target);
        }
        else
        {
            difference = (uint)(target - value);
        }

        return difference > delta;
    }

    /// <summary>
    /// Checks that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="target">The target <see cref="long"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is less than or equal to <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCloseTo(long value, long target, ulong delta)
    {
        ulong difference;

        if (value >= target)
        {
            difference = (ulong)(value - target);
        }
        else
        {
            difference = (ulong)(target - value);
        }

        return difference <= delta;
    }

    /// <summary>
    /// Checks that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="target">The target <see cref="long"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is more than <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotCloseTo(long value, long target, ulong delta)
    {
        ulong difference;

        if (value >= target)
        {
            difference = (ulong)(value - target);
        }
        else
        {
            difference = (ulong)(target - value);
        }

        return difference > delta;
    }

    /// <summary>
    /// Checks that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="target">The target <see cref="float"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is less than or equal to <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCloseTo(float value, float target, float delta)
    {
        return Math.Abs(value - target) <= delta;
    }

    /// <summary>
    /// Checks that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="target">The target <see cref="float"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is more than <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotCloseTo(float value, float target, float delta)
    {
        return Math.Abs(value - target) > delta;
    }

    /// <summary>
    /// Checks that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="target">The target <see cref="double"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is less than or equal to <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCloseTo(double value, double target, double delta)
    {
        return Math.Abs(value - target) <= delta;
    }

    /// <summary>
    /// Checks that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="target">The target <see cref="double"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is more than <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotCloseTo(double value, double target, double delta)
    {
        return Math.Abs(value - target) > delta;
    }

    /// <summary>
    /// Checks that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="target">The target <see langword="nint"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is less than or equal to <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCloseTo(nint value, nint target, nuint delta)
    {
        nuint difference;

        if (value >= target)
        {
            difference = (nuint)(value - target);
        }
        else
        {
            difference = (nuint)(target - value);
        }

        return difference <= delta;
    }

    /// <summary>
    /// Checks that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="target">The target <see langword="nint"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is more than <paramref name="delta"/> away from <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotCloseTo(nint value, nint target, nuint delta)
    {
        nuint difference;

        if (value >= target)
        {
            difference = (nuint)(value - target);
        }
        else
        {
            difference = (nuint)(target - value);
        }

        return difference > delta;
    }
}

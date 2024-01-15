// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class FGuard
{
    /// <summary>
    /// Asserts that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="target">The target <see cref="int"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by no more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IsCloseTo(int value, int target, uint delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="target">The target <see cref="int"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IsNotCloseTo(int value, int target, uint delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="target">The target <see cref="long"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by no more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long IsCloseTo(long value, long target, ulong delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="target">The target <see cref="long"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long IsNotCloseTo(long value, long target, ulong delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="target">The target <see cref="float"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by no more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float IsCloseTo(float value, float target, float delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="target">The target <see cref="float"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float IsNotCloseTo(float value, float target, float delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="target">The target <see cref="double"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by no more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double IsCloseTo(double value, double target, double delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="target">The target <see cref="double"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double IsNotCloseTo(double value, double target, double delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="target">The target <see langword="nint"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by no more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) > <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint IsCloseTo(nint value, nint target, nuint delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsCloseTo(value, target, delta, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be within a given distance from a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="target">The target <see langword="nint"/> value to test for.</param>
    /// <param name="delta">The maximum distance to allow between <paramref name="value"/> and <paramref name="target"/>.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> which differs from <paramref name="target"/> by more than the specified <paramref name="delta"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if (<paramref name="value"/> - <paramref name="target"/>) &lt;= <paramref name="delta"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint IsNotCloseTo(nint value, nint target, nuint delta, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotCloseTo(value, target, delta, name);
        return value;
    }
}

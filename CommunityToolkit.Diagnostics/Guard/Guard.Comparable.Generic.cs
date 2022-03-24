// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <summary>
    /// Asserts that the input value is <see langword="default"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not <see langword="default"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsDefault<T>(T value, [CallerArgumentExpression("value")] string name = "")
        where T : struct, IEquatable<T>
    {
        if (Check.IsDefault(value))
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsDefault(value, name);
    }

    /// <summary>
    /// Asserts that the input value is not <see langword="default"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="default"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsNotDefault<T>(T value, [CallerArgumentExpression("value")] string name = "")
        where T : struct, IEquatable<T>
    {
        if (Check.IsNotDefault(value))
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsNotDefault<T>(name);
    }

    /// <summary>
    /// Asserts that the input value must be equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsEqualTo<T>(T value, T target, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IEquatable<T>
    {
        if (Check.IsEqualTo(value, target))
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsEqualTo(value, target, name);
    }

    /// <summary>
    /// Asserts that the input value must be not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsNotEqualTo<T>(T value, T target, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IEquatable<T>
    {
        if (Check.IsNotEqualTo(value, target))
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsNotEqualTo(value, target, name);
    }

    /// <summary>
    /// Asserts that the input value must be a bitwise match with a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not a bitwise match for <paramref name="target"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void IsBitwiseEqualTo<T>(T value, T target, [CallerArgumentExpression("value")] string name = "")
        where T : unmanaged
    {
        if (Check.IsBitwiseEqualTo(value, target))
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForBitwiseEqualTo(value, target, name);
    }

    /// <summary>
    /// Asserts that the input value must be less than a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsLessThan<T>(T value, T maximum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsLessThan(value, maximum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThan(value, maximum, name);
    }

    /// <summary>
    /// Asserts that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsLessThanOrEqualTo<T>(T value, T maximum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsLessThanOrEqualTo(value, maximum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsLessThanOrEqualTo(value, maximum, name);
    }

    /// <summary>
    /// Asserts that the input value must be greater than a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsGreaterThan<T>(T value, T minimum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsGreaterThan(value, minimum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThan(value, minimum, name);
    }

    /// <summary>
    /// Asserts that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsGreaterThanOrEqualTo<T>(T value, T minimum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsGreaterThanOrEqualTo(value, minimum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsGreaterThanOrEqualTo(value, minimum, name);
    }

    /// <summary>
    /// Asserts that the input value must be in a given range.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsInRange<T>(T value, T minimum, T maximum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsInRange(value, minimum, maximum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsInRange(value, minimum, maximum, name);
    }

    /// <summary>
    /// Asserts that the input value must not be in a given range.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsNotInRange<T>(T value, T minimum, T maximum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsNotInRange(value, minimum, maximum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotInRange(value, minimum, maximum, name);
    }

    /// <summary>
    /// Asserts that the input value must be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsBetween<T>(T value, T minimum, T maximum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsBetween(value, minimum, maximum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetween(value, minimum, maximum, name);
    }

    /// <summary>
    /// Asserts that the input value must not be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> or &lt; <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsNotBetween<T>(T value, T minimum, T maximum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsNotBetween(value, minimum, maximum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetween(value, minimum, maximum, name);
    }

    /// <summary>
    /// Asserts that the input value must be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsBetweenOrEqualTo<T>(T value, T minimum, T maximum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsBetweenOrEqualTo(value, minimum, maximum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsBetweenOrEqualTo(value, minimum, maximum, name);
    }

    /// <summary>
    /// Asserts that the input value must not be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> or &lt;= <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsNotBetweenOrEqualTo<T>(T value, T minimum, T maximum, [CallerArgumentExpression("value")] string name = "")
        where T : notnull, IComparable<T>
    {
        if (Check.IsNotBetweenOrEqualTo(value, minimum, maximum))
        {
            return;
        }

        ThrowHelper.ThrowArgumentOutOfRangeExceptionForIsNotBetweenOrEqualTo(value, minimum, maximum, name);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class FGuard
{
    /// <summary>
    /// Asserts that the input value is <see langword="default"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is <see langword="default"/>(<typeparamref name="T"/>).</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not <see langword="default"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsDefault<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : struct, IEquatable<T>
    {
        Guard.IsDefault(value, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value is not <see langword="default"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is not <see langword="default"/>(<typeparamref name="T"/>).</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="default"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsNotDefault<T>(T value, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : struct, IEquatable<T>
    {
        Guard.IsNotDefault(value, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is equal to the <paramref name="target"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is != <paramref name="target"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    /// <seealso cref="IEquatable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsEqualTo<T>(T value, T target, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IEquatable<T>
    {
        Guard.IsEqualTo(value, target, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is not equal to the <paramref name="target"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is == <paramref name="target"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    /// <seealso cref="IEquatable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsNotEqualTo<T>(T value, T target, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IEquatable<T>
    {
        Guard.IsNotEqualTo(value, target, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be a bitwise match with a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is bitwise match to the <paramref name="target"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not a bitwise match for <paramref name="target"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T IsBitwiseEqualTo<T>(T value, T target, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : unmanaged
    {
        Guard.IsBitwiseEqualTo(value, target, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be less than a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that less than the <paramref name="maximum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="maximum"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsLessThan<T>(T value, T maximum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsLessThan(value, maximum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that less than or equal to the <paramref name="maximum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="maximum"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsLessThanOrEqualTo<T>(T value, T maximum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsLessThanOrEqualTo(value, maximum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be greater than a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that greater than the <paramref name="minimum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsGreaterThan<T>(T value, T minimum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsGreaterThan(value, minimum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that greater than or equal to the <paramref name="minimum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/>.</exception>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsGreaterThanOrEqualTo<T>(T value, T minimum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsGreaterThanOrEqualTo(value, minimum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be in a given range.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is >= <paramref name="minimum"/> and &lt; the <paramref name="maximum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsInRange<T>(T value, T minimum, T maximum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsInRange(value, minimum, maximum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be in a given range.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is &lt; <paramref name="minimum"/> or >= the <paramref name="maximum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> and &lt; <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsNotInRange<T>(T value, T minimum, T maximum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsNotInRange(value, minimum, maximum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is > the <paramref name="minimum"/> and &lt; the <paramref name="maximum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt;= <paramref name="minimum"/> or >= <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsBetween<T>(T value, T minimum, T maximum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsBetween(value, minimum, maximum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is &lt;= the <paramref name="minimum"/> or >= the <paramref name="maximum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is > <paramref name="minimum"/> and &lt; <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsNotBetween<T>(T value, T minimum, T maximum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsNotBetween(value, minimum, maximum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is >= the <paramref name="minimum"/> and &lt;= the <paramref name="maximum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is &lt; <paramref name="minimum"/> or > <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsBetweenOrEqualTo<T>(T value, T minimum, T maximum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsBetweenOrEqualTo(value, minimum, maximum, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is &lt; the <paramref name="minimum"/> or > the <paramref name="maximum"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is >= <paramref name="minimum"/> and &lt;= <paramref name="maximum"/>.</exception>
    /// <remarks>
    /// This API asserts the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    /// <seealso cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsNotBetweenOrEqualTo<T>(T value, T minimum, T maximum, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : notnull, IComparable<T>
    {
        Guard.IsNotBetweenOrEqualTo(value, minimum, maximum, name);
        return value;
    }
}

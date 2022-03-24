// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// =====================
// Auto generated file
// =====================

using System;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Check
{
    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="target">The target <see cref="byte"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(byte value, byte target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="target">The target <see cref="byte"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(byte value, byte target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(byte value, byte maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(byte value, byte maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(byte value, byte minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(byte value, byte minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(byte value, byte minimum, byte maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(byte value, byte minimum, byte maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="byte"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(byte value, byte minimum, byte maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="byte"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(byte value, byte minimum, byte maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(byte value, byte minimum, byte maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="byte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="byte"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="byte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(byte value, byte minimum, byte maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="target">The target <see cref="sbyte"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(sbyte value, sbyte target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="target">The target <see cref="sbyte"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(sbyte value, sbyte target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(sbyte value, sbyte maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(sbyte value, sbyte maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(sbyte value, sbyte minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(sbyte value, sbyte minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(sbyte value, sbyte minimum, sbyte maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(sbyte value, sbyte minimum, sbyte maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="sbyte"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(sbyte value, sbyte minimum, sbyte maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="sbyte"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(sbyte value, sbyte minimum, sbyte maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(sbyte value, sbyte minimum, sbyte maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="sbyte"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="sbyte"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="sbyte"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(sbyte value, sbyte minimum, sbyte maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="target">The target <see cref="short"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(short value, short target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="target">The target <see cref="short"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(short value, short target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(short value, short maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="short"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(short value, short maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="short"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(short value, short minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(short value, short minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(short value, short minimum, short maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(short value, short minimum, short maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="short"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(short value, short minimum, short maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="short"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="short"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(short value, short minimum, short maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="short"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(short value, short minimum, short maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="short"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="short"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="short"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(short value, short minimum, short maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="target">The target <see cref="ushort"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(ushort value, ushort target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="target">The target <see cref="ushort"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(ushort value, ushort target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(ushort value, ushort maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(ushort value, ushort maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(ushort value, ushort minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(ushort value, ushort minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(ushort value, ushort minimum, ushort maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(ushort value, ushort minimum, ushort maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="ushort"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(ushort value, ushort minimum, ushort maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="ushort"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(ushort value, ushort minimum, ushort maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(ushort value, ushort minimum, ushort maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="ushort"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ushort"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="ushort"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(ushort value, ushort minimum, ushort maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="target">The target <see cref="char"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(char value, char target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="target">The target <see cref="char"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(char value, char target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(char value, char maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="char"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(char value, char maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="char"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(char value, char minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(char value, char minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(char value, char minimum, char maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(char value, char minimum, char maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="char"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(char value, char minimum, char maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="char"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="char"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(char value, char minimum, char maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="char"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(char value, char minimum, char maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="char"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="char"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="char"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(char value, char minimum, char maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="target">The target <see cref="int"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(int value, int target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="target">The target <see cref="int"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(int value, int target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(int value, int maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="int"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(int value, int maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="int"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(int value, int minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(int value, int minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(int value, int minimum, int maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(int value, int minimum, int maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="int"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(int value, int minimum, int maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="int"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="int"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(int value, int minimum, int maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="int"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(int value, int minimum, int maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="int"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="int"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="int"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(int value, int minimum, int maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="target">The target <see cref="uint"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(uint value, uint target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="target">The target <see cref="uint"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(uint value, uint target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(uint value, uint maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(uint value, uint maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(uint value, uint minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(uint value, uint minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(uint value, uint minimum, uint maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(uint value, uint minimum, uint maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="uint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(uint value, uint minimum, uint maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="uint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(uint value, uint minimum, uint maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(uint value, uint minimum, uint maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="uint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="uint"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="uint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(uint value, uint minimum, uint maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="target">The target <see cref="float"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(float value, float target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="target">The target <see cref="float"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(float value, float target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(float value, float maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="float"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(float value, float maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="float"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(float value, float minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(float value, float minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(float value, float minimum, float maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(float value, float minimum, float maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="float"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(float value, float minimum, float maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="float"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="float"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(float value, float minimum, float maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="float"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(float value, float minimum, float maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="float"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="float"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="float"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(float value, float minimum, float maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="target">The target <see cref="long"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(long value, long target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="target">The target <see cref="long"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(long value, long target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(long value, long maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="long"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(long value, long maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="long"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(long value, long minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(long value, long minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(long value, long minimum, long maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(long value, long minimum, long maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="long"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(long value, long minimum, long maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="long"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="long"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(long value, long minimum, long maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="long"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(long value, long minimum, long maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="long"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="long"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="long"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(long value, long minimum, long maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="target">The target <see cref="ulong"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(ulong value, ulong target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="target">The target <see cref="ulong"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(ulong value, ulong target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(ulong value, ulong maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(ulong value, ulong maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(ulong value, ulong minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(ulong value, ulong minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(ulong value, ulong minimum, ulong maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(ulong value, ulong minimum, ulong maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="ulong"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(ulong value, ulong minimum, ulong maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="ulong"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(ulong value, ulong minimum, ulong maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(ulong value, ulong minimum, ulong maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="ulong"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="ulong"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="ulong"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(ulong value, ulong minimum, ulong maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="target">The target <see cref="double"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(double value, double target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="target">The target <see cref="double"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(double value, double target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(double value, double maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="double"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(double value, double maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="double"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(double value, double minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(double value, double minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(double value, double minimum, double maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(double value, double minimum, double maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="double"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(double value, double minimum, double maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="double"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="double"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(double value, double minimum, double maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="double"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(double value, double minimum, double maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="double"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="double"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="double"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(double value, double minimum, double maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="target">The target <see cref="decimal"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(decimal value, decimal target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="target">The target <see cref="decimal"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(decimal value, decimal target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(decimal value, decimal maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(decimal value, decimal maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(decimal value, decimal minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(decimal value, decimal minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(decimal value, decimal minimum, decimal maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(decimal value, decimal minimum, decimal maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="decimal"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(decimal value, decimal minimum, decimal maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see cref="decimal"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(decimal value, decimal minimum, decimal maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(decimal value, decimal minimum, decimal maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see cref="decimal"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see cref="decimal"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see cref="decimal"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(decimal value, decimal minimum, decimal maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="target">The target <see langword="nint"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(nint value, nint target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="target">The target <see langword="nint"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(nint value, nint target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(nint value, nint maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(nint value, nint maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(nint value, nint minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(nint value, nint minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(nint value, nint minimum, nint maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(nint value, nint minimum, nint maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see langword="nint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(nint value, nint minimum, nint maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see langword="nint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(nint value, nint minimum, nint maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(nint value, nint minimum, nint maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see langword="nint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nint"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see langword="nint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(nint value, nint minimum, nint maximum)
    {
        return value < minimum || value > maximum;
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="target">The target <see langword="nuint"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(nuint value, nuint target)
    {
        return value == target;
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="target">The target <see langword="nuint"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo(nuint value, nuint target)
    {
        return value != target;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan(nuint value, nuint maximum)
    {
        return value < maximum;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo(nuint value, nuint maximum)
    {
        return value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan(nuint value, nuint minimum)
    {
        return value > minimum;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo(nuint value, nuint minimum)
    {
        return value >= minimum;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange(nuint value, nuint minimum, nuint maximum)
    {
        return value >= minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange(nuint value, nuint minimum, nuint maximum)
    {
        return value < minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see langword="nuint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(nuint value, nuint minimum, nuint maximum)
    {
        return value > minimum && value < maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <see langword="nuint"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween(nuint value, nuint minimum, nuint maximum)
    {
        return value <= minimum || value >= maximum;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo(nuint value, nuint minimum, nuint maximum)
    {
        return value >= minimum && value <= maximum;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <param name="value">The input <see langword="nuint"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <see langword="nuint"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <see langword="nuint"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo(nuint value, nuint minimum, nuint maximum)
    {
        return value < minimum || value > maximum;
    }
}

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
    /// Checks that the input value is <see langword="default"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault<T>(T value)
        where T : struct, IEquatable<T>
    {
        return value.Equals(default);
    }

    /// <summary>
    /// Checks that the input value is not <see langword="default"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see langword="struct"/> value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotDefault<T>(T value)
        where T : struct, IEquatable<T>
    {
        return !value.Equals(default);
    }

    /// <summary>
    /// Checks that the input value must be equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo<T>(T value, T target)
        where T : notnull, IEquatable<T>
    {
        return value.Equals(target);
    }

    /// <summary>
    /// Checks that the input value must be not equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEqualTo<T>(T value, T target)
        where T : notnull, IEquatable<T>
    {
        return !value.Equals(target);
    }

    /// <summary>
    /// Checks that the input value must be a bitwise match with a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool IsBitwiseEqualTo<T>(T value, T target)
        where T : unmanaged
    {
        // Include some fast paths if the input type is of size 1, 2, 4, 8, or 16.
        // In those cases, just reinterpret the bytes as values of an integer type,
        // and compare them directly, which is much faster than having a loop over each byte.
        // The conditional branches below are known at compile time by the JIT compiler,
        // so that only the right one will actually be translated into native code.
        if (sizeof(T) == 1)
        {
            byte valueByte = Unsafe.As<T, byte>(ref value);
            byte targetByte = Unsafe.As<T, byte>(ref target);

            return valueByte == targetByte;
        }
        else if (sizeof(T) == 2)
        {
            ushort valueUShort = Unsafe.As<T, ushort>(ref value);
            ushort targetUShort = Unsafe.As<T, ushort>(ref target);

            return valueUShort == targetUShort;
        }
        else if (sizeof(T) == 4)
        {
            uint valueUInt = Unsafe.As<T, uint>(ref value);
            uint targetUInt = Unsafe.As<T, uint>(ref target);

            return valueUInt == targetUInt;
        }
        else if (sizeof(T) == 8)
        {
            ulong valueULong = Unsafe.As<T, ulong>(ref value);
            ulong targetULong = Unsafe.As<T, ulong>(ref target);

            return Bit64Compare(ref valueULong, ref targetULong);
        }
        else if (sizeof(T) == 16)
        {
            ulong valueULong0 = Unsafe.As<T, ulong>(ref value);
            ulong targetULong0 = Unsafe.As<T, ulong>(ref target);

            if (Bit64Compare(ref valueULong0, ref targetULong0))
            {
                ulong valueULong1 = Unsafe.Add(ref Unsafe.As<T, ulong>(ref value), 1);
                ulong targetULong1 = Unsafe.Add(ref Unsafe.As<T, ulong>(ref target), 1);

                if (Bit64Compare(ref valueULong1, ref targetULong1))
                {
                    return true;
                }
            }

            return false;
        }
        else
        {
            Span<byte> valueBytes = new(Unsafe.AsPointer(ref value), sizeof(T));
            Span<byte> targetBytes = new(Unsafe.AsPointer(ref target), sizeof(T));

            return valueBytes.SequenceEqual(targetBytes);
        }
    }

    // Compares 64 bits of data from two given memory locations for bitwise equality
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe bool Bit64Compare(ref ulong left, ref ulong right)
    {
        // Handles 32 bit case, because using ulong is inefficient
        if (sizeof(IntPtr) == 4)
        {
            ref int r0 = ref Unsafe.As<ulong, int>(ref left);
            ref int r1 = ref Unsafe.As<ulong, int>(ref right);

            return r0 == r1 &&
                   Unsafe.Add(ref r0, 1) == Unsafe.Add(ref r1, 1);
        }

        return left == right;
    }

    /// <summary>
    /// Checks that the input value must be less than a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThan<T>(T value, T maximum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(maximum) < 0;
    }

    /// <summary>
    /// Checks that the input value must be less than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLessThanOrEqualTo<T>(T value, T maximum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(maximum) <= 0;
    }

    /// <summary>
    /// Checks that the input value must be greater than a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThan<T>(T value, T minimum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(minimum) > 0;
    }

    /// <summary>
    /// Checks that the input value must be greater than or equal to a specified value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>The method is generic to avoid boxing the parameters, if they are value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGreaterThanOrEqualTo<T>(T value, T minimum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(minimum) >= 0;
    }

    /// <summary>
    /// Checks that the input value must be in a given range.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRange<T>(T value, T minimum, T maximum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(minimum) >= 0 && value.CompareTo(maximum) < 0;
    }

    /// <summary>
    /// Checks that the input value must not be in a given range.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotInRange<T>(T value, T minimum, T maximum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(minimum) < 0 || value.CompareTo(maximum) >= 0;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween<T>(T value, T minimum, T maximum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(minimum) > 0 && value.CompareTo(maximum) < 0;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The exclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The exclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in (<paramref name="minimum"/>, <paramref name="maximum"/>)", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetween<T>(T value, T minimum, T maximum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(minimum) <= 0 || value.CompareTo(maximum) >= 0;
    }

    /// <summary>
    /// Checks that the input value must be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetweenOrEqualTo<T>(T value, T minimum, T maximum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(minimum) >= 0 && value.CompareTo(maximum) <= 0;
    }

    /// <summary>
    /// Checks that the input value must not be in a given interval.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="minimum">The inclusive minimum <typeparamref name="T"/> value that is accepted.</param>
    /// <param name="maximum">The inclusive maximum <typeparamref name="T"/> value that is accepted.</param>
    /// <remarks>
    /// This API Checks the equivalent of "<paramref name="value"/> not in [<paramref name="minimum"/>, <paramref name="maximum"/>]", using arithmetic notation.
    /// The method is generic to avoid boxing the parameters, if they are value types.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBetweenOrEqualTo<T>(T value, T minimum, T maximum)
        where T : notnull, IComparable<T>
    {
        return value.CompareTo(minimum) < 0 || value.CompareTo(maximum) > 0;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

/// <summary>
/// A helper class for <see cref="Dictionary2{TKey,TValue}"/>.
/// </summary>
internal static class HashHelpers
{
    /// <summary>
    /// Maximum prime smaller than the maximum array length.
    /// </summary>
    private const int MaxPrimeArrayLength = 0x7FFFFFC3;

    /// <summary>
    /// An arbitrary prime factor used in <see cref="GetPrime(int)"/>.
    /// </summary>
    private const int HashPrime = 101;

    /// <summary>
    /// Table of prime numbers to use as hash table sizes.
    /// </summary>
    private static readonly int[] primes =
    {
        3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
        1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
        17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
        187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
        1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
    };

    /// <summary>
    /// Checks whether a value is a prime.
    /// </summary>
    /// <param name="candidate">The value to check.</param>
    /// <returns>Whether or not <paramref name="candidate"/> is a prime.</returns>
    private static bool IsPrime(int candidate)
    {
        if ((candidate & 1) != 0)
        {
            int limit = (int)Math.Sqrt(candidate);

            for (int divisor = 3; divisor <= limit; divisor += 2)
            {
                if ((candidate % divisor) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        return candidate == 2;
    }

    /// <summary>
    /// Gets the smallest prime bigger than a specified value.
    /// </summary>
    /// <param name="min">The target minimum value.</param>
    /// <returns>The new prime that was found.</returns>
    public static int GetPrime(int min)
    {
        foreach (int prime in primes)
        {
            if (prime >= min)
            {
                return prime;
            }
        }

        for (int i = min | 1; i < int.MaxValue; i += 2)
        {
            if (IsPrime(i) && ((i - 1) % HashPrime != 0))
            {
                return i;
            }
        }

        return min;
    }

    /// <summary>
    /// Returns size of hashtable to grow to.
    /// </summary>
    /// <param name="oldSize">The previous table size.</param>
    /// <returns>The expanded table size.</returns>
    public static int ExpandPrime(int oldSize)
    {
        int newSize = 2 * oldSize;

        if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
        {
            return MaxPrimeArrayLength;
        }

        return GetPrime(newSize);
    }

    /// <summary>
    /// Returns approximate reciprocal of the divisor: ceil(2**64 / divisor).
    /// </summary>
    /// <remarks>This should only be used on 64-bit.</remarks>
    public static ulong GetFastModMultiplier(uint divisor)
    {
        return ulong.MaxValue / divisor + 1;
    }

    /// <summary>
    /// Performs a mod operation using the multiplier pre-computed with <see cref="GetFastModMultiplier"/>.
    /// </summary>
    /// <remarks>This should only be used on 64-bit.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint FastMod(uint value, uint divisor, ulong multiplier)
    {
        return (uint)(((((multiplier * value) >> 32) + 1) * divisor) >> 32);
    }
}
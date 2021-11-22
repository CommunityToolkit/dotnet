// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET6_0_OR_GREATER

using System.Runtime.CompilerServices;
#if NETCOREAPP3_1
using System.Runtime.Intrinsics.X86;
using static System.Numerics.BitOperations;
#endif

namespace CommunityToolkit.HighPerformance.Helpers.Internals;

/// <summary>
/// Utility methods for intrinsic bit-twiddling operations. The methods use hardware intrinsics
/// when available on the underlying platform, otherwise they use optimized software fallbacks.
/// </summary>
internal static class BitOperations
{
    /// <summary>
    /// Round the given integral value up to a power of 2.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
    /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe uint RoundUpToPowerOf2(uint value)
    {
#if NETCOREAPP3_1
        if (Lzcnt.IsSupported)
        {
            if (sizeof(nint) == 8)
            {
                return (uint)(0x1_0000_0000ul >> LeadingZeroCount(value - 1));
            }
            else
            {
                int shift = 32 - LeadingZeroCount(value - 1);

                return (1u ^ (uint)(shift >> 5)) << shift;
            }
        }
#endif

        // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        return value + 1;
    }
}

#endif

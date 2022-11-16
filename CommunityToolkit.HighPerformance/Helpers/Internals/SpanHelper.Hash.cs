// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System.Numerics;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.HighPerformance.Helpers.Internals;

/// <inheritdoc/>
partial class SpanHelper
{
    /// <summary>
    /// Calculates the djb2 hash for the target sequence of items of a given type.
    /// </summary>
    /// <typeparam name="T">The type of items to hash.</typeparam>
    /// <param name="r0">The reference to the target memory area to hash.</param>
    /// <param name="length">The number of items to hash.</param>
    /// <returns>The Djb2 value for the input sequence of items.</returns>
    public static int GetDjb2HashCode<T>(ref T r0, nint length)
        where T : notnull
    {
        int hash = 5381;
        nint offset = 0;

        while (length >= 8)
        {
            // Doing a left shift by 5 and adding is equivalent to multiplying by 33.
            // This is preferred for performance reasons, as when working with integer
            // values most CPUs have higher latency for multiplication operations
            // compared to a simple shift and add. For more info on this, see the
            // details for imul, shl, add: https://gmplib.org/~tege/x86-timing.pdf.
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 0).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 1).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 2).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 3).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 4).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 5).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 6).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 7).GetHashCode());

            length -= 8;
            offset += 8;
        }

        if (length >= 4)
        {
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 0).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 1).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 2).GetHashCode());
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset + 3).GetHashCode());

            length -= 4;
            offset += 4;
        }

        while (length > 0)
        {
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset).GetHashCode());

            length -= 1;
            offset += 1;
        }

        return hash;
    }

    /// <summary>
    /// Gets a content hash from a given memory area.
    /// </summary>
    /// <param name="r0">A <see cref="byte"/> reference to the start of the memory area.</param>
    /// <param name="length">The size in bytes of the memory area.</param>
    /// <returns>The hash code for the contents of the source memory area.</returns>
    /// <remarks>
    /// While this method is similar to <see cref="GetDjb2HashCode{T}"/> and can in some cases
    /// produce the same output for a given memory area, it is not guaranteed to always be that way.
    /// This is because this method can use SIMD instructions if possible, which can cause a computed
    /// hash to differ for the same data, if processed on different machines with different CPU features.
    /// The advantage of this method is that when SIMD instructions are available, it performs much
    /// faster than <see cref="GetDjb2HashCode{T}"/>, as it can parallelize much of the workload.
    /// </remarks>
    public static unsafe int GetDjb2LikeByteHash(ref byte r0, nint length)
    {
        int hash = 5381;
        nint offset = 0;

        // Check whether SIMD instructions are supported, and also check
        // whether we have enough data to perform at least one unrolled
        // iteration of the vectorized path. This heuristics is to balance
        // the overhead of loading the constant values in the two registers,
        // and the final loop to combine the partial hash values.
        // Note that even when we use the vectorized path we don't need to do
        // any preprocessing to try to get memory aligned, as that would cause
        // the hash codes to potentially be different for the same data.
        if (Vector.IsHardwareAccelerated &&
            length >= (Vector<byte>.Count << 3))
        {
            Vector<int> vh = new(5381);
            Vector<int> v33 = new(33);

            // First vectorized loop, with 8 unrolled iterations.
            // Assuming 256-bit registers (AVX2), a total of 256 bytes are processed
            // per iteration, with the partial hashes being accumulated for later use.
            while (length >= (Vector<byte>.Count << 3))
            {
                ref byte ri0 = ref Unsafe.Add(ref r0, offset + (Vector<byte>.Count * 0));
                Vector<int> vi0 = Unsafe.ReadUnaligned<Vector<int>>(ref ri0);
                Vector<int> vp0 = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp0, vi0);

                ref byte ri1 = ref Unsafe.Add(ref r0, offset + (Vector<byte>.Count * 1));
                Vector<int> vi1 = Unsafe.ReadUnaligned<Vector<int>>(ref ri1);
                Vector<int> vp1 = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp1, vi1);

                ref byte ri2 = ref Unsafe.Add(ref r0, offset + (Vector<byte>.Count * 2));
                Vector<int> vi2 = Unsafe.ReadUnaligned<Vector<int>>(ref ri2);
                Vector<int> vp2 = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp2, vi2);

                ref byte ri3 = ref Unsafe.Add(ref r0, offset + (Vector<byte>.Count * 3));
                Vector<int> vi3 = Unsafe.ReadUnaligned<Vector<int>>(ref ri3);
                Vector<int> vp3 = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp3, vi3);

                ref byte ri4 = ref Unsafe.Add(ref r0, offset + (Vector<byte>.Count * 4));
                Vector<int> vi4 = Unsafe.ReadUnaligned<Vector<int>>(ref ri4);
                Vector<int> vp4 = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp4, vi4);

                ref byte ri5 = ref Unsafe.Add(ref r0, offset + (Vector<byte>.Count * 5));
                Vector<int> vi5 = Unsafe.ReadUnaligned<Vector<int>>(ref ri5);
                Vector<int> vp5 = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp5, vi5);

                ref byte ri6 = ref Unsafe.Add(ref r0, offset + (Vector<byte>.Count * 6));
                Vector<int> vi6 = Unsafe.ReadUnaligned<Vector<int>>(ref ri6);
                Vector<int> vp6 = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp6, vi6);

                ref byte ri7 = ref Unsafe.Add(ref r0, offset + (Vector<byte>.Count * 7));
                Vector<int> vi7 = Unsafe.ReadUnaligned<Vector<int>>(ref ri7);
                Vector<int> vp7 = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp7, vi7);

                length -= Vector<byte>.Count << 3;
                offset += Vector<byte>.Count << 3;
            }

            // When this loop is reached, there are up to 255 bytes left (on AVX2).
            // Each iteration processed an additional 32 bytes and accumulates the results.
            while (length >= Vector<byte>.Count)
            {
                ref byte ri = ref Unsafe.Add(ref r0, offset);
                Vector<int> vi = Unsafe.ReadUnaligned<Vector<int>>(ref ri);
                Vector<int> vp = Vector.Multiply(vh, v33);
                vh = Vector.Xor(vp, vi);

                length -= Vector<byte>.Count;
                offset += Vector<byte>.Count;
            }

            // Combine the partial hash values in each position.
            // The loop below should automatically be unrolled by the JIT.
            for (int j = 0; j < Vector<int>.Count; j++)
            {
                hash = unchecked(((hash << 5) + hash) ^ vh[j]);
            }
        }
        else
        {
            // Only use the loop working with 64-bit values if we are on a
            // 64-bit processor, otherwise the result would be much slower.
            // Each unrolled iteration processes 64 bytes.
            if (sizeof(nint) == sizeof(ulong))
            {
                while (length >= (sizeof(ulong) << 3))
                {
                    ref byte ri0 = ref Unsafe.Add(ref r0, offset + (sizeof(ulong) * 0));
                    ulong value0 = Unsafe.ReadUnaligned<ulong>(ref ri0);
                    hash = unchecked(((hash << 5) + hash) ^ (int)value0 ^ (int)(value0 >> 32));

                    ref byte ri1 = ref Unsafe.Add(ref r0, offset + (sizeof(ulong) * 1));
                    ulong value1 = Unsafe.ReadUnaligned<ulong>(ref ri1);
                    hash = unchecked(((hash << 5) + hash) ^ (int)value1 ^ (int)(value1 >> 32));

                    ref byte ri2 = ref Unsafe.Add(ref r0, offset + (sizeof(ulong) * 2));
                    ulong value2 = Unsafe.ReadUnaligned<ulong>(ref ri2);
                    hash = unchecked(((hash << 5) + hash) ^ (int)value2 ^ (int)(value2 >> 32));

                    ref byte ri3 = ref Unsafe.Add(ref r0, offset + (sizeof(ulong) * 3));
                    ulong value3 = Unsafe.ReadUnaligned<ulong>(ref ri3);
                    hash = unchecked(((hash << 5) + hash) ^ (int)value3 ^ (int)(value3 >> 32));

                    ref byte ri4 = ref Unsafe.Add(ref r0, offset + (sizeof(ulong) * 4));
                    ulong value4 = Unsafe.ReadUnaligned<ulong>(ref ri4);
                    hash = unchecked(((hash << 5) + hash) ^ (int)value4 ^ (int)(value4 >> 32));

                    ref byte ri5 = ref Unsafe.Add(ref r0, offset + (sizeof(ulong) * 5));
                    ulong value5 = Unsafe.ReadUnaligned<ulong>(ref ri5);
                    hash = unchecked(((hash << 5) + hash) ^ (int)value5 ^ (int)(value5 >> 32));

                    ref byte ri6 = ref Unsafe.Add(ref r0, offset + (sizeof(ulong) * 6));
                    ulong value6 = Unsafe.ReadUnaligned<ulong>(ref ri6);
                    hash = unchecked(((hash << 5) + hash) ^ (int)value6 ^ (int)(value6 >> 32));

                    ref byte ri7 = ref Unsafe.Add(ref r0, offset + (sizeof(ulong) * 7));
                    ulong value7 = Unsafe.ReadUnaligned<ulong>(ref ri7);
                    hash = unchecked(((hash << 5) + hash) ^ (int)value7 ^ (int)(value7 >> 32));

                    length -= sizeof(ulong) << 3;
                    offset += sizeof(ulong) << 3;
                }
            }

            // Each unrolled iteration processes 32 bytes
            while (length >= (sizeof(uint) << 3))
            {
                ref byte ri0 = ref Unsafe.Add(ref r0, offset + (sizeof(uint) * 0));
                uint value0 = Unsafe.ReadUnaligned<uint>(ref ri0);
                hash = unchecked(((hash << 5) + hash) ^ (int)value0);

                ref byte ri1 = ref Unsafe.Add(ref r0, offset + (sizeof(uint) * 1));
                uint value1 = Unsafe.ReadUnaligned<uint>(ref ri1);
                hash = unchecked(((hash << 5) + hash) ^ (int)value1);

                ref byte ri2 = ref Unsafe.Add(ref r0, offset + (sizeof(uint) * 2));
                uint value2 = Unsafe.ReadUnaligned<uint>(ref ri2);
                hash = unchecked(((hash << 5) + hash) ^ (int)value2);

                ref byte ri3 = ref Unsafe.Add(ref r0, offset + (sizeof(uint) * 3));
                uint value3 = Unsafe.ReadUnaligned<uint>(ref ri3);
                hash = unchecked(((hash << 5) + hash) ^ (int)value3);

                ref byte ri4 = ref Unsafe.Add(ref r0, offset + (sizeof(uint) * 4));
                uint value4 = Unsafe.ReadUnaligned<uint>(ref ri4);
                hash = unchecked(((hash << 5) + hash) ^ (int)value4);

                ref byte ri5 = ref Unsafe.Add(ref r0, offset + (sizeof(uint) * 5));
                uint value5 = Unsafe.ReadUnaligned<uint>(ref ri5);
                hash = unchecked(((hash << 5) + hash) ^ (int)value5);

                ref byte ri6 = ref Unsafe.Add(ref r0, offset + (sizeof(uint) * 6));
                uint value6 = Unsafe.ReadUnaligned<uint>(ref ri6);
                hash = unchecked(((hash << 5) + hash) ^ (int)value6);

                ref byte ri7 = ref Unsafe.Add(ref r0, offset + (sizeof(uint) * 7));
                uint value7 = Unsafe.ReadUnaligned<uint>(ref ri7);
                hash = unchecked(((hash << 5) + hash) ^ (int)value7);

                length -= sizeof(uint) << 3;
                offset += sizeof(uint) << 3;
            }
        }

        // At this point (assuming AVX2), there will be up to 31 bytes
        // left, both for the vectorized and non vectorized paths.
        // That number would go up to 63 on AVX512 systems, in which case it is
        // still useful to perform this last loop unrolling.
        if (length >= (sizeof(ushort) << 3))
        {
            ref byte ri0 = ref Unsafe.Add(ref r0, offset + (sizeof(ushort) * 0));
            ushort value0 = Unsafe.ReadUnaligned<ushort>(ref ri0);
            hash = unchecked(((hash << 5) + hash) ^ value0);

            ref byte ri1 = ref Unsafe.Add(ref r0, offset + (sizeof(ushort) * 1));
            ushort value1 = Unsafe.ReadUnaligned<ushort>(ref ri1);
            hash = unchecked(((hash << 5) + hash) ^ value1);

            ref byte ri2 = ref Unsafe.Add(ref r0, offset + (sizeof(ushort) * 2));
            ushort value2 = Unsafe.ReadUnaligned<ushort>(ref ri2);
            hash = unchecked(((hash << 5) + hash) ^ value2);

            ref byte ri3 = ref Unsafe.Add(ref r0, offset + (sizeof(ushort) * 3));
            ushort value3 = Unsafe.ReadUnaligned<ushort>(ref ri3);
            hash = unchecked(((hash << 5) + hash) ^ value3);

            ref byte ri4 = ref Unsafe.Add(ref r0, offset + (sizeof(ushort) * 4));
            ushort value4 = Unsafe.ReadUnaligned<ushort>(ref ri4);
            hash = unchecked(((hash << 5) + hash) ^ value4);

            ref byte ri5 = ref Unsafe.Add(ref r0, offset + (sizeof(ushort) * 5));
            ushort value5 = Unsafe.ReadUnaligned<ushort>(ref ri5);
            hash = unchecked(((hash << 5) + hash) ^ value5);

            ref byte ri6 = ref Unsafe.Add(ref r0, offset + (sizeof(ushort) * 6));
            ushort value6 = Unsafe.ReadUnaligned<ushort>(ref ri6);
            hash = unchecked(((hash << 5) + hash) ^ value6);

            ref byte ri7 = ref Unsafe.Add(ref r0, offset + (sizeof(ushort) * 7));
            ushort value7 = Unsafe.ReadUnaligned<ushort>(ref ri7);
            hash = unchecked(((hash << 5) + hash) ^ value7);

            length -= sizeof(ushort) << 3;
            offset += sizeof(ushort) << 3;
        }

        // Handle the leftover items
        while (length > 0)
        {
            hash = unchecked(((hash << 5) + hash) ^ Unsafe.Add(ref r0, offset));

            length -= 1;
            offset += 1;
        }

        return hash;
    }
}

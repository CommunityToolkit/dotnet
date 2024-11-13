// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This extension is restricted to the .NET 6 because it shares the same BCL
// across all targets, ensuring that the layout of our Nullable<T> mapping type
// will be correct. Exposing this API on older targets (especially .NET Standard)
// is not guaranteed to be correct and could result in invalid memory accesses.

#if NET6_0_OR_GREATER

using System;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// Helpers for working with the <see cref="Nullable{T}"/> type.
/// </summary>
public static class NullableExtensions
{
    /// <summary>
    /// Returns a reference to the value of the input <see cref="Nullable{T}"/> instance, regardless of whether
    /// the <see cref="Nullable{T}.HasValue"/> property is returning <see langword="true"/> or not. If that is not
    /// the case, this method will still return a reference to the underlying <see langword="default"/> value.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="value">The <see cref="Nullable{T}"/>.</param>
    /// <returns>A reference to the underlying value from the input <see cref="Nullable{T}"/> instance.</returns>
    /// <remarks>
    /// Note that attempting to mutate the returned reference will not change the value returned by <see cref="Nullable{T}.HasValue"/>.
    /// That means that reassigning the value of an empty instance will not make <see cref="Nullable{T}.HasValue"/> return <see langword="true"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T DangerousGetValueOrDefaultReference<T>(this ref T? value)
        where T : struct
    {
#if NET8_0_OR_GREATER
        return ref Unsafe.AsRef(in Nullable.GetValueRefOrDefaultRef(in value));
#else
        return ref Unsafe.As<T?, RawNullableData<T>>(ref value).Value;
#endif
    }

    /// <summary>
    /// Returns a reference to the value of the input <see cref="Nullable{T}"/> instance, or a <see langword="null"/> <typeparamref name="T"/> reference.
    /// </summary>
    /// <typeparam name="T">The type of the underlying value.</typeparam>
    /// <param name="value">The <see cref="Nullable{T}"/>.</param>
    /// <returns>A reference to the value of the input <see cref="Nullable{T}"/> instance, or a <see langword="null"/> <typeparamref name="T"/> reference.</returns>
    /// <remarks>The returned reference can be tested for <see langword="null"/> using <see cref="Unsafe.IsNullRef"/>.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ref T DangerousGetValueOrNullReference<T>(ref this T? value)
        where T : struct
    {
#if NET8_0_OR_GREATER
        ref T resultRef = ref *(T*)null;

        // This pattern ensures that the resulting code ends up having a single return, and a single
        // forward branch (the one where the value is null) that is predicted non taken. That is,
        // the initial null ref is very cheap as it's just clearing a register, and the rest of the
        // code is a single assignment (lea on x86-64) that should always be taken. This results in:
        // =============================
        // L0000: xor eax, eax
        // L0002: cmp byte ptr[rcx], 0
        // L0005: je short L000b
        // L0007: lea rax, [rcx + 4]
        // L000b: ret
        // =============================
        // This is better than what the code would've been with two separate returns in the method.
        if (value.HasValue)
        {
            resultRef = ref Unsafe.AsRef(in Nullable.GetValueRefOrDefaultRef(in value));
        }

        return ref resultRef;
#else
        if (value.HasValue)
        {
            return ref Unsafe.As<T?, RawNullableData<T>>(ref value).Value;
        }

        return ref *(T*)null;
#endif
    }

#if !NET8_0_OR_GREATER
    /// <summary>
    /// Mapping type that reflects the internal layout of the <see cref="Nullable{T}"/> type.
    /// See https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/Nullable.cs.
    /// </summary>
    /// <typeparam name="T">The value type wrapped by the current instance.</typeparam>
    private struct RawNullableData<T>
        where T : struct
    {
#pragma warning disable CS0649 // Unassigned fields
        public bool HasValue;
        public T Value;
#pragma warning restore CS0649
    }
#endif
}

#endif
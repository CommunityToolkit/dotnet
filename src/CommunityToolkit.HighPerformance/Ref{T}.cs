// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET8_0_OR_GREATER

using System.Runtime.CompilerServices;

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// A <see langword="struct"/> that can store a reference to a value of a specified type.
/// </summary>
/// <typeparam name="T">The type of value to reference.</typeparam>
public readonly ref struct Ref<T>
{
    /// <summary>
    /// The reference to the target <typeparamref name="T"/> value.
    /// </summary>
    private readonly ref T value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ref{T}"/> struct.
    /// </summary>
    /// <param name="value">The reference to the target <typeparamref name="T"/> value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Ref(ref T value)
    {
        this.value = ref value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ref{T}"/> struct.
    /// </summary>
    /// <param name="pointer">The pointer to the target value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Ref(void* pointer)
        : this(ref Unsafe.AsRef<T>(pointer))
    {
    }

    /// <summary>
    /// Gets the <typeparamref name="T"/> reference represented by the current <see cref="Ref{T}"/> instance.
    /// </summary>
    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this.value;
    }

    /// <summary>
    /// Implicitly gets the <typeparamref name="T"/> value from a given <see cref="Ref{T}"/> instance.
    /// </summary>
    /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(Ref<T> reference)
    {
        return reference.Value;
    }
}

#endif

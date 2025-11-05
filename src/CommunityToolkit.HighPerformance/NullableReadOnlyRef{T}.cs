// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET8_0_OR_GREATER

using System;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// A <see langword="struct"/> that can store an optional readonly reference to a value of a specified type.
/// </summary>
/// <typeparam name="T">The type of value to reference.</typeparam>
public readonly ref struct NullableReadOnlyRef<T>
{
    /// <summary>
    /// The reference to the target <typeparamref name="T"/> value.
    /// </summary>
    private readonly ref readonly T value;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableReadOnlyRef{T}"/> struct.
    /// </summary>
    /// <param name="value">The readonly reference to the target <typeparamref name="T"/> value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NullableReadOnlyRef(in T value)
    {
        this.value = ref value;
    }

    /// <summary>
    /// Gets a <see cref="NullableReadOnlyRef{T}"/> instance representing a <see langword="null"/> reference.
    /// </summary>
    public static NullableReadOnlyRef<T> Null
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
    }

    /// <summary>
    /// Gets a value indicating whether or not the current <see cref="NullableReadOnlyRef{T}"/> instance wraps a valid reference that can be accessed.
    /// </summary>
    public unsafe bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !Unsafe.IsNullRef(ref Unsafe.AsRef(in this.value));
    }

    /// <summary>
    /// Gets the <typeparamref name="T"/> reference represented by the current <see cref="NullableReadOnlyRef{T}"/> instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
    public ref readonly T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (!HasValue)
            {
                ThrowInvalidOperationException();
            }

            return ref this.value;
        }
    }

    /// <summary>
    /// Implicitly converts a <see cref="Ref{T}"/> instance into a <see cref="NullableReadOnlyRef{T}"/> one.
    /// </summary>
    /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableReadOnlyRef<T>(Ref<T> reference)
    {
        return new(in reference.Value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="ReadOnlyRef{T}"/> instance into a <see cref="NullableReadOnlyRef{T}"/> one.
    /// </summary>
    /// <param name="reference">The input <see cref="ReadOnlyRef{T}"/> instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableReadOnlyRef<T>(ReadOnlyRef<T> reference)
    {
        return new(in reference.Value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="NullableRef{T}"/> instance into a <see cref="NullableReadOnlyRef{T}"/> one.
    /// </summary>
    /// <param name="reference">The input <see cref="Ref{T}"/> instance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator NullableReadOnlyRef<T>(NullableRef<T> reference)
    {
        return new(in reference.Value);
    }

    /// <summary>
    /// Explicitly gets the <typeparamref name="T"/> value from a given <see cref="NullableReadOnlyRef{T}"/> instance.
    /// </summary>
    /// <param name="reference">The input <see cref="NullableReadOnlyRef{T}"/> instance.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator T(NullableReadOnlyRef<T> reference)
    {
        return reference.Value;
    }

    /// <summary>
    /// Throws a <see cref="InvalidOperationException"/> when trying to access <see cref="Value"/> for a default instance.
    /// </summary>
    private static void ThrowInvalidOperationException()
    {
        throw new InvalidOperationException("The current instance doesn't have a value that can be accessed.");
    }
}

#endif
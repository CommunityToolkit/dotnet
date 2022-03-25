// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#pragma warning disable CS8777

namespace CommunityToolkit.Diagnostics;

/// <summary>
/// Helper methods to verify conditions when running code.
/// </summary>
[DebuggerStepThrough]
public static partial class Check
{
    /// <summary>
    /// Checks that the input value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of reference value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is <see langword="null"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull<T>(T? value)
    {
        return value is null;
    }

    /// <summary>
    /// Checks that the input value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of nullable value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is <see langword="null"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull<T>(T? value)
        where T : struct
    {
        return value is null;
    }

    /// <summary>
    /// Checks that the input value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of reference value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is not <see langword="null"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNull<T>([NotNull] T? value)
    {
        return value is not null;
    }

    /// <summary>
    /// Checks that the input value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of nullable value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is not <see langword="null"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNull<T>([NotNull] T? value)
        where T : struct
    {
        return value is not null;
    }

    /// <summary>
    /// Checks that the input value is of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is of type <typeparamref name="T"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOfType<T>(object value)
    {
        return value.GetType() == typeof(T);
    }

    /// <summary>
    /// Checks that the input value is not of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is not of type <typeparamref name="T"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotOfType<T>(object value)
    {
        return value.GetType() != typeof(T);
    }

    /// <summary>
    /// Checks that the input value is of a specific type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is of type <paramref name="type"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOfType(object value, Type type)
    {
        return value.GetType() == type;
    }

    /// <summary>
    /// Checks that the input value is not of a specific type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is not of type <paramref name="type"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotOfType(object value, Type type)
    {
        return value.GetType() != type;
    }

    /// <summary>
    /// Checks that the input value can be assigned to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to check the input value against.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> can be assigned to type <typeparamref name="T"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAssignableToType<T>(object value)
    {
        return value is T;
    }

    /// <summary>
    /// Checks that the input value can't be assigned to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to check the input value against.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> cannot be assigned to type <typeparamref name="T"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotAssignableToType<T>(object value)
    {
        return value is not T;
    }

    /// <summary>
    /// Checks that the input value can be assigned to a specified type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> can be assigned to type <paramref name="type"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAssignableToType(object value, Type type)
    {
        return type.IsInstanceOfType(value);
    }

    /// <summary>
    /// Checks that the input value can't be assigned to a specified type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> cannot be assigned to type <paramref name="type"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotAssignableToType(object value, Type type)
    {
        return !type.IsInstanceOfType(value);
    }

    /// <summary>
    /// Checks that the input value must be the same instance as the target value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <remarks>The method is generic to prevent using it with value types.</remarks>
    /// <returns><see langword="true"/> if <paramref name="value"/> is the same instance as <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsReferenceEqualTo<T>(T value, T target)
        where T : class
    {
        return ReferenceEquals(value, target);
    }

    /// <summary>
    /// Checks that the input value must not be the same instance as the target value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <remarks>The method is generic to prevent using it with value types.</remarks>
    /// <returns><see langword="true"/> if <paramref name="value"/> is not the same instance as <paramref name="target"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsReferenceNotEqualTo<T>(T value, T target)
        where T : class
    {
        return !ReferenceEquals(value, target);
    }
}

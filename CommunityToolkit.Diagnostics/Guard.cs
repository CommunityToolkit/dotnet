// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <summary>
/// Helper methods to verify conditions when running code.
/// </summary>
[DebuggerStepThrough]
public static partial class Guard
{
    /// <summary>
    /// Asserts that the input value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of reference value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? IsNull<T>(T? value, [CallerArgumentExpression("value")] string name = "")
    {
        if (value is not null)
        {
            ThrowHelper.ThrowArgumentExceptionForIsNull(value, name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of nullable value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? IsNull<T>(T? value, [CallerArgumentExpression("value")] string name = "")
        where T : struct
    {
        if (value is not null)
        {
            ThrowHelper.ThrowArgumentExceptionForIsNull(value, name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of reference value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsNotNull<T>([NotNull] T? value, [CallerArgumentExpression("value")] string name = "")
    {
        if (value is null)
        {
            ThrowHelper.ThrowArgumentNullExceptionForIsNotNull<T>(name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of nullable value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsNotNull<T>([NotNull] T? value, [CallerArgumentExpression("value")] string name = "")
        where T : struct
    {
        if (value is null)
        {
            ThrowHelper.ThrowArgumentNullExceptionForIsNotNull<T?>(name);
        }

        return value.Value;
    }

    /// <summary>
    /// Asserts that the input value is of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not of type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsOfType<T>(object value, [CallerArgumentExpression("value")] string name = "")
    {
        if (value.GetType() != typeof(T))
        {
            ThrowHelper.ThrowArgumentExceptionForIsOfType<T>(value, name);
        }

        return (T)value;
    }

    /// <summary>
    /// Asserts that the input value is not of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is of type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsNotOfType<T>(object value, [CallerArgumentExpression("value")] string name = "")
    {
        if (value.GetType() == typeof(T))
        {
            ThrowHelper.ThrowArgumentExceptionForIsNotOfType<T>(value, name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value is of a specific type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the type of <paramref name="value"/> is not the same as <paramref name="type"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsOfType(object value, Type type, [CallerArgumentExpression("value")] string name = "")
    {
        if (value.GetType() != type)
        {
            ThrowHelper.ThrowArgumentExceptionForIsOfType(value, type, name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value is not of a specific type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if the type of <paramref name="value"/> is the same as <paramref name="type"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsNotOfType(object value, Type type, [CallerArgumentExpression("value")] string name = "")
    {
        if (value.GetType() == type)
        {
            ThrowHelper.ThrowArgumentExceptionForIsNotOfType(value, type, name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value can be assigned to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to check the input value against.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can't be assigned to type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsAssignableToType<T>(object value, [CallerArgumentExpression("value")] string name = "")
    {
        if (value is T t)
        {
            return t;
        }

        ThrowHelper.ThrowArgumentExceptionForIsAssignableToType<T>(value, name);
        return default;
    }

    /// <summary>
    /// Asserts that the input value can't be assigned to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to check the input value against.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can be assigned to type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsNotAssignableToType<T>(object value, [CallerArgumentExpression("value")] string name = "")
    {
        if (value is T)
        {
            ThrowHelper.ThrowArgumentExceptionForIsNotAssignableToType<T>(value, name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value can be assigned to a specified type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can't be assigned to <paramref name="type"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsAssignableToType(object value, Type type, [CallerArgumentExpression("value")] string name = "")
    {
        if (!type.IsInstanceOfType(value))
        {
            ThrowHelper.ThrowArgumentExceptionForIsAssignableToType(value, type, name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value can't be assigned to a specified type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can be assigned to <paramref name="type"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsNotAssignableToType(object value, Type type, [CallerArgumentExpression("value")] string name = "")
    {
        if (type.IsInstanceOfType(value))
        {
            ThrowHelper.ThrowArgumentExceptionForIsNotAssignableToType(value, type, name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value must be the same instance as the target value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not the same instance as <paramref name="target"/>.</exception>
    /// <remarks>The method is generic to prevent using it with value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsReferenceEqualTo<T>(T value, T target, [CallerArgumentExpression("value")] string name = "")
        where T : class
    {
        if (!ReferenceEquals(value, target))
        {
            ThrowHelper.ThrowArgumentExceptionForIsReferenceEqualTo<T>(name);
        }

        return value;
    }

    /// <summary>
    /// Asserts that the input value must not be the same instance as the target value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is the same instance as <paramref name="target"/>.</exception>
    /// <remarks>The method is generic to prevent using it with value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsReferenceNotEqualTo<T>(T value, T target, [CallerArgumentExpression("value")] string name = "")
        where T : class
    {
        if (ReferenceEquals(value, target))
        {
            ThrowHelper.ThrowArgumentExceptionForIsReferenceNotEqualTo<T>(name);
        }

        return value;
    }
}
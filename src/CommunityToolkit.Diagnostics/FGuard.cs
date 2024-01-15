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
public static partial class FGuard
{
    /// <summary>
    /// Asserts that the input value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of reference value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? IsNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNull(value, name);
        return value;
    }

    ///// <summary>
    ///// Asserts that the input value is <see langword="null"/>.
    ///// </summary>
    ///// <typeparam name="T">The type of nullable value type being tested.</typeparam>
    ///// <param name="value">The input value to test.</param>
    ///// <param name="name">The name of the input parameter being tested.</param>
    ///// <returns>The <paramref name="value"/> that is <see langword="null"/>.</returns>
    ///// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not <see langword="null"/>.</exception>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static T? IsNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string name = "")
    //    where T : struct
    //{
    //    Guard.IsNull(value, name);
    //    return value;
    //}

    /// <summary>
    /// Asserts that the input value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of reference value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is not <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static T? IsNotNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotNull(value, name);
        return value;
    }

    ///// <summary>
    ///// Asserts that the input value is not <see langword="null"/>.
    ///// </summary>
    ///// <typeparam name="T">The type of nullable value type being tested.</typeparam>
    ///// <param name="value">The input value to test.</param>
    ///// <param name="name">The name of the input parameter being tested.</param>
    ///// <returns>The <paramref name="value"/> that is not <see langword="null"/>.</returns>
    ///// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/>.</exception>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[return: NotNull]
    //public static T? IsNotNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string name = "")
    //    where T : struct
    //{
    //    Guard.IsNotNull(value, name);
    //    return value;
    //}

    /// <summary>
    /// Asserts that the input value is of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not of type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsOfType<T>(object value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsOfType<T>(value, name);
        if (typeof(T).IsValueType)
        {
            //return Box<T>.Dangerous
            //return Unsafe.Unbox<T>(value);
            return (T)value;
        }
        else
        {
            return Unsafe.As<object, T>(ref value);
        }
    }

    ///// <summary>
    ///// Asserts that the input value is of a specific type.
    ///// </summary>
    ///// <typeparam name="T">The type of the input value.</typeparam>
    ///// <param name="value">The input <see cref="object"/> to test.</param>
    ///// <param name="name">The name of the input parameter being tested.</param>
    ///// <returns>The <paramref name="value"/> of type <typeparamref name="T"/>.</returns>
    ///// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not of type <typeparamref name="T"/>.</exception>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    ////public static Box<T> IsBoxed<T>(object value, [CallerArgumentExpression(nameof(value))] string name = "")
    //public static T IsBoxed<T>(object value, [CallerArgumentExpression(nameof(value))] string name = "")
    //    where T : struct
    //{
    //    Guard.IsOfType<T>(value, name);
    //    //return Box<T>.Dangerous
    //    return Unsafe.Unbox<T>(value);
    //}

    /// <summary>
    /// Asserts that the input value is not of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is not the type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is of type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsNotOfType<T>(object value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotOfType<T>(value, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value is of a specific type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> of type <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the type of <paramref name="value"/> is not the same as <paramref name="type"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsOfType(object value, Type type, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsOfType(value, type, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value is not of a specific type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is not the type <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the type of <paramref name="value"/> is the same as <paramref name="type"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsNotOfType(object value, Type type, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotOfType(value, type, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value can be assigned to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to check the input value against.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is assignable to type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can't be assigned to type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsAssignableToType<T>(object value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsAssignableToType<T>(value, name);
        return Unsafe.As<object, T>(ref value);
    }

    /// <summary>
    /// Asserts that the input value can't be assigned to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to check the input value against.</typeparam>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is not assignable to type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can be assigned to type <typeparamref name="T"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsNotAssignableToType<T>(object value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotAssignableToType<T>(value, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value can be assigned to a specified type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is assignable to type <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can't be assigned to <paramref name="type"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsAssignableToType(object value, Type type, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsAssignableToType(value, type, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value can't be assigned to a specified type.
    /// </summary>
    /// <param name="value">The input <see cref="object"/> to test.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is not assignable to type <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> can be assigned to <paramref name="type"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object IsNotAssignableToType(object value, Type type, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsNotAssignableToType(value, type, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be the same instance as the target value.
    /// </summary>
    /// <typeparam name="T">The type of input values to compare.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to test.</param>
    /// <param name="target">The target <typeparamref name="T"/> value to test for.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is the same instance as <paramref name="target"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not the same instance as <paramref name="target"/>.</exception>
    /// <remarks>The method is generic to prevent using it with value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsReferenceEqualTo<T>(T value, T target, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : class
    {
        Guard.IsReferenceEqualTo(value, target, name);
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
    /// <returns>The <paramref name="value"/> that is different from the <paramref name="target"/> instance.</returns>
    /// <remarks>The method is generic to prevent using it with value types.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IsReferenceNotEqualTo<T>(T value, T target, [CallerArgumentExpression(nameof(value))] string name = "")
        where T : class
    {
        Guard.IsReferenceNotEqualTo(value, target, name);
        return value;
    }
}

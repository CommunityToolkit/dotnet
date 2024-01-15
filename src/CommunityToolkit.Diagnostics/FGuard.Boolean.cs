// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
#if NET6_0_OR_GREATER
using System.ComponentModel;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#if NET6_0_OR_GREATER
using System.Text;
#endif

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
public static partial class FGuard
{
    /// <summary>
    /// Asserts that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is <see langword="true"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[return: DoesNotReturnIf(false)]
    public static bool IsTrue([DoesNotReturnIf(false)] bool value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsTrue(value, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <param name="message">A message to display if <paramref name="value"/> is <see langword="false"/>.</param>
    /// <returns>The <paramref name="value"/> that is <see langword="true"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[return: DoesNotReturnIf(false)]
    public static bool IsTrue([DoesNotReturnIf(false)] bool value, string name, string message)
    {
        Guard.IsTrue(value, name, message);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="false"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="value"/> that is <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="true"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[return: DoesNotReturnIf(true)]
    public static bool IsFalse([DoesNotReturnIf(true)] bool value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        Guard.IsFalse(value, name);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="false"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <param name="message">A message to display if <paramref name="value"/> is <see langword="true"/>.</param>
    /// <returns>The <paramref name="value"/> that is <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="true"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[return: DoesNotReturnIf(true)]
    public static bool IsFalse([DoesNotReturnIf(true)] bool value, string name, string message)
    {
        Guard.IsFalse(value, name, message);
        return value;
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Asserts that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <param name="message">A message to display if <paramref name="value"/> is <see langword="false"/>.</param>
    /// <returns>The <paramref name="value"/> that is <see langword="true"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[return: DoesNotReturnIf(false)]
    public static bool IsTrue([DoesNotReturnIf(false)] bool value, string name, 
        [InterpolatedStringHandlerArgument(nameof(value))] ref Guard.IsTrueInterpolatedStringHandler message)
    {
        Guard.IsTrue(value, name, ref message);
        return value;
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="false"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <param name="message">A message to display if <paramref name="value"/> is <see langword="true"/>.</param>
    /// <returns>The <paramref name="value"/> that is <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="true"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[return: DoesNotReturnIf(true)]
    public static bool IsFalse([DoesNotReturnIf(true)] bool value, string name, 
        [InterpolatedStringHandlerArgument(nameof(value))] ref Guard.IsFalseInterpolatedStringHandler message)
    {
        Guard.IsFalse(value, name, ref message);
        return value;
    }
#endif
}

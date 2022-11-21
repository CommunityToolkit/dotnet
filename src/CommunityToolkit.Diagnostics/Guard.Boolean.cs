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
public static partial class Guard
{
    /// <summary>
    /// Asserts that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsTrue([DoesNotReturnIf(false)] bool value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        if (value)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsTrue(name);
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <param name="message">A message to display if <paramref name="value"/> is <see langword="false"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsTrue([DoesNotReturnIf(false)] bool value, string name, string message)
    {
        if (value)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsTrue(name, message);
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="false"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="true"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsFalse([DoesNotReturnIf(true)] bool value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        if (!value)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsFalse(name);
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="false"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <param name="message">A message to display if <paramref name="value"/> is <see langword="true"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="true"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsFalse([DoesNotReturnIf(true)] bool value, string name, string message)
    {
        if (!value)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsFalse(name, message);
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Asserts that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <param name="message">A message to display if <paramref name="value"/> is <see langword="false"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsTrue([DoesNotReturnIf(false)] bool value, string name, [InterpolatedStringHandlerArgument(nameof(value))] ref IsTrueInterpolatedStringHandler message)
    {
        if (value)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsTrue(name, message.ToStringAndClear());
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="false"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <param name="message">A message to display if <paramref name="value"/> is <see langword="true"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="true"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsFalse([DoesNotReturnIf(true)] bool value, string name, [InterpolatedStringHandlerArgument(nameof(value))] ref IsFalseInterpolatedStringHandler message)
    {
        if (!value)
        {
            return;
        }

        ThrowHelper.ThrowArgumentExceptionForIsFalse(name, message.ToStringAndClear());
    }

    /// <summary>
    /// Provides an interpolated string handler for <see cref="IsTrue(bool, string, ref IsTrueInterpolatedStringHandler)"/> that only performs formatting if the assert fails.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    public struct IsTrueInterpolatedStringHandler
    {
        /// <summary>
        /// The handler used to perform the formatting.
        /// </summary>
        private StringBuilder.AppendInterpolatedStringHandler handler;

        /// <summary>
        /// The underlying <see cref="StringBuilder"/> instance used by <see cref="handler"/>, if any.
        /// </summary>
        private StringBuilder? builder;

        /// <summary>
        /// Creates an instance of the handler.
        /// </summary>
        /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
        /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
        /// <param name="condition">The condition passed to the <see cref="Guard"/> method.</param>
        /// <param name="shouldAppend">A value indicating whether formatting should proceed.</param>
        /// <remarks>This is intended to be called only by compiler-generated code. Arguments are not validated as they'd otherwise be for members intended to be used directly.</remarks>
        public IsTrueInterpolatedStringHandler(int literalLength, int formattedCount, bool condition, out bool shouldAppend)
        {
            if (condition)
            {
                this.handler = default;
                this.builder = null;

                shouldAppend = false;
            }
            else
            {
                StringBuilder builder = new();

                this.handler = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, builder);
                this.builder = builder;

                shouldAppend = true;
            }
        }

        /// <summary>
        /// Extracts the built string from the handler.
        /// </summary>
        internal string ToStringAndClear()
        {
            string message = this.builder?.ToString() ?? string.Empty;

            this = default;

            return message;
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendLiteral(string)"/>
        public void AppendLiteral(string value)
        {
            this.handler.AppendLiteral(value);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted{T}(T)"/>
        public void AppendFormatted<T>(T value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted{T}(T, string?)"/>
        public void AppendFormatted<T>(T value, string? format)
        {
            this.handler.AppendFormatted(value, format);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted{T}(T, int)"/>
        public void AppendFormatted<T>(T value, int alignment)
        {
            this.handler.AppendFormatted(value, alignment);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted{T}(T, int, string?)"/>
        public void AppendFormatted<T>(T value, int alignment, string? format)
        {
            this.handler.AppendFormatted(value, alignment, format);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(ReadOnlySpan{char})"/>
        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(ReadOnlySpan{char}, int, string?)"/>
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
        {
            this.handler.AppendFormatted(value, alignment, format);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(string?)"/>
        public void AppendFormatted(string? value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(string?, int, string?)"/>
        public void AppendFormatted(string? value, int alignment = 0, string? format = null)
        {
            this.handler.AppendFormatted(value, alignment, format);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(object?, int, string?)"/>
        public void AppendFormatted(object? value, int alignment = 0, string? format = null)
        {
            this.handler.AppendFormatted(value, alignment, format);
        }
    }

    /// <summary>
    /// Provides an interpolated string handler for <see cref="IsFalse(bool, string, ref IsFalseInterpolatedStringHandler)"/> that only performs formatting if the assert fails.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    public struct IsFalseInterpolatedStringHandler
    {
        /// <summary>
        /// The handler used to perform the formatting.
        /// </summary>
        private StringBuilder.AppendInterpolatedStringHandler handler;

        /// <summary>
        /// The underlying <see cref="StringBuilder"/> instance used by <see cref="handler"/>, if any.
        /// </summary>
        private StringBuilder? builder;

        /// <summary>
        /// Creates an instance of the handler.
        /// </summary>
        /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
        /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
        /// <param name="condition">The condition passed to the <see cref="Guard"/> method.</param>
        /// <param name="shouldAppend">A value indicating whether formatting should proceed.</param>
        /// <remarks>This is intended to be called only by compiler-generated code. Arguments are not validated as they'd otherwise be for members intended to be used directly.</remarks>
        public IsFalseInterpolatedStringHandler(int literalLength, int formattedCount, bool condition, out bool shouldAppend)
        {
            if (!condition)
            {
                this.handler = default;
                this.builder = null;

                shouldAppend = false;
            }
            else
            {
                StringBuilder builder = new();

                this.handler = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, builder);
                this.builder = builder;

                shouldAppend = true;
            }
        }

        /// <summary>
        /// Extracts the built string from the handler.
        /// </summary>
        internal string ToStringAndClear()
        {
            string message = this.builder?.ToString() ?? string.Empty;

            this = default;

            return message;
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendLiteral(string)"/>
        public void AppendLiteral(string value)
        {
            this.handler.AppendLiteral(value);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted{T}(T)"/>
        public void AppendFormatted<T>(T value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted{T}(T, string?)"/>
        public void AppendFormatted<T>(T value, string? format)
        {
            this.handler.AppendFormatted(value, format);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted{T}(T, int)"/>
        public void AppendFormatted<T>(T value, int alignment)
        {
            this.handler.AppendFormatted(value, alignment);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted{T}(T, int, string?)"/>
        public void AppendFormatted<T>(T value, int alignment, string? format)
        {
            this.handler.AppendFormatted(value, alignment, format);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(ReadOnlySpan{char})"/>
        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(ReadOnlySpan{char}, int, string?)"/>
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
        {
            this.handler.AppendFormatted(value, alignment, format);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(string?)"/>
        public void AppendFormatted(string? value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(string?, int, string?)"/>
        public void AppendFormatted(string? value, int alignment = 0, string? format = null)
        {
            this.handler.AppendFormatted(value, alignment, format);
        }

        /// <inheritdoc cref="StringBuilder.AppendInterpolatedStringHandler.AppendFormatted(object?, int, string?)"/>
        public void AppendFormatted(object? value, int alignment = 0, string? format = null)
        {
            this.handler.AppendFormatted(value, alignment, format);
        }
    }
#endif
}

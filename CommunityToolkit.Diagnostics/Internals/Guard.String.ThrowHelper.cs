// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <inheritdoc/>
    partial class ThrowHelper
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNullOrEmpty"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNullOrEmpty(string? text, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must be null or empty, was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> or <see cref="ArgumentException"/> when <see cref="IsNotNullOrEmpty"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotNullOrEmpty(string? text, string name)
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            static Exception GetException(string? text, string name)
            {
                if (text is null)
                {
                    return new ArgumentNullException(name, $"Parameter {AssertString(name)} (string) must not be null or empty, was null.");
                }

                return new ArgumentException($"Parameter {AssertString(name)} (string) must not be null or empty, was empty.", name);
            }

            throw GetException(text, name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNullOrWhitespace"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNullOrWhiteSpace(string? text, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must be null or whitespace, was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotNullOrWhitespace"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotNullOrWhiteSpace(string? text, string name)
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            static Exception GetException(string? text, string name)
            {
                if (text is null)
                {
                    return new ArgumentNullException(name, $"Parameter {AssertString(name)} (string) must not be null or whitespace, was null.");
                }

                return new ArgumentException($"Parameter {AssertString(name)} (string) must not be null or whitespace, was whitespace.", name);
            }

            throw GetException(text, name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsEmpty"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsEmpty(string text, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must be empty, was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotEmpty"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotEmpty(string text, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must not be empty.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsWhitespace"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsWhiteSpace(string text, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must be whitespace, was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotWhitespace"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotWhiteSpace(string text, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must not be whitespace, was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasSizeEqualTo(string,int,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasSizeEqualTo(string text, int size, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size equal to {size}, had a size of {text.Length} and was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasSizeNotEqualTo"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasSizeNotEqualTo(string text, int size, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must not have a size equal to {size}, was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasSizeGreaterThan"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasSizeGreaterThan(string text, int size, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size over {size}, had a size of {text.Length} and was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasSizeGreaterThanOrEqualTo"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasSizeGreaterThanOrEqualTo(string text, int size, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size of at least {size}, had a size of {text.Length} and was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasSizeLessThan"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasSizeLessThan(string text, int size, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size less than {size}, had a size of {text.Length} and was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasSizeLessThanOrEqualTo(string,int,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo(string text, int size, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} (string) must have a size less than or equal to {size}, had a size of {text.Length} and was {AssertString(text)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasSizeEqualTo(string,string,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasSizeEqualTo(string source, string destination, string name)
        {
            throw new ArgumentException($"The source {AssertString(name)} (string) must have a size equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasSizeLessThanOrEqualTo(string,string,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasSizeLessThanOrEqualTo(string source, string destination, string name)
        {
            throw new ArgumentException($"The source {AssertString(name)} (string) must have a size less than or equal to {AssertString(destination.Length)} (the destination), had a size of {AssertString(source.Length)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="IsInRangeFor(int,string,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeExceptionForIsInRangeFor(int index, string text, string name)
        {
            throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must be in the range given by <0> and {AssertString(text.Length)} to be a valid index for the target string, was {AssertString(index)}.");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> when <see cref="IsNotInRangeFor(int,string,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentOutOfRangeExceptionForIsNotInRangeFor(int index, string text, string name)
        {
            throw new ArgumentOutOfRangeException(name, index, $"Parameter {AssertString(name)} (int) must not be in the range given by <0> and {AssertString(text.Length)} to be an invalid index for the target string, was {AssertString(index)}.");
        }
    }
}

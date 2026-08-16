// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <inheritdoc/>
    partial class ThrowHelper
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when an enum value is not defined.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForEnumNotDefined<TEnum>(TEnum value, string name)
            where TEnum : struct, Enum
        {
            throw new ArgumentException($"The value of enum {typeof(TEnum).Name} ({AssertString(value)}) is not defined.", name);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET6_0_OR_GREATER

namespace System.Runtime.CompilerServices;

/// <summary>
/// Helper methods for the <see cref="ConditionalWeakTable{TKey, TValue}"/> type.
/// </summary>
internal static class ConditionalWeakTableExtensions
{
    /// <summary>
    /// Tries to add a new pair to the table.
    /// </summary>
    /// <typeparam name="TKey">Tke key of items to store in the table.</typeparam>
    /// <typeparam name="TValue">The values to store in the table.</typeparam>
    /// <param name="table">The input <see cref="ConditionalWeakTable{TKey, TValue}"/> instance to modify.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to associate with key.</param>
    public static bool TryAdd<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key, TValue value)
        where TKey : class
        where TValue : class?
    {
        // There is no way to do this on .NET Standard 2.0 or 2.1 without exception handling
        try
        {
            table.Add(key, value);

            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}

#endif
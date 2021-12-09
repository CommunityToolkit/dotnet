// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.Generic;

/// <summary>
/// An interface providing key type contravariant and value type covariant access
/// to a <see cref="Dictionary2{TKey,TValue}"/> instance.
/// </summary>
/// <typeparam name="TKey">The contravariant type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The covariant type of values in the dictionary.</typeparam>
internal interface IDictionary2<in TKey, out TValue> : IDictionary2<TKey>
    where TKey : IEquatable<TKey>
    where TValue : class?
{
    /// <summary>
    /// Gets the value with the specified key.
    /// </summary>
    /// <param name="key">The key to look for.</param>
    /// <returns>The returned value.</returns>
    /// <exception cref="ArgumentException">Thrown if the key wasn't present.</exception>
    TValue this[TKey key] { get; }
}

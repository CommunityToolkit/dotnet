// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

namespace System.Collections.Generic;

/// <summary>
/// An interface providing key type contravariant access to a <see cref="Dictionary2{TKey,TValue}"/> instance.
/// </summary>
/// <typeparam name="TKey">The contravariant type of keys in the dictionary.</typeparam>
internal interface IDictionary2<in TKey> : IDictionary2
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Tries to remove a value with a specified key, if present.
    /// </summary>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>Whether or not the key was present.</returns>
    bool TryRemove(TKey key);
}

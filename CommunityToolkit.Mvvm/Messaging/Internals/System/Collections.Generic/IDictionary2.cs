// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.Generic;

/// <summary>
/// A base interface masking <see cref="Dictionary2{TKey,TValue}"/> instances and exposing non-generic functionalities.
/// </summary>
internal interface IDictionary2
{
    /// <summary>
    /// Gets the count of entries in the dictionary.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Clears the current dictionary.
    /// </summary>
    void Clear();
}

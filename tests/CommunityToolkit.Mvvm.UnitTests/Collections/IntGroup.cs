// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace CommunityToolkit.Mvvm.UnitTests;

/// <summary>
/// A simple <see cref="IGrouping{TKey, TElement}"/> implementation for <see cref="string"/> and <see cref="int"/> values.
/// </summary>
internal sealed class IntGroup : List<int>, IGrouping<string, int>
{
    /// <summary>
    /// Creates a new <see cref="IntGroup"/> instance with the specified parameters.
    /// </summary>
    /// <param name="key">The group key.</param>
    /// <param name="collection">The group values.</param>
    public IntGroup(string key, IEnumerable<int> collection)
        : base(collection)
    {
        Key = key;
    }

    /// <inheritdoc/>
    public string Key { get; }
}

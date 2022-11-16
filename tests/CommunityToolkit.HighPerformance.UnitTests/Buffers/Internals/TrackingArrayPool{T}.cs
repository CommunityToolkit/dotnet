// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System.Buffers;
using System.Collections.Generic;

namespace CommunityToolkit.HighPerformance.UnitTests.Buffers.Internals;

internal sealed class TrackingArrayPool<T> : ArrayPool<T>
{
    private readonly ArrayPool<T> pool = Create();

    private readonly HashSet<T[]> arrays = new();

    /// <summary>
    /// Gets the collection of currently rented out arrays
    /// </summary>
    public IReadOnlyCollection<T[]> RentedArrays => this.arrays;

    /// <inheritdoc/>
    public override T[] Rent(int minimumLength)
    {
        T[] array = this.pool.Rent(minimumLength);

        _ = this.arrays.Add(array);

        return array;
    }

    /// <inheritdoc/>
    public override void Return(T[] array, bool clearArray = false)
    {
        _ = this.arrays.Remove(array);

        this.pool.Return(array, clearArray);
    }
}

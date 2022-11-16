// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;

namespace CommunityToolkit.HighPerformance.Helpers;

/// <inheritdoc/>
partial class ParallelHelper
{
    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> when an invalid parameter is specified for the minimum actions per thread.
    /// </summary>
    private static void ThrowArgumentOutOfRangeExceptionForInvalidMinimumActionsPerThread()
    {
        // Having the argument name here manually typed is
        // not ideal, but this way we save passing that string as
        // a parameter, since it's always the same anyway.
        // Same goes for the other helper methods below.
        throw new ArgumentOutOfRangeException(
            "minimumActionsPerThread",
            "Each thread needs to perform at least one action");
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> when an invalid start parameter is specified for 1D loops.
    /// </summary>
    private static void ThrowArgumentOutOfRangeExceptionForStartGreaterThanEnd()
    {
        throw new ArgumentOutOfRangeException("start", "The start parameter must be less than or equal to end");
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when a range has an index starting from an end.
    /// </summary>
    private static void ThrowArgumentExceptionForRangeIndexFromEnd(string name)
    {
        throw new ArgumentException("The bounds of the range can't start from an end", name);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> when an invalid top parameter is specified for 2D loops.
    /// </summary>
    private static void ThrowArgumentOutOfRangeExceptionForTopGreaterThanBottom()
    {
        throw new ArgumentOutOfRangeException("top", "The top parameter must be less than or equal to bottom");
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> when an invalid left parameter is specified for 2D loops.
    /// </summary>
    private static void ThrowArgumentOutOfRangeExceptionForLeftGreaterThanRight()
    {
        throw new ArgumentOutOfRangeException("left", "The left parameter must be less than or equal to right");
    }
}

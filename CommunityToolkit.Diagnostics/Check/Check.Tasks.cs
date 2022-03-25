// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Check
{
    /// <summary>
    /// Checks that the input <see cref="Task"/> instance is in a completed state.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> is in a completed state, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompleted(Task task)
    {
        return task.IsCompleted;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance is not in a completed state.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> is not in a completed state, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotCompleted(Task task)
    {
        return !task.IsCompleted;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance has been completed successfully.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> has been completed successfully, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompletedSuccessfully(Task task)
    {
        return task.Status == TaskStatus.RanToCompletion;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance has not been completed successfully.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> has not been completed successfully, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotCompletedSuccessfully(Task task)
    {
        return task.Status != TaskStatus.RanToCompletion;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance is faulted.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> is faulted, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFaulted(Task task)
    {
        return task.IsFaulted;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance is not faulted.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> is not faulted, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotFaulted(Task task)
    {
        return !task.IsFaulted;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance is canceled.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> is canceled, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCanceled(Task task)
    {
        return task.IsCanceled;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance is not canceled.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> is not canceled, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotCanceled(Task task)
    {
        return !task.IsCanceled;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance has a specific status.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="status">The task status that is accepted.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> is in state <paramref name="status"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasStatusEqualTo(Task task, TaskStatus status)
    {
        return task.Status == status;
    }

    /// <summary>
    /// Checks that the input <see cref="Task"/> instance has not a specific status.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="status">The task status that is accepted.</param>
    /// <returns><see langword="true"/> if <paramref name="task"/> is not in state <paramref name="status"/>, <see langword="false"/> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasStatusNotEqualTo(Task task, TaskStatus status)
    {
        return task.Status != status;
    }
}

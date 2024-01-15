// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class FGuard
{
    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance is in a completed state.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that is in a completed state.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is not in a completed state.</exception>
    /// <seealso cref="Task.IsCompleted"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task IsCompleted(Task task, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.IsCompleted(task, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance is not in a completed state.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that is not in a completed state.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is in a completed state.</exception>
    /// <seealso cref="Task.IsCompleted"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task IsNotCompleted(Task task, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.IsNotCompleted(task, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance has been completed successfully.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that is completed successfully.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> has not been completed successfully.</exception>
    /// <seealso cref="Task.Status"/>
    /// <seealso cref="TaskStatus.RanToCompletion"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task IsCompletedSuccessfully(Task task, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.IsCompletedSuccessfully(task, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance has not been completed successfully.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that is not completed successfully.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> has been completed successfully.</exception>
    /// <seealso cref="Task.Status"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task IsNotCompletedSuccessfully(Task task, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.IsNotCompletedSuccessfully(task, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance is faulted.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that is faulted.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is not faulted.</exception>
    /// <seealso cref="Task.IsFaulted"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task IsFaulted(Task task, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.IsFaulted(task, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance is not faulted.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that is not faulted.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is faulted.</exception>
    /// <seealso cref="Task.IsFaulted"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task IsNotFaulted(Task task, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.IsNotFaulted(task, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance is canceled.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that is canceled.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is not canceled.</exception>
    /// <seealso cref="Task.IsCanceled"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task IsCanceled(Task task, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.IsCanceled(task, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance is not canceled.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that is not canceled.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is canceled.</exception>
    /// <seealso cref="Task.IsCanceled"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task IsNotCanceled(Task task, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.IsNotCanceled(task, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance has a specific status.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="status">The task status that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that has a specific <paramref name="status"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> doesn't match <paramref name="status"/>.</exception>
    /// <seealso cref="Task.Status"/>
    /// <seealso cref="TaskStatus"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task HasStatusEqualTo(Task task, TaskStatus status, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.HasStatusEqualTo(task, status, name);
        return task;
    }

    /// <summary>
    /// Asserts that the input <see cref="Task"/> instance has not a specific status.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> instance to test.</param>
    /// <param name="status">The task status that is accepted.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <returns>The <paramref name="task"/> that has not a specific <paramref name="status"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> matches <paramref name="status"/>.</exception>
    /// <seealso cref="Task.Status"/>
    /// <seealso cref="TaskStatus"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task HasStatusNotEqualTo(Task task, TaskStatus status, [CallerArgumentExpression(nameof(task))] string name = "")
    {
        Guard.HasStatusNotEqualTo(task, status, name);
        return task;
    }
}

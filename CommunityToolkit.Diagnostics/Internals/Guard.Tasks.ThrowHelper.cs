// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <inheritdoc/>
    private static partial class ThrowHelper
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsCompleted"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsCompleted(Task task, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must be completed, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotCompleted"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotCompleted(Task task, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must not be completed, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsCompletedSuccessfully"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsCompletedSuccessfully(Task task, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must be completed successfully, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotCompletedSuccessfully"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotCompletedSuccessfully(Task task, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must not be completed successfully, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsFaulted"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsFaulted(Task task, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must be faulted, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotFaulted"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotFaulted(Task task, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must not be faulted, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsCanceled"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsCanceled(Task task, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must be canceled, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotCanceled"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotCanceled(Task task, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must not be canceled, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasStatusEqualTo"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasStatusEqualTo(Task task, TaskStatus status, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must have status {status}, had status {AssertString(task.Status)}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="HasStatusNotEqualTo"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForHasStatusNotEqualTo(Task task, TaskStatus status, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({task.GetType().ToTypeString()}) must not have status {AssertString(status)}.", name);
        }
    }
}

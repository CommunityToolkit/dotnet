// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommunityToolkit.Mvvm.ComponentModel.__Internals;

/// <summary>
/// An internal helper used to support <see cref="ObservableObject"/> and generated code from its template.
/// This type is not intended to be used directly by user code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete("This type is not intended to be used directly by user code")]
public static class __TaskExtensions
{
    /// <summary>
    /// Gets an awaitable object that skips end validation.
    /// </summary>
    /// <param name="task">The input <see cref="Task"/> to get the awaitable for.</param>
    /// <returns>A <see cref="TaskAwaitableWithoutEndValidation"/> object wrapping <paramref name="task"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This method is not intended to be called directly by user code")]
    public static TaskAwaitableWithoutEndValidation GetAwaitableWithoutEndValidation(this Task task)
    {
        return new(task);
    }

    /// <summary>
    /// A custom task awaitable object that skips end validation.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This type is not intended to be called directly by user code")]
    public readonly struct TaskAwaitableWithoutEndValidation
    {
        /// <summary>
        /// The wrapped <see cref="Task"/> instance to create an awaiter for.
        /// </summary>
        private readonly Task task;

        /// <summary>
        /// Creates a new <see cref="TaskAwaitableWithoutEndValidation"/> instance with the specified parameters.
        /// </summary>
        /// <param name="task">The wrapped <see cref="Task"/> instance to create an awaiter for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TaskAwaitableWithoutEndValidation(Task task)
        {
            this.task = task;
        }

        /// <summary>
        /// Gets an <see cref="Awaiter"/> instance for the current underlying task.
        /// </summary>
        /// <returns>An <see cref="Awaiter"/> instance for the current underlying task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter()
        {
            return new(this.task);
        }

        /// <summary>
        /// An awaiter object for <see cref="TaskAwaitableWithoutEndValidation"/>.
        /// </summary>
        public readonly struct Awaiter : ICriticalNotifyCompletion
        {
            /// <summary>
            /// The underlying <see cref="TaskAwaiter"/> instance.
            /// </summary>
            private readonly TaskAwaiter taskAwaiter;

            /// <summary>
            /// Creates a new <see cref="Awaiter"/> instance with the specified parameters.
            /// </summary>
            /// <param name="task">The wrapped <see cref="Task"/> instance to create an awaiter for.</param>
            public Awaiter(Task task)
            {
                this.taskAwaiter = task.GetAwaiter();
            }

            /// <summary>
            /// Gets whether the operation has completed or not.
            /// </summary>
            /// <remarks>This property is intended for compiler user rather than use directly in code.</remarks>
            public bool IsCompleted
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => taskAwaiter.IsCompleted;
            }

            /// <summary>
            /// Ends the await operation.
            /// </summary>
            /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult()
            {
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                this.taskAwaiter.OnCompleted(continuation);
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation)
            {
                this.taskAwaiter.UnsafeOnCompleted(continuation);
            }
        }
    }
}

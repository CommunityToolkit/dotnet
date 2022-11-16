// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommunityToolkit.Mvvm.Messaging.Messages;

/// <summary>
/// A <see langword="class"/> for async request messages, which can either be used directly or through derived classes.
/// </summary>
/// <typeparam name="T">The type of request to make.</typeparam>
public class AsyncRequestMessage<T>
{
    private Task<T>? response;

    /// <summary>
    /// Gets the message response.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="HasReceivedResponse"/> is <see langword="false"/>.</exception>
    public Task<T> Response
    {
        get
        {
            if (!HasReceivedResponse)
            {
                ThrowInvalidOperationExceptionForNoResponseReceived();
            }

            return this.response!;
        }
    }

    /// <summary>
    /// Gets a value indicating whether a response has already been assigned to this instance.
    /// </summary>
    public bool HasReceivedResponse { get; private set; }

    /// <summary>
    /// Replies to the current request message.
    /// </summary>
    /// <param name="response">The response to use to reply to the request message.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="Response"/> has already been set.</exception>
    public void Reply(T response)
    {
        Reply(Task.FromResult(response));
    }

    /// <summary>
    /// Replies to the current request message.
    /// </summary>
    /// <param name="response">The response to use to reply to the request message.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="response"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="Response"/> has already been set.</exception>
    public void Reply(Task<T> response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (HasReceivedResponse)
        {
            ThrowInvalidOperationExceptionForDuplicateReply();
        }

        HasReceivedResponse = true;

        this.response = response;
    }

    /// <inheritdoc cref="Task{T}.GetAwaiter"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TaskAwaiter<T> GetAwaiter()
    {
        return this.Response.GetAwaiter();
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> when a response is not available.
    /// </summary>
    [DoesNotReturn]
    private static void ThrowInvalidOperationExceptionForNoResponseReceived()
    {
        throw new InvalidOperationException("No response was received for the given request message.");
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> when <see cref="Reply(T)"/> or <see cref="Reply(Task{T})"/> are called twice.
    /// </summary>
    [DoesNotReturn]
    private static void ThrowInvalidOperationExceptionForDuplicateReply()
    {
        throw new InvalidOperationException("A response has already been issued for the current message.");
    }
}

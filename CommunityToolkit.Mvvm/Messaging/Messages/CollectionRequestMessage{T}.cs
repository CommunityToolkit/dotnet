// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Mvvm.Messaging.Messages;

/// <summary>
/// A <see langword="class"/> for request messages that can receive multiple replies, which can either be used directly or through derived classes.
/// </summary>
/// <typeparam name="T">The type of request to make.</typeparam>
public class CollectionRequestMessage<T> : IEnumerable<T>
{
    private readonly List<T> responses = new();

    /// <summary>
    /// Gets the message responses.
    /// </summary>
    public IReadOnlyCollection<T> Responses => this.responses;

    /// <summary>
    /// Replies to the current request message.
    /// </summary>
    /// <param name="response">The response to use to reply to the request message.</param>
    public void Reply(T response)
    {
        this.responses.Add(response);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerator<T> GetEnumerator()
    {
        return this.responses.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

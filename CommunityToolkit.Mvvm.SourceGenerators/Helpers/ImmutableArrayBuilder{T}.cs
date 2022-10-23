// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;

namespace CommunityToolkit.Mvvm.SourceGenerators.Helpers;

/// <summary>
/// A helper type to build <see cref="ImmutableArray{T}"/> instances with pooled buffers.
/// </summary>
/// <typeparam name="T">The type of items to create arrays for.</typeparam>
internal ref struct ImmutableArrayBuilder<T>
{
    /// <summary>
    /// The shared <see cref="ObjectPool{T}"/> instance to share <see cref="ImmutableArray{T}.Builder"/> objects.
    /// </summary>
    private static readonly ObjectPool<ImmutableArray<T>.Builder> sharedObjectPool = new(ImmutableArray.CreateBuilder<T>);

    /// <summary>
    /// The rented <see cref="ImmutableArray{T}.Builder"/> instance to use.
    /// </summary>
    private ImmutableArray<T>.Builder? builder;

    /// <summary>
    /// Rents a new pooled <see cref="ImmutableArray{T}.Builder"/> instance through a new <see cref="ImmutableArrayBuilder{T}"/> value.
    /// </summary>
    /// <returns>A <see cref="ImmutableArrayBuilder{T}"/> to interact with the underlying <see cref="ImmutableArray{T}.Builder"/> instance.</returns>
    public static ImmutableArrayBuilder<T> Rent()
    {
        return new(sharedObjectPool.Allocate());
    }

    /// <summary>
    /// Creates a new <see cref="ImmutableArrayBuilder{T}"/> object with the specified parameters.
    /// </summary>
    /// <param name="builder"></param>
    private ImmutableArrayBuilder(ImmutableArray<T>.Builder builder)
    {
        this.builder = builder;
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Count"/>
    public readonly int Count
    {
        get => this.builder!.Count;
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Add(T)"/>
    public readonly void Add(T item)
    {
        this.builder!.Add(item);
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.ToImmutable"/>
    public readonly ImmutableArray<T> ToImmutable()
    {
        return this.builder!.ToImmutable();
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.ToArray"/>
    public readonly T[] ToArray()
    {
        return this.builder!.ToArray();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ImmutableArray<T>.Builder? builder = this.builder;

        this.builder = null;

        if (builder is not null)
        {
            builder.Clear();

            sharedObjectPool.Free(builder);
        }
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;

#pragma warning disable CS0618

namespace CommunityToolkit.Mvvm.SourceGenerators.Helpers;

/// <summary>
/// A helper class to build <see cref="ImmutableArray{T}"/> instances with pooled buffers.
/// </summary>
/// <typeparam name="T">The type of items to create arrays for.</typeparam>
internal static class ImmutableArrayBuilder<T>
{
    /// <summary>
    /// The shared <see cref="ObjectPool{T}"/> instance to share <see cref="ImmutableArray{T}.Builder"/> objects.
    /// </summary>
    private static readonly ObjectPool<ImmutableArray<T>.Builder> objectPool = new(ImmutableArray.CreateBuilder<T>);

    /// <summary>
    /// Rents a new pooled <see cref="ImmutableArray{T}.Builder"/> instance through a <see cref="Lease"/> value.
    /// </summary>
    /// <returns>A <see cref="Lease"/> to interact with the underlying <see cref="ImmutableArray{T}.Builder"/> instance.</returns>
    public static Lease Rent()
    {
        return new(objectPool, objectPool.Allocate());
    }

    /// <summary>
    /// A wrapper for a pooled <see cref="ImmutableArray{T}.Builder"/> instance.
    /// </summary>
    public ref struct Lease
    {
        /// <summary>
        /// The owner <see cref="ObjectPool{T}"/> instance.
        /// </summary>
        private readonly ObjectPool<ImmutableArray<T>.Builder> objectPool;

        /// <summary>
        /// The rented <see cref="ImmutableArray{T}.Builder"/> instance to use.
        /// </summary>
        private ImmutableArray<T>.Builder? builder;

        /// <summary>
        /// Creates a new <see cref="Lease"/> object with the specified parameters.
        /// </summary>
        /// <param name="objectPool"></param>
        /// <param name="builder"></param>
        [Obsolete("Don't create instances of this type manually, use ImmutableArrayBuilder<T>.Rent() instead.")]
        public Lease(ObjectPool<ImmutableArray<T>.Builder> objectPool, ImmutableArray<T>.Builder builder)
        {
            this.objectPool = objectPool;
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

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            ImmutableArray<T>.Builder? builder = this.builder;

            this.builder = null;

            if (builder is not null)
            {
                builder.Clear();

                this.objectPool.Free(builder);
            }
        }
    }
}
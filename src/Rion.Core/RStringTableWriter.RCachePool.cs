// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Generic;
using System.Threading;

using Rion.Core.Buffers;
using Rion.Core.Internal;

namespace Rion.Core;

public partial class RStringTableWriter
{
    /// <summary>
    /// Manages a pool of <see cref="RBufferWriter"/> instances specifically for <see cref="RStringTableWriter"/>
    /// operations, promoting efficient recycling and resource management to enhance performance and minimize
    /// memory allocation overhead during string table population.
    /// </summary>
    private sealed class RBufferWriterCachePool : RCachePool<RBufferWriter>
    {
        /// <summary>
        /// Gets a shared <see cref="RBufferWriterCachePool" /> instance.
        /// </summary>
        public static RBufferWriterCachePool Shared { get; } = new();

        /// <inheritdoc />
        public override void Reset(RBufferWriter obj) => obj.Reset();

        /// <inheritdoc />
        public override RBufferWriter CreateDefault() => new(1 << 22);
    }

    /// <summary>
    /// Manages a cache of <see cref="Dictionary{TKey, TValue}"/> instances optimized for interning dictionaries,
    /// specifically tailored for efficient reuse within string table operations. This class ensures that
    /// memory is effectively managed by recycling and cleaning up dictionary objects to minimize allocation overhead.
    /// </summary>
    private sealed class InternDictCachePool : RCachePool<Dictionary<string, int>>
    {
        /// <summary>
        /// Gets a shared <see cref="InternDictCachePool" /> instance.
        /// </summary>
        public static InternDictCachePool Shared { get; } = new();

        /// <inheritdoc />
        public override void Reset(Dictionary<string, int> obj) => obj.Clear();

        /// <inheritdoc />
        public override Dictionary<string, int> CreateDefault() => new(ushort.MaxValue);
    }

    /// <summary>
    /// Provides a generic mechanism for managing a pool of cache objects to enhance performance and efficiency by reusing instances of a specified type <typeparamref name="T"/>.
    /// This class is designed to minimize memory allocation and deallocation overhead byrenting and returning objects from/to the cache pool.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by this cache pool. Must be a reference type.</typeparam>
    private abstract class RCachePool<T> where T : class
    {
        /// <summary>
        /// A private collection that maintains weak references to cached objects of type <typeparamref name="T"/>.
        /// These objects are managed by the <see cref="RCachePool{T}"/> to facilitate efficient reuse and reduce memory allocations.
        /// </summary>
        private readonly List<WeakReference> _cachedObjects = [];

        /// <summary>
        /// A thread lock used to synchronize access to the cache pool, ensuring thread safety during rental and return operations.
        /// </summary>
        private readonly Lock _lock = new();

        /// <summary>
        /// Creates a default instance of the managed object type <typeparamref name="T"/>.
        /// This method should be overridden by derived classes to provide a custom default instance creation logic.
        /// </summary>
        /// <returns>A new instance of type <typeparamref name="T"/> that serves as the default for the cache pool.</returns>
        public virtual T CreateDefault() => Nothing.ToDo<T>();

        /// <summary>
        /// Resets the specified <paramref name="obj"/>, an instance of <typeparamref name="T"/>, to its initial state.
        /// The exact behavior of this method depends on the type <typeparamref name="T"/>.
        /// It is intended to be overridden by derived classes to provide specific reset logic for each managed type.
        /// </summary>
        /// <param name="obj">The object instance to reset.</param>
        public virtual void Reset(T obj) => Nothing.ToDo();

        /// <summary>
        /// Retrieves a cache object from the pool for reuse.
        /// </summary>
        /// <returns>A reusable instance of <typeparamref name="T"/> taken from the cache pool.</returns>
        public T Rent()
        {
            using (_lock.EnterScope())
            {
                CacheWrap? cache;
                foreach (var t in _cachedObjects)
                {
                    cache = t.Target as CacheWrap;
                    if (cache is not { IsBusy: false })
                        continue;

                    cache.IsBusy = true;
                    return cache.CacheObj;
                }

                cache = new CacheWrap(CreateDefault());
                _cachedObjects.Add(new WeakReference(cache));
                return cache.CacheObj;
            }
        }

        /// <summary>
        /// Releases the provided cache object back into the pool for reuse.
        /// </summary>
        /// <param name="obj">The cache object to be returned to the pool. It must be an instance that was previously rented from this or a compatible pool.</param>
        public void Return(T obj)
        {
            using (_lock.EnterScope())
            {
                CacheWrap? cache;
                foreach (var t in _cachedObjects)
                {
                    cache = t.Target as CacheWrap;
                    if (cache is null || cache.CacheObj != obj)
                        continue;

                    Reset(cache.CacheObj);
                    cache.IsBusy = false;
                    break;
                }

                // Clear
                for (var i = 0; i < _cachedObjects.Count; i++)
                {
                    cache = _cachedObjects[i].Target as CacheWrap;
                    if (cache is null)
                        _cachedObjects.RemoveAt(i--);
                }
            }
        }

        /// <summary>
        /// Represents a wrapper for cache objects, tracking their usage status to facilitate efficient management within a cache pool.
        /// </summary>
        private sealed class CacheWrap(T obj)
        {
            /// <summary>
            /// Represents the cached object within the <see cref="CacheWrap"/> which is managed by the <see cref="RCachePool{T}"/> to optimize performance by reusing instances.
            /// </summary>
            internal T CacheObj { get; } = obj;

            /// <summary>
            /// Indicates whether the cache object is currently in use.
            /// </summary>
            /// <value><c>true</c> if the cache object is busy; otherwise, <c>false</c>.</value>
            internal bool IsBusy { get; set; }
        }
    }
}

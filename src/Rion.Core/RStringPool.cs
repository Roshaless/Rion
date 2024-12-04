// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Rion.Core.Internal;

namespace Rion.Core;

/// <summary>
/// Manages a pool of strings to optimize memory usage and access efficiency by caching string instances using weak references.
/// It allows for efficient string retrieval and automatic memory management through garbage collection when strings are no longer in use.
/// </summary>
public static class RStringPool
{
    /// <summary>
    /// A dictionary that caches strings as weak references to optimize memory usage.
    /// It enables efficient retrieval of strings and automatically cleans up unreferenced entries.
    /// </summary>
    private static readonly Dictionary<ulong, WeakReference> s_stringCache = new(ushort.MaxValue)
    {
        // Empty
        { XxHash3.HashToUInt64(default), new WeakReference(string.Empty, true) }
    };

    /// <summary>
    /// Retrieves a string from the <see cref="RStringPool"/> based on the provided byte span. If the string does not exist,
    /// it is added to the pool before being returned.
    /// </summary>
    /// <param name="span">The byte span representing the string to retrieve or add.</param>
    /// <returns>The cached or newly added string corresponding to the byte span.</returns>
    public static string GetOrAdd(ReadOnlySpan<byte> span)
    {
        var stringHash = XxHash3.HashToUInt64(span);
        ref var stringCache = ref CollectionsMarshal.GetValueRefOrAddDefault(s_stringCache, stringHash, out var isExists);

        string? result;
        if (isExists)
        {
            Debug.Assert(stringCache is not null);
            result = stringCache.Target as string;
            if (result is not null)
                return result;
        }

        result = StringHelper.CreateStringFromBytes(span);
        s_stringCache[stringHash] = new WeakReference(result);
        return result;
    }

    /// <summary>
    /// Attempts to retrieve a string from the <see cref="RStringPool"/> based on the provided byte span, without adding it if it's not found.
    /// </summary>
    /// <param name="span">The byte span representing the string to retrieve.</param>
    /// <param name="result">When this method returns, contains the cached string if found; otherwise, null. This parameter is passed uninitialized.</param>
    /// <returns>true if the string was found in the pool; otherwise, false.</returns>
    public static bool TryGet(ReadOnlySpan<byte> span, [NotNullWhen(true)] out string? result)
    {
        ref var stringCache = ref CollectionsMarshal.GetValueRefOrNullRef(s_stringCache, XxHash3.HashToUInt64(span));
        if (Unsafe.IsNullRef(ref stringCache))
        {
            result = null;
            return false;
        }

        Debug.Assert(stringCache is not null);
        result = stringCache.Target as string;

        return result is not null;
    }

    /// <summary>
    /// Performs cleanup of the string cache by removing weak references to collected strings and ensures the auto-cleanup instance is present.
    /// This method should be called periodically to maintain the string pool's efficiency.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Cleanup()
    {
        foreach (var pair in s_stringCache)
        {
            if (pair.Value is { Target: not null })
            {
                s_stringCache.Remove(pair.Key);
            }
        }
    }
}

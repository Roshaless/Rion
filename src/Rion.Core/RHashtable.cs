// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Rion.Core.Hashing;

namespace Rion.Core;

/// <summary>
/// Represents a hashtable used for string tables.
/// </summary>
public static class RHashtable
{
    /// <summary>
    /// The loaded hashtable.
    /// </summary>
    private static ImmutableDictionary<ulong, string> s_loadedHashtable = ImmutableDictionary<ulong, string>.Empty;

    /// <summary>
    /// Get the name of a hash or its hex representation if the name is not found.
    /// </summary>
    /// <param name="hash">The hash to look up.</param>
    /// <returns>The name of the hash or its hex representation if the name is not found.</returns>
    public static string GetNameOrHexString(ulong hash)
    {
        return s_loadedHashtable.TryGetValue(hash, out var name) ? name : $"{{{hash:x}}}";
    }

    /// <summary>
    /// Load strings from a collection into the hashtable using a specific hash algorithm.
    /// </summary>
    /// <param name="strings">The collection of strings to load.</param>
    /// <param name="hashAlgorithm">The hash algorithm to use.</param>
    /// <typeparam name="T">The type of the collection.</typeparam>
    public static void LoadFromStrings<T>(T strings, IRSTHashAlgorithm? hashAlgorithm = null) where T : IEnumerable<string>
    {
        if (!strings.Any()) return;
        hashAlgorithm ??= RSTHashAlgorithm.Latest;

        s_loadedHashtable = s_loadedHashtable.AddRange(GetStringNames(strings)
            .Distinct().Select(x => KeyValuePair.Create(hashAlgorithm.Hash(x), x)));
    }

    /// <summary>
    /// Get the names of strings in a collection.
    /// </summary>
    /// <param name="strings">The collection of strings.</param>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <returns>The names of the strings.</returns>
    private static IEnumerable<string> GetStringNames<T>(T strings) where T : IEnumerable<string>
    {
#pragma warning disable format
        return from line in strings let index = line.IndexOf(' ')
               select index == -1 ? line : line[(index + 1)..]
               into hashName select hashName.ToLowerInvariant();
#pragma warning restore format
    }
}

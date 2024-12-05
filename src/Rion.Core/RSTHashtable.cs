// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

using Rion.Core.Hashing;

namespace Rion.Core;

/// <summary>
/// Represents a hashtable used for string tables.
/// </summary>
public static class RSTHashtable
{
    /// <summary>
    /// A list of loaded hashtable.
    /// </summary>
    private static readonly List<ImmutableDictionary<ulong, string>> s_loadedHashtable = [];

    /// <summary>
    /// Get the name of a hash or its hex representation if the name is not found.
    /// </summary>
    /// <param name="hash">The hash to look up.</param>
    /// <returns>The name of the hash or its hex representation if the name is not found.</returns>
    public static string GetNameOrHexString(ulong hash)
    {
        foreach (var hashtable in CollectionsMarshal.AsSpan(s_loadedHashtable))
        {
            if (hashtable.TryGetValue(hash, out var name))
            {
                return name;
            }
        }

        return hash.ToString("x");
    }

    /// <summary>
    /// Load strings from a collection into the hashtable.
    /// </summary>
    /// <param name="strings">The collection of strings to load.</param>
    /// <typeparam name="T">The type of the collection.</typeparam>
    public static void LoadFromStrings<T>(IEnumerable<string> strings)
        where T : IEnumerable<string> => LoadFromStrings(strings, RSTHashAlgorithm.Latest);

    /// <summary>
    /// Load strings from a collection into the hashtable using a specific hash algorithm.
    /// </summary>
    /// <param name="strings">The collection of strings to load.</param>
    /// <param name="hashAlgorithm">The hash algorithm to use.</param>
    /// <typeparam name="T">The type of the collection.</typeparam>
    public static void LoadFromStrings<T>(T strings, [MaybeNull] IRSTHashAlgorithm hashAlgorithm) where T : IEnumerable<string>
    {
        if (!strings.Any()) return;
        hashAlgorithm ??= RSTHashAlgorithm.Latest;

        s_loadedHashtable.Add(GetStringNames(strings)
            .Distinct().ToImmutableDictionary(hashAlgorithm.Hash));
    }

    /// <summary>
    /// Get the names of strings in a collection.
    /// </summary>
    /// <param name="strings">The collection of strings.</param>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <returns>The names of the strings.</returns>
    private static IEnumerable<string> GetStringNames<T>(T strings) where T : IEnumerable<string>
    {
        return
            from line in strings let index = line.IndexOf(' ')
            select index == -1 ? line : line[(index + 1)..]
            into hashName select hashName.ToLowerInvariant();
    }
}

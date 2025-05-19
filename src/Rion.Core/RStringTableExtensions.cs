// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Rion.Core.Internal;
using Rion.Core.Metadata;
using Rion.Core.Metadata.Legacy;

namespace Rion.Core;

/// <summary>
/// Provides extension methods for <see cref="IRStringTable"/>.
/// </summary>
public static partial class RStringTableExtensions
{
    /// <summary>
    /// Converts the given collection into a <see cref="RStringTable"/> using the specified metadata.
    /// </summary>
    /// <param name="collection">The collection to convert.</param>
    /// <param name="metadata">The metadata to set.</param>
    /// <returns>A new <see cref="RStringTable"/> instance.</returns>
    public static RStringTable ToRStringTable(
        this IEnumerable<KeyValuePair<ulong, string>> collection,
        IRStringTableMetadata metadata) => RStringTable.Create(metadata, collection);

    /// <summary>
    /// Converts the given dictionary into a <see cref="RStringTable"/> using the specified metadata.
    /// </summary>
    /// <param name="dictionary">The dictionary to convert.</param>
    /// <param name="metadata">The metadata to set.</param>
    /// <returns>A new <see cref="RStringTable"/> instance.</returns>
    public static RStringTable ToRStringTable(
        this IDictionary<ulong, string> dictionary,
        IRStringTableMetadata metadata) => RStringTable.Create(metadata, dictionary);

    // ======================================================================================
    // ================= Add - <ulong, Any> =================================================
    // ======================================================================================

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static void Add<TSource, TValue>(
        this TSource source, ulong key, TValue value)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => source.Add(key, value.ToString(CultureInfo.CurrentCulture));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <param name="provider">The format provider to use.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static void Add<TSource, TValue>(
        this TSource source, ulong key, TValue value, IFormatProvider provider)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => source.Add(key, value.ToString(provider));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public static void Add<TSource>(
        this TSource source, ulong key, object? value)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => source.Add(key, value?.ToString() ?? string.Empty);

    // ======================================================================================
    // ================= Add - <string, Any> ================================================
    // ======================================================================================

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified name and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="name">The name to hash and use as the key.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public static void Add<TSource>(
        this TSource source, string name, string value)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => source.Add(NameToHash(source, name), value);

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified name and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="name">The name to hash and use as the key.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static void Add<TSource, TValue>(
        this TSource source, string name, TValue value)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => source.Add(NameToHash(source, name), value.ToString(CultureInfo.CurrentCulture));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified name and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="name">The name to hash and use as the key.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <param name="provider">The format provider to use.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static void Add<TSource, TValue>(
        this TSource source, string name, TValue value, IFormatProvider provider)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => source.Add(NameToHash(source, name), value.ToString(provider));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified name and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="name">The name to hash and use as the key.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public static void Add<TSource>(
        this TSource source, string name, object? value)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => source.Add(NameToHash(source, name), value?.ToString() ?? string.Empty);

    // ======================================================================================
    // ================= Add - KeyValuePair<ulong, Any> =====================================
    // ======================================================================================

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="pair">The key and value to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static void Add<TSource, TValue>(
        this TSource source, KeyValuePair<ulong, TValue> pair)
        where TSource : RStringTable, ICollection<KeyValuePair<ulong, string>> where TValue : IConvertible
        => source.Add(KeyValuePair.Create(pair.Key, pair.Value.ToString(CultureInfo.CurrentCulture)));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="pair">The key and value to add.</param>
    /// <param name="provider">The format provider to use.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static void Add<TSource, TValue>(
        this TSource source, KeyValuePair<ulong, TValue> pair, IFormatProvider provider)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>> where TValue : IConvertible
        => source.Add(KeyValuePair.Create(pair.Key, pair.Value.ToString(provider)));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="pair">The key and value to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public static void Add<TSource>(
        this TSource source, KeyValuePair<ulong, object> pair)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>>
        => source.Add(KeyValuePair.Create(pair.Key, pair.Value.ToString() ?? string.Empty));

    // ======================================================================================
    // ================= Add - KeyValuePair<string, Any> ====================================
    // ======================================================================================

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="pair">The key and value to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public static void Add<TSource>(
        this TSource source, KeyValuePair<string, string> pair)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>>
        => source.Add(KeyValuePair.Create(NameToHash(source, pair.Key), pair.Value));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="pair">The key and value to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static void Add<TSource, TValue>(
        this TSource source, KeyValuePair<string, TValue> pair)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>> where TValue : IConvertible
        => source.Add(KeyValuePair.Create(NameToHash(source, pair.Key), pair.Value.ToString(CultureInfo.CurrentCulture)));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="pair">The key and value to add.</param>
    /// <param name="provider">The format provider to use.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public static void Add<TSource, TValue>(
        this TSource source, KeyValuePair<string, TValue> pair, IFormatProvider provider)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>> where TValue : IConvertible
        => source.Add(KeyValuePair.Create(NameToHash(source, pair.Key), pair.Value.ToString(provider)));

    /// <summary>
    /// Adds a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="pair">The key and value to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public static void Add<TSource>(
        this TSource source, KeyValuePair<string, object> pair)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>>
        => source.Add(KeyValuePair.Create(NameToHash(source, pair.Key), pair.Value.ToString() ?? string.Empty));

    // ======================================================================================
    // ================= ContainsKey, Remove, and TryGetValue ===============================
    // ======================================================================================

    /// <summary>
    /// Determines whether the <see cref="IRStringTable"/> contains the specified name.
    /// </summary>
    /// <param name="source">The source to search.</param>
    /// <param name="name">The name to hash and lookup.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns><see langword="true"/> if the <see cref="IRStringTable"/> contains the specified name; otherwise, <see langword="false"/>.</returns>
    public static bool ContainsKey<TSource>(
        this TSource source, string name)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => source.ContainsKey(NameToHash(source, name));

    /// <summary>
    /// Removes the entry with the specified name from the <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="source">The source to remove the entry from.</param>
    /// <param name="name">The name to hash and remove.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns><see langword="true"/> if the entry was successfully removed; otherwise, <see langword="false"/>.</returns>
    public static bool Remove<TSource>(
        this TSource source, string name)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => source.Remove(NameToHash(source, name));

    /// <summary>
    /// Try to get the value associated with the specified name from the <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="source">The source to get the value from.</param>
    /// <param name="name">The name to hash and lookup.</param>
    /// <param name="value">The value associated with the specified name, if found; otherwise, <see langword="null"/>.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns><see langword="true"/> if the value was found; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetValue<TSource>(
        this TSource source, string name, [NotNullWhen(true)] out string? value)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => source.TryGetValue(NameToHash(source, name), out value);

    // ======================================================================================
    // ================= TryAdd - <ulong, Any> ==============================================
    // ======================================================================================

    /// <summary>
    /// Try to add a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns><see langword="true"/> if the entry was successfully added; otherwise, <see langword="false"/>.</returns>
    public static bool TryAdd<TSource, TValue>(
        this TSource source, ulong key, TValue value)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => CollectionExtensions.TryAdd(source, key, value.ToString(CultureInfo.CurrentCulture));

    /// <summary>
    /// Try to add a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <param name="provider">The format provider to use.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns><see langword="true"/> if the entry was successfully added; otherwise, <see langword="false"/>.</returns>
    public static bool TryAdd<TSource, TValue>(
        this TSource source, ulong key, TValue value, IFormatProvider provider)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => CollectionExtensions.TryAdd(source, key, value.ToString(provider));

    /// <summary>
    /// Try to add a new entry to the <see cref="IRStringTable"/> with the specified key and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns><see langword="true"/> if the entry was successfully added; otherwise, <see langword="false"/>.</returns>
    public static bool TryAdd<TSource>(
        this TSource source, ulong key, object? value)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => CollectionExtensions.TryAdd(source, key, value?.ToString() ?? string.Empty);

    // ======================================================================================
    // ================= TryAdd - <string, Any> =============================================
    // ======================================================================================

    /// <summary>
    /// Try to add a new entry to the <see cref="IRStringTable"/> with the specified name and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="name">The name to hash and add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns><see langword="true"/> if the entry was successfully added; otherwise, <see langword="false"/>.</returns>
    public static bool TryAdd<TSource, TValue>(
        this TSource source, string name, TValue value)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => CollectionExtensions.TryAdd(source, NameToHash(source, name), value.ToString(CultureInfo.CurrentCulture));

    /// <summary>
    /// Try to add a new entry to the <see cref="IRStringTable"/> with the specified name and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="name">The name to hash and add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <param name="provider">The format provider to use.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns><see langword="true"/> if the entry was successfully added; otherwise, <see langword="false"/>.</returns>
    public static bool TryAdd<TSource, TValue>(
        this TSource source, string name, TValue value, IFormatProvider provider)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => CollectionExtensions.TryAdd(source, NameToHash(source, name), value.ToString(provider));

    /// <summary>
    /// Try to add a new entry to the <see cref="IRStringTable"/> with the specified name and value.
    /// </summary>
    /// <param name="source">The source to add the entry to.</param>
    /// <param name="name">The name to hash and add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns><see langword="true"/> if the entry was successfully added; otherwise, <see langword="false"/>.</returns>
    public static bool TryAdd<TSource>(
        this TSource source, string name, object? value)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => CollectionExtensions.TryAdd(source, NameToHash(source, name), value?.ToString() ?? string.Empty);


    // ======================================================================================
    // ================= Custom useful extension methods ====================================
    // ======================================================================================

    /// <summary>
    /// Get the hash of the specified name.
    /// </summary>
    /// <param name="source">The source to get the hash from.</param>
    /// <param name="name">The name to hash.</param>
    /// <returns>The hash of the specified name.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong NameToHash(
        this IRStringTable source, string name)
        => source.Metadata.HashAlgorithm.Hash(name);

    /// <summary>
    /// Replace all occurrences of the specified old text with the specified new text.
    /// </summary>
    /// <param name="source">The source to replace the text in.</param>
    /// <param name="oldText">The text to replace.</param>
    /// <param name="newText">The text to replace with.</param>
    /// <param name="caseSensitive">Whether to be case-sensitive.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns>The source with the replaced text.</returns>
    public static TSource ReplaceAll<TSource>(this TSource source,
        string oldText, string? newText, bool caseSensitive = false)
        where TSource : IRStringTable, IDictionary<ulong, string>
    {
        ArgumentException.ThrowIfNullOrEmpty(oldText);

        if (newText is null)
            return source;

        if (caseSensitive)
            foreach (var item in source.Where(x => x.Value.Contains(oldText)).ToArray())
                source[item.Key] = item.Value.Replace(oldText, newText, StringComparison.InvariantCulture);
        else
            foreach (var item in source.Where(
                         x => x.Value.IndexOf(oldText, 0, x.Value.Length,
                         StringComparison.CurrentCultureIgnoreCase) >= 0).ToArray())
                source[item.Key] = item.Value.Replace(oldText, newText, StringComparison.InvariantCulture);

        return source;
    }

    /// <summary>
    /// Compare two <see cref="IRStringTable"/>s for equality.
    /// </summary>
    /// <param name="x">The first <see cref="IRStringTable"/> to compare.</param>
    /// <param name="y">The second <see cref="IRStringTable"/> to compare.</param>
    /// <returns><see langword="true"/> if the two <see cref="IRStringTable"/>s are equal; otherwise, <see langword="false"/>.</returns>
    public static bool ItemsEquals(
        [NotNullWhen(true)] this IRStringTable? x,
        [NotNullWhen(true)] IRStringTable? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x == null || y == null)
            return false;

        if (x is ICollection<KeyValuePair<ulong, string>> collection &&
            y is ICollection<KeyValuePair<ulong, string>> collection2)
            if (collection.Count != collection2.Count) return false;

        using var enumerator1 = x.OrderBy(static x => x.Key).GetEnumerator();
        using var enumerator2 = y.OrderBy(static y => y.Key).GetEnumerator();

        while (enumerator1.MoveNext())
        {
            if (!enumerator2.MoveNext())
                return false;

            var pair1 = enumerator1.Current;
            var pair2 = enumerator2.Current;

            if (pair1.Key != pair2.Key)
                return false;

            if (!pair1.Value.Equals(pair2.Value))
                return false;
        }

        // Ensure same count of items
        return !enumerator2.MoveNext();
    }

    /// <summary>
    /// Compare two <see cref="IRStringTable"/>s for equality.
    /// </summary>
    /// <param name="left">The first <see cref="IRStringTable"/> to compare.</param>
    /// <param name="right">The second <see cref="IRStringTable"/> to compare.</param>
    /// <returns><see langword="true"/> if the two <see cref="IRStringTable"/>s are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals(
        [NotNullWhen(true)] this IRStringTable? left,
        [NotNullWhen(true)] IRStringTable? right)
    {
        return left.ItemsEquals(right) && left.Metadata.Equals(right.Metadata);
    }

    /// <summary>
    /// Calculate the content offset of the specified <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="source">The source to calculate the offset of.</param>
    /// <returns>The content offset of the specified <see cref="IRStringTable"/>.</returns>
    public static int CalcContentOffset(this IRStringTable source)
    {
        var metadata = source.Metadata;
        var version = metadata.Version;

        return version switch
        {
            > 4 => 8 + (8 * source.Count()),
            2 when metadata is ILegacyFontConfigMetadata legacyFontConfigMetadata &&
                   legacyFontConfigMetadata.FontConfig.IsNotNullOrWhiteSpace() => 14 +
                Encoding.UTF8.GetByteCount(legacyFontConfigMetadata.FontConfig) + (8 * source.Count()),
            2 => 10 + (8 * source.Count()),
            _ => 9 + (8 * source.Count())
        };
    }

    // ======================================================================================
    // ================= WithItem - IDictionary<ulong, string> ==============================
    // ======================================================================================

    /// <summary>
    /// Add a new item to the <see cref="IRStringTable"/> and return the source.
    /// </summary>
    /// <param name="source">The source to add the item to.</param>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns>The source with the added item.</returns>
    public static TSource WithItem<TSource>(
        this TSource source, ulong key, string value)
        where TSource : IRStringTable, IDictionary<ulong, string>
    {
        source[key] = value;
        return source;
    }

    /// <summary>
    /// Add a new item to the <see cref="IRStringTable"/> and return the source.
    /// </summary>
    /// <param name="source">The source to add the item to.</param>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The source with the added item.</returns>
    public static TSource WithItem<TSource, TValue>(
        this TSource source, ulong key, TValue value)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => WithItem(source, key, value.ToString(CultureInfo.CurrentCulture));

    /// <summary>
    /// Add a new item to the <see cref="IRStringTable"/> and return the source.
    /// </summary>
    /// <param name="source">The source to add the item to.</param>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    /// <param name="provider">The format provider to use.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The source with the added item.</returns>
    public static TSource WithItem<TSource, TValue>(
        this TSource source, ulong key, TValue value, IFormatProvider provider)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => WithItem(source, key, value.ToString(provider));

    /// <summary>
    /// Add a new item to the <see cref="IRStringTable"/> and return the source.
    /// </summary>
    /// <param name="source">The source to add the item to.</param>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns>The source with the added item.</returns>
    public static TSource WithItem<TSource>(
        this TSource source, ulong key, object? value)
        where TSource : IRStringTable, IDictionary<ulong, string>
        => WithItem(source, key, value?.ToString() ?? string.Empty);

    // ======================================================================================
    // ================= WithItem - ICollection<KeyValuePair<ulong, string>> ================
    // ======================================================================================

    /// <summary>
    /// Add a new item to the <see cref="IRStringTable"/> and return the source.
    /// </summary>
    /// <param name="source">The source to add the item to.</param>
    /// <param name="pair">The item to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns>The source with the added item.</returns>
    public static TSource WithItem<TSource>(
        this TSource source, KeyValuePair<ulong, string> pair)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>>
    {
        source.Add(pair);
        return source;
    }

    /// <summary>
    /// Add a new item to the <see cref="IRStringTable"/> and return the source.
    /// </summary>
    /// <param name="source">The source to add the item to.</param>
    /// <param name="pair">The item to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The source with the added item.</returns>
    public static TSource WithItem<TSource, TValue>(
        this TSource source, KeyValuePair<string, TValue> pair)
        where TSource : IRStringTable, IDictionary<ulong, string> where TValue : IConvertible
        => WithItem(source, KeyValuePair.Create(NameToHash(source, pair.Key), pair.Value.ToString(CultureInfo.CurrentCulture)));

    /// <summary>
    /// Add a new item to the <see cref="IRStringTable"/> and return the source.
    /// </summary>
    /// <param name="source">The source to add the item to.</param>
    /// <param name="pair">The item to add.</param>
    /// <param name="provider">The format provider to use.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The source with the added item.</returns>
    public static TSource WithItem<TSource, TValue>(
        this TSource source, KeyValuePair<string, TValue> pair, IFormatProvider provider)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>> where TValue : IConvertible
        => WithItem(source, KeyValuePair.Create(NameToHash(source, pair.Key), pair.Value.ToString(provider)));

    /// <summary>
    /// Add a new item to the <see cref="IRStringTable"/> and return the source.
    /// </summary>
    /// <param name="source">The source to add the item to.</param>
    /// <param name="pair">The item to add.</param>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <returns>The source with the added item.</returns>
    public static TSource WithItem<TSource>(
        this TSource source, KeyValuePair<string, object> pair)
        where TSource : IRStringTable, ICollection<KeyValuePair<ulong, string>>
        => WithItem(source, KeyValuePair.Create(NameToHash(source, pair.Key), pair.Value.ToString() ?? string.Empty));
}

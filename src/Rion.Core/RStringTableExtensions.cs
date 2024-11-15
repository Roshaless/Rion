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
using System.Text;

using Rion.Core.Metadata;
using Rion.Core.Metadata.Legacy;

namespace Rion.Core;

/// <summary>
/// The class of extension methods for string table.
/// </summary>
public static class RStringTableExtensions
{
    /// <summary>
    /// Creates a new <see cref="RStringTable"/> from an <see cref="IEnumerable{T}"/> and based on the <paramref name="metadata"/>.
    /// </summary>
    /// <param name="collection">The collection of hashes and strings.</param>
    /// <param name="metadata">The metadata to set.</param>
    /// <returns>A new instance of the <see cref="RStringTable"/> class, based on the <paramref name="collection"/> and <paramref name="metadata"/>.</returns>
    public static RStringTable ToRStringTable(
        this IEnumerable<KeyValuePair<ulong, string>> collection,
        IRStringTableMetadata metadata) => new(collection, metadata);

    /// <summary>
    /// Creates a new <see cref="RStringTable"/> from an <see cref="IDictionary{TKey, TValue}"/> and based on the <paramref name="metadata"/>.
    /// </summary>
    /// <param name="dictionary">The generic collection of key/value pairs.</param>
    /// <param name="metadata">The metadata to set.</param>
    /// <returns>A new instance of the <see cref="RStringTable"/> class, based on the <paramref name="dictionary"/> and <paramref name="metadata"/>.</returns>
    public static RStringTable ToRStringTable(
        this IDictionary<ulong, string> dictionary,
        IRStringTableMetadata metadata) => new(dictionary, metadata);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="name">The hash name of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
    public static void Add(this IRStringTable table, string name, string value)
        => table.Add(table.Metadata.HashAlgorithm.Hash(name), value);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="IRStringTable"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static void Add<T>(this IRStringTable table, ulong key, T value)
        where T : IConvertible => Add(table, key, value, CultureInfo.CurrentCulture);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <param name="cultureInfo">An object that supplies culture-specific casing rules.</param>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="IRStringTable"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static void Add<T>(this IRStringTable table, ulong key, T value, CultureInfo cultureInfo)
        where T : IConvertible => table.Add(key, value.ToString(cultureInfo));

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static void Add(this IRStringTable table, ulong key, object? value)
    {
        if (value is not null)
        {
            table.Add(key, value.ToString() ?? string.Empty);
        }
    }

    /// <summary>
    /// Determines whether the current <see cref="IRStringTable"/> contains the specified hash name.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="name">The hash name to locate in the <see cref="IRStringTable"/>.</param>
    /// <returns>True if the current <see cref="IRStringTable"/> contains an element with the specified key; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
    public static bool ContainsKey(this IRStringTable table, string name)
        => table.ContainsKey(table.Metadata.HashAlgorithm.Hash(name));

    /// <summary>
    /// Removes the value with the specified hash name in the current <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="name">The hash name of the element to remove.</param>
    /// <returns>True if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the <see cref="RStringTableBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
    public static bool Remove(this IRStringTable table, string name)
        => table.Remove(table.Metadata.HashAlgorithm.Hash(name));

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="name">The hash name of the value to get.</param>
    /// <param name="value">
    /// <para>When this method returns, contains the value associated with the specified name,</para>
    /// <para>if the key is found; otherwise, the default value for the type of the value parameter.</para>
    /// </param>
    /// <returns>True if the current <see cref="IRStringTable"/> contains an element with the specified key; otherwise, false.</returns>
    public static bool TryGetValue(this IRStringTable table, string name, [MaybeNullWhen(false)] out string? value)
        => table.TryGetValue(table.Metadata.HashAlgorithm.Hash(name), out value);

    /// <summary>
    /// Adds the specified key and value to the <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="name">The hash name of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
    public static bool TryAdd(this IRStringTable table, string name, object value)
        => TryAdd(table, table.Metadata.HashAlgorithm.Hash(name), value);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="name">The hash name of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
    public static bool TryAdd<T>(this IRStringTable table, string name, T value)
        where T : IConvertible => TryAdd(table, table.Metadata.HashAlgorithm.Hash(name), value);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static bool TryAdd<T>(this IRStringTable table, ulong key, T value)
        where T : IConvertible => TryAdd(table, key, value, CultureInfo.CurrentCulture);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <param name="cultureInfo">An object that supplies culture-specific casing rules.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static bool TryAdd<T>(this IRStringTable table, ulong key, T value, CultureInfo cultureInfo) where T : IConvertible
    {
        if (table.ContainsKey(key) is false)
        {
            table.Add(key, value.ToString(cultureInfo));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static bool TryAdd(this IRStringTable table, ulong key, object value)
    {
        if (table.ContainsKey(key) is false)
        {
            table.Add(key, value.ToString() ?? string.Empty);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculates the base offset of the content strings.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <returns>The base offset of strings.</returns>
    public static int CalcContentOffset(this IRStringTable table)
    {
        var metadata = table.Metadata;
        var version = metadata.Version;

        if (version > 4)
        {
            //return 4 + 4 + (8 * table.Count);
            return 8 + (8 * table.Count);

        }
        else
        {
            if (version is 2)
            {
                if (metadata is ILegacyFontConfigMetadata legacyFontConfigMetadata &&
                    string.IsNullOrWhiteSpace(legacyFontConfigMetadata.FontConfig) is false)
                {
                    //return 4 + 1 + 4 + Encoding.UTF8.GetByteCount(legacyFontConfigMetadata.FontConfig) + 4 + (8 * table.Count) + 1;
                    return 14 + Encoding.UTF8.GetByteCount(legacyFontConfigMetadata.FontConfig) + (8 * table.Count);
                }
                else
                {
                    //return 4 + 1 + 4 + (8 * table.Count) + 1;
                    return 10 + (8 * table.Count);
                }
            }

            //return 4 + 4 + (8 * table.Count) + 1;
            return 9 + (8 * table.Count);
        }
    }

    /// <summary>
    /// Determines whether the specified items of object are considered equal.
    /// </summary>
    /// <param name="x">The left to compare right.</param>
    /// <param name="y">The right to compare left.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> and <paramref name="y"/> refer to the same object,
    /// or <paramref name="x"/> and <paramref name="y"/> <see cref="ICollection{T}.Count">size</see> and all items are same,
    /// or <paramref name="x"/> and <paramref name="y"/> are <see langword="null"/>, otherwise, <see langword="false"/>.
    /// </returns>
    public static bool ItemsEquals(this IRStringTable? x, IRStringTable? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        if (x.Count != y.Count)
        {
            return false;
        }

        foreach (var pair in x)
        {
            if (y.TryGetValue(pair.Key, out var value))
            {
                if (!StringComparer.Ordinal.Equals(pair.Value, value))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Replaces all matched texts in the current <see cref="IRStringTable"/> and return itself.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="oldText">The string to replaced.</param>
    /// <param name="newText">Replace with a new string.</param>
    /// <param name="caseSensitive">Whether to enable case-sensitive comparison. Compares lowercase strings by default.</param>
    /// <returns>A reference to this instance after the operation is complete.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="oldText"/> or <paramref name="newText"/> is null.</exception>
    public static IRStringTable ReplaceAll(this IRStringTable table, string oldText, string newText, bool caseSensitive = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(oldText);

        if (newText is null)
        {
            return table;
        }

        if (caseSensitive)
        {
            foreach (var item in table.Where(x => x.Value.Contains(oldText)).ToArray())
            {
                table[item.Key] = item.Value.Replace(oldText, newText, StringComparison.InvariantCulture);
            }
        }
        else
        {
            foreach (var item in table.Where(x => x.Value.IndexOf(oldText, 0, x.Value.Length, StringComparison.CurrentCultureIgnoreCase) >= 0).ToArray())
            {
                table[item.Key] = item.Value.Replace(oldText, newText, StringComparison.InvariantCulture);
            }
        }

        return table;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/> and return itself.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static IRStringTable WithItem(this IRStringTable table, ulong key, string value)
    {
        table[key] = value;
        return table;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/> and return itself.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="name">The hash name of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
    public static IRStringTable WithItem(this IRStringTable table, string name, string value)
        => WithItem(table, table.Metadata.HashAlgorithm.Hash(name), value);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/> and return itself.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="IRStringTable"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static IRStringTable WithItem<T>(this IRStringTable table, ulong key, T value)
        where T : IConvertible => WithItem(table, key, value, CultureInfo.CurrentCulture);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/> and return itself.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <param name="cultureInfo">An object that supplies culture-specific casing rules.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="IRStringTable"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static IRStringTable WithItem<T>(this IRStringTable table, ulong key, T value, CultureInfo cultureInfo)
        where T : IConvertible => WithItem(table, key, value.ToString(cultureInfo));

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/> and return itself.
    /// </summary>
    /// <param name="table">The string table self.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public static IRStringTable WithItem(this IRStringTable table, ulong key, object? value)
    {
        if (null == value)
        {
            return table;
        }

        return WithItem(table, key, value.ToString() ?? string.Empty);
    }
}

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

using Rion.Core.Internal;
using Rion.Core.Metadata;
using Rion.Core.Metadata.Legacy;

namespace Rion.Core;

/// <summary>
/// Provides a series of extension methods to enhance functionality for working with <see cref="IRStringTable"/> instances.
/// These methods facilitate operations such as adding, removing, and querying items in a string table,
/// along with transformations like content offset calculation and item replacement.
/// </summary>
public static class RStringTableExtensions
{
    /// <summary>
    /// Converts a collection of key-value pairs into an <see cref="RStringTable"/> instance, associating it with the provided metadata.
    /// </summary>
    /// <param name="collection">The collection containing key-value pairs to be converted.</param>
    /// <param name="metadata">The metadata to be associated with the created <see cref="RStringTable"/>.</param>
    /// <returns>A new instance of <see cref="RStringTable"/> populated with the provided key-value pairs and associated metadata.</returns>
    public static RStringTable ToRStringTable(
        this IEnumerable<KeyValuePair<ulong, string>> collection,
        IRStringTableMetadata metadata) => new(collection, metadata);

    /// <summary>
    /// Converts a dictionary of key-value pairs into an <see cref="RStringTable"/> instance, using specified metadata.
    /// </summary>
    /// <param name="dictionary">The dictionary containing key-value pairs to convert.</param>
    /// <param name="metadata">Metadata to associate with the <see cref="RStringTable"/>.</param>
    /// <returns>A new <see cref="RStringTable"/> instance populated with the dictionary's content and provided metadata.</returns>
    public static RStringTable ToRStringTable(
        this IDictionary<ulong, string> dictionary,
        IRStringTableMetadata metadata) => new(dictionary, metadata);

    /// <summary>
    /// Adds the specified key-value pair to the <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The string table to which the value will be added.</param>
    /// <param name="name">The name (string) of the element to add, which will be hashed before addition.</param>
    /// <param name="value">The value to associate with the hashed key in the string table.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="name"/> is null.</exception>
    public static void Add(this IRStringTable table, string name, string value)
        => table.Add(table.Metadata.HashAlgorithm.Hash(name), value);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable"/>, using the current culture for conversion.
    /// </summary>
    /// <param name="table">The string table to which the value will be added.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add, which must implement <see cref="IConvertible"/>.</param>
    /// <typeparam name="T">The type of the value being added that implements <see cref="IConvertible"/>.</typeparam>
    /// <exception cref="ArgumentException">Thrown if an element with the same key already exists in the <see cref="IRStringTable"/>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="key"/> is null.</exception>
    public static void Add<T>(this IRStringTable table, ulong key, T value)
        where T : IConvertible => Add(table, key, value, CultureInfo.CurrentCulture);

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable" />, considering the specified culture.
    /// </summary>
    /// <param name="table">The string table instance.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add, convertible to string.</param>
    /// <param name="cultureInfo">Specifies the culture-specific format information for the conversion.</param>
    /// <typeparam name="T">The type of the value, constrained to implement <see cref="IConvertible"/>.</typeparam>
    /// <exception cref="ArgumentException">If an element with the same key already exists in the <see cref="IRStringTable"/>.</exception>
    /// <exception cref="ArgumentNullException">If the <paramref name="key"/> is null.</exception>
    public static void Add<T>(this IRStringTable table, ulong key, T value, CultureInfo cultureInfo)
        where T : IConvertible => table.Add(key, value.ToString(cultureInfo));

    /// <summary>
    /// Adds the specified key and value to the current <see cref="IRStringTable" /> if the value is not null.
    /// </summary>
    /// <param name="table">The string table to which the element will be added.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add, which can be null.</param>
    public static void Add(this IRStringTable table, ulong key, object? value)
    {
        if (value is not null)
        {
            table.Add(key, value.ToString() ?? string.Empty);
        }
    }

    /// <summary>
    /// Determines if the specified hash name exists within the <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> instance to search in.</param>
    /// <param name="name">The name to be hashed and checked for presence.</param>
    /// <returns><see langword="true"/> if the hash of <paramref name="name"/> is found in the table; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>.</exception>
    public static bool ContainsKey(this IRStringTable table, string name)
        => table.ContainsKey(table.Metadata.HashAlgorithm.Hash(name));

    /// <summary>
    /// Removes the entry with the specified name from the <see cref="IRStringTable"/>, using its hashed value.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> from which to remove the entry.</param>
    /// <param name="name">The name of the entry to remove. It will be hashed to find the corresponding key.</param>
    /// <returns><see langword="true"/> if the entry was found and removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">If the <paramref name="name"/> is <see langword="null"/>.</exception>
    public static bool Remove(this IRStringTable table, string name)
        => table.Remove(table.Metadata.HashAlgorithm.Hash(name));

    /// <summary>
    /// Attempts to get the value associated with the specified name from the string table.
    /// </summary>
    /// <param name="table">The string table instance.</param>
    /// <param name="name">The name whose value to retrieve.</param>
    /// <param name="value">Output parameter for the value associated with the specified name, if found.</param>
    /// <returns><see langword="true"/> if the value was found; otherwise, <see langworkd="false"/>.</returns>
    public static bool TryGetValue(this IRStringTable table, string name, [MaybeNullWhen(false)] out string? value)
        => table.TryGetValue(table.Metadata.HashAlgorithm.Hash(name), out value);

    /// <summary>
    /// Attempts to add the specified key and value to the <see cref="IRStringTable"/>.
    /// If the key already exists, the operation will not replace the existing value and will return false.
    /// </summary>
    /// <param name="table">The string table to which the element will be added.</param>
    /// <param name="name">The name (which will be hashed) of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the addition was successful; otherwise, false if the key already exists.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="name"/> is null.</exception>
    public static bool TryAdd(this IRStringTable table, string name, object value)
        => TryAdd(table, table.Metadata.HashAlgorithm.Hash(name), value);

    /// <summary>
    /// Attempts to add the specified key and value to the <see cref="IRStringTable"/>.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> instance to which the item will be added.</param>
    /// <param name="name">The name (which will be hashed) of the item to add.</param>
    /// <param name="value">The value of the item to add, must implement <see cref="IConvertible"/>.</param>
    /// <typeparam name="T">The type of the value, constrained to implement <see cref="IConvertible"/>.</typeparam>
    /// <returns>True if the addition was successful; otherwise, false if the key already exists.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
    public static bool TryAdd<T>(this IRStringTable table, string name, T value)
        where T : IConvertible => TryAdd(table, table.Metadata.HashAlgorithm.Hash(name), value);

    /// <summary>
    /// Attempts to add the specified key and value to the <see cref="IRStringTable"/> without specifying the type explicitly.
    /// The value is directly converted to a string using the current culture's format.
    /// </summary>
    /// <param name="table">The string table instance to add the key-value pair to.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to associate with the key, which will be converted to a string.</param>
    /// <returns>True if the element was added successfully; false if the key already exists in the table.</returns>
    public static bool TryAdd<T>(this IRStringTable table, ulong key, T value)
        where T : IConvertible => TryAdd(table, key, value, CultureInfo.CurrentCulture);

    /// <summary>
    /// Attempts to add the specified key and value to the current <see cref="IRStringTable" /> using the provided culture-specific formatting.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> instance to which the item will be added.</param>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add, convertible to string.</param>
    /// <param name="cultureInfo">The culture-specific information determining how value should be formatted.</param>
    /// <typeparam name="T">The type of the value being added, constrained to implement <see cref="IConvertible"/>.</typeparam>
    /// <returns>True if the addition was successful; false if the key already exists.</returns>
    public static bool TryAdd<T>(this IRStringTable table, ulong key, T value, CultureInfo cultureInfo)  where T : IConvertible
    {
        if (table.ContainsKey(key))
            return false;

        table.Add(key, value.ToString(cultureInfo));
        return true;
    }

    /// <summary>
    /// Attempts to add the specified key and value to the <see cref="IRStringTable"/>.
    /// If the key already exists, the addition is not performed and the method returns false.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> instance to which the item will be added.</param>
    /// <param name="key">The unique key for the item.</param>
    /// <param name="value">The value to associate with the key.</param>
    /// <returns>True if the item was added successfully; false if the key already exists.</returns>
    public static bool TryAdd(this IRStringTable table, ulong key, object? value)
    {
        value = value?.ToString();

        if (value is null)
            return false;

        if (table.ContainsKey(key))
            return false;

        table.Add(key, value);
        return true;
    }

    /// <summary>
    /// Calculates the base offset of the content strings within an <see cref="IRStringTable"/> instance.
    /// </summary>
    /// <param name="table">The string table whose content offset is to be calculated.</param>
    /// <returns>The calculated base offset for the content strings.</returns>
    public static int CalcContentOffset(this IRStringTable table)
    {
        var metadata = table.Metadata;
        var version = metadata.Version;

        return version switch
        {
            > 4 => 8 + (8 * table.Count),
            2 when metadata is ILegacyFontConfigMetadata legacyFontConfigMetadata &&
                   legacyFontConfigMetadata.FontConfig.IsNotNullOrWhiteSpace() => 14 +
                Encoding.UTF8.GetByteCount(legacyFontConfigMetadata.FontConfig) + (8 * table.Count),
            2 => 10 + (8 * table.Count),
            _ => 9 + (8 * table.Count)
        };
    }

    /// <summary>
    /// Determines whether the specified items of object are considered equal.
    /// </summary>
    /// <param name="x">The left to compare right.</param>
    /// <param name="y">The right to compare left.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="x" /> and <paramref name="y" /> refer to the same object,
    /// or <paramref name="x" /> and <paramref name="y" /> <see cref="ICollection{T}.Count">size</see> and all items are
    /// same,
    /// or <paramref name="x" /> and <paramref name="y" /> are <see langword="null" />, otherwise, <see langword="false" />
    /// .
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
    /// Replaces all occurrences of a specified text in the values of the <see cref="IRStringTable"/>
    /// with a new text, optionally considering case sensitivity, and returns the modified instance.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> instance whose values are to be modified.</param>
    /// <param name="oldText">The substring to find and replace in the table values.</param>
    /// <param name="newText">The replacement string to substitute for each occurrence of oldText.</param>
    /// <param name="caseSensitive">Determines if the search should be case-sensitive. Defaults to false.</param>
    /// <returns>The same <see cref="IRStringTable"/> instance with its values updated post-replacement.</returns>
    /// <exception cref="ArgumentNullException">Thrown if oldText is null or empty.</exception>
    public static IRStringTable ReplaceAll(this IRStringTable table, string oldText, string? newText, bool caseSensitive = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(oldText);

        if (newText is null)
            return table;

        if (caseSensitive)
            foreach (var item in table.Where(x => x.Value.Contains(oldText)).ToArray())
                table[item.Key] = item.Value.Replace(oldText, newText, StringComparison.InvariantCulture);
        else
            foreach (var item in table.Where(
                         x => x.Value.IndexOf(oldText, 0, x.Value.Length,
                         StringComparison.CurrentCultureIgnoreCase) >= 0).ToArray())
                table[item.Key] = item.Value.Replace(oldText, newText, StringComparison.InvariantCulture);

        return table;
    }

    /// <summary>
    /// Adds the specified item to the <see cref="IRStringTable"/> and returns the updated table instance.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> to add the item to.</param>
    /// <param name="key">The key of the item.</param>
    /// <param name="value">The value of the item.</param>
    /// <returns>The updated <see cref="IRStringTable"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If the key is null.</exception>
    public static IRStringTable WithItem(this IRStringTable table, ulong key, string value)
    {
        table[key] = value;
        return table;
    }

    /// <summary>
    /// Adds an item to the <see cref="IRStringTable"/> with the specified name, automatically hashing the name using the table's metadata.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> to which the item will be added.</param>
    /// <param name="name">The name of the item to add.</param>
    /// <param name="value">The value associated with the name to be added to the table.</param>
    /// <returns>The updated <see cref="IRStringTable"/> with the new item included.</returns>
    public static IRStringTable WithItem(this IRStringTable table, string name, string value)
        => WithItem(table, table.Metadata.HashAlgorithm.Hash(name), value);

    /// <summary>
    /// Adds the specified key-value pair to the <see cref="IRStringTable"/> and returns the table instance.
    /// </summary>
    /// <param name="table">The string table instance to add the item to.</param>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add, convertible to string.</param>
    /// <typeparam name="T">The type of the value which must implement <see cref="IConvertible"/>.</typeparam>
    /// <returns>The updated <see cref="IRStringTable"/> instance.</returns>
    /// <exception cref="ArgumentException">If the key already exists in the table.</exception>
    /// <exception cref="ArgumentNullException">If the key is null.</exception>
    public static IRStringTable WithItem<T>(this IRStringTable table, ulong key, T value)
        where T : IConvertible => WithItem(table, key, value, CultureInfo.CurrentCulture);

    /// <summary>
    /// Adds an item to the <see cref="IRStringTable"/> with the specified key, value, and culture information, returning the table instance.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> to which the item will be added.</param>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add, convertible to string.</param>
    /// <param name="cultureInfo">The culture information used for converting the value to a string.</param>
    /// <typeparam name="T">The type of the value, must implement <see cref="IConvertible"/>.</typeparam>
    /// <returns>The updated <see cref="IRStringTable"/> with the new item included.</returns>
    public static IRStringTable WithItem<T>(this IRStringTable table, ulong key, T value, CultureInfo cultureInfo)
        where T : IConvertible => WithItem(table, key, value.ToString(cultureInfo));

    /// <summary>
    /// Adds an item to the <see cref="IRStringTable"/> with the specified key and value, returning the table instance.
    /// </summary>
    /// <param name="table">The <see cref="IRStringTable"/> instance to add the item to.</param>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    /// <returns>The updated <see cref="IRStringTable"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="key"/> is null.</exception>
    public static IRStringTable WithItem(this IRStringTable table, ulong key, object? value)
        => null == value ? table : WithItem(table, key, value.ToString() ?? string.Empty);
}

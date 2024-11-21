// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Collections.Generic;

using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Defines an interface for a string table that associates unique hashes with strings,
/// enhancing basic dictionary functionality with metadata about the table.
/// </summary>
/// <seealso cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>
public interface IRStringTable : IDictionary<ulong, string>
{
    /// <summary>
    /// Indexer property for accessing string table entries by their string name, internally using the hash to fetch the value.
    /// </summary>
    /// <param name="name">The name of the string whose hash-value pair is to be accessed.</param>
    /// <returns>The string value associated with the hashed key of the input string name.</returns>
    /// <exception cref="KeyNotFoundException">If the hashed key does not exist in the string table.</exception>
    string this[string name] { get; set; }

    /// <summary>
    /// Gets the metadata associated with the string table, including hash algorithm and version details.
    /// </summary>
    IRStringTableMetadata Metadata { get; }
}

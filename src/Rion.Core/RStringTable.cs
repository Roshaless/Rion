// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Collections;
using System.Collections.Generic;

using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Represents a string table with support for custom metadata and hashing of keys.
/// Extends the basic dictionary functionality with specific methods and properties
/// tailored for managing hashed string data.
/// </summary>
public partial class RStringTable : Dictionary<ulong, string>, IRStringTable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable" /> class.
    /// </summary>
    public RStringTable() : this(RStringTableMetadata.Latest, []) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable" /> class based on the metadata.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    public RStringTable(IRStringTableMetadata metadata) => Metadata = metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable" /> class based on the metadata and capacity.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    /// <param name="capacity">The size of collection to init.</param>
    public RStringTable(IRStringTableMetadata metadata, int capacity) : base(capacity) => Metadata = metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable" /> class based on the items collection and metadata.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    /// <param name="collection">The collection of hashes and strings.</param>
    public RStringTable(IRStringTableMetadata metadata, IEnumerable<KeyValuePair<ulong, string>> collection) : base(collection) => Metadata = metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable" /> class based on the dictionary and metadata.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    /// <param name="dictionary">The generic collection of key/value pairs.</param>
    public RStringTable(IRStringTableMetadata metadata, IDictionary<ulong, string> dictionary) : base(dictionary) => Metadata = metadata;

    /// <summary>
    /// Gets or sets the string value associated with the specified name of the hash.
    /// </summary>
    /// <param name="name">The name of the hash.</param>
    public string this[string name]
    {
        get => this[Metadata.HashAlgorithm.Hash(name)];
        set => this[Metadata.HashAlgorithm.Hash(name)] = value;
    }

    /// <inheritdoc />
    public IRStringTableMetadata Metadata { get; }

    /// <summary>
    /// Represents a record of a string table with metadata and entries.
    /// </summary>
    /// <param name="Metadata">The metadata of the string table.</param>
    /// <param name="Entries">The entries of the string table.</param>
    private readonly record struct RecordRStringTable(IRStringTableMetadata Metadata, IEnumerable<KeyValuePair<ulong, string>> Entries) : IRStringTable
    {
        /// <summary>
        /// Gets an enumerator for the entries of the string table.
        /// </summary>
        /// <returns>An enumerator for the entries.</returns>
        public IEnumerator<KeyValuePair<ulong, string>> GetEnumerator() => Entries.GetEnumerator();

        /// <summary>
        /// Gets an enumerator for the entries of the string table.
        /// </summary>
        /// <returns>An enumerator for the entries.</returns>
        IEnumerator IEnumerable.GetEnumerator() => Entries.GetEnumerator();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RStringTable" /> class.
    /// </summary>
    /// <returns>A new instance of the <see cref="RStringTable" /> class.</returns>
    public static RStringTable Create()
    {
        return [];
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RStringTable" /> class with the specified metadata.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    /// <returns>A new instance of the <see cref="RStringTable" /> class.</returns>
    public static RStringTable Create(IRStringTableMetadata metadata)
    {
        return new RStringTable(metadata);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RStringTable" /> class with the specified metadata and capacity.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    /// <param name="capacity">The size of collection to init.</param>
    /// <returns>A new instance of the <see cref="RStringTable" /> class.</returns>
    public static RStringTable Create(IRStringTableMetadata metadata, int capacity)
    {
        return new RStringTable(metadata, capacity);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RStringTable" /> class with the specified metadata and collection.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    /// <param name="collection">The collection of hashes and strings.</param>
    /// <returns>A new instance of the <see cref="RStringTable" /> class.</returns>
    public static RStringTable Create(IRStringTableMetadata metadata, IEnumerable<KeyValuePair<ulong, string>> collection)
    {
        return new RStringTable(metadata, collection);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RStringTable" /> class with the specified metadata and dictionary.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    /// <param name="dictionary">The generic collection of key/value pairs.</param>
    /// <returns>A new instance of the <see cref="RStringTable" /> class.</returns>
    public static RStringTable Create(IRStringTableMetadata metadata, IDictionary<ulong, string> dictionary)
    {
        return new RStringTable(metadata, dictionary);
    }

    /// <summary>
    /// Creates a record of the string table with metadata and entries.
    /// </summary>
    /// <param name="metadata">The metadata of the string table.</param>
    /// <param name="entries">The entries of the string table.</param>
    /// <returns>A record of the string table.</returns>
    public static IRStringTable CreateRecord(IRStringTableMetadata metadata, IEnumerable<KeyValuePair<ulong, string>> entries)
    {
        return new RecordRStringTable(metadata, entries);
    }
}

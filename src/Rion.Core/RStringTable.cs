// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Represents a string table with support for custom metadata and hashing of keys.
/// Extends the basic dictionary functionality with specific methods and properties
/// tailored for managing hashed string data.
/// </summary>
public class RStringTable : Dictionary<ulong, string>, IEquatable<RStringTable>, IRStringTable
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

    /// <inheritdoc />
    public bool Equals(RStringTable? other) => other is not null && this.SequenceEqual(other) && Metadata.Equals(other.Metadata);

    /// <inheritdoc />
    public string this[string name]
    {
        get => this[Metadata.HashAlgorithm.Hash(name)];
        set => this[Metadata.HashAlgorithm.Hash(name)] = value;
    }

    /// <inheritdoc />
    public IRStringTableMetadata Metadata { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as RStringTable);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Metadata, this);

    public readonly record struct RecordRStringTable(IRStringTableMetadata Metadata, IEnumerable<KeyValuePair<ulong, string>> Entries) : IRStringTable
    {
        public readonly IEnumerator<KeyValuePair<ulong, string>> GetEnumerator() => Entries.GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => Entries.GetEnumerator();
    }
}

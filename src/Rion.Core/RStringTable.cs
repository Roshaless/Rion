// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Generic;

using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Represents a string table of hashes and strings.
/// </summary>
public sealed class RStringTable : Dictionary<ulong, string>, IRStringTable, IEquatable<RStringTable>
{
    /// <inheritdoc/>
    public string this[string name]
    {
        get => this[Metadata.HashAlgorithm.Hash(name)];
        set => this[Metadata.HashAlgorithm.Hash(name)] = value;
    }

    /// <inheritdoc/>
    public IRStringTableMetadata Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable"/> class.
    /// </summary>
    public RStringTable() : this([], RStringTableMetadata.Latest) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable"/> class based on the metadata.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    public RStringTable(IRStringTableMetadata metadata) => Metadata = metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable"/> class based on the metadata and capacity.
    /// </summary>
    /// <param name="metadata">The metadata to set.</param>
    /// <param name="capacity">The size of collection to init.</param>
    public RStringTable(IRStringTableMetadata metadata, int capacity) : base(capacity) => Metadata = metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable"/> class based on the items collection and metadata.
    /// </summary>
    /// <param name="collection">The collection of hashes and strings.</param>
    /// <param name="metadata">The metadata to set.</param>
    public RStringTable(IEnumerable<KeyValuePair<ulong, string>> collection, IRStringTableMetadata metadata) : base(collection) => Metadata = metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTable"/> class based on the dictionary and metadata.
    /// </summary>
    /// <param name="dictionary">The generic collection of key/value pairs.</param>
    /// <param name="metadata">The metadata to set.</param>
    public RStringTable(IDictionary<ulong, string> dictionary, IRStringTableMetadata metadata) : base(dictionary) => Metadata = metadata;

    /// <inheritdoc/>
    public bool Equals(RStringTable? other) => this.ItemsEquals(other) && Metadata.Equals(other?.Metadata);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as RStringTable);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Metadata, this);
}

// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using Rion.Core.Hashing;

namespace Rion.Core.Metadata;

/// <summary>
/// Defines the contract for metadata of a string table, specifying the hash algorithm and version details.
/// </summary>
public interface IRStringTableMetadata
{
    /// <summary>
    /// Gets the hash algorithm instance used for generating hashes within the string table.
    /// </summary>
    RSTHashAlgorithm HashAlgorithm { get; }

    /// <summary>
    /// Gets the current version of the string table metadata.
    /// </summary>
    /// <value>
    /// A byte representing the version number of the string table metadata.
    /// </value>
    byte Version { get; }
}

// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using Rion.Core.Hashing;

namespace Rion.Core.Metadata;

/// <summary>
/// Represents a metadata of string table.
/// </summary>
public interface IRStringTableMetadata
{
    /// <summary>
    /// Gets the current hash algorithm used of this metadata.
    /// </summary>
    IRSTHashAlgorithm HashAlgorithm { get; }

    /// <summary>
    /// Gets the current version of this metadata.
    /// </summary>
    byte Version { get; }
}

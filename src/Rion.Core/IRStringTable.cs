// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Collections.Generic;

using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Represents a string table, which is a collection of key-value pairs where the keys are hashed strings and the values are strings.
/// </summary>
public interface IRStringTable : IEnumerable<KeyValuePair<ulong, string>>
{
    /// <summary>
    /// Gets the metadata associated with the string table, including hash algorithm and version details.
    /// </summary>
    IRStringTableMetadata Metadata { get; }
}

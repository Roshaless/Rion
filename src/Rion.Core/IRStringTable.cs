// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Collections.Generic;

using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Represents a string table of hash/string pairs.
/// </summary>
public interface IRStringTable : IDictionary<ulong, string>
{
    /// <summary>
    /// Gets or sets the item with the specified name.
    /// </summary>
    /// <param name="name">The name of the item to get or set.</param>
    /// <returns>The item with the specified name.</returns>
    string this[string name] { get; set; }

    /// <summary>
    /// Gets the metadata of the string table.
    /// </summary>
    IRStringTableMetadata Metadata { get; }
}

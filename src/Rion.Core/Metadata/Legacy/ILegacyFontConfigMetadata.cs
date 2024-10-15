// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

namespace Rion.Core.Metadata.Legacy;

/// <summary>
/// [Legacy] Represents a metadata with font config (for v2).
/// </summary>
public interface ILegacyFontConfigMetadata : IRStringTableMetadata
{
    /// <summary>
    /// Gets or sets the font config of this Metadata.
    /// </summary>
    string? FontConfig { get; set; }
}

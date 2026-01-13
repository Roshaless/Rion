// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

namespace Rion.Core.Metadata;

/// <summary>
/// [Legacy] Defines the required properties for a font configuration metadata in a legacy rst (v2).
/// </summary>
public interface ILegacyFontConfigMetadata : IRStringTableMetadata
{
    /// <summary>
    /// Gets or sets the font configuration for the legacy metadata.
    /// This property holds the font settings used in the v2 legacy rst.
    /// </summary>
    string? FontConfig { get; set; }
}

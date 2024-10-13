// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

namespace Rion.Core.Metadata.Legacy;

public interface ILegacyFontConfigMetadata : IRStringTableMetadata
{
    string? FontConfig { get; set; }
}

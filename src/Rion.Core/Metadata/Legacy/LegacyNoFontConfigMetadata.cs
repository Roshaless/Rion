// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using Rion.Core.Hashing;

namespace Rion.Core.Metadata.Legacy;

/// <summary>
/// Represents legacy metadata that lacks font configuration for versions 3 and 4.
/// This class encapsulates details specific to these older versions, including the hash algorithm used.
/// </summary>
public sealed record LegacyNoFontConfigMetadata : IRStringTableMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyNoFontConfigMetadata"/> class.
    /// </summary>
    /// <param name="isV4NotV3">Determines whether the version is 4 or 3.</param>
    public LegacyNoFontConfigMetadata(bool isV4NotV3)
    {
        if (isV4NotV3)
        {
            Version = 4;
            HashAlgorithm = RSTHashAlgorithm.LegacyV4V5;
        }
        else
        {
            Version = 3;
            HashAlgorithm = RSTHashAlgorithm.LegacyV2V3;
        }
    }

    /// <inheritdoc />
    public IRSTHashAlgorithm HashAlgorithm { get; }

    /// <inheritdoc />
    public byte Version { get; }
}

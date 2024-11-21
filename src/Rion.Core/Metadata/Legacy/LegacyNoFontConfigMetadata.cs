// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;

using Rion.Core.Hashing;
using Rion.Core.Hashing.Legacy;

namespace Rion.Core.Metadata.Legacy;

/// <summary>
/// Represents legacy metadata that lacks font configuration for versions 3 and 4.
/// This class encapsulates details specific to these older versions, including the hash algorithm used.
/// </summary>
public sealed record LegacyNoFontConfigMetadata : IRStringTableMetadata
{
    /// <summary>
    /// Represents a sealed record class for legacy metadata devoid of font configuration
    /// applicable to version 3 and 4. It encapsulates specifics like the hash algorithm
    /// pertinent to these legacy versions.
    /// </summary>
    public LegacyNoFontConfigMetadata(RSTHashBitsMaskType bitsMaskType)
    {
        if (bitsMaskType is RSTHashBitsMaskType.Mask39)
        {
            Version = 4;
            HashAlgorithm = LegacyRSTHashAlgorithm.BitsMask40;
        }
        else if (bitsMaskType is RSTHashBitsMaskType.Mask40)
        {
            Version = 3;
            HashAlgorithm = LegacyRSTHashAlgorithm.BitsMask40;
        }
        else
        {
            throw new ArgumentException($"Unsupported BitsMask type: {bitsMaskType}", nameof(bitsMaskType));
        }
    }

    /// <inheritdoc />
    public IRSTHashAlgorithm HashAlgorithm { get; }

    /// <inheritdoc />
    public byte Version { get; }
}

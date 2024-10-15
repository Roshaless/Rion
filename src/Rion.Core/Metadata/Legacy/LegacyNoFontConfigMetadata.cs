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
/// [Legacy] The metadata without font config for old version (v3, v4).
/// </summary>
public sealed record class LegacyNoFontConfigMetadata : IRStringTableMetadata
{
    /// <inheritdoc/>
    public IRSTHashAlgorithm HashAlgorithm { get; }

    /// <inheritdoc/>
    public byte Version { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyNoFontConfigMetadata"/> class based on the specified BitsMask type.
    /// </summary>
    /// <param name="bitsMaskType">The specified BitsMask type.</param>
    /// <exception cref="ArgumentException">The BitsMask type is unsupported.</exception>
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
}

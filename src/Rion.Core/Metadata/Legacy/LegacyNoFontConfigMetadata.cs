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
/// 
/// </summary>
public sealed record class LegacyNoFontConfigMetadata : IRStringTableMetadata
{
    /// <inheritdoc/>
    public IRSTHashAlgorithm HashAlgorithm { get; }

    /// <inheritdoc/>
    public byte Version { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="ArgumentException"></exception>
    public LegacyNoFontConfigMetadata(RSTHashBitsMaskType type)
    {
        if (type is RSTHashBitsMaskType.Mask39)
        {
            Version = 4;
            HashAlgorithm = LegacyRSTHashAlgorithm.MaskType40;
        }
        else if (type is RSTHashBitsMaskType.Mask40)
        {
            Version = 3;
            HashAlgorithm = LegacyRSTHashAlgorithm.MaskType40;
        }
        else
        {
            throw new ArgumentException($"Unsupported BitsMask type: {type}", nameof(type));
        }
    }
}

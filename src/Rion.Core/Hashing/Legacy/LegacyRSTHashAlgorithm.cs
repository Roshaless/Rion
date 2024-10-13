// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO.Hashing;

namespace Rion.Core.Hashing.Legacy;

/// <summary>
/// 
/// </summary>
public sealed class LegacyRSTHashAlgorithm : RSTHashAlgorithmBase
{
    /// <summary>
    /// 
    /// </summary>
    public static LegacyRSTHashAlgorithm MaskType39 { get; } = new(RSTHashBitsMaskType.Mask39);

    /// <summary>
    /// 
    /// </summary>
    public static LegacyRSTHashAlgorithm MaskType40 { get; } = new(RSTHashBitsMaskType.Mask40);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bitsMaskType"></param>
    private LegacyRSTHashAlgorithm(RSTHashBitsMaskType bitsMaskType) : base(bitsMaskType) { }

    /// <inheritdoc/>
    public override ulong Hash(ReadOnlySpan<byte> source) => XxHash64.HashToUInt64(source) & BitsMaskValue;
}

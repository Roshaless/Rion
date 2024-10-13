// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO.Hashing;

namespace Rion.Core.Hashing;

/// <summary>
/// 
/// </summary>
public sealed class RSTHashAlgorithm : RSTHashAlgorithmBase
{
    /// <summary>
    /// 
    /// </summary>
    public static RSTHashAlgorithm MaskType39 { get; } = new(RSTHashBitsMaskType.Mask39);

    /* Where MaskType40 ? Wee don't need it, because that is obsolete. */

    /// <summary>
    /// Initializes the base class <see cref="RSTHashAlgorithm"/> with <see cref="RSTHashBitsMaskType"/>.
    /// </summary>
    /// <param name="bitsMaskType"></param>
    private RSTHashAlgorithm(RSTHashBitsMaskType bitsMaskType) : base(bitsMaskType) { }

    /// <inheritdoc/>
    public override ulong Hash(ReadOnlySpan<byte> source) => PatchNewRSTHash(XxHash3.HashToUInt64(source) & BitsMaskValue);

    // For v14.15+
    private static ulong PatchNewRSTHash(ulong hash)
    {
        unsafe
        {
            var bytes = (byte*)&hash;
            var result = (ulong)bytes[4];

            for (var i = 3; i != -1; i--)
            {
                result = bytes[i] | result << 8;
            }

            return result;
        }
    }
}

// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO.Hashing;

namespace Rion.Core.Hashing;

/// <summary>
/// Provides an implementation of the rst hash algorithm.
/// </summary>
public sealed class RSTHashAlgorithm : RSTHashAlgorithmBase
{
    /// <summary>
    /// Gets the rst hash algorithm of BitsMask39, for v5+(greater than v14.15).
    /// </summary>
    public static RSTHashAlgorithm BitsMask39 { get; } = new(RSTHashBitsMaskType.Mask39);

    /* Where MaskType40 ? Wee don't need it, because that is obsolete. */

    /// <summary>
    /// Initializes a new instance of the <see cref="RSTHashAlgorithm"/> class based on the specified BitsMask type.
    /// </summary>
    /// <param name="bitsMaskType">The specified BitsMask type.</param>
    private RSTHashAlgorithm(RSTHashBitsMaskType bitsMaskType) : base(bitsMaskType) { }

    /// <inheritdoc/>
    public override ulong Hash(ReadOnlySpan<byte> source) => TrimXxHash3(XxHash3.HashToUInt64(source) & BitsMaskValue);

    // For v14.15+
    private static ulong TrimXxHash3(ulong hash)
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

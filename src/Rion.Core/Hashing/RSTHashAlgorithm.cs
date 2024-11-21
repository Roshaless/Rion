// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO.Hashing;

namespace Rion.Core.Hashing;

/// <summary>
/// Represents an implementation of the RST hash algorithm, providing various hash computation methods.
/// </summary>
public sealed class RSTHashAlgorithm : RSTHashAlgorithmBase
{
    /// <summary>
    /// Represents an enumeration of different BitsMask types used by the RST hash algorithm.
    /// </summary>
    private RSTHashAlgorithm(RSTHashBitsMaskType bitsMaskType) : base(bitsMaskType) { }

    /// <summary>
    /// Gets the predefined instance of RSTHashAlgorithm configured with BitsMask39, for v5+(greater than v14.15).
    /// </summary>
    public static RSTHashAlgorithm BitsMask39 { get; } = new(RSTHashBitsMaskType.Mask39);

    /// <inheritdoc />
    public override ulong Hash(ReadOnlySpan<byte> source) => TrimXxHash3(XxHash3.HashToUInt64(source) & BitsMaskValue);

    /// <summary>
    /// Trims the specified XxHash3 hash value to a smaller representation used by the rst hash algorithm.
    /// </summary>
    /// <param name="hash">The original XxHash3 hash value to be trimmed.</param>
    /// <returns>The trimmed hash value according to rst algorithm's mask requirements.</returns>
    private static ulong TrimXxHash3(ulong hash)
    {
        unsafe
        {
            var bytes = (byte*)&hash;
            var result = (ulong)bytes[4];

            for (var i = 3; i != -1; i--)
            {
                result = bytes[i] | (result << 8);
            }

            return result;
        }
    }
}

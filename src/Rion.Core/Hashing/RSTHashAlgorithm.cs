// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO.Hashing;

namespace Rion.Core.Hashing;

/// <summary>
/// Provides a collection of predefined instances of <see cref="IRSTHashAlgorithm"/> for different rst hash scenarios.
/// </summary>
public static class RSTHashAlgorithm
{
    /// <summary>
    /// Represents a predefined instance of RSTHashAlgorithm configured with BitsMask40, for v2 and v3.
    /// </summary>
    public static IRSTHashAlgorithm LegacyV2V3 { get; } = new LegacyRSTHashAlgorithm(RSTHashBitsMaskType.Mask40);

    /// <summary>
    /// Represents a predefined instance of RSTHashAlgorithm configured with BitsMask39, for v4 and v5.
    /// </summary>
    public static IRSTHashAlgorithm LegacyV4V5 { get; } = new LegacyRSTHashAlgorithm(RSTHashBitsMaskType.Mask39);

    /// <summary>
    /// Represents a predefined instance of RSTHashAlgorithm configured with BitsMask39 and TrimHigh3BytesWithMaskLow8Bits, for v5+(greater than v14.15).
    /// </summary>
    public static IRSTHashAlgorithm Latest { get; } = new LatestRSTHashAlgorithm(RSTHashBitsMaskType.Mask39, RSTHashTrimmingOption.TrimHigh3BytesWithMaskLow8Bits);

    /// <summary>
    /// Represents a predefined instance of RSTHashAlgorithm configured with BitsMask39, for v5+(greater than v14.15).
    /// </summary>
    private sealed class LatestRSTHashAlgorithm : RSTHashAlgorithmBase
    {
        /// <summary>
        /// Represents an enumeration of different BitsMask types used by the rst hash algorithm.
        /// </summary>
        internal LatestRSTHashAlgorithm(RSTHashBitsMaskType bitsMaskType, RSTHashTrimmingOption trimmingOption) : base(bitsMaskType, trimmingOption) { }

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

    /// <summary>
    /// [Legacy] Represents the legacy implementation of the rst hash algorithm, offering specific bits mask types for hashing operations.
    /// </summary>
    private sealed class LegacyRSTHashAlgorithm : RSTHashAlgorithmBase
    {
        /// <summary>
        /// Represents a legacy implementation of the rst hash algorithm with predefined bits mask types for hashing operations.
        /// </summary>
        internal LegacyRSTHashAlgorithm(RSTHashBitsMaskType bitsMaskType) : base(bitsMaskType) { }

        /// <inheritdoc />
        public override ulong Hash(ReadOnlySpan<byte> source) => XxHash64.HashToUInt64(source) & BitsMaskValue;
    }
}

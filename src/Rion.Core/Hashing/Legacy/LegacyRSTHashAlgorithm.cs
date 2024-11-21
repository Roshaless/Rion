// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO.Hashing;

namespace Rion.Core.Hashing.Legacy;

/// <summary>
/// [Legacy] Represents the legacy implementation of the RST hash algorithm, offering
/// specific bits mask types for hashing operations.
/// </summary>
public sealed class LegacyRSTHashAlgorithm : RSTHashAlgorithmBase
{
    /// <summary>
    /// Represents a legacy implementation of the RST hash algorithm with predefined bits mask types for hashing operations.
    /// </summary>
    private LegacyRSTHashAlgorithm(RSTHashBitsMaskType bitsMaskType) : base(bitsMaskType) { }

    /// <summary>
    /// Represents a predefined instance of <see cref="LegacyRSTHashAlgorithm"/> configured with <see cref="RSTHashBitsMaskType.Mask39"/>.
    /// This is specifically designed for compatibility with legacy RST hash algorithm versions, targeting v4 and v5 (prior to v14.15).
    /// </summary>
    public static LegacyRSTHashAlgorithm BitsMask39 { get; } = new(RSTHashBitsMaskType.Mask39);

    /// <summary>
    /// Represents a predefined instance of <see cref="LegacyRSTHashAlgorithm"/> configured with <see cref="RSTHashBitsMaskType.Mask40"/>.
    /// Designed for compatibility with specific legacy RST hash scenarios, this property provides a mask for 40-bit hashing operations.
    /// </summary>
    public static LegacyRSTHashAlgorithm BitsMask40 { get; } = new(RSTHashBitsMaskType.Mask40);

    /// <inheritdoc />
    public override ulong Hash(ReadOnlySpan<byte> source) => XxHash64.HashToUInt64(source) & BitsMaskValue;
}

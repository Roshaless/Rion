// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO.Hashing;

namespace Rion.Core.Hashing.Legacy;

/// <summary>
/// [Legacy] Provides an implementation of the rst hash algorithm.
/// </summary>
public sealed class LegacyRSTHashAlgorithm : RSTHashAlgorithmBase
{
    /// <summary>
    /// [Legacy] Gets the rst hash algorithm of BitsMask39, for rst v4, v5(less than v14.15).
    /// </summary>
    public static LegacyRSTHashAlgorithm BitsMask39 { get; } = new(RSTHashBitsMaskType.Mask39);

    /// <summary>
    /// [Legacy] Gets the rst hash algorithm of BitsMask40, for rst v2, v3.
    /// </summary>
    public static LegacyRSTHashAlgorithm BitsMask40 { get; } = new(RSTHashBitsMaskType.Mask40);

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyRSTHashAlgorithm"/> class based on the specified BitsMask type.
    /// </summary>
    /// <param name="bitsMaskType">The specified bits mask type.</param>
    private LegacyRSTHashAlgorithm(RSTHashBitsMaskType bitsMaskType) : base(bitsMaskType) { }

    /// <inheritdoc/>
    public override ulong Hash(ReadOnlySpan<byte> source) => XxHash64.HashToUInt64(source) & BitsMaskValue;
}

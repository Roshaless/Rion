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
public partial class RSTHashAlgorithm
{
    /// <summary>
    /// Creates an instance of <see cref="IRSTHashAlgorithm"/> for XxHash3 hashing.
    /// </summary>
    /// <param name="options">The options for configuring the XxHash3 algorithm.</param>
    private sealed class XxHash3Impl(RSTHashAlgorithmOptions options) : RSTHashAlgorithm(options)
    {

        /// <inheritdoc />
        public override ulong Hash(ReadOnlySpan<byte> source) => XxHash3.HashToUInt64(source) & BitMask.Value;
    }
}

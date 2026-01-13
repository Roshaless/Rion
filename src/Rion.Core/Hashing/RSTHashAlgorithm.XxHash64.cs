// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO.Hashing;

namespace Rion.Core.Hashing;

/// <summary>
/// Provides a collection of predefined instances of <see cref="RSTHashAlgorithm"/> for different rst hash scenarios.
/// </summary>
public partial class RSTHashAlgorithm
{
    /// <summary>
    /// Represents an implementation of <see cref="RSTHashAlgorithm"/> for XxHash64.
    /// </summary>
    /// <param name="options">The options for configuring the XxHash64 algorithm.</param>
    private sealed class XxHash64Impl(RSTHashAlgorithmOptions options) : RSTHashAlgorithm(options)
    {
        /// <inheritdoc />
        public override ulong Hash(ReadOnlySpan<byte> source) => XxHash64.HashToUInt64(source) & BitMask.Value;
    }
}

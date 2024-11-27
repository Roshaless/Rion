// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

namespace Rion.Core.Hashing;

/// <summary>
/// Represents the options for trimming the hash value.
/// </summary>
public enum RSTHashTrimmingOption
{
    /// <summary>
    /// No trimming option.
    /// </summary>
    None,

    /// <summary>
    /// Trim the high 3 bytes of the hash value with a mask of the low 8 bits.
    /// </summary>
    TrimHigh3BytesWithMaskLow8Bits
}

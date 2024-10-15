// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

namespace Rion.Core.Hashing;

/// <summary>
/// The type of BitsMask used to rst hash algorithm.
/// </summary>
public enum RSTHashBitsMaskType
{
    /// <summary>
    /// Nothing...
    /// </summary>
    None = 0x00,

    /// <summary>
    /// BitsMask 39, using by RST v4, v5.
    /// </summary>
    Mask39 = 39,

    /// <summary>
    /// BitsMask 40, using by RST v2, v3.
    /// </summary>
    Mask40 = 40
}

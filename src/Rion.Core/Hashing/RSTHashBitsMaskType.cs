// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

namespace Rion.Core.Hashing;

/// <summary>
/// Specifies the types of bit masks utilized in the rst hashing algorithm.
/// Each mask type corresponds to a specific number of bits used during the hashing process.
/// </summary>
public enum RSTHashBitsMaskType
{
    /// <summary>
    /// Nothing...
    /// </summary>
    None = 0x00,

    /// <summary>
    /// BitsMask 39, using by rst v4, v5.
    /// </summary>
    Mask39 = 39,

    /// <summary>
    /// BitsMask 40, using by rst v2, v3.
    /// </summary>
    Mask40 = 40
}

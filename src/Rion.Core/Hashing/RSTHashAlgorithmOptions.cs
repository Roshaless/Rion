// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

namespace Rion.Core.Hashing;

/// <summary>
/// Represents configuration options for an rst hash algorithm, including the format and bit mask settings.
/// </summary>
public readonly record struct RSTHashAlgorithmOptions
{
    /// <summary>
    /// Gets the predefined hash algorithm options for Version 2, using the XxHash64 format with a 40-bit mask.
    /// </summary>
    public static RSTHashAlgorithmOptions Version2 => new(RSTHashAlgorithmFormat.XxHash64, new(40));

    /// <summary>
    /// Gets the predefined hash algorithm options for Version 3, using XxHash64 with a 40-bit mask.
    /// </summary>
    public static RSTHashAlgorithmOptions Version3 => new(RSTHashAlgorithmFormat.XxHash64, new(40));

    /// <summary>
    /// Gets the predefined options for the Version 4 hash algorithm, using XxHash64 with a 39-bit mask.
    /// </summary>
    public static RSTHashAlgorithmOptions Version4 => new(RSTHashAlgorithmFormat.XxHash64, new(39));

    /// <summary>
    /// Gets the predefined hash algorithm options for Version 5, using the XxHash64 format with a 39-bit mask.
    /// </summary>
    public static RSTHashAlgorithmOptions Version5 => new(RSTHashAlgorithmFormat.XxHash64, new(39));

    /// <summary>
    /// Gets the hash algorithm options for version 5, patch 1415, using the XxHash3 format with a 39-bit mask.
    /// </summary>
    public static RSTHashAlgorithmOptions Version5Patch1415 => new(RSTHashAlgorithmFormat.XxHash3, new(39));

    /// <summary>
    /// Gets the hash algorithm options corresponding to version 5, patch 1502, using the XxHash3 format with a 38-bit mask.
    /// </summary>
    public static RSTHashAlgorithmOptions Version5Patch1502 => new(RSTHashAlgorithmFormat.XxHash3, new(38));


    /// <summary>
    /// Specifies the hash algorithm format used by the rst hash operation.
    /// </summary>
    public readonly RSTHashAlgorithmFormat Format;

    /// <summary>
    /// Specifies the bitmask used to select or filter hash bits in the rst hash operation.
    /// </summary>
    public readonly RSTHashAlgorithmBitMask BitMask;

    /// <summary>
    /// Initializes a new instance of the <see cref="RSTHashAlgorithmOptions"/> struct with the specified hash algorithm format and bit
    /// mask.
    /// </summary>
    /// <param name="format">The format of the hash algorithm to use. Determines the output encoding or structure of the hash value.</param>
    /// <param name="bitMask">A bit mask specifying which bits of the hash are used or considered. Controls the effective length or selection
    /// of the hash output.</param>
    public RSTHashAlgorithmOptions(RSTHashAlgorithmFormat format, RSTHashAlgorithmBitMask bitMask)
    {
        Format = format;
        BitMask = bitMask;
    }
}

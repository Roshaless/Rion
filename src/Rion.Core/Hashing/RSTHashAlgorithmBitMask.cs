// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;

namespace Rion.Core.Hashing;

/// <summary>
/// Represents a bitmask with a specified number of bits, providing both the bit count and the corresponding mask value.
/// </summary>
/// <remarks>
/// This structure is immutable and is designed to represent a bitmask of up to 64 bits. The bitmask value is
/// calculated as a 64-bit unsigned integer where the least significant <see cref="Bits"/> bits are set to 1.
/// </remarks>
public readonly record struct RSTHashAlgorithmBitMask
{
    /// <summary>
    /// Gets the number of bits represented by this instance.
    /// </summary>
    public readonly int Bits;

    /// <summary>
    /// Gets the bitmask value represented as an unsigned 64-bit integer.
    /// </summary>
    public readonly ulong Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="RSTHashAlgorithmBitMask"/> class with the specified number of bits.
    /// </summary>
    /// <param name="bits">The number of bits to include in the mask. Must be between 1 and 64, inclusive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="bits"/> is less than 1 or greater than 64.</exception>
    public RSTHashAlgorithmBitMask(int bits)
    {
        if (bits is < 1 or > 64)
        {
            throw new ArgumentOutOfRangeException(nameof(bits), "Bits must be between 1 and 64.");
        }

        Bits = bits;
        Value = (1UL << bits) - 1;
    }

    /// <summary>
    /// Defines an explicit conversion from an integer value to an instance of the RSTHashBitsMask structure.
    /// </summary>
    /// <param name="value">The integer value to convert to an RSTHashBitsMask.</param>
    public static explicit operator RSTHashAlgorithmBitMask(int value) => new(value);
}

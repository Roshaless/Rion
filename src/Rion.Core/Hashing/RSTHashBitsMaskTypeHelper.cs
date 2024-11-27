// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Runtime.CompilerServices;

namespace Rion.Core.Hashing;

/// <summary>
/// Provides utility methods to obtain bits mask values based on <see cref="RSTHashBitsMaskType"/>.
/// These masks are integral in hash computations within the rst hashing algorithm.
/// </summary>
public static class RSTHashBitsMaskTypeHelper
{
    /// <summary>
    /// Retrieves the bits mask value corresponding to the specified <see cref="RSTHashBitsMaskType"/>.
    /// This value is utilized in the computation of hashes.
    /// </summary>
    /// <param name="type">The bits mask type which defines the number of bits to be used in hashing.</param>
    /// <returns>The computed bits mask value based on the provided type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetBitsMaskValue(this RSTHashBitsMaskType type) => (1UL << (int)type) - 1;
}

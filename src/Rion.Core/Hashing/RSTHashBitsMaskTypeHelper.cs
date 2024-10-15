// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Runtime.CompilerServices;

namespace Rion.Core.Hashing;

public static class RSTHashBitsMaskTypeHelper
{
    /// <summary>
    /// Get the value of BitsMask used to computes hashes.
    /// </summary>
    /// <param name="type">The type of <see cref="RSTFile"/>.</param>
    /// <returns>The bits mask value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetBitsMaskValue(this RSTHashBitsMaskType type) => (1UL << (int)type) - 1;
}

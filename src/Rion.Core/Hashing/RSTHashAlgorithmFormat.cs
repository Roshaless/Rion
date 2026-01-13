// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

namespace Rion.Core.Hashing;

/// <summary>
/// Specifies the available formats for non-cryptographic hash algorithms supported by the rst library.
/// </summary>
/// <remarks>Use this enumeration to select the hash algorithm format when computing hash values for checksums,
/// hash tables, or data integrity checks. The algorithms represented by this enumeration are optimized for speed and
/// are not suitable for cryptographic security purposes.</remarks>
public enum RSTHashAlgorithmFormat
{
    /// <summary>
    /// Provides methods for computing fast, non-cryptographic XxHash3 hash values.
    /// </summary>
    /// <remarks>XxHash3 is a high-speed hash algorithm suitable for checksums, hash tables, and data
    /// deduplication where cryptographic security is not required. This class is not intended for cryptographic
    /// purposes.</remarks>
    XxHash3,

    /// <summary>
    /// Provides methods for computing 64-bit XxHash64 hash values.
    /// </summary>
    /// <remarks>XxHash64 is a fast, non-cryptographic hash algorithm suitable for checksums, hash tables, and
    /// data integrity checks. This class is not intended for cryptographic purposes.</remarks>
    XxHash64
}

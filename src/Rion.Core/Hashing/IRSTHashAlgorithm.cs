// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Globalization;
using System.Text;

namespace Rion.Core.Hashing;

/// <summary>
/// Defines the contract for RST hash algorithm, providing methods to compute
/// hash values for strings and byte spans with optional cultural and encoding considerations.
/// </summary>
public interface IRSTHashAlgorithm
{
    /// <summary>
    /// Specifies the type of bits mask applied in the rst hashing algorithm.
    /// </summary>
    RSTHashBitsMaskType BitsMaskType { get; }

    /// <summary>
    /// Represents the value of the bits mask used in the rst hashing algorithm.
    /// This value is derived from the configured <see cref="RSTHashBitsMaskType"/>.
    /// </summary>
    ulong BitsMaskValue { get; }

    /// <summary>
    /// Specifies the type of trimming option applied in the rst hashing algorithm.
    /// </summary>
    RSTHashTrimmingOption TrimmingOption { get; }

    /// <summary>
    /// Computes the hash value for the provided string using the default encoding and culture.
    /// This method is part of the <see cref="IRSTHashAlgorithm"/> interface implementation.
    /// </summary>
    /// <param name="toHash">The string to be hashed.</param>
    /// <returns>The computed hash value as an unsigned 64-bit integer.</returns>
    ulong Hash(ReadOnlySpan<char> toHash);

    /// <summary>
    /// Computes the rst hash of the string using the specified character encoding.
    /// This method extends the <see cref="Hash(ReadOnlySpan{char})"/> by allowing custom encodings.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="encoding">The character encoding to use when converting the string to bytes.</param>
    /// <returns>The computed rst hash value as an unsigned 64-bit integer.</returns>
    ulong Hash(ReadOnlySpan<char> toHash, Encoding encoding);

    /// <summary>
    /// Computes the hash value for the provided string using the specified encoding and culture.
    /// This method extends the basic hashing functionality by allowing cultural casing considerations.
    /// </summary>
    /// <param name="toHash">The string to be hashed.</param>
    /// <param name="encoding">The character encoding to use for converting the string. If null, UTF8 is used.</param>
    /// <param name="culture">Specifies culture-specific casing rules for string normalization. If null, current culture is used.</param>
    /// <returns>The computed hash value as an unsigned 64-bit integer, adjusted with optional bit masking based on <see cref="BitsMaskType"/>.</returns>
    ulong Hash(ReadOnlySpan<char> toHash, Encoding encoding, CultureInfo culture);

    /// <summary>
    /// Computes the rst hash of the provided data without any encoding or culture consideration,
    /// directly operating on the byte span.
    /// </summary>
    /// <param name="source">The data as a read-only byte span to hash.</param>
    /// <returns>The computed rst hash value as an unsigned 64-bit integer.</returns>
    ulong Hash(ReadOnlySpan<byte> source);

    /// <summary>
    /// Computes the rst hash of the string with an additional offset value.
    /// This method extends the basic hashing functionality by incorporating an offset
    /// into the hash calculation process.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="offset">The offset value to be used in the hash computation, modifying the result.</param>
    /// <returns>The computed rst hash value adjusted by the specified offset.</returns>
    ulong HashWithOffset(ReadOnlySpan<char> toHash, long offset);

    /// <summary>
    /// Computes the RST hash of a string with an applied offset, considering the specified encoding.
    /// This method extends the basic hashing by incorporating an offset to vary the resulting hash value.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="offset">The offset value to incorporate into the hash computation.</param>
    /// <param name="encoding">Character encoding used for the string conversion.</param>
    /// <returns>The computed rst hash value adjusted by the specified offset.</returns>
    ulong HashWithOffset(ReadOnlySpan<char> toHash, long offset, Encoding encoding);

    /// <summary>
    /// Computes the rst hash of the string with an additional offset value,
    /// considering cultural and encoding settings specified.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="offset">The offset value to incorporate into the hash computation.</param>
    /// <param name="encoding">Character encoding used for the string conversion.</param>
    /// <param name="culture">Cultural context applied for string processing.</param>
    /// <returns>The computed rst hash value adjusted by the specified offset.</returns>
    ulong HashWithOffset(ReadOnlySpan<char> toHash, long offset, Encoding encoding, CultureInfo culture);

    /// <summary>
    /// Computes the rst hash of the bytes with an additional offset value.
    /// </summary>
    /// <param name="source">The bytes to be hashed.</param>
    /// <param name="offset">The offset value to incorporate into the hash computation.</param>
    /// <returns>The computed hash value with offset applied, as an unsigned 64-bit integer.</returns>
    ulong HashWithOffset(ReadOnlySpan<byte> source, long offset);

    /// <summary>
    /// Calculates a new rst hash value by incorporating an offset into the original hash.
    /// </summary>
    /// <param name="hash">The original rst hash value to which the offset will be applied.</param>
    /// <param name="offset">The offset to be added to the hash value, modifying it based on the current bits mask type.</param>
    /// <returns>The modified rst hash value including the specified offset.</returns>
    ulong WithOffset(ulong hash, long offset);
}

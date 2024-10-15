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
/// Represents a rst hash algorithm.
/// </summary>
public interface IRSTHashAlgorithm
{
    /// <summary>
    /// Gets the current BitsMask type.
    /// </summary>
    RSTHashBitsMaskType BitsMaskType { get; }

    /// <summary>
    /// Gets the value of current BitsMask type.
    /// </summary>
    ulong BitsMaskValue { get; }

    /// <summary>
    /// Computes the rst hash of the string.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <returns>The computed rst hash.</returns>
    ulong Hash(string toHash);

    /// <summary>
    /// Computes the rst hash of the string.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <returns>The computed rst hash.</returns>
    ulong Hash(string toHash, Encoding encoding);

    /// <summary>
    /// Computes the rst hash of the string.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    /// <returns>The computed rst hash.</returns>
    ulong Hash(string toHash, Encoding encoding, CultureInfo culture);

    /// <summary>
    /// Computes the rst hash of the provided data.
    /// </summary>
    /// <param name="source">The data to hash.</param>
    /// <returns>The computed rst hash.</returns>
    ulong Hash(ReadOnlySpan<byte> source);

    /// <summary>
    /// Computes the rst hash of the string.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="offset">The offset value for this hash computation.</param>
    /// <returns>The computed rst hash.</returns>
    ulong HashWithOffset(string toHash, long offset);

    /// <summary>
    /// Computes the rst hash of the string.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="offset">The offset value for this hash computation.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <returns>The computed rst hash.</returns>
    ulong HashWithOffset(string toHash, long offset, Encoding encoding);

    /// <summary>
    /// Computes the rst hash of the string.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="offset">The offset value for this hash computation.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    /// <returns>The computed rst hash.</returns>
    ulong HashWithOffset(string toHash, long offset, Encoding encoding, CultureInfo culture);

    /// <summary>
    /// Computes the rst hash of the provided data.
    /// </summary>
    /// <param name="source">The data to hash.</param>
    /// <param name="offset">The offset value for this hash computation.</param>
    /// <returns>The computed rst hash.</returns>
    ulong HashWithOffset(ReadOnlySpan<byte> source, long offset);

    /// <summary>
    /// Computes the new rst hash with offset value.
    /// </summary>
    /// <param name="hash">The cumputed rst hash.</param>
    /// <param name="offset">The offset value for this hash computation.</param>
    /// <returns>The new rst hash with offset value.</returns>
    ulong WithOffset(ulong hash, long offset);
}

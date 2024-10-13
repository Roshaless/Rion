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
/// 
/// </summary>
public interface IRSTHashAlgorithm
{
    /// <summary>
    /// 
    /// </summary>
    RSTHashBitsMaskType BitsMaskType { get; }

    /// <summary>
    /// 
    /// </summary>
    ulong BitsMaskValue { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toHash"></param>
    /// <returns></returns>
    ulong Hash(string toHash);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toHash"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    ulong Hash(string toHash, Encoding encoding);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toHash"></param>
    /// <param name="encoding"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    ulong Hash(string toHash, Encoding encoding, CultureInfo cultureInfo);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source">The data to hash.</param>
    /// <returns></returns>
    ulong Hash(ReadOnlySpan<byte> source);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toHash"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    ulong HashWithOffset(string toHash, long offset);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toHash"></param>
    /// <param name="offset"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    ulong HashWithOffset(string toHash, long offset, Encoding encoding);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toHash"></param>
    /// <param name="offset"></param>
    /// <param name="encoding"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    ulong HashWithOffset(string toHash, long offset, Encoding encoding, CultureInfo cultureInfo);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    ulong HashWithOffset(ReadOnlySpan<byte> source, long offset);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    ulong WithOffset(ulong hash, long offset);
}

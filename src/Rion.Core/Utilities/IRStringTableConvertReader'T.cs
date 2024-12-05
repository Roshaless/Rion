// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;

namespace Rion.Core.Utilities;

/// <summary>
/// Provides a method Read a string table from a byte array.
/// </summary>
/// <typeparam name="T">The type of the string table.</typeparam>
public interface IRStringTableConvertReader<out T> where T : IRStringTable
{
    /// <summary>
    /// Read a string table from a byte array.
    /// </summary>
    /// <param name="bytes">The byte array to read from.</param>
    /// <returns>The string table read from the byte array.</returns>
    T? Read(ReadOnlySpan<byte> bytes);
}

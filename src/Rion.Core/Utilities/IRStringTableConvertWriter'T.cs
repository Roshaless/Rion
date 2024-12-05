// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.IO;

namespace Rion.Core.Utilities;

/// <summary>
/// Provides a method to write a string table to a stream.
/// </summary>
/// <typeparam name="T">The type of the string table.</typeparam>
public interface IRStringTableConvertWriter<in T> where T : IRStringTable
{
    /// <summary>
    /// Write a string table to a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The string table to write.</param>
    void Write(Stream stream, T value);
}

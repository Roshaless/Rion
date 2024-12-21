// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

using Rion.Core.Conversion;

namespace Rion.Core;

partial class RConvert
{
    // ======================================================================================
    // ======================================== JSON ========================================
    // ======================================================================================

    /// <summary>
    /// Reads a string table from a byte array using the JSON converter.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The string table converted from the byte array, or <see langword="null"/> if an error occurs.</returns>
    public static RStringTable FromJson(ReadOnlySpan<byte> bytes)
        => From<RStringTable>(bytes, RStringTableConverter.JsonConverter);

    /// <summary>
    /// Reads a string table from a JSON file.
    /// </summary>
    /// <param name="path">The path of the JSON file.</param>
    /// <returns>The string table read from the JSON file, or null if an error occurs.</returns>
    public static RStringTable FromJsonFile(string path)
        => FromFile<RStringTable>(path, RStringTableConverter.JsonConverter);

    /// <summary>
    /// Writes a string table to a stream using the JSON converter.
    /// </summary>
    /// <param name="output">The stream to write the string table to.</param>
    /// <param name="value">The string table to write to the stream.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    public static void ToJson<T>(Stream output, T value) where T : IRStringTable
        => To(output, value, RStringTableConverter.JsonConverter);

    /// <summary>
    /// Writes a string table to a JSON file.
    /// </summary>
    /// <param name="outputPath">The path of the output JSON file.</param>
    /// <param name="value">The string table to write to the JSON file.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    public static void ToJsonFile<T>(string outputPath, T value) where T : IRStringTable
        => ToFile(outputPath, value, RStringTableConverter.JsonConverter);
}

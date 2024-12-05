// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.IO;

using Rion.Core.Buffers;
using Rion.Core.Utilities;

namespace Rion.Core;

/// <summary>
/// Provides methods for converting string tables between different formats.
/// </summary>
public static class RSTFileConvert
{
    /// <summary>
    /// Converts a string table from a file to a specified type.
    /// </summary>
    /// <param name="path">The path to the file containing the string table.</param>
    /// <param name="converter">The converter used to read the string table.</param>
    /// <typeparam name="T">The type of the string table to convert.</typeparam>
    /// <returns>The converted string table, or null if the conversion fails.</returns>
    public static T? From<T>(string path, IRStringTableConvertReader<T> converter) where T : IRStringTable
    {
        using var scope = RFileBufferScope.CreateFrom(path);
        {
            return converter.Read(scope.Span);
        }
    }

    /// <summary>
    /// Converts a string table to a file using a specified converter.
    /// </summary>
    /// <param name="outputPath">The path to the output file.</param>
    /// <param name="value">The string table to convert.</param>
    /// <param name="converter">The converter used to write the string table.</param>
    /// <typeparam name="T">The type of the string table to convert.</typeparam>
    public static void To<T>(string outputPath, T value, IRStringTableConvertWriter<T> converter) where T : IRStringTable
    {
        using var stream = OpenOrCreate(outputPath);
        {
            converter.Write(stream, value);
        }
    }

    /// <summary>
    /// Opens or creates a file stream at the specified path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>A file stream for writing.</returns>
    private static FileStream OpenOrCreate(string path) => new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);


    // ======================================================================================
    // ======================================== JSON ========================================
    // ======================================================================================

    /// <summary>
    /// Reads a string table from a JSON file.
    /// </summary>
    /// <param name="path">The path of the JSON file.</param>
    /// <returns>The string table read from the JSON file, or null if an error occurs.</returns>
    public static RStringTable? FromJson(string path) => From(path, RStringTableConverter.JsonConverter);

    /// <summary>
    /// Writes a string table to a JSON file.
    /// </summary>
    /// <param name="outputPath">The path of the output JSON file.</param>
    /// <param name="value">The string table to write to the JSON file.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    public static void ToJson<T>(string outputPath, T value) where T : IRStringTable => To(outputPath, value, RStringTableConverter.JsonConverter);

    /// <summary>
    /// Converts a string table from a file to a JSON format and saves it to another file.
    /// </summary>
    /// <param name="inputPath">The path to the input file.</param>
    /// <param name="outputPath">The path to the output file.</param>
    public static void ToJson(string inputPath, string outputPath)
    {
        using var scope = RFileBufferScope.CreateFrom(inputPath);
        {
            var reader = new RStringTableReader(scope.Span);
            {
                var stringTable = new RStringTable.RecordRStringTable(reader.Metadata, reader.ReadAll());
                {
                    To(outputPath, stringTable, RStringTableConverter.JsonConverter);
                }
            }
        }
    }
}

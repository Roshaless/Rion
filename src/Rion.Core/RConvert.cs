// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Runtime.CompilerServices;

using Rion.Core.Buffers;

using RStringTableConverter = Rion.Core.Conversion.RStringTableConverter;

namespace Rion.Core;

/// <summary>
/// Contains methods for converting between different string table formats.
/// </summary>
public static class RConvert
{
    /// <summary>
    /// Reads a string table from a file using a specified converter.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <param name="converter">The converter to use for reading the string table.</param>
    /// <typeparam name="T">The type of the string table to read.</typeparam>
    /// <returns>The string table read from the file, or null if an error occurs.</returns>
    public static T? FromFile<T>(string path, RStringTableConverter converter) where T : IRStringTable
    {
        if (converter.CanConvert(typeof(T)))
        {
            using var scope = RFileBufferScope.CreateFrom(path);
            {
                var result = converter.ConvertCore(scope.Span);
                if (result is not null) return Unsafe.As<object, T>(ref result);
            }
        }

        return Unsafe.NullRef<T>();
    }

    /// <summary>
    /// Writes a string table to a file using a specified converter.
    /// </summary>
    /// <param name="outputPath">The path of the output file.</param>
    /// <param name="value">The string table to write to the file.</param>
    /// <param name="converter">The converter to use for writing the string table.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    public static void ToFile<T>(string outputPath, T value, RStringTableConverter converter) where T : IRStringTable
    {
        if (converter.CanWrite(value.GetType()))
        {
            using var stream = RFile.OpenOrCreate(outputPath);
            {
                converter.WriteCore(stream, value);
            }
        }
    }

    // ======================================================================================
    // ======================================== JSON ========================================
    // ======================================================================================

    /// <summary>
    /// Reads a string table from a JSON file.
    /// </summary>
    /// <param name="path">The path of the JSON file.</param>
    /// <returns>The string table read from the JSON file, or null if an error occurs.</returns>
    public static RStringTable? FromJsonFile(string path)
        => FromFile<RStringTable>(path, RStringTableConverter.JsonConverter);

    /// <summary>
    /// Writes a string table to a JSON file.
    /// </summary>
    /// <param name="outputPath">The path of the output JSON file.</param>
    /// <param name="value">The string table to write to the JSON file.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    public static void ToJsonFile<T>(T value, string outputPath)
        where T : IRStringTable => ToFile(outputPath, value, RStringTableConverter.JsonConverter);
}

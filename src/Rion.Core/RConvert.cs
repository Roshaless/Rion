// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

using Rion.Core.Buffers;
using Rion.Core.Conversion;

namespace Rion.Core;

/// <summary>
/// Contains methods for converting between different string table formats.
/// </summary>
public static class RConvert
{
    /// <summary>
    /// Reads a string table from a byte array using a specified converter.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <param name="converter">The converter to use for converting the byte array.</param>
    /// <typeparam name="T">The type of the string table to convert.</typeparam>
    /// <returns>The string table converted from the byte array, or <see langword="null"/> if an error occurs.</returns>
    public static T? From<T>(ReadOnlySpan<byte> bytes, RStringTableConverter converter) where T : IRStringTable
    {
        ArgumentNullException.ThrowIfNull(converter);

        return (converter.CanConvert(typeof(T)) &&
                converter.ConvertCore(bytes) is T result) ? result : default;
    }

    /// <summary>
    /// Reads a string table from a file using a specified converter.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <param name="converter">The converter to use for reading the string table.</param>
    /// <typeparam name="T">The type of the string table to read.</typeparam>
    /// <returns>The string table read from the file, or null if an error occurs.</returns>
    public static T? FromFile<T>(string path, RStringTableConverter converter) where T : IRStringTable
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentNullException.ThrowIfNull(converter);

        if (!converter.CanConvert(typeof(T))) return default;
        using var scope = RFileBufferScope.CreateFrom(path);

        return converter.ConvertCore(scope.Span) is T result ? result : default;
    }

    /// <summary>
    /// Writes a string table to a stream using a specified converter.
    /// </summary>
    /// <param name="output">The stream to write the string table to.</param>
    /// <param name="value">The string table to write to the stream.</param>
    /// <param name="converter">The converter to use for writing the string table.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    public static void To<T>(Stream output, T value, RStringTableConverter converter) where T : IRStringTable
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(converter);

        if (converter.CanWrite(value.GetType()))
        {
            converter.WriteCore(output, value);
        }
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
        ArgumentException.ThrowIfNullOrEmpty(outputPath);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(converter);

        if (!converter.CanWrite(value.GetType())) return;
        using var stream = RFile.OpenOrCreate(outputPath);

        converter.WriteCore(stream, value);
    }

    // ======================================================================================
    // ======================================== JSON ========================================
    // ======================================================================================

    /// <summary>
    /// Reads a string table from a byte array using the JSON converter.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The string table converted from the byte array, or <see langword="null"/> if an error occurs.</returns>
    public static RStringTable? FromJson(ReadOnlySpan<byte> bytes)
        => From<RStringTable>(bytes, RStringTableConverter.JsonConverter);

    /// <summary>
    /// Reads a string table from a JSON file.
    /// </summary>
    /// <param name="path">The path of the JSON file.</param>
    /// <returns>The string table read from the JSON file, or null if an error occurs.</returns>
    public static RStringTable? FromJsonFile(string path)
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

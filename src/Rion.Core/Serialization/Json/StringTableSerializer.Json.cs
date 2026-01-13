// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

namespace Rion.Core.Serialization;

public static partial class StringTableSerializer
{
    /// <summary>
    /// Deserializes a JSON-encoded byte span into an <see cref="StringTable"/> object.
    /// </summary>
    /// <param name="bytes">A span of bytes containing the JSON data to deserialize. The span must represent a valid JSON-encoded <see
    /// cref="StringTable"/> object.</param>
    /// <returns>An <see cref="StringTable"/> object deserialized from the provided JSON data.</returns>
    public static StringTable DeserializeFromJson(Span<byte> bytes)
    {
        return StringTableJsonConverter.Instance.Deserialize(bytes);
    }

    /// <summary>
    /// Deserializes a JSON-encoded byte span into an <see cref="StringTable"/> object.
    /// </summary>
    /// <remarks>This method uses the <see cref="StringTableJsonConverter"/> to perform the deserialization.
    /// Ensure that the input JSON conforms to the expected structure of an <see cref="StringTable"/>.</remarks>
    /// <param name="bytes">A read-only span of bytes containing the JSON-encoded data to deserialize.</param>
    /// <returns>An instance of <see cref="StringTable"/> representing the deserialized data.</returns>
    public static StringTable DeserializeFromJson(ReadOnlySpan<byte> bytes)
    {
        return StringTableJsonConverter.Instance.Deserialize(bytes);
    }

    /// <summary>
    /// Serializes the specified string table to JSON format and writes it to the provided output stream.
    /// </summary>
    /// <param name="output">The stream to which the JSON representation of the string table will be written. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="stringTable">The string table to serialize. Cannot be <see langword="null"/>.</param>
    public static void SerializeToJson(Stream output, IStringTable stringTable)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(stringTable);

        StringTableJsonConverter.Instance.Serialize(output, stringTable);
    }

    /// <summary>
    /// Serializes the specified string table to a JSON file at the given output path.
    /// </summary>
    /// <remarks>
    /// This method creates or overwrites the file at the specified <paramref name="outputPath"/>.
    /// The string table is serialized using a predefined JSON format.
    /// </remarks>
    /// <param name="outputPath">The file path where the JSON representation of the string table will be written. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="stringTable">The string table to serialize. Cannot be <see langword="null"/>.</param>
    public static void SerializeToJsonFile(string outputPath, IStringTable stringTable)
    {
        ArgumentNullException.ThrowIfNull(outputPath);
        ArgumentNullException.ThrowIfNull(stringTable);

        using var stream = Internal.FileOperations.OpenOrCreate(outputPath);
        StringTableJsonConverter.Instance.Serialize(stream, stringTable);
    }
}

// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

using Rion.Core.Internal;
using Rion.Core.Metadata;
using Rion.Core.Metadata.Legacy;

#pragma warning disable IDE0130

namespace Rion.Core.Serialization;

/// <summary>
/// Provides a JSON converter for serializing and deserializing <see cref="RStringTable"/> objects.
/// </summary>
/// <remarks>
/// This converter handles the serialization and deserialization of <see cref="RStringTable"/> instances,
/// including their metadata and entries. It ensures compatibility with the expected JSON structure,
/// which includes properties for version, configuration, and entries.
/// </remarks>
public sealed class RStringTableJsonConverter : RStringTableConverter<RStringTable, IRStringTable>
{
    /// <summary>
    /// Provides a thread-safe, lazily initialized instance of <see cref="RStringTableConverter{TTable, TInterface}"/>
    /// for converting between <see cref="RStringTable"/> and <see cref="IRStringTable"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="Lazy{T}"/> ensures that the instance is created only when it is first
    /// accessed, making it efficient for scenarios where the converter may not always be needed.
    /// </remarks>
    private static readonly Lazy<RStringTableConverter<RStringTable, IRStringTable>> s_lazy = new();

    /// <summary>
    /// Gets the singleton instance of the <see cref="RStringTableConverter{RStringTable, IRStringTable}"/> class.
    /// </summary>
    public static RStringTableConverter<RStringTable, IRStringTable> Instance => s_lazy.Value;

    /// The names of the JSON properties.
    private const string JsonConfigName = "config";
    private const string JsonEntriesName = "entries";
    private const string JsonVersionName = "version";

    /// <inheritdoc />
    public override RStringTable Deserialize(ReadOnlySpan<byte> bytes)
    {
        // Create a reader to read the JSON
        var reader = new Utf8JsonReader(bytes, new() { CommentHandling = JsonCommentHandling.Skip });

        // Read the JSON document
        var jsonDocument = JsonDocument.ParseValue(ref reader);

        // Check if the JSON has entries.
        var hasEntries = jsonDocument.RootElement.TryGetProperty(JsonEntriesName, out var entries);
        if (hasEntries is false || entries.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Invalid JSON format.");
        }

        // Read the metadata
        var metadata = TryReadMetadata(jsonDocument.RootElement) ?? RStringTableMetadata.Latest;

        // Create the string table
        var stringTable = RStringTable.Create(metadata, entries.EnumerateObject().Count());
        {
            foreach (var jsonObject in entries.EnumerateObject())
            {
                var property = jsonObject.Name.AsSpan();
                if (property.StartEndWith('{', '}'))
                    property = property.Slice(1, property.Length - 2);

                if (!property.TryParseHex<ulong>(out var hash))
                    hash = metadata.HashAlgorithm.Hash(property);

                stringTable[hash] = jsonObject.Value.ToString();
            }
        }

        return stringTable;

        static IRStringTableMetadata? TryReadMetadata(JsonElement rootElement)
        {
            IRStringTableMetadata? metadata;
            if (rootElement.TryGetProperty(JsonVersionName, out var version))
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (version.ValueKind == JsonValueKind.String)
                {
                    RStringTableMetadata.TryGetMetadata(version.GetString(), out metadata);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            // ReSharper disable once InvertIf
            if (metadata is LegacyFontConfigMetadata legacyMetadata)
            {
                // Try read the font config
                // ReSharper disable once InvertIf
                if (rootElement.TryGetProperty(JsonConfigName, out var fontConfig))
                {
                    if (fontConfig.ValueKind == JsonValueKind.String)
                    {
                        legacyMetadata.FontConfig = fontConfig.GetString();
                    }
                }
            }

            return metadata;
        }
    }

    /// <inheritdoc />
    public override void Serialize(Stream stream, IRStringTable stringTable)
    {
        var writer = new Utf8JsonWriter(stream, new()
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        writer.WriteStartObject();
        {
            writer.WritePropertyName(JsonVersionName);
            writer.WriteStringValue(RStringTableMetadata.GetVersionString(stringTable.Metadata));

            if (stringTable.Metadata is ILegacyFontConfigMetadata legacyMetadata)
            {
                if (legacyMetadata.FontConfig.IsNotNullOrWhiteSpace())
                {
                    writer.WritePropertyName(JsonConfigName);
                    writer.WriteStringValue(legacyMetadata.FontConfig);
                }
            }

            writer.WritePropertyName(JsonEntriesName);
            writer.WriteStartObject();
            {
                foreach (var pair in stringTable.OrderBy(item => RHashtable.GetNameOrHexString(item.Key)))
                {
                    writer.WriteProperty(RHashtable.GetNameOrHexString(pair.Key), pair.Value);
                }
            }
            writer.WriteEndObject();
        }
        writer.WriteEndObject();
        writer.Flush();
    }
}

/// <summary>
/// Provides extension methods for working with JSON using <see cref="Utf8JsonWriter"/>.
/// </summary>
/// <remarks>This static class contains helper methods to simplify common JSON writing tasks.</remarks>
static file class JsonExtensions
{
    /// <summary>
    /// Writes a string property to the specified <see cref="Utf8JsonWriter"/>.
    /// </summary>
    /// <remarks>
    /// This method writes a property name and its corresponding string value to the JSON output.
    /// Ensure that the <see cref="Utf8JsonWriter"/> is in a valid state for writing a property before calling this method.
    /// </remarks>
    /// <param name="utf8JsonWriter">The <see cref="Utf8JsonWriter"/> instance to which the property will be written. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="prop">The name of the property to write. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="value">The string value of the property to write. Cannot be <see langword="null"/>.</param>
    internal static void WriteProperty(this Utf8JsonWriter utf8JsonWriter, string prop, string value)
    {
        utf8JsonWriter.WritePropertyName(prop);
        utf8JsonWriter.WriteStringValue(value);
    }
}


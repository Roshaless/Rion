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

namespace Rion.Core.Conversion;

public partial class RStringTableConverter
{
    /// <summary>
    /// A converter that can convert rst files to json.
    /// </summary>
    private sealed class JsonConverterImpl : RStringTableConverter<RStringTable, IRStringTable>
    {
        /// The names of the JSON properties.
        private const string JsonConfigName = "config";
        private const string JsonEntriesName = "entries";
        private const string JsonVersionName = "version";

        /// <inheritdoc />
        public override RStringTable? Convert(ReadOnlySpan<byte> bytes)
        {
            // Read the JSON.
            var reader = new Utf8JsonReader(bytes, new() { CommentHandling = JsonCommentHandling.Skip });
            if (JsonDocument.TryParseValue(ref reader, out var jsonDocument) is false) return null;

            // Read entries
            var hasEntries = jsonDocument.RootElement.TryGetProperty(JsonEntriesName, out var entries);
            if (hasEntries is false && entries.ValueKind != JsonValueKind.Object) return null;

            // Read the metadata
            var metadata = TryReadMetadata(jsonDocument.RootElement) ?? RStringTableMetadata.Latest;
            var stringTable = RStringTable.Create(metadata, entries.EnumerateObject().Count());
            {
                foreach (var jsonObject in entries.EnumerateObject())
                {
                    var property = jsonObject.Name;
                    if (property.IsNullOrWhiteSpace())
                        continue;

                    if (property.StartsWith('{'))
                        property = property.Substring(1, property.Length - 2);

                    if (!property.TryParse<ulong>(out var hash))
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
                    if (version.ValueKind == JsonValueKind.Number)
                    {
                        RStringTableMetadata.TryGetMetadata(version.GetInt32(), out metadata);
                    }
                    else if (version.ValueKind == JsonValueKind.String)
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
        public override void Write(Stream stream, IRStringTable value)
        {
            var writer = new Utf8JsonWriter(stream, new()
            {
                Indented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            writer.WriteStartObject();
            {
                writer.WritePropertyName(JsonVersionName);
                writer.WriteStringValue(value.Metadata.Version.ToString());

                if (value.Metadata is ILegacyFontConfigMetadata legacyMetadata)
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
                    foreach (var pair in value.OrderBy(item => RHashtable.GetNameOrHexString(item.Key)))
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
}

/// <summary>
/// Extensions for writing JSON.
/// </summary>
static file class JsonExtensions
{
    /// <summary>
    /// Write a property to the writer.
    /// </summary>
    /// <param name="utf8JsonWriter">The writer.</param>
    /// <param name="prop">The property name.</param>
    /// <param name="value">The value.</param>
    internal static void WriteProperty(this Utf8JsonWriter utf8JsonWriter, string prop, string value)
    {
        utf8JsonWriter.WritePropertyName(prop);
        utf8JsonWriter.WriteStringValue(value);
    }
}


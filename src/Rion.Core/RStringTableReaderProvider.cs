// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;

using Rion.Core.Buffers;
using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Provides an abstract base for implementing string table reading functionality from binary data sources.
/// This class outlines the structure necessary for parsing string table properties and individual strings,
/// enabling the creation of tailored readers for different binary formats or encoding schemes.
/// </summary>
public abstract class RStringTableReaderProvider
{
    /// <summary>
    /// Gets the provider for the latest supported string table.
    /// </summary>
    /// <remarks>Use this property to access the most recent version of the <see cref="RStringTableReaderProvider"/>.
    /// This is typically recommended unless a specific older version is required for compatibility.</remarks>
    public static RStringTableReaderProvider Default => PatchV1502;

    /// <summary>
    /// Gets the provider for reading string tables of version 15.2.
    /// </summary>
    public static RStringTableReaderProvider PatchV1502 { get; } = new PatchV1502ReaderProvider();

    /// <summary>
    /// Gets the provider for reading string tables of version 14.15.
    /// </summary>
    public static RStringTableReaderProvider PatchV1415 { get; } = new PatchV1415ReaderProvider();

    /// <summary>
    /// Gets a provider for reading string tables in the legacy V5 format.
    /// </summary>
    /// <remarks>Use this provider to read string tables that are stored in the legacy V5 format. This is
    /// intended for compatibility with older data sources that have not been migrated to newer formats.</remarks>
    public static RStringTableReaderProvider LegacyV5 { get; } = new LegacyV5ReaderProvider();

    /// <summary>
    /// Abstract method to read and extract the properties of a string table from the provided binary data reader.
    /// Implementations must parse the binary data to populate an instance of <see cref="RStringTableFileProperties"/>.
    /// </summary>
    /// <param name="reader">An instance of <see cref="RBufferReader"/> used to read the string table properties from binary data.</param>
    /// <returns>A <see cref="RStringTableFileProperties"/> object containing the extracted string table properties.</returns>
    public abstract RStringTableFileProperties ReadFileProperties(RBufferReader reader);

    /// <summary>
    /// Reads a string from the provided binary data span using the implemented reader logic.
    /// This method is designed to be overridden by subclasses to handle specific string encoding
    /// or format peculiarities present in the binary data.
    /// </summary>
    /// <param name="span">A read-only span of bytes representing the string in binary format.</param>
    /// <returns>The decoded string extracted from the binary data span.</returns>
    public abstract string ReadString(ReadOnlySpan<byte> span);

    /// <summary>
    /// Provides the default implementation for reading string table properties and strings from binary data.
    /// This class is a concrete subclass of <see cref="RStringTableReaderProvider" />, offering standard behavior
    /// for interpreting string table files, property extraction, and string decoding.
    /// </summary>
    private abstract class V5PatchReaderProvider : RStringTableReaderProvider
    {
        /// <inheritdoc />
        public override RStringTableFileProperties ReadFileProperties(RBufferReader reader)
        {
            var version = reader.Read<int>() >> 24;
            var metadata = version switch
            {
                5 => GetV5Metadata(),
                4 => RStringTableMetadata.Version4,
                3 => RStringTableMetadata.Version3,
                2 => ReadLegacyFontConfigMetadata(reader),
                _ => throw new NotSupportedException($"Not Supported rst version: {version}")
            };

            var entryCount = reader.Read<int>();
            var hashesOffset = reader.Position;
            var contentOffset = hashesOffset + (entryCount * 8) + (version is < 5 ? 1 : 0);
            return new RStringTableFileProperties(metadata, entryCount, hashesOffset, contentOffset);
        }

        /// <inheritdoc />
        public override string ReadString(ReadOnlySpan<byte> span) => RStringPool.GetOrAdd(span);

        /// <summary>
        /// Retrieves the metadata associated with the version 5 string table.
        /// </summary>
        /// <returns>An <see cref="IRStringTableMetadata"/> instance containing metadata for the version 5 string table.</returns>
        protected abstract IRStringTableMetadata GetV5Metadata();

        /// <summary>
        /// Reads the legacy font configuration metadata from the binary reader based on the specific versioning requirements.
        /// This method is intended for use with older string table formats and interprets the metadata accordingly.
        /// </summary>
        /// <param name="reader">
        /// The binary reader (<see cref="RBufferReader" />) positioned at the beginning of the legacy
        /// metadata section.
        /// </param>
        /// <returns>
        /// A <see cref="LegacyFontConfigMetadata" /> instance containing the parsed font configuration metadata. If no
        /// explicit configuration is detected, a default instance is returned with null for the font configuration string.
        /// </returns>
        private static LegacyFontConfigMetadata ReadLegacyFontConfigMetadata(RBufferReader reader)
        {
            // For Old Version 2
            var hasConfig = reader.Read<byte>();
            return new LegacyFontConfigMetadata(hasConfig is not 0
                ? RStringPool.GetOrAdd(reader.Read(reader.Read<int>()))
                : null);
        }
    }

    /// <summary>
    /// Provides a V5 patch reader implementation for version 15.2 string table metadata.
    /// </summary>
    private sealed class PatchV1502ReaderProvider : V5PatchReaderProvider
    {
        /// <inheritdoc />
        protected override IRStringTableMetadata GetV5Metadata() => RStringTableMetadata.Version5Patch1502;
    }

    /// <summary>
    /// Provides a V5 patch reader implementation for version 14.15 (to 15.2) string table metadata.
    /// </summary>
    private sealed class PatchV1415ReaderProvider : V5PatchReaderProvider
    {
        /// <inheritdoc />
        protected override IRStringTableMetadata GetV5Metadata() => RStringTableMetadata.Version5Patch1415;
    }

    /// <summary>
    /// Provides a V5-compatible implementation of the patch reader provider for legacy data formats.
    /// </summary>
    private sealed class LegacyV5ReaderProvider : V5PatchReaderProvider
    {
        /// <inheritdoc />
        protected override IRStringTableMetadata GetV5Metadata() => RStringTableMetadata.Version5Legacy;
    }
}

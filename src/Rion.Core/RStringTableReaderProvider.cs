// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;

using Rion.Core.Buffers;
using Rion.Core.Metadata;
using Rion.Core.Metadata.Legacy;

namespace Rion.Core;

/// <summary>
/// Provides an abstract base for implementing string table reading functionality from binary data sources.
/// This class outlines the structure necessary for parsing string table properties and individual strings,
/// enabling the creation of tailored readers for different binary formats or encoding schemes.
/// </summary>
public abstract class RStringTableReaderProvider
{
    /// <summary>
    /// Gets the default instance of <see cref="RStringTableReaderProvider"/>, which serves as the fundamental
    /// implementation for parsing string table data from binary sources. It adheres to standard parsing rules
    /// and string decoding, offering a ready-to-use solution for developers without needing to implement
    /// a custom provider.
    /// </summary>
    public static RStringTableReaderProvider Default { get; } = new DefaultReaderProvider();

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
    private sealed class DefaultReaderProvider : RStringTableReaderProvider
    {
        /// <inheritdoc />
        public override RStringTableFileProperties ReadFileProperties(RBufferReader reader)
        {
            var version = reader.Read<int>() >> 24;
            var metadata = version is not 2 ? GetMetadata(version) : ReadLegacyFontConfigMetadata(reader);

            var entryCount = reader.Read<int>();
            var hashesOffset = reader.Position;
            var contentOffset = hashesOffset + (entryCount * 8) + (version is < 5 ? 1 : 0);
            return new RStringTableFileProperties(metadata, entryCount, hashesOffset, contentOffset);
        }

        /// <inheritdoc />
        public override string ReadString(ReadOnlySpan<byte> span) => RStringPool.GetOrAdd(span);

        /// <summary>
        /// Retrieves the metadata for a specific version of the string table.
        /// This method selects the appropriate metadata based on the provided version number.
        /// If the version is not supported, a <see cref="NotSupportedException" /> is thrown.
        /// </summary>
        /// <param name="version">The version number of the string table for which metadata is required.</param>
        /// <returns>The metadata object corresponding to the specified version.</returns>
        /// <exception cref="NotSupportedException">Thrown when the provided version is not supported.</exception>
        private static IRStringTableMetadata GetMetadata(int version)
        {
            return version switch
            {
                5 => RStringTableMetadata.Latest,
                4 => RStringTableMetadata.Version4,
                3 => RStringTableMetadata.Version3,
                _ => throw new NotSupportedException($"Not Supported rst version: {version}")
            };
        }

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
}

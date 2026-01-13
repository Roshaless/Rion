// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;
using System.Linq;

using Microsoft.Win32.SafeHandles;

using Rion.Core.Buffers;
using Rion.Core.Internal;
using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Writes a string table to a stream or file using efficient serialization.
/// </summary>
/// <remarks>
/// The <see cref="StringTableFileWriter" /> class is designed to serialize a provided <see cref="IStringTable" />
/// instance to a binary format suitable for storage or transmission. It manages the serialization process,
/// including handling the content and metadata of the string table.
/// </remarks>
public sealed partial class StringTableFileWriter : IDisposable
{
    /// <summary>
    /// Internal buffer writer instance responsible for sequential serialization of string table data.
    /// This field utilizes the <see cref="RawMemoryWriter" /> capabilities to efficiently write
    /// binary data, including string offsets and lengths, facilitating the creation of the
    /// string table's binary format.
    /// </summary>
    private readonly RawMemoryWriter _bufferWriter;

    /// <summary>
    /// Internal instance responsible for writing the content part of the string table.
    /// This field encapsulates the logic to manage and write string data into memory,
    /// providing efficient serialization support for the <see cref="StringTableFileWriter" />.
    /// </summary>
    private readonly NTStringWriter _contentWriter;

    /// <summary>
    /// Internal reference to the string table instance being written.
    /// This field holds the data source for the <see cref="StringTableFileWriter" /> to serialize.
    /// </summary>
    private readonly IStringTable _stringTable;

    /// <summary>
    /// Indicates whether the <see cref="StringTableFileWriter" /> instance has been disposed.
    /// </summary>
    /// <value>
    /// <c>true</c> if the object is disposed; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// This field should be checked before attempting to use any methods or properties
    /// of the <see cref="StringTableFileWriter" /> to avoid accessing resources after they have been freed.
    /// </remarks>
    private bool _disposed;

    /// <summary>
    /// Represents a writer for creating string tables with efficient serialization capabilities.
    /// </summary>
    public StringTableFileWriter(IStringTable stringTable)
    {
        _stringTable = stringTable;
        _bufferWriter = new RawMemoryWriter(stringTable.CalcContentOffset());
        _contentWriter = new NTStringWriter();

        InitializeBuffers();
    }

    /// <summary>
    /// Gets the combined length of data written by both the internal buffer writer and the content writer.
    /// This property provides the total serialized size which includes the binary data
    /// for string offsets and lengths managed by the <see cref="RawMemoryWriter" /> and the
    /// additional content handled by <see cref="NTStringWriter" />.
    /// </summary>
    public int Length
    {
        get => _bufferWriter.Length + _contentWriter.Length;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        GC.SuppressFinalize(this);

        _bufferWriter.Dispose();
        _contentWriter.Dispose();
    }

    /// <summary>
    /// Represents a specialized writer for RStringTable serialization, facilitating the process
    /// of saving string data into a binary format. This class works in tandem with
    /// <see cref="IStringTable" /> to manage the encoding of string tables into files or streams,
    /// accounting for necessary metadata like offsets and lengths.
    /// </summary>
    ~StringTableFileWriter() => Dispose();

    /// <summary>
    /// Initializes the internal buffers required for serializing the string table data.
    /// This method sets up the header with magic identifier, version, optional legacy font configuration,
    /// string count, and hash-offset pairs for each string entry. It also handles version-specific metadata.
    /// </summary>
    private void InitializeBuffers()
    {
        var metadata = _stringTable.Metadata;
        var version = metadata.Version;

        _bufferWriter.Write("RST"u8);
        _bufferWriter.WriteByte(version);

        if (version is 2)
        {
            WriteLegacyFontConfigMetadata(metadata as ILegacyFontConfigMetadata);
        }

        _bufferWriter.WriteValue(_stringTable.Count());

        var hashAlgorithm = metadata.HashAlgorithm;
        {
            foreach (var item in _stringTable.OrderBy(static x => x.Key))
            {
                var offset = _contentWriter.GetOffsetOrAddNew(item.Value);
                _bufferWriter.WriteValue(hashAlgorithm.WithOffset(item.Key, offset));
            }
        }

        if (version < 5)
        {
            _bufferWriter.WriteByte(0x00);
        }
    }

    /// <summary>
    /// Writes the legacy font configuration metadata to the internal buffer if present.
    /// This method is designed for RStringTable versions where the font configuration
    /// was stored differently and requires special handling.
    /// </summary>
    /// <param name="metadata">The legacy font configuration metadata which may include a font configuration string.</param>
    private void WriteLegacyFontConfigMetadata(ILegacyFontConfigMetadata? metadata)
    {
        if (metadata is not null && metadata.FontConfig.IsNotNullOrWhiteSpace())
        {
            _bufferWriter.WriteByte(0x01);

            var buffer = _contentWriter.FastGetBytes(metadata.FontConfig);
            {
                _bufferWriter.WriteValue(buffer.Length);
                _bufferWriter.Write(buffer);
            }
        }
        else
        {
            _bufferWriter.WriteByte(0x00);
        }
    }

    /// <summary>
    /// Writes the serialized string table content to the provided FileStream.
    /// This method utilizes the underlying file stream's SafeFileHandle to perform low-level writes,
    /// first writing the buffer data followed by the content data at appropriate offsets.
    /// </summary>
    /// <param name="fileStream">The FileStream to which the string table content will be written.</param>
    public void WriteTo(FileStream fileStream)
        => WriteTo(fileStream.SafeFileHandle, (int)fileStream.Position);

    /// <summary>
    /// Writes the serialized string table content using low-level file access.
    /// This method writes the buffer data and content data to the specified file offset
    /// using the provided SafeFileHandle, ensuring correct positioning and flushing to disk.
    /// </summary>
    /// <param name="handle">A SafeFileHandle to the file where the data will be written.</param>
    /// <param name="fileOffset">The starting offset within the file where writing should begin.</param>
    public void WriteTo(SafeFileHandle handle, int fileOffset)
    {
        RandomAccess.Write(handle, _bufferWriter.RawData, fileOffset);
        RandomAccess.FlushToDisk(handle);

        RandomAccess.Write(handle, _contentWriter.RawData, fileOffset + _bufferWriter.Length);
        RandomAccess.FlushToDisk(handle);
    }

    /// <summary>
    /// Writes the serialized string table content to the provided stream.
    /// This method serializes the internal buffer data, including both the header (if applicable)
    /// and the string content, into the given stream.
    /// </summary>
    /// <param name="stream">The stream to which the serialized string table data will be written.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
    public void WriteTo(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));
        {
            stream.Write(_bufferWriter.RawData);
            stream.Write(_contentWriter.RawData);
        }
    }
}

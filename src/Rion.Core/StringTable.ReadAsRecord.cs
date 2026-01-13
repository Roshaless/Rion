// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Immutable;

using Rion.Core.Buffers;

namespace Rion.Core;

public partial class StringTable
{
    /// <summary>
    /// Reads a string table from the specified file and returns it as a record.
    /// </summary>
    /// <remarks>
    /// This method reads the entire string table from the specified file and converts it into a
    /// record format. The file must be in a valid format that can be parsed by the underlying string table reader.
    /// </remarks>
    /// <param name="path">The path to the file containing the string table. Must not be null or empty.</param>
    /// <returns>An <see cref="IStringTable"/> instance representing the string table read from the file.</returns>
    public static IStringTable ReadAsRecord(string path)
    {
        using var scope = FileBufferScope.CreateFrom(path);
        var reader = new StringTableFileReader(scope.Span);

        return CreateRecord(reader.Metadata, [.. reader.ReadAll()]);
    }

    /// <summary>
    /// Reads the provided binary data and converts it into an <see cref="IStringTable"/> record.
    /// </summary>
    /// <remarks>
    /// This method processes the binary data using a specialized reader to extract metadata and
    /// entries, which are then used to construct the resulting <see cref="IStringTable"/> record.
    /// </remarks>
    /// <param name="data">A read-only span of bytes containing the binary data to be parsed.</param>
    /// <returns>An <see cref="IStringTable"/> instance representing the parsed data as a record.</returns>
    public static IStringTable ReadAsRecord(ReadOnlySpan<byte> data)
    {
        var reader = new StringTableFileReader(data);
        var entries = reader.ReadAll().ToImmutableDictionary();

        return CreateRecord(reader.Metadata, entries);
    }
}

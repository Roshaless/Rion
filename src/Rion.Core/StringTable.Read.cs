// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;

using Rion.Core.Buffers;

namespace Rion.Core;

public partial class StringTable
{
    /// <summary>
    /// Reads an RStringTable from the specified file path.
    /// </summary>
    /// <param name="path">The path to the file containing the RStringTable data. Must not be null or empty.</param>
    /// <returns>An <see cref="StringTable"/> instance populated with the data from the specified file.</returns>
    public static StringTable Read(string path)
    {
        using var scope = FileBufferScope.CreateFrom(path);
        var reader = new StringTableFileReader(scope.Span);

        return Create(reader.Metadata, reader.ReadAll());
    }

    /// <summary>
    /// Reads a string table from the specified binary data.
    /// </summary>
    /// <remarks>
    /// This method parses the binary data into a string table, extracting metadata and entries.
    /// The returned <see cref="StringTable"/> is immutable and represents the complete parsed data.
    /// </remarks>
    /// <param name="data">A read-only span of bytes containing the binary data to parse.</param>
    /// <returns>An <see cref="StringTable"/> instance representing the parsed string table.</returns>
    public static StringTable Read(ReadOnlySpan<byte> data)
    {
        var reader = new StringTableFileReader(data);
        var entries = reader.ReadAll();

        return Create(reader.Metadata, entries);
    }
}

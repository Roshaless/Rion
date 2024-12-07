// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Immutable;
using System.IO;

using Microsoft.Win32.SafeHandles;

using Rion.Core.Buffers;

namespace Rion.Core;

/// <summary>
/// Provides methods for reading and writing <see cref="RStringTable"/> instances.
/// </summary>
public static class RFile
{
    /// <summary>
    /// Reads an <see cref="RStringTable"/> from the specified file path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>An <see cref="RStringTable"/> instance.</returns>
    public static RStringTable Read(string path)
    {
        using var scope = RFileBufferScope.CreateFrom(path);
        var reader = new RStringTableReader(scope.Span);
        {
            return RStringTable.Create(reader.Metadata, reader.ReadAll());
        }
    }

    /// <summary>
    /// Reads an <see cref="RStringTable"/> from the specified data.
    /// </summary>
    /// <param name="data">The data to read from.</param>
    /// <returns>An <see cref="RStringTable"/> instance.</returns>
    public static RStringTable Read(ReadOnlySpan<byte> data)
    {
        var reader = new RStringTableReader(data);
        {
            return RStringTable.Create(reader.Metadata, reader.ReadAll().ToImmutableArray());
        }
    }

    /// <summary>
    /// Reads an <see cref="IRStringTable"/> from the specified file path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>An <see cref="IRStringTable"/> instance.</returns>
    public static IRStringTable ReadAsRecord(string path)
    {
        using var scope = RFileBufferScope.CreateFrom(path);
        var reader = new RStringTableReader(scope.Span);
        {
            return RStringTable.CreateRecord(reader.Metadata, reader.ReadAll().ToImmutableArray());
        }
    }

    /// <summary>
    /// Reads an <see cref="IRStringTable"/> from the specified data.
    /// </summary>
    /// <param name="data">The data to read from.</param>
    /// <returns>An <see cref="IRStringTable"/> instance.</returns>
    public static IRStringTable ReadAsRecord(ReadOnlySpan<byte> data)
    {
        var reader = new RStringTableReader(data);
        {
            return RStringTable.CreateRecord(reader.Metadata, reader.ReadAll());
        }
    }

    /// <summary>
    /// Opens or creates a file stream at the specified path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>A file stream for writing.</returns>
    internal static FileStream OpenOrCreate(string path) => new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

    /// <summary>
    /// Writes an <see cref="IRStringTable"/> to the specified stream.
    /// </summary>
    /// <param name="output">The stream to write to.</param>
    /// <param name="value">The <see cref="IRStringTable"/> instance to write.</param>
    public static void Write(Stream output, IRStringTable value)
    {
        using var writer = new RStringTableWriter(value);
        {
            writer.WriteTo(output);
        }
    }

    /// <summary>
    /// Writes an <see cref="IRStringTable"/> to the specified file stream.
    /// </summary>
    /// <param name="output">The file stream to write to.</param>
    /// <param name="value">The <see cref="IRStringTable"/> instance to write.</param>
    public static void Write(FileStream output, IRStringTable value)
    {
        using var writer = new RStringTableWriter(value);
        {
            writer.WriteTo(output);
        }
    }

    /// <summary>
    /// Writes an <see cref="IRStringTable"/> to the specified file handle and offset.
    /// </summary>
    /// <param name="handle">The file handle to write to.</param>
    /// <param name="fileOffset">The offset within the file to write to.</param>
    /// <param name="value">The <see cref="IRStringTable"/> instance to write.</param>
    public static void Write(SafeFileHandle handle, int fileOffset, IRStringTable value)
    {
        using var writer = new RStringTableWriter(value);
        {
            writer.WriteTo(handle, fileOffset);
        }
    }

    /// <summary>
    /// Writes an <see cref="IRStringTable"/> to the specified file path.
    /// </summary>
    /// <param name="outputPath">The path to the file to write to.</param>
    /// <param name="value">The <see cref="IRStringTable"/> instance to write.</param>
    public static void Write(string outputPath, IRStringTable value)
    {
        using var file = OpenOrCreate(outputPath);
        {
            Write(file, value);
        }
    }
}

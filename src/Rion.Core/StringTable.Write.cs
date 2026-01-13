// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.IO;

using Microsoft.Win32.SafeHandles;

namespace Rion.Core;

public partial class StringTable
{

    /// <summary>
    /// Write a string table to the specified output stream.
    /// </summary>
    /// <param name="output">The output stream to write to.</param>
    /// <param name="table">The string table to write.</param>
    public static void Write(Stream output, IStringTable table)
    {
        using var writer = new StringTableFileWriter(table);
        writer.WriteTo(output);
    }

    /// <summary>
    /// Write a string table to the specified output file stream.
    /// </summary>
    /// <param name="output">The output stream to write to.</param>
    /// <param name="table">The string table to write.</param>
    public static void Write(FileStream output, IStringTable table)
    {
        using var writer = new StringTableFileWriter(table);
        writer.WriteTo(output);
    }

    /// <summary>
    /// Write a string table to the specified handle at the given file offset.
    /// </summary>
    /// <param name="handle">The handle to write to.</param>
    /// <param name="fileOffset">The file offset to write to.</param>
    /// <param name="table">The string table to write.</param>
    public static void Write(SafeFileHandle handle, int fileOffset, IStringTable table)
    {
        using var writer = new StringTableFileWriter(table);
        writer.WriteTo(handle, fileOffset);
    }

    /// <summary>
    /// Write a string table to the specified output file path.
    /// </summary>
    /// <param name="outputPath">The output file path to write to.</param>
    /// <param name="table">The string table to write.</param>
    public static void Write(string outputPath, IStringTable table)
    {
        using var file = Internal.FileOperations.OpenOrCreate(outputPath);
        using var writer = new StringTableFileWriter(table);
        writer.WriteTo(file);
    }
}

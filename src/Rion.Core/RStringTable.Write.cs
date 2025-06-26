// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.IO;

using Microsoft.Win32.SafeHandles;

#pragma warning disable IDE0063

namespace Rion.Core;

public partial class RStringTable
{

    public static void Write(Stream output, IRStringTable table)
    {
        using (var writer = new RStringTableWriter(table))
        {
            writer.WriteTo(output);
        }
    }

    ///
    public static void Write(FileStream output, IRStringTable table)
    {
        using (var writer = new RStringTableWriter(table))
        {
            writer.WriteTo(output);
        }
    }

    public static void Write(SafeFileHandle handle, int fileOffset, IRStringTable table)
    {
        using (var writer = new RStringTableWriter(table))
        {
            writer.WriteTo(handle, fileOffset);
        }
    }

    public static void Write(string outputPath, IRStringTable table)
    {
        using (var file = Internal.FileOperations.OpenOrCreate(outputPath))
        {
            using (var writer = new RStringTableWriter(table))
            {
                writer.WriteTo(file);
            }
        }
    }
}

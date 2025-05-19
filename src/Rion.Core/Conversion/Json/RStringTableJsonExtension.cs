// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

#pragma warning disable IDE0130

namespace Rion.Core.Conversion;

public static class RStringTableJsonExtension
{
    public static RStringTable ReadRStringTableFromJson(this Span<byte> bytes)
    {
        return RStringTableJsonConverter.Instance.Convert(bytes);
    }

    public static RStringTable ReadRStringTableFromJson(this ReadOnlySpan<byte> bytes)
    {
        return RStringTableJsonConverter.Instance.Convert(bytes);
    }

    public static void WriteToFile(this IRStringTable stringTable, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(stringTable);
        ArgumentNullException.ThrowIfNull(outputPath);

        using var stream = RFile.OpenOrCreate(outputPath);
        {
            RStringTableJsonConverter.Instance.Write(stream, stringTable);
        }
    }

    public static void WriteToStream(this IRStringTable stringTable, Stream output)
    {
        ArgumentNullException.ThrowIfNull(stringTable);
        ArgumentNullException.ThrowIfNull(output);

        RStringTableJsonConverter.Instance.Write(output, stringTable);
    }
}

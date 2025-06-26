// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rion.Core;
using Rion.Core.Buffers;
using Rion.Core.Hashing;
using Rion.Core.Serialization;


var toWriteRst = new List<(string, IRStringTable)>();
var writeToJson = new List<(string, IRStringTable)>();

foreach (var file in args)
{
    try
    {
        var fileStream = File.OpenRead(file);
        var firstByte = fileStream.ReadByte();
        {
            fileStream.Dispose();
        }

        using var scope = RFileBufferScope.CreateFrom(file);

        if (firstByte is 0x52)
        {
            writeToJson.Add((file, RStringTable.ReadAsRecord(scope.Span)));
        }
        else
        {
            toWriteRst.Add((file, RStringTableSerializer.DeserializeFromJson(scope.Span)));
        }
    }
    catch
    {
        Console.WriteLine($"Invalid input file: {file}.");
        continue;
    }
}

LoadHashes(
    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashes"),
    [.. writeToJson.Select(x => x.Item2.Metadata.HashAlgorithm).Distinct()]);


DoConvert(writeToJson, (x =>
{
    var outputPath = ChangeExt(x.Item1, ".json");
    {
        RStringTableSerializer.SerializeToJsonFile(outputPath, x.Item2);
    }

    return outputPath;
}));

DoConvert(toWriteRst, (x =>
{
    var outputPath = ChangeExt(x.Item1, ".stringtable");
    RStringTable.Write(outputPath, x.Item2);
    return outputPath;
}));

static void DoConvert(List<(string, IRStringTable)> collection, Func<(string, IRStringTable), string> convert)
{
    foreach (var item in collection)
    {
        Console.WriteLine($"Input: {item.Item1}");

        try
        {
            var outputPath = convert(item);
            Console.WriteLine($"Output: {outputPath}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }
}


static void LoadHashes(string hashesDir, params IRSTHashAlgorithm[] hashAlgorithms)
{
    if (hashAlgorithms.Length == 0)
        return;

    if (Directory.Exists(hashesDir) is false)
        return;

    try
    {
        foreach (var path in Directory.EnumerateFiles(hashesDir))
        {
            foreach (var hashAlgorithm in hashAlgorithms)
            {
                try
                {
                    RHashtable.LoadFromStrings(File.ReadAllLines(path), hashAlgorithm);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
    catch
    {
        // ignored
    }
}

static string ChangeExt(string path, string ext)
{
    return Path.ChangeExtension(Path.GetFullPath(path), ext);
}

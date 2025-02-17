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
using Rion.Core.Hashing;

internal static class Program
{
    internal static void Main(string[] args)
    {
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

                if (firstByte is 0x52)
                {
                    writeToJson.Add((file, RFile.ReadAsRecord(file)));
                }
                else
                {
                    toWriteRst.Add((file, RConvert.FromJsonFile(file)));
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
            var outputPath = ChangeExtExt(x.Item1, ".json");
            RConvert.ToJsonFile(outputPath, x.Item2);
            return outputPath;
        }));

        DoConvert(toWriteRst, (x =>
        {
            var outputPath = ChangeExtExt(x.Item1, ".stringtable");
            RFile.Write(outputPath, x.Item2);
            return outputPath;
        }));
    }

    static void DoConvert(List<(string, IRStringTable)> collection, Func<(string, IRStringTable), string> convert)
    {
        foreach (var item in collection)
        {
            Console.WriteLine($"input: {item.Item1}");

            try
            {
                var outputPath = convert(item);
                Console.WriteLine($"output: {outputPath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"error: {e.Message}");
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

    static string ChangeExtExt(string path, string ext)
    {
        return Path.ChangeExtension(Path.GetFullPath(path), ext);
    }
}

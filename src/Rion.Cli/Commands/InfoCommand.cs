// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Rion.Core;
using Rion.Core.Buffers;
using Rion.Core.Hashing;

namespace Rion.Cli.Commands;

/// <summary>
/// Provides the <c>info</c> command which displays metadata and statistics about a <c>.stringtable</c> file.
/// </summary>
internal static class InfoCommand
{
    /// <summary>
    /// Creates the <c>info</c> command.
    /// </summary>
    public static Command Create()
    {
        var fileArgument = new Argument<string>(
            "file",
            "Path to a .stringtable file."
        );

        var detailedOption = new Option<bool>(
            aliases: ["--detailed", "-d"],
            description: "Show detailed information including sample entries.",
            getDefaultValue: () => false
        );

        var command = new Command("info", "Display metadata and statistics about a .stringtable file.")
        {
            fileArgument,
            detailedOption,
        };

        command.SetHandler(HandleAsync, fileArgument, detailedOption);
        return command;
    }

    private static Task<int> HandleAsync(string file, bool detailed)
    {
        try
        {
            if (!File.Exists(file))
            {
                Console.Error.WriteLine($"Error: File not found — {file}");
                return Task.FromResult(1);
            }

            using var scope = FileBufferScope.CreateFrom(file);
            var reader = new StringTableFileReader(scope.Span);

            var metadata = reader.Metadata;
            var entries = reader.ReadAll();
            var entryList = entries.ToList();

            // Header
            Console.WriteLine();
            Console.WriteLine($"  File        : {Path.GetFullPath(file)}");
            Console.WriteLine($"  Size        : {new FileInfo(file).Length:N0} bytes");

            // Metadata
            Console.WriteLine($"  Version     : {metadata.Version}");

            // Statistics
            Console.WriteLine($"  Entries     : {entryList.Count:N0}");

            if (entryList.Count > 0)
            {
                var totalKeyChars = entryList.Sum(e => e.Value.Length);
                Console.WriteLine($"  Avg Key Len : {totalKeyChars / (double)entryList.Count:F1} chars");
            }

            // Detailed view
            if (detailed && entryList.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("  Sample entries:");
                var maxShow = Math.Min(entryList.Count, 20);
                for (var i = 0; i < maxShow; i++)
                {
                    var (hash, value) = entryList[i];
                    var name = RSTHashtable.GetNameOrHexString(hash);
                    Console.WriteLine($"    [{i + 1}] {name} => \"{value}\"");
                }

                if (entryList.Count > maxShow)
                {
                    Console.WriteLine($"    ... and {entryList.Count - maxShow:N0} more entries.");
                }
            }

            Console.WriteLine();
            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading file: {ex.Message}");
            return Task.FromResult(1);
        }
    }
}

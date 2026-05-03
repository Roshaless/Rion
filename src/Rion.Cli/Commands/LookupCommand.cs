// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.CommandLine;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Rion.Core;
using Rion.Core.Buffers;
using Rion.Core.Hashing;

namespace Rion.Cli.Commands;

/// <summary>
/// Provides the <c>lookup</c> command which queries a <c>.stringtable</c> file by hash or value.
/// </summary>
internal static class LookupCommand
{
    /// <summary>
    /// Creates the <c>lookup</c> command.
    /// </summary>
    public static Command Create()
    {
        var fileArgument = new Argument<string>(
            "file",
            "Path to a .stringtable file to search."
        );

        var hashOption = new Option<string?>(
            aliases: ["--hash", "-h"],
            description: "Search by hash value (hex with 0x prefix, or decimal)."
        );

        var valueOption = new Option<string?>(
            aliases: ["--value", "-v"],
            description: "Search by string value (case-insensitive substring match)."
        );

        var countOption = new Option<int>(
            aliases: ["--count", "-n"],
            description: "Maximum number of results to show.",
            getDefaultValue: () => 50
        );

        var command = new Command("lookup", "Search a .stringtable file by hash or value.")
        {
            fileArgument,
            hashOption,
            valueOption,
            countOption,
        };

        command.SetHandler(HandleAsync, fileArgument, hashOption, valueOption, countOption);
        return command;
    }

    private static Task<int> HandleAsync(string file, string? hash, string? value, int count)
    {
        try
        {
            if (!File.Exists(file))
            {
                Console.Error.WriteLine($"Error: File not found — {file}");
                return Task.FromResult(1);
            }

            if (hash is null && value is null)
            {
                Console.Error.WriteLine("Error: Specify --hash or --value to search.");
                return Task.FromResult(1);
            }

            using var scope = FileBufferScope.CreateFrom(file);
            var reader = new StringTableFileReader(scope.Span);
            var entries = reader.ReadAll().ToList();

            var results = entries.AsEnumerable();

            // Filter by hash
            if (hash is not null)
            {
                if (!TryParseHash(hash, out var hashValue))
                {
                    Console.Error.WriteLine(
                        "Error: Invalid hash format. Use hex (0x...) or decimal.");
                    return Task.FromResult(1);
                }

                results = results.Where(e => e.Key == hashValue);
            }

            // Filter by value substring
            if (value is not null)
            {
                results = results.Where(e =>
                    e.Value.Contains(value, StringComparison.OrdinalIgnoreCase));
            }

            var resultList = results.Take(count).ToList();

            Console.WriteLine();
            Console.WriteLine($"  File   : {Path.GetFullPath(file)}");
            Console.WriteLine($"  Entries: {entries.Count:N0}");
            Console.WriteLine($"  Matches: {resultList.Count}");
            Console.WriteLine();

            if (resultList.Count == 0)
            {
                Console.WriteLine("  No matching entries found.");
                return Task.FromResult(0);
            }

            // Display results
            foreach (var entry in resultList)
            {
                var name = RSTHashtable.GetNameOrHexString(entry.Key);
                Console.WriteLine($"  {name}");
                Console.WriteLine($"    → \"{entry.Value}\"");
                Console.WriteLine();
            }

            if (resultList.Count < results.Count())
            {
                Console.WriteLine($"  ... and more results. Use -n to increase limit.");
            }

            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return Task.FromResult(1);
        }
    }

    private static bool TryParseHash(string text, out ulong value)
    {
        text = text.Trim();
        if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("0X", StringComparison.OrdinalIgnoreCase))
        {
            return ulong.TryParse(
                text[2..],
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out value);
        }

        return ulong.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }
}

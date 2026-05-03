// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.CommandLine;
using System.Threading.Tasks;

using Rion.Core.Hashing;

namespace Rion.Cli.Commands;

/// <summary>
/// Provides the <c>hash</c> command which hashes a string using RST hash algorithms,
/// selected by file version identifier.
/// </summary>
internal static class HashCommand
{
    /// <summary>
    /// Creates the <c>hash</c> command.
    /// </summary>
    public static Command Create()
    {
        var textArgument = new Argument<string>(
            "text",
            () => "",
            "The text to hash."
        );

        var versionOption = new Option<string>(
            aliases: ["--version", "-v"],
            description: "File version identifier that determines the hash algorithm and bitmask.",
            getDefaultValue: () => "latest"
        );

        var countOption = new Option<int>(
            aliases: ["--count", "-n"],
            description: "Number of iterations for benchmarking.",
            getDefaultValue: () => 1
        );

        var listVersionsOption = new Option<bool>(
            aliases: ["--list-versions"],
            description: "List all known version identifiers and exit.",
            getDefaultValue: () => false
        );

        var command = new Command("hash", "Hash a string using RST hash algorithms.")
        {
            textArgument,
            versionOption,
            countOption,
            listVersionsOption,
        };

        command.SetHandler(HandleAsync, textArgument, versionOption, countOption, listVersionsOption);
        return command;
    }

    private static Task<int> HandleAsync(string text, string version, int count, bool listVersions)
    {
        if (listVersions)
        {
            PrintVersionTable();
            return Task.FromResult(0);
        }

        if (string.IsNullOrEmpty(text))
        {
            Console.Error.WriteLine("Error: No text provided to hash.");
            return Task.FromResult(1);
        }

        try
        {
            var (label, options) = ResolveVersion(version);
            var algorithm = RSTHashAlgorithm.Create(options);

            if (count <= 1)
            {
                var hash = algorithm.Hash(text.AsSpan());
                Console.WriteLine($"  Text    : {text}");
                Console.WriteLine($"  Version : {label}");
                Console.WriteLine($"  Algo    : {options.Format} / {options.BitMask.Bits} bits (0x{options.BitMask.Value:X})");
                Console.WriteLine($"  Hash    : {hash} (0x{hash:X16})");
            }
            else
            {
                Console.WriteLine($"  Hashing \"{text}\" × {count:N0} iterations...");

                var sw = System.Diagnostics.Stopwatch.StartNew();
                for (var i = 0; i < count; i++)
                    algorithm.Hash(text.AsSpan());
                sw.Stop();

                Console.WriteLine($"  Version   : {label}");
                Console.WriteLine($"  Algo      : {options.Format} / {options.BitMask.Bits} bits (0x{options.BitMask.Value:X})");
                Console.WriteLine($"  Total time: {sw.ElapsedMilliseconds} ms");
                Console.WriteLine($"  Avg time  : {sw.Elapsed.TotalMicroseconds / count:F2} µs/hash");
                Console.WriteLine($"  Rate      : {count / sw.Elapsed.TotalSeconds:F0} hashes/s");
            }

            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return Task.FromResult(1);
        }
    }

    /// <summary>
    /// Maps a version identifier string to its <see cref="RSTHashAlgorithmOptions"/> and a human-readable label.
    /// </summary>
    private static (string Label, RSTHashAlgorithmOptions Options) ResolveVersion(string version)
    {
        return version.ToLowerInvariant() switch
        {
            "v2" or "2" => ("v2", RSTHashAlgorithmOptions.Version2),
            "v3" or "3" => ("v3", RSTHashAlgorithmOptions.Version3),
            "v4" or "4" => ("v4", RSTHashAlgorithmOptions.Version4),
            "v5" or "5" or "v5-legacy" => ("v5 (legacy)", RSTHashAlgorithmOptions.Version5),
            "v5-1415" or "v5-patch1415" => ("v5 (patch 14.15)", RSTHashAlgorithmOptions.Version5Patch1415),
            "v5-1502" or "v5-patch1502" or "latest" => ("v5 (patch 15.02, latest)", RSTHashAlgorithmOptions.Version5Patch1502),
            _ => throw new ArgumentException(
                $"Unknown version '{version}'. Use --list-versions to see available versions."),
        };
    }

    private static void PrintVersionTable()
    {
        Console.WriteLine();
        Console.WriteLine("  Identifier         Format    Bits   Description");
        Console.WriteLine("  ─────────         ──────    ────   ───────────");
        Console.WriteLine("  v2                 XxHash64   40    Version 2");
        Console.WriteLine("  v3                 XxHash64   40    Version 3");
        Console.WriteLine("  v4                 XxHash64   39    Version 4");
        Console.WriteLine("  v5, v5-legacy      XxHash64   39    Version 5 (legacy)");
        Console.WriteLine("  v5-1415            XxHash3    39    Version 5, patch 14.15");
        Console.WriteLine("  v5-1502, latest    XxHash3    38    Version 5, patch 15.02 (default)");
        Console.WriteLine();
    }
}


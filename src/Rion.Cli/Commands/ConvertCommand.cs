// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;
using System.Threading.Tasks;

using Rion.Core;
using Rion.Core.Buffers;
using Rion.Core.Hashing;
using Rion.Core.Metadata;
using Rion.Core.Serialization;

namespace Rion.Cli.Commands;

/// <summary>
/// Provides file-format conversion between <c>.stringtable</c> and <c>.json</c>.
/// Used as the default behavior of the root command.
/// </summary>
internal static class ConvertCommand
{
    /// <summary>
    /// Handles conversion for a collection of input files.
    /// </summary>
    public static async Task<int> HandleFilesAsync(
        string[] files,
        string? output,
        DirectoryInfo? outputDir,
        string? format,
        DirectoryInfo? hashesDir,
        string? version)
    {
        if (files.Length == 0)
            return 0;

        if (output is not null && files.Length > 1)
        {
            Console.Error.WriteLine("Error: --output can only be used with a single input file.");
            return 1;
        }

        var targetFormat = ParseFormat(format);
        if (format is not null && targetFormat is null)
        {
            Console.Error.WriteLine("Error: --to must be 'json' or 'rst'.");
            return 1;
        }

        // Resolve --version
        RSTHashAlgorithmOptions? versionOptions = null;
        string? versionLabel = null;
        if (version is not null)
        {
            try
            {
                (versionLabel, versionOptions) = VersionHelper.Resolve(version);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        await LoadHashesAsync(hashesDir);

        var successCount = 0;
        var failCount = 0;

        foreach (var file in files)
        {
            try
            {
                if (!File.Exists(file))
                {
                    Console.Error.WriteLine($"Error: File not found — {file}");
                    failCount++;
                    continue;
                }

                var inputFormat = DetectFormat(file);
                var outputFormat = targetFormat
                    ?? (inputFormat == FileFormat.Rst ? FileFormat.Json : FileFormat.Rst);

                var outputPath = output
                    ?? BuildOutputPath(file, outputFormat, outputDir);
                ConvertFile(file, inputFormat, outputFormat, outputPath, versionLabel, versionOptions);

                Console.WriteLine($"{file} → {outputPath}");
                successCount++;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error converting {file}: {ex.Message}");
                failCount++;
            }
        }

        if (failCount > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Converted {successCount} file(s), {failCount} error(s).");
        }

        return failCount > 0 ? 1 : 0;
    }

    private static FileFormat DetectFormat(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        if (ext is ".stringtable" or ".rst")
            return FileFormat.Rst;
        if (ext is ".json")
            return FileFormat.Json;

        try
        {
            using var stream = File.OpenRead(filePath);
            var firstByte = stream.ReadByte();
            return firstByte == 0x52 ? FileFormat.Rst : FileFormat.Json;
        }
        catch
        {
            throw new InvalidOperationException(
                $"Unable to determine format of '{filePath}'. Specify --to explicitly.");
        }
    }

    private static void ConvertFile(
        string inputPath,
        FileFormat inputFormat,
        FileFormat outputFormat,
        string outputPath,
        string? versionLabel,
        RSTHashAlgorithmOptions? versionOptions)
    {
        using var scope = FileBufferScope.CreateFrom(inputPath);
        var span = scope.Span;

        if (inputFormat == FileFormat.Rst && outputFormat == FileFormat.Json)
        {
            // RST → JSON: use version-specific provider when --version is specified
            var provider = versionLabel is not null
                ? GetReaderProvider(versionLabel)
                : StringTableFileReaderProvider.Default;

            var reader = new StringTableFileReader(span, provider);
            var table = StringTable.CreateRecord(reader.Metadata, [.. reader.ReadAll()]);

            StringTableSerializer.SerializeToJsonFile(outputPath, table);
        }
        else if (inputFormat == FileFormat.Json && outputFormat == FileFormat.Rst)
        {
            // JSON → RST: version overrides the metadata used for output
            var table = StringTableSerializer.DeserializeFromJson(span);

            if (versionOptions is not null)
            {
                var metadata = StringTableMetadata.Create(versionOptions.Value, GetVersionByte(versionLabel!));
                table = StringTable.Create(metadata, table);
                Console.WriteLine($"  Using version: {versionLabel}");
            }

            StringTable.Write(outputPath, table);
        }
        else
        {
            throw new InvalidOperationException(
                $"Cannot convert {inputFormat} → {outputFormat}. Source and target are same format.");
        }
    }

    /// <summary>
    /// Selects the appropriate <see cref="StringTableFileReaderProvider"/> for a given version label.
    /// </summary>
    private static StringTableFileReaderProvider GetReaderProvider(string versionLabel)
    {
        return versionLabel switch
        {
            "v5 (patch 15.02, latest)" => StringTableFileReaderProvider.PatchV1502,
            "v5 (patch 14.15)" => StringTableFileReaderProvider.PatchV1415,
            "v5 (legacy)" => StringTableFileReaderProvider.LegacyV5,
            _ => StringTableFileReaderProvider.Default, // v2, v3, v4 → default handles these
        };
    }

    /// <summary>
    /// Extracts the version byte from a version label (e.g., "v5 (patch 15.02, latest)" → 5).
    /// </summary>
    private static byte GetVersionByte(string versionLabel)
    {
        // Parse the major version number from the label
        if (versionLabel.StartsWith("v5"))
            return 5;
        if (versionLabel.StartsWith("v4"))
            return 4;
        if (versionLabel.StartsWith("v3"))
            return 3;
        if (versionLabel.StartsWith("v2"))
            return 2;
        return 5; // default
    }

    private static string BuildOutputPath(
        string inputPath, FileFormat format, DirectoryInfo? outputDir)
    {
        var targetExt = format == FileFormat.Rst ? ".stringtable" : ".json";
        var fileName = Path.ChangeExtension(Path.GetFileName(inputPath), targetExt);
        return outputDir is not null
            ? Path.Combine(outputDir.FullName, fileName)
            : Path.ChangeExtension(Path.GetFullPath(inputPath), targetExt);
    }

    private static async Task LoadHashesAsync(DirectoryInfo? hashesDir)
    {
        DirectoryInfo?[] candidates = hashesDir is not null
            ? [hashesDir]
            : [new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashes"))];

        foreach (var dir in candidates)
        {
            if (dir is null || !dir.Exists)
                continue;

            await Task.Run(() =>
            {
                var knownOptions = new[]
                {
                    RSTHashAlgorithmOptions.Version2,
                    RSTHashAlgorithmOptions.Version3,
                    RSTHashAlgorithmOptions.Version4,
                    RSTHashAlgorithmOptions.Version5,
                    RSTHashAlgorithmOptions.Version5Patch1415,
                    RSTHashAlgorithmOptions.Version5Patch1502,
                };

                foreach (var path in Directory.EnumerateFiles(dir.FullName))
                {
                    try
                    {
                        var lines = File.ReadAllLines(path);
                        foreach (var opt in knownOptions)
                        {
                            try { RSTHashtable.LoadFromStrings(lines, RSTHashAlgorithm.Create(opt)); }
                            catch { /* unsupported combination */ }
                        }
                    }
                    catch { /* malformed file */ }
                }
            });
        }
    }

    private static FileFormat? ParseFormat(string? format) => format?.ToLowerInvariant() switch
    {
        "json" => FileFormat.Json,
        "rst" or "stringtable" => FileFormat.Rst,
        _ => null,
    };

    private enum FileFormat { Rst, Json }
}

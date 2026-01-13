// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics.CodeAnalysis;

using Rion.Core.Hashing;

namespace Rion.Core.Metadata;

/// <summary>
/// Represents the metadata for a string table, including hashing algorithm and version details.
/// </summary>
/// <param name="HashAlgorithm">The hash algorithm to set.</param>
/// <param name="Version">The version to set.</param>
public record class RStringTableMetadata(RSTHashAlgorithm HashAlgorithm, byte Version) : IRStringTableMetadata
{
    public static RStringTableMetadata Latest => Version5Patch1502;

    /// <summary>
    /// Represents the metadata for a string table with version 2.
    /// </summary>
    public static LegacyFontConfigMetadata Version2 => new();

    /// <summary>
    /// Represents the metadata for a string table with version 3.
    /// </summary>
    public static RStringTableMetadata Version3 => Create(RSTHashAlgorithmOptions.Version3, 3);

    /// <summary>
    /// Represents the metadata for a string table with version 4.
    /// </summary>
    public static RStringTableMetadata Version4 => Create(RSTHashAlgorithmOptions.Version4, 4);

    /// <summary>
    /// Represents the metadata for a string table with version 5 (legacy).
    /// </summary>
    public static RStringTableMetadata Version5Legacy => Create(RSTHashAlgorithmOptions.Version5, 5);

    /// <summary>
    /// Represents the metadata for a string table with version 5 and patch 14.15.
    /// </summary>
    public static RStringTableMetadata Version5Patch1415 => Create(RSTHashAlgorithmOptions.Version5Patch1415, 5);

    /// <summary>
    /// Represents the metadata for a string table with version 5 and patch 15.02.
    /// </summary>
    public static RStringTableMetadata Version5Patch1502 => Create(RSTHashAlgorithmOptions.Version5Patch1502, 5);

    /// <summary>
    /// Creates a new instance of <see cref="RStringTableMetadata"/> with the specified hash algorithm options and version.
    /// </summary>
    /// <param name="options">The hash algorithm options to use.</param>
    /// <param name="version">The version to set.</param>
    /// <returns>A new instance of <see cref="RStringTableMetadata"/>.</returns>
    public static RStringTableMetadata Create(RSTHashAlgorithmOptions options, byte version)
    {
        return new RStringTableMetadata(RSTHashAlgorithm.Create(options), version);
    }

    /// <summary>
    /// Retrieves the metadata for the specified version.
    /// </summary>
    /// <param name="version">The version string to retrieve metadata for.</param>
    /// <returns>The metadata for the specified version.</returns>
    /// <exception cref="ArgumentException">The specified version is not supported.</exception>
    public static IRStringTableMetadata GetMetadata(string version)
    {
        if (TryGetMetadata(version, out var metadata))
        {
            return metadata;
        }

        throw new ArgumentException($"Unsupported version: {version}");
    }

    /// <summary>
    /// Retrieves the version string associated with the specified metadata.
    /// </summary>
    /// <param name="metadata">The metadata to retrieve the version string for.</param>
    /// <returns>The version string associated with the specified metadata.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The specified metadata is not supported.</exception>
    public static string GetVersionString(IRStringTableMetadata metadata)
    {
        if (TryGetVersionString(metadata, out var result))
        {
            return result;
        }

        throw new ArgumentOutOfRangeException(nameof(metadata), metadata, "The specified metadata is not supported.");
    }

    /// <summary>
    /// Attempts to retrieve the version string associated with the specified metadata.
    /// </summary>
    /// <param name="metadata">The metadata to retrieve the version string for.</param>
    /// <param name="version">The version string associated with the specified metadata.</param>
    /// <returns><see langword="true"/> if the version string was successfully retrieved; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetVersionString(IRStringTableMetadata metadata, [NotNullWhen(true)] out string? version)
    {
        version = metadata switch
        {
            {
                Version: 2,
                HashAlgorithm.BitMask.Bits: 40
            } or ILegacyFontConfigMetadata => "v2",
            {
                Version: 3,
                HashAlgorithm.BitMask.Bits: 40
            } => "v3",
            {
                Version: 4,
                HashAlgorithm.BitMask.Bits: 39
            } => "v4",
            {
                Version: 5,
                HashAlgorithm.Options:
                {
                    Format: RSTHashAlgorithmFormat.XxHash64,
                    BitMask.Bits: 39
                }
            } => "v5-legacy",
            {
                Version: 5,
                HashAlgorithm.Options:
                {
                    Format: RSTHashAlgorithmFormat.XxHash3,
                    BitMask.Bits: 39
                }
            } => "v5-patch1415",
            {
                Version: 5,
                HashAlgorithm.Options:
                {
                    Format: RSTHashAlgorithmFormat.XxHash3,
                    BitMask.Bits: 38
                }
            } => "v5-patch1502",
            _ => null
        };

        return version is not null;
    }

    /// <summary>
    /// Attempts to retrieve the metadata for the specified version.
    /// </summary>
    /// <param name="version">The version string to retrieve metadata for.</param>
    /// <param name="metadata">The metadata for the specified version.</param>
    /// <returns><see langword="true"/> if the metadata was successfully retrieved; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetMetadata([NotNullWhen(true)] string? version, [NotNullWhen(true)] out IRStringTableMetadata? metadata)
    {
        if (version is null or { Length: < 1 })
        {
            metadata = null;
            return false;
        }

        metadata = version.ToLower() switch
        {
            "latest" => Latest,
            "v5-patch1502" => Version5Patch1502,
            "v5-patch1415" => Version5Patch1415,
            "v5-legacy" => Version5Legacy,
            "4" or "v4" => Version4,
            "3" or "v3" => Version3,
            "2" or "v2" => new LegacyFontConfigMetadata(),
            _ => null
        };

        return metadata is not null;
    }
}

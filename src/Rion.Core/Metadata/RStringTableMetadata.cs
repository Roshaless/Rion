// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics.CodeAnalysis;

using Rion.Core.Hashing;
using Rion.Core.Metadata.Legacy;

namespace Rion.Core.Metadata;

/// <summary>
/// Represents the metadata for a string table, including hashing algorithm and version details.
/// </summary>
public sealed record RStringTableMetadata : IRStringTableMetadata
{
    /// <summary>
    /// Represents the metadata for version 2, indicating a legacy configuration without a backing font configuration.
    /// </summary>
    public static readonly IRStringTableMetadata Version2 = LegacyFontConfigMetadata.NullMetadata;

    /// <summary>
    /// Represents the metadata for version 3, indicating a legacy configuration with a specific hash bits mask type set to <see cref="RSTHashBitsMaskType.Mask40"/>.
    /// </summary>
    public static readonly IRStringTableMetadata Version3 = new LegacyNoFontConfigMetadata(false);

    /// <summary>
    /// Represents the metadata for version 4, indicating a legacy configuration with a specific hash bits mask type set to <see cref="RSTHashBitsMaskType.Mask39"/>.
    /// </summary>
    public static readonly IRStringTableMetadata Version4 = new LegacyNoFontConfigMetadata(true);

    /// <summary>
    /// Represents the metadata for version 5 legacy configurations applicable to versions prior to v14.15.
    /// </summary>
    public static readonly IRStringTableMetadata Version5Legacy = new LegacyRStringTableMetadata();

    /// <summary>
    /// Points to the most up-to-date version of the string table metadata.
    /// This is used as the default for creating new <see cref="RStringTable"/> instances.
    /// </summary>
    public static readonly IRStringTableMetadata Latest = new RStringTableMetadata();

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
    /// Retrieves the metadata for the specified version.
    /// </summary>
    /// <param name="version">The version string to retrieve metadata for.</param>
    /// <returns>The metadata for the specified version.</returns>
    /// <exception cref="ArgumentException">The specified version is not supported.</exception>
    public static IRStringTableMetadata GetMetadata(int version)
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
    /// Attempts to retrieve the metadata for the specified version.
    /// </summary>
    /// <param name="version">The version string to retrieve metadata for.</param>
    /// <param name="metadata">The metadata for the specified version.</param>
    /// <returns><see langword="true"/> if the metadata was successfully retrieved; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetMetadata(int version, [NotNullWhen(true)] out IRStringTableMetadata? metadata)
    {
        metadata = version switch
        {
            5 => Latest,
            4 => Version4,
            3 => Version3,
            2 => new LegacyFontConfigMetadata(),
            _ => null
        };

        return metadata is not null;
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
                HashAlgorithm: { BitsMaskType: RSTHashBitsMaskType.Mask40, TrimmingOption: RSTHashTrimmingOption.None }
            } or ILegacyFontConfigMetadata => "v2",
            {
                Version: 3,
                HashAlgorithm: { BitsMaskType: RSTHashBitsMaskType.Mask40, TrimmingOption: RSTHashTrimmingOption.None }
            } => "v3",
            {
                Version: 4,
                HashAlgorithm: { BitsMaskType: RSTHashBitsMaskType.Mask39, TrimmingOption: RSTHashTrimmingOption.None }
            } => "v4",
            {
                Version: 5,
                HashAlgorithm: { BitsMaskType: RSTHashBitsMaskType.Mask39, TrimmingOption: RSTHashTrimmingOption.None }
            } => "v5-old",
            {
                Version: 5,
                HashAlgorithm:
                {
                    BitsMaskType: RSTHashBitsMaskType.Mask39,
                    TrimmingOption: RSTHashTrimmingOption.TrimHigh3BytesWithMaskLow8Bits
                }
            } => "v5",
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
        metadata = version switch
        {
            "v5-old" => Version5Legacy,
            "5" or "v5" => Latest,
            "4" or "v4" => Version4,
            "3" or "v3" => Version3,
            "2" or "v2" => new LegacyFontConfigMetadata(),
            _ => null
        };

        return metadata is not null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTableMetadata"/> class.
    /// </summary>
    private RStringTableMetadata() { }

    /// <inheritdoc />
    public IRSTHashAlgorithm HashAlgorithm => RSTHashAlgorithm.Latest;

    /// <inheritdoc />
    public byte Version => 5;

    /// <summary>
    /// Represents the legacy specific metadata for string table configurations used in versions prior to v14.15.
    /// This class is designed to maintain compatibility with historical system requirements and includes
    /// details on the hashing algorithm in use during that period.
    /// </summary>
    private sealed class LegacyRStringTableMetadata : IRStringTableMetadata
    {
        /// <inheritdoc />
        public IRSTHashAlgorithm HashAlgorithm => RSTHashAlgorithm.LegacyV4V5;

        /// <inheritdoc />
        public byte Version => 5;
    }
}

// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using Rion.Core.Hashing;
using Rion.Core.Hashing.Legacy;
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
    public static readonly IRStringTableMetadata Version3 = new LegacyNoFontConfigMetadata(RSTHashBitsMaskType.Mask40);

    /// <summary>
    /// Represents the metadata for version 4, indicating a legacy configuration with a specific hash bits mask type set to <see cref="RSTHashBitsMaskType.Mask39"/>.
    /// </summary>
    public static readonly IRStringTableMetadata Version4 = new LegacyNoFontConfigMetadata(RSTHashBitsMaskType.Mask39);

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
    /// Defines the contract for metadata related to RStringTable, specifying hash algorithm and version properties.
    /// </summary>
    private RStringTableMetadata() { }

    /// <inheritdoc />
    public IRSTHashAlgorithm HashAlgorithm => RSTHashAlgorithm.BitsMask39;

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
        public IRSTHashAlgorithm HashAlgorithm => LegacyRSTHashAlgorithm.BitsMask39;

        /// <inheritdoc />
        public byte Version  => 5;
    }
}

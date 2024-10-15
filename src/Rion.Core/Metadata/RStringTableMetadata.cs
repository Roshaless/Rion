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
/// The metadata of string table.
/// </summary>
public sealed record class RStringTableMetadata : IRStringTableMetadata
{
    /// <summary>
    /// A metadata for old version 2 (with no backing font config).
    /// </summary>
    public static readonly IRStringTableMetadata Version2 = LegacyFontConfigMetadata.NullMetadata;

    /// <summary>
    /// A metadata for old version 3.
    /// </summary>
    public static readonly IRStringTableMetadata Version3 = new LegacyNoFontConfigMetadata(RSTHashBitsMaskType.Mask40);

    /// <summary>
    /// A metadata for old version 4.
    /// </summary>
    public static readonly IRStringTableMetadata Version4 = new LegacyNoFontConfigMetadata(RSTHashBitsMaskType.Mask39);

    /// <summary>
    /// A metadata for version 5 less than v14.15.
    /// </summary>
    public static readonly IRStringTableMetadata Version5Legacy = new LegacyRStringTableMetadata();

    /// <summary>
    /// A metadata for latest (version 5 greater than v14.15+).
    /// </summary>
    public static readonly IRStringTableMetadata Latest = new RStringTableMetadata();

    /// <summary>
    /// The metadata for version 5 less than v14.15.
    /// </summary>
    private sealed class LegacyRStringTableMetadata : IRStringTableMetadata
    {
        /// <inheritdoc/>
        public IRSTHashAlgorithm HashAlgorithm => LegacyRSTHashAlgorithm.BitsMask39;

        /// <inheritdoc/>
        public byte Version => 5;
    }

    /// <inheritdoc/>
    public IRSTHashAlgorithm HashAlgorithm => RSTHashAlgorithm.BitsMask39;

    /// <inheritdoc/>
    public byte Version => 5;

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTableMetadata"/> class.
    /// </summary>
    public RStringTableMetadata() { }
}

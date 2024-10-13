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
/// 
/// </summary>
public sealed record class RStringTableMetadata : IRStringTableMetadata
{
    /// <summary>
    /// 
    /// </summary>
    public static IRStringTableMetadata Version2 { get; } = LegacyFontConfigMetadata.NullMetadata;

    /// <summary>
    /// 
    /// </summary>
    public static IRStringTableMetadata Version3 { get; } = new LegacyNoFontConfigMetadata(RSTHashBitsMaskType.Mask40);

    /// <summary>
    /// 
    /// </summary>
    public static IRStringTableMetadata Version4 { get; } = new LegacyNoFontConfigMetadata(RSTHashBitsMaskType.Mask39);

    /// <summary>
    /// Get if that verison is less than (<![CDATA[<]]>) 14.15
    /// </summary>
    public static IRStringTableMetadata Version5Legacy { get; } = new LegacyRStringTableMetadata();

    /// <summary>
    /// 
    /// </summary>
    public static IRStringTableMetadata Latest { get; } = new RStringTableMetadata();

    /// <summary>
    /// 
    /// </summary>
    private sealed class LegacyRStringTableMetadata : IRStringTableMetadata
    {
        /// <inheritdoc/>
        public IRSTHashAlgorithm HashAlgorithm => LegacyRSTHashAlgorithm.MaskType39;

        /// <inheritdoc/>
        public byte Version => 5;
    }

    /// <inheritdoc/>
    public IRSTHashAlgorithm HashAlgorithm => RSTHashAlgorithm.MaskType39;

    /// <inheritdoc/>
    public byte Version => 5;

    /// <summary>
    /// 
    /// </summary>
    public RStringTableMetadata() { }
}

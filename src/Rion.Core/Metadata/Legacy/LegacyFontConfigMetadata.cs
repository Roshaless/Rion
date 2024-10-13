// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using Rion.Core.Hashing;
using Rion.Core.Hashing.Legacy;

namespace Rion.Core.Metadata.Legacy;

/// <summary>
/// 
/// </summary>
public sealed record class LegacyFontConfigMetadata : ILegacyFontConfigMetadata
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly ILegacyFontConfigMetadata NullMetadata = new NullFontConfigMetadata();

    /// <summary>
    /// 
    /// </summary>
    private sealed class NullFontConfigMetadata : ILegacyFontConfigMetadata
    {
        /// <inheritdoc/>
        public IRSTHashAlgorithm HashAlgorithm => LegacyRSTHashAlgorithm.MaskType40;

        /// <inheritdoc/>
        public byte Version => 2;

        /// <inheritdoc/>
        public string? FontConfig
        {
            get => Internal.Nothing.ToDo<string>();
            set => Internal.Nothing.ToDo();
        }
    }

    /// <inheritdoc/>
    public IRSTHashAlgorithm HashAlgorithm { get; }

    /// <inheritdoc/>
    public byte Version { get; }

    /// <summary>
    /// 
    /// </summary>
    public string? FontConfig { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public LegacyFontConfigMetadata()
    {
        Version = 2;
        HashAlgorithm = LegacyRSTHashAlgorithm.MaskType40;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fontConfig"></param>
    public LegacyFontConfigMetadata(string fontConfig) : this() => FontConfig = fontConfig;
}

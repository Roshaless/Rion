// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using Rion.Core.Hashing;
using Rion.Core.Hashing.Legacy;

namespace Rion.Core.Metadata.Legacy;

/// <summary>
/// [Legacy] The metadata with font config for old version (v2).
/// </summary>
public sealed record class LegacyFontConfigMetadata : ILegacyFontConfigMetadata
{
    /// <summary>
    /// A metadata with no backing font config.
    /// </summary>
    public static readonly ILegacyFontConfigMetadata NullMetadata = new NullFontConfigMetadata();

    /// <summary>
    /// The metadata with no backing font config.
    /// </summary>
    private sealed class NullFontConfigMetadata : ILegacyFontConfigMetadata
    {
        /// <inheritdoc/>
        public IRSTHashAlgorithm HashAlgorithm => LegacyRSTHashAlgorithm.BitsMask40;

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

    /// <inheritdoc/>
    public string? FontConfig { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyFontConfigMetadata"/> class.
    /// </summary>
    public LegacyFontConfigMetadata()
    {
        Version = 2;
        HashAlgorithm = LegacyRSTHashAlgorithm.BitsMask40;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyFontConfigMetadata"/> class based on the specified font config.
    /// </summary>
    /// <param name="fontConfig">The font config.</param>
    public LegacyFontConfigMetadata(string fontConfig) : this() => FontConfig = fontConfig;
}

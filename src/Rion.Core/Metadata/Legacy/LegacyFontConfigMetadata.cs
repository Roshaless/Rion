// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using Rion.Core.Hashing;
using Rion.Core.Hashing.Legacy;

namespace Rion.Core.Metadata.Legacy;

/// <summary>
/// Defines the contract for legacy font configuration metadata, including versioning and hash algorithm details.
/// </summary>
public sealed record LegacyFontConfigMetadata : ILegacyFontConfigMetadata
{
    /// <summary>
    /// Represents a constant null instance of <see cref="ILegacyFontConfigMetadata"/>,
    /// providing default property values indicative of an uninitialized or placeholder metadata object.
    /// </summary>
    public static readonly ILegacyFontConfigMetadata NullMetadata = new NullFontConfigMetadata();

    /// <summary>
    /// Represents a null implementation of <see cref="ILegacyFontConfigMetadata"/>,
    /// providing default values for font configuration metadata properties indicating a lack of actual data.
    /// This class is utilized when a metadata object is needed for placeholder or error scenarios.
    /// </summary>
    private sealed class NullFontConfigMetadata : ILegacyFontConfigMetadata
    {
        /// <inheritdoc />
        public IRSTHashAlgorithm HashAlgorithm => LegacyRSTHashAlgorithm.BitsMask40;

        /// <inheritdoc />
        public byte Version => 2;

        /// <inheritdoc />
        public string? FontConfig { get => null; set { } }
    }

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
    /// <param name="fontConfig">The font config to set.</param>
    public LegacyFontConfigMetadata(string? fontConfig) : this() => FontConfig = fontConfig;

    /// <inheritdoc />
    public IRSTHashAlgorithm HashAlgorithm { get; }

    /// <inheritdoc />
    public byte Version { get; }

    /// <inheritdoc />
    public string? FontConfig { get; set; }
}

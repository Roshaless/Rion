// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;

using Rion.Core.Hashing;

namespace Rion.Cli.Commands;

/// <summary>
/// Provides version-string to <see cref="RSTHashAlgorithmOptions"/> resolution shared across commands.
/// </summary>
internal static class VersionHelper
{
    /// <summary>
    /// Maps a version identifier string to its <see cref="RSTHashAlgorithmOptions"/> and a human-readable label.
    /// </summary>
    public static (string Label, RSTHashAlgorithmOptions Options) Resolve(string version)
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
                $"Unknown version '{version}'. Supported: v2, v3, v4, v5, v5-legacy, v5-1415, v5-1502, latest."),
        };
    }
}

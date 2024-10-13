// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using Rion.Core.Hashing;

namespace Rion.Core.Metadata;

/// <summary>
/// 
/// </summary>
public interface IRStringTableMetadata
{
    /// <summary>
    /// 
    /// </summary>
    IRSTHashAlgorithm HashAlgorithm { get; }

    /// <summary>
    /// 
    /// </summary>
    byte Version { get; }
}

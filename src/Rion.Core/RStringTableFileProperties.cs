// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Diagnostics.CodeAnalysis;

using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Defines the properties of a string table file, including metadata, entry count, offsets for hashes, and content.
/// This record struct is utilized to store critical information about the structure and content of a string table file.
/// </summary>
public sealed record RStringTableFileProperties
{
    /// <summary>
    /// Represents the properties of a string table file, encapsulating crucial metadata and structural details.
    /// This record struct is utilized to store critical information about the structure and content of a string table file,
    /// including metadata, entry count, and offsets for hashes and content.
    /// </summary>
    public RStringTableFileProperties() { }

    /// <summary>
    /// Provides properties of a string table file, including metadata, entry count, and offsets for efficient data access.
    /// This class encapsulates essential details for reading and interpreting the structure of a string table file.
    /// </summary>
    [SetsRequiredMembers]
    public RStringTableFileProperties(IRStringTableMetadata metadata, int entryCount, int hashesOffset,
        int contentOffset)
    {
        Metadata = metadata;
        EntryCount = entryCount;
        HashesOffset = hashesOffset;
        ContentOffset = contentOffset;
    }

    /// <summary>
    /// Gets the metadata associated with the string table file.
    /// </summary>
    /// <value>
    /// An instance of <see cref="IRStringTableMetadata"/> containing details like hash algorithm and version of the string table.
    /// </value>
    public required IRStringTableMetadata Metadata { get; init; }

    /// <summary>
    /// Gets the count of entries in the string table.
    /// </summary>
    /// <value>
    /// An integer representing the total number of entries within the string table.
    /// </value>
    public required int EntryCount { get; init; }

    /// <summary>
    /// Gets the offset position of the hashes section within the string table file's data.
    /// </summary>
    /// <value>
    /// The integer value representing the starting position of the hashes block, measured in bytes from the beginning of the file.
    /// </value>
    public required int HashesOffset { get; init; }

    /// <summary>
    /// Gets the offset of the content within the string table file's data.
    /// This property holds the starting position, in bytes, from the beginning of the file where the actual content (strings) begin.
    /// </summary>
    /// <value>
    /// The integer value representing the offset of the content section.
    /// </value>
    public required int ContentOffset { get; init; }
}

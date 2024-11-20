// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Rion.Core.Buffers;
using Rion.Core.Internal;
using Rion.Core.Metadata;

namespace Rion.Core;

/// <summary>
/// Represents a reader for string tables stored in binary format, enabling efficient retrieval of hashed strings and their content.
/// </summary>
public sealed class RStringTableReader
{
    /// <summary>
    /// bitmask applied to raw hash values to isolate the actual hash for operations,
    /// excluding any metadata or alignment bits that may be embedded.
    /// </summary>
    private readonly ulong _bitsMask;

    /// <summary>
    /// Pointer to the beginning of the content in the string table data.
    /// This pointer directly accesses the sequential string data, with each string being null-terminated.
    /// </summary>
    private readonly unsafe byte* _contentPtr;

    /// <summary>
    /// The number of bits dedicated to representing the hash portion in the combined hash and offset value.
    /// This value is derived from the hash algorithm's configuration and is used to calculate the
    /// actual hash by applying a bitmask along with <see cref="_bitsMask"/>.
    /// </summary>
    private readonly int _hashBits;

    /// <summary>
    /// Pointer to the array of hashes in the string table's binary data.
    /// This unsafe pointer directly accesses the hash values used for string lookups,
    /// enhancing the performance of hash-based retrieval operations.
    /// </summary>
    private readonly unsafe ulong* _hashesPtr;

    /// <summary>
    /// The provider instance responsible for reading string table properties and converting binary data into strings.
    /// It holds a reference to an <see cref="RStringTableReaderProvider"/> implementation,
    /// defining behaviors crucial for interpreting the string table's binary format.
    /// </summary>
    private readonly RStringTableReaderProvider _provider;

    /// <summary>
    /// Represents a reader for string tables, providing functionality to read hashes and string content from binary data.
    /// </summary>
    public RStringTableReader(ReadOnlySpan<byte> data) : this(data, RStringTableReaderProvider.Default) { }

    /// <summary>
    /// Provides a reader for string tables in binary format, allowing iteration over hashed strings and their content.
    /// Initializes with binary data and an optional custom provider for specialized parsing.
    /// </summary>
    public RStringTableReader(ReadOnlySpan<byte> data, RStringTableReaderProvider provider)
    {
        VerifySignature(data);

        unsafe
        {
            var properties = provider.ReadFileProperties(new RBufferReader(data));
            {
                var dataPtr = data.AsReference().AsPointer();
                _hashesPtr = (ulong*)Unsafe.Add<byte>(dataPtr, properties.HashesOffset);
                _contentPtr = (byte*)Unsafe.Add<byte>(dataPtr, properties.ContentOffset);
            }

            var hashAlgorithm = properties.Metadata.HashAlgorithm;
            {
                _bitsMask = hashAlgorithm.BitsMaskValue;
                _hashBits = (int)hashAlgorithm.BitsMaskType;
            }

            _provider = provider;

            Metadata = properties.Metadata;
            EntryCount = properties.EntryCount;
        }
    }

    /// <summary>
    /// Provides metadata information about the string table, including details like hash algorithm and versioning.
    /// </summary>
    public IRStringTableMetadata Metadata { get; }

    /// <summary>
    /// Gets the total number of entries in the string table.
    /// </summary>
    /// <value>
    /// An integer representing the count of string entries in the table.
    /// </value>
    public int EntryCount { get; }

    /// <summary>
    /// Retrieves all entries from the string table as a collection of KeyValuePair containing hashes and their respective string content.
    /// </summary>
    /// <returns>
    /// An enumerable collection of KeyValuePair where each pair contains:
    /// - Key: The hash value as ulong representing the string entry.
    /// - Value: The actual string content associated with the hash.
    /// </returns>
    public IEnumerable<KeyValuePair<ulong, string>> ReadAll() => new HashAndStringEnumerator(this);

    /// <summary>
    /// Retrieves an enumerable collection of hashes for all string entries stored in the <see cref="RStringTableReader"/>.
    /// </summary>
    /// <returns>
    /// An enumerable collection of ulong representing the hashes of the string entries without
    /// materializing the full content, optimized for iterating over large datasets.
    /// </returns>
    public IEnumerable<ulong> ReadHashes() => new FastHashesEnumerator(this);

    /// <summary>
    /// Reads the string content at the specified offset within the string table.
    /// </summary>
    /// <param name="offset">The zero-based offset from the start of the content pointing to the desired string.</param>
    /// <returns>The string located at the given offset in the string table.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadContent(int offset)
    {
        unsafe
        {
            return _provider.ReadString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(_contentPtr + offset));
        }
    }

    /// <summary>
    /// Verifies the signature of the binary data to confirm it's a valid RST file header.
    /// Throws an <see cref="InvalidDataException"/> if the header signature is incorrect.
    /// </summary>
    /// <param name="data">The binary data span to verify the signature of.</param>
    private static void VerifySignature(ReadOnlySpan<byte> data)
    {
        byte b0 = data[0], b1 = data[1], b2 = data[2];
        if (b0 != 0x52 || b1 != 0x53 || b2 != 0x54)
        {
            ThrowHelper.ThrowInvalidFileHeaderException(b0, b1, b2);
        }
    }

    /// <summary>
    /// Efficiently enumerates the hashes of string entries from an <see cref="RStringTableReader"/>.
    /// This enumerator is designed for iterating through large sets of hashes without loading
    /// the associated string content into memory, optimizing performance for scenarios that
    /// solely require hash processing.
    /// </summary>
    private struct FastHashesEnumerator(RStringTableReader reader) : IEnumerable<ulong>, IEnumerator<ulong>
    {
        private int _index = -1;

        public unsafe ulong Current
        {
            get => reader._hashesPtr[_index] & reader._bitsMask;
        }

        public bool MoveNext() => ++_index < reader.EntryCount;

        public void Reset() => _index = -1;

        public readonly IEnumerator<ulong> GetEnumerator() => this;

        readonly IEnumerator IEnumerable.GetEnumerator() => this;

        object IEnumerator.Current
        {
            get => Current;
        }

        public readonly void Dispose() => Nothing.ToDo();
    }


    /// <summary>
    /// Represents an enumerator that iterates through the entries of a string table, yielding each entry's hash and its
    /// corresponding string content. This struct is designed to be used internally by <see cref="RStringTableReader"/>
    /// to provide an enumerable collection of string table entries with both hash values and string contents.
    /// </summary>
    private struct HashAndStringEnumerator(RStringTableReader reader)
        : IEnumerable<KeyValuePair<ulong, string>>, IEnumerator<KeyValuePair<ulong, string>>
    {
        private int _index = -1;

        public unsafe KeyValuePair<ulong, string> Current
        {
            get
            {
                var rawHash = reader._hashesPtr[_index];
                var hash = rawHash & reader._bitsMask;
                var offset = (int)(rawHash >> reader._hashBits);
                return new KeyValuePair<ulong, string>(hash, reader.ReadContent(offset));
            }
        }

        public bool MoveNext() => ++_index < reader.EntryCount;

        public void Reset() => _index = -1;

        public readonly IEnumerator<KeyValuePair<ulong, string>> GetEnumerator() => this;

        readonly IEnumerator IEnumerable.GetEnumerator() => this;

        object IEnumerator.Current
        {
            get => Current;
        }

        public readonly void Dispose() => Nothing.ToDo();
    }
}

// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Rion.Core.Buffers;

namespace Rion.Core;

public partial class RStringTableWriter
{
    /// <summary>
    /// Represents a private class within <see cref="RStringTableWriter"/> responsible for managing
    /// the content writing process of a string table, including encoding, storage, and disposal of resources.
    /// </summary>
    private sealed class RContentWriter : IDisposable
    {
        /// <summary>
        /// Internal buffer writer instance utilized for efficiently writing encoded data.
        /// This member ensures that data is sequentially written into memory before further processing or storage.
        /// </summary>
        private readonly RBufferWriter _bufferWriter;

        /// <summary>
        /// Encoder instance for efficient conversion of strings into byte sequences, utilized internally within <see cref="RContentWriter"/>.
        /// </summary>
        private readonly RStringEncoder _stringEncoder;

        /// <summary>
        /// A dictionary mapping unique strings to their respective offsets within the internal buffer.
        /// This member ensures efficient storage by preventing duplication of string data.
        /// </summary>
        private readonly Dictionary<string, int> _textToOffset;

        /// <summary>
        /// Represents a writer for string content used within the <see cref="RStringTableWriter"/> context.
        /// This class is responsible for managing buffer operations and interning strings to efficiently write
        /// string data into a binary format.
        /// </summary>
        public RContentWriter()
        {
            _textToOffset = InternDictCachePool.Shared.Rent();
            _bufferWriter = RBufferWriterCachePool.Shared.Rent();
            _stringEncoder = new RStringEncoder(4096);
        }

        /// <summary>
        /// Exposes the raw byte data that has been written to the internal buffer.
        /// This property allows direct access to the encoded data for further processing or storage operations.
        /// </summary>
        public Span<byte> RawData
        {
            get => _bufferWriter.RawData;
        }

        /// <summary>
        /// Gets the total number of bytes written to the internal buffer,
        /// representing the current length of the encoded data.
        /// </summary>
        /// <value>
        /// An integer indicating the combined length of data written by both
        /// the main <see cref="RBufferWriter"/> and the <see cref="RContentWriter"/>'s internal buffer.
        /// </value>
        public int Length
        {
            get => _bufferWriter.Length;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            InternDictCachePool.Shared.Return(_textToOffset);
            RBufferWriterCachePool.Shared.Return(_bufferWriter);
            _stringEncoder.Dispose();
        }

        /// <summary>
        /// Quickly retrieves the byte data representation of a given string using an optimized encoder.
        /// This method is designed for performance by utilizing aggressive inlining and avoiding
        /// unnecessary allocations, suitable for scenarios requiring frequent and efficient string encoding.
        /// </summary>
        /// <param name="str">The string to be converted into its byte sequence.</param>
        /// <returns>A read-only span of bytes representing the encoded string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> FastGetBytes(string str) => _stringEncoder.GetBytes(str);

        /// <summary>
        /// Retrieves the existing offset of the provided string within the written data, or adds the string to the data and returns the new offset.
        /// </summary>
        /// <param name="str">The string to locate or insert into the written content.</param>
        /// <returns>The offset position of the string within the written data.</returns>
        public int GetOffsetOrAddNew(string str)
        {
            ref var offset = ref CollectionsMarshal.GetValueRefOrAddDefault(_textToOffset, str, out var isExists);
            if (isExists is true) return offset;

            offset = _bufferWriter.Position;
            _bufferWriter.Write(_stringEncoder.GetBytesWithNullChar(str));

            return offset;
        }
    }
}

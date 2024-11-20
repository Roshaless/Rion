// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;

namespace Rion.Core.Buffers;

/// <summary>
/// Defines a contract for writing bytes into a buffer with control over the buffer's state.
/// </summary>
public interface IRBufferWriter : IDisposable
{
    /// <summary>
    /// Gets the current length of the buffer.
    /// </summary>
    /// <value>The length of the data in the buffer.</value>
    int Length { get; }

    /// <summary>
    /// Gets or sets the current write position within the buffer.
    /// </summary>
    /// <value>The current position.</value>
    int Position { get; set; }

    /// <summary>
    /// Exposes the raw binary data contained within the buffer as a <see cref="Span{T}"/> of bytes.
    /// This allows direct access to the underlying buffer for read or write operations.
    /// </summary>
    /// <value>A <see cref="Span{T}"/> representing the raw data in the buffer.</value>
    Span<byte> RawData { get; }

    /// <summary>
    /// Resets the buffer's position and length to their initial states, allowing for reuse.
    /// </summary>
    void Reset();

    /// <summary>
    /// Writes a single byte into the buffer at the current position and advances the position by one.
    /// </summary>
    /// <param name="value">The byte value to be written.</param>
    void WriteByte(byte value);

    /// <summary>
    /// Writes a value of any unmanaged type into the buffer at the current position and advances the position accordingly.
    /// </summary>
    /// <param name="value">The unmanaged value to be written into the buffer.</param>
    /// <typeparam name="T">The unmanaged type of the value to write.</typeparam>
    void WriteValue<T>(T value) where T : unmanaged;

    /// <summary>
    /// Writes a span of bytes into the buffer at the current position and advances the position by the span's length.
    /// </summary>
    /// <param name="data">The bytes to be written into the buffer.</param>
    void Write(scoped ReadOnlySpan<byte> data);
}

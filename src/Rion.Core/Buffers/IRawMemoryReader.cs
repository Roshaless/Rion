// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Rion.Core.Buffers;

/// <summary>
/// Defines a contract for reading from a buffer with capabilities to read sequences of bytes,
/// unmanaged values, and structs while tracking the read position.
/// </summary>
public interface IRawMemoryReader
{
    /// <summary>
    /// Gets the total number of bytes available for reading in the buffer.
    /// </summary>
    int Length { get; }

    /// <summary>
    /// Gets or sets the current read position within the buffer.
    /// </summary>
    /// <value>The current position in bytes from the start of the buffer.</value>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when setting the position is outside the buffer bounds.</exception>
    int Position { get; set; }

    /// <summary>
    /// Reads a sequence of bytes from the current buffer and advances the position within the current buffer by the specified count.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>A read-only span of bytes containing the read sequence.</returns>
    ReadOnlySpan<byte> Read(int count);

    /// <summary>
    /// Reads a value of type <typeparamref name="T"/> from the current buffer and advances the position within the current buffer by the size of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to read, must be an unmanaged type.</typeparam>
    /// <returns>The value of type <typeparamref name="T"/> read from the buffer.</returns>
    T Read<T>() where T : unmanaged;

    /// <summary>
    /// Reads a value of type <typeparamref name="T"/> from the current buffer as a struct and advances the position within the current buffer by the size of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the struct to read.</typeparam>
    /// <returns>The struct of type <typeparamref name="T"/> read from the buffer.</returns>
    T ReadAsStruct<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>() where T : struct;
}

// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Rion.Core.Internal;

namespace Rion.Core.Buffers;

/// <summary>
/// Represents a reader for reading data from a buffer.
/// This class provides methods to read bytes, structures, and strings from a given buffer.
/// </summary>
public sealed class RBufferReader : IRBufferReader
{
    /// <summary>
    /// Pointer reference to the start of the buffer data.
    /// </summary>
    private readonly unsafe byte* _byteRef;

    /// <summary>
    /// Provides functionality to read string table data from a binary buffer.
    /// Initializes with buffer data and a provider to handle file properties and string retrieval.
    /// </summary>
    public RBufferReader(ReadOnlySpan<byte> data)
    {
        unsafe
        {
            _byteRef = (byte*)data.AsReference().AsPointer();
            Length = data.Length;
        }
    }

    /// <inheritdoc />
    public int Length { get; }

    /// <inheritdoc />
    public int Position
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, Length);
            {
                field = value;
            }
        }
    }

    /// <inheritdoc />
    public ReadOnlySpan<byte> Read(int count)
    {
        var previousPos = Position;
        var newPosition = checked(previousPos + count);
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(newPosition, Length);
        }

        Position = newPosition;

        unsafe
        {
            return new ReadOnlySpan<byte>(_byteRef + previousPos, count);
        }
    }

    /// <inheritdoc />
    public T Read<T>() where T : unmanaged
    {
        unsafe
        {
            var previousPos = Position;
            var newPosition = checked(previousPos + sizeof(T));
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(newPosition, Length);
            }

            Position = newPosition;
            {
                return *(T*)(_byteRef + previousPos);
            }
        }
    }

    /// <inheritdoc />
    public T ReadAsStruct<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>() where T : struct
    {
        unsafe
        {
            var previousPos = Position;
            var newPosition = checked(previousPos + Marshal.SizeOf<T>());
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(newPosition, Length);
            }

            // Save the result before set new position, avoid exceptions...
            var structure = Marshal.PtrToStructure<T>((nint)(_byteRef + previousPos));
            {
                Position = newPosition;
                return structure;
            }
        }
    }
}

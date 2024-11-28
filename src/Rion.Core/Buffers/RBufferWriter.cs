// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Rion.Core.Internal;

namespace Rion.Core.Buffers;

/// <summary>
/// Represents a buffer writer that provides methods to write primitive types and spans of bytes into a managed buffer.
/// This class is designed to be efficient for sequential writes and can be used in performance-critical sections.
/// </summary>
public sealed class RBufferWriter : SafeHandle, IRBufferWriter
{
    /// <summary>
    /// The capacity of the buffer, indicating the total number of elements that the buffer can hold.
    /// </summary>
    private int _capacity;

    /// <summary>
    /// The current write position within the buffer.
    /// </summary>
    private int _position;

    /// <summary>
    /// Represents a buffer writer that supports writing primitive types and byte spans into a managed buffer.
    /// Optimized for sequential write operations, suitable for performance-sensitive scenarios.
    /// Implements <see cref="IRBufferWriter"/> and extends <see cref="SafeHandle"/>.
    /// </summary>
    public RBufferWriter() : base(nint.Zero, true) { }

    /// <summary>
    /// Manages a pool of <see cref="RBufferWriter"/> instances with a default initial capacity optimized for string table operations.
    /// This class is part of the internal cache management system for <see cref="RStringTableWriter"/>.
    /// </summary>
    public RBufferWriter(int capacity) : base(nint.Zero, true)
    {
        if (capacity > 0)
        {
            unsafe
            {
                SetHandle((nint)NativeMemory.Alloc((nuint)capacity));
                {
                    _capacity = capacity;
                }
            }
        }
    }

    /// <summary>
    /// Gets a pointer reference to the start of the buffer's internal memory, allowing direct access for writing operations.
    /// </summary>
    private unsafe byte* ByteRef
    {
        get => (byte*)handle;
    }

    /// <inheritdoc />
    public override bool IsInvalid
    {
        get => handle == nint.Zero;
    }

    /// <inheritdoc />
    public int Length { get; private set; }

    /// <inheritdoc />
    public int Position
    {
        get => _position;
        set
        {
            EnsureWritable();
            {
                if (value > _position)
                {
                    EnsureCapacity(value - _position);
                }
            }

            _position = value;
        }
    }

    /// <inheritdoc />
    public Span<byte> RawData
    {
        get
        {
            if (IsInvalid)
            {
                return default;
            }

            unsafe
            {
                return new Span<byte>(handle.ToPointer(), Length);
            }
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        _position = 0;
        Length = 0;
    }

    /// <inheritdoc />
    public void WriteByte(byte value)
    {
        EnsureWritable();
        EnsureCapacity(1);

        unsafe
        {
            ByteRef[_position++] = value;
        }
    }

    /// <inheritdoc />
    public void WriteValue<T>(T value) where T : unmanaged
    {
        EnsureWritable();

        unsafe
        {
            var size = sizeof(T);
            EnsureCapacity(size);
            {
                Buffer.MemoryCopy(&value, ByteRef + _position, size, size);
            }

            _position += size;
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped ReadOnlySpan<byte> data)
    {
        unsafe
        {
            Write(data.AsReference().AsPointer(), data.Length);
        }
    }

    /// <summary>
    /// Writes a sequence of bytes from a pointer source into the buffer writer.
    /// Ensures the buffer is writable and has enough capacity before performing the copy operation.
    /// </summary>
    /// <param name="source">The pointer to the source bytes to be written.</param>
    /// <param name="length">The number of bytes to write from the source pointer.</param>
    public unsafe void Write(void* source, int length)
    {
        EnsureWritable();
        EnsureCapacity(length);
        {
            Buffer.MemoryCopy(source, ByteRef + _position, length, length);
        }

        _position += length;
    }

    /// <summary>
    /// Ensures that the buffer has enough capacity to accommodate the specified number of bytes.
    /// If the current capacity is insufficient, the buffer's capacity is increased to the next power of 2 that can hold the required bytes.
    /// </summary>
    /// <param name="bufferSize">The number of additional bytes needed in the buffer.</param>
    private void EnsureCapacity(int bufferSize)
    {
        unsafe
        {
            var newPosition = _position + bufferSize;
            if (newPosition <= Length) return;
            if (newPosition > _capacity)
            {
                var newCapacity = BitOperations.RoundUpToPowerOf2((uint)(_position + bufferSize));
                {
                    SetHandle((nint)NativeMemory.Realloc(handle.ToPointer(), newCapacity));
                }

                _capacity = (int)newCapacity;
            }

            Length = newPosition;
        }
    }

    /// <summary>
    /// Ensures that the buffer writer is in a writable state.
    /// Throws an <see cref="InvalidOperationException"/> if the buffer writer has been disposed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attempting to write to a disposed buffer writer.</exception>
    private void EnsureWritable()
    {
        if (IsInvalid)
        {
            throw new InvalidOperationException("The buffer writer was disposed!");
        }
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        unsafe
        {
            if (handle != nint.Zero)
            {
                NativeMemory.Free(handle.ToPointer());
            }

            handle = nint.Zero;
            return true;
        }
    }
}

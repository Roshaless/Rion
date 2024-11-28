// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Rion.Core.Internal;

namespace Rion.Core.Buffers;

/// <summary>
///  A stream-like class that provides a buffer for writing and reading data.
/// </summary>
public sealed class RBufferStream : Stream
{
    /// <summary>
    ///  A handle to the buffer's underlying memory.
    /// </summary>
    private nint _handle;

    /// <summary>
    /// The capacity of the buffer, indicating the total number of elements that the buffer can hold.
    /// </summary>
    private int _capacity;

    /// <summary>
    /// The current length within the buffer.
    /// </summary>
    private int _length;

    /// <summary>
    /// The current write position within the buffer.
    /// </summary>
    private int _position;

    /// <summary>
    /// Gets a pointer reference to the start of the buffer's internal memory, allowing direct access for writing operations.
    /// </summary>
    private unsafe byte* ByteRef => (byte*)_handle;

    /// <summary>
    /// Gets a value indicating whether the buffer is invalid.
    /// </summary>
    private bool IsInvalid => _handle == nint.Zero;

    /// <inheritdoc />
    public override bool CanRead => !IsInvalid;

    /// <inheritdoc />
    public override bool CanSeek => !IsInvalid;

    /// <inheritdoc />
    public override bool CanWrite => !IsInvalid;

    /// <inheritdoc />
    public override long Length => _length;

    /// <inheritdoc />
    public override long Position
    {
        get => _position;
        set
        {
            EnsureNotClosed();
            {
                if (value > _length)
                {
                    EnsureCapacity((int)(value - _length));
                }
            }

            _position = (int)value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RBufferStream"/> class.
    /// </summary>
    public RBufferStream() { }

    /// <inheritdoc />
    public override void Flush() { }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int Read(byte[] buffer, int offset, int count)
    {
        return Read(new Span<byte>(buffer, offset, count));
    }

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        var sizeHint = buffer.Length;
        var previousPos = (int)Position;
        var newPosition = checked(previousPos + sizeHint);

        if (newPosition >= _length)
        {
            sizeHint = _length - newPosition;
        }

        unsafe
        {
            new ReadOnlySpan<byte>(ByteRef + previousPos, sizeHint).CopyTo(buffer);
        }

        Position = newPosition;
        return sizeHint;
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        EnsureNotClosed();

        var position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _position + offset,
            SeekOrigin.End => _length + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
        };

        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "The specified offset is negative.");
        }

        if (position > _length)
        {
            EnsureCapacity((int)(_length - position));
        }

        _position = (int)position;
        return position;
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        EnsureNotClosed();
        {
            if (value > _length)
            {
                EnsureCapacity((int)(value - _length));
            }
            else
            {
                _length = (int)value;
            }
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(byte[] buffer, int offset, int count)
    {
        unsafe
        {
            Write(buffer.AsReference().At(offset).AsPointer(), count);
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(scoped ReadOnlySpan<byte> data)
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
    private unsafe void Write(void* source, int length)
    {
        EnsureNotClosed();
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
            if (newPosition <= _length) return;
            if (newPosition > _capacity)
            {
                var newCapacity = BitOperations.RoundUpToPowerOf2((uint)(_position + bufferSize));
                {
                    _handle = ((nint)NativeMemory.Realloc(_handle.ToPointer(), newCapacity));
                }

                _capacity = (int)newCapacity;
            }

            _length = newPosition;
        }
    }

    /// <summary>
    /// Ensures that the buffer writer is in a writable state.
    /// Throws an <see cref="InvalidOperationException"/> if the buffer writer has been disposed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attempting to write to a disposed buffer writer.</exception>
    private void EnsureNotClosed()
    {
        if (IsInvalid)
        {
            throw new InvalidOperationException("The buffer stream was disposed!");
        }
    }
}

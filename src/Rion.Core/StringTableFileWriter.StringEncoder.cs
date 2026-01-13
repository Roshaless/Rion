// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

using Rion.Core.Internal;

namespace Rion.Core;

public partial class StringTableFileWriter
{
    /// <summary>
    /// Represents a memory-based encoder for strings, optimized for fast encoding operations.
    /// </summary>
    /// <remarks>
    /// This class is not intended for direct instantiation outside its enclosing class.
    /// It is used internally within the <see cref="StringTableFileWriter"/> to efficiently
    /// encode and manage string data.
    /// </remarks>
    private sealed class StringEncoder : SafeHandle
    {
        /// <summary>
        /// Represents the zero byte value, used for padding or termination in various encoding scenarios.
        /// </summary>
        private const byte ZeroByte = 0b_00_00;

        /// <summary>
        /// The character encoding used for string conversion operations within the <see cref="StringEncoder"/>.
        /// Defaults to UTF-8 encoding.
        /// </summary>
        private readonly Encoding _encoding = Encoding.UTF8;

        /// <summary>
        /// The internal capacity of the raw buffer used for encoding operations.
        /// </summary>
        /// <remarks>
        /// This field represents the size of the buffer that can hold encoded string data.
        /// It is adjusted as needed to accommodate varying sizes of input strings.
        /// </remarks>
        private int _capacity;

        /// <summary>
        /// Represents a memory-based encoder for strings, optimized for fast encoding operations.
        /// </summary>
        /// <remarks>
        /// This class is not intended for direct instantiation outside its enclosing class.
        /// It is used internally within the <see cref="StringTableFileWriter"/> to efficiently
        /// encode and manage string data.
        /// </remarks>
        public StringEncoder(int capacity) : base(nint.Zero, true)
        {
            unsafe
            {
                SetHandle((nint)NativeMemory.Alloc((nuint)capacity));
            }

            _capacity = capacity;
        }

        /// <summary>
        /// Gets a pointer to the raw memory buffer used for encoding operations.
        /// </summary>
        /// <value>
        /// A pointer to the byte array where encoded string data is stored.
        /// </value>
        /// <remarks>
        /// This property exposes direct access to the underlying buffer managed by the encoder.
        /// Caution is advised when using this pointer, as improper use can lead to memory corruption or security issues.
        /// </remarks>
        public unsafe byte* RawBuffer
        {
            get => (byte*)handle;
        }

        /// <inheritdoc />
        public override bool IsInvalid
        {
            get => handle == nint.Zero;
        }

        /// <summary>
        /// Retrieves the byte data of the specified string using the internal character encoder.
        /// </summary>
        /// <param name="s">The string to be converted into bytes.</param>
        /// <returns>A read-only span of bytes representing the encoded string.</returns>
        public ReadOnlySpan<byte> GetBytes(string s)
        {
            EnsureWritable();
            EnsureCapacity(_encoding.GetMaxByteCount(s.Length));

            unsafe
            {
                fixed (char* chars = &s.GetRawStringData())
                {
                    var writtenCount = _encoding.GetBytes(chars, s.Length, RawBuffer, _capacity);
                    return new ReadOnlySpan<byte>(handle.ToPointer(), writtenCount);
                }
            }
        }

        /// <summary>
        /// Retrieves the byte data of the string including a null character using the internal character encoder.
        /// </summary>
        /// <param name="s">The string to be encoded and retrieved as bytes.</param>
        /// <returns>A read-only span of bytes representing the encoded string with an appended null character.</returns>
        public ReadOnlySpan<byte> GetBytesWithNullChar(string s)
        {
            EnsureWritable();
            EnsureCapacity(_encoding.GetMaxByteCount(s.Length));

            unsafe
            {
                fixed (char* chars = &s.GetRawStringData())
                {
                    var writtenCount = _encoding.GetBytes(chars, s.Length, RawBuffer, _capacity);
                    {
                        RawBuffer[writtenCount] = ZeroByte;
                    }

                    return new ReadOnlySpan<byte>(handle.ToPointer(), writtenCount + 1);
                }
            }
        }

        /// <summary>
        /// Ensures that the internal buffer has the required capacity. If the current buffer is too small,
        /// it will be reallocated and expanded to a size that is sufficient to hold the specified number of bytes.
        /// </summary>
        /// <param name="bufferSize">The minimum capacity size required for the internal buffer.</param>
        private void EnsureCapacity(int bufferSize)
        {
            unsafe
            {
                if (bufferSize < _capacity) return;
                var newCapacity = BitOperations.RoundUpToPowerOf2((uint)bufferSize);
                {
                    SetHandle((nint)NativeMemory.Realloc(handle.ToPointer(), newCapacity));
                }

                _capacity = (int)newCapacity;
            }
        }

        /// <summary>
        /// Verifies that the internal buffer is in a writable state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the encoder has been disposed and is no longer valid for writing.</exception>
        private void EnsureWritable()
        {
            if (IsInvalid)
            {
                throw new InvalidOperationException("This string encoder was disposed!");
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
}

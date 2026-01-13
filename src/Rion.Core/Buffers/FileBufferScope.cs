// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

namespace Rion.Core.Buffers;

/// <summary>
/// Represents a scope for managing a file buffer, providing access to its contents as a span of bytes.
/// This class ensures proper handling and disposal of the underlying resources.
/// </summary>
public sealed class FileBufferScope : SafeHandle
{
    /// <summary>
    /// The length of the buffer managed by this scope.
    /// </summary>
    private readonly int _length;

    // The pointer and writer are used to store the file buffer,
    // and the pointer is used if the file bytes can be read all at once,
    // whereas the writer is used to copy the file bytes incrementally if the file size is not known.
    private RawMemoryWriter? _writer;

    /// <summary>
    /// Represents a scope for managing a file buffer, encapsulating access to its contents as a byte span.
    /// Ensures proper resource management and disposal of the underlying buffer.
    /// </summary>
    private FileBufferScope(nint buffer, int length) : base(nint.Zero, true)
    {
        SetHandle(buffer);
        {
            _length = length;
        }
    }

    /// <summary>
    /// Manages a file buffer scope, facilitating access to file content as a byte span.
    /// Automatically handles resource disposal and ensures efficient memory management.
    /// </summary>
    private FileBufferScope(RawMemoryWriter writer) : base(nint.Zero, true) => _writer = writer;

    /// <inheritdoc />
    public override bool IsInvalid => handle == nint.Zero && _writer is null;

    /// <summary>
    /// Gets the span of current buffer.
    /// </summary>
    public Span<byte> Span
    {
        get
        {
            if (IsInvalid)
            {
                return default;
            }

            if (_writer is null)
            {
                unsafe
                {
                    return new Span<byte>(handle.ToPointer(), _length);
                }
            }

            return _writer.RawData;
        }
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        if (handle != nint.Zero)
        {
            unsafe
            {
                NativeMemory.Free(handle.ToPointer());
            }

            handle = nint.Zero;
        }

        _writer?.Dispose();
        _writer = null;

        return true;
    }

    /// <summary>
    /// Creates an <see cref="FileBufferScope"/> instance from the specified file path,
    /// providing access to the file's contents as a managed buffer scope.
    /// </summary>
    /// <param name="path">The path to the file from which to create the buffer scope.</param>
    /// <returns>A new <see cref="FileBufferScope"/> instance initialized with the file's data.</returns>
    public static FileBufferScope CreateFrom(string path)
    {
        static unsafe FileBufferScope ReadByUnknownLength(SafeFileHandle sfh)
        {
            var writer = new RawMemoryWriter();
            void* buffer = stackalloc byte[512];

            try
            {
                while (true)
                {
                    var n = RandomAccess.Read(sfh, new Span<byte>(buffer, 512), writer.Length);
                    if (n == 0)
                    {
                        return new FileBufferScope(writer);
                    }

                    writer.Write(buffer, n);
                }
            }
            finally
            {
                sfh.Dispose();
            }
        }

        var opt = OperatingSystem.IsWindows() ? FileOptions.SequentialScan : FileOptions.None;
        var sfh = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, opt);
        {
            Debug.Assert(SafeFileHandle_CanSeek(sfh));
        }

        var fileLength = (int)SafeFileHandle_GetFileLength(sfh);

#if DEBUG
        fileLength = 0;
#endif

        if (fileLength == 0)
        {
            // Some file systems (e.g. procfs on Linux) return 0 for length even when there's content; also there are non-seekable files.
            // Thus we need to assume 0 doesn't mean empty.
            // return ReadAllBytesUnknownLength(sfh);

            return ReadByUnknownLength(sfh);
        }

        unsafe
        {
            var index = 0;
            var bytes = NativeMemory.Alloc((nuint)fileLength);
            var isComplete = false;

            try
            {
                while (index < fileLength)
                {
                    index += RandomAccess.Read(sfh,
                        new Span<byte>(Unsafe.Add<byte>(bytes, index), fileLength - index), index);
                }

                isComplete = true;
            }
            finally
            {
                if (!isComplete)
                {
                    NativeMemory.Free(bytes);
                }

                sfh.Dispose();
            }

            return new FileBufferScope((nint)bytes, fileLength);
        }
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_CanSeek")]
    private static extern bool SafeFileHandle_CanSeek(SafeFileHandle handle);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetFileLength")]
    private static extern long SafeFileHandle_GetFileLength(SafeFileHandle handle);
}

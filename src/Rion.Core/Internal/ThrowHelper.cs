// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

using Rion.Core.Serialization;

namespace Rion.Core.Internal;

/// <summary>
/// Helper class for throwing specialized exceptions within the application.
/// </summary>
internal static class ThrowHelper
{
    /// <summary>
    /// Throws an exception indicating an invalid file header based on the provided byte values.
    /// </summary>
    /// <param name="b0">The first byte of the file header.</param>
    /// <param name="b1">The second byte of the file header.</param>
    /// <param name="b2">The third byte of the file header.</param>
    /// <exception cref="InvalidDataException">Thrown when the header does not match the expected signature.</exception>
    [DoesNotReturn]
    [DebuggerStepThrough]
    public static void ThrowInvalidFileHeaderException(byte b0, byte b1, byte b2)
        => throw new InvalidDataException($"Invalid rst file header: '{{0x{b0:X}, 0x{b1:X}, 0x{b2:X} }}'");

    /// <summary>
    /// Throws an exception indicating an invalid file header based on the provided span of bytes.
    /// </summary>
    /// <param name="data">A span containing the first three bytes of the file header.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the span is shorter than 3 bytes.</exception>
    /// <exception cref="InvalidDataException">Thrown when the header does not match the expected signature.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowInvalidFileHeaderException(ReadOnlySpan<byte> data)
        => ThrowInvalidFileHeaderException(data[0], data[1], data[2]);

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the specified converter cannot serialize the given type.
    /// </summary>
    /// <param name="converter">The converter to validate for serialization capability.</param>
    /// <param name="targetType">The type to check for serialization support.</param>
    /// <exception cref="InvalidOperationException">Thrown if the <paramref name="converter"/> cannot serialize the specified <paramref name="targetType"/>.</exception>
    [DoesNotReturn]
    [DebuggerStepThrough]
    public static void ThrowIfCanNotSerialize(RStringTableConverter converter, Type targetType)
        => throw new InvalidOperationException(
            $"The converter '{converter.GetType().Name}' cannot serialize the type '{targetType.FullName}'." +
            $" Supported type: '{converter._serializedType.FullName}'.");
}

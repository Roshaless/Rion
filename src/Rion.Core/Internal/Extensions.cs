// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rion.Core.Internal;

/// <summary>
/// Provides a collection of utility extension methods for working with various data structures and types.
/// </summary>
internal static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T At<T>(this ref T source, int index)
        where T : struct => ref Unsafe.Add(ref source, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsReference<T>(this T[] source)
        where T : struct => ref MemoryMarshal.GetArrayDataReference(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsReference<T>(this Span<T> source)
        where T : struct => ref MemoryMarshal.GetReference(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsReference<T>(this ReadOnlySpan<T> source)
        where T : struct => ref MemoryMarshal.GetReference(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void* AsPointer<T>(this ref T source)
        where T : struct => Unsafe.AsPointer(ref source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadAt<T>(this ref byte source, int offset)
        where T : struct => Unsafe.ReadUnaligned<T>(ref source.At(offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? s)
        => string.IsNullOrWhiteSpace(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetRawStringData(this string s)
        //  Unwrap the readonly
        => ref Unsafe.AsRef(in s.GetPinnableReference());
}

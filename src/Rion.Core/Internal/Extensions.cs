// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
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
    public static bool IsNull<T>([NotNullWhen(false)] this T value)
        => value is null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNull<T>([NotNullWhen(true)] this T value)
        => value is not null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace([NotNullWhen(true)] this string? s)
        => string.IsNullOrWhiteSpace(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? s)
        => !string.IsNullOrWhiteSpace(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetRawStringData(this string s)
        //  Unwrap the readonly
        => ref Unsafe.AsRef(in s.GetPinnableReference());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartEndWith<T>(this ReadOnlySpan<T> span, T start, T end) where T : struct, IEquatable<T>
    {
        if (span.Length == 0)
            return false;

        ref var first = ref span.AsReference();
        ref var last = ref At(ref first, span.Length - 1);

        return first.Equals(start) && last.Equals(end);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseHex<T>([NotNullWhen(true)] this ReadOnlySpan<char> s, out T result)
        where T : struct, INumberBase<T> => T.TryParse(s, NumberStyles.HexNumber, null, out result);
}

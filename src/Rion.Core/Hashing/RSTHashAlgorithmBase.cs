// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Rion.Core.Hashing;

/// <summary>
/// Represents the base class for rst hash algorithm implementations, providing fundamental methods and properties.
/// </summary>
public abstract class RSTHashAlgorithmBase : IRSTHashAlgorithm
{
    /// <summary>
    /// Defines the fundamental methods and properties for rst hash algorithm implementations.
    /// This is an abstract class that serves as the base for creating rst hash algorithm instances.
    /// </summary>
    public RSTHashAlgorithmBase(RSTHashBitsMaskType bitsMaskType)
    {
        Debug.Assert(Enum.IsDefined(bitsMaskType));
        {
            BitsMaskType = bitsMaskType;
            BitsMaskValue = bitsMaskType.GetBitsMaskValue();
        }
    }

    /// <inheritdoc />
    public RSTHashBitsMaskType BitsMaskType { get; }

    /// <inheritdoc />
    public ulong BitsMaskValue { get; }

    /// <inheritdoc />
    public virtual ulong Hash(string toHash)
        => Hash(toHash, Encoding.UTF8, CultureInfo.CurrentCulture);

    /// <inheritdoc />
    public virtual ulong Hash(string toHash, Encoding encoding)
        => Hash(toHash, encoding, CultureInfo.CurrentCulture);

    /// <inheritdoc />
    public virtual ulong Hash(string toHash, Encoding? encoding, CultureInfo? culture)
    {
        if (string.IsNullOrWhiteSpace(toHash))
        {
            return Hash(default(ReadOnlySpan<byte>));
        }

        encoding ??= Encoding.UTF8;
        culture ??= CultureInfo.CurrentCulture;

        if (toHash.Length < 256)
        {
            Span<char> destination = stackalloc char[toHash.Length];
            {
                var written = toHash.AsSpan().ToLower(destination, culture);
                Debug.Assert(written == toHash.Length);
            }

            return Hash(encoding.GetBytes(destination.ToArray()));
        }

        return Hash(encoding.GetBytes(toHash.ToLower(culture)));
    }

    /// <inheritdoc />
    public abstract ulong Hash(ReadOnlySpan<byte> source);

    /// <inheritdoc />
    public virtual ulong HashWithOffset(string toHash, long offset)
        => HashWithOffset(toHash, offset, Encoding.UTF8, CultureInfo.CurrentCulture);

    /// <inheritdoc />
    public virtual ulong HashWithOffset(string toHash, long offset, Encoding encoding)
        => HashWithOffset(toHash, offset, encoding, CultureInfo.CurrentCulture);

    /// <inheritdoc />
    public virtual ulong HashWithOffset(string toHash, long offset, Encoding encoding, CultureInfo cultureInfo)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        {
            return WithOffset(Hash(toHash, encoding, cultureInfo), offset);
        }
    }

    /// <inheritdoc />
    public virtual ulong HashWithOffset(ReadOnlySpan<byte> source, long offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        {
            return WithOffset(Hash(source), offset);
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ulong WithOffset(ulong hash, long offset)
        => hash + ((ulong)offset << (byte)BitsMaskType);
}

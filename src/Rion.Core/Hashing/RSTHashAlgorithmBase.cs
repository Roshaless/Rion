// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Rion.Core.Hashing;

/// <summary>
/// The base class of the RST hash algorithm and provides some base implementations.
/// </summary>
public abstract class RSTHashAlgorithmBase : IRSTHashAlgorithm
{
    /// <inheritdoc/>
    public RSTHashBitsMaskType BitsMaskType { get; }

    /// <inheritdoc/>
    public ulong BitsMaskValue { get; }

    /// <summary>
    /// Initializes the base class <see cref="RSTHashAlgorithm"/> with <see cref="RSTHashBitsMaskType"/>.
    /// </summary>
    /// <param name="bitsMaskType">The specified bits mask type.</param>
    public RSTHashAlgorithmBase(RSTHashBitsMaskType bitsMaskType)
    {
        Debug.Assert(Enum.IsDefined(bitsMaskType));
        {
            BitsMaskType = bitsMaskType;
            BitsMaskValue = bitsMaskType.GetBitsMaskValue();
        }
    }

    /// <inheritdoc/>
    public virtual ulong Hash(string toHash)
        => Hash(toHash, Encoding.UTF8, CultureInfo.CurrentCulture);

    /// <inheritdoc/>
    public virtual ulong Hash(string toHash, Encoding encoding)
        => Hash(toHash, encoding, CultureInfo.CurrentCulture);

    /// <inheritdoc/>
    public virtual ulong Hash(string toHash, Encoding encoding, CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(encoding);
        ArgumentNullException.ThrowIfNull(cultureInfo);

        if (toHash.Length < 256)
        {
            Span<char> destination = stackalloc char[toHash.Length];
            {
                var written = toHash.AsSpan().ToLower(destination, cultureInfo);
                Debug.Assert(written == toHash.Length);
            }

            return Hash(encoding.GetBytes(destination.ToArray()));
        }
        else
        {
            return Hash(encoding.GetBytes(toHash.ToLower(cultureInfo)));
        }
    }

    /// <inheritdoc/>
    public abstract ulong Hash(ReadOnlySpan<byte> source);

    /// <inheritdoc/>
    public virtual ulong HashWithOffset(string toHash, long offset)
        => HashWithOffset(toHash, offset, Encoding.UTF8, CultureInfo.CurrentCulture);

    /// <inheritdoc/>
    public virtual ulong HashWithOffset(string toHash, long offset, Encoding encoding)
        => HashWithOffset(toHash, offset, encoding, CultureInfo.CurrentCulture);

    /// <inheritdoc/>
    public virtual ulong HashWithOffset(string toHash, long offset, Encoding encoding, CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(toHash);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        return WithOffset(Hash(toHash, encoding, cultureInfo), offset);
    }

    /// <inheritdoc/>
    public virtual ulong HashWithOffset(ReadOnlySpan<byte> source, long offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        {
            return WithOffset(Hash(source), offset);
        }
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ulong WithOffset(ulong hash, long offset)
        => hash + ((ulong)offset << (byte)BitsMaskType);
}

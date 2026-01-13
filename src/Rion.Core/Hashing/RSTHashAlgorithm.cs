// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Rion.Core.Hashing;

/// <summary>
/// Represents the base class for rst hash algorithm implementations, providing fundamental methods and properties.
/// </summary>
public abstract partial class RSTHashAlgorithm
{
    /// <summary>
    /// Provides a thread-safe cache that maps hash algorithm options to their corresponding algorithm instances.
    /// </summary>
    /// <remarks>This dictionary is used to efficiently retrieve or store instances of hash algorithms based
    /// on specified options. Access to this collection is safe for concurrent read and write operations from multiple
    /// threads.</remarks>
    private static readonly ConcurrentDictionary<RSTHashAlgorithmOptions, RSTHashAlgorithm> s_ht = [];

    /// <summary>
    /// Creates a new instance of an RSTHashAlgorithm based on the specified options.
    /// </summary>
    /// <remarks>If an algorithm instance with the same options has already been created, the existing
    /// instance is returned. Otherwise, a new instance is created and cached for future use.</remarks>
    /// <param name="options">The options that define the hash algorithm format and bit mask to use when creating the algorithm instance.</param>
    /// <returns>An RSTHashAlgorithm instance configured according to the specified options.</returns>
    /// <exception cref="NotSupportedException">Thrown if the specified hash algorithm format in options is not supported.</exception>
    public static RSTHashAlgorithm Create(RSTHashAlgorithmOptions options)
    {
        if (s_ht.TryGetValue(options, out var retvalObj))
        {
            return retvalObj;
        }

        retvalObj = options.Format switch
        {
            RSTHashAlgorithmFormat.XxHash3 => new XxHash3Impl(options),
            RSTHashAlgorithmFormat.XxHash64 => new XxHash64Impl(options),
            _ => throw new NotSupportedException("The specified hash algorithm format is not supported."),
        };

        s_ht[options] = retvalObj;
        return retvalObj;
    }

    /// <summary>
    /// Creates a new instance of the XXHash3 hash algorithm with the specified output bit length.
    /// </summary>
    /// <remarks>XXHash3 is a fast, non-cryptographic hash algorithm suitable for checksums and hash-based
    /// data structures. The output bit length affects the size and uniqueness of the hash values produced.</remarks>
    /// <param name="bitMask">A mask specifying the desired output bit length for the XXHash3 algorithm. Valid values determine whether the
    /// hash output is 64, 128, or another supported bit size.</param>
    /// <returns>An instance of <see cref="RSTHashAlgorithm"/> configured to use the XXHash3 algorithm with the specified bit
    /// length.</returns>
    public static RSTHashAlgorithm CreateXxHash3(RSTHashAlgorithmBitMask bitMask)
        => Create(new RSTHashAlgorithmOptions(RSTHashAlgorithmFormat.XxHash3, bitMask));

    /// <summary>
    /// Creates a new instance of the XXHash64 hash algorithm with the specified bits mask configuration.
    /// </summary>
    /// <remarks>Use this method to obtain a hash algorithm compatible with XXHash64, optionally restricting
    /// the output to a subset of bits as defined by <paramref name="bitMask"/>. The returned algorithm can be used for
    /// hashing data streams or buffers where XXHash64 compatibility is required.</remarks>
    /// <param name="bitMask">A mask that specifies which bits of the hash output to use. The mask determines the format and truncation of the
    /// resulting hash value.</param>
    /// <returns>An instance of <see cref="RSTHashAlgorithm"/> configured to compute XXHash64 hashes according to the specified
    /// bits mask.</returns>
    public static RSTHashAlgorithm CreateXxHash64(RSTHashAlgorithmBitMask bitMask)
        => Create(new RSTHashAlgorithmOptions(RSTHashAlgorithmFormat.XxHash64, bitMask));

    /// <summary>
    /// Gets the bitmask configuration used for hash operations.
    /// </summary>
    public RSTHashAlgorithmBitMask BitMask => Options.BitMask;

    /// <summary>
    /// Gets the configuration options for the rst hash algorithm.
    /// </summary>
    public RSTHashAlgorithmOptions Options => field;

    /// <summary>
    /// Initializes a new instance of the RSTHashAlgorithm class with the specified options.
    /// </summary>
    /// <param name="options">The configuration options to use for the hash algorithm. Cannot be null.</param>
    internal RSTHashAlgorithm(RSTHashAlgorithmOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Computes the hash value for the provided string using the default encoding and culture.
    /// This method is part of the <see cref="RSTHashAlgorithm"/> interface implementation.
    /// </summary>
    /// <param name="toHash">The string to be hashed.</param>
    /// <returns>The computed hash value as an unsigned 64-bit integer.</returns>
    public virtual ulong Hash(ReadOnlySpan<char> toHash)
    {
        if (toHash.Length == 0)
        {
            return Hash(default(ReadOnlySpan<byte>));
        }

        Span<char> destination = stackalloc char[toHash.Length];
        {
            var written = toHash.ToLowerInvariant(destination);
            Debug.Assert(written == toHash.Length);
        }


        return Hash(Encoding.UTF8.GetBytes(destination.ToArray()));
    }

    /// <summary>
    /// Computes the rst hash of the provided data without any encoding or culture consideration,
    /// directly operating on the byte span.
    /// </summary>
    /// <param name="source">The data as a read-only byte span to hash.</param>
    /// <returns>The computed rst hash value as an unsigned 64-bit integer.</returns>
    public abstract ulong Hash(ReadOnlySpan<byte> source);

    /// <summary>
    /// Computes the rst hash of the string with an additional offset value.
    /// This method extends the basic hashing functionality by incorporating an offset
    /// into the hash calculation process.
    /// </summary>
    /// <param name="toHash">The string to hash.</param>
    /// <param name="offset">The offset value to be used in the hash computation, modifying the result.</param>
    /// <returns>The computed rst hash value adjusted by the specified offset.</returns>
    public virtual ulong HashWithOffset(ReadOnlySpan<char> toHash, long offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        {
            return WithOffset(Hash(toHash), offset);
        }
    }

    /// <summary>
    /// Computes the rst hash of the bytes with an additional offset value.
    /// </summary>
    /// <param name="source">The bytes to be hashed.</param>
    /// <param name="offset">The offset value to incorporate into the hash computation.</param>
    /// <returns>The computed hash value with offset applied, as an unsigned 64-bit integer.</returns>
    public virtual ulong HashWithOffset(ReadOnlySpan<byte> source, long offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        {
            return WithOffset(Hash(source), offset);
        }
    }

    /// <summary>
    /// Calculates a new rst hash value by incorporating an offset into the original hash.
    /// </summary>
    /// <param name="hash">The original rst hash value to which the offset will be applied.</param>
    /// <param name="offset">The offset to be added to the hash value, modifying it based on the current bits mask type.</param>
    /// <returns>The modified rst hash value including the specified offset.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ulong WithOffset(ulong hash, long offset) => hash + ((ulong)offset << BitMask.Bits);
}

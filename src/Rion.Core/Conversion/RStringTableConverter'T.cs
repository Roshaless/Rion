// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

namespace Rion.Core.Conversion;

/// <summary>
/// Provides a base class for converting between different string table formats.
/// </summary>
/// <typeparam name="TSource">The type of the source string table.</typeparam>
public abstract class RStringTableConverter<TSource> : RStringTableConverter where TSource : IRStringTable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTableConverter{TSource}"/> class.
    /// </summary>
    public RStringTableConverter() : base(typeof(TSource), typeof(TSource)) { }

    /// <summary>
    /// Converts the specified byte array to a string table.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The converted string table.</returns>
    public abstract TSource Convert(ReadOnlySpan<byte> bytes);

    /// <summary>
    /// Writes the specified string table to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The string table to write.</param>
    public abstract void Write(Stream stream, TSource value);

    /// <summary>
    /// Converts the specified byte array to a string table.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The converted string table.</returns>
    internal override object ConvertCore(ReadOnlySpan<byte> bytes) => Convert(bytes);

    /// <summary>
    /// Writes the specified string table to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The string table to write.</param>
    internal override void WriteCore(Stream stream, object value) => Write(stream, (TSource)value);
}

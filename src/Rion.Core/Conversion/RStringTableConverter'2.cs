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
/// <typeparam name="TConvert">The type of the converted string table.</typeparam>
/// <typeparam name="TWrite">The type of the written string table.</typeparam>
public abstract class RStringTableConverter<TConvert, TWrite>() :
    RStringTableConverter(typeof(TConvert), typeof(TWrite))
    where TConvert : IRStringTable where TWrite : IRStringTable
{
    /// <summary>
    /// Converts the specified byte array to a string table.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The converted string table.</returns>
    public abstract TConvert Convert(ReadOnlySpan<byte> bytes);

    /// <summary>
    /// Writes the specified string table to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The string table to write.</param>
    public abstract void Write(Stream stream, TWrite value);

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
    internal override void WriteCore(Stream stream, object value) => Write(stream, (TWrite)value);
}

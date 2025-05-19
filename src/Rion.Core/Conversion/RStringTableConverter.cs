// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

namespace Rion.Core.Conversion;

/// <summary>
/// Represents a base class for string table converters.
/// </summary>
public abstract partial class RStringTableConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTableConverter"/> class.
    /// </summary>
    /// <param name="conversionType">The type that the converter converts.</param>
    /// <param name="writeInputType">The type that the converter accepts when writing.</param>
    internal RStringTableConverter(Type conversionType, Type writeInputType)
    {
        ConversionType = conversionType;
        WriteInputType = writeInputType;
    }

    /// <summary>
    /// Gets the type that the converter returns when reading.
    /// </summary>
    protected Type ConversionType { get; }

    /// <summary>
    /// Gets the type that the converter accepts when writing.
    /// </summary>
    protected Type WriteInputType { get; }

    /// <summary>
    /// Determines whether the specified type can be converted by this converter.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> if the type can be converted; otherwise, <see langword="falsed"/>.</returns>
    public bool CanConvert(Type type) => type == ConversionType || type.IsAssignableTo(ConversionType);

    /// <summary>
    /// Determines whether the specified type can be written by this converter.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> if the type can be written; otherwise, <see langword="falsed"/>.</returns>
    public bool CanWrite(Type type) => type == WriteInputType || type.IsAssignableTo(WriteInputType);

    /// <summary>
    /// Converts the specified byte array to an object.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The converted object.</returns>
    internal abstract object ConvertCore(ReadOnlySpan<byte> bytes);

    /// <summary>
    /// Writes the specified object to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The object to write.</param>
    internal abstract void WriteCore(Stream stream, object value);
}

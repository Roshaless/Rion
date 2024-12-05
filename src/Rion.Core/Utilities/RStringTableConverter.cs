// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

namespace Rion.Core.Utilities;

/// <summary>
/// Represents a base class for string table converters.
/// </summary>
public abstract partial class RStringTableConverter
{
    /// <summary>
    /// The default json converter.
    /// </summary>
    public static RStringTableConverter<RStringTable, IRStringTable> JsonConverter { get; }

    /// <summary>
    /// Initializes static members of the <see cref="RStringTableConverter"/> class.
    /// </summary>
    static RStringTableConverter()
    {
        JsonConverter = new JsonConverterImpl();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTableConverter"/> class.
    /// </summary>
    /// <param name="readReturnType">The type that the converter returns when reading. </param>
    /// <param name="writeInputType">The type that the converter accepts when writing.</param>
    internal RStringTableConverter(Type readReturnType, Type writeInputType)
    {
        ReadReturnType = readReturnType;
        WriteInputType = writeInputType;
    }

    /// <summary>
    /// Gets the type that the converter returns when reading.
    /// </summary>
    public Type ReadReturnType { get; }

    /// <summary>
    /// Gets the type that the converter accepts when writing.
    /// </summary>
    public Type WriteInputType { get; }
}

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
/// <typeparam name="TRead">The type of the string table to read.</typeparam>
/// <typeparam name="TWrite">The type of the string table to write.</typeparam>
public abstract class RStringTableConverter<TRead, TWrite>() :
    RStringTableConverter(typeof(TRead), typeof(TWrite)),
    IRStringTableConvertReader<TRead>, IRStringTableConvertWriter<TWrite>
    where TRead : IRStringTable where TWrite : IRStringTable
{
    /// <inheritdoc />
    public abstract TRead? Read(ReadOnlySpan<byte> bytes);

    /// <inheritdoc />
    public abstract void Write(Stream stream, TWrite value);
}

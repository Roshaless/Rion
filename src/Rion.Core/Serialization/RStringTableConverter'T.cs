// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

namespace Rion.Core.Serialization;

/// <summary>
/// Provides a base class for converting objects of type <typeparamref name="T"/> to and from a serialized format.
/// </summary>
/// <remarks>
/// This abstract class defines methods for serializing and deserializing objects of type <typeparamref
/// name="T"/>. Derived classes must implement the <see cref="Deserialize(ReadOnlySpan{byte})"/> and <see
/// cref="Serialize(Stream, T)"/> methods to handle the specific serialization logic for the type.
/// </remarks>
/// <typeparam name="T">The type of the string table that implements <see cref="IRStringTable"/>.</typeparam>
public abstract class RStringTableConverter<T> : RStringTableConverter where T : IRStringTable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTableConverter"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor sets up the base type conversion for the <see cref="RStringTableConverter"/>
    /// by specifying the source and target types as the same type.
    /// </remarks>
    public RStringTableConverter() : base(typeof(T), typeof(T)) { }

    /// <summary>
    /// Deserializes the specified byte span into an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="bytes">A read-only span of bytes representing the serialized data.</param>
    /// <returns>An instance of type <typeparamref name="T"/> deserialized from the provided byte span.</returns>
    public abstract T Deserialize(ReadOnlySpan<byte> bytes);

    /// <summary>
    /// Serializes the specified string table into the provided stream.
    /// </summary>
    /// <remarks>This method writes the serialized representation of the string table to the provided stream. Ensure
    /// the stream is properly disposed of after serialization to avoid resource leaks.</remarks>
    /// <param name="stream">The stream to which the string table will be serialized. Must be writable and not null.</param>
    /// <param name="stringTable">The string table to serialize. Cannot be null.</param>
    public abstract void Serialize(Stream stream, T stringTable);

    /// <inheritdoc/>
    internal override object DeserializeCore(ReadOnlySpan<byte> bytes) => Deserialize(bytes);

    /// <inheritdoc/>
    internal override void SerializeCore(Stream stream, object stringTable) => Serialize(stream, (T)stringTable);
}

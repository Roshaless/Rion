// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

namespace Rion.Core.Serialization;

/// <summary>
/// Provides an abstract base class for converting between serialized and deserialized representations of string tables.
/// </summary>
/// <remarks>
/// This class defines the contract for serialization and deserialization of string tables, where the
/// deserialized and serialized representations are strongly typed. Derived classes must implement the <see
/// cref="Deserialize"/> and <see cref="Serialize"/> methods to provide specific conversion logic.
/// </remarks>
/// <typeparam name="TDeserialized">The type representing the deserialized string table. Must implement <see cref="IRStringTable"/>.</typeparam>
/// <typeparam name="TSerialized">The type representing the serialized string table. Must implement <see cref="IRStringTable"/>.</typeparam>
public abstract class RStringTableConverter<TDeserialized, TSerialized> : RStringTableConverter where TDeserialized : IRStringTable where TSerialized : IRStringTable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RStringTableConverter"/> class, specifying the types to be
    /// converted.
    /// </summary>
    /// <remarks>
    /// This constructor sets up the base converter with the source type <typeparamref name="TDeserialized"/> 
    /// and the target type <typeparamref name="TSerialized"/>. It is typically used to handle
    /// conversions between these two types in serialization or deserialization scenarios.
    /// </remarks>
    public RStringTableConverter() : base(typeof(TDeserialized), typeof(TSerialized)) { }

    /// <summary>
    /// Deserializes the specified byte span into an instance of the target type.
    /// </summary>
    /// <remarks>
    /// The method converts the provided byte span into an object of the target type.
    /// The caller must ensure that the byte span represents a valid serialized object.
    /// </remarks>
    /// <param name="bytes">A read-only span of bytes representing the serialized data to be deserialized.</param>
    /// <returns>An instance of the deserialized object of type <typeparamref name="TDeserialized"/>.</returns>
    public abstract TDeserialized Deserialize(ReadOnlySpan<byte> bytes);

    /// <summary>
    /// Serializes the specified string table to the provided stream.
    /// </summary>
    /// <remarks>
    /// This method writes the serialized representation of the string table to the specified stream.
    /// Ensure the stream is writable and has sufficient capacity for the serialized data.
    /// </remarks>
    /// <param name="stream">The stream to which the string table will be serialized. Must not be <see langword="null"/>.</param>
    /// <param name="stringTable">The string table to serialize. Must not be <see langword="null"/>.</param>
    public abstract void Serialize(Stream stream, TSerialized stringTable);

    /// <inheritdoc/>
    internal override object DeserializeCore(ReadOnlySpan<byte> bytes) => Deserialize(bytes);

    /// <inheritdoc/>
    internal override void SerializeCore(Stream stream, object stringTable) => Serialize(stream, (TSerialized)stringTable);
}

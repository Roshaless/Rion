// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.IO;

namespace Rion.Core.Serialization;

/// <summary>
/// Provides a base class for converting between serialized and deserialized representations of objects.
/// </summary>
/// <remarks>
/// This class defines the core functionality for determining whether a type can be serialized or
/// deserialized and provides abstract methods for performing the actual serialization and deserialization operations.
/// Derived classes must implement the <see cref="DeserializeCore"/> and <see cref="SerializeCore"/> methods to handle
/// the specific serialization format.
/// </remarks>
public abstract partial class StringTableConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringTableConverter"/> class, specifying the types used for
    /// deserialization and serialization.
    /// </summary>
    /// <param name="deserializedType">The type that will be used during deserialization.</param>
    /// <param name="serializedType">The type that will be used during serialization.</param>
    internal StringTableConverter(Type deserializedType, Type serializedType)
    {
        _deserializedType = deserializedType;
        _serializedType = serializedType;
    }

    /// <summary>
    /// Represents the type of the object that has been deserialized.
    /// </summary>
    internal Type _deserializedType;

    /// <summary>
    /// Represents the type of the object being serialized or deserialized.
    /// </summary>
    internal Type _serializedType;

    /// <summary>
    /// Gets the type of the object that is deserialized by this instance.
    /// </summary>
    protected Type DeserializedType => _deserializedType;

    /// <summary>
    /// Gets the <see cref="Type"/> that represents the serialized form of the object.
    /// </summary>
    protected Type SerializedType => _serializedType;

    /// <summary>
    /// Determines whether the specified type can be deserialized by this instance.
    /// </summary>
    /// <param name="type">The type to check for deserialization compatibility.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <paramref name="type"/> is the same
    /// as or assignable to the deserialized type; otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanDeserialize(Type type) => type == DeserializedType || type.IsAssignableTo(DeserializedType);

    /// <summary>
    /// Determines whether the specified type can be serialized by this serializer.
    /// </summary>
    /// <param name="type">The type to check for serialization compatibility.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <paramref name="type"/> is the same as or assignable to the type
    /// supported by this serializer; otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanSerialize(Type type) => type == SerializedType || type.IsAssignableTo(SerializedType);

    /// <summary>
    /// Deserializes the specified byte span into an string table.
    /// </summary>
    /// <remarks>
    /// This method is intended to be implemented by derived classes to provide specific deserialization logic.
    /// The caller must ensure that the input data is in the expected format for the implementation.
    /// </remarks>
    /// <param name="bytes">A read-only span of bytes representing the serialized data to be deserialized.</param>
    /// <returns>The deserialized object represented by the input byte span.</returns>
    internal abstract object DeserializeCore(ReadOnlySpan<byte> bytes);

    /// <summary>
    /// Serializes the specified string table to the provided stream.
    /// </summary>
    /// <remarks>This method performs the core serialization logic for the string table .  Implementations must
    /// ensure that the stream is written in a format compatible with the deserialization process.</remarks>
    /// <param name="stream">The stream to which the string table  will be serialized. Must not be <see langword="null"/>.</param>
    /// <param name="stringTable">The string table to serialize. Must not be <see langword="null"/>.</param>
    internal abstract void SerializeCore(Stream stream, object stringTable);
}

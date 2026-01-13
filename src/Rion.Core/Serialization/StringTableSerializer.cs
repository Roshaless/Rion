// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

using Rion.Core.Buffers;
using Rion.Core.Internal;

namespace Rion.Core.Serialization;

/// <summary>
/// Provides methods for serializing and deserializing string table objects to and from various formats.
/// </summary>
/// <remarks>
/// The <see cref="StringTableSerializer"/> class includes static methods for handling serialization and
/// deserialization of objects that implement the <see cref="IStringTable"/> interface. It supports operations on byte
/// spans, streams, and file paths, and relies on user-provided converters to perform the actual serialization or
/// deserialization logic.
/// </remarks>
public static partial class StringTableSerializer
{
    /// <summary>
    /// Deserializes a byte span into an object of the specified type using the provided converter.
    /// </summary>
    /// <typeparam name="TDeserialized">The type of the deserialized object. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <typeparam name="TSerialized">The intermediate serialized type. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <param name="bytes">The span of bytes representing the serialized data.</param>
    /// <param name="converter">The converter used to perform the deserialization. Cannot be <see langword="null"/>.</param>
    /// <returns>An instance of <typeparamref name="TDeserialized"/> created from the serialized data.</returns>
    public static TDeserialized Deserialize<TDeserialized, TSerialized>(
        ReadOnlySpan<byte> bytes,
        StringTableConverter<TDeserialized, TSerialized> converter
    )
        where TDeserialized : class, IStringTable
        where TSerialized : IStringTable
    {
        ArgumentNullException.ThrowIfNull(converter);

        return Unsafe.As<TDeserialized>(converter.DeserializeCore(bytes));
    }

    /// <summary>
    /// Attempts to deserialize a sequence of bytes into an instance of the specified type.
    /// </summary>
    /// <remarks>
    /// If the deserialization fails due to an exception or if the converter is unable to deserialize
    /// the specified type,  the method returns <see langword="false"/> and sets <paramref name="result"/>
    /// to <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="TDeserialized">The target type to deserialize into. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <typeparam name="TSerialized">The serialized type. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <param name="bytes">The sequence of bytes to deserialize.</param>
    /// <param name="converter">The converter used to perform the deserialization. Must not be <see langword="null"/> and must support
    /// deserialization of <typeparamref name="TDeserialized"/>.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the deserialized object of type <typeparamref
    /// name="TDeserialized"/>.  When this method returns <see langword="false"/>, contains <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the deserialization was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryDeserialize<TDeserialized, TSerialized>(
        ReadOnlySpan<byte> bytes,
        StringTableConverter<TDeserialized, TSerialized> converter,
        [NotNullWhen(true)] out TDeserialized? result
    )
        where TDeserialized : class, IStringTable
        where TSerialized : IStringTable
    {
        if (converter.IsNotNull() && converter.CanDeserialize(typeof(TDeserialized)))
        {
            try
            {
                result = converter.DeserializeCore(bytes) as TDeserialized;
                return result is not null;
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Failed to convert bytes to type {typeof(TDeserialized)}: {e}");
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Attempts to deserialize a file at the specified path into an object of type <typeparamref name="TDeserialized"/>.
    /// </summary>
    /// <remarks>
    /// This method will return <see langword="false"/> if the converter is <see langword="null"/>,
    /// if the converter does not support deserialization of <typeparamref name="TDeserialized"/>,
    /// or if an error occurs during deserialization.
    /// </remarks>
    /// <typeparam name="TDeserialized">The type of the object to deserialize. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <typeparam name="TSerialized">The type of the serialized representation. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <param name="path">The file path to the serialized data.</param>
    /// <param name="converter">The converter used to deserialize the data. Must not be <see langword="null"/> and must support deserialization
    /// of <typeparamref name="TDeserialized"/>.</param>
    /// <param name="result">When this method returns, contains the deserialized object of type <typeparamref name="TDeserialized"/> if the
    /// operation succeeds; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the file was successfully deserialized into an object of type <typeparamref
    /// name="TDeserialized"/>; otherwise, <see langword="false"/>.</returns>
    public static bool TryDeserialize<TDeserialized, TSerialized>(
        string path,
        StringTableConverter<TDeserialized, TSerialized> converter,
        [NotNullWhen(true)] out TDeserialized? result
    )
        where TDeserialized : class, IStringTable
        where TSerialized : IStringTable
    {
        if (converter.IsNotNull() && converter.CanDeserialize(typeof(TDeserialized)))
        {
            FileBufferScope? scope = null;
            try
            {
                scope = FileBufferScope.CreateFrom(path);
                result = converter.DeserializeCore(scope.Span) as TDeserialized;
                return result is not null;
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Failed to convert {path} to {typeof(TDeserialized)}: {e.Message}");
            }
            finally
            {
                scope?.Dispose();
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Serializes the specified string table to the provided output stream using the given converter.
    /// </summary>
    /// <remarks>
    /// This method ensures that all required arguments are non-null and that the provided converter
    /// is capable of serializing the specified string table type before delegating the serialization
    /// process to the converter.
    /// </remarks>
    /// <typeparam name="T">The type of the string table to serialize. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <param name="output">The stream to which the string table will be serialized. Must not be <see langword="null"/>.</param>
    /// <param name="stringTable">The string table to serialize. Must not be <see langword="null"/>.</param>
    /// <param name="converter">The converter used to perform the serialization. Must not be <see langword="null"/> and must support
    /// serialization of the specified string table type.</param>
    public static void Serialize<T>(
        Stream output,
        T stringTable,
        StringTableConverter converter
    ) where T : IStringTable
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(stringTable);
        ArgumentNullException.ThrowIfNull(converter);
        ThrowHelper.ThrowIfCanNotSerialize(converter, typeof(T));

        converter.SerializeCore(output, stringTable);
    }

    /// <summary>
    /// Serializes the specified string table to the given output file path using the provided converter.
    /// </summary>
    /// <remarks>
    /// This method creates or overwrites the file at the specified <paramref name="outputPath"/>. 
    /// Ensure the caller has appropriate permissions to write to the file system at the specified location.
    /// </remarks>
    /// <typeparam name="T">The type of the string table to serialize. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <param name="outputPath">The file path where the serialized data will be written. Cannot be null or empty.</param>
    /// <param name="stringTable">The string table to serialize. Cannot be null.</param>
    /// <param name="converter">The converter used to perform the serialization. Cannot be null.</param>
    public static void Serialize<T>(
        string outputPath,
        T stringTable,
        StringTableConverter converter
    ) where T : IStringTable
    {
        ArgumentException.ThrowIfNullOrEmpty(outputPath);
        ArgumentNullException.ThrowIfNull(stringTable);
        ArgumentNullException.ThrowIfNull(converter);
        ThrowHelper.ThrowIfCanNotSerialize(converter, typeof(T));

        using var stream = FileOperations.OpenOrCreate(outputPath);
        converter.SerializeCore(stream, stringTable);
    }

    /// <summary>
    /// Attempts to serialize the specified string table to the provided output stream using the given converter.
    /// </summary>
    /// <remarks>
    /// If the converter is <see langword="null"/> or does not support serialization of the specified
    /// type, the method will return <see langword="false"/>. If an exception occurs during serialization, the method
    /// will log a warning and return <see langword="false"/>.
    /// </remarks>
    /// <typeparam name="T">The type of the string table to serialize. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <param name="output">The <see cref="Stream"/> to which the string table will be serialized. Must be writable.</param>
    /// <param name="stringTable">The string table instance to serialize.</param>
    /// <param name="converter">The <see cref="StringTableConverter"/> used to perform the serialization. Must not be <see langword="null"/>
    /// and must support serialization of the specified type.</param>
    /// <returns><see langword="true"/> if the string table was successfully serialized; otherwise, <see langword="false"/>.</returns>
    public static bool TrySerialize<T>(
        Stream output,
        T stringTable,
        StringTableConverter converter
    ) where T : IStringTable
    {
        if (converter.IsNotNull() && converter.CanSerialize(typeof(T)))
        {
            try
            {
                converter.SerializeCore(output, stringTable);
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Failed to convert {typeof(T)} to stream: {e.Message}");
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to serialize the specified string table to the given output path using the provided converter.
    /// </summary>
    /// <remarks>
    /// If the serialization fails due to an exception (e.g., file access issues or an unsupported
    /// type), the method logs a warning and returns <see langword="false"/>. Ensure that the
    /// <paramref name="converter"/> is properly configured to handle the type of the string table being serialized.
    /// </remarks>
    /// <typeparam name="T">The type of the string table to serialize. Must implement <see cref="IStringTable"/>.</typeparam>
    /// <param name="outputPath">The file path where the serialized string table will be written. This path must be writable.</param>
    /// <param name="stringTable">The string table to serialize. Must not be <see langword="null"/>.</param>
    /// <param name="converter">The converter used to serialize the string table. Must not be <see langword="null"/> and must support
    /// serialization of the specified type.</param>
    /// <returns><see langword="true"/> if the string table was successfully serialized to the specified output path; otherwise,
    /// <see langword="false"/>.</returns>
    public static bool TrySerialize<T>(
        string outputPath,
        T stringTable,
        StringTableConverter converter
    ) where T : IStringTable
    {
        if (converter.IsNotNull() && converter.CanSerialize(typeof(T)))
        {
            FileStream? stream = null;
            try
            {
                stream = FileOperations.OpenOrCreate(outputPath);
                converter.SerializeCore(stream, stringTable);
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Failed to write to file {outputPath}: {e.Message}");
            }
            finally
            {
                stream?.Dispose();
            }
        }

        return false;
    }
}

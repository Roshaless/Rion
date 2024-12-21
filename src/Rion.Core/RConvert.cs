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
using Rion.Core.Conversion;
using Rion.Core.Internal;

namespace Rion.Core;

/// <summary>
/// Contains methods for converting between different string table formats.
/// </summary>
public static partial class RConvert
{
    /// <summary>
    /// Reads a string table from a byte array using a specified converter.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <param name="converter">The converter to use for converting the byte array.</param>
    /// <typeparam name="T">The type of the string table to convert.</typeparam>
    /// <returns>The string table converted from the byte array, or <see langword="null"/> if an error occurs.</returns>
    public static T From<T>(ReadOnlySpan<byte> bytes, RStringTableConverter converter) where T : class, IRStringTable
    {
        ArgumentNullException.ThrowIfNull(converter);

        return Unsafe.As<T>(converter.ConvertCore(bytes));
    }

    /// <summary>
    /// Reads a string table from a file using a specified converter.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <param name="converter">The converter to use for reading the string table.</param>
    /// <typeparam name="T">The type of the string table to read.</typeparam>
    /// <returns>The string table read from the file, or null if an error occurs.</returns>
    public static T FromFile<T>(string path, RStringTableConverter converter) where T : class, IRStringTable
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentNullException.ThrowIfNull(converter);

        using var scope = RFileBufferScope.CreateFrom(path);
        return Unsafe.As<T>(converter.ConvertCore(scope.Span));
    }

    /// <summary>
    /// Writes a string table to a stream using a specified converter.
    /// </summary>
    /// <param name="output">The stream to write the string table to.</param>
    /// <param name="value">The string table to write to the stream.</param>
    /// <param name="converter">The converter to use for writing the string table.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    public static void To<T>(Stream output, T value, RStringTableConverter converter) where T : IRStringTable
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(converter);
        {
            converter.WriteCore(output, value);
        }
    }

    /// <summary>
    /// Writes a string table to a file using a specified converter.
    /// </summary>
    /// <param name="outputPath">The path of the output file.</param>
    /// <param name="value">The string table to write to the file.</param>
    /// <param name="converter">The converter to use for writing the string table.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    public static void ToFile<T>(string outputPath, T value, RStringTableConverter converter) where T : IRStringTable
    {
        ArgumentException.ThrowIfNullOrEmpty(outputPath);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(converter);

        using var stream = RFile.OpenOrCreate(outputPath);
        {
            converter.WriteCore(stream, value);
        }
    }
    /// <summary>
    /// Tries to read a string table from a byte array using a specified converter.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <param name="converter">The converter to use for converting the byte array.</param>
    /// <param name="result">The string table converted from the byte array, or <see langword="null"/> if an error occurs.</param>
    /// <typeparam name="T">The type of the string table to convert.</typeparam>
    /// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryFrom<T>(ReadOnlySpan<byte> bytes, RStringTableConverter converter, [NotNullWhen(true)] out T? result) where T : class, IRStringTable
    {
        if (converter.IsNotNull() && converter.CanConvert(typeof(T)))
        {
            try
            {
                result = converter.ConvertCore(bytes) as T;
                return result is not null;
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Failed to convert bytes to type {typeof(T)}: {e}");
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Tries to read a string table from a file using a specified converter.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <param name="converter">The converter to use for reading the string table.</param>
    /// <param name="result">The string table converted from the input file, or <see langword="null"/> if an error occurs.</param>
    /// <typeparam name="T">The type of the string table to read.</typeparam>
    /// <returns><see langword="true"/> if the conversion succeeded, <see langword="false"/> otherwise.</returns>
    public static bool TryFromFile<T>(string path, RStringTableConverter converter, [NotNullWhen(true)] out T? result) where T : class, IRStringTable
    {
        if (converter.IsNotNull() && converter.CanConvert(typeof(T)))
        {
            RFileBufferScope? scope = null;
            try
            {
                scope = RFileBufferScope.CreateFrom(path);
                result = converter.ConvertCore(scope.Span) as T;
                return result is not null;
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Failed to convert {path} to {typeof(T)}: {e.Message}");
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
    /// Tries to write a string table to a stream using a specified converter.
    /// </summary>
    /// <param name="output">The stream to write the string table to.</param>
    /// <param name="value">The string table to write to the stream.</param>
    /// <param name="converter">The converter to use for writing the string table.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    /// <returns><see langword="true"/> if the value was written successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryTo<T>(Stream output, T value, RStringTableConverter converter) where T : IRStringTable
    {
        if (converter.IsNotNull() && converter.CanWrite(typeof(T)))
        {
            try
            {
                converter.WriteCore(output, value);
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
    /// Tries to write a string table to a file using a specified converter.
    /// </summary>
    /// <param name="outputPath">The path of the output file.</param>
    /// <param name="value">The string table to write to the file.</param>
    /// <param name="converter">The converter to use for writing the string table.</param>
    /// <typeparam name="T">The type of the string table to write.</typeparam>
    /// <returns><see langword="true"/> if the operation was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryToFile<T>(string outputPath, T value, RStringTableConverter converter) where T : IRStringTable
    {
        if (converter.IsNotNull() && converter.CanWrite(typeof(T)))
        {
            FileStream? stream = null;
            try
            {
                stream = RFile.OpenOrCreate(outputPath);
                converter.WriteCore(stream, value);
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

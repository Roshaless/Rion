// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics;
using System.Text;

namespace Rion.Core.Internal;

/// <summary>
/// Provides utility methods for working with strings, including creation from byte arrays and spans.
/// </summary>
internal static class StringHelper
{
    /// <summary>
    /// Static read-only field representing the default encoding used for string operations within the StringHelper class.
    /// </summary>
    private static readonly Encoding s_encoding = Encoding.UTF8;

    /// <summary>
    /// Creates a string from a byte pointer and a specified length.
    /// </summary>
    /// <param name="bytes">Pointer to the byte array containing the string data.</param>
    /// <param name="length">The number of bytes to convert into a string.</param>
    /// <returns>The string representation of the byte data.</returns>
    public static unsafe string CreateStringFromBytes(byte* bytes, int length)
    {
        var stringLength = s_encoding.GetCharCount(bytes, length);
        Debug.Assert(stringLength >= 0, "stringLength is less than 0!");

        var result = new string(char.MinValue, stringLength);
        fixed (char* chars = &result.GetRawStringData())
        {
            s_encoding.GetChars(bytes, length, chars, stringLength);
        }

        return result;
    }

    /// <summary>
    /// Creates a string from a read-only span of bytes.
    /// </summary>
    /// <param name="data">The span of bytes to convert into a string.</param>
    /// <returns>The string representation of the byte data.</returns>
    public static string CreateStringFromBytes(ReadOnlySpan<byte> data)
    {
        unsafe
        {
            fixed (byte* bytes = &data.AsReference())
            {
                return CreateStringFromBytes(bytes, data.Length);
            }
        }
    }
}

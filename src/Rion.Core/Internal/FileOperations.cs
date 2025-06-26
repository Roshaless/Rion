// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.IO;
using System.Runtime.CompilerServices;

namespace Rion.Core.Internal;

/// <summary>
/// Provides utility methods for performing file operations.
/// </summary>
internal static class FileOperations
{
    /// <summary>
    /// Opens an existing file or creates a new file at the specified path with write access.
    /// </summary>
    /// <remarks>
    /// The file is opened in <see cref="FileMode.OpenOrCreate"/>, which means that if the file
    /// exists, it is opened; otherwise, a new file is created. The file is opened with write access (<see cref="FileAccess.Write"/>).
    /// </remarks>
    /// <param name="path">The path of the file to open or create. This cannot be null or empty.</param>
    /// <returns>A <see cref="FileStream"/> object that provides write access to the file.  The file is opened with <see
    /// cref="FileShare.Read"/> to allow other processes to read the file while it is open.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static FileStream OpenOrCreate(string path) => new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
}

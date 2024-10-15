// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Runtime.CompilerServices;

namespace Rion.Core.Internal;

/// <summary>
/// Nothing...
/// </summary>
internal static class Nothing
{
    /// <summary>
    /// Nothing Todo...
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToDo() { }

    /// <summary>
    /// Nothing Todo...
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ToDo<T>() where T : class => null!;
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

namespace CommunityToolkit.Common.Helpers;

/// <summary>
/// Represents the types of items available in a directory.
/// </summary>
public enum DirectoryItemType
{
    /// <summary>
    /// The item is neither a file or a folder.
    /// </summary>
    None,

    /// <summary>
    /// Represents a file type item.
    /// </summary>
    File,

    /// <summary>
    /// Represents a folder type item.
    /// </summary>
    Folder
}

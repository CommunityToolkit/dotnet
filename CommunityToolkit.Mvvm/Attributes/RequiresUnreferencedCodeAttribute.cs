// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET6_0_OR_GREATER

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates that the specified method requires dynamic access to code that is not referenced statically.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
[Conditional("DEBUG")]
internal sealed class RequiresUnreferencedCodeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiresUnreferencedCodeAttribute"/> class.
    /// </summary>
    /// <param name="message">A message that contains information about the usage of unreferenced code.</param>
    public RequiresUnreferencedCodeAttribute(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Gets a message that contains information about the usage of unreferenced code.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets or sets an optional URL that contains more information about the method,
    /// why it requires unreferenced code, and what options a consumer has to deal with it.
    /// </summary>
    public string? Url { get; set; }
}

#endif
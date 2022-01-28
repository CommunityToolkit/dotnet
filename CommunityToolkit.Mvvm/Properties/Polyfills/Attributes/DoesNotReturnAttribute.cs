// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

#if NETSTANDARD2_0

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Applied to a method that will never return under any circumstance.
/// </summary>
/// <remarks>Internal copy from the BCL attribute.</remarks>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute
{
}

#endif
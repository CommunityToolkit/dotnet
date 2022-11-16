// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Runtime.CompilerServices;
#if NETSTANDARD
using System.Runtime.InteropServices;
#endif
using CommunityToolkit.HighPerformance.Enumerables;
using CommunityToolkit.HighPerformance.Helpers.Internals;

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// Helpers for working with the <see cref="string"/> type.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Returns a reference to the first element within a given <see cref="string"/>, with no bounds checks.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance.</param>
    /// <returns>A reference to the first element within <paramref name="text"/>, or the location it would have used, if <paramref name="text"/> is empty.</returns>
    /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to perform checks in case the returned value is dereferenced.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char DangerousGetReference(this string text)
    {
#if NET6_0_OR_GREATER
        return ref Unsafe.AsRef(text.GetPinnableReference());
#else
        return ref MemoryMarshal.GetReference(text.AsSpan());
#endif
    }

    /// <summary>
    /// Returns a reference to an element at a specified index within a given <see cref="string"/>, with no bounds checks.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance.</param>
    /// <param name="i">The index of the element to retrieve within <paramref name="text"/>.</param>
    /// <returns>A reference to the element within <paramref name="text"/> at the index specified by <paramref name="i"/>.</returns>
    /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to ensure the <paramref name="i"/> parameter is valid.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char DangerousGetReferenceAt(this string text, int i)
    {
#if NET6_0_OR_GREATER
        ref char r0 = ref Unsafe.AsRef(text.GetPinnableReference());
#else
        ref char r0 = ref MemoryMarshal.GetReference(text.AsSpan());
#endif
        ref char ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

        return ref ri;
    }

    /// <summary>
    /// Counts the number of occurrences of a given character into a target <see cref="string"/> instance.
    /// </summary>
    /// <param name="text">The input <see cref="string"/> instance to read.</param>
    /// <param name="c">The character to look for.</param>
    /// <returns>The number of occurrences of <paramref name="c"/> in <paramref name="text"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count(this string text, char c)
    {
        ref char r0 = ref text.DangerousGetReference();
        nint length = (nint)(uint)text.Length;

        return (int)SpanHelper.Count(ref r0, length, c);
    }

    /// <summary>
    /// Enumerates the items in the input <see cref="string"/> instance, as pairs of value/index values.
    /// This extension should be used directly within a <see langword="foreach"/> loop:
    /// <code>
    /// string text = "Hello, world!";
    ///
    /// foreach (var item in text.Enumerate())
    /// {
    ///     // Access the index and value of each item here...
    ///     int index = item.Index;
    ///     char value = item.Value;
    /// }
    /// </code>
    /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> to enumerate.</param>
    /// <returns>A wrapper type that will handle the value/index enumeration for <paramref name="text"/>.</returns>
    /// <remarks>The returned <see cref="ReadOnlySpanEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpanEnumerable<char> Enumerate(this string text)
    {
        return new(text.AsSpan());
    }

    /// <summary>
    /// Tokenizes the values in the input <see cref="string"/> instance using a specified separator.
    /// This extension should be used directly within a <see langword="foreach"/> loop:
    /// <code>
    /// string text = "Hello, world!";
    ///
    /// foreach (var token in text.Tokenize(','))
    /// {
    ///     // Access the tokens here...
    /// }
    /// </code>
    /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> to tokenize.</param>
    /// <param name="separator">The separator character to use.</param>
    /// <returns>A wrapper type that will handle the tokenization for <paramref name="text"/>.</returns>
    /// <remarks>The returned <see cref="ReadOnlySpanTokenizer{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpanTokenizer<char> Tokenize(this string text, char separator)
    {
        return new(text.AsSpan(), separator);
    }

    /// <summary>
    /// Gets a content hash from the input <see cref="string"/> instance using the Djb2 algorithm.
    /// For more info, see the documentation for <see cref="ReadOnlySpanExtensions.GetDjb2HashCode{T}"/>.
    /// </summary>
    /// <param name="text">The source <see cref="string"/> to enumerate.</param>
    /// <returns>The Djb2 value for the input <see cref="string"/> instance.</returns>
    /// <remarks>The Djb2 hash is fully deterministic and with no random components.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int GetDjb2HashCode(this string text)
    {
        ref char r0 = ref text.DangerousGetReference();
        nint length = (nint)(uint)text.Length;

        return SpanHelper.GetDjb2HashCode(ref r0, length);
    }
}

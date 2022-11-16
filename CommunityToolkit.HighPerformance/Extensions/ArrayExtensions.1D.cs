// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Runtime.CompilerServices;
#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
using CommunityToolkit.HighPerformance.Enumerables;
#if NETSTANDARD
using CommunityToolkit.HighPerformance.Helpers;
#endif
using CommunityToolkit.HighPerformance.Helpers.Internals;
using RuntimeHelpers = CommunityToolkit.HighPerformance.Helpers.Internals.RuntimeHelpers;

namespace CommunityToolkit.HighPerformance;

/// <summary>
/// Helpers for working with the <see cref="Array"/> type.
/// </summary>
public static partial class ArrayExtensions
{
    /// <summary>
    /// Returns a reference to the first element within a given <typeparamref name="T"/> array, with no bounds checks.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input <typeparamref name="T"/> array instance.</typeparam>
    /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
    /// <returns>A reference to the first element within <paramref name="array"/>, or the location it would have used, if <paramref name="array"/> is empty.</returns>
    /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to perform checks in case the returned value is dereferenced.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T DangerousGetReference<T>(this T[] array)
    {
#if NET6_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference(array);
#else
        IntPtr offset = RuntimeHelpers.GetArrayDataByteOffset<T>();

        return ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
#endif
    }

    /// <summary>
    /// Returns a reference to an element at a specified index within a given <typeparamref name="T"/> array, with no bounds checks.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input <typeparamref name="T"/> array instance.</typeparam>
    /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
    /// <param name="i">The index of the element to retrieve within <paramref name="array"/>.</param>
    /// <returns>A reference to the element within <paramref name="array"/> at the index specified by <paramref name="i"/>.</returns>
    /// <remarks>This method doesn't do any bounds checks, therefore it is responsibility of the caller to ensure the <paramref name="i"/> parameter is valid.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T DangerousGetReferenceAt<T>(this T[] array, int i)
    {
#if NET6_0_OR_GREATER
        ref T r0 = ref MemoryMarshal.GetArrayDataReference(array);
        ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

        return ref ri;
#else
        IntPtr offset = RuntimeHelpers.GetArrayDataByteOffset<T>();
        ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, offset);
        ref T ri = ref Unsafe.Add(ref r0, (nint)(uint)i);

        return ref ri;
#endif
    }

    /// <summary>
    /// Counts the number of occurrences of a given value into a target <typeparamref name="T"/> array instance.
    /// </summary>
    /// <typeparam name="T">The type of items in the input <typeparamref name="T"/> array instance.</typeparam>
    /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
    /// <param name="value">The <typeparamref name="T"/> value to look for.</param>
    /// <returns>The number of occurrences of <paramref name="value"/> in <paramref name="array"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<T>(this T[] array, T value)
        where T : IEquatable<T>
    {
        ref T r0 = ref array.DangerousGetReference();
        nint length = RuntimeHelpers.GetArrayNativeLength(array);
        nint count = SpanHelper.Count(ref r0, length, value);

        if ((nuint)count > int.MaxValue)
        {
            ThrowOverflowException();
        }

        return (int)count;
    }

    /// <summary>
    /// Enumerates the items in the input <typeparamref name="T"/> array instance, as pairs of reference/index values.
    /// This extension should be used directly within a <see langword="foreach"/> loop:
    /// <code>
    /// int[] numbers = new[] { 1, 2, 3, 4, 5, 6, 7 };
    ///
    /// foreach (var item in numbers.Enumerate())
    /// {
    ///     // Access the index and value of each item here...
    ///     int index = item.Index;
    ///     ref int value = ref item.Value;
    /// }
    /// </code>
    /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
    /// </summary>
    /// <typeparam name="T">The type of items to enumerate.</typeparam>
    /// <param name="array">The source <typeparamref name="T"/> array to enumerate.</param>
    /// <returns>A wrapper type that will handle the reference/index enumeration for <paramref name="array"/>.</returns>
    /// <remarks>The returned <see cref="SpanEnumerable{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanEnumerable<T> Enumerate<T>(this T[] array)
    {
        return new(array);
    }

    /// <summary>
    /// Tokenizes the values in the input <typeparamref name="T"/> array instance using a specified separator.
    /// This extension should be used directly within a <see langword="foreach"/> loop:
    /// <code>
    /// char[] text = "Hello, world!".ToCharArray();
    ///
    /// foreach (var token in text.Tokenize(','))
    /// {
    ///     // Access the tokens here...
    /// }
    /// </code>
    /// The compiler will take care of properly setting up the <see langword="foreach"/> loop with the type returned from this method.
    /// </summary>
    /// <typeparam name="T">The type of items in the <typeparamref name="T"/> array to tokenize.</typeparam>
    /// <param name="array">The source <typeparamref name="T"/> array to tokenize.</param>
    /// <param name="separator">The separator <typeparamref name="T"/> item to use.</param>
    /// <returns>A wrapper type that will handle the tokenization for <paramref name="array"/>.</returns>
    /// <remarks>The returned <see cref="SpanTokenizer{T}"/> value shouldn't be used directly: use this extension in a <see langword="foreach"/> loop.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanTokenizer<T> Tokenize<T>(this T[] array, T separator)
        where T : IEquatable<T>
    {
        return new(array, separator);
    }

    /// <summary>
    /// Gets a content hash from the input <typeparamref name="T"/> array instance using the Djb2 algorithm.
    /// For more info, see the documentation for <see cref="ReadOnlySpanExtensions.GetDjb2HashCode{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the input <typeparamref name="T"/> array instance.</typeparam>
    /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
    /// <returns>The Djb2 value for the input <typeparamref name="T"/> array instance.</returns>
    /// <remarks>The Djb2 hash is fully deterministic and with no random components.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetDjb2HashCode<T>(this T[] array)
        where T : notnull
    {
        ref T r0 = ref array.DangerousGetReference();
        nint length = RuntimeHelpers.GetArrayNativeLength(array);

        return SpanHelper.GetDjb2HashCode(ref r0, length);
    }

    /// <summary>
    /// Checks whether or not a given <typeparamref name="T"/> array is covariant.
    /// </summary>
    /// <typeparam name="T">The type of items in the input <typeparamref name="T"/> array instance.</typeparam>
    /// <param name="array">The input <typeparamref name="T"/> array instance.</param>
    /// <returns>Whether or not <paramref name="array"/> is covariant.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCovariant<T>(this T[] array)
    {
        return default(T) is null && array.GetType() != typeof(T[]);
    }

    /// <summary>
    /// Throws an <see cref="OverflowException"/> when the "column" parameter is invalid.
    /// </summary>
    private static void ThrowOverflowException()
    {
        throw new OverflowException();
    }
}

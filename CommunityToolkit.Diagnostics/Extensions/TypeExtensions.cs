// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Diagnostics;

/// <summary>
/// Helpers for working with types.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// The mapping of built-in types to their simple representation.
    /// </summary>
    private static readonly IReadOnlyDictionary<Type, string> BuiltInTypesMap = new Dictionary<Type, string>
    {
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(char)] = "char",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(float)] = "float",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(object)] = "object",
        [typeof(string)] = "string",
        [typeof(void)] = "void"
    };

    /// <summary>
    /// A thread-safe mapping of precomputed string representation of types.
    /// </summary>
    private static readonly ConditionalWeakTable<Type, string> DisplayNames = new();

    /// <summary>
    /// Returns a simple string representation of a type.
    /// </summary>
    /// <param name="type">The input type.</param>
    /// <returns>The string representation of <paramref name="type"/>.</returns>
    public static string ToTypeString(this Type type)
    {
        // Local function to create the formatted string for a given type
        static string FormatDisplayString(Type type, int genericTypeOffset, ReadOnlySpan<Type> typeArguments)
        {
            // Primitive types use the keyword name
            if (BuiltInTypesMap.TryGetValue(type, out string? typeName))
            {
                return typeName!;
            }

            // Array types are displayed as Foo[]
            if (type.IsArray)
            {
                Type? elementType = type.GetElementType()!;
                int rank = type.GetArrayRank();

                return $"{FormatDisplayString(elementType, 0, elementType.GetGenericArguments())}[{new string(',', rank - 1)}]";
            }

            // By checking generic types here we are only interested in specific cases,
            // ie. nullable value types or value tuples. We have a separate path for custom
            // generic types, as we can't rely on this API in that case, as it doesn't show
            // a difference between nested types that are themselves generic, or nested simple
            // types from a generic declaring type. To deal with that, we need to manually track
            // the offset within the array of generic arguments for the whole constructed type.
            if (type.IsGenericType)
            {
                Type? genericTypeDefinition = type.GetGenericTypeDefinition();

                // Nullable<T> types are displayed as T?
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    Type[]? nullableArguments = type.GetGenericArguments();

                    return $"{FormatDisplayString(nullableArguments[0], 0, nullableArguments)}?";
                }

                // ValueTuple<T1, T2> types are displayed as (T1, T2)
                if (genericTypeDefinition == typeof(ValueTuple<>) ||
                    genericTypeDefinition == typeof(ValueTuple<,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,,,,>) ||
                    genericTypeDefinition == typeof(ValueTuple<,,,,,,,>))
                {
                    IEnumerable<string>? formattedTypes = type.GetGenericArguments().Select(t => FormatDisplayString(t, 0, t.GetGenericArguments()));

                    return $"({string.Join(", ", formattedTypes)})";
                }
            }

            string displayName;

            // Generic types
            if (type.Name.Contains('`'))
            {
                // Retrieve the current generic arguments for the current type (leaf or not)
                string[]? tokens = type.Name.Split('`');
                int genericArgumentsCount = int.Parse(tokens[1]);
                int typeArgumentsOffset = typeArguments.Length - genericTypeOffset - genericArgumentsCount;
                Type[]? currentTypeArguments = typeArguments.Slice(typeArgumentsOffset, genericArgumentsCount).ToArray();
                IEnumerable<string>? formattedTypes = currentTypeArguments.Select(t => FormatDisplayString(t, 0, t.GetGenericArguments()));

                // Standard generic types are displayed as Foo<T>
                displayName = $"{tokens[0]}<{string.Join(", ", formattedTypes)}>";

                // Track the current offset for the shared generic arguments list
                genericTypeOffset += genericArgumentsCount;
            }
            else
            {
                // Simple custom types
                displayName = type.Name;
            }

            // If the type is nested, recursively format the hierarchy as well
            if (type.IsNested)
            {
                return $"{FormatDisplayString(type.DeclaringType!, genericTypeOffset, typeArguments)}.{displayName}";
            }

            return $"{type.Namespace}.{displayName}";
        }

        // Atomically get or build the display string for the current type.
        return DisplayNames.GetValue(type, t =>
        {
            // By-ref types are displayed as T&
            if (t.IsByRef)
            {
                t = t.GetElementType()!;

                return $"{FormatDisplayString(t, 0, t.GetGenericArguments())}&";
            }

            // Pointer types are displayed as T*
            if (t.IsPointer)
            {
                int depth = 0;

                // Calculate the pointer indirection level
                while (t.IsPointer)
                {
                    depth++;
                    t = t.GetElementType()!;
                }

                return $"{FormatDisplayString(t, 0, t.GetGenericArguments())}{new string('*', depth)}";
            }

            // Standard path for concrete types
            return FormatDisplayString(t, 0, t.GetGenericArguments());
        });
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;

namespace CommunityToolkit.Common.Helpers;

/// <summary>
/// A bare-bones serializer which knows how to deal with primitive types and strings only.
/// It is recommended for more complex scenarios to implement your own <see cref="IObjectSerializer"/> based on System.Text.Json, Newtonsoft.Json, or DataContractJsonSerializer see https://aka.ms/wct/storagehelper-migration
/// </summary>
public class SystemSerializer : IObjectSerializer
{
    /// <summary>
    /// Take a primitive value from storage and return it as the requested type using the <see cref="Convert.ChangeType(object, Type)"/> API.
    /// </summary>
    /// <typeparam name="T">Type to convert value to.</typeparam>
    /// <param name="value">Value from storage to convert.</param>
    /// <returns>Deserialized value or default value.</returns>
    public T Deserialize<T>(string value)
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        throw new NotSupportedException("This serializer can only handle primitive types and strings. Please implement your own IObjectSerializer for more complex scenarios.");
    }

    /// <summary>
    /// Returns the value so that it can be serialized directly.
    /// </summary>
    /// <typeparam name="T">Type to serialize from.</typeparam>
    /// <param name="value">Value to serialize.</param>
    /// <returns>String representation of value.</returns>
    public string? Serialize<T>(T value)
    {
        return value?.ToString();
    }
}

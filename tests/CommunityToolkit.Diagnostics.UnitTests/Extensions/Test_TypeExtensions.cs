// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Diagnostics.UnitTests.Extensions;

[TestClass]
public class Test_TypeExtensions
{
    [TestMethod]
    [DataRow("bool", typeof(bool))]
    [DataRow("int", typeof(int))]
    [DataRow("float", typeof(float))]
    [DataRow("double", typeof(double))]
    [DataRow("decimal", typeof(decimal))]
    [DataRow("object", typeof(object))]
    [DataRow("string", typeof(string))]
    [DataRow("void", typeof(void))]
    public void Test_TypeExtensions_BuiltInTypes(string name, Type type)
    {
        Assert.AreEqual(name, type.ToTypeString());
    }

    [TestMethod]
    [DataRow("int?", typeof(int?))]
    [DataRow("System.DateTime?", typeof(DateTime?))]
    [DataRow("(int, float)", typeof((int, float)))]
    [DataRow("(double?, string, int)?", typeof((double?, string, int)?))]
    [DataRow("int[]", typeof(int[]))]
    [DataRow("int[,]", typeof(int[,]))]
    [DataRow("System.Span<float>", typeof(Span<float>))]
    [DataRow("System.Memory<char>", typeof(Memory<char>))]
    [DataRow("System.Collections.Generic.IEnumerable<int>", typeof(IEnumerable<int>))]
    [DataRow("System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<float>>", typeof(Dictionary<int, List<float>>))]
    public void Test_TypeExtensions_GenericTypes(string name, Type type)
    {
        Assert.AreEqual(name, type.ToTypeString());
    }

    [TestMethod]
    [DataRow("System.Span<>", typeof(Span<>))]
    [DataRow("System.Collections.Generic.List<>", typeof(List<>))]
    [DataRow("System.Collections.Generic.Dictionary<,>", typeof(Dictionary<,>))]
    [DataRow("(,)", typeof(ValueTuple<,>))]
    [DataRow("(,,,,,)", typeof(ValueTuple<,,,,,>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<>.Foo<>", typeof(Animal.Rabbit<>.Foo<>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Llama<,>.Foo<>", typeof(Animal.Llama<,>.Foo<>))]
    public void Test_TypeExtensions_OpenGenericTypes(string name, Type type)
    {
        Assert.AreEqual(name, type.ToTypeString());
    }

    [TestMethod]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal", typeof(Animal))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat", typeof(Animal.Cat))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Dog", typeof(Animal.Dog))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<int?>", typeof(Animal.Rabbit<int?>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<string>", typeof(Animal.Rabbit<string>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Dog>", typeof(Animal.Rabbit<Animal.Dog>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<int>.Foo", typeof(Animal.Rabbit<int>.Foo))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<(string, int)?>.Foo", typeof(Animal.Rabbit<(string, int)?>.Foo))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<int>.Foo<string>", typeof(Animal.Rabbit<int>.Foo<string>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<int>.Foo<int[]>", typeof(Animal.Rabbit<int>.Foo<int[]>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<string[]>.Foo<object>", typeof(Animal.Rabbit<string[]>.Foo<object>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<(string, int)?>.Foo<(int, int?)>", typeof(Animal.Rabbit<(string, int)?>.Foo<(int, int?)>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Llama<float, System.DateTime>", typeof(Animal.Llama<float, DateTime>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Llama<string, (int?, object)>", typeof(Animal.Llama<string, (int?, object)>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Llama<string, (int?, object)?>.Foo", typeof(Animal.Llama<string, (int?, object)?>.Foo))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Llama<float, System.DateTime>.Foo", typeof(Animal.Llama<float, DateTime>.Foo))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Llama<string, (int?, object)?>.Foo<string>", typeof(Animal.Llama<string, (int?, object)?>.Foo<string>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Llama<float, System.DateTime>.Foo<(float?, int)?>", typeof(Animal.Llama<float, DateTime>.Foo<(float?, int)?>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Vehicle<double>", typeof(Vehicle<double>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Vehicle<int?>[]", typeof(Vehicle<int?>[]))]
    [DataRow("System.Collections.Generic.List<CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Vehicle<int>>", typeof(List<Vehicle<int>>))]
    [DataRow("System.Collections.Generic.List<CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Rabbit<int?>>", typeof(List<Animal.Rabbit<int?>>))]
    [DataRow("System.Collections.Generic.List<CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Llama<float, System.DateTime[]>>", typeof(List<Animal.Llama<float, DateTime[]>>))]
    public void Test_TypeExtensions_NestedTypes(string name, Type type)
    {
        Assert.AreEqual(name, type.ToTypeString());
    }

    [TestMethod]
    [DataRow("void*", typeof(void*))]
    [DataRow("int**", typeof(int**))]
    [DataRow("byte***", typeof(byte***))]
    [DataRow("System.Guid*", typeof(Guid*))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Foo<int>*", typeof(Foo<int>*))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat**", typeof(Animal.Cat**))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat<int>*", typeof(Animal.Cat<int>*))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat<float>.Bar**", typeof(Animal.Cat<float>.Bar**))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat<double>.Bar<int>***", typeof(Animal.Cat<double>.Bar<int>***))]
    public void Test_TypeExtensions_PointerTypes(string name, Type type)
    {
        Assert.AreEqual(name, type.ToTypeString());
    }

    [TestMethod]
    [DataRow("int&", typeof(int))]
    [DataRow("byte&", typeof(byte))]
    [DataRow("System.Guid&", typeof(Guid))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Foo<int>&", typeof(Foo<int>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat&", typeof(Animal.Cat))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat<int>&", typeof(Animal.Cat<int>))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat<float>.Bar&", typeof(Animal.Cat<float>.Bar))]
    [DataRow("CommunityToolkit.Diagnostics.UnitTests.Extensions.Test_TypeExtensions.Animal.Cat<double>.Bar<int>&", typeof(Animal.Cat<double>.Bar<int>))]
    public void Test_TypeExtensions_RefTypes(string name, Type type)
    {
        Assert.AreEqual(name, type.MakeByRefType().ToTypeString());
    }

    private class Animal
    {
        public struct Cat
        {
        }

        public struct Cat<T1>
        {
            public struct Bar
            {
            }

            public struct Bar<T2>
            {
            }
        }

        public class Dog
        {
        }

        public class Rabbit<T>
        {
            public class Foo
            {
            }

            public class Foo<T2>
            {
            }
        }

        public class Llama<T1, T2>
        {
            public class Foo
            {
            }

            public class Foo<T3>
            {
            }
        }
    }

    private class Vehicle<T>
    {
    }
}

internal struct Foo<T>
{
}

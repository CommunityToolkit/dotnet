// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Diagnostics.UnitTests;

[TestClass]
public class Test_ThrowHelper
{
    /// <summary>
    /// Default values to be used from tests in <see cref="Test_ThrowHelper_Throw"/>.
    /// </summary>
    private static readonly IReadOnlyDictionary<Type, object> DefaultValues = new Dictionary<Type, object>
    {
        [typeof(string)] = "Hello world",
        [typeof(Exception)] = new Exception("Test"),
        [typeof(object)] = new object(),
        [typeof(int)] = 42,
        [typeof(CancellationToken)] = default(CancellationToken)
    };

    [TestMethod]
    [DataRow(typeof(ArrayTypeMismatchException))]
    [DataRow(typeof(ArgumentException))]
    [DataRow(typeof(ArgumentNullException))]
    [DataRow(typeof(ArgumentOutOfRangeException))]
    [DataRow(typeof(COMException))]
    [DataRow(typeof(ExternalException))]
    [DataRow(typeof(FormatException))]
    [DataRow(typeof(InsufficientMemoryException))]
    [DataRow(typeof(InvalidDataException))]
    [DataRow(typeof(InvalidOperationException))]
    [DataRow(typeof(LockRecursionException))]
    [DataRow(typeof(MissingFieldException))]
    [DataRow(typeof(MissingMemberException))]
    [DataRow(typeof(MissingMethodException))]
    [DataRow(typeof(NotSupportedException))]
    [DataRow(typeof(ObjectDisposedException))]
    [DataRow(typeof(OperationCanceledException))]
    [DataRow(typeof(PlatformNotSupportedException))]
    [DataRow(typeof(SynchronizationLockException))]
    [DataRow(typeof(TimeoutException))]
    [DataRow(typeof(UnauthorizedAccessException))]
    [DataRow(typeof(Win32Exception))]
    public void Test_ThrowHelper_Throw(Type exceptionType)
    {
        MethodInfo[]? methods = (
            from method in typeof(ThrowHelper).GetMethods(BindingFlags.Public | BindingFlags.Static)
            where method.Name == $"Throw{exceptionType.Name}" &&
                  !method.IsGenericMethod
            select method).ToArray();

        foreach (MethodInfo? method in methods)
        {
            // Prepare the parameters with the default value
            object[]? parameters = (
                from parameter in method.GetParameters()
                select DefaultValues[parameter.ParameterType]).ToArray();

            // Invoke the throw method
            try
            {
                _ = method.Invoke(null, parameters);
            }
            catch (TargetInvocationException e)
            {
                Assert.IsInstanceOfType(e.InnerException, exceptionType);
            }
        }
    }

    [TestMethod]
    [DataRow(typeof(ArrayTypeMismatchException))]
    [DataRow(typeof(ArgumentException))]
    [DataRow(typeof(ArgumentNullException))]
    [DataRow(typeof(ArgumentOutOfRangeException))]
    [DataRow(typeof(COMException))]
    [DataRow(typeof(ExternalException))]
    [DataRow(typeof(FormatException))]
    [DataRow(typeof(InsufficientMemoryException))]
    [DataRow(typeof(InvalidDataException))]
    [DataRow(typeof(InvalidOperationException))]
    [DataRow(typeof(LockRecursionException))]
    [DataRow(typeof(MissingFieldException))]
    [DataRow(typeof(MissingMemberException))]
    [DataRow(typeof(MissingMethodException))]
    [DataRow(typeof(NotSupportedException))]
    [DataRow(typeof(ObjectDisposedException))]
    [DataRow(typeof(OperationCanceledException))]
    [DataRow(typeof(PlatformNotSupportedException))]
    [DataRow(typeof(SynchronizationLockException))]
    [DataRow(typeof(TimeoutException))]
    [DataRow(typeof(UnauthorizedAccessException))]
    [DataRow(typeof(Win32Exception))]
    public void Test_ThrowHelper_Generic_Throw(Type exceptionType)
    {
        MethodInfo[]? methods = (
            from method in typeof(ThrowHelper).GetMethods(BindingFlags.Public | BindingFlags.Static)
            where method.Name == $"Throw{exceptionType.Name}" &&
                  method.IsGenericMethod
            select method).ToArray();

        foreach (MethodInfo? method in methods)
        {
            // Prepare the parameters with the default value
            object[]? parameters = (
                from parameter in method.GetParameters()
                select DefaultValues[parameter.ParameterType]).ToArray();

            // Invoke with value type
            try
            {
                _ = method.MakeGenericMethod(typeof(int)).Invoke(null, parameters);
            }
            catch (TargetInvocationException e)
            {
                Assert.IsInstanceOfType(e.InnerException, exceptionType);
            }

            // Invoke with reference type
            try
            {
                _ = method.MakeGenericMethod(typeof(string)).Invoke(null, parameters);
            }
            catch (TargetInvocationException e)
            {
                Assert.IsInstanceOfType(e.InnerException, exceptionType);
            }
        }
    }
}

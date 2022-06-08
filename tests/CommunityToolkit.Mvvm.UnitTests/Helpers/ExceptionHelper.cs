// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Mvvm.UnitTests.Helpers;

/// <summary>
/// A helper class to validate scenarios related to <see cref="Exception"/>-s.
/// </summary>
internal static class ExceptionHelper
{
    /// <summary>
    /// Asserts that a given action throws an <see cref="ArgumentException"/> with a specific parameter name.
    /// </summary>
    /// <param name="action">The input <see cref="Action"/> to invoke.</param>
    /// <param name="parameterName">The expected parameter name.</param>
    public static void ThrowsArgumentExceptionWithParameterName(Action action, string parameterName)
    {
        bool success = false;

        try
        {
            action();
        }
        catch (Exception e)
        {
            Assert.IsTrue(e.GetType() == typeof(ArgumentException));
            Assert.AreEqual(parameterName, ((ArgumentException)e).ParamName);

            success = true;
        }

        Assert.IsTrue(success);
    }
}

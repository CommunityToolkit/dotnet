// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

public partial class Test_ArrayExtensions
{
    [TestMethod]
    public void Test_ArrayExtensions_3D_DangerousGetReference_Int()
    {
        int[,,] array = new int[10, 20, 12];

        // See comments in Test_ArrayExtensions.1D for how these tests work
        ref int r0 = ref array.DangerousGetReference();
        ref int r1 = ref array[0, 0, 0];

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_DangerousGetReference_String()
    {
        string[,,] array = new string[10, 20, 12];

        ref string r0 = ref array.DangerousGetReference();
        ref string r1 = ref array[0, 0, 0];

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_DangerousGetReferenceAt_Zero()
    {
        int[,,] array = new int[10, 20, 12];

        ref int r0 = ref array.DangerousGetReferenceAt(0, 0, 0);
        ref int r1 = ref array[0, 0, 0];

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }

    [TestMethod]
    public void Test_ArrayExtensions_3D_DangerousGetReferenceAt_Index()
    {
        int[,,] array = new int[10, 20, 12];

        ref int r0 = ref array.DangerousGetReferenceAt(5, 3, 4);
        ref int r1 = ref array[5, 3, 4];

        Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
    }
}

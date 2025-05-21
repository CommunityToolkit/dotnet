// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS0618

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

[TestClass]
public class Test_SpinLockExtensions
{
    [TestMethod]
    public unsafe void Test_ArrayExtensions_Pointer()
    {
        SpinLock spinLock = default;
        SpinLock* p = &spinLock;

        int sum = 0;

        _ = Parallel.For(0, 1000, i =>
            {
                for (int j = 0; j < 10; j++)
                {
                    using (SpinLockExtensions.Enter(p))
                    {
                        sum++;
                    }
                }
            });

        Assert.AreEqual(sum, 1000 * 10);
    }

#if NET8_0_OR_GREATER
    [TestMethod]
    public void Test_ArrayExtensions_Ref()
    {
        SpinLockOwner? spinLockOwner = new();

        int sum = 0;

        _ = Parallel.For(0, 1000, i =>
        {
            for (int j = 0; j < 10; j++)
            {
                using (spinLockOwner.Lock.Enter())
                {
                    sum++;
                }
            }
        });

        Assert.AreEqual(sum, 1000 * 10);
    }

    /// <summary>
    /// A dummy model that owns a <see cref="SpinLock"/> object.
    /// </summary>
    private sealed class SpinLockOwner
    {
        public SpinLock Lock;
    }
#endif
}

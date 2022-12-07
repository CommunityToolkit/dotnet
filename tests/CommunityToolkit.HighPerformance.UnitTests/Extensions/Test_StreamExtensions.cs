// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.HighPerformance.UnitTests.Extensions;

[TestClass]
public class Test_StreamExtensions
{
    [TestMethod]
    public void Test_StreamExtensions_ReadWrite()
    {
        // bool (1), int (4), float (4), long (8) = 17 bytes.
        // Leave two extra bytes for the partial read (fail).
        Stream stream = new byte[19].AsMemory().AsStream();

        stream.Write(true);
        stream.Write(42);
        stream.Write(3.14f);
        stream.Write(unchecked(uint.MaxValue * 324823489204ul));

        Assert.AreEqual(stream.Position, 17);

        _ = Assert.ThrowsException<ArgumentException>(() => stream.Write(long.MaxValue));

        stream.Position = 0;

        Assert.AreEqual(true, stream.Read<bool>());
        Assert.AreEqual(42, stream.Read<int>());
        Assert.AreEqual(3.14f, stream.Read<float>());
        Assert.AreEqual(unchecked(uint.MaxValue * 324823489204ul), stream.Read<ulong>());

        _ = Assert.ThrowsException<InvalidOperationException>(() => stream.Read<long>());
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/513
    [TestMethod]
    public void Test_StreamExtensions_ReadWrite_WithBufferedStream()
    {
        Stream stream = new BufferedStream();

        stream.Write(true);
        stream.Write(42);
        stream.Write(3.14f);
        stream.Write(unchecked(uint.MaxValue * 324823489204ul));

        stream.Position = 0;

        Assert.AreEqual(true, stream.Read<bool>());
        Assert.AreEqual(42, stream.Read<int>());
        Assert.AreEqual(3.14f, stream.Read<float>());
        Assert.AreEqual(unchecked(uint.MaxValue * 324823489204ul), stream.Read<ulong>());

        _ = Assert.ThrowsException<InvalidOperationException>(() => stream.Read<long>());
    }

    private sealed class BufferedStream : MemoryStream
    {
        private ReadOnlyMemory<byte> bufferedBytes;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.bufferedBytes.IsEmpty)
            {
                this.bufferedBytes = ReadMoreBytes();
            }

            int bytesToCopy = Math.Min(this.bufferedBytes.Length, count);

            this.bufferedBytes.Span.Slice(0, bytesToCopy).CopyTo(buffer.AsSpan(offset, count));
            this.bufferedBytes = this.bufferedBytes.Slice(bytesToCopy);

            return bytesToCopy;
        }

#if NET6_0_OR_GREATER
        public override int Read(Span<byte> buffer)
        {
            if (this.bufferedBytes.IsEmpty)
            {
                this.bufferedBytes = ReadMoreBytes();
            }

            int bytesToCopy = Math.Min(this.bufferedBytes.Length, buffer.Length);

            this.bufferedBytes.Span.Slice(0, bytesToCopy).CopyTo(buffer);
            this.bufferedBytes = this.bufferedBytes.Slice(bytesToCopy);

            return bytesToCopy;
        }
#endif

        private byte[] ReadMoreBytes()
        {
            byte[] array = new byte[3];
            int bytesOffset = 0;

            do
            {
                int bytesRead = base.Read(array, bytesOffset, 3 - bytesOffset);

                bytesOffset += bytesRead;

                if (bytesRead == 0)
                {
                    return array.AsSpan(0, bytesOffset).ToArray();
                }
            }
            while (bytesOffset < 3);

            return array;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CommunityToolkit.HighPerformance.UnitTests.Streams;

partial class Test_ReadOnlySequenceStream
{
    private sealed class ReadOnlySequenceSegmentOfByte : ReadOnlySequenceSegment<byte>
    {
        public ReadOnlySequenceSegmentOfByte(ReadOnlyMemory<byte> memory)
        {
            Memory = memory;
        }

        public ReadOnlySequenceSegmentOfByte Append(ReadOnlyMemory<byte> memory)
        {
            ReadOnlySequenceSegmentOfByte nextSegment = new ReadOnlySequenceSegmentOfByte(memory) { RunningIndex = RunningIndex + Memory.Length };
            Next = nextSegment;
            return nextSegment;
        }
    }
}

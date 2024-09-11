// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityToolkit.HighPerformance.UnitTests.Streams;

partial class Test_ReadOnlySequenceStream
{
    public static ReadOnlySequence<byte> CreateReadOnlySequence(params ReadOnlyMemory<byte>[] segments)
    {
        if (segments is not { Length: > 0 })
        {
            return ReadOnlySequence<byte>.Empty;
        }
        else if (segments.Length == 1)
        {
            return new(segments[0]);
        }
        else
        {
            ReadOnlySequenceSegmentOfByte first = new(segments[0]);
            ReadOnlySequenceSegmentOfByte last = first;
            long length = first.Memory.Length;

            for (int i = 1; i < segments.Length; i++)
            {
                ReadOnlyMemory<byte> segment = segments[i];

                length += segment.Length;

                last = last.Append(segment);
            }

            return new(first,0, last, (int)(length - last.RunningIndex));
        }
    }
}

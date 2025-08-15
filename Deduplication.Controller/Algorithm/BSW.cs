using Deduplication.Controller.Extensions;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deduplication.Controller.Algorithm
{
    internal class BSW : DeduplicationAlgorithm
    {
        public int D { get; set; }
        public int R { get; set; }

        public int MinT { get; set; }

        public BSW(int d, int r, int minT, Action<ProgressInfo, string> updateProgress = null) : base(updateProgress)
        {
            D = d;
            R = r;
            MinT = minT;
        }

        public override IEnumerable<Chunk> Chunk(Stream stream)
        {
            HashSet<Chunk> chunks = new HashSet<Chunk>();
            long streamLength = stream.Length;

            UpdateChunkingProgress("Start chunking", 0, streamLength);
            for (long outset = 0, boundary = 0; boundary < streamLength; boundary++)
            {
                var padding = boundary + 1;
                var scope = padding - outset;
                if (scope >= MinT || (scope > 0 && padding == streamLength))
                {
                    var piece = ReadStreamSegment(stream, (int)outset, (int)scope);
                    var f = _comparer.GetHashCode(piece);
                    if (f % D == R || padding == streamLength)
                    {
                        var chunk = new Chunk() {
                            Id = GetSHA256Str(piece),
                            Bytes = piece
                        };

                        chunks.Add(chunk);
                        UpdateChunkingProgress("Current break point", boundary);

                        outset = padding;
                    }
                }
            }
            UpdateChunkingProgress("Finished", streamLength, streamLength);

            return chunks;
        }

        private byte[] ReadStreamSegment(Stream stream, int start, int length)
        {
            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream must support seeking for this algorithm");
                
            byte[] segment = new byte[length];
            stream.Position = start;
            int bytesRead = stream.Read(segment, 0, length);
            
            if (bytesRead < length)
            {
                // Resize array if we couldn't read the full length
                Array.Resize(ref segment, bytesRead);
            }
            
            return segment;
        }
    }
}

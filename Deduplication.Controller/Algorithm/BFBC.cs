using Deduplication.Controller.Extensions;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using Deduplication.Controller.HashAlgorithm;

namespace Deduplication.Controller.Algorithm
{
    public class BFBC : DeduplicationAlgorithm
    {
        private readonly int _minT, _maxT;

        public BFBC(int minT, int maxT, Action<ProgressInfo, string> updateProgress = null) : base(updateProgress)
        {
            _minT = minT;
            _maxT = maxT;
        }

        public override IEnumerable<Chunk> Chunk(byte[] bytes)
        {
            HashSet<Chunk> chunks = new HashSet<Chunk>();

            TripleHash tripleHash = new TripleHash();
            UpdateChunkingProgress("Start chuncking", 0, bytes.Length);

            if (bytes.Length <= _minT)
            {
                string chunkID = tripleHash.ComputeTripleHash(bytes);
                var chunk = new Chunk()
                {
                    Id = chunkID,
                    Bytes = bytes
                };
                chunks.Add(chunk);
                UpdateChunkingProgress("Finished chunking", bytes.Length, bytes.Length);
            }
            else
            {
                byte[] divisors = GetDivisorsByFrequency(bytes);
                int lastP = 0, bp = 0;
                for (int i=0; i<bytes.Length; ++i)
                {
                    if (i + 2 == bytes.Length)
                    {
                        string chunkID = tripleHash.ComputeTripleHash(bytes);
                        var chunk = new Chunk()
                        {
                            Id = chunkID,
                            Bytes = bytes
                        };
                        chunks.Add(chunk);
                    }
                    else
                    {
                        if (bytes[i] == divisors[0]
                                && bytes[i + 1] == divisors[1])
                        {
                            bp = i + 2;
                            var piece = bytes.SubArray(bp, bp - lastP);
                            string chunkID = tripleHash.ComputeTripleHash(piece);
                            var chunk = new Chunk()
                            {
                                Id = chunkID,
                                Bytes = piece
                            };
                            chunks.Add(chunk);
                            lastP = bp;
                            UpdateChunkingProgress("Chunking", bp, bytes.Length);
                        }
                    }
                }
                UpdateChunkingProgress("Finished chunking", bytes.Length, bytes.Length);
            }
            return chunks;
        }

        private byte[] GetDivisorsByFrequency(byte[] bytes)
        {
            return new byte[] { 0, 1 };
        }
    }
}

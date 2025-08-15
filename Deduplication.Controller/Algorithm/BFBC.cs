using Deduplication.Controller.Extensions;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Deduplication.Controller.HashAlgorithm;
using System.IO;

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

        public override IEnumerable<Chunk> Chunk(Stream stream)
        {
            // Convert Stream to byte[]
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

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
                int lastP = 0, bp = 0, length;
                for (int i=0; i+2 <= bytes.Length; i++)
                {
                    bp = i + 2;
                    length = bp - lastP;
                    if (i + 2 == bytes.Length)
                    {
                        var piece = bytes.SubArray(lastP, length);
                        string chunkID = tripleHash.ComputeTripleHash(piece);
                        var chunk = new Chunk()
                        {
                            Id = chunkID,
                            Bytes = piece
                        };
                        chunks.Add(chunk);
                    }
                    else
                    {
                        if(length < _minT)
                        {
                            continue;
                        }
                        if ((bytes[i] == divisors[0]
                                && bytes[i + 1] == divisors[1])
                                || length >= _maxT)
                        {
                            var piece = bytes.SubArray(lastP, length);
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
            ushort index = 0;
            long[] counts = new long[UInt16.MaxValue+1];
            for (int i = 0; i + 1 < bytes.Length; i++)
            {
                index = (ushort)((bytes[i] << 8) + bytes[i + 1]);
                counts[index]++;
            }
            (int highestIndex, int secondIndex) indexes = DetermineTopTwoBytes(counts);

            IEnumerable<byte> bytePair = BitConverter.GetBytes((ushort)indexes.highestIndex).Reverse();
            return bytePair.ToArray();
        }
        private (int highestIndex, int secondHighestIndex) DetermineTopTwoBytes(long[] histogram)
        {
            (int index, long frequency) highest = (0, 0), second = (0, 0);
            for (int i = 0; i < histogram.Length; i++)
            {
                if (histogram[i] > highest.frequency)
                {
                    if (highest.index != i)
                    {
                        second.index = highest.index;
                        second.frequency = highest.frequency;
                    }
                    highest.index = i;
                    highest.frequency = histogram[i];
                }
                else if (histogram[i] > second.frequency && highest.index != i)
                {
                    second.index = i;
                    second.frequency = histogram[i];
                }
            }
            return (highest.index, second.index);
        }
    }
}

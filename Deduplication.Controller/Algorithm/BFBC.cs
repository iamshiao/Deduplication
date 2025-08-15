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
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new InvalidOperationException("Stream must be readable");
            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream must support seeking for this algorithm");

            HashSet<Chunk> chunks = new HashSet<Chunk>();
            TripleHash tripleHash = new TripleHash();
            
            // Get stream length for progress tracking
            long streamLength = stream.Length;
            UpdateChunkingProgress("Start chunking", 0, streamLength);

            if (streamLength <= _minT)
            {
                // For small streams, read all data and create single chunk
                byte[] bytes = new byte[streamLength];
                stream.Read(bytes, 0, (int)streamLength);
                string chunkID = tripleHash.ComputeTripleHash(bytes);
                var chunk = new Chunk()
                {
                    Id = chunkID,
                    Bytes = bytes
                };
                chunks.Add(chunk);
                UpdateChunkingProgress("Finished chunking", streamLength, streamLength);
            }
            else
            {
                // For larger streams, process in chunks
                byte[] divisors = GetDivisorsByFrequencyFromStream(stream);
                stream.Position = 0; // Reset position after reading for divisors
                
                int lastP = 0, bp = 0, length;
                byte[] buffer = new byte[2];
                
                for (int i = 0; i + 2 <= streamLength; i++)
                {
                    bp = i + 2;
                    length = bp - lastP;
                    
                    if (i + 2 == streamLength)
                    {
                        var piece = ReadStreamSegment(stream, lastP, length);
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
                        if (length < _minT)
                        {
                            continue;
                        }
                        
                        // Read current position bytes for comparison
                        stream.Position = i;
                        stream.Read(buffer, 0, 2);
                        
                        if ((buffer[0] == divisors[0] && buffer[1] == divisors[1]) || length >= _maxT)
                        {
                            var piece = ReadStreamSegment(stream, lastP, length);
                            string chunkID = tripleHash.ComputeTripleHash(piece);
                            var chunk = new Chunk()
                            {
                                Id = chunkID,
                                Bytes = piece
                            };
                            chunks.Add(chunk);
                            lastP = bp;
                            UpdateChunkingProgress("Chunking", bp, streamLength);
                        }
                    }
                }
                UpdateChunkingProgress("Finished chunking", streamLength, streamLength);
            }
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

        private byte[] GetDivisorsByFrequencyFromStream(Stream stream)
        {
            ushort index = 0;
            long[] counts = new long[UInt16.MaxValue + 1];
            byte[] buffer = new byte[2];
            
            for (int i = 0; i + 1 < stream.Length; i++)
            {
                stream.Position = i;
                stream.Read(buffer, 0, 2);
                index = (ushort)((buffer[0] << 8) + buffer[1]);
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

using Deduplication.Controller.Extensions;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deduplication.Controller.Algorithm
{
    internal class TTTD : DeduplicationAlgorithm
    {
        private readonly int _mainD, _secondD, _minT, _maxT;
        List<int> _breakPoints = new List<int>();

        public TTTD(int mainD, int secondD, int minT, int maxT, Action<ProgressInfo, string> updateProgress = null)
           : base(updateProgress)
        {
            _mainD = mainD;
            _secondD = secondD;

            _minT = minT;
            _maxT = maxT;
        }

        public override IEnumerable<Chunk> Chunk(Stream stream)
        {
            HashSet<Chunk> chunks = new HashSet<Chunk>();
            long streamLength = stream.Length;

            ComputeFromStream(stream);

            UpdateChunkingProgress("Start chunking", 0, streamLength);
            int lastP = 0;
            foreach (var bp in _breakPoints)
            {
                var piece = ReadStreamSegment(stream, lastP, bp - lastP);
                var f = _comparer.GetHashCode(piece);

                var chunk = new Chunk()
                {
                    Id = GetSHA256Str(piece),
                    Bytes = piece
                };
                chunks.Add(chunk);
                UpdateChunkingProgress("Current break point", bp);

                lastP = bp;
            }

            if (lastP < streamLength) // cover the last piece
            {
                var piece = ReadStreamSegment(stream, lastP, (int)(streamLength - lastP));
                var f = _comparer.GetHashCode(piece);

                var chunk = new Chunk()
                {
                    Id = GetSHA256Str(piece),
                    Bytes = piece
                };
                chunks.Add(chunk);
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

        private void AddBreakPoint(int breakPoint)
        {
            _breakPoints.Add(breakPoint);
        }

        /// <summary>
        /// Conduct the core algorithm 
        /// </summary>
        /// <param name="stream">stream of the file</param>
        private void ComputeFromStream(Stream stream)
        {
            _breakPoints = new List<int>();
            long streamLength = stream.Length;

            int currP = 0, lastP = 0, backupBreak = 0;
            for (; currP < streamLength; currP++)
            {
                if (currP - lastP < _minT)
                {
                    continue;
                }

                var piece = ReadStreamSegment(stream, lastP, currP - lastP - 1);
                var f = _comparer.GetHashCode(piece);

                if ((int)f % _secondD == _secondD - 1)
                {
                    backupBreak = currP;
                }

                if ((int)f % _mainD == _mainD - 1)
                {
                    AddBreakPoint(currP);
                    backupBreak = 0;
                    lastP = currP;
                    continue;
                }

                if (currP - lastP < _maxT)
                {
                    continue;
                }

                if (backupBreak != 0)
                {
                    AddBreakPoint(backupBreak);
                    lastP = backupBreak;
                    backupBreak = 0;
                }
                else
                {
                    AddBreakPoint(currP);
                    lastP = currP;
                    backupBreak = 0;
                }
            }
        }
    }
}

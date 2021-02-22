using Deduplication.Controller.Extensions;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;

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

        public override IEnumerable<Chunk> Chunk(byte[] bytes)
        {
            HashSet<Chunk> chunks = new HashSet<Chunk>();

            Compute(bytes);

            UpdateChunkingProgress("Start chuncking", 0, bytes.Length);
            int lastP = 0;
            foreach (var bp in _breakPoints)
            {
                var piece = bytes.SubArray(lastP, bp - lastP);
                var f = piece.GetHashCode();

                var chunk = new Chunk()
                {
                    Id = GetSHA256Str(piece),
                    Bytes = piece
                };
                chunks.Add(chunk);
                UpdateChunkingProgress("Current break point", bp);

                lastP = bp;
            }

            if (lastP < bytes.Length) // cover the last piece
            {
                var piece = bytes.SubArray(lastP, bytes.Length - lastP);
                var f = piece.GetHashCode();

                var chunk = new Chunk()
                {
                    Id = GetSHA256Str(piece),
                    Bytes = piece
                };
                chunks.Add(chunk);
            }
            UpdateChunkingProgress("Finished", bytes.Length, bytes.Length);

            return chunks;
        }
        
        private void AddBreakPoint(int breakPoint)
        {
            _breakPoints.Add(breakPoint);
        }

        /// <summary>
        /// Conduct the core algorithm 
        /// </summary>
        /// <param name="bytes">bytes of the file</param>
        private void Compute(byte[] bytes)
        {
            _breakPoints = new List<int>();

            int currP = 0, lastP = 0, backupBreak = 0;
            for (; currP < bytes.Length; currP++)
            {
                if (currP - lastP < _minT)
                {
                    continue;
                }

                var piece = bytes.SubArray(lastP, currP - lastP - 1);
                var f = piece.GetHashCode();

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

using Deduplication.Controller.Extensions;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;

namespace Deduplication.Controller.Algorithm
{
    public class OTTTD : DeduplicationAlgorithm
    {
        private readonly int _minT, _maxT, _mainD, _secondD;

        public OTTTD(int mainD, int secondD, int minT, int maxT, Action<ProgressInfo, string> updateProgress = null)
           : base(updateProgress)
        {
            if ((mainD & (mainD - 1)) != 0)
            {
                throw new Exception("mainD has to be 2^n");
            }
            if ((secondD & (secondD - 1)) != 0)
            {
                throw new Exception("secondD has to be 2^n");
            }

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

            return chunks;
        }

        List<int> _breakPoints = new List<int>();
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
            List<int> _breakPoints = new List<int>();

            int currP = 0, lastP = 0, backupBreak = 0;
            for (; currP < bytes.Length; currP++)
            {
                if (currP - lastP < _minT)
                {
                    continue;
                }

                var piece = bytes.SubArray(lastP, currP - lastP - 1);
                var f = piece.GetHashCode();

                if ((f & _secondD - 1) == _secondD - 1)
                {
                    backupBreak = currP;
                }

                if ((f & _mainD - 1) == _mainD - 1)
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

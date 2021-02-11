using Deduplication.Controller.Extension;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Deduplication.Controller.Algorithm
{
    public abstract class DeduplicationAlgorithm : IDeduplicationAlgorithm
    {
        protected ProgressInfo ProgressInfo { get; set; }
        protected Action<ProgressInfo, string> UpdateProgress { get; set; }

        public DeduplicationAlgorithm(Action<ProgressInfo, string> updateProgress = null)
        {
            ProgressInfo = new ProgressInfo();
            UpdateProgress = updateProgress;
        }

        public abstract IEnumerable<Chunk> Chunk(byte[] bytes);

        protected void UpdateChunkingProgress(string msg = null, long? processed = null, long? total = null)
        {
            if (total != null)
                ProgressInfo.Total = total.Value;
            if (processed != null)
                ProgressInfo.Processed = processed.Value;
            if (msg != null)
                ProgressInfo.Message = msg;
        }

        public void EnableProgress()
        {
            Task.Run(async () =>
            {
                using (System.Timers.Timer timer = new System.Timers.Timer())
                {
                    timer.Elapsed += new System.Timers.ElapsedEventHandler((source, e) =>
                    {
                        UpdateProgress?.Invoke(ProgressInfo, "chunks");
                    });
                    timer.Interval = 1000;
                    timer.Enabled = true;

                    while (ProgressInfo.Total != ProgressInfo.Processed)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                    }
                    UpdateProgress?.Invoke(ProgressInfo, "chunks");
                }
            });
        }

        protected int GetHashCode(byte[] bytes)
        {
            var comparer = new ArrayEqualityComparer<byte>();
            return comparer.GetHashCode(bytes);
        }

        protected string GetSHA256Str(byte[] bytes)
        {
            byte[] hashed;
            using (var provider = new SHA256CryptoServiceProvider())
            {
                hashed = provider.ComputeHash(bytes);
            }

            return BitConverter.ToString(hashed).Replace("-", "");
        }
    }
}

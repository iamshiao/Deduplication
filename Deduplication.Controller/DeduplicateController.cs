using Deduplication.Controller.Algorithm;
using Deduplication.Model.DAL;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Deduplication.Controller
{
    public class DeduplicateController
    {
        private readonly IDeduplicationAlgorithm _algorithm;
        private readonly IStorage _storage;

        private readonly ProgressInfo _fileProgress, _bytesProgress;

        private Action<ProgressInfo, string> _UpdateProgress;

        public DeduplicateController(string algorithm, IStorage storage, Action<ProgressInfo, string> UpdateProgress = null)
        {
            _storage = storage;
            _fileProgress = new ProgressInfo();
            _bytesProgress = new ProgressInfo();
            _UpdateProgress = UpdateProgress;

            IDeduplicationAlgorithm alg;
            switch (algorithm)
            {
                case "BSW":
                    alg = new BSW(540, 539, 512, UpdateProgress);
                    break;
                case "TTTD":
                    alg = new TTTD(512, 256, 460, 2800, UpdateProgress);
                    break;
                case "OTTTD":
                    alg = new OTTTD(512, 256, 460, 2800, UpdateProgress);
                    break;
                default:
                    alg = new BSW(540, 539, 512, UpdateProgress);
                    break;
            }

            _algorithm = alg;
        }
        
        public void ImportFile(FileInfo fi)
        {
            var bytes = File.ReadAllBytes(fi.FullName);

            _algorithm.EnableProgress();
            var sw = Stopwatch.StartNew();
            var chunks = _algorithm.Chunk(bytes);
            sw.Stop();
            _storage.AddChunks(chunks);
            FileViewModel fvmo = new FileViewModel()
            {
                Name = fi.Name,
                Size = fi.Length,
                Chunks = chunks,
                ProcessTime = sw.Elapsed.Duration()
            };
            _storage.AddFileViewModel(fvmo);
        }

        public void ImportFiles(IEnumerable<FileInfo> fileInfos)
        {
            long totalBytes = fileInfos.Sum(fi => fi.Length);
            ReportBytesProgress(totalBytes, 0, "Init blobs import");

            int totalBlobsCount = fileInfos.Count();
            ReportFilesProgress(totalBlobsCount, 0, "Begin blobs import");

            long processedBytes = 0;
            int processedBlobsCount = 0;
            foreach (var fi in fileInfos)
            {
                ImportFile(fi);
                processedBytes += fi.Length;
                ReportBytesProgress(totalBytes, processedBytes, "has processed bytes");

                processedBlobsCount++;
                ReportFilesProgress(totalBlobsCount, processedBlobsCount, "has processed blobs");
            }
        }

        private void ReportFilesProgress(long total, long processed, string msg)
        {
            _fileProgress.Total = total;
            _fileProgress.Processed = processed;
            _fileProgress.Message = msg;
            _UpdateProgress(_fileProgress, "files");
        }

        private void ReportBytesProgress(long total, long processed, string msg)
        {
            _bytesProgress.Total = total;
            _bytesProgress.Processed = processed;
            _bytesProgress.Message = msg;
            _UpdateProgress(_bytesProgress, "bytes");
        }
    }
}

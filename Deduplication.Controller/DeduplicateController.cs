using Deduplication.Controller.Algorithm;
using Deduplication.Model.DAL;
using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                default:
                    alg = new BSW(540, 539, 512, UpdateProgress);
                    break;
            }

            _algorithm = alg;
        }

        public void ImportFile(FileViewModel fvmi)
        {
            _algorithm.EnableProgress();
            var sw = Stopwatch.StartNew();
            var chunks = _algorithm.Chunk(fvmi.Bytes);
            sw.Stop();
            _storage.AddChunks(chunks);
            FileViewModel fvmo = new FileViewModel()
            {
                Name = fvmi.Name,
                Size = fvmi.Bytes.Length,
                Chunks = chunks,
                ProcessTime = sw.Elapsed.Duration(),
                Bytes = fvmi.Bytes
            };
            _storage.AddFileViewModel(fvmo);
        }

        public void ImportFiles(IEnumerable<FileViewModel> fvmi)
        {
            int totalBytes = fvmi.Sum(s => s.Bytes.Length);
            ReportBytesProgress(totalBytes, 0, "Init blobs import");

            int totalBlobsCount = fvmi.Count();
            ReportFilesProgress(totalBlobsCount, 0, "Begin blobs import");


            int processedBytes = 0, processedBlobsCount = 0;
            foreach (var bm in fvmi)
            {
                ImportFile(bm);
                processedBytes += bm.Bytes.Length;
                ReportBytesProgress(totalBytes, processedBytes, "has processed bytes");

                processedBlobsCount++;
                ReportFilesProgress(totalBlobsCount, processedBlobsCount, "has processed blobs");
            }
        }

        private void ReportFilesProgress(int total, int processed, string msg)
        {
            _fileProgress.Total = total;
            _fileProgress.Processed = processed;
            _fileProgress.Message = msg;
            _UpdateProgress(_fileProgress, "files");
        }

        private void ReportBytesProgress(int total, int processed, string msg)
        {
            _bytesProgress.Total = total;
            _bytesProgress.Processed = processed;
            _bytesProgress.Message = msg;
            _UpdateProgress(_bytesProgress, "bytes");
        }
    }
}

using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Deduplication.Model.DAL
{
    public class LocalStorage : IStorage
    {
        private readonly string _localStoragePath;
        private readonly List<FileViewModel> _fileViewModels;

        protected ProgressInfo ProgressInfo { get; set; }
        protected Action<ProgressInfo, string> UpdateProgress { get; set; }

        public LocalStorage(string algorithm, Action<ProgressInfo, string> updateProgress = null)
        {
            _localStoragePath = $@"{Path.GetTempPath()}/chunkstore/{algorithm}";
            Directory.CreateDirectory(_localStoragePath);
            _fileViewModels = new List<FileViewModel>();

            UpdateProgress = updateProgress;
        }

        public void AddFileViewModel(FileViewModel fileViewModel)
        {
            _fileViewModels.Add(fileViewModel);
        }

        public void AddFileViewModels(IEnumerable<FileViewModel> fileViewModels)
        {
            _fileViewModels.AddRange(fileViewModels);
        }

        public void AddChunk(Chunk chunk)
        {
            ProgressInfo = new ProgressInfo()
            {
                Message = "has processed bytes",
                Processed = ProgressInfo.Processed + chunk.Bytes.Length,
                Total = ProgressInfo.Total
            };
            if (!File.Exists($@"{_localStoragePath}\{chunk.Id}.chk"))
                File.WriteAllBytes($@"{_localStoragePath}\{chunk.Id}.chk", chunk.Bytes);
        }

        public void AddChunks(IEnumerable<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                AddChunk(chunk);
            }
        }

        public IEnumerable<FileViewModel> GetAllFileViewModels()
        {
            return _fileViewModels;
        }

        public IEnumerable<Chunk> GetAllChunks()
        {
            return null;
        }

        public IEnumerable<Chunk> GetAllChunks(string fileId)
        {
            return _fileViewModels.First(f => f.Id == fileId).Chunks;
        }

        public void Reassembly(FileViewModel fileViewModel, string outputFullPath)
        {
            List<byte> destBytes = new List<byte>();
            var chunkIds = fileViewModel.Chunks.Select(c => c.Id);

            foreach (var id in chunkIds)
            {
                var bytes = File.ReadAllBytes($@"{_localStoragePath}\{id}.chk");
                destBytes.AddRange(bytes);
            }

            File.WriteAllBytes(outputFullPath, destBytes.ToArray());
        }

        private byte[] GetBytesFromChunk(Chunk chunk)
        {
            return File.ReadAllBytes($@"{_localStoragePath}\{chunk.Id}.chk");
        }

        public void EnableProgress(ProgressInfo pi)
        {
            ProgressInfo = pi;

            Task.Run(async () =>
            {
                using (System.Timers.Timer timer = new System.Timers.Timer())
                {
                    timer.Elapsed += new System.Timers.ElapsedEventHandler((source, e) =>
                    {
                        UpdateProgress?.Invoke(ProgressInfo, "store");
                    });
                    timer.Interval = 500;
                    timer.Enabled = true;

                    while (ProgressInfo.Total != ProgressInfo.Processed)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(300));
                    }
                    UpdateProgress?.Invoke(ProgressInfo, "store");
                }
            });
        }
    }
}

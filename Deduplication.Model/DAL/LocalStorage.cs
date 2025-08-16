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
            if (ProgressInfo != null)
            {
                ProgressInfo.Message = "has processed bytes";
                ProgressInfo.Processed = ProgressInfo.Processed + chunk.Bytes.Length;
                ProgressInfo.UpdateElapsedTime();
            }
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
            Reassembly(fileViewModel, outputFullPath, null);
        }

        public void Reassembly(FileViewModel fileViewModel, string outputFullPath, Action<ProgressInfo, string> updateProgress)
        {
            var chunks = fileViewModel.Chunks.ToList();
            var totalChunks = chunks.Count;
            var progressInfo = new ProgressInfo(totalChunks, 0, "Starting reassembly");
            
            updateProgress?.Invoke(progressInfo, "reassembly");

            using (var outputStream = new FileStream(outputFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int processedChunks = 0;
                var lastUpdateTime = DateTime.Now;
                var updateInterval = TimeSpan.FromSeconds(3);
                
                foreach (var chunk in chunks)
                {
                    var chunkPath = Path.Combine(_localStoragePath, $"{chunk.Id}.chk");
                    
                    using (var inputStream = new FileStream(chunkPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                    
                    processedChunks++;
                    
                    // Update progress every 3 seconds or for the last chunk
                    if (DateTime.Now - lastUpdateTime >= updateInterval || processedChunks == totalChunks)
                    {
                        progressInfo.Total = totalChunks;
                        progressInfo.Processed = processedChunks;
                        progressInfo.Message = $"Processed chunk {processedChunks}/{totalChunks}";
                        progressInfo.UpdateElapsedTime();
                        updateProgress?.Invoke(progressInfo, "reassembly");
                        lastUpdateTime = DateTime.Now;
                    }
                }
            }

            progressInfo.Total = totalChunks;
            progressInfo.Processed = totalChunks;
            progressInfo.Message = "Reassembly completed";
            progressInfo.UpdateElapsedTime();
            updateProgress?.Invoke(progressInfo, "reassembly");
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

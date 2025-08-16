using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Deduplication.Model.DAL
{
    public class MemoryStorage : IStorage
    {
        private readonly HashSet<Chunk> _chunks;
        private readonly List<FileViewModel> _fileViewModels;

        public MemoryStorage()
        {
            _chunks = new HashSet<Chunk>();
            _fileViewModels = new List<FileViewModel>();
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
            _chunks.Add(chunk);
        }

        public void AddChunks(IEnumerable<Chunk> chunks)
        {
            _chunks.UnionWith(chunks);
        }

        public IEnumerable<FileViewModel> GetAllFileViewModels()
        {
            return _fileViewModels;
        }

        public IEnumerable<Chunk> GetAllChunks()
        {
            return _chunks;
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

            List<byte> bytes = new List<byte>();
            int processedChunks = 0;
            var lastUpdateTime = DateTime.Now;
            var updateInterval = TimeSpan.FromSeconds(3);

            foreach (var chunk in chunks)
            {
                bytes.AddRange(chunk.Bytes);
                processedChunks++;
                
                // Update progress every 3 seconds or for the last chunk
                if (DateTime.Now - lastUpdateTime >= updateInterval || processedChunks == totalChunks)
                {
                    progressInfo.Total = totalChunks;
                    progressInfo.Processed = processedChunks;
                    progressInfo.Message = $"Processing chunk {processedChunks}/{totalChunks}";
                    progressInfo.UpdateElapsedTime();
                    updateProgress?.Invoke(progressInfo, "reassembly");
                    lastUpdateTime = DateTime.Now;
                }
            }

            progressInfo.Total = totalChunks;
            progressInfo.Processed = processedChunks;
            progressInfo.Message = "Writing file to disk";
            progressInfo.UpdateElapsedTime();
            updateProgress?.Invoke(progressInfo, "reassembly");

            File.WriteAllBytes(outputFullPath, bytes.ToArray());

            progressInfo.Total = totalChunks;
            progressInfo.Processed = totalChunks;
            progressInfo.Message = "Reassembly completed";
            progressInfo.UpdateElapsedTime();
            updateProgress?.Invoke(progressInfo, "reassembly");
        }
    }
}

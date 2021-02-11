using Deduplication.Model.DTO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Deduplication.Model.DAL
{
    public class LocalStorage : IStorage
    {
        private readonly string _localStoragePath;
        private readonly List<FileViewModel> _fileViewModels;

        public LocalStorage(string algorithm)
        {
            _localStoragePath = $@"{Path.GetTempPath()}/chunkstore/{algorithm}";
            Directory.CreateDirectory(_localStoragePath);
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
    }
}

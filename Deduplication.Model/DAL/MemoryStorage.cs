using Deduplication.Model.DTO;
using System.Collections.Generic;

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
    }
}

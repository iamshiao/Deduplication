using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deduplication.Model.DAL
{
    public interface IStorage
    {
        void AddChunk(Chunk chunk);

        void AddChunks(IEnumerable<Chunk> chunks);

        IEnumerable<Chunk> GetAllChunks();

        void AddFileViewModel(FileViewModel fileViewModel);

        void AddFileViewModels(IEnumerable<FileViewModel> fileViewModels);

        IEnumerable<FileViewModel> GetAllFileViewModels();

        void Reassembly(FileViewModel fileViewModel, string outputFullPath);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deduplication.Model.DTO
{
    public class FileViewModel
    {
        public FileViewModel()
        {

        }

        public FileViewModel(string name, int size, IEnumerable<Chunk> chunks, TimeSpan processTime)
        {
            Name = name;
            Size = size;
            Chunks = chunks;
            ProcessTime = processTime;
        }

        public string Name { get; set; }
        public byte[] Bytes { get; set; }
        public int Size { get; set; }
        public IEnumerable<Chunk> Chunks { get; set; }
        public TimeSpan ProcessTime { get; set; }
    }
}

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

        public string Name { get; set; }
        public byte[] Bytes { get; set; }
        public int Size { get; set; }
        public IEnumerable<Chunk> Chunks { get; set; }
        public TimeSpan ProcessTime { get; set; }
    }
}

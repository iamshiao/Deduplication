using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deduplication.Model.DTO
{
    public class Chunk
    {
        public Chunk() { }
        public Chunk(string id, long length, long offset)
        {
            ID = id;
            Length = length;
            Offset = offset;
        }

        public string ID { get; set; }

        public long Length { get; set; }

        public long Offset { get; set; }
    }
}

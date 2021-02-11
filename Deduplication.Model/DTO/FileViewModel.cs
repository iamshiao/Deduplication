using System;
using System.Collections.Generic;

namespace Deduplication.Model.DTO
{
    public class FileViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public IEnumerable<Chunk> Chunks { get; set; }

        public TimeSpan ProcessTime { get; set; }
    }
}

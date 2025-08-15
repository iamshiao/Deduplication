using Deduplication.Model.DTO;
using System.Collections.Generic;
using System.IO;

namespace Deduplication.Controller.Algorithm
{
    public interface IDeduplicationAlgorithm
    {
        IEnumerable<Chunk> Chunk(Stream stream);

        void EnableProgress();
    }
}

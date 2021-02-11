using Deduplication.Model.DTO;
using System.Collections.Generic;

namespace Deduplication.Controller.Algorithm
{
    public interface IDeduplicationAlgorithm
    {
        IEnumerable<Chunk> Chunk(byte[] bytes);

        void EnableProgress();
    }
}

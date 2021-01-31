using Deduplication.Model.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deduplication.Controller.Algorithm
{
    public interface IDeduplicationAlgorithm
    {
        IEnumerable<Chunk> Chunk(byte[] bytes);

        void EnableProgress();
    }
}

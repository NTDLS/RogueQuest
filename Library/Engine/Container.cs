using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class Container
    {
        public Guid ContainerId { get; set; }
        public List<LevelChunk> Chunks { get; set; }

        public Container(Guid containerId)
        {
            ContainerId = containerId;
            Chunks = new List<LevelChunk>();
        }

        public Container()
        {
            Chunks = new List<LevelChunk>();
        }

        public void Add(LevelChunk chunk)
        {
            Chunks.Add(chunk);
        }

        public void Clear()
        {
            Chunks.Clear();
        }
    }
}

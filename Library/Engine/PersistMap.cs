using System.Collections.Generic;

namespace Library.Engine
{
    public class PersistMap
    {
        public string Name { get; set; }
        public List<PersistMapChunk> Chunks { get; set; }
        public GameState State { get; set; }
        public PersistMap()
        {
            Chunks = new List<PersistMapChunk>();
        }
    }
}

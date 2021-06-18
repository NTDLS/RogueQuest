using System.Collections.Generic;

namespace Library.Engine
{
    public class PersistMap
    {
        public string Name { get; set; }
        public List<PersistMapEntity> Chunks { get; set; }
        public PersistMap()
        {
            Chunks = new List<PersistMapEntity>();
        }
    }
}

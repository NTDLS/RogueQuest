using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

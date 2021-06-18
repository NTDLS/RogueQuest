using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{    public class TerrainMetadata
    {
        public bool CanWalkOn { get; set; }
        public bool CanTakeDamage { get; set; }
        public int HitPoints { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class PersistMapEntity
    {
        public string TileTypeKey { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public int DrawOrder { get; set; }
        public TerrainMetadata Meta { get; set; }
    }
}

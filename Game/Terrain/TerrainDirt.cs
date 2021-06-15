using Game.Engine;
using Library.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Terrain
{
    public class TerrainDirt : TerrainBase
    {
        public TerrainDirt(EngineCore core)
            : base(core)
        {
            this.SetImage(@"Terrain\Dirt.png");
        }
    }
}

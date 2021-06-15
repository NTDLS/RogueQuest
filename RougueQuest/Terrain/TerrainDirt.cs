using RougueQuest.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RougueQuest.Terrain
{
    public class TerrainDirt : TerrainBase
    {
        public TerrainDirt(EngineCore core)
        {
            this.SetImage(@".\..\..\..\..\Assets\Terrain\Dirt.png");
        }
    }
}

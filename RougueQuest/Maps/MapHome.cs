using RougueQuest.Engine;
using RougueQuest.Terrain;
using RougueQuest.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RougueQuest.Maps
{
    public class MapHome: MapBase
    {
        #region Public properties.

        #endregion

        public MapHome(EngineCore core)
            :base(core)
        {
            var dirtTile = new TerrainDirt(Core);


            for (int y = 100; y < core.Display.DrawingSurface.Height - 100; y += dirtTile.Size.Height)
            {
                for (int x = 100; x < core.Display.DrawingSurface.Width - 100; x += dirtTile.Size.Width)
                {
                    var dirt = core.AddNewTerrain<TerrainDirt>(x, y);
                }
            }


            Core.AddNewTextBlock("Consolas", Brushes.Black, 20, 200,150, true, "Welcome to Rougue Quest");
        }
    }
}

using Library.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Engine
{
    public class TerrainEditorTile : TerrainBase
    {
        public string TileTypeKey { get; set; }

        public TerrainEditorTile(EngineCore core, string tileTypeKey, Bitmap bitmap)
            : base(core)
        {
            TileTypeKey = tileTypeKey;
            this.SetImage(bitmap);
        }
    }
}

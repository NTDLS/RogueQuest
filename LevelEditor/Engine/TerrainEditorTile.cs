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
        public string TileNameKey { get; set; }

        public TerrainEditorTile(EngineCore core, string tileNameKey, Bitmap bitmap)
            : base(core)
        {
            TileNameKey = tileNameKey;
            this.SetImage(bitmap);
        }
    }
}

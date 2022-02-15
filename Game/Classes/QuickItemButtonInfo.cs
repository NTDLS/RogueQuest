using Library.Engine;
using System;
using System.Windows.Forms;

namespace Game.Classes
{
    public class QuickItemButtonInfo
    {
        public Guid UID { get; set; }
        public TileIdentifier Tile { get; set; }
        public ToolStripButton Button { get; set; }

    }
}

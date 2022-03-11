using Library.Engine.Types;
using Newtonsoft.Json;

namespace Library.Engine
{
    public class LevelChunk
    {
        private int? _DrawOrder = null;
        public double? _Angle { get; set; }
        public string TilePath { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double? Angle { get { return _Angle == 0 ? null : _Angle; } set { _Angle = value; } }
        public int? DrawOrder { get { return _DrawOrder == 0 ? null : _DrawOrder; } set { _DrawOrder = value; } }
        public TileMetadata Meta { get; set; } = new TileMetadata();
    }
}

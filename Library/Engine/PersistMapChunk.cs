namespace Library.Engine
{
    public class PersistMapChunk
    {
        public string TilePath { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public int DrawOrder { get; set; }
        public TileMetadata Meta { get; set; }
    }
}

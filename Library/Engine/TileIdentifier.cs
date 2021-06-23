namespace Library.Engine
{
    public class TileIdentifier
    {
        public string TilePath { get; set; }
        public TileMetadata Meta { get; set; }

        public TileIdentifier()
        {
        }

        public TileIdentifier(string tilePath)
        {
            this.TilePath = tilePath;
        }
    }
}

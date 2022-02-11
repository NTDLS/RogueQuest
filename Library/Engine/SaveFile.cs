using System.Collections.Generic;

namespace Library.Engine
{
    public class SaveFile
    {
        public List<TileIdentifier> Materials { get; set; }
        public ScenarioMetaData Meta { get; set; }
        public GameState State { get; set; }
        public List<Level> Collection { get; set; }
        public List<PersistentStore> Stores { get; set; }
    }
}

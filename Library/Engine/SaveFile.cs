using System.Collections.Generic;

namespace Library.Engine
{
    public class SaveFile
    {
        /// <summary>
        /// A list of all materials. Not just the ones used in the game, but all. This can be used for random drops and store population.
        /// </summary>
        public List<TileIdentifier> Materials { get; set; }
        public ScenarioMetaData Meta { get; set; }
        public GameState State { get; set; }
        public List<Level> Collection { get; set; }
        public List<PersistentStore> Stores { get; set; }
    }
}

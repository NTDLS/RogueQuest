using System;
using System.Collections.Generic;

namespace Library.Engine
{
    public class PersistentStore
    {
        /// <summary>
        /// The id of the tile that this store was generated for.
        /// </summary>
        public Guid StoreID { get; set; }
        /// <summary>
        /// The time in game time when the contents were generated.
        /// </summary>
        public int GameTime { get; set; }

        public List<TileIdentifier> Items = new List<TileIdentifier>();
    }
}

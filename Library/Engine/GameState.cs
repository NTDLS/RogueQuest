using System.Collections.Generic;

namespace Library.Engine
{
    public class GameState
    {
        public PlayerState Character { get; set; } = new PlayerState();
        public List<CustodyItem> Items { get; set; } = new List<CustodyItem>();

        /// <summary>
        /// The level/map-number that the player is currently on.
        /// </summary>
        public int CurrentLevel { get; set; }
        public int DefaultLevel { get; set; }
    }
}

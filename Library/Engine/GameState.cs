using System.Collections.Generic;

namespace Library.Engine
{
    public class GameState
    {
        public PlayerState Character { get; set; } = new PlayerState();
        public List<CustodyItem> Items { get; set; } = new List<CustodyItem>();
    }
}

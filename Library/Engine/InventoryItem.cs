using System;

namespace Library.Engine
{
    public class InventoryItem
    {
        public Guid ContainerId { get; set; }
        public TileIdentifier Tile { get; set; }
    }
}

using System;

namespace Library.Engine
{
    public class CustodyItem
    {
        public Guid? ContainerId { get; set; }
        public TileIdentifier Tile { get; set; }
    }
}

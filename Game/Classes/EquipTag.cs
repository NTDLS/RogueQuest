using Library.Engine;
using Library.Engine.Types;
using System.Collections.Generic;

namespace Game.Classes
{
    public class EquipTag
    {
        public TileIdentifier Tile { get; set; }
        public List<ActorSubType> AcceptTypes { get; set; } = new List<ActorSubType>();
        public EquipSlot Slot { get; set; }
    }
}

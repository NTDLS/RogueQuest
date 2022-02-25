using Library.Engine;
using System.Collections.Generic;
using static Game.Engine.Types;

namespace Game.Classes
{
    class GameLogicParam
    {
        public TickInput Input { get; set; }
        public List<ActorBase> Intersections { get; set; }
    }
}

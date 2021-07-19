using Game.Engine;
using Library.Engine;

namespace Game.Actors
{
    public class ActorWarpTarget : ActorBase
    {
        public ActorWarpTarget(EngineCore core)
            : base(core)
        {
            DoNotDraw = true;
        }
    }
}

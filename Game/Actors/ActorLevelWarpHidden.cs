using Game.Engine;
using Library.Engine;

namespace Game.Actors
{
    public class ActorLevelWarpHidden : ActorBase
    {
        public ActorLevelWarpHidden(EngineCore core)
            : base(core)
        {
            DoNotDraw = true;
        }
    }
}

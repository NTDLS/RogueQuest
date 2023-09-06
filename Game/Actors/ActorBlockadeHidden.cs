using Game.Engine;
using Library.Engine;

namespace Game.Actors
{
    public class ActorBlockadeHidden : ActorBase
    {
        public ActorBlockadeHidden(EngineCore core)
            : base(core)
        {
            DoNotDraw = true;
        }
    }
}

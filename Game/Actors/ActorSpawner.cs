using Game.Engine;
using Library.Engine;

namespace Game.Actors
{
    public class ActorSpawner : ActorBase
    {
        public ActorSpawner(EngineCore core)
            : base(core)
        {
            DoNotDraw = true;
        }
    }
}

using Game.Engine;
using Library.Engine;

namespace Game.Actors
{
    public class ActorSpawnPoint : ActorBase
    {
        public ActorSpawnPoint(EngineCore core)
            : base(core)
        {
            DoNotDraw = true;
        }
    }
}

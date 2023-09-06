using Game.Engine;
using Library.Engine;

namespace Game.Actors
{
    public class ActorHiddenMessage : ActorBase
    {
        public ActorHiddenMessage(EngineCore core)
            : base(core)
        {
            DoNotDraw = true;
        }
    }
}

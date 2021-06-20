using Game.Engine;
using Library.Engine;
using Library.Engine.Types;

namespace Game.Actors
{
    public class ActorPlayer : ActorBase
    {
        public ActorPlayer(EngineCore core)
            : base(core)
        {
            RotationMode = RotationMode.None;
            Velocity.MaxSpeed = 10;
            Velocity.ThrottlePercentage = 100;
        }
    }
}

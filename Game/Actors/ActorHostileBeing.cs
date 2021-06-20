using Game.Engine;
using Library.Engine;
using Library.Engine.Types;

namespace Game.Actors
{
    public class ActorHostileBeing : ActorBase
    {
        public int MaxFollowDistance { get; set; } = 100;

        public ActorHostileBeing(EngineCore core)
            : base(core)
        {
            RotationMode = RotationMode.None;
            Velocity.MaxSpeed = 8;
            Velocity.ThrottlePercentage = 100;
        }
    }
}

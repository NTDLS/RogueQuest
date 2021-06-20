using Game.Engine;
using Library.Engine;
using Library.Engine.Types;


namespace Game.Actors
{
    public class ActorFriendyBeing : ActorBase
    {
        public ActorFriendyBeing(EngineCore core)
            : base(core)
        {
            RotationMode = RotationMode.None;
            Velocity.MaxSpeed = 8;
            Velocity.ThrottlePercentage = 100;
        }
    }
}

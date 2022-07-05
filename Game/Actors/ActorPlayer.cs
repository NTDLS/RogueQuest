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

        public string DamageText(EngineCore core)
        {
            double damage = ((double)core.State.Character.AvailableHitpoints / (double)core.State.Character.Hitpoints) * 100.0;

            if (damage >= 100)
            {
                return "Uninjured";
            }
            else if (damage >= 80)
            {
                return "Barely Injured";
            }
            else if (damage >= 60)
            {
                return "Injured";
            }
            else if (damage >= 40)
            {
                return "Badly Injured";
            }
            else if (damage > 0)
            {
                return "Near Death";
            }

            throw new System.Exception("Entity can not have this much health!");
        }
    }
}

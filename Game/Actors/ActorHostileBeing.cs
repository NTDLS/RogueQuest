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

        public string DamageText
        {
            get
            {
                double damage = ((double)this.Meta.HitPoints / (double)this.Meta.OriginalHitPoints) * 100.0;

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

                return "Godlike"; //:D
            }
        }
    }
}

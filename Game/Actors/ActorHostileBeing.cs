using Game.Engine;
using Library.Engine;
using Library.Engine.Types;

namespace Game.Actors
{
    public class ActorHostileBeing : ActorBase
    {
        private int _maxFollowDistance = 20;

        /// <summary>
        /// Max follow distance is 20 + (Dextirity * 5).
        /// </summary>
        public int MaxFollowDistance
        {
            get
            {
                return _maxFollowDistance + ((this.Meta.Dexterity ?? 0) * 5);
            }
            set
            {
                _maxFollowDistance = value;
            }
        }

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

                throw new System.Exception("Entity can not have this much health!");
            }
        }
    }
}

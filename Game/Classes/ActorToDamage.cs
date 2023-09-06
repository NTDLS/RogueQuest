using Library.Engine;

namespace Game.Classes
{
    public class ActorToDamage
    {
        public ActorBase Actor { get; set; }
        public double DistanceFromPrimary { get; set; }
        public bool IsSplashDamage { get; set; }
        public bool IsPrimaryTarget { get; set; }
    }
}

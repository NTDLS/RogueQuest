using Library.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

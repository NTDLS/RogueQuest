
using Library.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Engine
{
    public class Types
    {
        public enum HitType
        {
            CriticalMiss,
            Miss,
            Hit,
            CriticalHit
        }

        public enum TickInputType
        {
            Movement,
            Rest,
            Get,
            Ranged,
            DialogInput
            //Maybe later we can add other input types? Magic? Arrows?
        }

        public class TickInput
        {
            public ActorBase RangedTarget { get; set; }
            public CustodyItem RangedItem { get; set; }
            public CustodyItem RangedProjectile { get; set; }
            public TickInputType InputType { get; set; }
            public double Degrees { get; set; }
            public double Throttle { get; set; }
            public Keys Key { get; set; }
        }
    }
}

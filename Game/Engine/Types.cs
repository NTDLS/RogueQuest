
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
        public enum TickInputType
        {
            Movement,
            Rest,
            Get,
            DialogInput
            //Maybe later we can add other input types? Magic? Arrows?
        }

        public class TickInput
        {
            public TickInputType InputType { get; set; }
            public double Degrees { get; set; }
            public double Throttle { get; set; }
            public Keys Key { get; set; }
        }
    }
}

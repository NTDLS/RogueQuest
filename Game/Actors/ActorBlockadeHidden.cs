using Game.Engine;
using Library.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Actors
{
    public class ActorBlockadeHidden : ActorBase
    {
        public ActorBlockadeHidden(EngineCore core)
            : base(core)
        {
            DoNotDraw = true;
        }
    }
}

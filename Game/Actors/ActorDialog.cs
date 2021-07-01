using Game.Engine;
using Library.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Actors
{
    public class ActorDialog : ActorBase
    {
        public ActorDialog(EngineCore core)
            : base(core)
        {
            DrawRealitiveToBackgroundOffset = false;
            this.Meta.ActorClass = Library.Engine.Types.ActorClassName.ActorDialog;
        }
    }
}
